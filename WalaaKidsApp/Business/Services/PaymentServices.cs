using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WalaaKidsApp.DataAccess;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Business.Services
{
    public static class PaymentServices
    {
        public static async Task<List<Payment>> GetPaymentsByStudentIdAsync(int studentId)
        {
            using (var context = new AppDbContext())
            {
                var result = new List<Payment>();
                var subscriptions = context.Subscriptions
                    .Where(s => s.StudentId == studentId)
                    .Include(s=>s.Payments)
                    .AsQueryable();
                var activesubscription = await subscriptions
                    .SingleOrDefaultAsync(s=>s.IsActive && s.ExpiryDate > DateTime.UtcNow);
                if (activesubscription == null)
                    throw new InvalidOperationException("الطالب لا يملك اشتراكًا نشطًا حاليًا.");
                return activesubscription.Payments.ToList();
            }
        }
        public static async Task RegisterPaymentAsync(Payment payment,Subscription subscription)
        {
            using (var context = new AppDbContext())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Payments.Add(payment);
                        context.Subscriptions.Attach(subscription);
                        await context.SaveChangesAsync();
                        dbContextTransaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        throw ex;
                    }
                }
              
            }
        }
    }
}
