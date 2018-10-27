using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Receive.Helpers;

namespace Receive
{
    class Receive
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = Helpers.Constants.HOST };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: Constants.EXCHANGE,
                                   type: "direct",
                                   durable:true,
                                   autoDelete: false);

                QueueConfigHelper.AddQueueToChannel(channel, Constants.F1_QUEUE);
                QueueConfigHelper.AddQueueToChannel(channel, Constants.F2_QUEUE);
                QueueConfigHelper.AddQueueToChannel(channel, Constants.F3_QUEUE);

                if (HasArgsError(args))
                    return;

                foreach (var queueName in args)
                {
                    channel.QueueBind(queue: queueName,
                                      exchange: Constants.EXCHANGE,
                                      routingKey: queueName
                                      );
                }

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      $">> {Util.QueueWeightDecoder(routingKey)} <<", message);
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
        static bool HasArgsError(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: {0} [F1] [F2] [F3]",
                                        Environment.GetCommandLineArgs()[0]);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                Environment.ExitCode = 1;
                return true;
            }
            else
                return false;
        }
    }
}

