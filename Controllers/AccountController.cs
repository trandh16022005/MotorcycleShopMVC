using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Models;
using MotorcycleShopMVC.Models.ViewModels;
using System.Security.Claims;
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
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.FullName) ? user.Email : user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(user.Role) ? "Customer" : user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(model.RememberMe ? 7 : 1)
                });

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

            // Unique email
            var existedEmail = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (existedEmail)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.FullName) ? user.Email : user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "Customer")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });

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
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        //private enum PasswordStrengthLevel
        //{
        //    Weak=1,
        //    Medium=2,
        //    Strong=3,
        //    VeryStrong=4
        //}

        //private static PasswordStrengthLevel EvaluatePasswordStrength(string password)
        //{
        //    if (string.IsNullOrWhiteSpace(password))
        //        return PasswordStrengthLevel.Weak;

        //    int length = password.Length;
        //    bool hasLower = password.Any(char.IsLower);
        //    bool hasUpper = password.Any(char.IsUpper);
        //    bool hasDigit = password.Any(char.IsDigit);
        //    bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        //    //very common predictable patterns
        //    string lower = password.ToLowerInvariant();
        //    string[] common = { "123456", "passoword", "qwerty", "111111", "abc123", "admin", "iloveyou" };
        //    bool containsCommon = common.Any(c => lower.Contains(c));

        //    //simple sequence check
        //    bool isSimpleSequence = "0123456789abcdefghijklmnopqrstuvwxyz".Contains(lower) || "abcdefghijklmnopqrstuvwxyz".Contains(lower);

        //    int groups = (hasLower ? 1 : 0) + (hasUpper ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSpecial ? 1 : 0);

        //    //Weak
        //    if (length < 6 || groups <= 1 || containsCommon || isSimpleSequence)
        //        return PasswordStrengthLevel.Weak;

        //    //Medium
        //    if (length >= 6 && length <= 10 && hasDigit && (hasLower || hasUpper) && groups >= 2)
        //        return PasswordStrengthLevel.Medium;

        //    // Strong
        //    if (length >= 10 && groups == 4 && !containsCommon)
        //        return PasswordStrengthLevel.Strong;


        //    // Very Strong (recommended)
        //    if (length >= 12 && groups >=3 && !containsCommon)
        //        return PasswordStrengthLevel.VeryStrong;

        //    return PasswordStrengthLevel.Medium;
        //}

    }
}
