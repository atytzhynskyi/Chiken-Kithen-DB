using BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvanceClasses
{
    public class Accounting
    {
        public double Budget { get; private set; }
        public double CollectedTax { get; private set; } = 0;
        public double CollectedTip { get; private set; } = 0;
        public Dictionary<Ingredient, int> IngredientsPrice { get; set; } = new Dictionary<Ingredient, int>();

        public readonly double transactionTax;
        public readonly double dailyTax;
        public readonly double tipTax;
        public readonly double wasteTax;

        public readonly double marginProfit;
        public readonly double maxTipPercent;
        
        readonly double startBudget;

        public Accounting(double _Budget, int _transactionTax, int _marginProfit, int _dailyTax, int _tipTax, int _maxTipPercent, int _wasteTax, Dictionary<Ingredient, int> IngredientsPrice)
        {
            Budget = _Budget;
            startBudget = _Budget;
            transactionTax = Math.Round((float)_transactionTax/100,2);
            marginProfit = Math.Round((float)_marginProfit /100, 2);
            dailyTax = Math.Round((float)_dailyTax /100, 2);
            tipTax = Math.Round((float)_tipTax / 100, 2);
            maxTipPercent = Math.Round((float)_maxTipPercent / 100, 2);
            this.IngredientsPrice = IngredientsPrice;
        }
        public Accounting(double _Budget, double _transactionTax, double _marginProfit, double _dailyTax, double _tipTax, double _maxTipPercent, double _wasteTax, Dictionary<Ingredient, int> IngredientsPrice)
        {
            Budget = _Budget;
            startBudget = _Budget;
            transactionTax = _transactionTax;
            marginProfit = _marginProfit;
            dailyTax = _dailyTax;
            maxTipPercent = _maxTipPercent;
            tipTax = _tipTax;
            wasteTax = _wasteTax;
            this.IngredientsPrice = IngredientsPrice;
        }

        public double CalculateFoodCostPrice(List<Food> Recipes, Food food)
        {
            double price = 0;

            if (Recipes.Any(r=>r.Name == food.Name))
            {
                food = Recipes.Find(r => r.Name == food.Name);
            }
            
            foreach (Food foodRecipe in food.RecipeFoods)
            {
                price += CalculateFoodCostPrice(Recipes, foodRecipe);
            }

            foreach (Ingredient ingredient in food.RecipeIngredients)
            {
                price += IngredientsPrice[IngredientsPrice.Keys.Where(i => i.Name == ingredient.Name).First()];
            }

            return Math.Round(price,2);
        }


        public double CalculateFoodMenuPrice(List<Food> Recipes, Food food)
        {
            return Math.Round(CalculateFoodCostPrice(Recipes, food) * (1 + marginProfit), 2);
        }
        public void PayDayTax()
        {
            Budget = Math.Round(Budget - CalculateDailyTax(),2);
        }
        public double CalculateDailyTax()
        {
            //double profit = Budget - startBudget - CollectedTax;

            //CollectedTax doesn't matter because we added amount to budget without tax when we have "buy command"
            //and added amount to budget with tax when we have "order command"
            //Therefore, when we subtract budget before and after we get a profit
            //in the other words after "buy" we have new budget without needed to pay a tax
            //and after "order" our budget decrease and new budget without needed to pay a tax too
            //and we get the same conclusion - a profit it's subtract a budget before and after
            //Tips we cannot include in profit for daily tax
            double profit = Budget - startBudget - CollectedTip;
            double dailyTax = profit * this.dailyTax;

            if (dailyTax <= 0) return 0;

            return Math.Round(dailyTax, 2);
        }
        public void UseMoney(double amount)
        {
            Budget -= Math.Round(amount + CalculateTransactionTax(amount), 2);
            //CollectedTax += Math.Round(CalculateTransactionTax(amount), 2);
            Budget = Math.Round(Budget, 2);
        }
        public void AddMoney(double amount)
        {
            Budget = Math.Round(Budget + amount - CalculateTransactionTax(amount), 2);
            //CollectedTax = Math.Round(CollectedTax + CalculateTransactionTax(amount), 2);
        }
        public void AddMoneyWithoutTax(double amount)
        {
            Budget += amount;
            Budget = Math.Round(Budget, 2);
        }
        public void AddTip(double amount)
        {
            CollectedTip += amount;
            CollectedTip = Math.Round(CollectedTip, 2);

            Budget += amount - CalculateTipTax(amount);
            Budget = Math.Round(Budget, 2);
        }

        public void UseMoneyWithoutTax(double amount)
        {
            Budget -= amount;
            Budget = Math.Round(Budget, 2);
        }
        public void SetMoney(double amount)
        {
            Budget = amount;
        }
        public double CalculateTransactionTax(double amount)
        {
            return Math.Round(amount * transactionTax, 2);
        }

        public double CalculateTipTax(double amount)
        {
            return Math.Round(amount * tipTax, 2);
        }
        public double GetStartBudget()
        {
            return startBudget;
        }

        public bool isTip()
        {
            Random rnd = new Random();
            int tips = rnd.Next(2);

            return tips == 0 ? false : true;
        }

        private double GetTipPercent()
        {
            if (maxTipPercent <= 0)
            {
                return 0;
            }

            Random rnd = new Random();
            var tipPercent = rnd.NextDouble() * maxTipPercent;

            return tipPercent;
        }

        public double GetTip(double price)
        {
            return Math.Round(price * GetTipPercent(), 2);
        }
    }
}
