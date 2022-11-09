using GrpcServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Shared;

class GrpcService {
    static readonly SettingsManager settingsManager = new SettingsManager();

    static void Main(string[] args) {
        startTCPServer();
        startGRPCServer(args);
    }

    static void startTCPServer() {
        Console.WriteLine("Iniciando TCP server...");

        string ServerIp = settingsManager.ReadSettings(ServerConfig.ServerIPConfigKey);
        int ServerPort = int.Parse(settingsManager.ReadSettings(ServerConfig.ServerPortConfigKey));
        TcpService service = new TcpService(ServerIp, ServerPort);

        TCPController controller = new TCPController(service);

        service.Start();
    }

    static void startGRPCServer(string[] args) {
        Console.WriteLine("Iniciando GRPC server...");

        var builder = WebApplication.CreateBuilder(args);

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        builder.WebHost.ConfigureKestrel(options =>
        {
            // Setup a HTTP/2 endpoint without TLS.
            options.ListenLocalhost(5202, o => o.Protocols =
                HttpProtocols.Http2);
        });

        // Add services to the container.
        builder.Services.AddGrpc();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<GreeterService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}
