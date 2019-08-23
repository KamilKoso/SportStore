using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductRepository repository;

        public NavController(IProductRepository repo)
        {
            repository = repo;
        }

        public PartialViewResult Menu(string category = null, bool horizontalLayout = false)
        {
            if (category != null)
                category = category.Replace("-", " ");  //In view replacing all spaces with - for clean routing, now we need to put back spaces.
            ViewBag.SelectedCategory = category;
            IEnumerable<string> categories = repository.Products
                                            .Select(x => x.Category)
                                            .Distinct()
                                            .OrderBy(x => x);
            
            return PartialView("FlexMenu", categories);
        }
       
    }
}