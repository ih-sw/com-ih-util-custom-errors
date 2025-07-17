using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace com.ih.util.custom.errors.Domain
{
    [Serializable]
    public class CustomErrorException : Exception, ISerializable
    {
        public virtual string message { get { return "Internal Error"; } }
        public virtual string code { get { return "internal_server_error"; } }
        public virtual int status { get { return 500; } }
        public virtual Dictionary<string, string> details { get; set; }

        public virtual string contentBodyRequest { get; set; }
        public virtual string contentBodyResponse { get; set; }

        public override string Message
        {
            get
            {
                var info = base.Message + " ::: ADITIONAL INFORMATION => STATUS: " + status + " | CODE: " + code + " | MESSAGE: " + message + ".";

                if (!string.IsNullOrEmpty(contentBodyRequest))
                {
                    info += " - [ DATA REQUEST: " + contentBodyRequest + " ]";
                }

                if (!string.IsNullOrEmpty(contentBodyResponse))
                {
                    info += " - [ DATA RESPONSE: " + contentBodyResponse + " ]";
                }

                return info;
            }
        }
    }
}