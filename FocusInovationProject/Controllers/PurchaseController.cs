using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using FocusInovationProject.DTOs.PurchaseDtos;
using FocusInovationProject.Repositories.CustomerRepositories;
using FocusInovationProject.Repositories.ProductRepositories;
using FocusInovationProject.Repositories.PurchaseRepositories;
using FocusInovationProject.Repositories.StockRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FocusInovationProject.Controllers
{
    // Satınalma operasyonlarını ve buna bağlı stok hareketlerini yöneten controller
    public class PurchaseController : Controller
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStockRepository _stockRepository;

        // Çoklu repository kullanımıyla cross-functional (ürün, müşteri, stok) bir yapı kurulmuş
        public PurchaseController(IPurchaseRepository purchaseRepository, IProductRepository productRepository, ICustomerRepository customerRepository, IStockRepository stockRepository)
        {
            _purchaseRepository = purchaseRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _stockRepository = stockRepository;
        }

        // View tarafındaki SelectBox'ları beslemek için aktif ürün listesini projeksiyon yapıyoruz
        private async Task GetProducts()
        {
            var products = await _productRepository.GetAllAsync();

            ViewBag.products = (from product in products
                                select new SelectListItem
                                {
                                    Value = product.ID.ToString(),
                                    Text = product.NAME
                                }).ToList();
        }

        // Müşteri/Tedarikçi bilgilerini UI tarafında anlamlı bir metinle birleştirerek hazırlıyoruz
        private async Task GetCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();

            ViewBag.customers = (from customer in customers
                                 select new SelectListItem
                                 {
                                     Value = customer.ID.ToString(),
                                     Text = customer.CUSTOMERTITLE + " " + customer.CUSTOMERNUMBER
                                 }).ToList();
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchases(DataSourceLoadOptions loadOptions)
        {
            // İlişkili tabloları (Product, Customer) içeren genişletilmiş datayı asenkron getiriyoruz
            var values = await _purchaseRepository.GetPurchaseWithProductAndCustomer();

            // DevExtreme DataGrid yükleme parametrelerine (sorting/paging) göre veriyi serialize ediyoruz
            return Json(DataSourceLoader.Load(values, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> CreatePurchase()
        {
            // Form render edilmeden önce gerekli lookup listelerini asenkron dolduruyoruz
            await GetProducts();
            await GetCustomers();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchase(CreatePurchaseDto purchaseDto)
        {
            // Validasyon hatası durumunda UI elementlerinin boş gelmemesi için listeleri tekrar yüklüyoruz
            await GetProducts();
            await GetCustomers();

            // Satınalma kaydını oluşturuyoruz
            await _purchaseRepository.CreateAsync(purchaseDto);

            // KRİTİK: Satınalma gerçekleştiği için stok miktarını asenkron olarak artırıyoruz
            await _stockRepository.UpdateQuantityIncreasingAsync((int)purchaseDto.PRODUCT_ID, (double)purchaseDto.QUANTITY);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            // İlgili satınalma kaydını sistemden temizliyoruz
            await _purchaseRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePurchase(int id)
        {
            await GetProducts();
            await GetCustomers();

            // Güncellenecek kaydın varlığını kontrol edip asenkron olarak getiriyoruz
            var purchase = await _purchaseRepository.GetByIdAsync(id);
            if (purchase == null)
                return RedirectToAction("Index");

            return View(purchase);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePurchase(UpdatePurchaseDto purchaseDto)
        {
            // Mevcut satınalma bilgisini Persistence katmanında güncelliyoruz
            await _purchaseRepository.UpdateAsync(purchaseDto);
            return RedirectToAction("Index");
        }
    }
}