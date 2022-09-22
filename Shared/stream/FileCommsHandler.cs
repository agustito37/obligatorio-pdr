using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public class FileCommsHandler
{
    private readonly FileStreamHandler _fileStreamHandler;
    private Socket _socket;

    public FileCommsHandler(Socket socket)
    {
        _fileStreamHandler = new FileStreamHandler();
        _socket = socket;
    }

    public void SendFile(string path)
    {
        if (FileHandler.FileExists(path))
        {
            var fileName = FileHandler.GetFileName(path);
            // ---> Enviar el largo del nombre del archivo
            NetworkDataHelper.Send(_socket, Protocol.EncodeInt(fileName.Length));
            // ---> Enviar el nombre del archivo
            NetworkDataHelper.Send(_socket, Protocol.EncodeString(fileName));

            // ---> Obtener el tamaño del archivo
            long fileSize = FileHandler.GetFileSize(path);
            // ---> Enviar el tamaño del archivo
            var convertedFileSize = Protocol.EncodeLong(fileSize);
            NetworkDataHelper.Send(_socket, convertedFileSize);
            // ---> Enviar el archivo (pero con file stream)
            SendFileWithStream(fileSize, path);
        }
        else
        {
            throw new Exception("File does not exist");
        }
    }

    public string ReceiveFile()
    {
        // ---> Recibir el largo del nombre del archivo
        int fileNameSize = Protocol.DecodeInt(
            NetworkDataHelper.Receive(_socket, Protocol.FixedDataSize));
        // ---> Recibir el nombre del archivo
        string fileName = Protocol.DecodeString(NetworkDataHelper.Receive(_socket, fileNameSize));
        // ---> Recibir el largo del archivo
        long fileSize = Protocol.DecodeLong(
            NetworkDataHelper.Receive(_socket, Protocol.FixedFileSize));
        // ---> Recibir el archivo
        ReceiveFileWithStreams(fileSize, fileName);

        return fileName;
    }

    private void SendFileWithStream(long fileSize, string path)
    {
        long fileParts = Protocol.CalculateFileParts(fileSize);
        long offset = 0;
        long currentPart = 1;

        while (fileSize > offset)
        {
            byte[] data;
            if (currentPart == fileParts)
            {
                var lastPartSize = (int)(fileSize - offset);
                data = _fileStreamHandler.Read(path, offset, lastPartSize);
                offset += lastPartSize;
            }
            else
            {
                data = _fileStreamHandler.Read(path, offset, Protocol.MaxPacketSize);
                offset += Protocol.MaxPacketSize;
            }

            NetworkDataHelper.Send(_socket, data);
            currentPart++;
        }
    }

    private void ReceiveFileWithStreams(long fileSize, string fileName)
    {
        long fileParts = Protocol.CalculateFileParts(fileSize);
        long offset = 0;
        long currentPart = 1;

        while (fileSize > offset)
        {
            byte[] data;
            if (currentPart == fileParts)
            {
                var lastPartSize = (int)(fileSize - offset);
                data = NetworkDataHelper.Receive(_socket, lastPartSize);
                offset += lastPartSize;
            }
            else
            {
                data = NetworkDataHelper.Receive(_socket, Protocol.MaxPacketSize);
                offset += Protocol.MaxPacketSize;
            }
            _fileStreamHandler.Write(fileName, data);
            currentPart++;
        }
    }
}
