using Dapper;
using SV22T1020161.DataLayers.Interfaces;
using SV22T1020161.Models.Sales;

namespace SV22T1020161.DataLayers.SqlServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu thống kê Dashboard trên SQL Server
    /// </summary>
    public class DashboardRepository : BaseSqlDAL, IDashboardRepository
    {
        public DashboardRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<int> CountCustomersAsync()
        {
            using (var connection = GetConnection())
            {
                var sql = @"SELECT COUNT(*) FROM Customers WHERE IsLocked = 0 OR IsLocked IS NULL";
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> CountProductsAsync()
        {
            using (var connection = GetConnection())
            {
                var sql = @"SELECT COUNT(*) FROM Products WHERE IsSelling = 1";
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<int> CountPendingOrdersAsync()
        {
            using (var connection = GetConnection())
            {
                var sql = @"SELECT COUNT(*) FROM Orders WHERE Status = @Status";
                return await connection.ExecuteScalarAsync<int>(sql, new { Status = (int)OrderStatusEnum.New });
            }
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            using (var connection = GetConnection())
            {
                var sql = @"SELECT ISNULL(SUM(od.Quantity * od.SalePrice), 0)
                            FROM Orders o
                            JOIN OrderDetails od ON o.OrderID = od.OrderID
                            WHERE o.Status = @CompletedStatus
                              AND CAST(o.FinishedTime AS DATE) = CAST(GETDATE() AS DATE)";
                return await connection.ExecuteScalarAsync<decimal>(sql, new { CompletedStatus = (int)OrderStatusEnum.Completed });
            }
        }

        public async Task<List<OrderViewInfo>> GetRecentPendingOrdersAsync(int take = 5)
        {
            using (var connection = GetConnection())
            {
                var sql = @"SELECT TOP (@Take) o.*,
                                   c.CustomerName, c.ContactName AS CustomerContactName,
                                   c.Phone AS CustomerPhone, c.Email AS CustomerEmail, c.Address AS CustomerAddress,
                                   e.FullName AS EmployeeName,
                                   s.ShipperName, s.Phone AS ShipperPhone,
                                   (SELECT SUM(Quantity * SalePrice) FROM OrderDetails WHERE OrderID = o.OrderID) AS TotalAmount
                            FROM Orders o
                            LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                            LEFT JOIN Employees e ON o.EmployeeID = e.EmployeeID
                            LEFT JOIN Shippers s ON o.ShipperID = s.ShipperID
                            WHERE o.Status = @Status
                            ORDER BY o.OrderTime DESC";

                var data = (await connection.QueryAsync<OrderViewInfo>(sql, new { Status = (int)OrderStatusEnum.New, Take = take })).ToList();
                return data;
            }
        }

        public async Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync(int months = 6)
        {
            using (var connection = GetConnection())
            {
                var sql = @"SELECT
                                MONTH(o.FinishedTime) AS Month,
                                YEAR(o.FinishedTime) AS Year,
                                ISNULL(SUM(od.Quantity * od.SalePrice), 0) AS Revenue
                            FROM Orders o
                            JOIN OrderDetails od ON o.OrderID = od.OrderID
                            WHERE o.Status = @CompletedStatus
                              AND o.FinishedTime >= DATEADD(MONTH, -@Months, GETDATE())
                            GROUP BY MONTH(o.FinishedTime), YEAR(o.FinishedTime)
                            ORDER BY Year, Month";

                var data = (await connection.QueryAsync<MonthlyRevenue>(sql,
                    new { CompletedStatus = (int)OrderStatusEnum.Completed, Months = months })).ToList();
                return data;
            }
        }
    }
}
