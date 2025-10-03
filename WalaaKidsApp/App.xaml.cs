using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WalaaKidsApp.DataAccess;

namespace WalaaKidsApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Task.Run(() =>
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        context.Students.Count();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حد خطأ اثناء الاتصال الاولي بقاعدة البيانات\n{ex?.Message}" +
                        $"innererror={ex?.InnerException?.Message}");
                }
            });
        }
        public static string GetFullExceptionMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                ex = ex.InnerException;
            }
            return sb.ToString();
        }
    }
}
