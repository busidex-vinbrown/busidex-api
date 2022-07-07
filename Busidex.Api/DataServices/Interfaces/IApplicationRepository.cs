using System.Threading.Tasks;

namespace Busidex.Api.DataServices.Interfaces {
    public interface IApplicationRepository
    {
        Task SaveApplicationError(string error, string innerException, string stackTrace, long userId);
    }
}
