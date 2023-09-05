using Microsoft.EntityFrameworkCore;
using Microsoft.Web.WebView2.Core;
using MySql.Data.MySqlClient;
using phinance2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace phinance2
{
    /// <summary>
    /// Interaction logic for TradingView.xaml
    /// </summary>
    public partial class TradingView : Window
    {
        public int UserId { get; set; }

        public string CryptoSymbol { get; set; }

        private PhinanceWpfDatabaseContext dbContext = new PhinanceWpfDatabaseContext();

        public TradingView()
        {
            InitializeComponent();

            // Ensure WebView2 is ready
            webView.EnsureCoreWebView2Async(null);

            // Subscribe to the CoreWebView2 initialization event
            webView.CoreWebView2InitializationCompleted += TradingView_Loaded;

            //dbContext.Database.EnsureCreated();

            CryptoSymbol = "btc";

            LoadCryptoSymbolPlaceholder();

        }

        private void LoadCryptoSymbolPlaceholder()
        {
            CryptoSymbolBuyPlaceholder.Text = CryptoSymbol.ToUpper();
            CryptoSymbolSellPlaceholder.Text = CryptoSymbol.ToUpper();
        }

        private void TradingView_Loaded(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                try
                {
                    // Load tradingView HTML file
                    webView.CoreWebView2.Navigate(System.IO.Path.Combine("file:", System.AppDomain.CurrentDomain.BaseDirectory, "html", "tradingView.html"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading HTML file: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("WebView2 initialization failed.");
            }
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

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBuyAmount.Text))
            {
                decimal.TryParse(txtBuyAmount.Text, out decimal buyAmount);

                //CryptoPriceUpdater.UpdateCryptoPricesAsync();
                UpdateCryptoPrice();

                // Fetch the user's account balance from the database
                var accountBalance = dbContext.AccountBalances
                    .Where(ab => ab.UserId == UserId)
                    .FirstOrDefault();

                if (accountBalance != null)
                {
                    // Calculate newCryptoBalance based on the selected CryptoSymbol
                    string cryptoSymbol = CryptoSymbol;
                    var cryptoPrice = dbContext.CryptoPrices
                        .Where(cp => cp.Symbol == cryptoSymbol)
                        .FirstOrDefault();

                    if (cryptoPrice != null)
                    {

                        decimal buyAmountValue = buyAmount * cryptoPrice.Price;
                        if (buyAmountValue <= accountBalance.Usd)
                        {
                            accountBalance.Usd = (decimal)(accountBalance.Usd - buyAmountValue);
                            decimal newCryptoBalance;
                            switch (CryptoSymbol)
                            {
                                case "btc":
                                    accountBalance.Btc = (decimal)(accountBalance.Btc + buyAmount);
                                    break;
                                case "eth":
                                    accountBalance.Eth = (decimal)(accountBalance.Eth + buyAmount);
                                    break;
                                case "doge":
                                    accountBalance.Doge = (decimal)(accountBalance.Doge + buyAmount);
                                    break;
                                case "xrp":
                                    accountBalance.Xrp = (decimal)(accountBalance.Xrp + buyAmount);
                                    break;
                                case "bnb":
                                    accountBalance.Bnb = (decimal)(accountBalance.Bnb + buyAmount);
                                    break;
                                default:
                                    MessageBox.Show("Unsupported crypto symbol.");
                                    return;
                            }
                            // Save the changes to the database
                            dbContext.SaveChanges();

                            MessageBox.Show("Successfully bought "+buyAmount+" "+CryptoSymbol.ToUpper()+"!");
                            // Optionally, you can update the user interface or perform other actions.


                        }
                        else
                        {
                            MessageBox.Show("You don't have enough USD");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to retrieve cryptocurrency price.");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve user account balance.");
                }
            }
            else
            {
                MessageBox.Show("Please type in the amount of crypto.");
            }
        }

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSellAmount.Text))
            {
                decimal.TryParse(txtSellAmount.Text, out decimal sellAmount);

                //CryptoPriceUpdater.UpdateCryptoPricesAsync();
                UpdateCryptoPrice();

                // Fetch the user's account balance from the database
                var accountBalance = dbContext.AccountBalances
                    .Where(ab => ab.UserId == UserId)
                    .FirstOrDefault();

                if (accountBalance != null)
                {
                    // Calculate newCryptoBalance based on the selected CryptoSymbol
                    string cryptoSymbol = CryptoSymbol;
                    var cryptoPrice = dbContext.CryptoPrices
                        .Where(cp => cp.Symbol == cryptoSymbol)
                        .FirstOrDefault();

                    if (cryptoPrice != null)
                    {

                        decimal sellAmountValue = sellAmount * cryptoPrice.Price;
                        decimal cryptoAmount;
                        switch (CryptoSymbol)
                        {
                            case "btc":
                                cryptoAmount = (decimal)accountBalance.Btc;
                                break;
                            case "eth":
                                cryptoAmount = (decimal)accountBalance.Eth;
                                break;
                            case "doge":
                                cryptoAmount = (decimal)accountBalance.Doge;
                                break;
                            case "xrp":
                                cryptoAmount = (decimal)accountBalance.Xrp;
                                break;
                            case "bnb":
                                cryptoAmount = (decimal)accountBalance.Bnb;
                                break;
                            default:
                                MessageBox.Show("Unsupported crypto symbol.");
                                return;
                        }

                        if (sellAmount <= cryptoAmount)
                        {
                            accountBalance.Usd = (decimal)(accountBalance.Usd + sellAmountValue);
                            decimal newCryptoBalance;
                            switch (CryptoSymbol)
                            {
                                case "btc":
                                    accountBalance.Btc = (decimal)(accountBalance.Btc - sellAmount);
                                    break;
                                case "eth":
                                    accountBalance.Eth = (decimal)(accountBalance.Eth - sellAmount);
                                    break;
                                case "doge":
                                    accountBalance.Doge = (decimal)(accountBalance.Doge - sellAmount);
                                    break;
                                case "xrp":
                                    accountBalance.Xrp = (decimal)(accountBalance.Xrp -sellAmount);
                                    break;
                                case "bnb":
                                    accountBalance.Bnb = (decimal)(accountBalance.Bnb - sellAmount);
                                    break;
                                default:
                                    MessageBox.Show("Unsupported crypto symbol.");
                                    return;
                            }
                            // Save the changes to the database
                            dbContext.SaveChanges();

                            MessageBox.Show("Successfully sold "+sellAmount+" "+CryptoSymbol.ToUpper()+"!");
                            // Optionally, you can update the user interface or perform other actions.


                        }
                        else
                        {
                            MessageBox.Show("You don't have enough "+ cryptoSymbol.ToUpper());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to retrieve cryptocurrency price.");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve user account balance.");
                }
            }
            else
            {
                MessageBox.Show("Please type in the amount of crypto.");
            }
        }

        private void webView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            CryptoSymbol = e.TryGetWebMessageAsString().Replace("USDT","").ToLower();
            LoadCryptoSymbolPlaceholder();
        }

        private async void UpdateCryptoPrice()
        {
            using (var dbContextForCryptoPrices = new PhinanceWpfDatabaseContext())
            {
                await CryptoPriceUpdater.UpdateCryptoPricesAsync(dbContextForCryptoPrices);
            }
        }

        private void TxtBuyAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBuyAmount.Text) && txtBuyAmount.Text.Length > 0)
                textBuyAmount.Visibility = Visibility.Collapsed;
            else
                textBuyAmount.Visibility = Visibility.Visible;
        }

        private void TextBuyAmout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtBuyAmount.Focus();
        }

        private void TxtSellAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSellAmount.Text) && txtSellAmount.Text.Length > 0)
                textSellAmount.Visibility = Visibility.Collapsed;
            else
                textSellAmount.Visibility = Visibility.Visible;
        }

        private void TextSellAmout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSellAmount.Focus();
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
