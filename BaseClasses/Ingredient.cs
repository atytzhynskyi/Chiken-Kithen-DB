using System.ComponentModel.DataAnnotations.Schema;

namespace BaseClasses
{

    [Table("Ingredients")]
    public class Ingredient
    {
        static void Main(string[] args)
        { }
        [Column("IngredientId")]
        public int Id { get; set; }
        public string Name { get; set; }
        public Ingredient() { }
        public Ingredient(string _Name) {
            Name = _Name;
        }
    }
}
