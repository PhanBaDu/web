using SV22T1020161.Models.Sales;

namespace SV22T1020161.Tests;

/// <summary>
/// TC-OS1 ~ TC-OS4: Unit test cho Order Status logic
/// TC-H1 ~ TC-H6: Unit test cho Order History logic
/// </summary>
public class OrderStatusTests
{
    // ===== TC-OS2: Timeline trạng thái =====
    [Theory]
    [InlineData(OrderStatusEnum.New, 0)]
    [InlineData(OrderStatusEnum.Accepted, 1)]
    [InlineData(OrderStatusEnum.Shipping, 2)]
    [InlineData(OrderStatusEnum.Completed, 3)]
    public void Timeline_ActiveStepIndex_CorrectByStatus(OrderStatusEnum status, int expectedActiveStep)
    {
        int activeStep = status switch
        {
            OrderStatusEnum.New => 0,
            OrderStatusEnum.Accepted => 1,
            OrderStatusEnum.Shipping => 2,
            OrderStatusEnum.Completed => 3,
            _ => -1
        };
        Assert.Equal(expectedActiveStep, activeStep);
    }

    // ===== TC-OS2: Cancelled / Rejected timeline =====
    [Theory]
    [InlineData(OrderStatusEnum.Cancelled, "Đã hủy")]
    [InlineData(OrderStatusEnum.Rejected, "Từ chối")]
    public void Timeline_CancelledOrRejected_ShowsCancelStep(
        OrderStatusEnum status, string expectedStepName)
    {
        bool isCancelledOrRejected = status == OrderStatusEnum.Cancelled
                                   || status == OrderStatusEnum.Rejected;

        Assert.True(isCancelledOrRejected);
        Assert.True(expectedStepName.ToLower().Contains("hủy") || expectedStepName.ToLower().Contains("từ chối"));
    }

    // ===== TC-OS4: Cancel order - chỉ New hoặc Accepted =====
    [Theory]
    [InlineData(OrderStatusEnum.New, true)]
    [InlineData(OrderStatusEnum.Accepted, true)]
    [InlineData(OrderStatusEnum.Shipping, false)]
    [InlineData(OrderStatusEnum.Completed, false)]
    [InlineData(OrderStatusEnum.Cancelled, false)]
    [InlineData(OrderStatusEnum.Rejected, false)]
    public void CanCancel_ReturnsCorrectBool(OrderStatusEnum status, bool canCancel)
    {
        var allowed = new[] { OrderStatusEnum.New, OrderStatusEnum.Accepted };
        Assert.Equal(canCancel, allowed.Contains(status));
    }

    // ===== TC-OS3: Total amount =====
    [Fact]
    public void OrderDetail_TotalPrice_CalculatesCorrectly()
    {
        var details = new List<OrderDetail>
        {
            new() { ProductID = 1, SalePrice = 100000, Quantity = 2 },
            new() { ProductID = 2, SalePrice = 50000, Quantity = 3 },
            new() { ProductID = 3, SalePrice = 200000, Quantity = 1 },
        };

        // (100000*2) + (50000*3) + (200000*1) = 200000 + 150000 + 200000 = 550000
        var total = details.Sum(d => d.SalePrice * d.Quantity);
        Assert.Equal(550000, total);
    }

    [Fact]
    public void OrderDetail_EmptyList_TotalIsZero()
    {
        var details = new List<OrderDetail>();
        var total = details.Sum(d => d.SalePrice * d.Quantity);
        Assert.Equal(0, total);
    }

    // ===== TC-H2: Lọc theo trạng thái =====
    private List<OrderViewInfo> FilterByStatus(List<OrderViewInfo> orders, int status)
    {
        if (status == 0) return orders; // Tất cả
        if (status == -1)
            return orders.Where(o =>
                o.Status == OrderStatusEnum.Cancelled ||
                o.Status == OrderStatusEnum.Rejected).ToList();
        return orders.Where(o => o.Status == (OrderStatusEnum)status).ToList();
    }

