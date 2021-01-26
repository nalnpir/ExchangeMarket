using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeMarket.Model.Exceptions
{
    public class InvalidCurrencyException : BaseApplicationException
    {
        public InvalidCurrencyException(string currency) :
            base($"The provided currency ({currency}) is not supported")
        { }
    }
}
