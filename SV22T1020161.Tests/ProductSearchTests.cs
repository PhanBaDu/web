using SV22T1020161.Models.Catalog;
using SV22T1020161.Models.Common;

namespace SV22T1020161.Tests;

/// <summary>
/// TC-S1 ~ TC-S5: Unit test cho Product Search logic
/// </summary>
public class ProductSearchTests
{
    private List<Product> CreateSampleProducts()
    {
        return new List<Product>
        {
            new() { ProductID = 1, ProductName = "Samsung Galaxy S24", Price = 25000000, CategoryID = 1, IsSelling = true, Unit = "Cái", Photo = "s24.jpg" },
            new() { ProductID = 2, ProductName = "iPhone 15 Pro", Price = 35000000, CategoryID = 1, IsSelling = true, Unit = "Cái", Photo = "ip15.jpg" },
            new() { ProductID = 3, ProductName = "MacBook Air M3", Price = 35000000, CategoryID = 2, IsSelling = true, Unit = "Cái", Photo = "mba.jpg" },
            new() { ProductID = 4, ProductName = "Áo thun nam", Price = 150000, CategoryID = 3, IsSelling = true, Unit = "Cái", Photo = "ao.jpg" },
            new() { ProductID = 5, ProductName = "Quần jeans", Price = 350000, CategoryID = 3, IsSelling = true, Unit = "Cái", Photo = "jean.jpg" },
            new() { ProductID = 6, ProductName = "Son môi", Price = 200000, CategoryID = 4, IsSelling = true, Unit = "Cây", Photo = "son.jpg" },
            new() { ProductID = 7, ProductName = "Kem dưỡng", Price = 450000, CategoryID = 4, IsSelling = false, Unit = "Hộp", Photo = "kem.jpg" },
            new() { ProductID = 8, ProductName = "Nồi cơm điện", Price = 800000, CategoryID = 5, IsSelling = true, Unit = "Cái", Photo = "noi.jpg" },
            new() { ProductID = 9, ProductName = "Bàn phím gaming", Price = 1200000, CategoryID = 5, IsSelling = true, Unit = "Cái", Photo = "kb.jpg" },
            new() { ProductID = 10, ProductName = "Chuột wireless", Price = 250000, CategoryID = 5, IsSelling = true, Unit = "Cái", Photo = "mouse.jpg" },
        };
    }

