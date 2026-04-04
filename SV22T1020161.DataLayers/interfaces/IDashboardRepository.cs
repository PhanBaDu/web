namespace SV22T1020161.DataLayers.Interfaces
{
    /// <summary>
    /// Repository cung cấp dữ liệu thống kê cho Dashboard
    /// </summary>
    public interface IDashboardRepository
    {
        /// <summary>
        /// Đếm tổng số khách hàng
        /// </summary>
        Task<int> CountCustomersAsync();

        /// <summary>
        /// Đếm tổng số sản phẩm đang bán
        /// </summary>
        Task<int> CountProductsAsync();

        /// <summary>
        /// Đếm tổng số đơn hàng đang chờ xử lý (trạng thái New)
        /// </summary>
        Task<int> CountPendingOrdersAsync();

        /// <summary>
        /// Tính tổng doanh thu hôm nay
        /// </summary>
        Task<decimal> GetTodayRevenueAsync();

        /// <summary>
        /// Lấy danh sách đơn hàng mới cần xử lý (phân trang)
        /// </summary>
        Task<List<SV22T1020161.Models.Sales.OrderViewInfo>> GetRecentPendingOrdersAsync(int take = 5);

        /// <summary>
        /// Lấy số liệu doanh thu theo tháng (12 tháng gần nhất)
        /// </summary>
        Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync(int months = 6);
    }

    /// <summary>
    /// DTO cho doanh thu theo tháng
    /// </summary>
    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public string Label => $"{Month}/{Year}";
    }
}
