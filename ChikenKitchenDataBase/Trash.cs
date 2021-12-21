
using BaseClasses;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChikenKitchenDataBase
{
    [Table("Trash")]
    public class Trash
    {
        public int Id { get; set; }
        //public int IngredientId { get; set; }
        //[ForeignKey("IngredientId")]
        //public Ingredient Ingredient { get; }
        public int Count{ get; set; }

        public Trash()
        {
        }

        //public Trash(int count)
        //{
        //    Count = count;
        //}
        
    }
}
