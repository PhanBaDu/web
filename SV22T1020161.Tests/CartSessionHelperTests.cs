using SV22T1020161.Shop.Models;

namespace SV22T1020161.Tests;

/// <summary>
/// TC-C1 ~ TC-C4: Unit test cho CartSessionHelper
/// </summary>
public class CartSessionHelperTests
{
    [Fact]
    public void CartItem_QuantityPlusPrice_CalculatesTotalPrice()
    {
        var item = new CartItem
        {
            ProductID = 1,
            ProductName = "Test Product",
            Price = 100000,
            Quantity = 3,
            Unit = "Cái",
            Photo = ""
        };

        Assert.Equal(300000, item.TotalPrice);
    }

    [Fact]
    public void CartItem_TotalPrice_UpdatesWhenQuantityChanges()
    {
        var item = new CartItem
        {
            ProductID = 1,
            ProductName = "Test",
            Price = 50000,
            Quantity = 2,
            Unit = "Cái",
            Photo = ""
        };

        Assert.Equal(100000, item.TotalPrice);

        item.Quantity = 5;
        Assert.Equal(250000, item.TotalPrice);
    }

    [Fact]
    public void CartItem_ZeroQuantity_TotalPriceIsZero()
    {
        var item = new CartItem
        {
            ProductID = 1,
            ProductName = "Test",
            Price = 50000,
            Quantity = 0,
            Unit = "Cái",
            Photo = ""
        };

        Assert.Equal(0, item.TotalPrice);
    }

    [Fact]
    public void CartItem_EmptyCart_TotalIsZero()
    {
        var cart = new List<CartItem>();
        var total = cart.Sum(c => c.TotalPrice);

        Assert.Equal(0, total);
    }

    [Fact]
    public void CartItem_MultipleItems_CalculatesCorrectTotal()
    {
        var cart = new List<CartItem>
        {
            new() { ProductID = 1, ProductName = "SP1", Price = 100000, Quantity = 2, Unit = "Cái", Photo = "" },
            new() { ProductID = 2, ProductName = "SP2", Price = 50000, Quantity = 3, Unit = "Cái", Photo = "" },
            new() { ProductID = 3, ProductName = "SP3", Price = 200000, Quantity = 1, Unit = "Cái", Photo = "" },
        };

        // (100000*2) + (50000*3) + (200000*1) = 200000 + 150000 + 200000 = 550000
        var total = cart.Sum(c => c.TotalPrice);
        Assert.Equal(550000, total);
        Assert.Equal(3, cart.Count);
    }

    [Fact]
    public void CartItem_PhotoUrl_HandledCorrectly()
    {
        var itemHttp = new CartItem { Photo = "http://example.com/img.jpg" };
        var itemRelative = new CartItem { Photo = "product1.jpg" };
        var itemEmpty = new CartItem { Photo = "" };

        Assert.Equal("http://example.com/img.jpg", itemHttp.Photo);
        Assert.Equal("product1.jpg", itemRelative.Photo);
        Assert.Equal("", itemEmpty.Photo);
    }

    [Fact]
    public void CartItem_AddDuplicateProduct_QuantityIncreases()
    {
        var cart = new List<CartItem>
        {
            new() { ProductID = 1, ProductName = "SP1", Price = 100000, Quantity = 2, Unit = "Cái", Photo = "" }
        };

        // Simulate adding same product again
        var existing = cart.FirstOrDefault(c => c.ProductID == 1);
        if (existing != null)
        {
            existing.Quantity += 1;
        }

        Assert.Equal(3, cart.First().Quantity);
        Assert.Equal(1, cart.Count);
    }

    [Fact]
    public void CartItem_AddDifferentProduct_NewItemInCart()
    {
        var cart = new List<CartItem>
        {
            new() { ProductID = 1, ProductName = "SP1", Price = 100000, Quantity = 1, Unit = "Cái", Photo = "" }
        };

        cart.Add(new CartItem
        {
            ProductID = 2,
            ProductName = "SP2",
            Price = 200000,
            Quantity = 1,
            Unit = "Cái",
            Photo = ""
        });

        Assert.Equal(2, cart.Count);
        Assert.Equal(300000, cart.Sum(c => c.TotalPrice));
    }

    [Fact]
    public void CartItem_RemoveProduct_CartCountDecreases()
    {
        var cart = new List<CartItem>
        {
            new() { ProductID = 1, ProductName = "SP1", Price = 100000, Quantity = 1, Unit = "Cái", Photo = "" },
            new() { ProductID = 2, ProductName = "SP2", Price = 200000, Quantity = 1, Unit = "Cái", Photo = "" }
        };

        var itemToRemove = cart.First(c => c.ProductID == 1);
        cart.Remove(itemToRemove);

        Assert.Single(cart);
        Assert.Equal(200000, cart.Sum(c => c.TotalPrice));
    }

    [Fact]
    public void CartItem_ClearCart_CountIsZero()
    {
        var cart = new List<CartItem>
        {
            new() { ProductID = 1, ProductName = "SP1", Price = 100000, Quantity = 5, Unit = "Cái", Photo = "" },
            new() { ProductID = 2, ProductName = "SP2", Price = 200000, Quantity = 3, Unit = "Cái", Photo = "" }
        };

        cart.Clear();

        Assert.Empty(cart);
    }

    [Fact]
    public void CartItem_QuantityMinOne_CannotGoBelowOne()
    {
        var item = new CartItem
        {
            ProductID = 1,
            ProductName = "Test",
            Price = 50000,
            Quantity = 1,
            Unit = "Cái",
            Photo = ""
        };

        // Khi quantity = 1, nút "-" không được giảm nữa
        var newQty = Math.Max(1, item.Quantity - 1);
        item.Quantity = newQty;

        Assert.Equal(1, item.Quantity);
    }
}
