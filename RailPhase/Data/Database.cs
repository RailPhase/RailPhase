using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Common;
using System.Data.Entity;
using System.Reflection;

namespace RailPhase
{
    public class ModelDbContext: DbContext
    {
        public ModelDbContext(string connection) :
            base(connection)
        {
        }
    }
    public static class Database
    {
        public static Dictionary<Type, DbContext> DbContextsByModelType { get; private set; }
        public static Dictionary<Type, IEnumerable<ModelBase>> DbSetsByModelType { get; private set; }

        static Database()
        {
            DbContextsByModelType = new Dictionary<Type, DbContext>();
            DbSetsByModelType = new Dictionary<Type, IEnumerable<ModelBase>>();
        }

        public static void SaveAllModelChanges()
        {
            foreach(var dbContext in DbContextsByModelType.Values)
            {
                dbContext.SaveChanges();
            }
        }

        public static void InitializeDatabase(string connection)
        {
            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

            var modelType = typeof(Model<>);
            var modelBaseType = typeof(ModelBase);

            var modelAssembly = modelBaseType.Assembly.FullName;

            // Models can be defined in any assembly that references the assembly that contains the model base class
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetReferencedAssemblies().Where(r => r.FullName == modelAssembly).Count() > 0 || a.FullName == modelAssembly);

            foreach (var assembly in assemblies)
            {
                // Find all types that are derived from ModelBase, as they are possible candidates to be models
                var entityTypes = assembly
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(modelBaseType));

                foreach(var type in entityTypes)
                {
                    Type modelGenericType;

                    // Make sure the type is actually derived from Model<type>.
                    // Swallow the ArgumentExceptions that MakeGenericType will throw because of the generic constraints
                    // if type is not a subclass of Model<type>.
                    try
                    {
                        modelGenericType = modelType.MakeGenericType(type);
                    }
                    catch(ArgumentException)
                    {
                        continue;
                    }

                    // Initialize the types that are actual models
                    if (type.IsSubclassOf(modelGenericType))
                    {
                        var initMethod = modelGenericType.GetMethod("InitializeDatabaseConnection", BindingFlags.Static | BindingFlags.NonPublic);
                        var dbContext = (DbContext)initMethod.Invoke(null, new object[] { connection });

                        var objectsProperty = modelGenericType.GetProperty("Objects", BindingFlags.Static | BindingFlags.Public);

                        var dbSet = ((IEnumerable<ModelBase>)objectsProperty.GetValue(null)).Cast<ModelBase>();

                        DbContextsByModelType.Add(type, dbContext);
                        DbSetsByModelType.Add(type, dbSet);
                    }
                }
            }
        }
    }
}
