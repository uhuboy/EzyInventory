namespace EzyInventory.Models;

public sealed class Product
{
    public required string Sku { get; init; }
    public required string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }

    public decimal InventoryValue => UnitPrice * QuantityInStock;
}
