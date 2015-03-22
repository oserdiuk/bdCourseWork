//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Web;
//using WorkFlow.Models.DataBaseModels;

//namespace WorkFlow.Models
//{
//    public interface ICompanyRepository : IDisposable
//    {
//        IEnumerable<Company> GetCompanies();
//        Company GetCompanyByID(int companyId);
//        void InsertCompany(Company company);
//        void DeleteCompany(int companyID);
//        void UpdateCompany(Company company);
//        void Save();
//    }

//    public class CompaniesRepository : ICompanyRepository, IDisposable
//    {
//        private DBContext context;

//        public CompaniesRepository(DBContext context)
//        {
//            this.context = context;
//        }

//        public IEnumerable<Company> GetStudents()
//        {
//            return context.Students.ToList();
//        }

//        public Student GetStudentByID(int id)
//        {
//            return context.Students.Find(id);
//        }

//        public void InsertStudent(Student student)
//        {
//            context.Students.Add(student);
//        }

//        public void DeleteStudent(int studentID)
//        {
//            Student student = context.Students.Find(studentID);
//            context.Students.Remove(student);
//        }

//        public void UpdateStudent(Student student)
//        {
//            context.Entry(student).State = EntityState.Modified;
//        }

//        public void Save()
//        {
//            context.SaveChanges();
//        }

//        private bool disposed = false;

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!this.disposed)
//            {
//                if (disposing)
//                {
//                    context.Dispose();
//                }
//            }
//            this.disposed = true;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//    }
//    }
//}