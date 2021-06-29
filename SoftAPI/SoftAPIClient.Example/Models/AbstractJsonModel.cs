using SoftAPIClient.Example.Core;

namespace SoftAPIClient.Example.Models
{
    public abstract class AbstractJsonModel
    {
        public override string ToString()
        {
            return CommonUtils.WrapToJson(this);
        }
    }
}
