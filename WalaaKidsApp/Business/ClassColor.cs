using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WalaaKidsApp.Business
{
    public class ClassColor
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public Brush ColorBrush => new SolidColorBrush(Color);
        public string Hex => Color.ToString();
    }
}
