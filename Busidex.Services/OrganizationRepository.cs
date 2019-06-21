using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Busidex.DataAccess;
using Busidex.DomainModels;
using Busidex.Services.Interfaces;
using Microsoft.WindowsAzure.Storage;

namespace Busidex.Services
{
    public class OrganizationRepository : RepositoryBase, IOrganizationRepository
    {
        //readonly BusidexDao _dao = new BusidexDao();

        public OrganizationRepository(IBusidexDataContext busidexDal)
            : base(busidexDal)
        {
        }

        public Organization GetOrganizationById(long id)
        {
            return _dao.GetOrganizationById(id);
        }

        public List<Organization> GetOrganizationsByUserId(long userId)
        {
            return _dao.GetOrganizationsByUserId(userId);
        }

        public void UpdateOrganization(Organization organization, long userId)
        {
            _dao.UpdateOrganization(organization, userId);
        }

        public void AddOrganization(Organization organization, long userId)
        {
            _dao.AddOrganization(organization, userId);
        }

        public void AddOrganizationCard(long organizationId, long cardId, long userId)
        {
            _dao.AddOrganizationCard(organizationId, cardId, userId);    
        }

        public void DeleteOrganizationCard(long organizationId, long cardId, long userId)
        {
            _dao.DeleteOrganizationCard(organizationId, cardId, userId);
        }

        public List<OrganizationGuest> GetOrganizationGuests(long organizationId, long userId)
        {
            return _dao.GetOrganizationGuests(organizationId, userId);
        }

        public List<CardDetailModel> GetOrganizationCards(long organizationId, long userId, bool includeImages = false)
        {
            return _dao.GetOrganizationCards(organizationId, userId, includeImages);
        }

        public List<Group> GetOrganizationGroups(long ownerId, long userId)
        {
            return _dao.GetOrganizationGroups(ownerId, userId);
        }

        public List<UserCard> GetOrganizationReferrals(long userId, long organizationId)
        {
            return _dao.GetOrganizationReferrals(userId, organizationId);
        }

        public void LogoToFile(Organization organization)
        {
            var mimeTypes = new Dictionary<string, string>
            {
                {"jpg", "image/jpeg"},
                {"jpeg", "image/jpeg"},
                {"png", "image/x-png"},
                {"gif", "image/gif"},
                {"bmp", "image/bmp"}
            };

            try
            {

                #region Update file name in DB

                if (!organization.LogoFileName.HasValue)
                {
                    organization.LogoFileName = Guid.NewGuid();
                }

                _dao.UpdateOrganizationLogoId(organization.OrganizationId, organization.LogoFileName.Value, organization.LogoType);

                #endregion

                #region Save card to file system

                var storageAccount =
                    CloudStorageAccount.Parse(
                        ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
                var blobStorage = storageAccount.CreateCloudBlobClient();
                var container =
                    blobStorage.GetContainerReference(ConfigurationManager.AppSettings["BlobStorageContainer"]);

                string uniqueBlobName = string.Format("{0}.{1}", organization.LogoFileName, organization.LogoType);
                if (organization.Logo != null)
                {
                    var frontImageBlob = container.GetBlockBlobReference(uniqueBlobName);
                    frontImageBlob.Properties.ContentType = mimeTypes[organization.LogoType];
                    frontImageBlob.Properties.CacheControl = "public, max-age=31536000";
                    frontImageBlob.UploadFromStream(new MemoryStream(organization.Logo.ToArray()));
                }               

                #endregion

            }
            catch (Exception ex)
            {
                SaveApplicationError(ex, 0);
            }
        }
    }
}