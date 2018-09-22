using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Busidex.Services
{
    [ServiceContract]
    public interface IBusidexService
    {

        //[OperationContract]
        //[WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetMyBusidex?id={userId}", Method="GET")]
        //List<Card> GetMyBusidex(long userId);

        //[OperationContract]
        //[WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateLizzidex?c={Coffee}&t={Thing}", Method = "GET")]
        //void UpdateLizzidex(int coffee, int thing);

        //[OperationContract]
        //[WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetLizzidex", Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //string GetLizzidex();


    }
    [DataContract]

    [Serializable]
    public class LizzidexCount
    {
        [DataMember]
        public int CoffeeCount { get; set; }

        [DataMember]
        public int ThingCount { get; set; }
    }
}
