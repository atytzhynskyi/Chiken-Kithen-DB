using BaseClasses;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChikenKitchenDataBase
{
    [Table("IngredientsProperties")]
    public class IngredientProperties
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        [ForeignKey("IngredientId")]
        public Ingredient Ingredient { get; }
        public int Count { get; set; }
        [Column("Price")]
        public int Price { get; set; }
        [Column("Trash")]
        public int Trash { get; set; }
        public IngredientProperties() { }
        public IngredientProperties(Ingredient _Ingredient, int _Count, int _Price, int trash)
        {
            Ingredient = _Ingredient;
            Count = _Count;
            Price = _Price;
            Trash = trash;
        }
        public IngredientProperties(Ingredient _Ingredient, int _Count)
        {
            Ingredient = _Ingredient;
            Count = _Count;
        }
    }
}
