using System.IO;
using Microsoft.Azure.WebJobs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using HandlebarsDotNet;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.Azure;

namespace OutboundStudent
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessOutboundStudent([ServiceBusTrigger("domainevents", "outbound-students")] StudentChangedEvent message, TextWriter log)
        {
            log.WriteLine($"Message received, studentId: {message.StudentId}");
            var student = GetStudentInformation(message.StudentId);
            log.WriteLine($"Student: {student.FirstMidName} {student.LastName}");
            var fileContent = GenerateFileContent(student);
        }


        private static Student GetStudentInformation(int studentId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://mainapplication20170507091516.azurewebsites.net/api/student");

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync($"?id={studentId}").Result;
            if (response.IsSuccessStatusCode)
            {
                var dataObjects = response.GetBodyFromJsonAsync<Student>().Result;
                return dataObjects;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }

        private static string GenerateFileContent(Student student)
        {
            var formatCustomHelper = new FormatCustomHelper();
            var config = new HandlebarsConfiguration();
            config.Helpers.Add(formatCustomHelper.Key, formatCustomHelper.Write);

            string fileTemplate = File.ReadAllText("OutboundStudent.hbs");
            var template = Handlebars.Create(config).Compile(fileTemplate);

            var result = template(student);
            return result;
        }

        //private static void SaveFileToTheBlobStorage(string fileName, string content)
        //{
        //    var account = CloudStorageAccount.Parse(GetSetting("StorageConnectionString"));
        //    var blobClient = account.CreateCloudBlobClient();
        //    var container = blobClient.GetContainerReference("outbound");

        //}


        public static string GetSetting(string key)
        {
#if DEBUG
            var value = ConfigurationManager.AppSettings.Get(key);
#else
            var value = CloudConfigurationManager.GetSetting(key);
#endif
            if (value == null) throw new ConfigurationErrorsException($"No value configured for key: '{key}'");
            return value;
        }
    }

    public class StudentChangedEvent
    {
        public int StudentId { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
