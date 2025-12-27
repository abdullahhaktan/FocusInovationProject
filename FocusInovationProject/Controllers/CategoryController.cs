using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using FocusInovationProject.DTOs.CategoryDtos;
using FocusInovationProject.Repositories.CategoryRepositories;
using FocusInovationProject.Repositories.SaleRepositories;
using Microsoft.AspNetCore.Mvc;

namespace FocusInovationProject.Controllers
{
    // Kategorilerin yönetimini ve raporlama ekranlarını kontrol eden merkez
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        // Repository'leri içeri alarak veritabanı işlemlerini bu servisler üzerinden yürütüyoruz
        public CategoryController(ICategoryRepository categoryRepository, ISaleRepository saleRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Kategori listeleme sayfasını açar
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories(DataSourceLoadOptions loadOptions)
        {
            // Tüm kategorileri asenkron olarak getiriyoruz
            var values = await _categoryRepository.GetAllAsync();

            // DevExtreme tablosunun sayfalama ve filtreleme yapabilmesi için veriyi uygun formatta dönüyoruz
            return Json(DataSourceLoader.Load(values, loadOptions));
        }

        // Yeni kategori ekleme sayfasını açar
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto categoryDto)
        {
            // Modelde bir eksiklik (boş alan vb.) varsa formu verilerle birlikte geri gönder
            if (!ModelState.IsValid)
                return View(categoryDto);

            // Yeni kategoriyi veritabanına ekle
            await _categoryRepository.CreateAsync(categoryDto);

            // İşlem bitince ana listeye geri dön
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            // Güncellenecek kategoriyi ID ile bulup getiriyoruz
            var category = await _categoryRepository.GetByIdAsync(id);

            // Eğer kategori bulunamazsa ana sayfaya yönlendir (Hata almamak için önlem)
            if (category == null)
                return RedirectToAction("Index");

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto categoryDto)
        {
            // Kategori bilgilerini güncelliyoruz
            await _categoryRepository.UpdateAsync(categoryDto);

            return RedirectToAction("Index");
        }

        // Satış raporu sayfasını açan metod
        public IActionResult CategorySalesReportView()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCategoryReportData(DateTime? startDate, DateTime? endDate)
        {
            // Seçilen tarih aralığına göre rapor verilerini repository'den istiyoruz
            var data = _categoryRepository.GetCategorySalesReport(startDate, endDate);

            // Veriyi grafiklerde veya tablolarda kullanmak için JSON olarak gönderiyoruz
            return Json(data);
        }

        // Stok raporu sayfasını açan metod
        public IActionResult StockReportView()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetStockReportData()
        {
            // Stok durumlarını listeleyen rapor verisini çekiyoruz
            var data = _categoryRepository.GetStockReport();

            return Json(data);
        }
    }
}