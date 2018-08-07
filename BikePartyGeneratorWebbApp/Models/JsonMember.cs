using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikePartyGeneratorWebbApp.Models
{
    public class JsonMember
    {
        public JsonMember(string name)
        {
            this.name = name;
        }

        public JsonMember(){}
        public int Id { get; set; }
        public string name { get; set; }

        public string duty { get; set; }

        public string dinner { get; set; }
        public string starter { get; set; }
        public string dessert { get; set; }
    }
}