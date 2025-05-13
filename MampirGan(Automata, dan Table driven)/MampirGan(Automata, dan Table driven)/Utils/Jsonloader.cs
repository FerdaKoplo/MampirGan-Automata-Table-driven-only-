using MampirGan_Automata__dan_Table_driven_.Objects;
using MampirGanApp.Models;
using System.Text.Json;

namespace MampirGan_Automata__dan_Table_driven_.Utils
{
    public class Jsonloader
    {
        public static List<Products> LoadProducts(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File tidak ditemukan, membuat list kosong.");
                    return new List<Products>();
                }

                var jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<List<Products>>(jsonString, options) ?? new List<Products>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal membaca file product: {ex.Message}");
                return new List<Products>();
            }

        }

        public static List<Cart> LoadCartFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File tidak ditemukan, membuat list kosong.");
                    return new List<Cart>();
                }

                var jsonData = File.ReadAllText(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<List<Cart>>(jsonData) ?? new List<Cart>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal membaca file cart: {ex.Message}");
                return new List<Cart>();
            }
        }

        public static List<Category> LoadCategoryFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File tidak ditemukan, membuat list kosong.");
                    return new List<Category>();
                }

                var jsonData = File.ReadAllText(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<List<Category>>(jsonData) ?? new List<Category>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal membaca file cart: {ex.Message}");
                return new List<Category>();
            }
        }
    }
}
