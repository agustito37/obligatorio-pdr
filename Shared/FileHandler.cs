using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public static class FileHandler
{
    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public static string GetFileName(string path)
    {
        if (FileExists(path))
        {
            return new FileInfo(path).Name;
        }

        throw new Exception("File does not exist");
    }

    public static long GetFileSize(string path)
    {
        if (FileExists(path))
        {
            return new FileInfo(path).Length;
        }

        throw new Exception("File does not exist");
    }

    public static void RemoveFile(string path)
    {
        if (FileExists(path))
        {
            File.Delete(path);
        }

        throw new Exception("File does not exist");
    }
}
