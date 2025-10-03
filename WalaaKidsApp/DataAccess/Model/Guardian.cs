namespace WalaaKidsApp.DataAccess.Model
{
    public class Guardian
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string PhoneNumber { get; set; }
        public string SecondaryPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Job { get; set; }
    }
}