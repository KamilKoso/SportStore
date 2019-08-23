using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository repository;
        private IOrderProcessor orderProcessor;

        public CartController(IProductRepository repo, IOrderProcessor orderProcessor)
        {
            repository = repo;
            this.orderProcessor = orderProcessor;
        }

        public RedirectToRouteResult AddToCart(Cart cart,int productId, string returnUrl)       //Gdy metoda ta wywolywana jest po raz pierwszy tworzone jest ciasteczko z produktami, nastepnie Cart żądane jest po raz drugi
        {                                                                                       //W widoku index, wtedy ciasteczko juz != null więc cart jest przypisany z ciasteczka sesyjnego i przekazane do metody Index
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
                cart.AddItem(product, 1);           //Zapisanie produktu do sesji dokonuje sie w cartmodelbinder, MVC Sama wyszukuje bindingi i jesli są okreslone używa ich, jesli nie ma używa defaultowego
            return RedirectToAction("Index", new { returnUrl });    //Dokonanie bindingu znajduje sie w pliki Global.asax
        }
        
        public RedirectToRouteResult RemoveFromCart(Cart cart,int productId, string returnUrl)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
                cart.RemoveLine(product);
            return RedirectToAction("Index", new { returnUrl });
        }


        


        public ViewResult Index(Cart cart,string returnUrl)
        {

            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }
        
        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails() );
        }


        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Twój koszyk jest pusty");
            }
            if (ModelState.IsValid)
            {
                orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }
            else { return View(shippingDetails); }
        }

    
    }
}