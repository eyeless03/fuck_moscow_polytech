namespace Lab4;

public sealed class Product
{
    private string _name = string.Empty;
    private int _stockQuantity;

    public string ProductCode { get; set; } = string.Empty;

    public string Name
    {
        get => _name;
        set => _name = !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : throw new ArgumentException("Название товара не может быть пустым.", nameof(value));
    }

    public int StockQuantity
    {
        get => _stockQuantity;
        set => _stockQuantity = value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), "Количество не может быть отрицательным.");
    }

    public bool IsLowStock => StockQuantity < 25;

    public Product(string productCode, string name, int stockQuantity)
    {
        ProductCode = productCode;
        Name = name;
        StockQuantity = stockQuantity;
    }

    public string GetInfo() =>
        $"Код: {ProductCode}; товар: {Name}; количество: {StockQuantity}; мало на складе: {IsLowStock}";

    public string GetInfo(bool includeIntProperty) => includeIntProperty
        ? GetInfo()
        : $"Код: {ProductCode}; товар: {Name}; мало на складе: {IsLowStock}";
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Product notebook = new("NB-104", "Ноутбук", 18);
        Console.WriteLine(notebook.GetInfo());
        Console.WriteLine(notebook.GetInfo(includeIntProperty: false));

        notebook.StockQuantity = 30;
        Console.WriteLine($"После поставки: {notebook.GetInfo()}");
    }
}
