using BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChikenKitchenDataBase
{
    class Component
    {
        public int Id { get; set; }
        public int? IngredientId { get; set; }
        [ForeignKey("IngredientId")]
        public Ingredient Ingredient{get; set;}
        public int? FoodId { get; set; }
        [ForeignKey("FoodId")]
        public Food Food { get; set; }
    }
}
