using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Busidex.DAL;

namespace Busidex.BL
{
    public class LizzidexRepository
    {

        private IBusidexDataContext bdc;

        public LizzidexRepository(IBusidexDataContext _bdc)
        {
            this.bdc = _bdc;
        }

        //public Lizzidex GetLizzidex() {

        //    Lizzidex lizzy = bdc.GetLizzidex();
        //    return lizzy;
        //}

        //public void UpdateLizzidex(int Coffee, int Thing) {
        //    bdc.UpdateLizzidex(Coffee, Thing);
        //}
    }
}
