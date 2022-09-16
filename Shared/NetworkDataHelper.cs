using System.Net.Sockets;

namespace Shared;

public class NetworkDataHelper
{
    public static void Send(Socket socket, byte[] data)
    {
        int offset = 0;
        int size = data.Length;
        while (offset < size) 
        {
            int sent = socket.Send(data, offset, size - offset, SocketFlags.None);
            if (sent == 0)
            {
                throw new SocketException();
            }
            offset += sent; 
        }
    }

    public static byte[] Receive(Socket socket, int limit)
    {
        byte[] data = new byte[limit];
        int offset = 0;
        while (offset < limit) 
        {
            int received = socket.Receive(data, offset, limit - offset, SocketFlags.None);
            if (received == 0) 
            {
                throw new SocketException();
            }
            offset += received; 
        }
        return data;
    }
}
