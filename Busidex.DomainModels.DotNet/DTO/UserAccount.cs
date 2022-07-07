using System;
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class UserAccount : ModelBase
    {
        public long UserAccountId { get; set; }
        public long UserId { get; set; }
        public int AccountTypeId { get; set; }
        public DateTime Created { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; }
        public Guid? ActivationToken { get; set; }
        public string DisplayName { get; set; }

        public virtual AccountType AccountType { get; set; }
        public virtual BusidexUser BusidexUser { get; set; }
        public string ReferredBy { get; set; }
        public bool? OnboardingComplete { get; set; }

        public UserAccount() { }
        public UserAccount(IDataReader reader)
        {
            UserAccountId = ConvertValue<long>(reader["UserAccountId"]);
            UserId = ConvertValue<long>(reader["UserId"]);
            AccountTypeId = ConvertValue<int>(reader["AccountTypeId"]);
            Created = ConvertValue<DateTime>(reader["Created"]);
            Active = ConvertValue<bool>(reader["AccountTypeActive"]);
            Notes = ConvertValue<string>(reader["Notes"]);
            ActivationToken = ConvertValue<Guid?>(reader["ActivationToken"]);
            DisplayName = ConvertValue<string>(reader["DisplayName"]);
            //AccountType = accountType;
            //BusidexUser = busidexUser;
            ReferredBy = ConvertValue<string>(reader["ReferredBy"]);
            OnboardingComplete = ConvertValue<bool?>(reader["OnboardingComplete"]);
        }
    }
}
