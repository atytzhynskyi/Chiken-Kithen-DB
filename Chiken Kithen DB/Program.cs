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
