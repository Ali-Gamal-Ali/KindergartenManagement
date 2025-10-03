using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for ModifyDeleteClassroomScreen.xaml
    /// </summary>
    public partial class ModifyDeleteClassroomScreen : UserControl
    {
        private MainWindow _mainWindow;
        public ObservableCollection<Classroom> Classrooms { get; set; } = new ObservableCollection<Classroom>();

        public ModifyDeleteClassroomScreen(MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            _mainWindow = mainWindow;
        }

        private async void ModifyDeleteClassroomUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await FillClassroomsAsync();
            await this.Dispatcher.InvokeAsync(() =>
            {
                var sb = (Storyboard)this.Resources["FadeInStoryboard"];
                sb.Begin();
            });
        }

        private async Task FillClassroomsAsync()
        {
            Classrooms.Clear();
            foreach (var classroom in await ClassroomServices.GetAllClassrooms())
            {
                Classrooms.Add(classroom);
            }
        }

        private async void ChangeNameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ClassroomNameTextBox.Text))
                    throw new ArgumentException("برجاء كتابة اسم صحيح");
                var selectedClassroom = ClassroomsBox.SelectedItem as Classroom;
                if (selectedClassroom == null)
                    throw new ArgumentNullException("حدث خطأ اثناء تحديد الفصل المطلوب تعديله");
                await ClassroomServices.ChangeNameAsync(selectedClassroom, ClassroomNameTextBox.Text);
                MessageBox.Show("تم تعديل اسم الفصل بنجاح", "عملية ناجحة"
                         , MessageBoxButton.OK
                         , MessageBoxImage.Information);
                _mainWindow.CurrentViewControl.Content = new ModifyDeleteClassroomScreen(_mainWindow);

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التنفيذ\n" + App.GetFullExceptionMessage(ex),
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedClassroom = ClassroomsBox.SelectedItem as Classroom;
                if (selectedClassroom == null)
                    throw new ArgumentNullException("حدث خطأ اثناء تحديد الفصل المطلوب تعديله");
                if (MessageBox.Show("هل انت متأكد؟", "تأكيد"
                         , MessageBoxButton.YesNo
                         , MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await ClassroomServices.RemoveClassroomAsync(selectedClassroom);
                    MessageBox.Show("تم حذف الفصل بنجاح", "عملية ناجحة"
                             , MessageBoxButton.OK
                             , MessageBoxImage.Information);
                    _mainWindow.CurrentViewControl.Content = new ModifyDeleteClassroomScreen(_mainWindow);
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التنفيذ\n" + App.GetFullExceptionMessage(ex),
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
