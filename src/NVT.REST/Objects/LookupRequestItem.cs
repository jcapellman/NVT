namespace NVT.REST.Objects
{
    public class LookupRequestItem
    {
        public string query { get; set; }

        public LookupRequestItem(string ipAddress)
        {
            query = ipAddress;
        }
    }
}