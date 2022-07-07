using System;

namespace Busidex.DomainModels.DotNet.DTO {

    [Serializable]
    public abstract class ViewModelBase {

        public bool IsAdmin { get; set; }
        public ModelErrorsBase ModelErrors { get; set; }
    }
}
