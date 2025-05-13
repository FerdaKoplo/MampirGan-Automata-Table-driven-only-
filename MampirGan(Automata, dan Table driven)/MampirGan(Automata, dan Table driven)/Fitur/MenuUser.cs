namespace MampirGan_Automata__dan_Table_driven_.Fitur
{
    public class MenuUser
    {
        private bool running = true;
        private readonly MenuUserView view = new();
        private readonly CartFeature cart = new();
        private readonly CategoryLogic category = new();
        public void ViewMenuUser()
        {
            running = true;
            while (running)
            {
                view.ShowMenu();
                var input = Console.ReadLine();

                switch (input)
                {
                    case "3":
                        category.Run();
                        break;
                    case "4":
                        cart.Run();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        view.ShowInvalidInput();
                        break;
                }
            }
        }
    }
    public class MenuUserView
    {
        public void ShowMenu()
        {
            Console.WriteLine("=== Mampir Gan ===");
            Console.WriteLine("1. Lihat Menu");
            Console.WriteLine("2. Cari Menu");
            Console.WriteLine("3. Cari Kerdasarkan Kategori");
            Console.WriteLine("4. Keranjang");
            Console.WriteLine("5. History Pembelian");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih: ");
        }
        public void ShowInvalidInput()
        {
            Console.WriteLine("Input tidak valid.");
        }
    }
}
