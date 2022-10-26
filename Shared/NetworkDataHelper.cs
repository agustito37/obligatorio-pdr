using System.Net.Sockets;

namespace Shared;

public class NetworkDataHelper
{
    public static void Send(TcpClient client, byte[] data)
    {
        int size = data.Length;
        NetworkStream stream = client.GetStream();
        try
        {
            stream.Write(data, 0, size);
        }
        catch (Exception)
        {
            throw new SocketException();
        }
    }

    public static byte[] Receive(TcpClient client, int limit)
    {
        byte[] data = new byte[limit];
        int offset = 0;
        NetworkStream stream = client.GetStream();
        while (offset < limit)
        {
            try
            {
                int received = stream.Read(data, offset, limit - offset);
                if (received == 0)
                {
                    throw new SocketException();
                }
                offset += received;
            }
            catch (Exception)
            {
                throw new SocketException();
            }
            
        }
        return data;
    }
}
