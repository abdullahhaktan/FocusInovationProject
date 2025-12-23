using FocusInovationProject.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusInovationProject.DTOs.StockDtos
{
    public class ResultStockDto
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
