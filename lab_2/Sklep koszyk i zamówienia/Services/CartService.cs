namespace Lab02_Shop.Services;

public class CartService
{
    private readonly Dictionary<int, int> _items = new();

    public Dictionary<int, int> GetAll()
        => new(_items);

    public void Add(int productId, int qty)
    {
        if (_items.ContainsKey(productId))
            _items[productId] += qty;
        else
            _items[productId] = qty;
    }

    public void Clear() => _items.Clear();
}
