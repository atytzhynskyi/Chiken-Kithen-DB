using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Chiken_Kithen_DB
{
    [Table("Customers")]
    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Food Order { get; set; }
        public List<Allergy> Allergies { get; set; } = new List<Allergy>();

        public Customer() { }

        public Customer(string _Name) {
            Name = _Name;
        }

        public Customer(string _Name, Food _Order, params Allergy[] _Allergies){
            Name = _Name;
            Order = _Order;
            Allergies.AddRange(_Allergies);
        }
    }
}
