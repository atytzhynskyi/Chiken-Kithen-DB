using BaseClasses;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChikenKitchenDataBase
{
    [Table("FoodProperties")]
    public class FoodProperties
    {
        public int Id { get; set; }
        public int FoodId { get; set; }
        [ForeignKey("FoodId")]
        public Food Food { get; set; }
        public int Count { get; set; }

        public FoodProperties(Food food, int count)
        {
            Food = food;
            Count = count;
        }

        public FoodProperties(Food food)
        {
            Food = food;
        }

        public FoodProperties()
        {
        }
    }
}
