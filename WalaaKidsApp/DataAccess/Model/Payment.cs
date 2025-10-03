using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalaaKidsApp.DataAccess.Model
{
    public class Payment
    {
        private string _localPaymentDateOnly;
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int SubscriptionId { get; set; }
        [NotMapped]
        public string LocalPaymentDateOnly { 
            get
            {
                return this.PaymentDate.Date.ToLocalTime().ToString("dd-MM-yyyy");
            }
            set
            {
                this._localPaymentDateOnly = value;
            }
        }
    }
}
