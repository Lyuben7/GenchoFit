using GenchoFit.Data;
using GenchoFit.Models;
using GenchoFit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GenchoFit.Controllers;

public class CartController(ShopDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var items = CartSession.GetItems(HttpContext.Session);
        if (items.Count == 0) return View(new List<CartLine>());

        var products = await db.Products
            .Where(p => items.Keys.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var lines = items
            .Where(pair => products.ContainsKey(pair.Key))
            .Select(pair =>
            {
                var product = products[pair.Key];
                return new CartLine
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Emoji = product.Emoji,
                    Price = product.Price,
                    Quantity = pair.Value
                };
            })
            .OrderBy(line => line.Name)
            .ToList();

        return View(lines);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int id, string? returnUrl = null)
    {
        CartSession.Add(HttpContext.Session, id);
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        CartSession.Remove(HttpContext.Session, id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        CartSession.Clear(HttpContext.Session);
        return RedirectToAction(nameof(Index));
    }
}
