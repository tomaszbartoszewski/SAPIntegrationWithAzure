using System.IO;
using Microsoft.Azure.WebJobs;

namespace OutboundStudent
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessOutboundStudent([ServiceBusTrigger("domainevents", "outbound-students")] StudentChangedEvent message, TextWriter log)
        {
            log.WriteLine(message);
        }
    }

    public class StudentChangedEvent
    {
        public int StudentId { get; set; }
    }
}
