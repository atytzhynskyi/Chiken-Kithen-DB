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
                    Console.WriteLine("What you prefer to do?\n 1.Get order and serve\n 2.Show all recipe\n 3.Show all Ingredient\n 4.Add new ingredient\n 5.Add new recipe\n 6.Close program\n");
                    int input = Convert.ToInt32(Console.ReadLine());
                    if (input == 6) break;
                    switch (input)
                    {
                        case 1:
                            Console.WriteLine("Please enter name and order");
                            string inputStr = Console.ReadLine();
                            while (inputStr != "")
                            {
                                string name = inputStr.Split(", ")[0];
                                if (hall.isNewCustomer(name))
                                {
                                    continue;
                                }
                                Customer customer = hall.GetCustomer(name);
                                Food food = new Food(inputStr.Split(", ")[1]);
                                if(!kitchen.RecipeBook.Recipes.Any(f => f.Name == food.Name))
                                {
                                    continue;
                                }
                                Console.Write(name + " - " + food.Name + ": ");
                                customer.SetOrder(kitchen.RecipeBook.Recipes, food);
                                inputStr = Console.ReadLine();
                            }
                            foreach (Customer customer in from Customer customer in hall.AllCustomers.Customers
                                                          where object.Equals(customer.Order, null)
                                                          select customer)
                            {
                                if (customer.isAllergic(kitchen.RecipeBook.Recipes, customer.Order).Item1)
                                {
                                    Console.Write("Can't order: allergic to: " + customer.isAllergic(kitchen.RecipeBook.Recipes, customer.Order).Item2);
                                    continue;
                                }
                                if (!kitchen.isEnoughIngredients(customer.Order))
                                {
                                    Console.WriteLine("Can't order: dont have enough ingredients");
                                    continue;
                                }
                                kitchen.Cook(customer.Order);
                                hall.GiveFood(kitchen, customer);
                                Console.Write("success\n");
                            }
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
