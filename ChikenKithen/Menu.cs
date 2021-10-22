using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using BaseClasses;
using CsvHelper;

namespace ChikenKithen
{
    class Menu
    {
        public List<Food> Foods = new List<Food>();
        public List<Food> GetFoods() => Foods;
        public Menu()
        {
        }
        public Menu(List<Food> _Foods)
        {
            Foods.AddRange(_Foods);
        }
        private void ReadFromFile()
        {
            using var streamReader = File.OpenText(@"..\..\..\Ingredients.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);
        }
        public void ShowAll()
        {
            foreach (Food food in Foods)
            {
                Console.WriteLine(food.Name);
            }
        }
    }
}
