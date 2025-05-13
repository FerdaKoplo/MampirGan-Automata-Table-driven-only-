using MampirGan_Automata__dan_Table_driven_.Objects;
using MampirGan_Automata__dan_Table_driven_.Utils;
using MampirGanApp.Enums.Events;
using MampirGanApp.Enums.States;
using MampirGanApp.Models;

namespace MampirGan_Automata__dan_Table_driven_.Fitur
{
    //
    public class CartRule
    {
        public CartEvent Event { get; }
        public string Key { get; }
        public string Label { get; }
        public CartState[] AllowedStates { get; }
        public Action<CartFeature> Action { get; }


        // Constructor CartRule
        public CartRule(string key, string label, CartEvent ev, CartState[] allowed, Action<CartFeature> action)
        {
            Key = key;
            Label = label;
            Event = ev;
            AllowedStates = allowed;
            Action = action;
        }
    }
    //Automata + table driven logic
    public class CartFeature
    {
        private CartState state = CartState.Empty;
        private readonly CartService service = new();
        private CheckoutLogic checkout;

        // dictionary untuk transisi state, menggantikan fungsionalitas if state (if (state == CartState.Active && event == CartEvent.ClearCart) return state = CartState.Empty;))
        private static readonly Dictionary<(CartState, CartEvent), CartState> Transitions = new()
    {
        { (CartState.Empty,     CartEvent.AddItem),    CartState.Active    },
        { (CartState.Active,    CartEvent.AddItem),    CartState.Active    },
        { (CartState.Active,    CartEvent.RemoveItem), CartState.Active    },
        { (CartState.Active,    CartEvent.ClearCart),  CartState.Empty     },
        { (CartState.Active,    CartEvent.Checkout),   CartState.CheckedOut},
        { (CartState.CheckedOut, CartEvent.Exit),      CartState.CheckedOut}
    };

        private readonly List<CartRule> cartCommands;

        public CartFeature()
        {
            service = new CartService();
            if (service.HasItems())
            {
                state = CartState.Active;
            }

            cartCommands = new List<CartRule>
        {
            new("1", "Tambahkan Item",    CartEvent.AddItem,    new[]{CartState.Empty, CartState.Active}, ctrl => ctrl.AddItem()),
            new("2", "Hapus Item", CartEvent.RemoveItem, new[]{CartState.Active},              ctrl => ctrl.RemoveItem()),
            new("3", "Lihat Cart",   CartEvent.ViewCart,   new[]{CartState.Empty, CartState.Active}, ctrl => ctrl.ViewCart()),
            new("4", "Bersihkan Keranjang",  CartEvent.ClearCart,  new[]{CartState.Active},              ctrl => ctrl.ClearCart()),
            new("5", "Checkout", CartEvent.Checkout, new[]{CartState.Active}, ctrl => ctrl.Checkout()),
            new("0", "Exit",        CartEvent.Exit,       new[]{CartState.Empty, CartState.Active, CartState.CheckedOut}, ctrl => ctrl.Exit())
        };
        }
        public void Run()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine($"\n=== Keranjang Action: {state} ===");
                foreach (var cmd in cartCommands)
                {
                    if (Array.Exists(cmd.AllowedStates, s => s == state))
                        Console.WriteLine($"{cmd.Key}. {cmd.Label}");
                }

                Console.Write("Pilih: ");
                var choice = Console.ReadLine();
                var rule = cartCommands.Find(c => c.Key == choice);

                if (rule == null || Array.IndexOf(rule.AllowedStates, state) < 0)
                {
                    Console.WriteLine("Pilih yang ada pada menu.");
                    continue;
                }
                rule.Action(this);

