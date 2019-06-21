namespace Busidex.DomainModels.DTO
{
    public class ChangePasswordModel
    {
        public long UserId { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}