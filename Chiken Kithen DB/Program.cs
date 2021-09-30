using System;

namespace Chiken_Kithen_DB
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
                RecipeBook recipes = new RecipeBook(applicationContext);
                recipes.ShowRecipes();
                //applicationContext.SaveRecipeItems();
                //applicationContext.Database.EnsureDeleted();
            }
        }
    }
}
