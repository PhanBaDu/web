using SV22T1020161.DataLayers.Interfaces;
using SV22T1020161.DataLayers.SqlServer;
using SV22T1020161.Models.Sales;

namespace SV22T1020161.BusinessLayers
{
    /// <summary>
    /// Cung cấp dữ liệu thống kê cho Dashboard
    /// </summary>
    public static class DashboardDataService
    {
        private static readonly IDashboardRepository dashboardDB;

        static DashboardDataService()
        {
            dashboardDB = new DashboardRepository(Configuration.ConnectionString);
        }

        /// <summary>
        /// Lấy tổng số khách hàng
        /// </summary>
        public static async Task<int> GetCustomerCountAsync()
        {
            return await dashboardDB.CountCustomersAsync();
        }

        /// <summary>
        /// Lấy tổng số sản phẩm đang bán
        /// </summary>
        public static async Task<int> GetProductCountAsync()
        {
            return await dashboardDB.CountProductsAsync();
        }

        /// <summary>
        /// Lấy số đơn hàng đang chờ xử lý
        /// </summary>
        public static async Task<int> GetPendingOrderCountAsync()
        {
            return await dashboardDB.CountPendingOrdersAsync();
        }

        /// <summary>
        /// Lấy doanh thu hôm nay
        /// </summary>
        public static async Task<decimal> GetTodayRevenueAsync()
        {
            return await dashboardDB.GetTodayRevenueAsync();
        }

        /// <summary>
        /// Lấy danh sách đơn hàng mới cần xử lý
        /// </summary>
        public static async Task<List<OrderViewInfo>> GetRecentPendingOrdersAsync(int take = 5)
        {
            return await dashboardDB.GetRecentPendingOrdersAsync(take);
        }

        /// <summary>
        /// Lấy doanh thu theo tháng
        /// </summary>
        public static async Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync(int months = 6)
        {
            return await dashboardDB.GetMonthlyRevenueAsync(months);
        }
    }
}
