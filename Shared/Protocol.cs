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
    public const int ProfileGetPhoto = 10;
}

public class Protocol
{
    public static readonly int FixedDataSize = 4;
    public static readonly int FixedFileSize = 8;
    public static readonly int MaxPacketSize = 32768;
    public static readonly int OperationLen = 2;
    public static readonly int ContentLengthLen = 4;
    public static readonly int HeaderLen = OperationLen + ContentLengthLen;
    public static readonly string ListSeparator = "<#>";
    public static readonly string ParamSeparator = "<=>";

    public static long CalculateFileParts(long fileSize)
    {
        var fileParts = fileSize / MaxPacketSize;
        return fileParts * MaxPacketSize == fileSize ? fileParts : fileParts + 1;
    }

    public static byte[] EncodeInt(int value)
    {
        return BitConverter.GetBytes(value);
    }

    public static int DecodeInt(byte[] value)
    {
        return BitConverter.ToInt32(value);
    }

    public static byte[] EncodeLong(long value)
    {
        return BitConverter.GetBytes(value);
    }

    public static long DecodeLong(byte[] value)
    {
        return BitConverter.ToInt64(value);
    }

    public static byte[] EncodeString(string toEncode)
    {
        return Encoding.UTF8.GetBytes(toEncode);
    }

    public static byte[] EncodeString(int toEncode)
    {
        return Encoding.UTF8.GetBytes(toEncode.ToString());
    }

    public static string DecodeString(byte[] toDecode)
    {
        return Encoding.UTF8.GetString(toDecode);
    }

    public static byte[] EncodeStringList(List<string> list)
    {
        string encoded = "";
        for (int i = 0; i < list.Count; i++)
        {
            encoded += list[i];
            if (i != list.Count - 1)
            {
                encoded += Protocol.ListSeparator;
            }
        }
        return EncodeString(encoded);
    }

    public static List<string> DecodeStringList(string encodedData)
    {
        List<string> entityList = new();

        if (encodedData != "")
        {
            foreach (string msg in encodedData.Split(Protocol.ListSeparator))
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
        string encoded = "";
        for (int i = 0; i < entityList.Count; i++) {
            encoded += encoder(entityList[i]);
            if (i != entityList.Count - 1) {
                encoded += Protocol.ListSeparator;
            }
        }
        return EncodeString(encoded);
    }

    public static T Decode<T>(string encodedData, Func<string, T> decoder)
    {
        return decoder(encodedData);
    }

    public static List<T> DecodeList<T>(string encodedData, Func<string, T> decoder)
    {
        List<T> entityList = new ();

        if (encodedData != "") {
            foreach (string msg in encodedData.Split(Protocol.ListSeparator)) {
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
        byte[] operationBytes = FillBytes(BitConverter.GetBytes(operation), OperationLen);
        byte[] lengthBytes = FillBytes(BitConverter.GetBytes(content.Length), ContentLengthLen);

        return operationBytes.Concat(lengthBytes).ToArray();
    }

    public static (int operation, int contentLen) DecodeHeader(byte[] bytes)
    {
        byte[] operationBytes = bytes.Take(OperationLen).ToArray();
        byte[] contentLengthBytes = bytes.Skip(OperationLen).Take(ContentLengthLen).ToArray();
        int operation = BitConverter.ToInt16(operationBytes);
        int contentLength = BitConverter.ToInt32(contentLengthBytes);

        return (operation, contentLength);
    }

    public static string createParam(string key, string value)
    {
        return key + ParamSeparator + value;
    }

    public static (string key, string value) getParam(string param)
    {
        string[] search = param.Split(ParamSeparator);
        return (search[0], search[1]);
    }
}
