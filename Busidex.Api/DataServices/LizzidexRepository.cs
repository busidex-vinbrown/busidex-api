﻿using Busidex.Api.DataAccess;

namespace Busidex.Api.DataServices
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
