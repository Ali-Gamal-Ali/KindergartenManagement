using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalaaKidsApp.Business.Services;

namespace WalaaKidsApp.DataAccess.Model
{
    public class Student
    {
        public int Id { get; set; }
        public string NationalId { get; set; }
        public int? ClassroomId { get; set; } = null;
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Classroom Classroom { get; set; }
        public Guardian Guardian { get; set; }
        public BusSubscription BusSubscription { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
        public decimal AmountDue => SubscriptionServices
            .CalculateAmountDue(this.Subscriptions.SingleOrDefault(s => s.IsActive && s.ExpiryDate > DateTime.UtcNow));
        public string SubscriptionType => this.Subscriptions.SingleOrDefault(s => s.IsActive && s.ExpiryDate > DateTime.UtcNow)
            ?.SubscriptionType == 0 ? "سنوى" : "شهري";
    }
}
