using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Diplom.ViewModels;
using Diplom.Models;
using System;
using System.Text;
using System.Security.Cryptography;


namespace Diplom.Controllers
{
    public class AccountController : Controller
    {
        private UserContext db;
        public AccountController(UserContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //Hash
                static byte[] GetHash(string inputString)
                {
                    HashAlgorithm algorithm = SHA256.Create();
                    return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
                }
                static string GetHashString(string inputString)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in GetHash(inputString))
                    sb.Append(b.ToString("X2"));
                    return sb.ToString();
                }
                String salt = GetHashString(GetHashString(model.Password + model.Email));
                

                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == salt);
                if (user != null)
                {
                    await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    //Hash
                    static byte[] GetHash(string inputString)
                    {
                        HashAlgorithm algorithm = SHA256.Create();
                        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
                    }
                    static string GetHashString(string inputString)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (byte b in GetHash(inputString))
                            sb.Append(b.ToString("X2"));
                        return sb.ToString();
                    }
                    String salt = GetHashString(GetHashString(model.Password + model.Email));

                    // добавляем пользователя в бд
                    db.Users.Add(new User { Email = model.Email, Password = salt });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
