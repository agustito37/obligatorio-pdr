using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public class FileCommsHandler
{
    private readonly FileStreamHandler fileStreamHandler;
    private TcpClient client;

    public FileCommsHandler(TcpClient client)
    {
        this.fileStreamHandler = new FileStreamHandler();
        this.client = client;
    }

    public void SendFile(string path)
    {
        if (FileHandler.FileExists(path))
        {
            var fileName = FileHandler.GetFileName(path);
            // ---> Enviar el largo del nombre del archivo
            NetworkDataHelper.Send(client, Protocol.EncodeInt(fileName.Length));
            // ---> Enviar el nombre del archivo
            NetworkDataHelper.Send(client, Protocol.EncodeString(fileName));

            // ---> Obtener el tamaño del archivo
            long fileSize = FileHandler.GetFileSize(path);
            // ---> Enviar el tamaño del archivo
            var convertedFileSize = Protocol.EncodeLong(fileSize);
            NetworkDataHelper.Send(client, convertedFileSize);
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
            NetworkDataHelper.Receive(client, Protocol.FixedDataSize));
        // ---> Recibir el nombre del archivo
        string fileName = Protocol.DecodeString(NetworkDataHelper.Receive(client, fileNameSize));
        // ---> Recibir el largo del archivo
        long fileSize = Protocol.DecodeLong(
            NetworkDataHelper.Receive(client, Protocol.FixedFileSize));
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
                data = fileStreamHandler.Read(path, offset, lastPartSize);
                offset += lastPartSize;
            }
            else
            {
                data = fileStreamHandler.Read(path, offset, Protocol.MaxPacketSize);
                offset += Protocol.MaxPacketSize;
            }

            NetworkDataHelper.Send(client, data);
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
                data = NetworkDataHelper.Receive(client, lastPartSize);
                offset += lastPartSize;
            }
            else
            {
                data = NetworkDataHelper.Receive(client, Protocol.MaxPacketSize);
                offset += Protocol.MaxPacketSize;
            }
            fileStreamHandler.Write(fileName, data);
            currentPart++;
        }
    }
}
