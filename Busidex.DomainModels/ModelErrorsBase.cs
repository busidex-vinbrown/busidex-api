using System;
using System.Collections.Generic;

namespace Busidex.DomainModels {

    [Serializable]
    public class ModelErrorsBase {

        public Dictionary<string, string> ErrorCollection { get; set; }

        public ModelErrorsBase()
        {
            ErrorCollection = new Dictionary<string, string>();
        }
    }
}
