using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WalaaKidsApp.Business;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess.Model;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for AddClassroomScreen.xaml
    /// </summary>
    public partial class AddClassroomScreen : UserControl, INotifyPropertyChanged, IDataErrorInfo
    {
        private string _classroomName;
        private ClassColor _selectedColor;
        private readonly MainWindow _mainWindow;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ClassColor> AvailableColors { get; set; } = new List<ClassColor>
         {
             new ClassColor { Name = "Red", Color = Colors.Red },
             new ClassColor { Name = "Green", Color = Colors.Green },
             new ClassColor { Name = "Blue", Color = Colors.Blue },
             new ClassColor { Name = "Yellow", Color = Colors.Yellow },
             new ClassColor { Name = "Orange", Color = Colors.Orange },
             new ClassColor { Name = "Purple", Color = Colors.Purple },
             new ClassColor { Name = "Pink",Color = Colors.Pink }
         };

        public ClassColor SelectedColor
        {
            get
            {
                return this._selectedColor;
            }
            set
            {
                this._selectedColor = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(SelectedColor)));
            }
        }

        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF0-9\u0660-\u0669 \-]+$", ErrorMessage = "حروف و ارقام و - فقط")]
        public string ClassroomName
        {
            get
            {
                return this._classroomName;
            }
            set
            {
                this._classroomName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassroomName)));
            }
        }

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

        public string Error => throw new NotImplementedException();

        public AddClassroomScreen(MainWindow mainWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            _mainWindow = mainWindow;
        }

        private void AddClassroomUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                var sb = (Storyboard)this.Resources["FadeInStoryboard"];
                sb.Begin();
            });
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(await ClassroomServices.CheckDuplicatedClassroom(this.ClassroomName))
                {
                    throw new ArgumentException("يوجد فصل مسجل بنفس الأسم");
                }
                var classroom = new Classroom
                {
                    Name = this.ClassroomName,
                    Color = this.SelectedColor.Color.ToString()
                };
                await ClassroomServices.AddClassroom(classroom);
                MessageBox.Show("تم تسجيل الفصل بنجاح", "عملية ناجحة"
                    , MessageBoxButton.OK
                    , MessageBoxImage.Information);
                _mainWindow.CurrentViewControl.Content = new AddClassroomScreen(_mainWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التنفيذ\n" + App.GetFullExceptionMessage(ex),
                                 "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
