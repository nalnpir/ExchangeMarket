using ExchangeMarket.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExchangeMarket.Model.Exceptions;
using System;
using Serilog;

namespace ExchangeMarket.Infrastructure.Filters
{
    /// <summary>
    /// The aim of this class is a catch all exceptions and make appropiate http responses
    /// </summary>
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var result = new JsonResult(
                this.GetError(context.Exception)
            );

            if (result.StatusCode == (int)HttpStatusCode.InternalServerError)
                Log.Logger.Error(context.Exception.Message);

            context.Result = result;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Error Handling depending upon the type of exception
        /// </summary>
        /// <param name="ex">exception</param>
        /// <returns>object to send to the user</returns>
        private ErrorResponse GetError(Exception ex) => ex switch
        {
            MonthlyLimitException appEx => new ErrorResponse((int)HttpStatusCode.Unauthorized, appEx.Message),
            BaseApplicationException appEx => new ErrorResponse((int)HttpStatusCode.BadRequest, appEx.Message),
            _ => new ErrorResponse((int)HttpStatusCode.InternalServerError, "oops, something went wrong")
        };
    }
}
