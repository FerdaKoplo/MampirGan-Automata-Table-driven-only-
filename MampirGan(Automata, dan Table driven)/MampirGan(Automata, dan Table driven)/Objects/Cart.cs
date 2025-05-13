using MampirGan_Automata__dan_Table_driven_.Objects;

namespace MampirGanApp.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public Products products { get; set; }
        public int Quantity { get; set; }
    }
}
