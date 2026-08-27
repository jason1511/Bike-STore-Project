using System;
using System.Collections.Generic;

namespace Bike_STore_Project
{
    public sealed class InvoiceDraftItem
    {
        public int ProductId { get; set; }
        public string Brand { get; set; } = "";
        public string Type { get; set; } = "";
        public string? Color { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string FrameNumbers { get; set; } = "";
        public decimal LineTotal => Quantity * UnitPrice;
        public string Bike => $"{Brand} {Type}".Trim();
    }

    public sealed class InvoiceHeader
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string PaymentBank { get; set; } = "";
        public string Notes { get; set; } = "";
        public string Status { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
        public List<InvoiceDraftItem> Items { get; set; } = new();
    }
}
