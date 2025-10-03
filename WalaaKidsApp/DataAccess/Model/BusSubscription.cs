using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalaaKidsApp.DataAccess.Model
{
    public class BusSubscription
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string LocalSubscriptionDateOnly => this.SubscriptionDate.Date.ToLocalTime().ToString("dd-MM-yyyy");
        public string LocalExpiryDateOnly => this.ExpiryDate.Date.ToLocalTime().ToString("dd-MM-yyyy");
        public bool IsValid => this.ExpiryDate > DateTime.UtcNow;

    }
}
