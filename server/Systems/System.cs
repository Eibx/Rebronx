namespace Rebronx.Server.Systems
{
    public class System
    {
        protected T GetData<T>(Message message) where T : class
        {
            if (string.IsNullOrEmpty(message?.Data))
                return null;

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
