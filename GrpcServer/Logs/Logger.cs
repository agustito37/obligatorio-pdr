using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Shared.domain;

namespace GrpcServer.Logs
{
    public class Logger
    {
        private static readonly string host = "localhost";

        public static Logger Instance { get; private set; }

        private IModel Channel;

        public Logger()
        {
            Instance = this;
            Console.WriteLine("Conectando al servidor de logs");
            try
            {
                var factory = new ConnectionFactory() { HostName = host };
                var connection = factory.CreateConnection();
                Channel = connection.CreateModel();
                Channel.QueueDeclare(queue: "Logs",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
            }
            catch (Exception)
            {
                Console.WriteLine("Error intentando conectarse a " + host);
            }
        }

        public void WriteError(string message)
        {
            this.WriteOfType(LogType.Error, message);
        }

        public void WriteWarning(string message)
        {
            this.WriteOfType(LogType.Warning, message);
        }

        public void WriteMessage(string message)
        {
            this.WriteOfType(LogType.Message, message);
        }

        private void WriteOfType(LogType type, string message)
        {
            var body = Encoding.UTF8.GetBytes(Log.Encoder(new Log() { Type = type, Message = message }));
            Channel.BasicPublish(exchange: "",
                            routingKey: "Logs",
                            basicProperties: null,
                            body: body);
        }
    }
}