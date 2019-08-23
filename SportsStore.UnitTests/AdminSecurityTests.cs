using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;
using System.Web.Mvc;


namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {
            //przygotowanie
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "admin")).Returns(true);

            LoginViewModel model = new LoginViewModel { UserName = "admin", Password = "admin" };

            AccountController target = new AccountController(mock.Object);
            //dzialanie
            ActionResult result = target.Login(model, "/MyUrl");
            //assercje
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credentials()
        {
            //przygotowanie
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("wrongLogin", "wrongPassword")).Returns(false);

            LoginViewModel model = new LoginViewModel { UserName = "wrongLogin", Password = "wrongPassword" };

            AccountController target = new AccountController(mock.Object);
            //dzialanie
            ActionResult result = target.Login(model, "/MyUrl");

            //assercje
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
