namespace Shared;

public class FileStreamHandler
{
    public byte[] Read(string path, long offset, int length)
    {
        if (FileHandler.FileExists(path))
        {
            var data = new byte[length];

            using var fs = new FileStream(path, FileMode.Open) { Position = offset };
            var bytesRead = 0;
            while (bytesRead < length)
            {
                var read = fs.Read(data, bytesRead, length - bytesRead);
                if (read == 0)
                    throw new Exception("Error reading file");
                bytesRead += read;
            }

            return data;
        }

        throw new Exception("File does not exist");
    }

    public void Write(string fileName, byte[] data)
    {
        var fileMode = FileHandler.FileExists(fileName) ? FileMode.Append : FileMode.Create;
        using var fs = new FileStream(fileName, fileMode);
        fs.Write(data, 0, data.Length);
    }
}
