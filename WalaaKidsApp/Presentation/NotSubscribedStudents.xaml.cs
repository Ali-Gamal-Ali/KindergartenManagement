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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for NotSubscribedStudents.xaml
    /// </summary>
    public partial class NotSubscribedStudents : UserControl, INotifyPropertyChanged
    {
        private MainWindow _mainWindow;
        private Student _selectedStudent;

        public event PropertyChangedEventHandler PropertyChanged;

        public Student SelectedStudent
        {
            get
            {
                return _selectedStudent;
            }
            set
            {
                _selectedStudent = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStudent)));
            }
        }
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();
        
        public NotSubscribedStudents(MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            _mainWindow = mainWindow;
        }

        private async void NotSubscribedStudentsUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await GetAllStudentsAsync();
            await this.Dispatcher.InvokeAsync(() =>
            {
                var sb = (Storyboard)this.Resources["FadeInStoryboard"];
                sb.Begin();
            });
        }

        private async Task GetAllStudentsAsync()
        {
            Students.Clear();
            foreach (var student in await StudentServices.GetUnSubscribedStudentAsync())
            {
                Students.Add(student);
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var keyword = KeywordTextBox.Text;
            var result = await StudentServices.GetUnSubscribedStudentAsync(keyword);
            Students.Clear();
            foreach (var student in result)
                Students.Add(student);
        }

        private void NotSubscribedStudentsUserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, e);
            }
        }

        private void RenewSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            var addStudentScreen = new AddStudentScreen(_mainWindow,SelectedStudent,true);
            addStudentScreen.Opacity=0;
            addStudentScreen.SubscriptionDetailsGroupBox.Visibility = Visibility.Visible;
            addStudentScreen.StudentDetailsGroupBox.Visibility = Visibility.Hidden;
            addStudentScreen.GuardianDetailsGroupBox.Visibility = Visibility.Hidden;
            _mainWindow.CurrentViewControl.Content = addStudentScreen;
            var sb = (Storyboard)addStudentScreen.Resources["FadeInStoryboard"];
            sb.Begin();
        }

        private void StudentDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.CurrentViewControl.Content = new StudentDetailsScreen(SelectedStudent, _mainWindow,false);

        }
    }
}
