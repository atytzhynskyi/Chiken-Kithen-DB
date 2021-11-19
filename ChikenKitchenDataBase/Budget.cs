using System;
using System.Collections.Generic;
using System.Text;

namespace ChikenKitchenDataBase
{
    public class Budget
    {
        public int Id { get; set; }
        public double Balance { get; set; }

        public Budget() { }
        public Budget(double _Balance) { Balance = _Balance; }
    }
}
