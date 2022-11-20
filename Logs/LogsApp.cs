using Logs;
using Shared;

class LogsApp
{
    static async Task Main(string[] args)
    {
        SettingsManager settingsManager = new SettingsManager();

        new QueueService(settingsManager.ReadSettings(ServerConfig.LogServerURL));

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}
