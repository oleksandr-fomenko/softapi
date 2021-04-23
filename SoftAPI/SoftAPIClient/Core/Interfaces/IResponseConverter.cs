using System;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Core.Interfaces
{
    public interface IResponseConverter
    {
        Response Convert(Func<Request> request);
    }
}
