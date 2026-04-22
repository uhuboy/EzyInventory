# EzyInventory (C# WinForms)

A Windows Forms-based inventory management system built in C#.

## Features

- Add products (SKU, name, unit price, quantity)
- View products in a grid
- Update product details
- Adjust stock up/down (prevents negative stock)
- Remove products
- View total inventory value

## Project structure

- `Program.cs` — WinForms app entry point
- `Forms/MainForm.cs` — main UI and event handlers
- `Models/Product.cs` — product entity
- `Services/InventoryService.cs` — inventory business logic

## Run locally

```bash
dotnet run
```

## Notes

The app stores inventory in memory while running.
