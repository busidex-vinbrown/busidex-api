using System;
using System.Collections.Generic;

namespace Busidex.Api.DataAccess.DTO {

    [Serializable]
    public class ModelErrorsBase {

        public Dictionary<string, string> ErrorCollection { get; set; }

        public ModelErrorsBase()
        {
            ErrorCollection = new Dictionary<string, string>();
        }
    }
}
