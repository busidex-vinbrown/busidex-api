using System;

namespace Busidex.Api.DataAccess.DTO
{

    [Serializable]
    public partial class CardCategory : IComparable
    {

        public CardCategory(long cardId, int categoryId)
        {
            CardId = cardId;
            CategoryId = categoryId;
        }

        public long CategoryId { get; set; }
        public long CardId { get; set; }
        //public CardCategory(Card card, Category category) {
        //    Card = card;
        //    Category = category;
        //}

        //public CardCategory(Card card, int categoryId) {
        //    Card = card;
        //    CategoryId = categoryId;
        //}

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var otherCard = obj as CardCategory;
            if (otherCard != null)
            {
                if (otherCard.CategoryId == CategoryId)
                {
                    return 0;
                }
            }
            return -1;
        }

        #endregion
    }
}