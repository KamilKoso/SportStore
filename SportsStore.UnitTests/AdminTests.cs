using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID=1, Name="P1" },
                new Product{ProductID=2, Name="P2" },
                new Product{ProductID=3, Name="P3"}
            });
            AdminController target = new AdminController(mock.Object);

            //dzialanie
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();
            //assercje
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual(2, result[1].ProductID);
            Assert.AreEqual("P3", result[2].Name);
        }
        
        [TestMethod]
        public void Can_Edit_Product()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[]{
                new Product{ProductID=1,Name="P1"},
                new Product{ProductID=2,Name="P2"},
                new Product{ProductID=3,Name="P3"}
                });

            AdminController target = new AdminController(mock.Object);
            //dzialanie
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 =  target.Edit(3).ViewData.Model as Product; //Rzutowanie przy pomocy as, w przypadku nieudanego rzutowania zwróci nulla rzutowanie (x) w przypadku nieudanego rzutowania zwroci wyjatek.
            //assercje
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Non_Existinting_Product()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(
                new Product[]{
                new Product{ProductID=1,Name="P1"},
                new Product{ProductID=2,Name="P2"},
                new Product{ProductID=3,Name="P3"}
                });

            AdminController target = new AdminController(mock.Object);
            //działanie
            Product result = (Product)target.Edit(4).ViewData.Model;    //Zapytanie LinQ FirstOrDefault nie odnajduje w repository produktu majacego ID == 4 więc zwraca nulla

            //assercje
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            //dzialanie
            ActionResult result = target.Edit(product);
            //assercje
            mock.Verify(m => m.SaveProduct(product), Times.Once);       //Sprawdzanie czy metoda SaveProduct zostala wywolana tylko raz
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));     //Sprawdzanie typu zwracanego z metody
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };
            target.ModelState.AddModelError("error", "error");      //dodanie bledu do stanu modelu
            //dzialanie
            ActionResult result = target.Edit(product);         //proba zapisania produktu
            //assercje
            mock.Verify(m => m.SaveProduct(product), Times.Never);      //Metoda saveproduct nie powinna zostac wywolana ani razu
            Assert.IsInstanceOfType(result, typeof(ViewResult));        //sprawdzenie typu zwaracnego  z  metody. ModelState.IsValid ma wartosc false wiec powinnismy wpasc do else
        }
        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            //przygotowanie
            Product prod = new Product { ProductID = 2, Name = "Test" };
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID=1,Name="P1"},
                prod,
                new Product{ProductID=3,Name="P3"}
            });


            //działanie
            AdminController target = new AdminController(mock.Object);
            target.Delete(prod.ProductID);
            //assercje
            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
    }

    
}

