using System;
using AmazingCo.Business;
using AmazingCo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AmazingCo.L2.Tests
{
    public class AppFixture : IDisposable
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AmazingCo_L2;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private bool disposedValue = false;
        private readonly IServiceProvider serviceProvider;

        public INodeBusiness nodeBusiness;

        public AppFixture()
        {
            var services = new ServiceCollection();

            services.AddScoped<IRepository, Repository>();

            //Add Business Layer
            services.AddScoped<INodeBusiness, NodeBusiness>();
            services.AddSingleton<IBackgroundWorker, BackgroundWorker>();

            services.AddDbContext<ApplicationContext>(options =>
                    options.UseSqlServer(_connectionString));

            serviceProvider = services.BuildServiceProvider();

            var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceProvider.GetService<ApplicationContext>();
            context.Database.Migrate();
            context.Seed();

            nodeBusiness = serviceProvider.GetRequiredService<INodeBusiness>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    using (var context = serviceProvider.GetService<ApplicationContext>())
                    {
                        context.Database.EnsureDeleted();
                    }
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
