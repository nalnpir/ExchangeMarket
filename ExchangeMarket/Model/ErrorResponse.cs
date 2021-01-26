using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeMarket.Model
{
    /// <summary>
    /// StatusCode: Web error code
    /// Message: Error Message to show end user
    /// </summary>
    public record ErrorResponse(int StatusCode, string Message);
}
