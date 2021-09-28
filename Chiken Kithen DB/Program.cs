using System;

namespace Chiken_Kithen_DB
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Storage storage = new Storage())
            {
                //storage.AddBaseIngredients();
                //storage.ShowIngredients();
                storage.Database.EnsureDeleted();
                storage.SaveChanges();
            }
            using (RecipeBook recipeBook = new RecipeBook())
            {
                recipeBook.AddBaseRecipe();
                recipeBook.AddNewFood(new Food("Salt chiken", new Ingredient("Chiken", 1)));
                recipeBook.RewriteRecipe(new Food("Salt chiken", new Ingredient("Chiken", 1), new Ingredient("Salt", 1)), "Salt chiken");
                recipeBook.ShowRecipe();
                recipeBook.Database.EnsureDeleted();
                recipeBook.SaveChanges();
            }
            using (CustomerBase customerBase = new CustomerBase())
            {
                customerBase.Database.EnsureDeleted();
                customerBase.Customers.Add(new Customer("Jey Jo"));
                customerBase.SaveChanges();
                //customerBase.FillCustomerList();
                customerBase.ShowCustomers();
                customerBase.Database.EnsureDeleted();
                customerBase.SaveChanges();
            }
        }
    }
}
