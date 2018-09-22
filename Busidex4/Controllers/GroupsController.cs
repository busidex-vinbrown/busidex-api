using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex4.Models;

namespace Busidex4.Controllers
{
    [Authorize]
    public class GroupsController : BaseController
    {

        public GroupsController(ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {
        }

        public ActionResult Mine()
        {
            var userId = GetUserId();
            var model = _cardRepository.GetMyBusiGroups(userId);
            return View(model);
        }

        public ActionResult Details(long id)
        {
            var cards = _cardRepository.GetBusiGroupCards(id, true);
            var group =  _cardRepository.GetBusiGroupById(id);
            var model = new BusiGroupModel
                        {
                            Busigroup = group,
                            BusigroupCards = cards
                        };
            return View(model);
        }

        
        
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult Add()
        {
            var userId = GetUserId();
            var myBusidex = _cardRepository.GetMyBusidex(userId, true);

            myBusidex.ForEach(c=>c.Selected = false);

            var allCards = myBusidex.Select(card => card).ToList();
            List<string> allTags = (from cards in allCards
                                    from tag in cards.Card.Tags
                                    select tag.Text).ToList();

            var tags = (from tag in allTags
                        group tag by tag into t
                        select new { key = t.First(), Value = t.Count() })
                .ToDictionary(t => t.key, t => t.Value);

            var model = new AddGroupModel
            {
                Busigroup = new Group(),
                Busidex = myBusidex,
                TagCloud = tags
            };
            return PartialView("_UpdateGroup", model);
        }

        public void Delete(long id)
        {
            _cardRepository.DeleteGroup(id);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult Edit(long id)
        {
            var userId = GetUserId();
            var myBusidex = _cardRepository.GetMyBusidex(userId, true);

            var allCards = myBusidex.Select(card => card).ToList();
            List<string> allTags = (from cards in allCards
                                    from tag in cards.Card.Tags
                                    select tag.Text).ToList();

            var tags = (from tag in allTags
                        group tag by tag into t
                        select new { key = t.First(), Value = t.Count() })
                .ToDictionary(t => t.key, t => t.Value);

            var group = _cardRepository.GetBusiGroupById(id) ?? new Group();
            var groupCards = _cardRepository.GetBusiGroupCards(id);
            allCards.ForEach(c => c.Selected = groupCards.Any(gc => gc.CardId == c.CardId));

            var model = new AddGroupModel
            {
                Busigroup = group,
                Busidex = myBusidex,
                TagCloud = tags
            };
            return PartialView("_UpdateGroup", model);
        }

        public JsonResult SaveGroupCardNotes(long id, string notes)
        {
            _cardRepository.SaveGroupCardNotes(id, notes);

            return Json(new { Success = true });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public long Update(long? id, string cardIds, string description)
        {
            /*
             * This method wil create the following:
             * 1 group record
             * 1 UserGroupCard record for each person in the group
             * 1 UserCard record for each person in the group (if they don't already have the card in their busidex)
             * 
             * We will also send an email to each person added to the group
             */
            if (string.IsNullOrEmpty(cardIds) || string.IsNullOrEmpty(description))
            {
                return -1;
            }

            var userId = GetUserId();
            var group = new Group
            {
                Description = description,
                UserId = userId,
                GroupId = id.GetValueOrDefault()
            };

            // Get the card info for each card in the list of cardIds
            var cardIdList = new List<long>();
            cardIdList.AddRange(cardIds.Split(',')
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(cardId => Convert.ToInt64(cardId)));

            if (id.GetValueOrDefault() <= 0)
            {
                id = _cardRepository.AddGroup(group);
                if (id.GetValueOrDefault() > 0)
                {
                    _cardRepository.AddUserGroupCards(cardIds, id.Value, userId);
                }
            }
            else
            {
                id = _cardRepository.UpdateGroup(group);
                if (id.GetValueOrDefault() > 0)
                {
                    var groupCards = _cardRepository.GetBusiGroupCards(id.Value);

                    #region Add new Cards to the group
                    var newCardIds = new List<long>();
                    newCardIds.AddRange((from gc in cardIdList
                                         where !groupCards.Exists(cl => cl.CardId == gc)
                        select gc).ToList());
                    if (newCardIds.Count > 0)
                    {
                        _cardRepository.AddUserGroupCards(string.Join(",", newCardIds), id.Value, userId);
                    }
                    #endregion

                    #region  Process any cards that have been removed from the group

                    var existingCardIds = new List<long?>();
                    existingCardIds.AddRange((from gc in groupCards
                                             select gc.CardId).ToList());

                    var deletedCardIds = new List<long?>();
                    deletedCardIds.AddRange((from gc in existingCardIds
                        where !cardIdList.Exists(cl => cl == gc.GetValueOrDefault())
                        select gc).ToList());

                    if (deletedCardIds.Count > 0)
                    {
                        _cardRepository.RemoveUserGroupCards(string.Join(",", deletedCardIds), id.Value, userId);
                    }

                    #endregion
                }
            }
            return id.GetValueOrDefault();
        }
    }
}
