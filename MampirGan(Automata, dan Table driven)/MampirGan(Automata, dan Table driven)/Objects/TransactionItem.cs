namespace MampirGan_Automata__dan_Table_driven_.Objects
{
    public class TransactionItem
    {
        public int TransactionItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public List<Products> Products { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
