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
        private bool _isPooled;
        public List<Buy> Buys = new List<Buy>();

        List<Customer> _Customers = new List<Customer>();
        List<Food> _Orders = new List<Food>();

        private double _budgetPool = 0;
        private Dictionary<Customer, double> _donatedForTips = new Dictionary<Customer, double>();

        public Table(Accounting accounting, Hall hall, Kitchen kitchen, string _FullCommand)
        {
            this.accounting = accounting;
            this.hall = hall;
            this.kitchen = kitchen;

            FullCommand = _FullCommand;
            CommandType = FullCommand.Split(", ")[0];
            IsAllowed = false;
            _isPooled = FullCommand.Split(", ")[1] == "Pooled";
            foreach (var foodName in GetFoodsNameFromCommand())
            {
                Food searchFood = kitchen.Storage.GetRecipe(foodName);
                if (!object.Equals(searchFood, null)) _Orders.Add(searchFood);
            }

            foreach (var customerName in GetCustomersNameFromCommand())
            {
                Customer searchCustomer = hall.GetCustomer(customerName);
                if (!object.Equals(searchCustomer, null)) _Customers.Add(searchCustomer);
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

                buy.ExecuteCommand();
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
            _Customers.ForEach(c => c.budget = Math.Round(_donatedForTips[c] / poolTipAmount * leftTipAmount, 2));
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

            var maxTipsAmount = budgetPoolNeeded * accounting.maxTipPercent * 8;

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
                buys.Add(new Buy(accounting, hall, kitchen, $"Buy, {_Customers[i].Name}, {_Orders[i].Name}"));
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

            if (_Customers.Any(c => c.budget < accounting.CalculateFoodMenuPrice(
                                                         kitchen.Storage.Recipes, c.Order)) && !_isPooled)
            {
                Result = "FAIL. One or more persons dont have enough money";
                return;
            }

        }

        private bool IsEnoughIngredients()
        {
            List<string> foodsName = GetFoodsNameFromCommand();

            //form MEGAfood recipe which contain every recipes of foods in orders
            Food megaFood = new Food("");
            foreach (var foodName in foodsName)
            {
                Food searchFood = kitchen.Storage.GetRecipe(foodName); //Get Food by Name
                megaFood.RecipeFoods.AddRange(
                    searchFood.RecipeFoods.
                    Where(x => true)); //"Where" using to prevent linking to one list

                megaFood.RecipeIngredients.AddRange(
                    searchFood.RecipeIngredients.Where(x => true));
            }

            return true;
        }
    }
}
