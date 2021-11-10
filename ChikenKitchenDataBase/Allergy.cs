using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BaseClasses
{
    public class Allergy
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public int IngredientId { get; set; }
        [ForeignKey("IngedientId")]
        public Ingredient Ingredient { get; set; }
        public Allergy() { }
        public Allergy(Customer _Customer, Ingredient _Ingredient)
        {
            Customer = _Customer;
            Ingredient = _Ingredient;
        }
    }
}
