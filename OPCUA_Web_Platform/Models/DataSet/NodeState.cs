using Newtonsoft.Json.Linq;

namespace WebPlatform.Models.DataSet
{
    public class VariableState
    {
        public JToken Value { get; set; }

        public bool IsValid => Value != null;
    }
}
