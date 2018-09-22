using System;

namespace Busidex.Api.DataAccess.DTO {

    [Serializable]
    public abstract class ViewModelBase {

        public bool IsAdmin { get; set; }
        public ModelErrorsBase ModelErrors { get; set; }
    }
}
