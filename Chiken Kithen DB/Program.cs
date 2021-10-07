using ChikenKitchenDataBase;
using System;
using System.Linq;

namespace ChikenKithen
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ApplicationContext applicationContext = new ApplicationContext())
            {
                /*applicationContext.FillCustomerList();
                applicationContext.AddBaseIngredients();
                applicationContext.AddBaseRecipe();*/
                RecipeBook recipes = new RecipeBook(applicationContext.Recipes.ToList());
                recipes.ShowRecipes();
                //applicationContext.Database.EnsureDeleted();
            }
        }
    }
}
