﻿using SportsStore.Domain.Abstract;
using System.Web.Mvc;
using SportsStore.Domain.Entities;
using System.Linq;
using System.Web;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductRepository repository;
        public AdminController(IProductRepository repo)
        {
            repository = repo;
        }
        public ViewResult Index()
        {
            return View(repository.Products);
        }
        public ViewResult Edit(int ProductID)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == ProductID);
            return View(product);
        }
        
        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase image=null)
        {
            if (ModelState.IsValid)
            {
                if(image != null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }
                repository.SaveProduct(product);
                TempData["message"] = string.Format("Zapisano {0}", product.Name);
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        public ViewResult Create()
        {
            return View("Edit", new Product());
        }
        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product deletedProduct = repository.DeleteProduct(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("Usunięto {0}", deletedProduct.Name);
            }
            return RedirectToAction("Index");
        }
    }
}