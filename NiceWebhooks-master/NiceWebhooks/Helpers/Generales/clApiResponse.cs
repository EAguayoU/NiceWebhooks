namespace NiceWebhooks.Helpers.Generales
{
    public class clApiResponse
    {
        public bool success = false;
        public DateTime date = DateTime.Now;
        public int records = 0;
        public string message = "";
        public string messageDev = "";
        public string executionTime = "";
        public dynamic data;
    }
}
