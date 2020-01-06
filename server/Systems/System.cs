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
                // TODO: use System.Text.Json
                // TODO: Remove Newtonsoft.Json.JsonConvert
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message.Data);
            }
            catch
            {
                return null;
            }
        }
    }
}
