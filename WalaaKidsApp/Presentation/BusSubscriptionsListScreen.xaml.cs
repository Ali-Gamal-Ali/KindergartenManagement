using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for BusSubscriptionsListScreen.xaml
    /// </summary>
    public partial class BusSubscriptionsListScreen : UserControl, INotifyPropertyChanged
    {
        private MainWindow _mainWindow;
        private BusSubscription _selectedSubscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public BusSubscription SelectedSubscription
        {
            get
            {
                return _selectedSubscription;
            }
            set
            {
                this._selectedSubscription = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSubscription)));
            }
        }
        public ObservableCollection<BusSubscription> BusSubscriptions { get; set; } = new ObservableCollection<BusSubscription>();
        public BusSubscriptionsListScreen(MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            _mainWindow = mainWindow;
        }

        private async void BusSubscriptionListUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeSubscriptionsList();
            await this.Dispatcher.InvokeAsync(() =>
            {
                var sb = (Storyboard)this.Resources["FadeInStoryboard"];
                sb.Begin();
            });
        }

        private async Task InitializeSubscriptionsList()
        {
            BusSubscriptions.Clear();
            foreach (var busSubscription in await BusSubscriptionServices.GetBusSubscriptionsAsync())
            {
                BusSubscriptions.Add(busSubscription);
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var list = await BusSubscriptionServices.GetBusSubscriptionsAsync();
            if (int.TryParse(KeywordTextBox.Text, out int parsingResult))
            {
                var result = list.Where(bs => bs.Student.Id == parsingResult).ToList();
                this.BusSubscriptions.Clear();
                foreach (var busSubscription in result)
                    BusSubscriptions.Add(busSubscription);
            }
            else
            {
                var result = list.Where(bs => bs.Student.Name.StartsWith(KeywordTextBox.Text)).ToList();
                this.BusSubscriptions.Clear();
                foreach (var busSubscription in result)
                    BusSubscriptions.Add(busSubscription);
            }
        }

        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSubscription == null)
            {
                MessageBox.Show("قم باختيار اشتراك اولا", "تنبيه",
                    MessageBoxButton.OK
                    , MessageBoxImage.Exclamation);
                return;
            }
            _mainWindow.CurrentViewControl.Content = new BusSubscriptionScreen(SelectedSubscription.Student, _mainWindow);
        }

        private async void CancelSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSubscription == null)
                {
                    MessageBox.Show("قم باختيار اشتراك اولا", "تنبيه",
                        MessageBoxButton.OK
                        , MessageBoxImage.Exclamation);
                    return;
                }
                if (MessageBox.Show("هل انت متأكد؟", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question
                ) == MessageBoxResult.Yes)
                {
                    await BusSubscriptionServices.CancelSubscriptionAsync(SelectedSubscription);
                    MessageBox.Show("تم حذف الاشتراك", "عملية ناجحة", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    await InitializeSubscriptionsList();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التنفيذ\n" + App.GetFullExceptionMessage(ex),
                               "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void KeywordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, e);
            }
        }
    }
}
