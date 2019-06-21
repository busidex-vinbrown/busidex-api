using System;

namespace Busidex.DomainModels {

    [Serializable]
    public abstract class ViewModelBase {

        public bool IsAdmin { get; set; }
        public ModelErrorsBase ModelErrors { get; set; }
    }
}
