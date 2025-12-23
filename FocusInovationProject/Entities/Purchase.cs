using System.ComponentModel.DataAnnotations.Schema;

namespace FocusInovationProject.Entities
{
    public class Purchase
    {
        public int ID { get; set; }
        public double QUANTITY { get; set; }
        public double PRICE { get; set; }
        public double AMOUNT { get; set; }
        public DateTime DATE { get; set; }


        [Column("CUSTOMER_ID")]
        public int? CUSTOMER_ID { get; set; }

        [ForeignKey(nameof(CUSTOMER_ID))]
        public Customer CUSTOMER { get; set; }



        [Column("PRODUCT_ID")]
        public int? PRODUCT_ID { get; set; }

        [ForeignKey(nameof(PRODUCT_ID))]
        public Product PRODUCT { get; set; }
    }
}
