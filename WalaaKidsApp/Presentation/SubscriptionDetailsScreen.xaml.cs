using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WalaaKidsApp.Business.Enums;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess.Model;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for SubscriptionDetailsScreen.xaml
    /// </summary>
    public partial class SubscriptionDetailsScreen : UserControl, INotifyPropertyChanged, IDataErrorInfo
    {
        private Student _student;
        private DateTime _paymentDate = DateTime.Now;
        private string _paymentAmount;
        private decimal _amountDue;
        private readonly MainWindow _mainWindow;
        public DateTime PaymentDate
        {
            get
            {
                return _paymentDate;
            }
            set
            {
                _paymentDate = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentDate)));
            }
        }
        public string PaymentAmount
        {
            get
            {
                return _paymentAmount;
            }
            set
            {
                _paymentAmount = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentAmount)));
            }
        }
        public decimal AmountDue
        {
            get
            {
                return _amountDue;
            }
            set
            {
                _amountDue = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(AmountDue)));
            }
        }

        public ObservableCollection<Payment> Payments { get; set; } = new ObservableCollection<Payment>();

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                var context = new ValidationContext(this) { MemberName = columnName };
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateProperty(GetType().GetProperty(columnName).GetValue(this), context, results))
                {
                    return string.Join("\n", results.Select(r => r.ErrorMessage));
                }
                return null;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public SubscriptionDetailsScreen(Student student,MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            _student = student;
            _mainWindow = mainWindow;
            Payments.CollectionChanged +=Payments_CollectionChanged;
        }

        private void Payments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if(PaymentsList.Items.Count>0)
                PaymentsList.ScrollIntoView(PaymentsList.Items[PaymentsList.Items.Count-1]);
            });
        }

        private async Task InitializePaymentsListAsync(Student student)
        {
            var payments = await PaymentServices.GetPaymentsByStudentIdAsync(student.Id);
            Payments.Clear();
            foreach (Payment payment in payments)
            {
                Payments.Add(payment);
            }
            TotalTextBlock.Text = Payments.Sum(p => p.PaymentAmount).ToString();
        }

        private void InitializeFeildsAsync(Student student)
        {
            var subscription = student.Subscriptions
                .SingleOrDefault(sp => sp.IsActive && sp.ExpiryDate > DateTime.UtcNow);
            if (subscription != null)
            {
                SubscriptionAmountTextBlock.Text = subscription.SubscriptionAmount.ToString();
                SubscriptionDateTextBlock.Text = subscription.SubscriptionDate.Date.ToString("dd-MM-yyyy");
                ExpiryDateTextBlock.Text = subscription.ExpiryDate.ToString("dd-MM-yyyy");
                SubscriptionTypeTextBlock.Text =
                    Enum.GetName(typeof(SubscriptionType), subscription.SubscriptionType)?.ToString();
                AmountDue = SubscriptionServices.CalculateAmountDue(subscription);
            }
        }

        private async void SubscriptionDetailsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeFeildsAsync(_student);
            await InitializePaymentsListAsync(_student);
        }

        private async void RegisterPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var subscription = _student.Subscriptions.SingleOrDefault(sp => sp.IsActive && sp.ExpiryDate > DateTime.UtcNow);
                if (subscription == null)
                    throw new InvalidOperationException("حدث خطأ اثناء جلب الاشتراك");
                if (!decimal.TryParse(PaymentAmount, out var amount))
                    throw new ArgumentException("برجاء ادخال قيمة صحيحة");
                if (amount > SubscriptionServices.CalculateAmountDue(subscription))
                    throw new ArgumentException("قيمة السداد اكبر من المديونية الحالية");
                if (amount <= 0)
                    throw new ArgumentException("برجاء ادخال قيمة صحيحة");
                var payment = new Payment
                {
                    PaymentDate = this.PaymentDate,
                    PaymentAmount = amount,
                    StudentId = _student.Id,
                    SubscriptionId = subscription.Id
                };
                await PaymentServices.RegisterPaymentAsync(payment, subscription);
                await InitializePaymentsListAsync(_student);
                AmountDue = SubscriptionServices.CalculateAmountDue(subscription);
                PaymentAmount = string.Empty;
                MessageBox.Show("تم تسجيل السداد بنجاح", "عملية ناجحة"
                   , MessageBoxButton.OK
                   , MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التنفيذ\n" + App.GetFullExceptionMessage(ex),
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CancelSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("هل انت متأكد ؟", "تأكيد"
                  , MessageBoxButton.YesNo
                  , MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var subscription = _student.Subscriptions.SingleOrDefault(sp => sp.IsActive && sp.ExpiryDate > DateTime.UtcNow);
                    if (subscription == null)
                        throw new InvalidOperationException("حدث خطأ اثناء جلب الاشتراك");
                    await SubscriptionServices.CancelSubscriptionAsync(subscription);
                    MessageBox.Show("تم الغاء الاشتراك بنجاح", "عملية ناجحة"
                      , MessageBoxButton.OK
                      , MessageBoxImage.Information);
                    _mainWindow.CurrentViewControl.Content = new StudentsListScreen(_mainWindow);
                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التنفيذ\n" + App.GetFullExceptionMessage(ex),
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SubscriptionDetailsUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Payments.CollectionChanged -= Payments_CollectionChanged;
        }
    }
}
