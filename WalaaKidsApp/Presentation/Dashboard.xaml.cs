using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WalaaKidsApp.Business.Services;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl, INotifyPropertyChanged
    {
        private readonly CultureInfo _cultureInfo = new CultureInfo("ar-EG");
        private SeriesCollection _columnSeriesCollection;
        private SeriesCollection _pieSeriesCollection;

        public event PropertyChangedEventHandler PropertyChanged;

        public SeriesCollection ColumnSeriesCollection
        {
            get
            {
                return _columnSeriesCollection;
            }
            set
            {
                _columnSeriesCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColumnSeriesCollection)));
            }
        }
        public SeriesCollection PieSeriesCollection
        {
            get
            {
                return _pieSeriesCollection;
            }
            set
            {
                _pieSeriesCollection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PieSeriesCollection)));
            }
        }
        public string[] Labels { get; set; }

        public Dashboard()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitilaizeColumnSeries();
            InitializePieChart();
            StudentCountTextBlock.Text =  $"اجمالى عدد الطلاب\n" +
                $"{DashboardServices.GetStudentsCount().ToString()}";
            CurrentMonthRevenue.Text =  $"ايرادات الشهر\n" +
                $"{await DashboardServices.GetCurrentMonthPayments()}";
            TotalAmountDueTextBlock.Text =  $"متأخرات سداد\n" +
                $"{DashboardServices.CalculateTotalAmountDue()}";
            _=this.Dispatcher.InvokeAsync(() =>
            {
                var sb = (Storyboard)this.Resources["FadeInStoryboard"];
                sb.Begin();
            });
        }

        private void InitilaizeColumnSeries()
        {
            var keyValuePair = DashboardServices.GetStudentCountPerClassroom();
            Labels = keyValuePair.Keys.ToArray();
            var columnSeries = new ColumnSeries
            {
                Values = new ChartValues<int>(keyValuePair.Values),
                Fill = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
                     {
                         new GradientStop((Color)ColorConverter.ConvertFromString("#662D8C"), 1),
                         new GradientStop((Color)ColorConverter.ConvertFromString("#ED1E79"), 0.1)
                     }
                }
                ,
                MaxColumnWidth = 20
            };
            ColumnSeriesCollection = new SeriesCollection { columnSeries };
        }
        private void InitializePieChart()
        {
            var summary = DashboardServices.GetSubscriptionPaymentSummary();
            PieSeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "مدفوع بالكامل",
                    Values = new ChartValues<decimal>
                    {
                        summary.FullyPaidCount
                    }
                },
                new PieSeries
                {
                    Title = "غير مدفوع بالكامل",
                    Values = new ChartValues<decimal>
                    {
                        summary.NotFullyPaidCount
                    }
                }
            };
        }
    }
}
