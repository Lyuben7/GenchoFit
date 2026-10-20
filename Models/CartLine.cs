namespace GenchoFit.Models;

public class CartLine
{
    public int ProductId { get; set; }
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total => Price * Quantity;
}
