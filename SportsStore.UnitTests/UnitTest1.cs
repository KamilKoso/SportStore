using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
using System.Web.Mvc;


namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {

            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name="P1"},
                new Product {ProductID=2, Name="P2" },
                new Product {ProductID=3, Name="P3"},
                new Product {ProductID=4,Name="P4"},
                new Product {ProductID=5, Name="P5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //dzialanie
            ProductListViewModel result = (ProductListViewModel)controller.List(null,2).Model;

            //Assercje
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }


        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //Przygotowanie
            HtmlHelper myHelper = null;
            PagingInfo pagingInfo = new PagingInfo { CurrentPage = 2, ItemsPerPage = 10, TotalItems = 28 };
            //Definiowanie delegata
            Func<int, string> pageURLDelegate = i => "Strona " + i;
            //Działanie
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageURLDelegate);
            //assercje
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Strona 1"">1</a>" +
                            @"<a class=""btn btn-default btn-primary selected"" href=""Strona 2"">2</a>" +
                            @"<a class=""btn btn-default"" href=""Strona 3"">3</a>", result.ToString());
        }
        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //przygotowanie
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name="P1"},
                new Product {ProductID=2, Name="P2" },
                new Product {ProductID=3, Name="P3"},
                new Product {ProductID=4,Name="P4"},
                new Product {ProductID=5, Name="P5"}
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            //dzialanie
            ProductListViewModel result = (ProductListViewModel)controller.List(null,2).Model;
            //assercje
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //przygotowanie
            //Imitacja repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                    new Product {ProductID=1, Name="P1", Category="Cat1"},
                    new Product {ProductID=2, Name="P2", Category="Cat2" },
                    new Product {ProductID=3, Name="P3", Category="Cat1"},
                    new Product {ProductID=4, Name="P4", Category="Cat2"},
                    new Product {ProductID=5, Name="P5", Category="Cat3"}
            });
            //Utworzenie kontrolera wraz z przekazaniem imitacji obiektu
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //dzialanie
            Product[] result = ((ProductListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();
            //assercje
            Assert.AreEqual(result.Length, 2); ;
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //przygotowanie  imitacja obiektu
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                     new Product{ProductID=1, Name="P1", Category="Jabłka"},
                     new Product{ProductID=2, Name="P2", Category="Jabłka"},
                     new Product{ProductID=3, Name="P3", Category="Śliwki"},
                     new Product{ProductID=4, Name="P4", Category="Pomarańcze"}
                });
            //Utworzenie obiektu navcontrol
            NavController target = new NavController(mock.Object);

            //działanie
            string[] result = ((IEnumerable<string>)target.Menu().Model).ToArray();

            //assercje
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0], "Jabłka");
            Assert.AreEqual(result[1], "Pomarańcze");
            Assert.AreEqual(result[2], "Śliwki");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //przygotowanie
            //Imitacja repozytorium
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                     new Product{ProductID=1, Name="P1", Category="Jabłka"},
                     new Product{ProductID=4, Name="P4", Category="Pomarańcze"}
                });
            //Utworzenie instancji kontrolera i kategorii do wyłuskania
            NavController target = new NavController(mock.Object);
            string selectedCategory = "Jabłka";

            //dzialanie
            string result =target.Menu(selectedCategory).ViewBag.SelectedCategory;
            //assercje
            Assert.AreEqual(selectedCategory, result);
        }
    }
}
