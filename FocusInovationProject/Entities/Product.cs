using System.ComponentModel.DataAnnotations.Schema;

namespace FocusInovationProject.Entities
{
    public class Product
    {
        public int ID { get; set; }

        [Column("CATEGORY_ID")]
        public int CATEGORY_ID { get; set; }

        [ForeignKey(nameof(CATEGORY_ID))]
        public Category CATEGORY { get; set; }

        public string? NAME { get; set; }
        public string? IMAGE_SRC { get; set; }
        public double? SALESPRICE { get; set; }
    }
}
