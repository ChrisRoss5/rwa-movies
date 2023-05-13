using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.Models;
using System.Diagnostics;

namespace RwaMovies.Controllers
{
    public class HomeController : Controller
    {
        private readonly RwaMoviesContext _dbContext;

        public HomeController(RwaMoviesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[controller]/[action]")]
        public ActionResult<bool> TestConnection()
        {
            return _dbContext.Database.CanConnect();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}