using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex.Providers;

namespace Busidex.BL
{
    public abstract class RepositoryBase : IApplicationRepository
    {

        protected readonly IBusidexDataContext BusidexDAL;
        protected readonly BusidexCacheProvider Bcp = new BusidexCacheProvider();

        protected RepositoryBase(IBusidexDataContext busidexDal)
        {
            BusidexDAL = busidexDal;
        }

        public BusidexUser GetBusidexUserById(long userId)
        {
            return BusidexDAL.GetBusidexUserById(userId);
        }

        public void SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            BusidexDAL.SaveApplicationError(error, innerException, stackTrace, userId);
        }

    }
}
