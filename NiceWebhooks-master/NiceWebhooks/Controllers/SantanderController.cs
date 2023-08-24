using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceWebhooks.Helpers.Generales;
using System.Dynamic;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Xml;

namespace NiceWebhooks.Controllers
{
    public class SantanderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("[controller]/WebhooksSantanderTest")]
        public ContentResult Get(string strResponse)
        {
            dynamic objRespuesta = new ExpandoObject();
            objRespuesta.success = false;
            objRespuesta.message = "No fue posible realizar el pago";
            objRespuesta.data = null;

            return Content(JsonConvert.SerializeObject(objRespuesta));
        }

        [HttpPost]
        [Route("[controller]/WebhooksSantanderTest")]
        public ContentResult Post(string strResponse)
        {
            dynamic objRespuesta = new ExpandoObject();
            objRespuesta.success = false;
            objRespuesta.message = "";
            objRespuesta.data = null;

            try
            {
                if (string.IsNullOrEmpty(strResponse))
                {
                    objRespuesta.message = "La información del pago no fue recibida correctamente o este fue rechazado por el banco(empty)";
                    return Content(JsonConvert.SerializeObject(objRespuesta));
                }
                string sURL = clGenerales.UrlEncode(strResponse);
                WebClient webClient = new WebClient();
                webClient.QueryString.Add("strResponse", sURL);
                string sDecryptedString = webClient.DownloadString("https://pagos.nice.com.mx/pay/Santander/desencriptar.php");

                //Arroja un mensaje cuando no se pudo Desencriptar
                if (sDecryptedString == "")
                {
                    objRespuesta.message = "La información del pago no fue recibida correctamente (decrypt)";
                    return Content(JsonConvert.SerializeObject(objRespuesta));
                }
                //Guardamos en Log
                dynamic objBody = new ExpandoObject();
                objBody.Webhook = "SANTANDER";
                objBody.Incoming = sDecryptedString;

                clApiRequest objApiRequest = new clApiRequest();
                objApiRequest.Method = "POST";
                objApiRequest.Url = "Sales/WebhookCall";
                objApiRequest.Body = JsonConvert.SerializeObject(objBody);
                dynamic objApiResponse = objApiRequest.RequestApi();

                //Convertimos XML a Objeto
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(sDecryptedString);
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, false);
                dynamic objXML = JsonConvert.DeserializeObject(jsonText);
                objXML = objXML.CENTEROFPAYMENTS;
                if ((string)objXML.response != "approved")
                {
                    objRespuesta.message = "El pago no fue aprobado";
                    return Content(JsonConvert.SerializeObject(objRespuesta));
                }
                
                //Se sacan los Datos de Pago
                string sReference = objXML.reference;
                var vReference = sReference.Split('-');
                string sDataBank = objXML.cc_type;
                var vDataBank = sDataBank.Split("/");
                string sOrderID = vReference[0];
                int nLanguajeID = 1;
                string sAmount = objXML.amount;
                string sAuth = objXML.auth;
                string sTrxID = objXML.foliocpagos;
                string sCard = objXML.cc_number;
                string sEspirationYear = objXML.cc_expyear;
                string sEspirationMonth = objXML.cc_expmonth;
                string sCardMask = objXML.cc_mask;
                string sCardType = objXML.cc_type;
                string sDate = objXML.date;
                string sTime = objXML.time;
                decimal xAmount = Convert.ToDecimal(sAmount);
                
                //Verificamos Valores
                if (string.IsNullOrEmpty(sOrderID) || string.IsNullOrEmpty(sAuth) || xAmount == 0) 
                {
                    objRespuesta.message = "Los valores del pago no se recibieron correctamente (auth)";
                    return Content(JsonConvert.SerializeObject(objRespuesta));
                }

                //Validamos tipo de orden
                if (sReference.Substring(0,2) != "CN")
                {
                    //ORDEN 
                    dynamic objNewOpenedorder = new ExpandoObject();
                    objNewOpenedorder.OrderOpenID = objXML.sOrderID;
                    clApiRequest objOrderDB = new clApiRequest();
                    objOrderDB.Method = "GET";
                    objOrderDB.Url = "Sales/OrderOpen";
                    objOrderDB.Body = JsonConvert.SerializeObject(objNewOpenedorder);
                    dynamic objOrderDBResponse = objOrderDB.RequestApi();

                    if (objOrderDBResponse.success != true)
                    {
                        objRespuesta.message = "La orden ingresada ya fue pegada o no existe (orderopen)";
                        return Content(JsonConvert.SerializeObject(objRespuesta));
                    }

                    
                }
                else
                {
                    //CREDINICE
                }

            }
            catch (Exception ex) 
            {
                objRespuesta.message = ex.Message;
            }
            return Content(JsonConvert.SerializeObject(objRespuesta));
        }
    }
}
