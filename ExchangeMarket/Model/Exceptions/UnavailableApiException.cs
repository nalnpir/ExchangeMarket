using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeMarket.Model.Exceptions
{
    public class UnavailableApiException : BaseApplicationException
    {
        public UnavailableApiException() 
            : base($"The exchange rate api is unavailable, please retry later")
        { }
    }
}
