using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bike_STore_Project
{
    public class FifoPreviewRow
    {
        public int LotId { get; set; }
        public DateTime ReceivedAt { get; set; }
        public decimal UnitCost { get; set; }
        public int QtyToTake { get; set; }
    }

}
