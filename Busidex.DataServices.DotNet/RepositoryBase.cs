using Busidex.DataAccess.DotNet;
using Busidex.DomainModels.DotNet.DTO;

namespace Busidex.DataServices.DotNet
{
    public abstract class RepositoryBase : IApplicationRepository
    {

        //protected readonly IBusidexDataContext BusidexDAL;
        protected readonly BusidexDao _dao;
        protected readonly string _connectionString;

        protected RepositoryBase(string connectionString = "")
        {
            //BusidexDAL = busidexDal;
            _connectionString = connectionString;
            if (!string.IsNullOrEmpty(_connectionString))
            {
                _dao = new BusidexDao(_connectionString);
            }
            else
            {
                _dao = new BusidexDao();
            }
        }

        public async Task<BusidexUser> GetBusidexUserById(long userId)
        {
            return await _dao.GetBusidexUserById(userId);
        }

        //public List<UserTerm> GetUserTerms(long userId)
        //{
        //    return _dao.GetUserTerms(userId);
        //}

        //public async Task AcceptUserTerms(long userId)
        //{
        //    await _dao.AcceptUserTerms(userId);
        //}

        public async Task SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            await _dao.SaveApplicationError(error, innerException, stackTrace, userId);
        }

        public async Task SaveApplicationError(Exception ex, long userId)
        {
            string error = ex.Message;
            string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            string stackTrace = ex.StackTrace;
            await SaveApplicationError(error, innerException, stackTrace, userId);
        }
    }
}
