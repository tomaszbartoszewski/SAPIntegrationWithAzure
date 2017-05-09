using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace OutboundStudent
{
    public class GradeInbound
    {
        public static void ProcessInboundGrades([BlobTrigger("inbound/Grades_{name}.txt")] TextReader input, string name, TextWriter log)
        {
            log.WriteLine($"File received {name}");
            var grades = JsonConvert.DeserializeObject<CourseGrade[]>(input.ReadToEnd());
            SaveGrades(grades);
        }

        private static void SaveGrades(IEnumerable<CourseGrade> courseGrades)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(SettingsHelper.GetSetting("ApiUri"));

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var studentGrades in courseGrades.GroupBy(c => c.StudentId))
            {
                var toSave = studentGrades.Select(s => new CourseGradeForApi { CourseId = s.CourseId, Grade = s.Grade }).ToArray();
                var jsonObject = JsonConvert.SerializeObject(toSave);
                var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                var response = client.PutAsync($"api/student/{studentGrades.Key}", content).Result;
                    
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    throw new Exception("Something bad happened");
                }
            }
        }
    }

    public class CourseGradeForApi
    {
        public int CourseId { get; set; }
        public Grade Grade { get; set; }
    }

    public class CourseGrade
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public Grade Grade { get; set; }
    }

    public enum Grade
    {
        A, B, C, D, F
    }
}
