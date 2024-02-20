using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using baking_website.Models;
using baking_website.ViewModels;

namespace baking_website.Controllers;

public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;
    private readonly ICategoryRepository _categoryRepository;

    public PieController(IPieRepository pieRepository, ICategoryRepository categoryRepository)
    {
        _pieRepository = pieRepository;
        _categoryRepository = categoryRepository;
    }

    public IActionResult List()
    {
        ViewBag.CurrentCategory = "Cheese Cakes";
        return View(new PieListViewModel(_pieRepository.AllPies, "Cheese Cakes"));
    }

}
