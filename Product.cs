using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bike_STore_Project
{
    public class Product
    {
        public int Id { get; set; }           // maps to products.id
        public string Serial { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
