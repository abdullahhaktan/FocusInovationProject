namespace FocusInovationProject.Entities
{
    public class Category
    {
        public int ID { get; set; }
        public string? NAME { get; set; }
        public IList<Product> PRODUCTS { get; set; }
    }
}
