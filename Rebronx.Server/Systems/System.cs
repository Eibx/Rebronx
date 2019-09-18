namespace Rebronx.Server.Systems
{
    public class System
    {
        public T GetData<T>(Message message) where T : class
        {
            if (message == null || string.IsNullOrEmpty(message.Data))
            {
                return null;
            }

            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message.Data);
            }
            catch
            {
                return null;
            }
        }
    }
}
