using BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
        public IngredientProperties() { }
        public IngredientProperties(Ingredient _Ingredient, int _Count, int _Price)
        {
            Ingredient = _Ingredient;
            Count = _Count;
            Price = _Price;
        }
        public IngredientProperties(Ingredient _Ingredient, int _Count)
        {
            Ingredient = _Ingredient;
            Count = _Count;
        }
    }
}
