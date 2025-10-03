using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using WalaaKidsApp.Business.Enums;

namespace WalaaKidsApp.DataAccess.Model
{
    public class Subscription
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal SubscriptionAmount { get; set; }
        public bool IsActive { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public string LocalSubscriptionDateOnly => this.SubscriptionDate.Date.ToLocalTime().ToString("dd-MM-yyyy");
        public string LocalExpiryDateOnly => this.ExpiryDate.Date.ToLocalTime().ToString("dd-MM-yyyy");
    }
}