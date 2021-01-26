using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeMarket.Model.Exceptions
{
    public class MonthlyLimitException : BaseApplicationException
    {
        public MonthlyLimitException(string currency)
            : base($"You have reached the monthly limit for the Currency: {currency}")
        { }
    }
}
