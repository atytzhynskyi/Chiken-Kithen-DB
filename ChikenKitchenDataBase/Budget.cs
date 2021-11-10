using System;
using System.Collections.Generic;
using System.Text;

namespace ChikenKitchenDataBase
{
    public class Budget
    {
        public int Id { get; set; }
        public int Balance { get; set; }

        public Budget() { }
        public Budget(int _Balance) { Balance = _Balance; }
    }
}
