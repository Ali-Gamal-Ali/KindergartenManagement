using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using WalaaKidsApp.DataAccess;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Business.Services
{
    public static class SubscriptionServices
    {
        public static async Task CancelSubscriptionAsync(Subscription subscription)
        {
            using (var context = new AppDbContext())
            {
                context.Subscriptions.Attach(subscription);
                subscription.IsActive = false;
                await context.SaveChangesAsync();
            }
        }
        public static decimal CalculateAmountDue(Subscription subscription)
        {
            using (var context = new AppDbContext())
            {
                var payments = context.Payments.Where(p => p.SubscriptionId == subscription.Id).ToList();
                if (payments.Count > 0)
                {
                    var result = subscription.SubscriptionAmount - payments.Sum(p => p.PaymentAmount);
                    return result;
                }
                return subscription.SubscriptionAmount;

            }
        }
        public static async Task RenewSubscriptionAsync(Subscription subscription, Payment payment,Classroom classroom)
        {
            using (var context = new AppDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Subscriptions.Add(subscription);
                        await context.SaveChangesAsync();
                        if(payment != null)
                        {
                            payment.SubscriptionId = subscription.Id;
                            payment.StudentId = subscription.StudentId;
                            context.Payments.Add(payment);
                            await context.SaveChangesAsync();
                        }
                        var student = await context.Students.FindAsync(subscription.StudentId) ?? throw new InvalidOperationException();
                        if(classroom != null)
                            student.ClassroomId = classroom.Id;
                        else
                            student.ClassroomId = null;
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public static async Task<List<Subscription>> GetOldSubscriptionsAsync(Student student)
        {
            using (var context = new AppDbContext())
            {
                var studentSubscriptions = context.Subscriptions.Where(s => s.StudentId==student.Id).AsQueryable();
                var studentOldSubscriptions = studentSubscriptions.Where(s=>!s.IsActive || s.ExpiryDate <  DateTime.UtcNow)
                    .Include(s=>s.Payments);
                return await studentOldSubscriptions.ToListAsync();
            }
        }
    }
}
