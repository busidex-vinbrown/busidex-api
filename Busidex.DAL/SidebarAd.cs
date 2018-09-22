using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busidex.DAL {
    public class SidebarAd {
        public string DomID { get; set; }
        public int CardId { get; set; }
        public string Title { get; set; }

        public SidebarAd(string id, int cardId, string title) {
            this.DomID = id;
            this.CardId = cardId;
            this.Title = title;
        }
    }
}