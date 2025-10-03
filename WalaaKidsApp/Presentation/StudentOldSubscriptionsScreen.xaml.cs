using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for StudentOldSubscriptionsScreen.xaml
    /// </summary>
    public partial class StudentOldSubscriptionsScreen : UserControl, INotifyPropertyChanged
    {
        private Student _targetStudent;
        private Subscription _selectedSubscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public Subscription SelectedSubscription { get
            {
                return _selectedSubscription;
            }
            set
            {
                this._selectedSubscription = value;
                FillSubscriptionPayments();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSubscription)));
            }
        }

        private void FillSubscriptionPayments()
        {
            if (SelectedSubscription!=null)
            {
                SubscriptionPayments.Clear();
                foreach (var payment in SelectedSubscription.Payments)
                    SubscriptionPayments.Add(payment);
            }
            else
            {
                SubscriptionPayments.Clear();
            }
        }

        public ObservableCollection<Subscription> OldSubscriptions { get; set; } = new ObservableCollection<Subscription>();
        public ObservableCollection<Payment> SubscriptionPayments { get; set; } = new ObservableCollection<Payment>();
        public StudentOldSubscriptionsScreen(Student targetStudent)
        {
            InitializeComponent();
            this.DataContext = this;
            this._targetStudent = targetStudent;
        }

        private async void StudentOldSubscriptionsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await GetOldSubscriptions();
            SubscriptionPayments.CollectionChanged+=SubscriptionPayments_CollectionChanged;
        }

        private void SubscriptionPayments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TotalTextBlock.Text = SubscriptionPayments.Sum(p => p.PaymentAmount).ToString("0.00");
        }

        private async Task GetOldSubscriptions()
        {
            OldSubscriptions.Clear();
            foreach (var oldSubscription in await SubscriptionServices.GetOldSubscriptionsAsync(_targetStudent))
            {
                OldSubscriptions.Add(oldSubscription);
            }
        }
    }
}
