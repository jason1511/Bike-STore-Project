using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bike_STore_Project
{
    public class Product
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Color { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}


