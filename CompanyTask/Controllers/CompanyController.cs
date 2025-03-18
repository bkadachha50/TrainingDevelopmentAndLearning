using Microsoft.AspNetCore.Mvc;
using CompanyTask.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CompanyTask.Controllers
{
    public class CompanyController : Controller
    {
        private const string CookieLoggedInUser = "LoggedInUser";
        private const string CookieCompanyList = "CompanyList";

        private bool IsUserLoggedIn() => Request.Cookies[CookieLoggedInUser] != null;

        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var companies = GetCompaniesFromCookies();
            return View(companies.Where(c => !c.IsDeleted).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Create(CompanyModel model)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var companies = GetCompaniesFromCookies();

                model.Id = companies.Any() ? companies.Max(c => c.Id) + 1 : 1;
                companies.Add(model);
                SetCompaniesToCookies(companies);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var companies = GetCompaniesFromCookies();
            var company = companies.FirstOrDefault(c => c.Id == id);

            return company == null ? NotFound() : View(company);
        }

        [HttpPost]
        public IActionResult Edit(CompanyModel model)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var companies = GetCompaniesFromCookies();
                var company = companies.FirstOrDefault(c => c.Id == model.Id);

                if (company == null) return NotFound();

                company.CompanyName = model.CompanyName;
                company.Startdate = model.Startdate;
                company.IsActive = model.IsActive;

                SetCompaniesToCookies(companies);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            var companies = GetCompaniesFromCookies();
            var company = companies.FirstOrDefault(c => c.Id == id);

            if (company == null) return NotFound();

            company.IsDeleted = true;
            company.IsActive = false;

            SetCompaniesToCookies(companies);
            return RedirectToAction("Index");
        }

        private List<CompanyModel> GetCompaniesFromCookies()
        {
            var cookie = Request.Cookies[CookieCompanyList];
            return string.IsNullOrEmpty(cookie)
                ? new List<CompanyModel>()
                : JsonSerializer.Deserialize<List<CompanyModel>>(cookie);
        }

        private void SetCompaniesToCookies(List<CompanyModel> companies)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append(CookieCompanyList, JsonSerializer.Serialize(companies), options);
        }
    }
}