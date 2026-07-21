using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;
using RealEstateProject.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstateProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // صفحة الـ Landing التعريفية (الصفحة الرئيسية الافتراضية للمشروع)
        public IActionResult Landing()
        {
            // نجيب أحدث العقارات المنشورة عشان نعرض إحصائية بسيطة في الـ Landing
            ViewBag.PropertiesCount = _context.Properties
                .Count(p => p.PublishStatus == "Published");
            ViewBag.OwnersCount = _context.Owners.Count();

            return View();
        }

        // إضافة متغيرات البحث هنا
        public async Task<IActionResult> Index(string location, string propType, string purpose, decimal? maxPrice)
        {
            // 1. تحويل الاستعلام لـ Queryable عشان نقدر نضيف عليه شروط براحتنا
            var propertiesQuery = _context.Properties
                .Include(p => p.Owner).ThenInclude(o => o.User)
                .Include(p => p.MediaFiles)
                .Where(p => p.ListingStatus == "Available" && p.PublishStatus == "Published")
                .AsQueryable();

            // 2. تطبيق الفلاتر لو المستخدم كتب حاجة في البحث
            if (!string.IsNullOrEmpty(location))
            {
                propertiesQuery = propertiesQuery.Where(p => p.Location.Contains(location));
            }

            if (!string.IsNullOrEmpty(propType))
            {
                propertiesQuery = propertiesQuery.Where(p => p.PropType == propType);
            }

            if (!string.IsNullOrEmpty(purpose))
            {
                propertiesQuery = propertiesQuery.Where(p => p.Purpose == purpose);
            }

            if (maxPrice.HasValue)
            {
                propertiesQuery = propertiesQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // 3. تنفيذ الاستعلام النهائي وجلب البيانات
            var availableProperties = await propertiesQuery
                .OrderByDescending(p => p.PropId)
                .ToListAsync();

            // حفظ قيم البحث عشان تفضل ظاهرة في الـ Inputs بعد ما الصفحة تحمل
            ViewData["CurrentLocation"] = location;
            ViewData["CurrentPropType"] = propType;
            ViewData["CurrentPurpose"] = purpose;
            ViewData["CurrentMaxPrice"] = maxPrice;

            // الـ OwnerId الحالي عشان كارت العقار يظهر أزرار Edit/Delete للمالك بس
            ViewBag.CurrentOwnerId = await GetCurrentOwnerIdAsync();

            return View(availableProperties);
        }

        // بيرجع OwnerId بتاع اليوزر الحالي لو هو Owner، وإلا null
        private async Task<int?> GetCurrentOwnerIdAsync()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return null;

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (appUser == null) return null;

            var owner = await _context.Owners.FirstOrDefaultAsync(o => o.OwnerId == appUser.UserId);
            return owner?.OwnerId;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}