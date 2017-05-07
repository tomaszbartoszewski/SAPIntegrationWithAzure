using MainApplication.DAL;
using MainApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MainApplication.APIControllers
{
    public class StudentController : ApiController
    {
        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        private StudentRepository studentRepository = new StudentRepository();

        [HttpGet]
        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            Student student = studentRepository.Get(id);
            return Ok(new OutboundStudent
                {
                    Id = student.ID,
                    EnrollmentDate = student.EnrollmentDate,
                    LastName = student.LastName,
                    FirstMidName = student.FirstMidName
                });
        }

        // POST api/<controller>
        //[HttpPost]
        //public void Post([FromBody]CourseResult[] value)
        //{
        //}

        // PUT api/<controller>/5
        [HttpPut]
        public void Put(int id, [FromBody]CourseResult[] courseResults)
        {
            var enrollments = courseResults
                .Select(c => new Enrollment { CourseID = c.CourseId, StudentID = id, Grade = c.Grade })
                .ToArray();
            studentRepository.AddCourseResult(enrollments);

        }

        // DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                studentRepository.Dispose();
        }
    }
    
    public class OutboundStudent
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class CourseResult
    {
        public int CourseId { get; set; }

        public Grade Grade { get; set; }
    }
}