using System.ComponentModel.DataAnnotations.Schema;

namespace FocusInovationProject.Entities
{
    public class Stock
    {
        public int ID { get; set; }

        [Column("PRODUCT_ID")]
        public int PRODUCT_ID { get; set; }

        [ForeignKey(nameof(PRODUCT_ID))]
        public Product PRODUCT { get; set; }

        public double QUANTITY { get; set; }
        public DateTime DATE { get; set; }
    }
}
