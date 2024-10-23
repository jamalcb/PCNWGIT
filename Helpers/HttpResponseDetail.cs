namespace PCNW.Helpers
{
    public class HttpResponseDetail<T>
    {
        public bool success { get; set; }
        public string statusMessage { get; set; }
        public T data { get; set; }
        public string statusCode { get; set; }
        public int TempValue { get; set; } = 0;
        public T Value { get; set; }
    }
}
