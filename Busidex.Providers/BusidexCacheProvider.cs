namespace Busidex.Providers
{

    //public class BusidexCacheProvider
    //{

    //    public enum CachKeys
    //    {
    //        MyBusidex,
    //        Categories,
    //        CurrentUser,
    //        IsACardOwner,
    //        MyBusiGroups,
    //        SharedCards
    //    }

    //    public object GetFromCache(object cacheKey)
    //    {
    //        var ctx = System.Web.HttpContext.Current;
    //        if (ctx != null && ctx.Session != null)
    //        {
    //            return ctx.Session[cacheKey.ToString()];
    //        }
    //        return null;
    //    }

    //    public void UpdateCache(object cacheKey, object thing)
    //    {
    //        var ctx = System.Web.HttpContext.Current;
    //        if (ctx != null && ctx.Session != null)
    //        {
    //            ctx.Session[cacheKey.ToString()] = thing;
    //        }
    //    }
    //}
}
