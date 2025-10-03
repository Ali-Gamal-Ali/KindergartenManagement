using System;
using System.Collections.Generic;
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
using WalaaKidsApp.DataAccess.Model;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for StudentDetailsScreen.xaml
    /// </summary>
    public partial class StudentDetailsScreen : UserControl
    {
        private Student _student;
        public StudentDetailsScreen(Student student,MainWindow mainWindow,bool IsSubscribed = true)
        {
            InitializeComponent();
            this.DataContext = this;
            this._student = student;
            NameTextBlock.Text = student.Name;
            ClassroomTextBlock.Text = student?.Classroom?.Name ?? "بدون فصل";
            if (IsSubscribed)
            {
                StudentDetailsTab.Content = new StudentInformationScreen(_student);
                StudentSubscriptionDetailsTab.Content = new SubscriptionDetailsScreen(_student, mainWindow);
                StudentOldSubscriptionsDetailsTab.Content = new StudentOldSubscriptionsScreen(_student);
            }
            else
            {
                StudentSubscriptionDetailsTab.Visibility=Visibility.Collapsed;
                StudentDetailsTab.Content = new StudentInformationScreen(_student);
                StudentOldSubscriptionsDetailsTab.Content = new StudentOldSubscriptionsScreen(_student);
            }
        }
    }
}
