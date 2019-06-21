using System;
using System.Collections.Generic;
using Busidex.DataAccess;
using Busidex.DomainModels;
using Busidex.Services.Interfaces;

namespace Busidex.Services
{
    public abstract class RepositoryBase : IApplicationRepository
    {

        protected readonly IBusidexDataContext BusidexDAL;
        protected readonly BusidexDao _dao = new BusidexDao();

        protected RepositoryBase(IBusidexDataContext busidexDal)
        {
            BusidexDAL = busidexDal;            
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

        public void SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            BusidexDAL.SaveApplicationError(error, innerException, stackTrace, userId);
        }
        public void SaveApplicationError(Exception ex, long userId)
        {
            string error = ex.Message;
            string innerException = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            string stackTrace = ex.StackTrace;
            SaveApplicationError(error, innerException, stackTrace, userId);
        }
    }
}
