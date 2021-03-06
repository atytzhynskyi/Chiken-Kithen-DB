using AdvanceClasses;
using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandsModule
{
    public class Table : ICommand
    {
        public string FullCommand { get; private set; }
        public string CommandType { get; private set; }
        public string Result { get; private set; }
        public bool IsAllowed { get; set; }

        private Accounting accounting { get; set; }
        private Kitchen kitchen { get; set; }
        private Hall hall { get; set; }
        private readonly bool _isPooled;
        private readonly bool _isRecommend;

        public List<Buy> Buys = new List<Buy>();

        private List<Customer> _Customers = new List<Customer>();
        private List<Food> _Orders = new List<Food>();

        private double _budgetPool = 0;
        private Dictionary<Customer, double> _donatedForTips = new Dictionary<Customer, double>();

        private Dictionary<Customer, List<Food>> _foodsRecommendedForCustomers { get; set; } = new Dictionary<Customer, List<Food>>();

        public Table(Accounting accounting, Hall hall, Kitchen kitchen, string _FullCommand)
        {
            this.accounting = accounting;
            this.hall = hall;
            this.kitchen = kitchen;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
            IsAllowed = false;
            _isPooled = FullCommand.Split(", ")[1] == "Pooled";

            foreach (var customerName in GetCustomersNameFromCommand())
            {
                Customer searchCustomer = hall.GetCustomer(customerName);
                if (!object.Equals(searchCustomer, null)) _Customers.Add(searchCustomer);
            }

            _isRecommend = HasRecommend();

            if (_isRecommend)
            {
                FillProbableOrders();
            }
            else
            {
                foreach (var foodName in GetFoodsNameFromCommand())
                {
                    Food searchFood = kitchen.Storage.GetRecipe(foodName);
                    if (!object.Equals(searchFood, null)) _Orders.Add(searchFood);
                }
            }

        }

        public void ExecuteCommand()
        {
            if (!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            SetCustomersOrders();

            if (_isRecommend)
            {
                _Customers.ForEach(c => _foodsRecommendedForCustomers.Add(c, GetRecommendedRecipeFoods(c, c.Order)));

                if (_isPooled)
                {
                    SetOptimizationRecommendedRecipeFoodsForCustomers();
                }
                else
                {
                    _Customers.ForEach(c => c.Order = GetTheMostExpensiveRecipeFoods(_foodsRecommendedForCustomers[c]));
                }

            }

            SetResultIfIssues();
            if (!object.Equals(Result, null)) return;

            Buys = FormBuysFromCommand();

            if (_isPooled && _Customers.Any(c => c.budget < accounting.CalculateFoodMenuPrice(
                                                      kitchen.Storage.Recipes, c.Order)))
            {
                if (CheckNeedPooled())
                {
                    ExecuteTablePooled();
                    return;
                }

            }

            double startBudget = accounting.Budget;
            foreach (var buy in Buys)
            {
                buy.ExecuteCommand();
            }

            double moneyAmount = Math.Round(accounting.Budget - startBudget, 2);

            //it's not correct, because "moneyAmount" is our profit after this command
            //amount tax in buy command inside have to equal tax table command 
            //double tax = Math.Round(moneyAmount * accounting.transactionTax, 2);
            //Result = $"success; money amount: {moneyAmount}; tax:{tax}\n" + '{';
            Result = $"success; money amount: {moneyAmount}; tax:XXX\n" + '{';


            Buys.ForEach(b => Result += $"\n\t{b.FullCommand} -> {b.Result}");
            Result += "\n}";
        }

        private List<Food> GetRecommendedRecipeFoods(Customer customer, Food order)
        {
            List<Food> recommendedRecipeFoods = new List<Food>();

            if (order.Name == "Recommend")
            {
                recommendedRecipeFoods = kitchen.Storage.GetRecommendedRecipeFoods(kitchen.Storage.Recipes, order.RecipeIngredients);
                recommendedRecipeFoods = kitchen.Storage.GetRecipeFoodsWithoutAllergy(recommendedRecipeFoods, customer);

                if (!_isPooled)
                {
                    recommendedRecipeFoods = GetRecipeFoodsCustomerCanAfford(recommendedRecipeFoods, customer);
                }

                recommendedRecipeFoods = kitchen.GetRecipeFoodsWithEnoughIngredients(recommendedRecipeFoods);

                return recommendedRecipeFoods;
            }

            recommendedRecipeFoods.Add(order);
            return recommendedRecipeFoods;
        }

        private bool CheckNeedPooled()
        {
            //get a list of customers who there are need to be pooled
            var customersNeedPooled = _Customers.Where(c => c.budget < accounting.CalculateFoodMenuPrice(kitchen.Storage.Recipes, c.Order) && !c.isAllergic(kitchen.Storage.Recipes, c.Order).Item1).ToList();

            if (customersNeedPooled.Count() == 0)
            {
                //all customers have enough money and run standart "table"
                return false;
            }

            return true;
        }

        private void ExecuteTablePooled()
        {
            _Customers.ForEach(c => _donatedForTips.Add(c, 0));

            FillBudgetPool();
            AddMoneyForTips();

            double poolTipAmount = _donatedForTips.Values.Sum();

            double startBudget = accounting.Budget;

            foreach (var buy in Buys)
            {
                var price = accounting.CalculateFoodMenuPrice(kitchen.Storage.Recipes, buy.Food);

                double budgetBefore = 0;

                if (!buy.Customer.isAllergic(kitchen.Storage.Recipes, buy.Customer.Order).Item1)
                {
                    //we made right the budget of customers after all corrected before, this budget 100% left in the customer after buying command

                    //we can pass only the price of food and pool of tip
                    //step by spep (I mean "buy command") we decrease the pool of tip

                    budgetBefore = buy.Customer.budget;
                    buy.Customer.budget = Math.Round(price + poolTipAmount, 2);

                    buy.ExecuteCommand();

                    poolTipAmount = buy.Customer.budget;
                    buy.Customer.budget = budgetBefore;
                }
                else
                {
                    buy.ExecuteCommand();
                }
            }

            //and pool of tips that remain after that, we pay back to customers
            PayBackLeftTips(poolTipAmount);

            double moneyAmount = Math.Round(accounting.Budget - startBudget, 2);

            Result = $"success; money amount: {moneyAmount}; tax:XXX\n" + '{';


            Buys.ForEach(b => Result += $"\n\t{b.FullCommand} -> {b.Result}");
            Result += "\n}";
        }

        private void PayBackLeftTips(double leftTipAmount)
        {
            if (leftTipAmount == 0)
                return;

            double poolTipAmount = _donatedForTips.Values.Sum();

            var customersWithoutAllergy = _Customers.Where(c => !c.isAllergic(kitchen.Storage.Recipes, c.Order).Item1).ToList();
            customersWithoutAllergy.ForEach(c => c.budget = Math.Round(_donatedForTips[c] / poolTipAmount * leftTipAmount, 2));

        }

        private void FillBudgetPool()
        {
            double budgetPoolNeeded = 0;
            var customersWithoutAllergy = _Customers.Where(c => !c.isAllergic(kitchen.Storage.Recipes, c.Order).Item1).ToList();
            customersWithoutAllergy.ForEach(c => budgetPoolNeeded += accounting.CalculateFoodMenuPrice(kitchen.Storage.Recipes, c.Order));

            //get customers with money and have not allergy
            var customersWithMoney = customersWithoutAllergy.Where(c => c.budget > 0).ToList();

            while (budgetPoolNeeded != _budgetPool && customersWithMoney.Count > 0)
            {
                customersWithMoney = customersWithoutAllergy.Where(c => c.budget > 0).ToList();

                if (customersWithMoney.Count == 0)
                {
                    break;
                }

                //find the purest customers
                Customer pureCustomer = new Customer();
                double minBudget = int.MaxValue;
                foreach (var customer in customersWithMoney)
                {
                    if (minBudget >= customer.budget)
                    {
                        pureCustomer = customer;
                        minBudget = customer.budget;
                    }
                }

                //how much money we would take at this iteration?
                var i = (budgetPoolNeeded - _budgetPool) / customersWithMoney.Count > pureCustomer.budget ?
                                                                                        pureCustomer.budget : (budgetPoolNeeded - _budgetPool) / customersWithMoney.Count;

                //pool money from customers budgets to pool
                _budgetPool = Math.Round(_budgetPool + (i * customersWithMoney.Count), 2);
                customersWithMoney.ForEach(c => c.budget = Math.Round(c.budget - i, 2));
            }
        }

        private void AddMoneyForTips()
        {
            var budgetPoolNeeded = 0.0;
            var customersWithoutAllergy = _Customers.Where(c => !c.isAllergic(kitchen.Storage.Recipes, c.Order).Item1).ToList();
            customersWithoutAllergy.ForEach(c => budgetPoolNeeded += accounting.CalculateFoodMenuPrice(kitchen.Storage.Recipes, c.Order));

            var maxTipsAmount = Math.Round(budgetPoolNeeded * accounting.maxTipPercent * 8, 2);

            //get customers with money and have not allergy
            var customersWithMoney = customersWithoutAllergy.Where(c => c.budget > 0).ToList();

            var customersBudgetAmount = 0.0;
            customersWithMoney.ForEach(c => customersBudgetAmount += c.budget);

            maxTipsAmount = maxTipsAmount > customersBudgetAmount ? customersBudgetAmount : maxTipsAmount;

            while (customersWithMoney.Count > 0 && maxTipsAmount > Math.Round(_donatedForTips.Values.Sum(), 2))
            {
                customersWithMoney = customersWithoutAllergy.Where(c => c.budget > 0).ToList();

                Customer pureCustomer = new Customer();
                double minBudget = int.MaxValue;
                foreach (var customer in customersWithMoney)
                {
                    if (minBudget >= customer.budget)
                    {
                        pureCustomer = customer;
                        minBudget = customer.budget;
                    }
                }

                var i = (maxTipsAmount - _donatedForTips.Values.Sum()) / customersWithMoney.Count > pureCustomer.budget ?
                                                                                                        pureCustomer.budget : (maxTipsAmount - _donatedForTips.Values.Sum()) / customersWithMoney.Count;

                //here can lost 1 coin (100 / 3 = 33.3) | 33.3 * 3 = 99.9, we lost 0.01
                //I save exact value of tip
                var exactTip = (maxTipsAmount - _donatedForTips.Values.Sum()) / customersWithMoney.Count > pureCustomer.budget ?
                                                                                                        pureCustomer.budget : maxTipsAmount - _donatedForTips.Values.Sum();
                var donatedForTipsBefore = _donatedForTips.Values.Sum();
                var times = 1;
                foreach (var customer in customersWithMoney)
                {
                    _donatedForTips[customer] = Math.Round(_donatedForTips[customer] + i, 2);
                    customer.budget = Math.Round(customer.budget - i, 2);

                    if (times == customersWithMoney.Count())
                    {
                        if (donatedForTipsBefore + exactTip != _donatedForTips.Values.Sum())
                        {
                            var delta = _donatedForTips.Values.Sum() - exactTip - donatedForTipsBefore;

                            //yes, of course we can add this delta for costomers who has the most money, it'd might more correct but...
                            _donatedForTips[customer] = Math.Round(_donatedForTips[customer] - delta, 2);
                            customer.budget = Math.Round(customer.budget + delta, 2);
                        }
                    }

                    times++;
                }
            }
        }

        private void SetCustomersOrders()
        {
            if (_Customers.Count != _Orders.Count)
            {
                //I dont set result to "FAIL" becouse this method must only set customers orders
                return;
            }
            for (int i = 0; i < _Customers.Count; i++)
            {
                _Customers[i].Order = _Orders[i];
            }
        }

        private List<Buy> FormBuysFromCommand()
        {
            List<Buy> buys = new List<Buy>();


            if (_Customers.Count != _Orders.Count)
            {
                Result = "Customers and Orders counts doesnt equal";
                return buys;
            }

            for (int i = 0; i < _Customers.Count; i++)
            {
                //buys.Add(new Buy(accounting, hall, kitchen, $"Buy, {_Customers[i].Name}, {_Orders[i].Name}"));
                buys.Add(new Buy(accounting, hall, kitchen, $"Buy, {_Customers[i].Name}, {_Customers[i].Order.Name}"));
                buys[i].IsAllowed = true;
            }
            return buys;
        }

        private List<string> GetCustomersNameFromCommand()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> customersName = (List<string>)commandSplit.Where(s => !object.Equals(hall.GetCustomer(s), null)).ToList();
            return customersName;
        }

        private List<string> GetFoodsNameFromCommand()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> foodsName = commandSplit.Where(s => !object.Equals(kitchen.Storage.GetRecipe(s), null)).ToList();
            return foodsName;
        }

        private bool HasRecommend()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            return !object.Equals(commandSplit.FirstOrDefault(s => s == "Recommend"), null);
        }

        private void FillProbableOrders()
        {
            //we fill in "_orders" with fake food as well

            List<string> commandSplit = new List<string>(FullCommand.Split(", "));

            Food fakeFood = null;

            commandSplit.ForEach(s =>
            {
                Food searchFood = kitchen.Storage.GetRecipe(s);

                if (!object.Equals(searchFood, null))
                {
                    if (!object.Equals(fakeFood, null))
                    {
                        _Orders.Add(fakeFood);
                        fakeFood = null;
                    }

                    _Orders.Add(searchFood);
                }
                else if (s == "Recommend")
                {
                    if (!object.Equals(fakeFood, null))
                    {
                        _Orders.Add(fakeFood);
                        fakeFood = null;
                    }

                    fakeFood = new Food("Recommend");
                }
                else if (!object.Equals(fakeFood, null))
                {
                    Ingredient searchIngredient = kitchen.Storage.GetIngredient(s);

                    if (!object.Equals(searchIngredient, null))
                    {
                        fakeFood.RecipeIngredients.Add(searchIngredient);
                    }

                }
                else
                {
                    if (!object.Equals(fakeFood, null))
                    {
                        _Orders.Add(fakeFood);
                        fakeFood = null;
                    }
                }

            });

            if (!object.Equals(fakeFood, null))
            {
                _Orders.Add(fakeFood);
                fakeFood = null;
            }

        }

        private void SetResultIfIssues()
        {
            if (accounting.Budget < 0)
            {
                Result = "RESTAURANT BANKRUPT";
                return;
            }
            if (_Customers.Count > _Orders.Count)
            {
                Result = "FAIL. Every person needs something to eat. So, whole table fails.";
                return;
            }

            if (_Customers.Count < _Orders.Count)
            {
                Result = "FAIL. One person can have one type of food only. So, whole table fails";
                return;
            }

            if (_Customers.Count != _Customers.Distinct().Count())
            {
                Result = "FAIL. One person cant be by one table twice. So, whole table fails.";
                return;
            }

            if (!IsEnoughIngredients())
            {
                Result = "FAIL. We dont have enough ingredients";
                return;
            }

            if (_Customers.Any(c => !c.isAllergic(kitchen.Storage.Recipes, c.Order).Item1 &&
                c.budget < accounting.CalculateFoodMenuPrice(kitchen.Storage.Recipes, c.Order)) && !_isPooled)
            {
                Result = "FAIL. One or more persons dont have enough money";
                return;
            }

        }

        private bool IsEnoughIngredients()
        {
            //form MEGAfood recipe which contain every recipes of foods in orders
            Food megaFood = new Food("");

            foreach (var customer in _Customers)
            {
                if (customer.isAllergic(kitchen.Storage.Recipes, customer.Order).Item1)
                {
                    continue;
                }

                var order = customer.Order;

                megaFood.RecipeFoods.AddRange(
                    order.RecipeFoods.
                    Where(x => true)); //"Where" using to prevent linking to one list

                megaFood.RecipeIngredients.AddRange(
                    order.RecipeIngredients.Where(x => true));
            }

            return kitchen.IsEnoughIngredients(megaFood);
        }

        public List<Food> GetRecipeFoodsCustomerCanAfford(List<Food> listRecipeFoods, Customer customer)
        {
            if (listRecipeFoods.Count == 0)
                return listRecipeFoods;

            List<Food> recommendedRecipeFoods = new List<Food>();

            listRecipeFoods.ForEach(r =>
            {
                if (customer.budget >= accounting.CalculateFoodMenuPrice(
                                kitchen.Storage.Recipes, r))
                {
                    recommendedRecipeFoods.Add(r);
                }
            });

            return recommendedRecipeFoods;
        }

        public Food GetTheMostExpensiveRecipeFoods(List<Food> listRecipeFoods)
        {
            if (listRecipeFoods.Count == 0)
                return null;

            Dictionary<Food, double> recipeFoodsPrice = new Dictionary<Food, double>();

            listRecipeFoods.ForEach(r => recipeFoodsPrice.Add(r, accounting.CalculateFoodMenuPrice(
                            kitchen.Storage.Recipes, r)));

            return recipeFoodsPrice.FirstOrDefault(p => p.Value == recipeFoodsPrice.Values.Max()).Key;
        }

        public void SetOptimizationRecommendedRecipeFoodsForCustomers()
        {
            double poolBudgetCustomers = 0;
            var customersWithoutAllergy = _Customers.Where(c => c.Order.Name == "Recommend" || !c.isAllergic(kitchen.Storage.Recipes, c.Order).Item1).ToList();
            customersWithoutAllergy.ForEach(c => poolBudgetCustomers += c.budget);

            List<List<Dictionary<Customer, Food>>> allProbableCombinationRecipeFoods = GetAllProbableCombinationRecipeFoods();

            Dictionary<int, double> combinationRecipeFoodsPrice = new Dictionary<int, double>();

            for (int i = 0; i < allProbableCombinationRecipeFoods.Count; i++)
            {
                double recipeFoodsPrice = 0;

                allProbableCombinationRecipeFoods[i].ForEach(r => recipeFoodsPrice += accounting.CalculateFoodMenuPrice(
                    kitchen.Storage.Recipes, r.ToList()[0].Value));

                combinationRecipeFoodsPrice.Add(i, recipeFoodsPrice);
            }

            while (combinationRecipeFoodsPrice.Count() > 0)
            {
                var theExapansiveCombination = combinationRecipeFoodsPrice.FirstOrDefault(p => p.Value == combinationRecipeFoodsPrice.Values.Max());
                var idxCombination = theExapansiveCombination.Key;
                var priceCombination = theExapansiveCombination.Value;

                if (priceCombination > poolBudgetCustomers)
                {
                    combinationRecipeFoodsPrice.Remove(idxCombination);
                    continue;
                }

                allProbableCombinationRecipeFoods[idxCombination].ForEach(r =>
                {
                    var res = r.ToList();
                    res.ForEach(t =>
                    {
                        t.Key.Order = t.Value;
                    });

                });

                if (!IsEnoughIngredients())
                {
                    combinationRecipeFoodsPrice.Remove(idxCombination);
                    continue;
                }

                break;
            }

        }

        private List<List<Dictionary<Customer, Food>>> GetAllProbableCombinationRecipeFoods()
        {
            List<List<Dictionary<Customer, Food>>> allProbableRecipeFoods = new List<List<Dictionary<Customer, Food>>>();

            Dictionary<Customer, Food> ProbableRecipeFoods = new Dictionary<Customer, Food>();

            for (int i = 0; i < _foodsRecommendedForCustomers.Count; i++)
            {
                var item = _foodsRecommendedForCustomers.ElementAt(i);

                if (item.Key.Order.Name == "Recommend" || !item.Key.isAllergic(kitchen.Storage.Recipes, item.Key.Order).Item1)
                {
                    allProbableRecipeFoods = AddCombinationRecipeFoods(item.Key, item.Value, allProbableRecipeFoods);
                }
            }

            return allProbableRecipeFoods;
        }

        private List<List<Dictionary<Customer, Food>>> AddCombinationRecipeFoods(Customer customer, List<Food> recipeFoods, List<List<Dictionary<Customer, Food>>> allProbableRecipeFoods)
        {
            List<List<Dictionary<Customer, Food>>> probableRecipeFoodsNew = new List<List<Dictionary<Customer, Food>>>();

            if (allProbableRecipeFoods.Count() == 0)
            {
                foreach (var recipeFood in recipeFoods)
                {
                    Dictionary<Customer, Food> foodRecommended = new Dictionary<Customer, Food>();
                    List<Dictionary<Customer, Food>> listFoodsRecommended = new List<Dictionary<Customer, Food>>();

                    foodRecommended.Add(customer, recipeFood);
                    listFoodsRecommended.Add(foodRecommended);
                    probableRecipeFoodsNew.Add(listFoodsRecommended);
                }
            }
            else
            {
                foreach (var recipeFood in recipeFoods)
                {
                    Dictionary<Customer, Food> foodsRecommended = new Dictionary<Customer, Food>();

                    foodsRecommended.Add(customer, recipeFood);

                    allProbableRecipeFoods.ForEach(r =>
                    {
                        List<Dictionary<Customer, Food>> listFoodsRecommended = new List<Dictionary<Customer, Food>>();
                        listFoodsRecommended.Add(foodsRecommended);
                        listFoodsRecommended.AddRange(r);

                        probableRecipeFoodsNew.Add(listFoodsRecommended);
                    });

                }
            }

            return probableRecipeFoodsNew;
        }
    }
}