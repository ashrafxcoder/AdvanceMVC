using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdvanceASPNET.Infrastructure.ActionResults
{
    /*The default implementation of ISO 8601 Dates in Json.NET breaks 
     * Internet Explorer 9 and below when it attempts to parse it with new Date(...). 
     * In other words, these parse fine in Internet Explorer 9:

        var date = new Date('2014-09-18T17:21:57.669');
        var date = new Date('2014-09-18T17:21:57.600');
        But this throws an exception:

        var date = new Date('2014-09-18T17:21:57.6');
        Internet Explorer 9's Date() implementation can't cope with anything 
     * but exactly three millisecond places. To fix this you have to override the 
     * Json.NET date format to force it. 
     */
    public class JsonNetResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            var settings = new JsonSerializerSettings
            {
                Converters = new[] {new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK"
            }}
            };
            var jsonSerializer = JsonSerializer.Create(settings);

            jsonSerializer.Serialize(response.Output, Data);
        }
    }
}