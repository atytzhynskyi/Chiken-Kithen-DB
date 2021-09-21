using System;
using System.Collections.Generic;
using System.Text;

namespace Chiken_Kithen_DB
{
    class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public Ingredient() { }
        public Ingredient(string _Name, int _Count, int _Id)
        {
            Id = _Id;
            Name = _Name;
            Count = _Count;
        }
        public Ingredient(string _Name, int _Count) {
            Name = _Name;
            Count = _Count;
        }
        public Ingredient(string _Name)
        {
            Name = _Name;
            Count = 0;
        }
    }
}
