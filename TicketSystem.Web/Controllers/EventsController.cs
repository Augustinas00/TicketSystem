using Microsoft.AspNetCore.Mvc;
using TicketSystem.Web.Models;
using TicketSystem.Web.Services;

namespace TicketSystem.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId, string? city)
        {
            var events = await _apiService.GetEventsAsync(search, categoryId, city);
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.City = city;
            return View(events);
        }

        public async Task<IActionResult> Details(int id)
        {
            var evt = await _apiService.GetEventAsync(id);
            if (evt == null) return NotFound();
            return View(evt);
        }

        [HttpPost]
        public async Task<IActionResult> Buy(int id, int row, int seat)
        {
            await _apiService.BuyTicketAsync(id, row, seat);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}