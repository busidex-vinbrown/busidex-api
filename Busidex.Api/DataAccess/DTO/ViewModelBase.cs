using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Busidex.Api.DataAccess.DTO {

    [Serializable]
    public abstract class ViewModelBase: SerializationBinder
    {

        public bool IsAdmin { get; set; }
        public ModelErrorsBase ModelErrors { get; set; }

        public override Type BindToType(string assemblyName, string typeName)
        {
            Type tyType = null;
            string sShortAssemblyName = assemblyName.Split(',')[0];

            Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly ayAssembly in ayAssemblies)
            {
                if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0])
                {
                    tyType = ayAssembly.GetType(typeName);
                    break;
                }
            }
            return tyType;
        }
    }
}