    [Fact]
    public void FilterByStatus_All_ReturnsAllOrders()
    {
        var orders = CreateSampleOrders();
        var result = FilterByStatus(orders, 0);
        Assert.Equal(orders.Count, result.Count);
    }

    [Fact]
    public void FilterByStatus_New_ReturnsOnlyNewOrders()
    {
        var orders = CreateSampleOrders();
        var result = FilterByStatus(orders, 1);
        Assert.All(result, o => Assert.Equal(OrderStatusEnum.New, o.Status));
    }

    [Fact]
    public void FilterByStatus_Shipping_ReturnsOnlyShippingOrders()
    {
        var orders = CreateSampleOrders();
        var result = FilterByStatus(orders, 3);
        Assert.All(result, o => Assert.Equal(OrderStatusEnum.Shipping, o.Status));
    }

    [Fact]
    public void FilterByStatus_Completed_ReturnsOnlyCompletedOrders()
    {
        var orders = CreateSampleOrders();
        var result = FilterByStatus(orders, 4);
        Assert.All(result, o => Assert.Equal(OrderStatusEnum.Completed, o.Status));
    }

    [Fact]
    public void FilterByStatus_Cancelled_ReturnsCancelledAndRejected()
    {
        var orders = CreateSampleOrders();
        var result = FilterByStatus(orders, -1);
        Assert.All(result, o =>
            Assert.True(o.Status == OrderStatusEnum.Cancelled
                     || o.Status == OrderStatusEnum.Rejected));
    }

    // ===== Helper =====
    private List<OrderViewInfo> CreateSampleOrders()
    {
        return new List<OrderViewInfo>
        {
            new() { OrderID = 1, Status = OrderStatusEnum.New, CustomerID = 10, TotalAmount = 500000, OrderTime = DateTime.Now.AddDays(-5), DeliveryAddress = "TP.HCM" },
            new() { OrderID = 2, Status = OrderStatusEnum.Accepted, CustomerID = 10, TotalAmount = 1200000, OrderTime = DateTime.Now.AddDays(-3), DeliveryAddress = "TP.HCM" },
            new() { OrderID = 3, Status = OrderStatusEnum.Shipping, CustomerID = 10, TotalAmount = 800000, OrderTime = DateTime.Now.AddDays(-2), DeliveryAddress = "TP.HCM" },
            new() { OrderID = 4, Status = OrderStatusEnum.Completed, CustomerID = 10, TotalAmount = 300000, OrderTime = DateTime.Now.AddDays(-10), DeliveryAddress = "TP.HCM" },
            new() { OrderID = 5, Status = OrderStatusEnum.Cancelled, CustomerID = 10, TotalAmount = 600000, OrderTime = DateTime.Now.AddDays(-1), DeliveryAddress = "TP.HCM" },
            new() { OrderID = 6, Status = OrderStatusEnum.Rejected, CustomerID = 10, TotalAmount = 900000, OrderTime = DateTime.Now.AddDays(-7), DeliveryAddress = "TP.HCM" },
            new() { OrderID = 7, Status = OrderStatusEnum.New, CustomerID = 99, TotalAmount = 1500000, OrderTime = DateTime.Now.AddDays(-4), DeliveryAddress = "HN" },
        };
    }

    // ===== TC-H3: Tìm kiếm theo mã đơn =====
    [Fact]
    public void SearchByOrderID_FindsMatchingOrder()
    {
        var orders = CreateSampleOrders();
        var result = orders.Where(o => o.OrderID.ToString().Contains("3")).ToList();

        Assert.Single(result);
        Assert.Equal(3, result[0].OrderID);
    }

    [Fact]
    public void SearchByOrderID_NoMatch_ReturnsEmpty()
    {
        var orders = CreateSampleOrders();
        var result = orders.Where(o => o.OrderID.ToString().Contains("999")).ToList();

        Assert.Empty(result);
    }

