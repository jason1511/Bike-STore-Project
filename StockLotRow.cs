namespace Bike_STore_Project
{
    public class StockLotRow
    {
        public int LotId { get; set; }
        public int ProductId { get; set; }

        public string Brand { get; set; } = "";
        public string Type { get; set; } = "";
        public string? Color { get; set; }

        public int QtyReceived { get; set; }
        public int QtyRemaining { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string? Notes { get; set; }
    }
}
