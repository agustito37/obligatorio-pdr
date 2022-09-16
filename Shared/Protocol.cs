using System.Collections.Generic;
using System.Text;

namespace Shared;

public class Protocol
{
    public static readonly int operationLen = 2;
    public static readonly int contentLengthLen = 4;
    public static readonly int headerLen = operationLen + contentLengthLen;
    public static readonly string listSeparator = "#";
    public enum operations: int {
        OK = 0,
        ERROR = 1,
        USER_CREATE = 2,
        PROFILE_CREATE = 3,
        PROFILE_UPDATE_PHOTO = 4,
        PROFILE_GET = 5,
        PROFILE_GET_LIST = 6,
        MESSAGE_CREATE = 7,
        MESSAGE_GET_LIST = 8
    };

    public static byte[] Encode(string message)
    {
        return Encoding.UTF8.GetBytes(message);
    }

    public static byte[] Encode(Object entity, Func<Object, string> encoder)
    {
        return Encoding.UTF8.GetBytes(encoder(entity));
    }

    public static byte[] EncodeList(List<string> messageList)
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
        return Encoding.UTF8.GetBytes(message);
    }

    public static byte[] EncodeList(List<Object> entityList, Func<Object, string> encoder)
    {
        string message = "";
        for (int i = 0; i < entityList.Count; i++) {
            message += encoder(entityList[i]);
            if (i != entityList.Count - 1) {
                message += Protocol.listSeparator;
            }
        }
        return Encoding.UTF8.GetBytes(message);
    }

    public static Object Decode(string encodedData, Func<string, Object> decoder)
    {
        return decoder(encodedData);
    }

    public static List<Object> DecodeList(string encodedData, Func<string, Object> decoder)
    {
        List<Object> entityList = new ();

        if (encodedData != "") {
            foreach (string msg in encodedData.Split(Protocol.listSeparator)) {
                entityList.Add(decoder(msg));
            }
        }

        return entityList;
    }

    public static List<string> DecodeList(string encodedData)
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
