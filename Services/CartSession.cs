using System.Text.Json;

namespace GenchoFit.Services;

public static class CartSession
{
    private const string Key = "cart";

    public static Dictionary<int, int> GetItems(ISession session)
    {
        var json = session.GetString(Key);
        if (string.IsNullOrWhiteSpace(json)) return new Dictionary<int, int>();
        return JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
    }

    public static void Save(ISession session, Dictionary<int, int> items)
        => session.SetString(Key, JsonSerializer.Serialize(items));

    public static int Count(ISession session) => GetItems(session).Values.Sum();

    public static void Add(ISession session, int productId, int quantity = 1)
    {
        var items = GetItems(session);
        items[productId] = items.GetValueOrDefault(productId) + quantity;
        Save(session, items);
    }

    public static void Remove(ISession session, int productId)
    {
        var items = GetItems(session);
        items.Remove(productId);
        Save(session, items);
    }

    public static void Clear(ISession session) => session.Remove(Key);
}