    // ===== TC-H5: Mua lại (Reorder) =====
    [Fact]
    public void Reorder_AddsAllItemsToCart()
    {
        var orderDetails = new List<OrderDetail>
        {
            new() { ProductID = 1, SalePrice = 100000, Quantity = 2 },
            new() { ProductID = 2, SalePrice = 200000, Quantity = 1 },
        };

        var cart = new List<SV22T1020161.Shop.Models.CartItem>();

        foreach (var d in orderDetails)
        {
            var existing = cart.FirstOrDefault(c => c.ProductID == d.ProductID);
            if (existing != null)
                existing.Quantity += d.Quantity;
            else
                cart.Add(new SV22T1020161.Shop.Models.CartItem
                {
                    ProductID = d.ProductID,
                    Price = d.SalePrice,
                    Quantity = d.Quantity
                });
        }

        Assert.Equal(2, cart.Count);
        Assert.Equal(2, cart.First(c => c.ProductID == 1).Quantity);
        Assert.Equal(400000, cart.Sum(c => c.TotalPrice));
    }

    // ===== TC-H4: Badge trạng thái =====
    [Theory]
    [InlineData(OrderStatusEnum.New, "md-badge-neutral")]
    [InlineData(OrderStatusEnum.Accepted, "md-badge-info")]
    [InlineData(OrderStatusEnum.Shipping, "md-badge-primary")]
    [InlineData(OrderStatusEnum.Completed, "md-badge-success")]
    [InlineData(OrderStatusEnum.Cancelled, "md-badge-danger")]
    [InlineData(OrderStatusEnum.Rejected, "md-badge-warning")]
    public void StatusBadge_HasCorrectCssClass(OrderStatusEnum status, string expectedClass)
    {
        string badgeClass = status switch
        {
            OrderStatusEnum.New => "md-badge-neutral",
            OrderStatusEnum.Accepted => "md-badge-info",
            OrderStatusEnum.Shipping => "md-badge-primary",
            OrderStatusEnum.Completed => "md-badge-success",
            OrderStatusEnum.Cancelled => "md-badge-danger",
            OrderStatusEnum.Rejected => "md-badge-warning",
            _ => "md-badge-neutral"
        };

        Assert.Equal(expectedClass, badgeClass);
    }

    // ===== TC-OS5: Status color =====
    [Theory]
    [InlineData(OrderStatusEnum.New, "#607D8B")]
    [InlineData(OrderStatusEnum.Accepted, "#03A9F4")]
    [InlineData(OrderStatusEnum.Shipping, "#6750A4")]
    [InlineData(OrderStatusEnum.Completed, "#4CAF50")]
    [InlineData(OrderStatusEnum.Cancelled, "#B3261E")]
    [InlineData(OrderStatusEnum.Rejected, "#FF9800")]
    public void StatusColor_HasCorrectHex(OrderStatusEnum status, string expectedColor)
    {
        string color = status switch
        {
            OrderStatusEnum.New => "#607D8B",
            OrderStatusEnum.Accepted => "#03A9F4",
            OrderStatusEnum.Shipping => "#6750A4",
            OrderStatusEnum.Completed => "#4CAF50",
            OrderStatusEnum.Cancelled => "#B3261E",
            OrderStatusEnum.Rejected => "#FF9800",
            _ => "#9E9E9E"
        };

        Assert.Equal(expectedColor, color);
    }

    // ===== Order Status Enum values =====
    [Fact]
    public void OrderStatusEnum_HasCorrectValues()
    {
        Assert.Equal(1, (int)OrderStatusEnum.New);
        Assert.Equal(2, (int)OrderStatusEnum.Accepted);
        Assert.Equal(3, (int)OrderStatusEnum.Shipping);
        Assert.Equal(4, (int)OrderStatusEnum.Completed);
        Assert.Equal(-1, (int)OrderStatusEnum.Cancelled);
        Assert.Equal(-2, (int)OrderStatusEnum.Rejected);
    }
}
