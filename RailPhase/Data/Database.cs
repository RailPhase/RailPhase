using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Reflection;

namespace RailPhase.Data
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(string connectionName):
            base(connectionName)
        {
            Users.Count();
        }

        public DbSet<Auth.User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var entityTypes = assembly
                    .GetTypes()
                    .Where(t => (t.IsSubclassOf(typeof(ModelBase)) && t.BaseType != typeof(ModelBase)));

                foreach (var type in entityTypes)
                {
                    entityMethod.MakeGenericMethod(type)
                        .Invoke(modelBuilder, new object[] { });

                    var initMethod = type.BaseType.GetMethod("InitializeDatabaseConnection", BindingFlags.Static | BindingFlags.NonPublic);

                    initMethod.Invoke(null, new object[] { this });
                }
            }
        }
    }
}
