namespace Receive.Helpers
{
    public class Util
    {
         public static string QueueWeightDecoder(string routingKey)
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