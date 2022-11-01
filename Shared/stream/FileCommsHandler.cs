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

    public async Task SendFile(string path)
    {
        if (FileHandler.FileExists(path))
        {
            var fileName = FileHandler.GetFileName(path);
            // ---> Enviar el largo del nombre del archivo
            await NetworkDataHelper.Send(client, Protocol.EncodeInt(fileName.Length));
            // ---> Enviar el nombre del archivo
            await NetworkDataHelper.Send(client, Protocol.EncodeString(fileName));

            // ---> Obtener el tamaño del archivo
            long fileSize = FileHandler.GetFileSize(path);
            // ---> Enviar el tamaño del archivo
            var convertedFileSize = Protocol.EncodeLong(fileSize);
            await NetworkDataHelper.Send(client, convertedFileSize);
            // ---> Enviar el archivo (pero con file stream)
            await SendFileWithStream(fileSize, path);
        }
        else
        {
            throw new Exception("File does not exist");
        }
    }

    public async Task<string> ReceiveFile()
    {
        // ---> Recibir el largo del nombre del archivo
        int fileNameSize = Protocol.DecodeInt(
            await NetworkDataHelper.Receive(client, Protocol.FixedDataSize)
        );
        // ---> Recibir el nombre del archivo
        string fileName = Protocol.DecodeString(
            await NetworkDataHelper.Receive(client, fileNameSize)
        );
        // ---> Recibir el largo del archivo
        long fileSize = Protocol.DecodeLong(
            await NetworkDataHelper.Receive(client, Protocol.FixedFileSize)
        );
        // ---> Recibir el archivo
        await ReceiveFileWithStreams(fileSize, fileName);

        return fileName;
    }

    private async Task SendFileWithStream(long fileSize, string path)
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
                data = await fileStreamHandler.Read(path, offset, lastPartSize);
                offset += lastPartSize;
            }
            else
            {
                data = await fileStreamHandler.Read(path, offset, Protocol.MaxPacketSize);
                offset += Protocol.MaxPacketSize;
            }

            await NetworkDataHelper.Send(client, data);
            currentPart++;
        }
    }

    private async Task ReceiveFileWithStreams(long fileSize, string fileName)
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
                data = await NetworkDataHelper.Receive(client, lastPartSize);
                offset += lastPartSize;
            }
            else
            {
                data = await NetworkDataHelper.Receive(client, Protocol.MaxPacketSize);
                offset += Protocol.MaxPacketSize;
            }
            await fileStreamHandler.Write(fileName, data);
            currentPart++;
        }
    }
}
