using MainApplication.Models;
using System;
using System.Linq;

namespace MainApplication.DAL
{
    public class StudentRepository : IDisposable
    {
        private SchoolContext db = new SchoolContext();

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
        }

        public void Edit(Student student)
        {
            var studentToUpdate = db.Students.Find(student.ID);
            studentToUpdate.EnrollmentDate = student.EnrollmentDate;
            studentToUpdate.FirstMidName = student.FirstMidName;
            studentToUpdate.LastName = student.LastName;
            db.SaveChanges();
        }

        public void AddCourseResult(Enrollment[] enrollments)
        {
            foreach (var studentEnrollment in enrollments.GroupBy(e => e.StudentID))
            {
                var student = db.Students.Find(studentEnrollment.Key);
                student.Enrollments.Clear();
                foreach (var e in studentEnrollment)
                    student.Enrollments.Add(e);
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