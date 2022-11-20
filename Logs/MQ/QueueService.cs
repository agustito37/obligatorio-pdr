using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Shared;
using Shared.domain;
using System.Text.Json;
using System;

namespace Logs
{
    public class QueueService
    {
        public QueueService(string host)
        {
            try
            {
                // Conexión con RabbitMQ local: 
                var factory = new ConnectionFactory() { HostName = host }; 

                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                channel.QueueDeclare(queue: "Logs",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                //Defino el mecanismo de consumo
                var consumer = new EventingBasicConsumer(channel);
                //Defino el evento que sera invocado cuando llegue un mensaje 
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();

                    Log message = Log.Decoder(Encoding.UTF8.GetString(body));
                    message.Date = DateTime.Now;
                    Persistence.Instance.AddLog(message);
                    Console.WriteLine(message.Date.ToString() + " - " + message.Type.ToString().ToUpper() + " - " + message.Message);
                };

                //"PRENDO" el consumo de mensajes
                channel.BasicConsume(queue: "Logs",
                    autoAck: true,
                    consumer: consumer);
            }
            catch (Exception)
            {
                Console.WriteLine("Was not able to RabbitMQ: {0}", host);
            }
        }
    }
}