using Microsoft.AspNetCore.Mvc;
using TicketSystem.Web.Models;
using TicketSystem.Web.Services;

namespace TicketSystem.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _apiService;

        public AdminController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var token = await _apiService.LoginAsync(email, password);
            HttpContext.Session.SetString("Token", token);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetEventsAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event evt)
        {
            if (!ModelState.IsValid)
            {
                return View(evt); // Return to form with validation errors
            }

            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "Please log in again.");
                return View(evt);
            }

            try
            {
                await _apiService.CreateEventAsync(evt, token);
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Failed to save event: {ex.Message}");
                return View(evt);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            await _apiService.DeleteEventAsync(id, token);
            return RedirectToAction("Index");
        }
    }
}