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

        private IModel? Channel;

        public Logger()
        {
            Console.WriteLine("Conectando al servidor de logs");
            try
            {
                Instance = this;
                var factory = new ConnectionFactory() { HostName = host };
                var connection = factory.CreateConnection();
                Channel = connection.CreateModel();
                Channel.QueueDeclare(
                    queue: "Logs",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
            }
            catch (Exception)
            {
                Console.WriteLine("Error intentando conectarse a {0}", host);
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

        public void WriteInfo(string message)
        {
            this.WriteOfType(LogType.Info, message);
        }

        private void WriteOfType(LogType type, string message)
        {
            Console.WriteLine("{0}: {1}", type.ToString().ToUpper(), message);
            try {
                byte[] body = Encoding.UTF8.GetBytes(Log.Encoder(new Log() { Type = type, Message = message }));
                Channel.BasicPublish(
                    exchange: "",
                    routingKey: "Logs",
                    basicProperties: null,
                    body: body
                 );
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}