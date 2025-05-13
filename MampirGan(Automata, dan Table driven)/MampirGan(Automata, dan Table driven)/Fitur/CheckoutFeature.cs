using MampirGan_Automata__dan_Table_driven_.Objects;
using MampirGan_Automata__dan_Table_driven_.Utils;
using MampirGanApp.Enums.Events;
using MampirGanApp.Enums.States;
using MampirGanApp.Models;

namespace MampirGan_Automata__dan_Table_driven_.Fitur
{
    public class CheckoutRule
    {
        public string Key { get; }
        public string Label { get; }
        public CheckoutEvent Event { get; }
        public CheckoutState[] AllowedStates { get; }
        public Action<CheckoutLogic> Action { get; }

        public CheckoutRule(string key, string label, CheckoutEvent ev, CheckoutState[] allowed, Action<CheckoutLogic> action)
        {
            Key = key;
            Label = label;
            Event = ev;
            AllowedStates = allowed;
            Action = action;
        }
    }
    public class CheckoutLogic
    {
        private CheckoutState state = CheckoutState.Idle;
        private readonly CheckoutService service = new();
        private readonly List<Cart> currentCart;
        private bool running = true;

        private static readonly Dictionary<(CheckoutState, CheckoutEvent), CheckoutState> Transitions = new()
        {
            { (CheckoutState.Idle, CheckoutEvent.ConfirmCheckout), CheckoutState.Confirming },
            { (CheckoutState.Confirming, CheckoutEvent.Pay), CheckoutState.Processing },
            { (CheckoutState.Processing, CheckoutEvent.Success), CheckoutState.Completed },
            { (CheckoutState.Completed, CheckoutEvent.Exit), CheckoutState.Completed }
        };

        private readonly List<CheckoutRule> transactionCommands;
        public CheckoutLogic(List<Cart> cart)
        {
            currentCart = cart;

            transactionCommands = new List<CheckoutRule>
        {
            new("1", "Konfirmasi Checkout", CheckoutEvent.ConfirmCheckout, new[]{ CheckoutState.Idle}, ctrl => ctrl.ConfirmCheckout()),
            new("2", "Bayar", CheckoutEvent.Pay, new[]{ CheckoutState.Confirming }, ctrl => ctrl.Pay()),
            new("3", "Exit", CheckoutEvent.Exit, new[]{ CheckoutState.Completed }, ctrl => ctrl.Exit())
        };
        }

        public void Run()
        {
            running = true;

            while (running)
            {
                Console.WriteLine($"\n=== Checkout State: {state} ===");
                foreach (var cmd in transactionCommands)
                {
                    if (cmd.AllowedStates.Contains(state))
                        Console.WriteLine($"{cmd.Key}. {cmd.Label}");
                }

                Console.Write("Pilih: ");
                var input = Console.ReadLine();
                var rule = transactionCommands.Find(r => r.Key == input);

                if (rule == null || !rule.AllowedStates.Contains(state))
                {
                    Console.WriteLine("Pilihan tidak valid.");
                    continue;
                }

                rule.Action(this);

                if (Transitions.TryGetValue((state, rule.Event), out var next))
                    state = next;

                if (state == CheckoutState.Completed)
                    running = false;
            }
        }

        private void ConfirmCheckout()
        {
            Console.WriteLine("Checkout dikonfirmasi.");
        }

        private void Pay()
        {
            Console.WriteLine("Memproses pembayaran...");
            service.ProcessCheckout(currentCart);
            if (Transitions.TryGetValue((state, CheckoutEvent.Success), out var successState))
                state = successState;
            running = false;
        }
        private void Exit()
        {
            Console.WriteLine("Keluar dari proses checkout.");
        }

    }
    public class CheckoutService
    {
        public static List<Transaction> transactions = new();
        private SaveToJson saveToJson = new();
        private readonly string checkoutDataPath = "C:\\Users\\IVAN\\source\\repos\\MampirGan(Automata, dan Table driven)\\MampirGan(Automata, dan Table driven)\\Jsons\\CheckoutData.json";
        public void ProcessCheckout(List<Cart> cartItems)
        {
            if (cartItems == null || cartItems.Count == 0)
                throw new ArgumentException("Keranjang kosong. Tidak bisa checkout.");

            if (cartItems.Any(item => item.products == null))
                throw new InvalidOperationException("Terdapat item dalam keranjang yang tidak memiliki data produk.");

            var transaction = new Transaction
            {
                TransactionID = GenerateTransactionID(),
                Date = DateTime.Now,
                TotalAmount = cartItems.Sum(i => i.Quantity * i.products.Price),
                Items = new List<TransactionItem>()
            };

            foreach (var item in cartItems)
            {
                transaction.Items.Add(new TransactionItem
                {
                    ProductID = item.ProductID,
                    ProductName = item.products.ProductName,
                    Quantity = item.Quantity,
                    SubTotal = item.Quantity * item.products.Price
                });
            }

            transactions.Add(transaction);

            if (!transactions.Any(tran => tran.TransactionID == transaction.TransactionID))
                throw new Exception("Gagal menambahkan transaksi ke daftar transaksi.");

            Console.WriteLine($"Transaksi berhasil! ID: {transaction.TransactionID}");
            Console.WriteLine($"Tanggal: {transaction.Date}");
            Console.WriteLine("Detail:");
            foreach (var item in transaction.Items)
            {
                Console.WriteLine($"- {item.ProductName} x{item.Quantity} = {item.SubTotal:C}");
            }
            Console.WriteLine($"Total: {transaction.TotalAmount:C}");
            try
            {
                saveToJson.SaveCheckoutToFile(checkoutDataPath, transactions);
            }
            catch (Exception ex)
            {
                throw new IOException("Gagal menyimpan transaksi ke file JSON.", ex);
            }
        }
        private int GenerateTransactionID()
        {
            var ran = new Random();
            int id;
            do
            {
                id = ran.Next(1, 999);
            }
            while (transactions.Any(t => t.TransactionID == id));
            return id;
        }
    }

}
