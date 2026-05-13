using ComChienMaDui.Data;
using ComChienMaDui.DTOs;
using ComChienMaDui.Models;
using ComChienMaDui.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ComChienMaDui.Controllers
{
    public class AccountController : Controller
    {
        private readonly EcommerceLaptopContext _context;
        private readonly IEmailService _emailService;
        private readonly IDataProtector _protector;
        private readonly IJwtService _jwtService;

        public AccountController(EcommerceLaptopContext context, IEmailService emailService,
                                 IDataProtectionProvider provider, IJwtService jwtService)
        {
            _context = context;
            _emailService = emailService;
            _protector = provider.CreateProtector("EmailVerificationProtector");
            _jwtService = jwtService;
        }

        // --- PHẦN ĐĂNG KÝ ---
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    IsActive = false,
                    CreatedAt = DateTime.Now,
                };

                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Tạo token bằng cách mã hóa Id người dùng
                string token = _protector.Protect(user.Id.ToString());

                // Tạo link xác nhận có chứa token
                string verificationLink = Url.Action("VerifyEmail", "Account", new { token = token }, Request.Scheme);

                // Gồm HTML chứa link kích hoạt
                string emailBody = $@"
                    <h2>Chào mừng {user.FullName} đến với hệ thống!</h2>
                    <p>Vui lòng click vào nút bên dưới để kích hoạt tài khoản của bạn:</p>
                    <a href='{verificationLink}' style='display:inline-block; padding:10px 20px; background-color:#007bff; color:#ffffff; text-decoration:none; border-radius:5px;'>Kích hoạt tài khoản</a>
                    <p>Hoặc copy đường dẫn này dán vào trình duyệt: <br/> {verificationLink}</p>";

                // Gửi email xác nhận
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Xác nhận kích hoạt tài khoản",
                    emailBody
                );

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng kiểm tra email để kích hoạt tài khoản.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // --- XÁC NHẬN EMAIL ---
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token không hợp lệ.");
            }

            try
            {
                string userIdStr = _protector.Unprotect(token);
                if (int.TryParse(userIdStr, out int userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user == null) return NotFound("Không tìm thấy người dùng.");

                    if (user.IsActive)
                    {
                        TempData["InfoMessage"] = "Tài khoản này đã được kích hoạt từ trước.";
                        return RedirectToAction("Login");
                    }

                    user.IsActive = true;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tài khoản của bạn đã được kích hoạt thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                return BadRequest("Dữ liệu token không hợp lệ.");
            }
            catch (Exception)
            {
                return BadRequest("Đường dẫn kích hoạt không hợp lệ hoặc đã hết hạn.");
            }
        }

        // --- PHẦN ĐĂNG NHẬP ---
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản chưa được kích hoạt.");
                    return View(model);
                }

                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    var token = _jwtService.GenerateToken(user);
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(7)
                    });

                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
            }
            return View(model);
        }

        // --- PHẦN ĐĂNG XUẤT ---
        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction("Index", "Home");
        }
    }
}