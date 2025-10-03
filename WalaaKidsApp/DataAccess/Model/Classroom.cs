using System.Collections.Generic;
using System.Windows.Media;

namespace WalaaKidsApp.DataAccess.Model
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public Brush ColorBrush => !string.IsNullOrWhiteSpace(Color)
        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(Color))
        : Brushes.Transparent;
        public ICollection<Student> Students { get; set; }
        public int StudentsCount => Students?.Count ?? 0;

    }
}