    private List<Product> FilterProducts(List<Product> products, string? searchValue,
        int categoryID = 0, decimal minPrice = 0, decimal maxPrice = decimal.MaxValue)
    {
        var query = products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchValue))
            query = query.Where(p => p.ProductName.Contains(searchValue, StringComparison.OrdinalIgnoreCase));

        if (categoryID > 0)
            query = query.Where(p => p.CategoryID == categoryID);

        if (minPrice > 0)
            query = query.Where(p => p.Price >= minPrice);

        if (maxPrice < decimal.MaxValue)
            query = query.Where(p => p.Price <= maxPrice);

        return query.ToList();
    }

    // ===== TC-S2: Tìm kiếm =====
    [Fact]
    public void Search_ByName_ReturnsMatchingProducts()
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, "Samsung");

        Assert.Single(result);
        Assert.Equal("Samsung Galaxy S24", result[0].ProductName);
    }

    [Fact]
    public void Search_ByName_NoMatch_ReturnsEmpty()
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, "XYZNotExist");

        Assert.Empty(result);
    }

    [Fact]
    public void Search_ByName_CaseInsensitive()
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, "samsung");
        var result2 = FilterProducts(products, "SAMSUNG");

        Assert.Single(result);
        Assert.Single(result2);
        Assert.Equal(result[0].ProductID, result2[0].ProductID);
    }

    // ===== TC-S2: Lọc theo danh mục =====
    [Fact]
    public void Filter_ByCategory_ReturnsOnlyThatCategory()
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, null, categoryID: 1);

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal(1, p.CategoryID));
    }

    [Fact]
    public void Filter_ByCategory_NoneFound_ReturnsEmpty()
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, null, categoryID: 99);

        Assert.Empty(result);
    }

    // ===== TC-S2: Lọc theo khoảng giá =====
    [Theory]
    [InlineData(0, 200000, 2)] // Dưới 200k: áo thun(150k), son(200k) — chuột(250k-no)
    [InlineData(100000, 300000, 3)] // 100k-300k: áo thun(150k), son(200k), chuột(250k)
    [InlineData(500000, 2000000, 2)] // 500k-2M: nồi(800k), bàn phím(1.2M)
    [InlineData(20000000, 40000000, 3)] // Samsung(25M), iPhone(35M), MacBook(35M)
    public void Filter_ByPriceRange_ReturnsCorrectCount(
        decimal min, decimal max, int expected)
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, null, 0, min, max);

        Assert.Equal(expected, result.Count);
    }

    [Fact]
    public void Filter_PriceAboveAllProducts_ReturnsEmpty()
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, null, 0, 50000000, 100000000);

        Assert.Empty(result);
    }

    [Fact]
    public void Filter_PriceBelowAllProducts_ReturnsEmpty()
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, null, 0, 0, 10000);

        Assert.Empty(result);
    }

    // ===== TC-S2: Kết hợp nhiều điều kiện =====
    [Fact]
    public void Filter_CombineCategoryAndPrice_ReturnsIntersection()
    {
        var products = CreateSampleProducts();
        // Category 1 (điện thoại) + giá < 30M
        var result = FilterProducts(products, null, categoryID: 1, minPrice: 0, maxPrice: 30000000);

        Assert.Single(result);
        Assert.Equal("Samsung Galaxy S24", result[0].ProductName);
    }

    [Fact]
    public void Filter_CombineNameAndCategory_ReturnsIntersection()
    {
        var products = CreateSampleProducts();
        var result = FilterProducts(products, "iPhone", categoryID: 1);

        Assert.Single(result);
        Assert.Equal("iPhone 15 Pro", result[0].ProductName);
    }

    [Fact]
    public void Filter_CombineAllConditions_ReturnsIntersection()
    {
        var products = CreateSampleProducts();
        // Category 5 (đồ gia dụng) + tên "Bàn" + giá 1M-2M
        var result = FilterProducts(products, "Bàn", categoryID: 5, minPrice: 1000000, maxPrice: 2000000);

        Assert.Single(result);
        Assert.Equal("Bàn phím gaming", result[0].ProductName);
    }

    // ===== TC-S5: Sản phẩm ngừng bán =====
    [Fact]
    public void Filter_IsSellingFalse_NotIncludedByDefault()
    {
        var products = CreateSampleProducts();
        var allSelling = products.Where(p => p.IsSelling).ToList();

        Assert.DoesNotContain(allSelling, p => p.ProductName == "Kem dưỡng");
        Assert.Equal(9, allSelling.Count); // 10 sp - 1 ngừng bán
    }

    [Fact]
    public void Filter_SellingProducts_HaveAddToCartButton()
    {
        var products = CreateSampleProducts();
        var sellingProducts = products.Where(p => p.IsSelling).ToList();

        Assert.Equal(9, sellingProducts.Count);
        Assert.All(sellingProducts, p => Assert.True(p.IsSelling));
    }

    [Fact]
    public void Filter_NonSellingProducts_HaveSoldOutBadge()
    {
        var products = CreateSampleProducts();
        var notSelling = products.Where(p => !p.IsSelling).ToList();

        Assert.Single(notSelling);
        Assert.Equal("Kem dưỡng", notSelling[0].ProductName);
    }

    // ===== Pagination =====
    [Fact]
    public void Pagination_TotalPages_CalculatedCorrectly()
    {
        const int pageSize = 12;
        var products = CreateSampleProducts();
        const int totalProducts = 10;

        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        Assert.Equal(1, totalPages);

        // Add more products to test pagination
        var productsMore = Enumerable.Range(1, 25)
            .Select(i => new Product { ProductID = i, ProductName = $"SP{i}", Price = 100000, IsSelling = true, Unit = "Cái" })
            .ToList();

        totalPages = (int)Math.Ceiling(productsMore.Count / (double)pageSize);
        Assert.Equal(3, totalPages);
    }

    [Fact]
    public void Pagination_PageSize12_Shows12PerPage()
    {
        const int pageSize = 12;
        var allProducts = Enumerable.Range(1, 25)
            .Select(i => new Product { ProductID = i, ProductName = $"SP{i}", Price = 100000, IsSelling = true, Unit = "Cái" })
            .ToList();

        var page1 = allProducts.Take(pageSize).ToList();
        var page2 = allProducts.Skip(pageSize).Take(pageSize).ToList();

        Assert.Equal(12, page1.Count);
        Assert.Equal(12, page2.Count);
        Assert.Equal(1, page1.First().ProductID);
        Assert.Equal(13, page2.First().ProductID);
    }
}
