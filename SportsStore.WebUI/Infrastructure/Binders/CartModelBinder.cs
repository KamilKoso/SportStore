using SportsStore.Domain.Entities;
using System.Web.Mvc;

namespace SportsStore.WebUI.Infrastructure.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
           
            //pobranie obiektu cart z sesji
            Cart cart = null;
            if(controllerContext.HttpContext.Session[sessionKey]!=null)
            {
                cart = (Cart)controllerContext.HttpContext.Session[sessionKey];     //Jeżeli istnieje ciasteczko sesyjne przypisz go do cart
            }

            //Utworzenie obiektu cart jesli nie zostal odnaleziony w danych sesji
            if (cart == null)
            {
                cart = new Cart();
                if (controllerContext.HttpContext.Session[sessionKey] == null)          
                {
                    controllerContext.HttpContext.Session[sessionKey] = cart;
                }
            }
            return cart;
        }
    }
}