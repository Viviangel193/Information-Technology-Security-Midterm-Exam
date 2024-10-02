using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Data
{
    public class StudentData : IStudent
    {
        public readonly ApplicationDbContext _db;
        public StudentData(ApplicationDbContext db)
        {
            _db = db;
        }
        public Student AddStudent(Student student)
        {
            try
            {
                _db.Students.Add(student);
                _db.SaveChanges();
                return student;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public void DeleteStudent(string NIM)
        {
            throw new NotImplementedException();
        }

        public Student UpdateStudent(Student student)
        {
            throw new NotImplementedException();
        }
        public Student GetStudent(string NIM)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<Student> GetStudents()
        {
            var student = _db.Students.OrderBy(s => s.FullName);
            return student;
        }
    }
}