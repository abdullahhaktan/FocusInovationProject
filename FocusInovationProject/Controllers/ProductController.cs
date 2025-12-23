using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using FocusInovationProject.DTOs.ProductDtos;
using FocusInovationProject.Repositories.CategoryRepositories;
using FocusInovationProject.Repositories.ProductRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace FocusInovationProject.Controllers
{
    // Ürün yönetim süreçlerini ve ilgili business logic akışını kontrol eden sınıf
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        // Repository pattern kullanarak veriye erişimi soyutlaştırıyoruz
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // UI tarafındaki dropdown'ları beslemek için kategorileri SelectListItem tipine map'liyoruz
        private async Task GetCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();

            // Linq kullanarak hızlı bir projeksiyon işlemi gerçekleştiriyoruz
            ViewBag.categories = (from category in categories
                                  select new SelectListItem
                                  {
                                      Value = category.ID.ToString(),
                                      Text = category.NAME
                                  }).ToList();
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(DataSourceLoadOptions loadOptions)
        {
            // Performance için ürünleri direkt kategorileriyle beraber (Include/Join) çekiyoruz
            var values = await _productRepository.GetProductsWithCategories();

            // DevExtreme entegrasyonu için veriyi DataSourceLoader üzerinden server-side yükleme formatına sokuyoruz
            return Json(DataSourceLoader.Load(values, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            // View tarafında category listesine ihtiyaç olduğu için asenkron olarak dolduruyoruz
            await GetCategories();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto productDto)
        {
            // İşlem başarısız olsa dahi dropdown verisinin kaybolmaması için listeyi tazeliyoruz
            await GetCategories();

            // DTO üzerinden gelen veriyi repository katmanına asenkron olarak paslıyoruz
            await _productRepository.CreateAsync(productDto);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // ID bazlı silme işlemini gerçekleştirip ana sayfaya redirect oluyoruz
            await _productRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            await GetCategories();

            // Mevcut veriyi kontrol ediyoruz, eğer kayıt yoksa null check yapıp güvenli çıkış sağlıyoruz
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return RedirectToAction("Index");

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto productDto)
        {
            // Mevcut nesneyi DTO'dan gelen güncel verilerle persist ediyoruz
            await _productRepository.UpdateAsync(productDto);
            return RedirectToAction("Index");
        }
    }
}