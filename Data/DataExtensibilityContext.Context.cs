﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using Models.DataExtensibilityModel;
    
    public partial class DataExtensibilityContext : DbContext
    {
    	private Action<DbModelBuilder> _configuration;
        public DataExtensibilityContext()
            : base("name=DataExtensibilityContext")
        {
        }
    
        public DataExtensibilityContext(Action<DbModelBuilder> configuration)
            : base("name=DataExtensibilityContext")
        {
    		_configuration = configuration;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
    		new DataExtensibilityConfiguration().InjectConfiguration(modelBuilder);
    
            if (_configuration != null)
    		{
    			_configuration(modelBuilder);
    		}
        }
    
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Lookup> Lookups { get; set; }
    }
}
