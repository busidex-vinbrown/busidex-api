namespace Busidex.DomainModels.DTO
{
    public class LoginResetParams : LoginParams
    {
        public string TempData { get; set; }
    }
}