using System.Collections.Generic;
using System.Linq;

namespace Busidex.Profile.DataAccess
{
    public class BusidexDao
    {

        public CardDetailModel GetCardBySEOName(string name)
        {
            using (var ctx = new BusidexEntities())
            {
                
                var model = ctx.Database.SqlQuery<CardDetailModel>(
                    "exec dbo.usp_getCardsBySEOName @name={0}, @latitude={1}, @longitude={2}, @radiusInMiles={3}",
                    name, null, null, null).FirstOrDefault();
                if (model != null)
                {
                    var phoneNumberData = ctx.Database.SqlQuery<PhoneNumber>("exec dbo.usp_GetCardPhoneNumbers @cardIds={0}", model.CardId).ToList();
                    var phoneNumberTypes =ctx.Database.SqlQuery<PhoneNumberType>("exec dbo.usp_GetAllPhoneNumberTypes")
                        .ToDictionary(t => t.PhoneNumberTypeId, t => t.Name);
                    var phoneNumbers = phoneNumberData.Select(p => new PhoneNumber
                    {
                        CardId = p.CardId,
                        Created = p.Created,
                        Deleted = p.Deleted,
                        Extension = p.Extension,
                        Number = p.Number,
                        PhoneNumberTypeId = p.PhoneNumberTypeId,
                        Updated = p.Updated,
                        PhoneNumberId = p.PhoneNumberId,
                        PhoneNumberType = new PhoneNumberType
                        {
                            PhoneNumberTypeId = p.PhoneNumberTypeId,
                            Name = phoneNumberTypes[p.PhoneNumberTypeId]
                        }
                    }).ToList();
                    var tagData = ctx.Database.SqlQuery<CardTag>("exec dbo.usp_GetCardTagsByIds @CardIds={0}", model.CardId).ToList();
                    var stateCodes = ctx.Database.SqlQuery<StateCode>("exec dbo.usp_GetAllStateCodes").ToList();
                    var addressData = ctx.Database.SqlQuery<CardAddress>("exec dbo.usp_GetCardAddressesByIds @CardIds={0}", model.CardId).ToList();
                    var cardAddresses = addressData.Select(a => new CardAddress
                    {
                        Address1 = a.Address1,
                        Address2 = a.Address2,
                        CardAddressId = a.CardAddressId,
                        CardId = a.CardId,
                        City = a.City,
                        Country = a.Country,
                        Deleted = a.Deleted,
                        Region = a.Region,
                        State = stateCodes.SingleOrDefault(sc => sc.StateCodeId == a.StateCodeId) ?? new StateCode(),
                        ZipCode = a.ZipCode
                    }).ToList();
                    var tags = tagData.Where(t => t.CardId == model.CardId)
                        .Select(t => new Tag {TagId = t.TagId, Text = t.Text, TagType = (TagType) t.TagTypeId})
                        .ToList();

                    model.Tags = tags ?? new List<Tag>();
                    model.PhoneNumbers = phoneNumbers ?? new List<PhoneNumber>();
                    model.Addresses = cardAddresses ?? new List<CardAddress>();
                }

                return model;

            }
        }

        public List<CardDetailModel> GetAllCards()
        {
            using (var ctx = new BusidexEntities())
            {
                var cards = ctx.Database.SqlQuery<CardDetailModel>("exec dbo.usp_getAllCards");
                return cards.Where(c=>c.OwnerId.HasValue).ToList();
            }
        } 
    }
}
