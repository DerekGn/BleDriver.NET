using System;

using BgApiApp.Exceptions;

using BgApiDriver;

namespace BgApiApp
{
    public abstract class BaseBlueGiga
    {
        internal T ExecuteOperation<T>(Func<T> operation) where T : BgApiResponse
        {
            var response = operation();

            if (response.result != 0)
            {
                throw new BlueGigaBleException(response.result);
            }

            return response;
        }
    }
}
