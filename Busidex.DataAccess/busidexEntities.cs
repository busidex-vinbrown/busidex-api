using System.Data.Entity;

namespace Busidex.DataAccess
{
    [DbConfigurationType(typeof(BusidexDatabaseConfiguration))]
    public partial class busidexEntities : DbContext
    {
    }
}
