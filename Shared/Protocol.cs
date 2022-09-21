using System.Collections.Generic;
using System.Text;

namespace Shared;

public class Operations {
    public const int Ok = 0;
    public const int Error = 1;
    public const int UserCreate = 2;
    public const int ProfileCreate = 3;
    public const int ProfileUpdatePhoto = 4;
    public const int ProfileGet = 5;
    public const int ProfileGetList = 6;
    public const int MessageCreate = 7;
    public const int MessageGetList = 9;
}

public class Protocol
{
    public static readonly int operationLen = 2;
    public static readonly int contentLengthLen = 4;
    public static readonly int headerLen = operationLen + contentLengthLen;
    public static readonly string listSeparator = "#";

    public static byte[] EncodeString(string toEncode)
    {
        return Encoding.UTF8.GetBytes(toEncode);
    }

    public static string DecodeBytes(byte[] toDecode)
    {
        return Encoding.UTF8.GetString(toDecode);
    }

    public static byte[] EncodeStringList(List<string> messageList)
    {
        string message = "";
        for (int i = 0; i < messageList.Count; i++)
        {
            message += messageList[i];
            if (i != messageList.Count - 1)
            {
                message += Protocol.listSeparator;
            }
        }
        return EncodeString(message);
    }

    public static List<string> DecodeStringList(string encodedData)
    {
        List<string> entityList = new();

        if (encodedData != "")
        {
            foreach (string msg in encodedData.Split(Protocol.listSeparator))
            {
                entityList.Add(msg);
            }
        }

        return entityList;
    }

    public static byte[] Encode<T>(T entity, Func<T, string> encoder)
    {
        return EncodeString(encoder(entity));
    }

    public static byte[] EncodeList<T>(List<T> entityList, Func<T, string> encoder)
    {
        string message = "";
        for (int i = 0; i < entityList.Count; i++) {
            message += encoder(entityList[i]);
            if (i != entityList.Count - 1) {
                message += Protocol.listSeparator;
            }
        }
        return EncodeString(message);
    }

    public static T Decode<T>(string encodedData, Func<string, T> decoder)
    {
        return decoder(encodedData);
    }

    public static List<T> DecodeList<T>(string encodedData, Func<string, T> decoder)
    {
        List<T> entityList = new ();

        if (encodedData != "") {
            foreach (string msg in encodedData.Split(Protocol.listSeparator)) {
                entityList.Add(decoder(msg));
            }
        }

        return entityList;
    }

    private static byte[] FillBytes(byte[] data, int len) {
        byte[] bytes = new byte[len];
        for (int x = 0; x < len; x++) {
            bytes[x] = data[x];
        }
        return bytes;
    }

    public static byte[] EncodeHeader(int operation, byte[] content) {
        byte[] operationBytes = FillBytes(BitConverter.GetBytes(operation), operationLen);
        byte[] lengthBytes = FillBytes(BitConverter.GetBytes(content.Length), contentLengthLen);

        return operationBytes.Concat(lengthBytes).ToArray();
    }

    public static (int operation, int contentLen) DecodeHeader(byte[] bytes)
    {
        byte[] operationBytes = bytes.Take(operationLen).ToArray();
        byte[] contentLengthBytes = bytes.Skip(operationLen).Take(contentLengthLen).ToArray();
        int operation = BitConverter.ToInt16(operationBytes);
        int contentLength = BitConverter.ToInt32(contentLengthBytes);

        return (operation, contentLength);
    }
}
