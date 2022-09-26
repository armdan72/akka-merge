namespace EventSourcing
{
    public class Evt
    {
        public Evt(string data)
        {
            Data = data;
        }

        public string Data { get; }
    }
}
