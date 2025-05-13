namespace MampirGan_Automata__dan_Table_driven_.Objects
{
    public class Transaction
    {
        public int TransactionID { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public List<TransactionItem> Items { get; set; } = new List<TransactionItem>();
    }
}
