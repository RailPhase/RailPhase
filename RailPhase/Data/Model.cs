using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace RailPhase
{
    public abstract class ModelBase
    {
    }

    public abstract class Model<T>: ModelBase
        where T : Model<T>
    {
        public class DataContext: ModelDbContext
        {
            public DataContext(string connection):
                base(connection)
            {}

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                DbModel = modelBuilder.Build(this.Database.Connection);
                EdmType = DbModel.ConceptualModel.EntityTypes.Where(t => t.FullName == typeof(T).FullName).Single();
            }

            DbModel DbModel;
            EntityType EdmType;

            public DbSet<T> Objects { get; set; }
        }

        public static DbSet<T> Objects
        {
            get; private set;
        }

        public static DataContext DbContext
        {
            get; private set;
        }

        protected static DbContext InitializeDatabaseConnection(string connection)
        {
            DbContext = new DataContext(connection);
            Objects = DbContext.Set<T>();

            return DbContext;
        }
    }
}
