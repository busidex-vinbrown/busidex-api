using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Busidex.BL.Interfaces {
    public interface IApplicationRepository
    {

        void SaveApplicationError(string error, string innerException, string stackTrace, long userId);
    }
}
