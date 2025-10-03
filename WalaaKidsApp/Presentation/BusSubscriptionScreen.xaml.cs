using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WalaaKidsApp.Business.Enums;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for BusSubscriptionScreen.xaml
    /// </summary>
    public partial class BusSubscriptionScreen : UserControl, INotifyPropertyChanged
    {
        private DateTime _subscriptionDate = DateTime.Now;
        private DateTime _subscriptionExpiryDate = new DateTime(DateTime.Now.AddYears(1).Year, 06, 30);
        private SubscriptionType _subscriptionType;
        private Student _targetStudent;
        private MainWindow _mainWindow;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime SubscriptionDate
        {
            get
            {
                return _subscriptionDate;
            }
            set
            {
                this._subscriptionDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubscriptionDate)));
            }
        }

        public DateTime SubscriptionExpiryDate
        {
            get
            {
                return _subscriptionExpiryDate;
            }
            set
            {
                this._subscriptionExpiryDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubscriptionExpiryDate)));
            }
        }

        public SubscriptionType SubscriptionType
        {
            get
            {
                return this._subscriptionType;
            }
            set
            {
                this._subscriptionType = value;
                if (value == SubscriptionType.شهري)
                    this.SubscriptionExpiryDate = DateTime.Now.AddDays(30);
                else
                    this.SubscriptionExpiryDate = new DateTime(DateTime.Now.AddYears(1).Year, 06, 30);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubscriptionType)));
            }
        }

        public BusSubscriptionScreen(Student student,MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            _targetStudent = student;
            _mainWindow = mainWindow;
        }

        private async void BusSubscriptionUserControl_Loaded(object sender, RoutedEventArgs e)
        {
             await this.Dispatcher.InvokeAsync(() =>
            {
                var sb = (Storyboard)this.Resources["FadeInStoryboard"];
                sb.Begin();
            });
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SubscriptionExpiryDate <= this.SubscriptionDate)
                    throw new ArgumentOutOfRangeException($"{nameof(this.SubscriptionExpiryDate)}",null
                        ,"تاريخ انتهاء الاشتراك غير صحيح");
                var busSubscription = new BusSubscription()
                {
                    StudentId = _targetStudent.Id,
                    SubscriptionDate = this.SubscriptionDate.ToUniversalTime(),
                    ExpiryDate = this.SubscriptionExpiryDate.ToUniversalTime()
                };
                await BusSubscriptionServices.RegisterBusSubscriptionAsync(busSubscription);
                MessageBox.Show("تم تسجيل الاشتراك بنجاح", "عملية ناجحة"
                           , MessageBoxButton.OK
                           , MessageBoxImage.Information);
                _mainWindow.CurrentViewControl.Content = new StudentsListScreen(_mainWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التنفيذ\n" + App.GetFullExceptionMessage(ex),
                               "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
