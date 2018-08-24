using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikePartyGeneratorWebbApp.Models
{
    public class JsonScheme
    {     
        public List<JsonMember> dateList;

        public JsonScheme()
        {
            dateList = new List<JsonMember>();
        }

        public string message { get; set; }
    }
}