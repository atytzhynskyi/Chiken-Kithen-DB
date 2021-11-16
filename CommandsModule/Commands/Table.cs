﻿using BaseClasses;
using ChikenKithen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsModule
{
    public class Table : Command
    {
        public List<Buy> Buys = new List<Buy>();
        public Table(Hall _Hall, Kitchen _Kitchen, string _FullCommand) : base(_FullCommand)
        {
            Kitchen = _Kitchen;
            Hall = _Hall;
            Buys = FormBuysFromCommand();
        }
        public override void ExecuteCommand()
        {
            if(!IsAllowed)
            {
                Result = "Command not allowed";
                return;
            }

            SetResultIfIssues();
            if(object.Equals(Result, null))
            {
                foreach(Command buy in Buys)
                {
                    buy.ExecuteCommand();
                }
                Result = "success";
            }
        }

        private List<Buy> FormBuysFromCommand()
        {
            List<Buy> buys = new List<Buy>();

            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> customersName = GetCustomersFromCommand();
            List<string> foodsName = GetFoodsFromCommand();

            if (customersName.Count != foodsName.Count)
            {
                return buys;
            }

            for(int i = 0; i < customersName.Count; i++)
            {
                buys.Add(new Buy(Hall, Kitchen, $"Buy, {customersName[i]}, {foodsName[i]}"));
            }
            return buys;
        }
        private List<string> GetCustomersFromCommand()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> customersName = (List<string>)commandSplit.Where(s => Hall.GetCustomer(s).Name != "NULL");
            return customersName;
        }
        private List<string> GetFoodsFromCommand()
        {
            List<string> commandSplit = new List<string>(FullCommand.Split(", "));
            List<string> foodsName = (List<string>)commandSplit.Where(s => Kitchen.Storage.GetRecipeByName(s).Name != "NULL");
            return foodsName;
        }
        private void SetResultIfIssues()
        {
            List<string> customers = GetCustomersFromCommand();
            List<string> foods = GetFoodsFromCommand();

            if (customers.Count > foods.Count)
            {
                Result = "ERROR. Every person needs something to eat. So, whole table fails.";
                return;
            }

            if (customers.Count < foods.Count)
            {
                Result = "ERROR. One person can have one type of food only. So, whole table fails";
                return;
            }

            if (customers.Count != customers.Distinct().Count())
            {
                Result = "ERROR. One person cant be by one table twice. So, whole table fails.";
                return;
            }
        }
    }
}