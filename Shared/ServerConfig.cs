﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared;

public static class ServerConfig
{
    public static string ClientIPConfigKey = "ClientIpAddress";
    public static string ServerIPConfigKey = "ServerIpAddress";
    public static string ServerPortConfigKey = "ServerPort";
    public static string LogServerURL = "LogServerURL";
    public static string GrpcAddress = "GrpcAddress";
}