using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using FocusInovationProject.DTOs.StockDtos;
using FocusInovationProject.Repositories.ProductRepositories;
using FocusInovationProject.Repositories.StockRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FocusInovationProject.Controllers
{
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;

        // Repository katmanlarını enjekte ederek veri erişimini soyutluyoruz
        public StockController(IStockRepository stockRepository, IProductRepository productRepository)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        // Stok girişi veya güncelleme formlarında ürün seçimi yapabilmek için lookup listesini hazırlar
        private async Task GetProducts()
        {
            var products = await _productRepository.GetAllAsync();

            // Ürün verilerini asenkron olarak çekip SelectListItem tipine ayarlıyoruz
            ViewBag.products = (from product in products
                                  select new SelectListItem
                                  {
                                      Value = product.ID.ToString(),
                                      Text = product.NAME
                                  }).ToList();
        }

        // Stok listesinin ana görünümünü döner
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks(DataSourceLoadOptions loadOptions)
        {
            // Verimlilik adına ürün detaylarıyla (Join) zenginleştirilmiş stok verisini getiriyoruz
            var values = await _stockRepository.GetStocksWithProduct();

            // DevExtreme bileşeninin server-side işlemlerini (paging, sorting) yürütebilmesi için veriyi döner
            return Json(DataSourceLoader.Load(values, loadOptions));
        }

        // Yeni stok tanımı yapmak için gerekli formu hazırlar
        [HttpGet]
        public async Task<IActionResult> CreateStock()
        {
            await GetProducts();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStock(CreateStockDto stockDto)
        {
            // Olası bir validasyon hatasında UI'daki seçim listesinin kaybolmaması için listeyi tazeliyoruz
            await GetProducts();

            // Yeni stok bilgisini asenkron olarak kaydediyoruz
            await _stockRepository.CreateAsync(stockDto);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStock(int id)
        {
            // İlgili stok kaydını asenkron olarak sistemden kaldırır
            await _stockRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStock(int id)
        {
            await GetProducts();

            // Güncellenecek nesneyi ID üzerinden asenkron resolve ediyoruz
            var stock = await _stockRepository.GetByIdAsync(id);

            // Veri tutarlılığı için null check (Guard Clause) yapıp güvenli çıkış sağlıyoruz
            if (stock == null)
                return RedirectToAction("Index");

            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(UpdateStockDto stockDto)
        {
            // Mevcut stok bilgilerini DTO'dan gelen güncel verilerle persist ediyoruz
            await _stockRepository.UpdateAsync(stockDto);
            return RedirectToAction("Index");
        }
    }
}