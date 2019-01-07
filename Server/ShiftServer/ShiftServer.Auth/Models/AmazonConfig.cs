using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftServer.Auth.Models
{
    public class AmazonConfig
    {
        public string AWSRegion { get; set; }
        public string CLIENT_ID { get; set; }
        public string USERPOOL_ID { get; set; }
        public string IDENITYPOOL_ID { get; set; }
        public string IDENITY_PROVIDER { get; set; }

        public AmazonConfig()
        {

        }
    }
}
