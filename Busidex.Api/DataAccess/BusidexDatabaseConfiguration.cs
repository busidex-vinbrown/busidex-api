using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Runtime.Remoting.Messaging;

namespace Busidex.Api.DataAccess
{
    
    public class BusidexDatabaseConfiguration : DbConfiguration
    {
        public BusidexDatabaseConfiguration()
        {
            //SetExecutionStrategy(
            //    "System.Data.SqlClient",
            //    () => new SqlAzureExecutionStrategy(5, TimeSpan.FromSeconds(30)));
            this.SetExecutionStrategy("System.Data.SqlClient", () => SuspendExecutionStrategy
              ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
              : new SqlAzureExecutionStrategy());
        }

        public static bool SuspendExecutionStrategy
        {
            get
            {
                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false;
            }
            set
            {
                CallContext.LogicalSetData("SuspendExecutionStrategy", value);
            }
        }
    }
}