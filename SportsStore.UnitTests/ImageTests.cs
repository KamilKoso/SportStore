using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Retrive_Image_Data()
        {
            //przygotwanie
            Product prod = new Product      //Tworzenie testowego produktu
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            Mock<IProductRepository> mock = new Mock<IProductRepository>();         //Tworzenie imitacji repozytorium z naszym obiektem testowym oraz 2 innymi
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product{ProductID=1,Name="P1"},
                prod,
                new Product{ProductID=3,Name="P3"}
            });

            ProductController target = new ProductController(mock.Object);

            //dzialanie-wywolanie metody akcji getimage z parametrem ProductId = 2;
            ActionResult result = target.GetImage(2);
            //assercje
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_Retrive_Image_Data_For_Invalid_ID()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();         //Tworzenie imitacji repozytorium
            mock.Setup(p => p.Products).Returns(new Product[]
            {
                new Product{ProductID=1,Name="P1"},
                new Product{ProductID=3,Name="P3"}
            });

            ProductController target = new ProductController(mock.Object);

            //dzialanie-wywolanie metody akcji getimage z parametrem ProductId = 100 którego nie ma w repozytorium;
            ActionResult result = target.GetImage(100);

            //assercje
            Assert.IsNull(result);
        }
    }
}
