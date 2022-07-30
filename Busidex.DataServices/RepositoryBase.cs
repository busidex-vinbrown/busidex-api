using Busidex.DataAccess;
using Busidex.DomainModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Busidex.DataServices
{
    public abstract class RepositoryBase : IApplicationRepository
    {

        protected readonly IBusidexDataContext BusidexDAL;
        protected readonly BusidexDao _dao;
        protected readonly string _connectionString;

        protected RepositoryBase(IBusidexDataContext busidexDal, string connectionString = "")
        {
            BusidexDAL = busidexDal;
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

        public BusidexUser GetBusidexUserById(long userId)
        {
            return _dao.GetBusidexUserById(userId);
        }

        public List<UserTerm> GetUserTerms(long userId)
        {
            return _dao.GetUserTerms(userId);
        }

        public void AcceptUserTerms(long userId)
        {
            _dao.AcceptUserTerms(userId);
        }

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
