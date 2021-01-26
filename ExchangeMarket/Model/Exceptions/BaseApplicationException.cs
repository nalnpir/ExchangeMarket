using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeMarket.Model.Exceptions
{
    /// <summary>
    /// Base exception in order to distinguish between app exceptions and not controlled ones
    /// </summary>
    public class BaseApplicationException : Exception
    {
        public BaseApplicationException()
        {
        }

        public BaseApplicationException(string message) : base(message)
        {
        }
    }
}
