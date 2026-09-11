using EzyInventory.Models;

namespace EzyInventory.Services;

public sealed class InventoryService
{
    private readonly Dictionary<string, Product> _products = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<Product> ListProducts() => _products.Values.OrderBy(p => p.Name).ToArray();

    public bool AddProduct(Product product)
    {
        if (_products.ContainsKey(product.Sku))
        {
            return false;
        }

        _products[product.Sku] = product;
        return true;
    }

    public bool UpdateProduct(string sku, string? name = null, decimal? unitPrice = null)
    {
        if (!_products.TryGetValue(sku, out var existing))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            existing.Name = name.Trim();
        }

        if (unitPrice.HasValue && unitPrice.Value >= 0)
        {
            existing.UnitPrice = unitPrice.Value;
        }

        return true;
    }

    public bool AdjustStock(string sku, int delta)
    {
        if (!_products.TryGetValue(sku, out var existing))
        {
            return false;
        }

        var updatedStock = existing.QuantityInStock + delta;
        if (updatedStock < 0)
        {
            return false;
        }

        existing.QuantityInStock = updatedStock;
        return true;
    }

    public bool RemoveProduct(string sku) => _products.Remove(sku);

    public decimal TotalInventoryValue() => _products.Values.Sum(p => p.InventoryValue);
}
