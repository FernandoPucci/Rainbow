using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receive
{
    class Receive
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "REPORT_PRIORITY_EXCHANGE",
                                   type: "direct");

                // var queueName = channel.QueueDeclare().QueueName;
                channel.QueueDeclare(queue: "F1",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                channel.QueueDeclare(queue: "F2",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                channel.QueueDeclare(queue: "F3",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

                if (args.Length < 1)
                {
                    Console.Error.WriteLine("Usage: {0} [F1] [F2] [F3]",
                                            Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }

                foreach (var weightProcessing in args)
                {
                    channel.QueueBind(queue: weightProcessing,
                                      exchange: "REPORT_PRIORITY_EXCHANGE",
                                      routingKey: weightProcessing);
                }

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      $">> {QueueWeightDecoder(routingKey)} <<", message);
                };

                foreach (var weightProcessing in args)
                {
                    channel.BasicConsume(queue: weightProcessing,
                                         autoAck: true,
                                         consumer: consumer);
                }
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        static string QueueWeightDecoder(string routingKey)
        {
            switch (routingKey.ToUpperInvariant())
            {
                case "F1":
                    return "HIGH RESOURCE CONSUMPTION QUEUE - SLOW PROCESSING";
                case "F3":
                    return "LOW RESOURCE CONSUMPTION QUEUE - FAST PROCESSING";
                default:
                    return "F2 QUEUE - ORDINARY PROCESSING";
            }
        }
    }
}

