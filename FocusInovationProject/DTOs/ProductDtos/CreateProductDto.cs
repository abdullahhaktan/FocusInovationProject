using FocusInovationProject.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusInovationProject.DTOs.ProductDtos
{
    public class CreateProductDto
    {

        [Column("CATEGORY_ID")]
        public int CATEGORY_ID { get; set; }

        public string? NAME { get; set; }
        public string? IMAGE_SRC { get; set; }
        public double? SALESPRICE { get; set; }
    }
}
