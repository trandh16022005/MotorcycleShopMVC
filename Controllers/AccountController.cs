using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Models;
using MotorcycleShopMVC.Models.ViewModels;
//using MotorcycleShopMVC.Models.ViewModels;

namespace MotorcycleShopMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        
        public AccountController(ApplicationDbContext context)
        {
            _context=context;
            _passwordHasher=new PasswordHasher<User>();
        }
       
        //GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        { 
            // Nếu chưa có returnUrl thì lấy từ Referer (trang trước đó)
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrWhiteSpace(referer))
                {
                    var uri = new Uri(referer);
                    var pathAndQuery = uri.PathAndQuery;
                    if (Url.IsLocalUrl(pathAndQuery) && !pathAndQuery.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase))
                    {
                        returnUrl = pathAndQuery;
                    }
                }
            }

            // Đã đăng nhập thì quay về returnUrl (nếu hợp lệ) hoặc Home
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Lưu session đăng nhập
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFullName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Ưu tiên quay về trang cũ
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register(string? returnUrl=null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrWhiteSpace(referer))
                {
                    var uri = new Uri(referer);
                    var pathAndQuery = uri.PathAndQuery;
                    if (Url.IsLocalUrl(pathAndQuery) &&
                        !pathAndQuery.Contains("/Account/Register", StringComparison.OrdinalIgnoreCase) &&
                        !pathAndQuery.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase))
                    {
                        returnUrl = pathAndQuery;
                    }
                }
            }

            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new RegisterViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl=null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var strength = EvaluatePasswordStrength(model.Password);

            if (strength == PasswordStrengthLevel.Weak)
            {
                ModelState.AddModelError("Password",
                    "Mật khẩu quá yếu. Sử dụng ít nhất 6 ký tự và kết hợp các chữ cái + số. Ưu tiên viết hoa, viết thường, số và ký tự đặc biệt.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }


            // Unique email
            var existedEmail = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (existedEmail)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Unique phone (nếu nhập)
            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                var existedPhone = await _context.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber);
                if (existedPhone)
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại");
                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber,
                Address = string.IsNullOrWhiteSpace(model.Address) ? null : model.Address,
                Role = "Customer",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Hash mật khẩu đúng theo cột PasswordHash
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Auto login sau khi đăng ký
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFullName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Ưu tiên quay về trang cũ
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private enum PasswordStrengthLevel
        {
            Weak=1,
            Medium=2,
            Strong=3,
            VeryStrong=4
        }

        private static PasswordStrengthLevel EvaluatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return PasswordStrengthLevel.Weak;

            int length = password.Length;
            bool hasLower = password.Any(char.IsLower);
            bool hasUpper = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            //very common predictable patterns
            string lower = password.ToLowerInvariant();
            string[] common = { "123456", "passoword", "qwerty", "111111", "abc123", "admin", "iloveyou" };
            bool containsCommon = common.Any(c => lower.Contains(c));

            //simple sequence check
            bool isSimpleSequence = "0123456789abcdefghijklmnopqrstuvwxyz".Contains(lower) || "abcdefghijklmnopqrstuvwxyz".Contains(lower);

            int groups = (hasLower ? 1 : 0) + (hasUpper ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSpecial ? 1 : 0);

            //Weak
            if (length < 6 || groups <= 1 || containsCommon || isSimpleSequence)
                return PasswordStrengthLevel.Weak;

            //Medium
            if (length >= 6 && length <= 10 && hasDigit && (hasLower || hasUpper) && groups >= 2)
                return PasswordStrengthLevel.Medium;

            // Strong
            if (length >= 10 && groups == 4 && !containsCommon)
                return PasswordStrengthLevel.Strong;


            // Very Strong (recommended)
            if (length >= 12 && groups >=3 && !containsCommon)
                return PasswordStrengthLevel.VeryStrong;

            return PasswordStrengthLevel.Medium;
        }

    }
}
