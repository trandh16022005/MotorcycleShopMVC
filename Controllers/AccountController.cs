using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Models;
using MotorcycleShopMVC.Models.ViewModels;
using System.Security.Claims;

namespace MotorcycleShopMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: /Account/Login
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

                    if (Url.IsLocalUrl(pathAndQuery) &&
                        !pathAndQuery.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase))
                    {
                        returnUrl = pathAndQuery;
                    }
                }
            }

            // Đã đăng nhập rồi
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) ||
                User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Lấy role từ Session
                var role = HttpContext.Session.GetString("UserRole");

                // Nếu Session chưa có thì lấy từ Claims
                if (string.IsNullOrWhiteSpace(role))
                {
                    role = User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.Role)
                        ?.Value;
                }

                // Admin -> Admin Dashboard
                if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Admin");
                }

                // Vendor/User -> Home
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl; return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                model.Password);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Vendor chưa được Admin duyệt
            if (string.Equals(user.Role, "Vendor",
                    StringComparison.OrdinalIgnoreCase)
                && !user.IsApproved)
            {
                ModelState.AddModelError(
                    "",
                    "Tài khoản người bán của bạn đang chờ Admin phê duyệt.");

                ViewBag.ReturnUrl = returnUrl;

                return View(model);
            }

            // =========================
            // COOKIE AUTHENTICATION
            // =========================

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(
                    ClaimTypes.Name,
                    string.IsNullOrWhiteSpace(user.FullName)
                        ? user.Email
                        : user.FullName),

                new Claim(ClaimTypes.Email, user.Email),

                new Claim(
                    ClaimTypes.Role,
                    string.IsNullOrWhiteSpace(user.Role)
                        ? "Customer"
                        : user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(
                        model.RememberMe ? 7 : 1)
                });

            // =========================
            // SESSION 
            // =========================

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFullName", user.FullName ?? "");
            HttpContext.Session.SetString("UserRole", user.Role ?? "Customer");

            // Ưu tiên quay về trang cũ
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Admin
            if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Admin");
            }

            // Vendor / Customer
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            // Nếu chưa có returnUrl thì lấy từ Referer
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

            // Đã login
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")) ||
                User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;

            // Mặc định role là Customer (Người mua)
            return View(new RegisterViewModel { Role = "customer" });
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model,
            string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // VALIDATE ROLE SERVER-SIDE (chỉ cho phép customer hoặc vendor)
            var normalizedRole = string.Equals(model.Role, "vendor")
                ? "vendor"
                : "customer"; // default to customer for any other value (defensive)

            // =========================
            // PASSWORD STRENGTH
            // =========================

            var strength = EvaluatePasswordStrength(model.Password);

            if (strength == PasswordStrengthLevel.Weak)
            {
                ModelState.AddModelError(
                    "Password",
                    "Mật khẩu quá yếu. Sử dụng ít nhất 6 ký tự và kết hợp chữ hoa, chữ thường, số và ký tự đặc biệt.");

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // =========================
            // UNIQUE EMAIL
            // =========================

            var existedEmail = await _context.Users
                .AnyAsync(u => u.Email == model.Email);

            if (existedEmail)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // =========================
            // UNIQUE PHONE
            // =========================

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                var existedPhone = await _context.Users
                    .AnyAsync(u => u.PhoneNumber == model.PhoneNumber);

                if (existedPhone)
                {
                    ModelState.AddModelError(
                        "PhoneNumber",
                        "Số điện thoại đã tồn tại");

                    ViewBag.ReturnUrl = returnUrl;
                    return View(model);
                }
            }

            // =========================
            // CREATE USER
            // =========================

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,

                PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber)
        ? null
        : model.PhoneNumber,

                Address = string.IsNullOrWhiteSpace(model.Address)
        ? null
        : model.Address,

                Role = normalizedRole,

                IsApproved =
        normalizedRole.Equals(
            "Customer",
            StringComparison.OrdinalIgnoreCase),

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            if (normalizedRole == "vendor")
            {
                TempData["Success"] =
                    "Tài khoản người bán đã được tạo và đang chờ Admin phê duyệt.";

                return RedirectToAction(nameof(Login));
            }

            // =========================
            // COOKIE AUTH
            // =========================

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    string.IsNullOrWhiteSpace(user.FullName)
                        ? user.Email
                        : user.FullName),

                new Claim(ClaimTypes.Email, user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role ?? "customer")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });

            // =========================
            // SESSION
            // =========================

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserFullName", user.FullName ?? "");
            HttpContext.Session.SetString("UserRole", user.Role ?? "customer");

            // Quay lại trang cũ
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Clear Session
            HttpContext.Session.Clear();

            // Clear Cookie Auth
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // =========================
        // PASSWORD STRENGTH
        // =========================

        private enum PasswordStrengthLevel
        {
            Weak = 1,
            Medium = 2,
            Strong = 3,
            VeryStrong = 4
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

            string lower = password.ToLowerInvariant();

            string[] common =
            {
                "123456",
                "password",
                "qwerty",
                "111111",
                "abc123",
                "admin",
                "iloveyou"
            };

            bool containsCommon =
                common.Any(c => lower.Contains(c));

            bool isSimpleSequence =
                "0123456789abcdefghijklmnopqrstuvwxyz".Contains(lower);

            int groups =
                (hasLower ? 1 : 0) +
                (hasUpper ? 1 : 0) +
                (hasDigit ? 1 : 0) +
                (hasSpecial ? 1 : 0);

            // Weak
            if (length < 6 ||
                groups <= 1 ||
                containsCommon ||
                isSimpleSequence)
            {
                return PasswordStrengthLevel.Weak;
            }

            // Medium
            if (length >= 6 &&
                length <= 10 &&
                hasDigit &&
                (hasLower || hasUpper) &&
                groups >= 2)
            {
                return PasswordStrengthLevel.Medium;
            }

            // Strong
            if (length >= 10 &&
                groups == 4 &&
                !containsCommon)
            {
                return PasswordStrengthLevel.Strong;
            }

            // Very Strong
            if (length >= 12 &&
                groups >= 3 &&
                !containsCommon)
            {
                return PasswordStrengthLevel.VeryStrong;
            }

            return PasswordStrengthLevel.Medium;
        }
    }
}