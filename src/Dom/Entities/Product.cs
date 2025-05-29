namespace Dom.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Construtor para EF
    protected Product() { }

    // Construtor principal
    public Product(string name, string description, decimal price, int stock)
    {
        Id = Guid.NewGuid();
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetStock(stock);
        CreatedAt = DateTime.UtcNow;
    }

    // Métodos de negócio
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome é obrigatório.");

        Name = name.Trim();
    }

    public void SetDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
    }

    public void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("O preço deve ser maior que zero.");

        Price = price;
    }

    public void SetStock(int stock)
    {
        if (stock < 0)
            throw new ArgumentException("O estoque não pode ser negativo.");

        Stock = stock;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        Stock += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.");

        if (quantity > Stock)
            throw new InvalidOperationException("Estoque insuficiente.");

        Stock -= quantity;
    }
}
