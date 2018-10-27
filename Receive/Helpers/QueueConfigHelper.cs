using RabbitMQ.Client;

namespace Receive.Helpers
{
    public class QueueConfigHelper
    {
        public static IModel AddQueueToChannel(IModel channel, string queueName)
        {

            channel.QueueDeclare(queue: queueName,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

            return channel;

        }
    }
}