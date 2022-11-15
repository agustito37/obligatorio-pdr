using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Shared;
using Shared.domain;

namespace Logs
{
    public class QueueServer
    {
        public QueueServer(string host)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = host };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "Logs",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (sender, eventArgs) =>
                    {
                        Console.WriteLine("LOG");
                        var body = eventArgs.Body.ToArray();

                        Log message = Log.Decoder(Encoding.UTF8.GetString(body));
                        message.Date = DateTime.Now;
                        Persistence.Instance.AddLog(message);
                        Console.WriteLine(message.Date.ToString() + " - " + message.Type.ToString().ToUpper() + " - " + message.Message);
                    };

                    channel.BasicConsume(queue: "Logs", autoAck: true, consumer: consumer);

                    Console.WriteLine("Listening to messages at: {0}", host);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Was not able to RabbitMQ: {0}", host);
            }
        }
    }
}