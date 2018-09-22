using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Busidex.DAL;

namespace Busidex.BL {
    public interface ICommand<T>
        where T : ViewModelBase {

        void Execute();

    }
}
