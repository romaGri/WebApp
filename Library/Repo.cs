using System;
using Library.Torrents;
using System.Collections.Generic;
using EFCore.BulkExtensions;

namespace Library
{
    public class Repo : IDisposable
    {
        private FEDbContext dbContext;

        public Repo()
        {
            dbContext = new FEDbContext();
        }

        public void Insert<T>(IList<T> torrentItems, int packageSize = 1000) where T : class, IRootInterface 
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                dbContext.BulkInsert(torrentItems, new BulkConfig { BatchSize = packageSize });
                transaction.Commit();
            }
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
