using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotorcycleShopMVC.Models;
using MotorcycleShopMVC.Models.ViewModels;
using System.Security.Claims;

namespace MotorcycleShopMVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IWebHostEnvironment? webHostEnvironment;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private int GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            var model = new UserProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Gender = user.Gender,
                BirthDay = user.BirthDay,
                BirthMonth = user.BirthMonth,
                BirthYear = user.BirthYear,
                AvatarPath = user.AvatarPath
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Email = user.Email;
                model.AvatarPath = user.AvatarPath;
                return View(model);
            }

            //Luu avatar moi neu co chon file
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(model.AvatarFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Chỉ chấp nhận file .jpg, .jpeg, .png.");
                    model.Email = user.Email;
                    model.AvatarPath = user.AvatarPath;
                    return View(model);
                }

                if (model.AvatarFile.Length > 1 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFile", "Ảnh không được vượt quá 1MB.");
                    model.Email = user.Email;
                    model.AvatarPath = user.AvatarPath;
                    return View(model);
                }

                var avatarFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatar");
                Directory.CreateDirectory(avatarFolder);

                if (!string.IsNullOrWhiteSpace(user.AvatarPath))
                {
                    var oldFileName = Path.GetFileName(user.AvatarPath);
                    var oldFilePath = Path.Combine(avatarFolder, oldFileName);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var newFileName = $"{user.Id}{extension}";
                var newFilePath = Path.Combine(avatarFolder, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(stream);
                }

                user.AvatarPath = $"/images/avatar/{newFileName}";
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.Gender = model.Gender;
            user.BirthDay = model.BirthDay;
            user.BirthMonth = model.BirthMonth;
            user.BirthYear = model.BirthYear;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }
    }
}