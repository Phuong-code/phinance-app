using phinance2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace phinance2
{
    public static class CryptoPriceUpdater
    {
        //private static PhinanceWpfDatabaseContext dbContext = new PhinanceWpfDatabaseContext();

        public static async Task UpdateCryptoPricesAsync(PhinanceWpfDatabaseContext dbContext)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Send an HTTP GET request to the API
                    var response = await client.GetAsync("https://api.binance.com/api/v3/ticker/price");

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response into an array of objects
                        var prices = Newtonsoft.Json.JsonConvert.DeserializeObject<CryptoPrice[]>(content);

                        // Filter and store the data for selected symbols
                        var symbolsToKeep = new[] { "BTCUSDT", "ETHUSDT", "DOGEUSDT", "XRPUSDT", "BNBUSDT" };
                        foreach (var price in prices)
                        {
                            var symbolWithoutUSDT = price.Symbol.Replace("USDT", "");

                            if (symbolsToKeep.Contains(price.Symbol))
                            {
                                // Check if a record with the same symbol already exists in the database
                                var existingPrice = dbContext.CryptoPrices.FirstOrDefault(cp => cp.Symbol == symbolWithoutUSDT);

                                if (existingPrice != null)
                                {
                                    // Update the existing record with the new price
                                    existingPrice.Price = price.Price;
                                }
                                else
                                {
                                    // Add a new record if it doesn't exist
                                    dbContext.CryptoPrices.Add(new CryptoPrice
                                    {
                                        Symbol = symbolWithoutUSDT,
                                        Price = price.Price
                                    });
                                }
                            }
                        }

                        // Save changes to the database
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        MessageBox.Show("Failed to fetch crypto prices from the API.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
