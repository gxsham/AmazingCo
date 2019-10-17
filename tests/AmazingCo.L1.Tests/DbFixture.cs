using System;
using AmazingCo.Data;
using Microsoft.EntityFrameworkCore;

namespace AmazingCo.L1.Tests
{
    public class DbFixture: IDisposable
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AmazingCo_L1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private bool disposedValue = false;

        public ApplicationContext Context;


        public DbFixture()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseSqlServer(_connectionString);
            
            Context = new ApplicationContext(optionsBuilder.Options);
            Context.Database.EnsureCreated();
            Context.Seed();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Context.Database.EnsureDeleted();
                    Context.Dispose();
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
