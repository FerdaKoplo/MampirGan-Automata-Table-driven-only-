using MampirGan_Automata__dan_Table_driven_.Objects;
using MampirGanApp.Models;

namespace MampirGan_Automata__dan_Table_driven_.Utils
{
    public class SaveToJson
    {
        public void SaveCartToFile(string filePath, List<Cart> CartItem)
        {
            try
            {
                var jsonData = System.Text.Json.JsonSerializer.Serialize(CartItem, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(filePath, jsonData);
                Console.WriteLine("Cart telah disimpan ke file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal menyimpan cart ke file: {ex.Message}");
            }
        }

        public void SaveCheckoutToFile(string filePath, List<Transaction> transactions)
        {
            try
            {
                var jsonData = System.Text.Json.JsonSerializer.Serialize(transactions, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(filePath, jsonData);
                Console.WriteLine("Hasil Checkout telah disimpan ke file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal menyimpan hasil chekout ke file: {ex.Message}");
            }
        }

    }
}
