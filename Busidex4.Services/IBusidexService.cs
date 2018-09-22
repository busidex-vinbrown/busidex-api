using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Busidex.DAL;

namespace Busidex4.Services {
    [ServiceContract]
    public interface IBusidexService {

        //[OperationContract]
        //[WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetMyBusidex?id={userId}", Method="GET")]
        //List<Card> GetMyBusidex(long userId);

        //[OperationContract]
        ////[WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/UpdateLizzidex?c={Coffee}&t={Thing}", Method="GET")]
        //void UpdateLizzidex(int coffee, int thing);

        //[OperationContract]
        ////[WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetLizzidex", Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]        
        //string GetLizzidex();

       
        //[WebInvoke(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/GetMyBusidex", Method = "GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]        
        [OperationContract]
        List<Card> GetMyBusidex(long userId);

    }
    [DataContract]
    [Serializable]
    public class LizzidexCount {
        [DataMember]
        public int CoffeeCount { get; set; }

        [DataMember]
        public int ThingCount { get; set; }
    }
}
