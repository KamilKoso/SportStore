using System;
using System.Collections.Generic;
using System.Linq;

namespace SportsStore.Domain.Entities
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public void AddItem(Product product, int quanity)
        {
            CartLine line = lineCollection.Where(p => p.Product.ProductID == product.ProductID).FirstOrDefault();   //Sprawdzenie czy w naszej kolekcji lineCollection
                                                                                                                    //Wystepuje już product, jesli występuje dodajemy tylko
            if(line==null)                                                                                          //quanity w else, jesli nie wystepuje metoda firstordefault
            {                                                                                                       //zwraca null a nastepnie w ifie dodajemy produkt do kolekcji
                lineCollection.Add(new CartLine { Product = product, Quantity = quanity });
            }
            else { line.Quantity += quanity;
            }
        }

        public void RemoveLine(Product product)
        {
            lineCollection.RemoveAll(p => p.Product.ProductID == product.ProductID);
        }

        public decimal ComputeTotalValue()
        {
            return lineCollection.Sum(e => e.Product.Price * e.Quantity);
        }

        public void Clear()
        {
            lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines{ get { return lineCollection; } }
    }

    public class CartLine
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
