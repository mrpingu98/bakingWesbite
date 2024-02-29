using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using baking_website.Models;
using baking_website.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace baking_website.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IPieRepository _pieRepository;
        private readonly IShoppingCart _shoppingCart;

        public ShoppingCartController(IPieRepository pieRepository, IShoppingCart shoppingCart)
        {
            _pieRepository = pieRepository;
            _shoppingCart = shoppingCart;

        }

        public ViewResult Index()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            //this method returns all the shopping cart items for this shopping cart (using the sessions shopping cart id)

            _shoppingCart.ShoppingCartItems = items;
            //we then set the value of the ShoppingCartItems property, of the shoppingCart instance that's been made to use
            //in this class, equal to items
            //so now, the instantiated object we are using of _shoppingCart, has its property of ShoppingCartItems
            //set equal to what was returned in the GetShoppingCartItems() method

            var shoppingCartViewModel = new ShoppingCartViewModel(_shoppingCart, _shoppingCart.GetShoppingCartTotal());

            return View(shoppingCartViewModel);
        }

        public RedirectToActionResult AddToShoppingCart(int pieId)
        {
            var selectedPie = _pieRepository.AllPies.FirstOrDefault(p => p.PieId == pieId);

            if (selectedPie != null)
            {
                _shoppingCart.AddToCart(selectedPie);
            }
            return RedirectToAction("Index");
        }

        //search in pie repo for the pie id
        //if not null, call the AddCart method, passing in the pieId
        //RedirectToAction method - redirect us to the index of the shopping cart


        public RedirectToActionResult RemoveFromShoppingCart(int pieId)
        {
            var selectedPie = _pieRepository.AllPies.FirstOrDefault(p => p.PieId == pieId);

            if (selectedPie != null)
            {
                _shoppingCart.RemoveFromCart(selectedPie);
            }
            return RedirectToAction("Index");
        }
    }
}

