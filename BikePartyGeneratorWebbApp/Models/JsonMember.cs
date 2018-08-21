using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikePartyGeneratorWebbApp.Models
{
    public class JsonMember
    {
        public int Id { get; set; }
        public List<string> names { get; set; }
        public string address { get; set; }
        public string phone { get; set; }

        public string duty { get; set; }

        public string dinner { get; set; }
        public string starter { get; set; }
        public string dessert { get; set; }
    }
}