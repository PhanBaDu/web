using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020161.Admin;
using SV22T1020161.Admin.AppCodes;
using SV22T1020161.BusinessLayers;
using SV22T1020161.Models;
using SV22T1020161.Models.Constants;

namespace SV22T1020161.Admin.Controllers;

[Authorize]
public class HomeController : Controller
{
    /// <summary>
    /// Giao diện trang chủ Dashboard
    /// </summary>
    [AuthorizePermission(Permissions.DashboardView)]
    public async Task<IActionResult> Index()
    {
        var todayRevenue = await DashboardDataService.GetTodayRevenueAsync();
        var pendingOrders = await DashboardDataService.GetPendingOrderCountAsync();
        var customerCount = await DashboardDataService.GetCustomerCountAsync();
        var productCount = await DashboardDataService.GetProductCountAsync();
        var recentOrders = await DashboardDataService.GetRecentPendingOrdersAsync(5);
        var monthlyRevenue = await DashboardDataService.GetMonthlyRevenueAsync(6);

        ViewBag.TodayRevenue = todayRevenue;
        ViewBag.PendingOrders = pendingOrders;
        ViewBag.CustomerCount = customerCount;
        ViewBag.ProductCount = productCount;
        ViewBag.RecentOrders = recentOrders;
        ViewBag.MonthlyRevenue = monthlyRevenue;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