                if (Transitions.TryGetValue((state, rule.Event), out var next))
                    state = next;
            }
        }

        private void AddItem()
        {
            Console.Write("Item yang ingin ditambah: ");
            string productName = Console.ReadLine();

            Console.Write("Jumlah yang ingin dtambah: ");
            if (!int.TryParse(Console.ReadLine(), out int qty)) return;

            service.AddItem(productName, qty);
        }

        private void RemoveItem()
        {
            Console.Write("Item yang ingin dihapus: ");
            if (!int.TryParse(Console.ReadLine(), out int pid)) return;
            service.RemoveItem(pid);
        }

        private void ViewCart() => service.ViewCart();

        private void ClearCart() => service.ClearCart();
        public void Checkout()
        {
            if (!service.HasItems())
            {
                throw new InvalidOperationException("Tidak bisa checkout karena keranjang kosong.");
            }

            service.ViewCart();
            Console.WriteLine("\nLanjutkan ke checkout...");
            checkout = new CheckoutLogic(service.GetCart());
            checkout.Run();
        }
        private void Exit() => Console.WriteLine("Keluar Keranjang.");

    }
    //Services
    public class CartService
    {
        private readonly List<Cart> CartItems = new();
        private readonly List<Products> products;
        private SaveToJson saveToJson = new SaveToJson();
        private readonly string cartDataPath = "C:\\Users\\IVAN\\source\\repos\\MampirGan(Automata, dan Table driven)\\MampirGan(Automata, dan Table driven)\\Jsons\\CartData.json";

        public CartService()
        {
            products = Jsonloader.LoadProducts("C:\\Users\\IVAN\\source\\repos\\MampirGan(Automata, dan Table driven)\\MampirGan(Automata, dan Table driven)\\Jsons\\ProductDummy.json");
            var loadedCart = Jsonloader.LoadCartFromFile(cartDataPath);
            if (loadedCart.Any())
            {
                CartItems = loadedCart;
                Console.WriteLine("Data keranjang sebelumnya dimuat");
            }
            else
            {
                Console.WriteLine("Tidak ada data keranjang");
            }
        }
        public void AddItem(string productName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Nama produk tidak boleh kosong.");

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Jumlah harus lebih dari nol.");

            var prod = products.Find(p => p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));
            if (prod == null)
                throw new InvalidOperationException($"Produk dengan nama '{productName}' tidak ditemukan.");

            var existingItem = CartItems.FirstOrDefault(c => c.ProductID == prod.ProductID);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                Console.WriteLine($"Jumlah Item dirubah dengan nama {prod.ProductName} menjadi {existingItem.Quantity}");
            }
            else
            {
                var cartItem = new Cart
                {
                    CartID = GenerateCartID(),
                    ProductID = prod.ProductID,
                    Quantity = quantity,
                    products = prod
                };
                CartItems.Add(cartItem);
                Console.WriteLine($"Item {prod.ProductName} dengan jumlah {quantity} telah ditambahkan ke keranjang.");
            }

            if (!CartItems.Any(c => c.ProductID == prod.ProductID))
                throw new Exception("Gagal menambahkan item ke keranjang.");

            saveToJson.SaveCartToFile(cartDataPath, CartItems);
        }
        public void RemoveItem(int productID)
        {
            if (productID <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(productID), "ID produk tidak valid.");
            }


            var existingItem = CartItems.FirstOrDefault(c => c.ProductID == productID);
            if (existingItem == null)
            {
                throw new InvalidOperationException("Item tidak ditemukan di keranjang.");
            }

            CartItems.Remove(existingItem);
            Console.WriteLine($"Item {existingItem.products.ProductName} dihapus dari keranjang");

            if (CartItems.Any(c => c.ProductID == productID))
                throw new Exception("Item masih ada di keranjang setelah penghapusan.");

            saveToJson.SaveCartToFile(cartDataPath, CartItems);
        }

        public bool HasItems() => CartItems.Any();

        public void ViewCart()
        {
            if (CartItems == null)
                throw new InvalidOperationException("Data keranjang tidak tersedia.");

            if (!CartItems.Any())
            {
                Console.WriteLine("Keranjang kosong.");
                return;
            }

            Console.WriteLine("=== Isi Keranjang ===");
            foreach (var item in CartItems)
            {
                Console.WriteLine(
                    $"- {item.products.ProductName.PadRight(15)} | Qty: {item.Quantity,3} | " +
                    $"Harga: Rp{item.products.Price:N0} | Subtotal: Rp{(item.products.Price * item.Quantity):N0}");
            }

            var total = CartItems.Sum(i => i.products.Price * i.Quantity);
            Console.WriteLine($"Total belanja: Rp{total:N0}");
        }

        public void ClearCart()
        {
            CartItems.Clear();
            Console.WriteLine("Semua item di keranjang telah dihapus.");

            if (CartItems.Count != 0)
                throw new Exception("Keranjang belum benar-benar kosong setelah dibersihkan.");

            saveToJson.SaveCartToFile(cartDataPath, CartItems);
        }

        public List<Cart> GetCart()
        {
            return CartItems;
        }

        private int GenerateCartID()
        {
            var ran = new Random();
            int id;
            do
            {
                id = ran.Next(1, 999);
            }
            while (CartItems.Any(t => t.CartID == id));
            return id;
        }
    }
}
