using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class ModelBase
    {
        protected T? ConvertValue<T>(object val)
        {
            return val == DBNull.Value
                ? default
                : (T)val;
        }
    }
}
