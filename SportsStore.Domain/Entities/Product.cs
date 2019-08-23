using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace SportsStore.Domain.Entities
{
   public class Product
    {
        [HiddenInput(DisplayValue=false)]
        public int ProductID { get; set; }          //Nazwa klasy+ID powoduje ze EntityFramework identyfikuje takie pole jako Klucz w bazie danych

        [Required(ErrorMessage ="Proszę podać nazwę produktu")]
        [Display(Name="Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Proszę podać opis produktu")]
        [DataType(DataType.MultilineText), Display(Name="Opis")]
        public string Description { get; set; }

        [Required]
        [Range(0.01,double.MaxValue,ErrorMessage ="Proszę podać dodatnią cenę")]
        [Display(Name="Cena")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="Proszę podać kategorię produktu")]
        [Display(Name="Kategoria")]
        public string Category { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}
