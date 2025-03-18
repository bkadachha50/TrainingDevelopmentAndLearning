using Microsoft.AspNetCore.Mvc;
using CompanyTask.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CompanyTask.Controllers
{
    public class AccountController : Controller
    {
        private const string CookieUserList = "UserList";
        private const string CookieLoggedInUser = "LoggedInUser";

        public IActionResult Login()
        {
            if (Request.Cookies.ContainsKey(CookieLoggedInUser))
            {
                return RedirectToAction("Index", "Company");
            }
            return View();
        }

        public IActionResult Register()
        {
            if (Request.Cookies.ContainsKey(CookieLoggedInUser))
            {
                return RedirectToAction("Index", "Company");
            }
            return View();
        }   

        [HttpPost]
        public IActionResult Register(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var userList = GetUsersFromCookies();

                if (userList.Any(u => u.Email.ToLower() == model.Email.ToLower()))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View("Register", model);
                }

                model.Id = userList.Any() ? userList.Max(u => u.Id) + 1 : 1;
                userList.Add(model);
                SetUsersToCookies(userList);

                TempData["Success"] = "Registration successful. Please login.";
                return RedirectToAction("Login");
            }
            return View("Register", model);
        }

        [HttpPost]
        public IActionResult Login(UserModel model)
        {
            var userList = GetUsersFromCookies();
            var user = userList.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase) && u.Password == model.Password);

            if (user != null)
            {
                SetCookie(CookieLoggedInUser, user.Email, 30);
                return RedirectToAction("Index", "Company");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View("Login", model);
        }

        public IActionResult UserList()
        {
            var userList = GetUsersFromCookies();
            return View(userList);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete(CookieLoggedInUser);
            return RedirectToAction("Login");
        }

        private List<UserModel> GetUsersFromCookies()
        {
            var cookie = Request.Cookies[CookieUserList];
            return string.IsNullOrEmpty(cookie) ? new List<UserModel>() : JsonSerializer.Deserialize<List<UserModel>>(cookie);
        }

        private void SetUsersToCookies(List<UserModel> users)
        {
            SetCookie(CookieUserList, JsonSerializer.Serialize(users), 30);
        }

        private void SetCookie(string key, string value, int days)
        {
            var options = new CookieOptions { Expires = DateTime.Now.AddDays(days) };
            Response.Cookies.Append(key, value, options);
        }
    }
}