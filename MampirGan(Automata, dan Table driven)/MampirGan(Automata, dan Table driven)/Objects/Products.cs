namespace MampirGan_Automata__dan_Table_driven_.Objects
{
    public class Products
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
    }
}
