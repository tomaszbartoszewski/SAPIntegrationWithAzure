using MainApplication.Models;
using MainApplication.ServiceBus;
using System;
using System.Linq;

namespace MainApplication.DAL
{
    public class StudentRepository : IDisposable
    {
        private SchoolContext db = new SchoolContext();
        private IServiceBusPublishService serviceBusPublisheService = new ServiceBusPublishService();
        private const string serviceBusTopic = "domainevents";

        public Student[] GetAll()
        {
            return db.Students.ToArray();
        }

        public Student Get(int? id)
        {
            return db.Students.Find(id);
        }

        public void Add(Student student)
        {
            db.Students.Add(student);
            db.SaveChanges();
            serviceBusPublisheService.Send(new StudentChangedEvent { StudentId = student.ID }, serviceBusTopic);
        }

        public void Edit(Student student)
        {
            var studentToUpdate = db.Students.Find(student.ID);
            studentToUpdate.EnrollmentDate = student.EnrollmentDate;
            studentToUpdate.FirstMidName = student.FirstMidName;
            studentToUpdate.LastName = student.LastName;
            db.SaveChanges();
            serviceBusPublisheService.Send(new StudentChangedEvent { StudentId = student.ID }, serviceBusTopic);
        }

        public void AddCourseResult(Enrollment[] enrollments)
        {
            foreach (var studentEnrollment in enrollments.GroupBy(e => e.StudentID))
            {
                var student = db.Students.Find(studentEnrollment.Key);
                var enr = student.Enrollments.ToList();
                student.Enrollments.Clear();
                foreach (var e in enr)
                {
                    db.Enrollments.Remove(e);
                }
                foreach (var e in studentEnrollment)
                {
                    db.Enrollments.Add(e);
                }
            }

            db.SaveChanges();
        }

        public void Remove(int studentId)
        {
            Student student = db.Students.Find(studentId);
            db.Students.Remove(student);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}