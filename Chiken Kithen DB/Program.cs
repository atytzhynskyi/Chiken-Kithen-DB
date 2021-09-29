using System;

namespace Chiken_Kithen_DB
{
    class Program
    {
        static void Main(string[] args)
        {
            using (DataBase dataBase = new DataBase())
            {
                //dataBase.FillCustomerList();
                dataBase.AddBaseIngredients();
                dataBase.AddBaseRecipe();
            }
        }
    }
}
