using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;


namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //przygotowanie
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();
            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            //assercje
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }


        [TestMethod]
        public void Can_Add_Quanity_For_Existing_Lines()
        {
            //przygotowanie
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();
            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //assercje
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Lines()
        {
            //przygotowanie
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();
            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p2, 10);
            target.AddItem(p1, 5);
            target.RemoveLine(p2);

            //assercje
            Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(target.Lines.Count(), 1);
        }

        [TestMethod]
        public void Can_Compute_Total_Cart_Value()
        {
            //przygotowanie
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 50M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 10M };

            Cart target = new Cart();
            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p2, 2);
            decimal result = target.ComputeTotalValue();

            //Assercje
            Assert.AreEqual(result, 80M);
        }

        [TestMethod]
        public void Can_Clear_Cart_Content()
        {
            //przygotowanie
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();
            //dzialanie
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p2, 4);
            target.Clear();

            //assercje
            Assert.AreEqual(target.Lines.Count(), 0);

        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1", Category="Jab"},
            }.AsQueryable());

            Cart cart = new Cart();
            CartController target = new CartController(mock.Object, null);
            //dzialanie
            target.AddToCart(cart, 1, null);
            //assercje
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID=1, Name="P1", Category="Jab"},
            }.AsQueryable());

            Cart cart = new Cart();
            CartController target = new CartController(mock.Object, null);
            //dzialanie
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");      //Używanie metody akcji AddToCart ma typ RedirectToRouteResult, ten typ w dictionary ma zawsze klucz "action"
            //assercje                                                              //Któremu zawsze odpowiada metoda akcji którą zwraca, pozostałe stringi są kolejnymi kluczami dictionary
            Assert.AreEqual(result.RouteValues["action"],"Index");                  //RedirectToRouteResult ma 2 klucze w dictionary action
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
            
        }
        [TestMethod]
        public void Can_View_Cart_Contenst()
        {
            //przygotownie
            Cart cart = new Cart();
            CartController target = new CartController(null, null);
            //dzialanie
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;
            //assercje
            Assert.AreEqual(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }


        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //przygotowanie
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController target = new CartController(null, mock.Object);
            //dzialanie
            ViewResult result = target.Checkout(cart, shippingDetails);
            //assercje
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());         //sprawdzenie czy zamowienie zostalo przekazane do procesora, jest sprawdzane w tescie tak samo jak Assercje :)
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //przygotowanie
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController target = new CartController(null, mock.Object);
            //dodanie bledu do modelu
            target.ModelState.AddModelError("error", "error");
            //dzialanie
            ViewResult result = target.Checkout(cart, new ShippingDetails());
            //assercje
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);
            // spr czy metoda zwraca domyslny widok 
            Assert.AreEqual("", result.ViewName);
            // spr czy przekazujemy nieprawidlowy model do widoku
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            //przygotowanie
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController target = new CartController(null, mock.Object);

            //dzialanie
            ViewResult result = target.Checkout(cart, new ShippingDetails()); // proba ukonczenia zamowienia
            //assercje
            Assert.AreEqual("Completed", result.ViewName); // czy zwraca widok Completed
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
    }
