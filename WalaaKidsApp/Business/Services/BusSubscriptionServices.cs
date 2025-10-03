using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using WalaaKidsApp.DataAccess;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Business.Services
{
    public static class BusSubscriptionServices
    {
        public static async Task RegisterBusSubscriptionAsync(BusSubscription busSubscription)
        {
            using (var context = new AppDbContext())
            {
                var subscription = await context.BusSubscriptions
                    .SingleOrDefaultAsync(bs => bs.StudentId == busSubscription.StudentId);
                if (subscription == null)
                {
                    context.BusSubscriptions.Add(busSubscription);
                    await context.SaveChangesAsync();
                }
                else
                {
                    subscription.SubscriptionDate = busSubscription.SubscriptionDate;
                    subscription.ExpiryDate = busSubscription.ExpiryDate;
                    await context.SaveChangesAsync();
                }
            }
        }
        public static async Task<bool> IsSubscriber(Student student)
        {
            using (var context = new AppDbContext()) {
                return await context.BusSubscriptions.AnyAsync(bs => bs.StudentId == student.Id);
            }
        }
        public static async Task<List<BusSubscription>> GetBusSubscriptionsAsync()
        {
            using (var context = new AppDbContext())
            {
                var result = await context.BusSubscriptions
                    .Include(bs => bs.Student.Classroom)
                    .ToListAsync();
                return result;
            }
        }
        public static async Task CancelSubscriptionAsync(BusSubscription busSubscription)
        {
            using (var context = new AppDbContext())
            {
                context.BusSubscriptions.Attach(busSubscription);
                context.BusSubscriptions.Remove(busSubscription);
                await context.SaveChangesAsync();
            }
        }
    }
}
