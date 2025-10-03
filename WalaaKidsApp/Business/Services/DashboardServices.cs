using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using WalaaKidsApp.DataAccess;

namespace WalaaKidsApp.Business.Services
{
    public static class DashboardServices
    {
        public static int GetStudentsCount()
        {
            using (var context = new AppDbContext())
            {
                var subscribedStudents = context.Students
                    .Include(s => s.Subscriptions)
                    .Where(s => s.Subscriptions.Any(sb => sb.IsActive && sb.ExpiryDate > DateTime.UtcNow));
                return subscribedStudents.Count();
            }
        }
        public static async Task<decimal> GetCurrentMonthPayments()
        {
            using (var context = new AppDbContext())
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;

                var dateFrom = new DateTime(year, month, 1);
                var dateTo = dateFrom.AddMonths(1);

                var currentMonthPayments = context.Payments
                    .Where(p => p.PaymentDate >= dateFrom && p.PaymentDate < dateTo);
                decimal? result = await currentMonthPayments.SumAsync(p => (decimal?)p.PaymentAmount);
                return result ?? 0;
            }

        }
        public static decimal CalculateTotalAmountDue()
        {
            using (var context = new AppDbContext()) {
                decimal result = 0;
                var activeSubscriptions = context.Subscriptions
                    .Where(sb=> sb.IsActive && sb.ExpiryDate > DateTime.UtcNow);
                foreach (var subscription in activeSubscriptions)
                {
                    result += SubscriptionServices.CalculateAmountDue(subscription);
                }
            return result;
            }

        }
        public static Dictionary<string,int> GetStudentCountPerClassroom()
        {
            using (var context = new AppDbContext())
            {
                return context.Classrooms
                                      .Include(c => c.Students)
                                      .AsNoTracking()
                                      .ToDictionary(c => c.Name, c => c.Students.Count);
            }
        }
        public static SubscriptionPaymentSummary GetSubscriptionPaymentSummary()
        {
            using (var context = new AppDbContext())
            {
                var query = context.Subscriptions
                    .Where(s => s.IsActive && s.ExpiryDate > DateTime.UtcNow)
                    .Select(s => new
                    {
                        IsFullyPaid = (s.Payments.Sum(p => (decimal?)p.PaymentAmount) ?? 0) >= s.SubscriptionAmount
                    });

                var fullyPaidCount = query.Count(x => x.IsFullyPaid);
                var notFullyPaidCount = query.Count(x => !x.IsFullyPaid);

                return new SubscriptionPaymentSummary
                {
                    FullyPaidCount = fullyPaidCount,
                    NotFullyPaidCount = notFullyPaidCount
                };
            }
        }
        public class SubscriptionPaymentSummary
        {
            public int FullyPaidCount { get; set; }
            public int NotFullyPaidCount { get; set; }
        }
    }
}
