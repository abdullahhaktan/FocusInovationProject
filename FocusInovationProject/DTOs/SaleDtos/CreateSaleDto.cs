using FocusInovationProject.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusInovationProject.DTOs.SaleDtos
{
    public class CreateSaleDto
    {
        public int ID { get; set; }

        public int PRODUCT_ID { get; set; }

        public int CUSTOMER_ID { get; set; }

        public double? QUANTITY { get; set; }
        public double? SALESPRICE { get; set; }
        public DateTime DATE { get; set; }
        public double AMOUNT { get; set; }
        public double LISTPRICE { get; set; }
        public double DISCOUNTRATE { get; set; }
    }
}
