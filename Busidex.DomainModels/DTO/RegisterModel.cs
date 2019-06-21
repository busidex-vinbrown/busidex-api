using System;

namespace Busidex.DomainModels.DTO
{
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string ConfirmEmail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool Agree { get; set; }
        public int AccountTypeId { get; set; }
        public string MobileNumber { get; set; }
        public string CardOwnerToken { get; set; }
        public bool IsMyCard { get; set; }
        public Guid? FrontFileId { get; set; }
        public string FrontFileType { get; set; }
        public string HumanAnswer { get; set; }
        public bool IsValid { get; set; }
        public bool IsMobile { get; set; }
        public string PromoCode { get; set; }
        public long InviteUserId { get; set; }
        public Guid? InviteCardToken { get; set; }
    }
}
