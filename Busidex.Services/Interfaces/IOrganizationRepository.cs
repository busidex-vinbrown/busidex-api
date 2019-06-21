using System.Collections.Generic;
using Busidex.DomainModels;

namespace Busidex.Services.Interfaces
{
    public interface IOrganizationRepository
    {
        Organization GetOrganizationById(long id);
        List<Organization> GetOrganizationsByUserId(long userId);
        void UpdateOrganization(Organization organization, long userId);
        void LogoToFile(Organization organization);
        List<OrganizationGuest> GetOrganizationGuests(long organizationId, long userId);
        List<CardDetailModel> GetOrganizationCards(long organizationId, long userId, bool includeImages = false);
        List<Group> GetOrganizationGroups(long ownerId, long userId);
        void AddOrganization(Organization organization, long userId);
        void AddOrganizationCard(long organizationId, long cardId, long userId);
        void DeleteOrganizationCard(long organizationId, long cardId, long userId);
        List<UserCard> GetOrganizationReferrals(long userId, long organizationId);
    }
}
