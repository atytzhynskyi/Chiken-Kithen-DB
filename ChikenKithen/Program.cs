using BaseClasses;
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
                //applicationContext.Database.EnsureDeleted();
                applicationContext.FillCustomerList();
                applicationContext.AddBaseIngredients();
                applicationContext.AddIngredientCount();
                applicationContext.AddBaseRecipe();
                Storage storage = new Storage(applicationContext.Ingredients.ToList(), applicationContext.GetIngredientsAmount());
                storage.SetDictionaryFromFile();
                RecipeBook recipeBook = new RecipeBook(applicationContext.Recipes.ToList());
                Kitchen kitchen = new Kitchen(storage, recipeBook);
                Menu menu = new Menu(kitchen.RecipeBook.Recipes);
                Hall hall = new Hall(applicationContext.Customers.ToList(), new Menu(kitchen.RecipeBook.Recipes));
                while (2 + 2 != 5)
                {
                    Console.WriteLine("What you prefer to do?\n 1.Get order and serve\n 2.Show all recipe\n 3.Show all Ingredient\n 4.Add new ingredient\n 5.Add new recipe\n 6.Close program");
                    int input = Convert.ToInt32(Console.ReadLine());
                    if (input == 6) break;
                    switch (input)
                    {
                        case 1:
                            Console.WriteLine("Please enter name and order");
                            string inputStr = Console.ReadLine();
                            string name = inputStr.Split(", ")[0];
                            if (hall.isNewCustomer(name))
                            {
                                hall.AddNewCustomer(new Customer(name, hall.AskAllergiesIngredients().ToArray()));
                                applicationContext.SaveChanges();
                            }
                            Customer customer = hall.GetCustomer(name);
                            Food food = new Food(inputStr.Split(", ")[1]);
                            Console.Write(name + " - " + food.Name + ": ");
                            if (customer.isAllergic(kitchen.RecipeBook.Recipes, food).Item1)
                            {
                                Console.Write("Can't order: allergic to: " + customer.isAllergic(kitchen.RecipeBook.Recipes, food).Item2);
                                break;
                            }
                            customer.SetOrder(menu.Foods, food);
                            if (!kitchen.isEnoughIngredients(food))
                            {
                                Console.WriteLine("Can't order: dont have enough ingredients");
                                break;
                            }
                            kitchen.Cook(customer.Order);
                            hall.GiveFood(kitchen, customer);
                            Console.WriteLine("success");
                            break;
                        case 2:
                            recipeBook.ShowRecipes();
                            break;
                        case 3:
                            kitchen.Storage.ShowIngredients();
                            break;
                        case 4:
                            kitchen.Storage.AddNewIngredient();
                            applicationContext.SaveChanges();
                            break;
                        case 5:
                            kitchen.RecipeBook.AddNewRecipe();
                            applicationContext.SaveChanges();
                            break;
                        default:
                            Console.WriteLine("Unknow comand");
                            break;
                    }
                }
                applicationContext.SaveAll(storage.Ingredients, storage.IngredientsAmount, kitchen.RecipeBook.Recipes, hall.AllCustomers.Customers);
                //applicationContext.Database.EnsureDeleted();*/
            }
        }
    }
}
