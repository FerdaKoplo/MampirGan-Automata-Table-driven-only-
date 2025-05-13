namespace MampirGan_Automata__dan_Table_driven_.Objects
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public List<Products> Products { get; set; } = new List<Products>();
    }
}

