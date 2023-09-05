using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using phinance2.Models;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace phinance2
{
    public partial class MyAccount : Window
    {
        private PhinanceWpfDatabaseContext dbContext = new PhinanceWpfDatabaseContext();

        private DispatcherTimer timer;

        public int UserId { get; set; }

        public AccountBalance? AccountBalance { get; set; }

        public MyAccount()
        {
            InitializeComponent();

            DataContext = this;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5); // Set the refresh interval (e.g., every 0.5 second)
            timer.Tick += Timer_Tick;
            timer.Tick += UpdateCryptoPrice_Tick;
            timer.Start();
        }

        private void DepositUSD_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DepositAmountInput.Text))
            {
                decimal.TryParse(DepositAmountInput.Text, out decimal depositAmount);

                // Fetch the user's account balance from the database
                var accountBalance = dbContext.AccountBalances
                    .Where(ab => ab.UserId == UserId)
                    .FirstOrDefault();

                if (accountBalance != null)
                {

                    accountBalance.Usd = (decimal)(accountBalance.Usd + depositAmount);
                    dbContext.SaveChanges();
                    MessageBox.Show("Successfully deposited " + depositAmount + " USD");
                    UpdateAccountBalanceDisplay();

                }
                else
                {
                    MessageBox.Show("Failed to retrieve user account balance.");
                }
            }
            else
            {
                MessageBox.Show("Please type in the amount of USD.");
            }
        }

        private void WithdrawUSD_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(withdrawAmountInput.Text))
            {
                decimal.TryParse(withdrawAmountInput.Text, out decimal withdrawAmount);

                // Fetch the user's account balance from the database
                var accountBalance = dbContext.AccountBalances
                    .Where(ab => ab.UserId == UserId)
                    .FirstOrDefault();

                if (accountBalance != null)
                {
                    if (accountBalance.Usd >= withdrawAmount)
                    {
                        accountBalance.Usd = (decimal)(accountBalance.Usd - withdrawAmount);
                        dbContext.SaveChanges();
                        MessageBox.Show("Successfully Withdrew " + withdrawAmount + " USD");
                        UpdateAccountBalanceDisplay();
                    }
                    else
                    {
                        MessageBox.Show("Failed to withdraw. Insufficient funds.");
                    }
                 

                }
                else
                {
                    MessageBox.Show("Failed to retrieve user account balance.");
                }
            }
            else
            {
                MessageBox.Show("Please type in the amount of USD.");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            dbContext = new PhinanceWpfDatabaseContext();
            // Query the database for the latest AccountBalance
            AccountBalance = dbContext.AccountBalances
                .Where(ab => ab.UserId == UserId)
                .FirstOrDefault();

            UpdateAccountBalanceDisplay();
        }

        private async void UpdateCryptoPrice_Tick(object sender, EventArgs e)
        {
            //await CryptoPriceUpdater.UpdateCryptoPricesAsync();
            using (var dbContextForCryptoPrices = new PhinanceWpfDatabaseContext())
            {
                await CryptoPriceUpdater.UpdateCryptoPricesAsync(dbContextForCryptoPrices);
            }
        }

        private void UpdateAccountBalanceDisplay()
        {

            decimal btcValue = CalculateCryptoValue("BTC", (decimal)AccountBalance.Btc);
            decimal ethValue = CalculateCryptoValue("ETH", (decimal)AccountBalance.Eth);
            decimal dogeValue = CalculateCryptoValue("DOGE", (decimal)AccountBalance.Doge);
            decimal xrpValue = CalculateCryptoValue("XRP", (decimal)AccountBalance.Xrp);
            decimal bnbValue = CalculateCryptoValue("BNB", (decimal)AccountBalance.Bnb);

            txtUsdAmount.Text = string.Format("{0:0.0000}", AccountBalance.Usd);
            txtBtcAmount.Text = string.Format("{0:0.0000}", AccountBalance.Btc);
            txtBtcValue.Text = btcValue.ToString("0.0000");
            txtEthAmount.Text = string.Format("{0:0.0000}", AccountBalance.Eth);
            txtEthValue.Text = ethValue.ToString("0.0000");
            txtDogeAmount.Text = string.Format("{0:0.0000}", AccountBalance.Doge);
            txtDogeValue.Text = dogeValue.ToString("0.0000");
            txtXrpAmount.Text = string.Format("{0:0.0000}", AccountBalance.Xrp);
            txtXrpValue.Text = xrpValue.ToString("0.0000");
            txtBnbAmount.Text = string.Format("{0:0.0000}", AccountBalance.Bnb);
            txtBnbValue.Text = bnbValue.ToString("0.0000");

            decimal totalValue = (decimal)AccountBalance.Usd + btcValue + ethValue + dogeValue + xrpValue + bnbValue;

            txtTotalValue.Text = "Estimated Total Value in USD: $ " + totalValue.ToString("0.0000");

        }

        private decimal CalculateCryptoValue(string symbol, decimal amount)
        {
            CryptoPrice cryptoPrice = dbContext.CryptoPrices
                .Where(cp => cp.Symbol == symbol)
                .FirstOrDefault();

            if (cryptoPrice != null)
            {
                return amount * cryptoPrice.Price;
            }

            // Return 0 if the cryptocurrency symbol is not found
            return 0;
        }

        private void NumericInputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Use a regular expression to allow only numeric input
            string regexPattern = "^[0-9]*(?:\\.[0-9]*)?$"; // Allows integers and decimal numbers

            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text + e.Text;

            if (!System.Text.RegularExpressions.Regex.IsMatch(newText, regexPattern))
            {
                e.Handled = true; // Prevents non-numeric input
            }
        }

        private void TxtDepositAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DepositAmountInput.Text) && DepositAmountInput.Text.Length > 0)
                textDepositAmount.Visibility = Visibility.Collapsed;
            else
                textDepositAmount.Visibility = Visibility.Visible;
        }

        private void TextDepositAmout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DepositAmountInput.Focus();
        }

        private void TxtWithdrawAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(withdrawAmountInput.Text) && withdrawAmountInput.Text.Length > 0)
                textWithdrawAmount.Visibility = Visibility.Collapsed;
            else
                textWithdrawAmount.Visibility = Visibility.Visible;
        }

        private void TextWithdrawAmout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            withdrawAmountInput.Focus();
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
            //To Close this window
            this.Close();
        }
    }
}
