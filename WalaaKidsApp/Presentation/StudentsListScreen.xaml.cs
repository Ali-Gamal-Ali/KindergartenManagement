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
using System.Windows.Threading;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for StudentsListScreen.xaml
    /// </summary>
    public partial class StudentsListScreen : UserControl , INotifyPropertyChanged
    {
        private MainWindow _mainWindow;
        private Student _selectedStudent;
        public Student SelectedStudent { get
            {
                return _selectedStudent;
            }
            set
            {
                _selectedStudent = value;
                PropertyChanged.Invoke(this,new PropertyChangedEventArgs(nameof(SelectedStudent)));
            }
        }
        public ObservableCollection<Classroom> Classrooms { get; set; } = new ObservableCollection<Classroom>();
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        public StudentsListScreen(MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            _mainWindow=mainWindow;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task FillClassrooms()
        {
            Classrooms.Clear();
            Classrooms.Add(new Classroom
            {
                Id=0,
                Color=null,
                Name="الكل"
            });
            foreach(var classroom in await ClassroomServices.GetAllClassrooms())
            {
                Classrooms.Add(classroom);
            }
        }

        private async void AddClassroomUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await FillClassrooms();
            await this.Dispatcher.InvokeAsync(() =>
            {
                var sb = (Storyboard)this.Resources["FadeInStoryboard"];
                sb.Begin();
            });
           
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null && listView.SelectedItem != null)
            {
                var selectedClass = listView.SelectedItem as Classroom;
                if(selectedClass != null)
                {
                    if (selectedClass.Id!=0)
                    {
                        Students.Clear();
                        foreach (var student in await StudentServices.GetStudentsAsync(null, selectedClass))
                        {
                            Students.Add(student);
                        }
                    }
                    else
                    {
                        Students.Clear();
                        foreach (var student in await StudentServices.GetStudentsAsync())
                        {
                            Students.Add(student);
                        }
                    }
                }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClassroom = ClassroomListView.SelectedItem as Classroom;
            if (selectedClassroom?.Id ==0)
                selectedClassroom = null;
            var keyword = KeywordTextBox.Text;
            var result = await StudentServices.GetStudentsAsync(keyword, selectedClassroom);
            Students.Clear();
            foreach (var student in result)
                Students.Add(student);
        }

        private void StudentDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.CurrentViewControl.Content = new StudentDetailsScreen(SelectedStudent,_mainWindow);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.CurrentViewControl.Content = new AddStudentScreen(_mainWindow, SelectedStudent);
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedStudent!=null)
            {
                _mainWindow.CurrentViewControl.Content = new StudentDetailsScreen(SelectedStudent, _mainWindow);
                e.Handled = true;
            }
           
        }

        private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep is DataGridRow row)
            {
                row.IsSelected = true;
            }
        }

        private async void BusSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (await BusSubscriptionServices.IsSubscriber(SelectedStudent))
            {
                if (MessageBox.Show("الطالب مشترك بالباص , هل ترغيب في تجديد - تعديل الاشتراك ؟", "تنبيه",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _mainWindow.CurrentViewControl.Content = new BusSubscriptionScreen(SelectedStudent, _mainWindow);
                }
            }
            else
            {
                _mainWindow.CurrentViewControl.Content = new BusSubscriptionScreen(SelectedStudent, _mainWindow);
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
