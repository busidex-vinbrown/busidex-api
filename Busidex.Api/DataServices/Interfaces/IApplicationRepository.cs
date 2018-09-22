namespace Busidex.Api.DataServices.Interfaces {
    public interface IApplicationRepository
    {

        void SaveApplicationError(string error, string innerException, string stackTrace, long userId);
    }
}
