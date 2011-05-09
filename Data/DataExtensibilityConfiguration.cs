using System.Data.Entity;
using Models.DataExtensibilityModel;

namespace Data
{
    public class DataExtensibilityConfiguration
    {
        public void InjectConfiguration(DbModelBuilder modelBuilder)
        {
            var orderConfiguration = modelBuilder.Entity<Order>();
            orderConfiguration.ToTable("Orders");
            orderConfiguration.HasKey(o => o.OrderId);
            modelBuilder.Register<Order>();

            var productConfiguration = modelBuilder.Entity<Product>();
            productConfiguration.ToTable("Products");
            productConfiguration.HasKey(p => p.ProductId);
            modelBuilder.Register<Product>();

            var orderDetailConfiguration = modelBuilder.Entity<OrderDetail>();
            orderDetailConfiguration.ToTable("OrderDetails");
            orderDetailConfiguration.HasKey(od => new { od.OrderId, od.ProductId });
            modelBuilder.Register<OrderDetail>();

            var customerConfiguration = modelBuilder.Entity<Customer>();
            customerConfiguration.ToTable("Customers");
            customerConfiguration.HasKey(c => c.CustomerId);
            modelBuilder.Register<Customer>();

            var lookupConfiguration = modelBuilder.Entity<Lookup>();
            lookupConfiguration.ToTable("Lookup");
            lookupConfiguration.HasKey(l => l.LookupId);
            modelBuilder.Register<Lookup>();
        }
    }
}
