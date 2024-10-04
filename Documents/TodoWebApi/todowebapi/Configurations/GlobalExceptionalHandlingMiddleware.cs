using System.Net;
using System.Text.Json;
using todowebapi.Exceptions;
using KeyNotFoundException = todowebapi.Exceptions.KeyNotFoundException;
using NotFoundException = todowebapi.Exceptions.NotFoundException;
using BadRequestException =  todowebapi.Exceptions.BadRequestException;
using NotImplementedException = todowebapi.Exceptions.NotImplementedExcpetion;
using UnAuthorisedException =  todowebapi.Exceptions.UnAuthorizedException;
namespace todowebapi.Configurations
{
    public class GlobalExceptionalHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionalHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);  
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotImplementedException => StatusCodes.Status501NotImplemented,
                NotFoundException => StatusCodes.Status404NotFound,
                KeyNotFoundException => StatusCodes.Status406NotAcceptable,
                UnAuthorisedException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };


            return context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message
            }.ToString());
        }
    }
}
