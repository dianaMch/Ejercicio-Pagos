using PAGO.WEBSERVICE.WCF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PAGO.WEBSERVICE.WCF
{
    [ServiceContract]
    public interface IPagoService
    {

        [OperationContract]
        PagosDataOutput ConsultaEstatus(int IdPago);
    }
}
