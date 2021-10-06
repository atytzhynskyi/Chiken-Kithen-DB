using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chiken_Kithen_DB
{
    [Table("Ingredients")]
    public class Ingredient
    {
        [Column("IngredientId")]
        public int Id { get; set; }
        public string Name { get; set; }
        public Ingredient() { }
        public Ingredient(string _Name) {
            Name = _Name;
        }
    }
}
