using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Storage : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public Storage()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=storage;Trusted_Connection=True;");
        }
        public void AddBaseIngredients()
        {
            List<Ingredient> BaseIngredients = new List<Ingredient>();
            string[] BaseIngredientsStr = "Chicken, Tuna, Potatoes, Asparagus, Milk, Honey, Paprika, Garlic, Water, Lemon, Tomatoes, Pickles, Feta, Vinegar, Rice, Chocolate".Split(", ");
            foreach (string i in BaseIngredientsStr)
            {
                BaseIngredients.Add(new Ingredient(i, 10));
            }
            foreach(Ingredient ingredient in BaseIngredients)
            {
                Ingredients.Add(ingredient);
            }
            SaveChanges();
        }
        public void AddNewIngredient(Ingredient ingredient)
        {
            Ingredients.Add(ingredient);
            SaveChanges();
        }
        public void RewriteIngredientCount(Ingredient _ingredient, string chengeIngredientName)
        {
            foreach (Ingredient ingredient in Ingredients)
            {
                if (ingredient.Name == chengeIngredientName)
                {
                    ingredient.Name = _ingredient.Name;
                    ingredient.Count = _ingredient.Count;
                    SaveChanges();
                    return;
                }
            }
        }
        public void DeleteRecipe(string _ingredientName)
        {
            foreach (Ingredient ingredient in Ingredients)
            {
                if (ingredient.Name == _ingredientName)
                {
                    Ingredients.Remove(ingredient);
                    return;
                }
            }
        }
        public void ShowIngredients()
        {
            var ingredients = Ingredients.ToList();
            Console.WriteLine("Ingredients List:");
            foreach(Ingredient ingredient in ingredients)
            {
                Console.WriteLine(ingredient.Id + " " + ingredient.Name + " " + ingredient.Count);
            }
        }
        public void Clear()
        {
            foreach(Ingredient ingredient in Ingredients)
            {
                Ingredients.Remove(ingredient);
            }
            SaveChanges();
        }
    }
}
