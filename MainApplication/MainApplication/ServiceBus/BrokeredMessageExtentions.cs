using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MainApplication.ServiceBus
{
    public static class BrokeredMessageExtentions
    {
        public static void SendMessageAsJson<T>(this TopicClient topicClient, T message)
        {
            using (Stream stream = new MemoryStream())
                topicClient.Send(message.AsBrokeredMessage(stream));
        }

        public static async Task SendMessageAsJsonAsync<T>(this TopicClient topicClient, T message)
        {
            using (Stream stream = new MemoryStream())
                await topicClient.SendAsync(message.AsBrokeredMessage(stream));
        }

        public static BrokeredMessage AsBrokeredMessage<T>(this T message, Stream stream)
        {
            stream.WriteMessage(message);

            var brokeredMessage = new BrokeredMessage(stream) { ContentType = "application/json" };
            brokeredMessage.Properties.Add("PayloadType", message.GetType().Name);
            return brokeredMessage;
        }

        private static void WriteMessage<T>(this Stream stream, T message)
        {
            var jsonString = JsonConvert.SerializeObject(message);
            var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(jsonString);
            writer.Flush();
            stream.Position = 0;
        }
    }
}