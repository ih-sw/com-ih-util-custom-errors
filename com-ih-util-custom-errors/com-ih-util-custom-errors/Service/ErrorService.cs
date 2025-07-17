using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.ih.util.custom.errors.Domain;
using com.ih.util.custom.errors.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace com.ih.util.custom.errors.Service
{
    public class ErrorService
    {
        public static void HandleError(IApplicationBuilder errorApp)
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                Exception ex = exceptionHandlerPathFeature?.Error;

                if (ex != null && ex is CustomErrorException)
                {
                    CustomErrorException custom = (CustomErrorException)ex;

                    if (LogUtil.LogInitialized)
                    {
                        LogUtil.Register(custom);
                    }
                    
                    await WriteResponse(context, custom);
                }
                else
                {
                    if (LogUtil.LogInitialized)
                    {
                        LogUtil.Register(ex);
                    }
                    
                    await WriteResponse(context, new CustomErrorException());
                }
            });
        }

        private static async Task WriteResponse(HttpContext context, CustomErrorException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.status;

            var contentResponse = new ContentResponseVm()
            {
                code = ex.code,
                message = ex.message,
                details = ex.details != null ? ex.details : new Dictionary<string, string>()
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(contentResponse));
        }
    }

    public class ContentResponseVm
    {
        public string code { get; set; }
        public string message { get; set; }
        public Dictionary<string, string> details { get; set; }
    }
}