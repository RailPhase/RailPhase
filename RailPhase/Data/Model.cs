using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

namespace RailPhase.Data
{
    public abstract class ModelBase {}

    public abstract class Model<T>: ModelBase
        where T : Model<T>
    {
        public static DbSet<T> Objects;

        protected static void InitializeDatabaseConnection(DatabaseContext db)
        {
            Objects = db.Set<T>();
        }
    }
}
