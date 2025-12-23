using FocusInovationProject.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FocusInovationProject.DTOs.ProductDtos
{
    public class ResultProductDto
    {
        public int ID { get; set; }

        [Column("CATEGORY_ID")]
        public int CATEGORY_ID { get; set; }

        [ForeignKey(nameof(CATEGORY_ID))]
        public Category CATEGORY { get; set; }

        public string? NAME { get; set; }
        public string? SRC { get; set; }
        public double? SALESPRICE { get; set; }
    }
}
