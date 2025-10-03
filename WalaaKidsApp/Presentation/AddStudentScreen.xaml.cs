using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WalaaKidsApp.Business;
using WalaaKidsApp.Business.Enums;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess.Model;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for AddStudentScreen.xaml
    /// </summary>
    public partial class AddStudentScreen : UserControl, IDataErrorInfo, INotifyPropertyChanged
    {
        private string _nationalId;
        private string _studentName;
        private string _age;
        private string _dateOfBirth;
        private string _gender;
        private string _fatherName;
        private string _motherName;
        private string _primaryPhoneNumber;
        private string _secondaryPhoneNumber;
        private string _fatherJob;
        private string _address;
        private string _subscriptionAmount;
        private string _paymentAmount = "0";
        private DateTime _subscriptionDate = DateTime.Now;
        private DateTime _subscriptionExpiryDate = new DateTime(DateTime.Now.AddYears(1).Year, 06, 30);
        private Classroom _selectedClassroom;
        private SubscriptionType _subscriptionType;
        private Student _targetStudent;
        private readonly MainWindow mainWindow;

        [EgyptianNationalID(ErrorMessage = "برجاء ادخال رقم قومي صحيح !")]
        public string NationalId
        {
            get
            {
                return _nationalId;
            }
            set
            {
                this._nationalId = value;
                if (value.IsValidNationalId())
                {
                    UpdateFieldsByNationalId();
                }
                else
                {
                    Age=string.Empty;
                    DateOfBirth=string.Empty;
                    Gender=string.Empty;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NationalId)));
            }
        }
        [RegularExpression(@"^[a-z A-Z\u0600-\u06FF]+$", ErrorMessage = "برجاء ادخال حروف فقط !")]
        public string StudentName
        {
            get
            {
                return _studentName;
            }
            set
            {
                this._studentName = value;
                if (value.IsValidName())
                {
                    var splittedName = value.Split(' ');
                    FatherName = string.Join(" ", splittedName.Skip(1));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StudentName)));
            }
        }
        [RegularExpression(@"^[\d-.]+$", ErrorMessage = "برجاء ادخال ارقام فقط !")]
        public string Age
        {
            get
            {
                return _age;
            }
            set
            {
                this._age = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Age)));
            }
        }
        public string DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }
            set
            {
                this._dateOfBirth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DateOfBirth)));
            }
        }
        public string Gender
        {
            get
            {
                return _gender;
            }
            set
            {
                this._gender = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gender)));
            }
        }
        [RegularExpression(@"^[a-z A-Z\u0600-\u06FF]+$", ErrorMessage = "برجاء ادخال حروف فقط !")]
        public string FatherName
        {
            get
            {
                return _fatherName;
            }
            set
            {
                this._fatherName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FatherName)));
            }
        }
        [RegularExpression(@"^[a-z A-Z\u0600-\u06FF]+$", ErrorMessage = "برجاء ادخال حروف فقط !")]
        public string MotherName
        {
            get
            {
                return _motherName;
            }
            set
            {
                this._motherName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MotherName)));
            }
        }
        [EgyptianPhoneNumber(ErrorMessage = "برجاء ادخال رقم تليفون صحيح !")]
        public string PrimaryPhoneNumber
        {
            get
            {
                return _primaryPhoneNumber;
            }
            set
            {
                this._primaryPhoneNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrimaryPhoneNumber)));
            }
        }
        [EgyptianPhoneNumber(ErrorMessage = "برجاء ادخال رقم تليفون صحيح !")]
        public string SecondaryPhoneNumber
        {
            get
            {
                return _secondaryPhoneNumber;
            }
            set
            {
                this._secondaryPhoneNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SecondaryPhoneNumber)));
            }
        }
        [RegularExpression(@"^[a-z A-Z\u0600-\u06FF]+$", ErrorMessage = "برجاء ادخال حروف فقط !")]
        public string FatherJob
        {
            get
            {
                return _fatherJob;
            }
            set
            {
                this._fatherJob = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FatherJob)));
            }
        }
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF0-9\u0660-\u0669 \-]+$", ErrorMessage = "حروف و ارقام و - فقط")]
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                this._address = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Address)));
            }
        }
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
        [RegularExpression(@"^[\d.]+$", ErrorMessage = "برجاء ادخال ارقام فقط !")]
        public string SubscriptionAmount
        {
            get
            {
                return _subscriptionAmount;
            }
            set
            {
                this._subscriptionAmount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubscriptionAmount)));
            }
        }
        [RegularExpression(@"^[\d.]+$", ErrorMessage = "برجاء ادخال ارقام فقط !")]
        public string PaymentAmount
        {
            get
            {
                return _paymentAmount;
            }
            set
            {
                this._paymentAmount = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentAmount)));
            }
        }
        public Classroom SelectedClassroom
        {
            get
            {
                return _selectedClassroom;
            }
            set
            {
                this._selectedClassroom = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedClassroom)));
            }
        }
        public ObservableCollection<Classroom> Classrooms { get; set; } = new ObservableCollection<Classroom>();
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
        public bool IsRenewingSubscription { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

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

        public AddStudentScreen(MainWindow mainWindow, Student targetStudent = null, bool isRenewingSubscription = false)
        {
            InitializeComponent();
            this.DataContext = this;
            this.mainWindow=mainWindow;
            _targetStudent = targetStudent;
            IsRenewingSubscription=isRenewingSubscription;
        }

        private void InitializeEditing()
        {
            var _targetSubscription = _targetStudent.Subscriptions.SingleOrDefault(sp => sp.IsActive && sp.ExpiryDate > DateTime.UtcNow);
            this.NationalId = _targetStudent.NationalId;
            this.StudentName = _targetStudent.Name;
            this.FatherName = _targetStudent.Guardian.FatherName;
            this.MotherName = _targetStudent.Guardian.MotherName;
            this.PrimaryPhoneNumber = _targetStudent.Guardian.PhoneNumber;
            this.SecondaryPhoneNumber = _targetStudent.Guardian.SecondaryPhoneNumber;
            this.FatherJob = _targetStudent.Guardian.Job;
            this.Address = _targetStudent.Guardian.Address;
            if (_targetStudent.Classroom == null)
            {
                this.SelectedClassroom = Classrooms[0];
            }
            else
            {
                foreach (var clasroom in Classrooms)
                {
                    if (_targetStudent.ClassroomId==clasroom.Id)
                        SelectedClassroom = clasroom;
                }
            }
            if (_targetSubscription != null)
            {
                this.SubscriptionAmount = _targetSubscription.SubscriptionAmount.ToString();
                this.SubscriptionDate = _targetSubscription.SubscriptionDate;
                this.SubscriptionExpiryDate = _targetSubscription.ExpiryDate;
                this.SubscriptionType = _targetSubscription.SubscriptionType;
            }
            PaymentAmountBox.IsEnabled = false;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await FillClassrooms();
            this.SelectedClassroom = Classrooms[0];
            if (_targetStudent != null && !IsRenewingSubscription)
            {
                InitializeEditing();
            }
            if (IsRenewingSubscription)
                BackButton2.Visibility = Visibility.Collapsed;
             await this.Dispatcher.InvokeAsync(() =>
            {
                var sb = (Storyboard)this.Resources["FadeInStoryboard"];
                sb.Begin();
            });
        }
        private void UpdateFieldsByNationalId()
        {
            if (string.IsNullOrWhiteSpace(NationalId))
            {
                DateOfBirth = string.Empty;
                Age = string.Empty;
                Gender = string.Empty;
            }
            else
            {
                int year = Convert.ToInt32(NationalId.Substring(1, 2));
                int month = Convert.ToInt32(NationalId.Substring(3, 2));
                int day = Convert.ToInt32(NationalId.Substring(5, 2));
                int fullYear = Convert.ToInt32((NationalId[0] == '2' ? "19" : "20") + year);
                DateOfBirth = new DateTime(fullYear, month, day).ToString("dd/MM/yyyy");
                Age = (DateTime.Now.Year - fullYear).ToString();
                Gender = Convert.ToInt32(NationalId[12])%2==0 ? "انثى" : "ذكر";
            }
        }
        private async Task FillClassrooms()
        {
            try
            {
                Classrooms.Clear();
                foreach (var classroom in await ClassroomServices.GetAllClassrooms())
                {
                    Classrooms.Add(classroom);
                }
                Classrooms.Insert(0, new Classroom() { Name ="بدون فصل", Id=0 });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ اثناء جلب بيانات الفصول \n {ex.Message}\n{ex?.InnerException.Message}"
                    , "خطأ"
                    , MessageBoxButton.OK
                    , MessageBoxImage.Error);
            }
        }
        #region NavigationButtons
        private async void NextButton1_Click(object sender, RoutedEventArgs e)
        {
           if(_targetStudent == null)
            {
                if (await StudentServices.CheckNationalId(NationalId))
                {
                    MessageBox.Show("الطالب مسجل بالفعل, برجاء البحث في الاشتراكات الحالية او الاشتراكات المنتهية - ملغاة",
                        "تنبيه"
                            , MessageBoxButton.OK
                            , MessageBoxImage.Exclamation);
                    return;
                }
            }
            this.Opacity=0;
            GuardianDetailsGroupBox.Visibility = Visibility.Visible;
            StudentDetailsGroupBox.Visibility = Visibility.Hidden;
            SubscriptionDetailsGroupBox.Visibility = Visibility.Hidden;
            var sb = (Storyboard)this.Resources["FadeInStoryboard"];
            sb.Begin();
        }

        private void BackButton1_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity=0;
            StudentDetailsGroupBox.Visibility = Visibility.Visible;
            GuardianDetailsGroupBox.Visibility = Visibility.Hidden;
            SubscriptionDetailsGroupBox.Visibility = Visibility.Hidden;
            var sb = (Storyboard)this.Resources["FadeInStoryboard"];
            sb.Begin();
        }

        private void NextButton2_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity=0;
            SubscriptionDetailsGroupBox.Visibility = Visibility.Visible;
            StudentDetailsGroupBox.Visibility = Visibility.Hidden;
            GuardianDetailsGroupBox.Visibility = Visibility.Hidden;
            var sb = (Storyboard)this.Resources["FadeInStoryboard"];
            sb.Begin();
        }

        private void BackButton2_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity=0;
            GuardianDetailsGroupBox.Visibility = Visibility.Visible;
            SubscriptionDetailsGroupBox.Visibility = Visibility.Hidden;
            StudentDetailsGroupBox.Visibility = Visibility.Hidden;
            var sb = (Storyboard)this.Resources["FadeInStoryboard"];
            sb.Begin();
        }

        #endregion

        private async void SaveStudentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsRenewingSubscription)
                {
                    if (_targetStudent == null)
                    {
                        await AddStudentAsync();
                        MessageBox.Show("تم تسجيل الطالب بنجاح", "عملية ناجحة"
                            , MessageBoxButton.OK
                            , MessageBoxImage.Information);
                        mainWindow.CurrentViewControl.Content = new AddStudentScreen(mainWindow);
                    }
                    else
                    {
                        await EditStudentAsync();
                        MessageBox.Show("تم تحديث بيانت الطالب بنجاح", "عملية ناجحة"
                           , MessageBoxButton.OK
                           , MessageBoxImage.Information);
                        mainWindow.CurrentViewControl.Content = new StudentsListScreen(mainWindow);
                    }
                }
                else
                {
                    await RenewSubscriptionAsync();
                    MessageBox.Show("تم تجديد اشتراك الطالب بنجاح", "عملية ناجحة"
                         , MessageBoxButton.OK
                         , MessageBoxImage.Information);
                    mainWindow.CurrentViewControl.Content = new StudentsListScreen(mainWindow);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التنفيذ\n" + App.GetFullExceptionMessage(ex),
                                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RenewSubscriptionAsync()
        {
            var numericSubscriptionAmount = decimal.TryParse(this.SubscriptionAmount, out var subscriptionAmount) ? subscriptionAmount
                                        : throw new ArgumentException("حدث خطأ اثناء تسجيل المدفوعة");
            var numericPaymentAmount = decimal.TryParse(this.PaymentAmount, out var paymentAmount) ? paymentAmount
                : throw new ArgumentException("حدث خطأ اثناء تسجيل المدفوعة");
            var studentClassroom = this?.SelectedClassroom?.Id == 0 ? null : this.SelectedClassroom;
            var subscription = new Subscription
            {
                StudentId = this._targetStudent.Id,
                SubscriptionDate = this.SubscriptionDate.ToUniversalTime(),
                ExpiryDate = this.SubscriptionExpiryDate.ToUniversalTime(),
                SubscriptionType = this.SubscriptionType,
                IsActive = true,
                SubscriptionAmount = numericSubscriptionAmount
            };
            var payment = numericPaymentAmount == 0 ? null :
                   new Payment
                   {
                       PaymentAmount = numericPaymentAmount,
                       PaymentDate = DateTime.UtcNow
                   };
            await SubscriptionServices.RenewSubscriptionAsync(subscription, payment,studentClassroom);
        }

        private async Task EditStudentAsync()
        {
            var _targetSubscription = _targetStudent.Subscriptions.SingleOrDefault(sp => sp.IsActive && sp.ExpiryDate > DateTime.UtcNow);
            _targetStudent.NationalId = this.NationalId;
            _targetStudent.Name = this.StudentName;
            _targetStudent.Guardian.FatherName = this.FatherName;
            _targetStudent.Guardian.MotherName = this.MotherName;
            _targetStudent.Guardian.PhoneNumber = this.PrimaryPhoneNumber;
            _targetStudent.Guardian.SecondaryPhoneNumber = this.SecondaryPhoneNumber;
            _targetStudent.Guardian.Job = this.FatherJob;
            _targetStudent.Guardian.Address = this.Address;
            if (this.SelectedClassroom.Id == 0)
            {
                _targetStudent.ClassroomId = null;

            }
            else
            {
                _targetStudent.ClassroomId = SelectedClassroom.Id;
            }
            if (_targetSubscription != null)
            {
                _targetSubscription.SubscriptionAmount = decimal.TryParse(this.SubscriptionAmount, out decimal result) ? result : 0;
                _targetSubscription.SubscriptionDate = this.SubscriptionDate;
                _targetSubscription.ExpiryDate = this.SubscriptionExpiryDate;
                _targetSubscription.SubscriptionType = this.SubscriptionType;
            }
            await StudentServices.EditStudentAsync(_targetStudent);
        }

        private async Task AddStudentAsync()
        {
            var numericSubscriptionAmount = decimal.TryParse(this.SubscriptionAmount, out var subscriptionAmount) ? subscriptionAmount
                : throw new ArgumentException("حدث خطأ اثناء تسجيل المدفوعة");
            var numericPaymentAmount = decimal.TryParse(this.PaymentAmount, out var paymentAmount) ? paymentAmount
                : throw new ArgumentException("حدث خطأ اثناء تسجيل المدفوعة");
            var studentClassroom = this?.SelectedClassroom?.Id == 0 ? null : this.SelectedClassroom;
            if (numericSubscriptionAmount<=0)
                throw new ArgumentException("قيمة الاشتراك غير صحيحة");
            if (numericPaymentAmount>numericSubscriptionAmount)
                throw new ArgumentException("لا يمكن سداد قيمة اكبر من قيمة الاشتراك");
            var student = new Student
            {
                NationalId = this.NationalId,
                Name = this.StudentName,
                Age=int.TryParse(this.Age, out var age) ? age : 0,
                DateOfBirth = DateTime.TryParseExact(this.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth)
                ? dateOfBirth : default,
                Gender=this.Gender,
                ClassroomId = studentClassroom?.Id == 0 ? null : studentClassroom?.Id
            };
            var guardian = new Guardian
            {
                FatherName = this.FatherName,
                MotherName = this.MotherName,
                PhoneNumber = this.PrimaryPhoneNumber,
                SecondaryPhoneNumber = this.SecondaryPhoneNumber,
                Address = this.Address,
                Job = this.FatherJob
            };
            var subscription = new Subscription
            {
                SubscriptionDate = this.SubscriptionDate.ToUniversalTime(),
                SubscriptionAmount = numericSubscriptionAmount,
                SubscriptionType = this.SubscriptionType,
                ExpiryDate =this.SubscriptionExpiryDate.ToUniversalTime(),
                IsActive = true
            };
            var payment = numericPaymentAmount == 0 ? null :
                new Payment
                {
                    PaymentAmount = numericPaymentAmount,
                    PaymentDate = DateTime.UtcNow
                };
            await StudentServices.AddStudent(student, guardian, subscription, payment);
        }
    }
}

