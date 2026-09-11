using EzyInventory.Models;
using EzyInventory.Services;

namespace EzyInventory;

public sealed class MainForm : Form
{
    private readonly InventoryService _inventory = new();

    private readonly TextBox _skuInput = new() { PlaceholderText = "SKU" };
    private readonly TextBox _nameInput = new() { PlaceholderText = "Name" };
    private readonly NumericUpDown _priceInput = new() { DecimalPlaces = 2, Maximum = 1_000_000, Minimum = 0 };
    private readonly NumericUpDown _quantityInput = new() { Maximum = 1_000_000, Minimum = 0 };
    private readonly NumericUpDown _stockDeltaInput = new() { Maximum = 1_000_000, Minimum = -1_000_000 };

    private readonly Label _totalValueLabel = new() { AutoSize = true, Text = "Total Inventory Value: $0.00" };
    private readonly DataGridView _productGrid = new()
    {
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };

    public MainForm()
    {
        Text = "Ezy Inventory Management";
        Width = 1000;
        Height = 620;
        StartPosition = FormStartPosition.CenterScreen;

        var buttonPanel = BuildButtonPanel();
        var inputPanel = BuildInputPanel();

        _productGrid.Columns.Add("Sku", "SKU");
        _productGrid.Columns.Add("Name", "Name");
        _productGrid.Columns.Add("UnitPrice", "Unit Price");
        _productGrid.Columns.Add("Quantity", "Quantity");
        _productGrid.Columns.Add("Value", "Inventory Value");

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(12)
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(inputPanel, 0, 0);
        layout.Controls.Add(buttonPanel, 0, 1);
        layout.Controls.Add(_productGrid, 0, 2);
        layout.Controls.Add(_totalValueLabel, 0, 3);

        Controls.Add(layout);
    }

    private Control BuildInputPanel()
    {
        var inputPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            WrapContents = true
        };

        _priceInput.Width = 140;
        _quantityInput.Width = 140;
        _stockDeltaInput.Width = 140;

        inputPanel.Controls.AddRange([
            new Label { Text = "SKU", AutoSize = true, Padding = new Padding(0, 8, 0, 0) },
            _skuInput,
            new Label { Text = "Name", AutoSize = true, Padding = new Padding(8, 8, 0, 0) },
            _nameInput,
            new Label { Text = "Unit Price", AutoSize = true, Padding = new Padding(8, 8, 0, 0) },
            _priceInput,
            new Label { Text = "Quantity", AutoSize = true, Padding = new Padding(8, 8, 0, 0) },
            _quantityInput,
            new Label { Text = "Stock Delta (+/-)", AutoSize = true, Padding = new Padding(8, 8, 0, 0) },
            _stockDeltaInput
        ]);

        _skuInput.Width = 140;
        _nameInput.Width = 180;

        return inputPanel;
    }

    private Control BuildButtonPanel()
    {
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            WrapContents = true,
            Margin = new Padding(0, 8, 0, 8)
        };

        var addButton = new Button { Text = "Add Product", AutoSize = true };
        var updateButton = new Button { Text = "Update Product", AutoSize = true };
        var adjustStockButton = new Button { Text = "Adjust Stock", AutoSize = true };
        var removeButton = new Button { Text = "Remove Product", AutoSize = true };
        var refreshButton = new Button { Text = "Refresh", AutoSize = true };

        addButton.Click += (_, _) => AddProduct();
        updateButton.Click += (_, _) => UpdateProduct();
        adjustStockButton.Click += (_, _) => AdjustStock();
        removeButton.Click += (_, _) => RemoveProduct();
        refreshButton.Click += (_, _) => RefreshGrid();

        buttonPanel.Controls.AddRange([addButton, updateButton, adjustStockButton, removeButton, refreshButton]);

        return buttonPanel;
    }

    private void AddProduct()
    {
        if (string.IsNullOrWhiteSpace(_skuInput.Text) || string.IsNullOrWhiteSpace(_nameInput.Text))
        {
            MessageBox.Show("SKU and Name are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var added = _inventory.AddProduct(new Product
        {
            Sku = _skuInput.Text.Trim(),
            Name = _nameInput.Text.Trim(),
            UnitPrice = _priceInput.Value,
            QuantityInStock = (int)_quantityInput.Value
        });

        if (!added)
        {
            MessageBox.Show("A product with this SKU already exists.", "Duplicate SKU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RefreshGrid();
        MessageBox.Show("Product added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void UpdateProduct()
    {
        if (string.IsNullOrWhiteSpace(_skuInput.Text))
        {
            MessageBox.Show("SKU is required to update a product.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var updated = _inventory.UpdateProduct(_skuInput.Text.Trim(), _nameInput.Text.Trim(), _priceInput.Value);
        MessageBox.Show(updated ? "Product updated." : "Product not found.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
        RefreshGrid();
    }

    private void AdjustStock()
    {
        if (string.IsNullOrWhiteSpace(_skuInput.Text))
        {
            MessageBox.Show("SKU is required to adjust stock.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var adjusted = _inventory.AdjustStock(_skuInput.Text.Trim(), (int)_stockDeltaInput.Value);
        MessageBox.Show(
            adjusted ? "Stock adjusted." : "Adjustment failed (product missing or stock would go below zero).",
            "Adjust Stock",
            MessageBoxButtons.OK,
            adjusted ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        RefreshGrid();
    }

    private void RemoveProduct()
    {
        if (string.IsNullOrWhiteSpace(_skuInput.Text))
        {
            MessageBox.Show("SKU is required to remove a product.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var removed = _inventory.RemoveProduct(_skuInput.Text.Trim());
        MessageBox.Show(removed ? "Product removed." : "Product not found.", "Remove Product", MessageBoxButtons.OK, MessageBoxIcon.Information);
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        _productGrid.Rows.Clear();

        foreach (var product in _inventory.ListProducts())
        {
            _productGrid.Rows.Add(product.Sku, product.Name, product.UnitPrice.ToString("C"), product.QuantityInStock, product.InventoryValue.ToString("C"));
        }

        _totalValueLabel.Text = $"Total Inventory Value: {_inventory.TotalInventoryValue():C}";
    }
}
