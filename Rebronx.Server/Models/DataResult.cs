using System.Collections.Generic;

namespace Rebronx.Server.Models
{
    public class DataResult<T>
    {
        public List<T> Data { get; set; }
    }
}