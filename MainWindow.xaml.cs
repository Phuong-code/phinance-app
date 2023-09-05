using Microsoft.EntityFrameworkCore;
using phinance2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace phinance2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private PhinanceWpfDatabaseContext dbContext = new PhinanceWpfDatabaseContext();

        public string Email { get; set; }

        public int UserId { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

        }

        private AccountBalance _accountBalance;
        public AccountBalance AccountBalance
        {
            get { return _accountBalance; }
            set
            {
                if (_accountBalance != value)
                {
                    _accountBalance = value;
                    OnPropertyChanged(nameof(AccountBalance));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LiveMarket_Click(object sender, RoutedEventArgs e)
        {
            TradingView tradingView = new TradingView();
            tradingView.UserId = UserId;
            tradingView.Owner = this;
            tradingView.Show();
        }

        private void AccountBalance_Click(object sender, RoutedEventArgs e)
        {
            dbContext = new PhinanceWpfDatabaseContext();
            MyAccount myAccount = new MyAccount();
            AccountBalance = dbContext.AccountBalances
                .Where(ab => ab.UserId == UserId)
                .FirstOrDefault();
            myAccount.AccountBalance = AccountBalance;
            myAccount.UserId = UserId;
            myAccount.Owner = this;
            myAccount.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            myAccount.Show();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            //To minimize the window
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            //To Maximize the window when the window is normal and vice versa...
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //To Close the application
            Application.Current.Shutdown();
        }
    }
}
