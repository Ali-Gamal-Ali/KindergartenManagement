using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using WalaaKidsApp.Business.Services;
using WalaaKidsApp.DataAccess;
using Expression = System.Linq.Expressions.Expression;

namespace WalaaKidsApp.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            //x => x * 2 + 1
            InitializeComponent();
            this.DataContext=this;
        }


        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentViewControl.Content= new AddStudentScreen(this);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState==WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState=WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MainBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount==1 && e.ChangedButton==MouseButton.Left)
            {
                this.DragMove();
            }
            else if (e.ClickCount==2 && e.ChangedButton==MouseButton.Left)
            {
                if (this.WindowState == WindowState.Normal)
                    this.WindowState = WindowState.Maximized;
                else
                    this.WindowState = WindowState.Normal;

            }


        }

        private void AddClassroomButton_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentViewControl.Content = new AddClassroomScreen(this);
        }

        private void ViewStudentsButton_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentViewControl.Content = new StudentsListScreen(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CurrentViewControl.Content = new Dashboard();
        }

        private void NotSubscripedButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentViewControl.Content = new NotSubscribedStudents(this);
        }

        private void ViewBusSubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentViewControl.Content = new BusSubscriptionsListScreen(this);
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentViewControl.Content = new Dashboard();
        }

        private void ModifyDeleteClassroomButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentViewControl.Content = new ModifyDeleteClassroomScreen(this);

        }
    }
}
