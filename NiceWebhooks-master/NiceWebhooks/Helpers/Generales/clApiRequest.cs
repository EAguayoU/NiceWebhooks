using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text;


namespace NiceWebhooks.Helpers.Generales
{
    public class clApiRequest
    {
        private int nUserID = 215613;
        public string Url { get; set; } = "";
        public string Method { get; set; } = "";
        public string Body { get; set; } = "{}";

        public dynamic RequestApi(int nEnviroment = 1)
        {
            clApiResponse objApiResponse = new clApiResponse();

            //Varibales
            string sApiUrl = "";
            if (nEnviroment == 777)
                sApiUrl = "http://45.232.254.14:677/" + Url;
            else if (nEnviroment == 666)
                sApiUrl = "http://45.232.254.14:667/" + Url;
            else if (nEnviroment == 1)
                sApiUrl = "https://nesgoapi.azurewebsites.net/" + Url;

            string sToken = "BASIC " + Convert.ToBase64String(Encoding.ASCII.GetBytes("NES:Pruebas"));
            var vRequest = new RestRequest();

            //Metodo
            if (Method.ToUpper() == "GET")
            {
                vRequest = new RestRequest("", RestSharp.Method.Get);
                vRequest.AddHeader("Parameters", Body);
            }
            else if (Method.ToUpper() == "POST")
            {
                vRequest = new RestRequest("", RestSharp.Method.Post);
                vRequest.AddStringBody(Body, DataFormat.Json);
            }
            else if (Method.ToUpper() == "PUT")
            {
                vRequest = new RestRequest("", RestSharp.Method.Put);
                vRequest.AddStringBody(Body, DataFormat.Json);
            }
            else if (Method.ToUpper() == "PATCH")
            {
                vRequest = new RestRequest("", RestSharp.Method.Patch);
                vRequest.AddStringBody(Body, DataFormat.Json);
            }
            else if (Method.ToUpper() == "DELETE")
            {
                vRequest = new RestRequest("", RestSharp.Method.Delete);

                //Modificamos ApiUrl
                dynamic objBody = JsonConvert.DeserializeObject(Body);
                foreach (var c in objBody)
                    foreach (object i in c)
                        sApiUrl += "/" + i.GetType().GetProperty("Value").GetValue(i);
            }
            else
            {
                objApiResponse.success = false;
                objApiResponse.message = "No ha especificado un método válido para ejecutar esta api";
                objApiResponse.messageDev = "El método '" + Method + "' es inválido";
                goto Final;
            }

            //Header
            vRequest.AddHeader("UserID", nUserID );
            vRequest.AddHeader("Environment", nEnviroment);
            vRequest.AddHeader("Authorization", sToken);
            vRequest.AddHeader("WindowName", "WebNiceOnline");

            //Hacemos request
            var vOptions = new RestClientOptions(sApiUrl)
            {
                ThrowOnAnyError = true,
                MaxTimeout = 600000  // 10 minutos
            };
            var vClient = new RestClient(vOptions);
            var vResponse = vClient.ExecuteAsync(vRequest).Result;
            if (vResponse.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject(vResponse.Content);
            }
            else
            {
                objApiResponse.success = false;
                objApiResponse.message = "Occurrió un error al ejecutar el api, por favor verifique";
                objApiResponse.messageDev = vResponse.StatusCode + " - " + vResponse.ErrorMessage;
            }

        Final:
            return objApiResponse;
        }


    }


}
