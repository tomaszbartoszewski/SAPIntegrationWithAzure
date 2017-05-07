using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using System.Threading.Tasks;

namespace MainApplication.ServiceBus
{
    public interface IServiceBusPublishService
    {
        void Send(object message, string topicName);
        Task SendAsync(object message, string topicName);
    }

    public class ServiceBusPublishService : IServiceBusPublishService
    {
        public void Send(object message, string topicName)
        {
            try
            {
                var topicClient = GetTopicClient(topicName);
                topicClient.SendMessageAsJson(message);
            }
            catch
            {
            }
        }

        public async Task SendAsync(object message, string topicName)
        {
            try
            {
                var topicClient = GetTopicClient(topicName);
                await topicClient.SendMessageAsJsonAsync(message);
            }
            catch
            {
            }
        }

        private TopicClient GetTopicClient(string topicName)
        {
            return TopicClient.CreateFromConnectionString(ConfigurationManager.AppSettings["ServiceBusConnectionString"], topicName);
        }
    }
}