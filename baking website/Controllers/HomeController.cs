using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using baking_website.Models;
using Microsoft.AspNetCore.Mvc;
using baking_website.ViewModels;

namespace baking_website.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPieRepository _pieRepository;

        public HomeController(IPieRepository pieRepository)
        {
            _pieRepository = pieRepository;
        }

        public IActionResult Index()
        {
            var pieOftheWeek = _pieRepository.PiesOfTheWeek;
            return View(new HomeViewModel(pieOftheWeek) );
        }
    }
}

