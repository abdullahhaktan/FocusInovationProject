using FocusInovationProject.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusInovationProject.DTOs.SaleDtos
{
    public class UpdateSaleDto
    {
        public int ID { get; set; }

        [Column("PRODUCT_ID")]
        public int PRODUCT_ID { get; set; }

        [ForeignKey(nameof(PRODUCT_ID))]
        public Product PRODUCT { get; set; }


        [Column("CUSTOMER_ID")]
        public int CUSTOMER_ID { get; set; }

        [ForeignKey(nameof(CUSTOMER_ID))]
        public Customer CUSTOMER { get; set; }

        public double? QUANTITY { get; set; }
        public double? SALESPRICE { get; set; }
        public DateTime DATE { get; set; }
        public double AMOUNT { get; set; }
        public double LISTPRICE { get; set; }
        public double DISCOUNTRATE { get; set; }
    }
}
