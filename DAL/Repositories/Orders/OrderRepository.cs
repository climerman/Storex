using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL.Interfaces;
using DAL.Interfaces.Orders;
using Domain.Orders;

namespace DAL.Repositories.Orders
{
    public class OrderRepository : EFRepository<Order>, IOrderRepository
    {
        public OrderRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        public List<Order> GetAllUserOrders(int userId)
        {
            return DbSet.Where(o => o.UserId == userId)
                .OrderBy(o => o.ModifiedAtDT)
                .ThenBy(o => o.CreatedAtDT)
                .Include(p => p.StoredProducts)
                .ToList();
        }

        public Order GetUserOrder(int orderId, int userId)
        {
            return DbSet.FirstOrDefault(o => o.UserId == userId && o.OrderId == orderId);
        }
    }
}