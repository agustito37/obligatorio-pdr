using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class ServerConfig
    {
        public static string ServerIPConfigKey = "ServerIpAddress";
        public static string ServerPortConfigKey = "ServerPort";
        public static string GrpcURL = "GrpcURL"; //No se si no la tienen declarada en otro lado
    }
}
