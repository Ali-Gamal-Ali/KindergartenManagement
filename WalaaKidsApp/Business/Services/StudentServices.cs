using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WalaaKidsApp.DataAccess;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Business.Services
{
    public static class StudentServices
    {
        public static async Task AddStudent(Student student, Guardian guardian, Subscription subscription, Payment payment)
        {
            using (var context = new AppDbContext())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Students.Add(student);
                        await context.SaveChangesAsync();
                        subscription.StudentId = student.Id;
                        guardian.StudentId = student.Id;
                        context.Subscriptions.Add(subscription);
                        context.Guardians.Add(guardian);
                        if (payment !=null)
                        {
                            payment.StudentId = student.Id;
                            payment.SubscriptionId = subscription.Id;
                            context.Payments.Add(payment);
                        }

                        await context.SaveChangesAsync();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public static async Task<List<Student>> GetStudentsAsync(string keyword = null, Classroom classroom = null)
        {
            using (var context = new AppDbContext())
            {
                var query = context.Students.AsNoTracking()
                .Include(s => s.Guardian)
                .Include(s => s.Classroom)
                .Include(s => s.Subscriptions)
                .Where(s => s.Subscriptions
                .Count(sub => sub.IsActive && sub.ExpiryDate > DateTime.UtcNow) == 1)
                .AsQueryable();
                if (classroom != null)
                    query = query.Where(s => s.ClassroomId == classroom.Id);
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (int.TryParse(keyword, out int id))
                        query = query.Where(s => s.Id == id);
                    else
                    {
                        var result = await query.ToListAsync();
                        return result.Where(s => s.Name.StartsWith(keyword)).ToList();
                    }

                }
                return await query.ToListAsync();
            }
        }
        public static async Task<List<Student>> GetUnSubscribedStudentAsync(string keyword = null)
        {
            using (var context = new AppDbContext())
            {
                var query = context.Students.AsNoTracking()
                .Include(s => s.Guardian)
                .Include(s => s.Subscriptions)
                .Include(s=>s.Classroom)
                .Where(s => s.Subscriptions
                .Count(sub => sub.IsActive && sub.ExpiryDate > DateTime.UtcNow) == 0)
                .AsQueryable();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (int.TryParse(keyword, out int id))
                        query = query.Where(s => s.Id == id);
                    else
                    {
                        var result = await query.ToListAsync();
                        return result.Where(s => s.Name.StartsWith(keyword)).ToList();
                    }

                }
                return await query.ToListAsync();
            }

        }
        public static async Task EditStudentAsync(Student updatedStudent)
        {
            using (var context = new AppDbContext())
            {
                var student = await context.Students
                .Include(s => s.Guardian)
                .Include(s => s.Subscriptions)
                .FirstOrDefaultAsync(s => s.Id == updatedStudent.Id);
                if (updatedStudent == null)
                    return;

                // تحديث بيانات الطالب
                student.NationalId = updatedStudent.NationalId;
                student.Name = updatedStudent.Name;
                student.ClassroomId = updatedStudent?.ClassroomId;

                // تحديث بيانات ولي الأمر
                if (student.Guardian != null && updatedStudent.Guardian != null)
                {
                    student.Guardian.FatherName = updatedStudent.Guardian.FatherName;
                    student.Guardian.MotherName = updatedStudent.Guardian.MotherName;
                    student.Guardian.PhoneNumber = updatedStudent.Guardian.PhoneNumber;
                    student.Guardian.SecondaryPhoneNumber = updatedStudent.Guardian.SecondaryPhoneNumber;
                    student.Guardian.Job = updatedStudent.Guardian.Job;
                    student.Guardian.Address = updatedStudent.Guardian.Address;
                }

                // تحديث الاشتراك النشط فقط
                var existingSub = student.Subscriptions.FirstOrDefault(s => s.IsActive && s.ExpiryDate > DateTime.UtcNow);
                var updatedSub = updatedStudent.Subscriptions.FirstOrDefault(s => s.IsActive && s.ExpiryDate > DateTime.UtcNow);

                if (existingSub != null && updatedSub != null)
                {
                    existingSub.SubscriptionAmount = updatedSub.SubscriptionAmount;
                    existingSub.SubscriptionDate = updatedSub.SubscriptionDate;
                    existingSub.ExpiryDate = updatedSub.ExpiryDate;
                    existingSub.SubscriptionType = updatedSub.SubscriptionType;
                }

                await context.SaveChangesAsync();
            }
        }
        public static async Task<bool> CheckNationalId(string nationalId)
        {
            using (var context = new AppDbContext())
            {
                return await context.Students.AnyAsync(s => s.NationalId == nationalId);
            }
        }

    }
}
