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
    /// Interaction logic for StudentInformationScreen.xaml
    /// </summary>
    public partial class StudentInformationScreen : UserControl
    {
        public StudentInformationScreen(Student student)
        {
            InitializeComponent();
            IdTextBlock.Text = student.Id.ToString();
            NameTextBlock.Text = student.Name;
            AgeTextBlock.Text = student.Age.ToString();
            ClassroomTextBlock.Text = student?.Classroom?.Name ?? "بدون فصل";
            GenderTextBlock.Text = student.Gender;
            FatherNameTextBlock.Text = student?.Guardian?.FatherName;
            MotherNameTextBlock.Text = student?.Guardian?.MotherName;
            FatherPhoneTextBlock.Text = student?.Guardian?.PhoneNumber;
            MotherPhoneTextBlock.Text = student?.Guardian?.SecondaryPhoneNumber;
            JobTextBlock.Text = student?.Guardian?.Job;
            AddressTextBlock.Text = student?.Guardian?.Address;
        }
    }
}
