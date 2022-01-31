using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BaseClasses.Tests
{
    [TestClass()]
    public class FoodTests
    {
        Ingredient potatoes = new Ingredient("Potatoes");
        Ingredient tuna = new Ingredient("Tuna");
        Food smashedPotatoes;
        Food fries;
        Food irishFish;

        [TestInitialize]
        public void SetupContext()
        {
            smashedPotatoes = new Food("Smashed Potatoes");
            smashedPotatoes.RecipeIngredients.Add(potatoes);

            fries = new Food("Fries");
            fries.RecipeIngredients.Add(potatoes);

            irishFish = new Food("Irish Fish");
            irishFish.RecipeIngredients.Add(tuna);
            irishFish.RecipeFoods.Add(smashedPotatoes);
            irishFish.RecipeFoods.Add(fries);
        }


        [TestMethod()]
        public void HasIngredientPositiveResult()
        {
            var result = irishFish.HasIngredient(potatoes);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void HasIngredientNegativeResult()
        {
            Ingredient lemon = new Ingredient("Lemon");
            
            var result = irishFish.HasIngredient(lemon);

            Assert.IsFalse(result);
        }

    }
}