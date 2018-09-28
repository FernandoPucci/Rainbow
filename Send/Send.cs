using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace Send
{
    /// <summary>
    /// Class containing methods to send data to a queue
    /// </summary>    
    class Send
    {

        /// <summary>
        /// Send Message by route exchange
        /// </summary>
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "REPORT_PRIORITY_EXCHANGE",
                                                   type: "direct");

                var weightProcessing = (args.Length > 0) ? args[0] : "F1";

                var message = (args.Length > 1)
                              ? string.Join(" ", args.Skip(1).ToArray())
                              : "Hello Enqueue System!!";
                var body = Encoding.UTF8.GetBytes($"{message} at {DateTime.Now.ToString()}");
                channel.BasicPublish(exchange: "REPORT_PRIORITY_EXCHANGE",
                                     routingKey: weightProcessing,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", weightProcessing, message);

            }

            // Console.WriteLine(" Press [enter] to exit.");
            // Console.ReadLine();
        }
    }
}

