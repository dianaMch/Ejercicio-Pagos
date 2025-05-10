using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PAGO.WEBSERVICE.Models
{

    [DataContract]
    public class PagosDataOutput
    {
        [DataMember]
        public int IdPago { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public string Error { get; set; }
    }
}