using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using FocusInovationProject.DTOs.SaleDtos;
using FocusInovationProject.Repositories.CustomerRepositories;
using FocusInovationProject.Repositories.ProductRepositories;
using FocusInovationProject.Repositories.SaleRepositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Linq;
using FocusInovationProject.Repositories.StockRepositories;

namespace FocusInovationProject.Controllers
{
    // Satış süreçlerini, indirim hesaplamalarını ve stok entegrasyonunu yöneten controller
    public class SaleController : Controller
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStockRepository _stockRepository;

        // Constructor üzerinden bağımlılıkları yöneterek gevşek bağlı (loose-coupled) bir yapı kuruyoruz
        public SaleController(ISaleRepository saleRepository, IProductRepository productRespository, ICustomerRepository customerRepository, IStockRepository stockRepository)
        {
            _saleRepository = saleRepository;
            _customerRepository = customerRepository;
            _productRepository = productRespository;
            _stockRepository = stockRepository;
        }

        // UI tarafındaki SelectBox bileşenlerini beslemek için ürünleri map'liyoruz
        private async Task GetProducts()
        {
            var products = await _productRepository.GetAllAsync();
            ViewBag.products = products.Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.NAME
            }).ToList();
        }

        // Müşteri verilerini asenkron olarak çekip View tarafındaki seçim listelerine hazırlıyoruz
        private async Task GetCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();
            ViewBag.customers = customers.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.CUSTOMERTITLE
            }).ToList();
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSales(DataSourceLoadOptions loadOptions)
        {
            // Performans için müşteri ve ürün bilgilerini içeren join'li veriyi çekiyoruz
            var values = await _saleRepository.GetSalesWithCustomerAndProduct();

            // DevExtreme DataGrid'in beklediği server-side yükleme formatına dönüştürüyoruz
            return Json(DataSourceLoader.Load(values, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> CreateSale()
        {
            await GetCustomers();
            await GetProducts();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(CreateSaleDto saleDto)
        {
            await GetCustomers();
            await GetProducts();

            // Satış kaydını ana tabloya işliyoruz
            await _saleRepository.CreateAsync(saleDto);

            // KRİTİK: Satış sonrası stok miktarını güncelleyerek veri tutarlılığını sağlıyoruz
            await _stockRepository.UpdateQuantityAsync(saleDto.PRODUCT_ID, saleDto.QUANTITY ?? 0);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSale(int id)
        {
            // Silme işlemini repository katmanına delege ediyoruz
            await _saleRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSale(int id)
        {
            await GetCustomers();
            await GetProducts();

            // Güncellenecek kaydı asenkron resolve ediyoruz, kayıt yoksa güvenli çıkış yapıyoruz
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null) return RedirectToAction("Index");

            return View(sale);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSale(UpdateSaleDto saleDto)
        {
            await GetCustomers();
            await GetProducts();

            // Form verilerini persistence katmanına asenkron olarak iletiyoruz
            await _saleRepository.UpdateAsync(saleDto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateListPrice([FromForm] int id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null) return NotFound();

            double discountRate = sale.DISCOUNTRATE;
            double salesPrice = sale.SALESPRICE ?? 0;

            // Matematiksel olarak yeni listprice hesaplama (tersine mühendislik)
            double calculated = salesPrice;
            if (discountRate > 0 && discountRate < 100)
                calculated = salesPrice / (1 - (discountRate / 100.0));

            // Hesaplanan liste fiyatını veritabanında güncelliyoruz
            await _saleRepository.UpdateListPriceAsync(id, calculated);

            return Ok(calculated);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalesPrice([FromForm] int id, [FromForm] double salesPrice)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null) return NotFound();

            // Satış Fiyatını Güncelleme
            await _saleRepository.UpdateSalesPriceAsync(id, salesPrice);

            // UI tarafında anlık değişimleri göstermek için güncel entity durumunu dönüyoruz
            sale = await _saleRepository.GetByIdAsync(id);

            return Ok(new { salesPrice = sale.SALESPRICE, discountRate = sale.DISCOUNTRATE });
        }
    }
}