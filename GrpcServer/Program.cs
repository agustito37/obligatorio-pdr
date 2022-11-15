using GrpcServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Shared;
using GrpcServer.Logs;

internal class GrpcService
{
    private static readonly SettingsManager settingsManager = new SettingsManager();

    private static void Main(string[] args)
    {
        string message = "Starting server";
        Logger.Instance.WriteMessage(message);

        startTCPServer();
        startGRPCServer(args);
    }

    private static void startTCPServer()
    {
        string message = "Iniciando TCP server";
        Logger.Instance.WriteMessage(message);

        string ServerIp = settingsManager.ReadSettings(ServerConfig.ServerIPConfigKey);
        int ServerPort = int.Parse(settingsManager.ReadSettings(ServerConfig.ServerPortConfigKey));
        TcpService service = new TcpService(ServerIp, ServerPort);

        TcpController controller = new TcpController(service);

        service.Start();
    }

    private static void startGRPCServer(string[] args)
    {
        string message = "Iniciando GRPC server";
        Logger.Instance.WriteMessage(message);

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
        app.MapGrpcService<ProfilesService>();
        app.MapGrpcService<UsersService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}