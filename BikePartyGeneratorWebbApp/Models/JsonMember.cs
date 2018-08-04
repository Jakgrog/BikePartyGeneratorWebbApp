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
            this.Name = name;
        }

        public JsonMember(){}
        public int Id { get; set; }
        public string Name { get; set; }
    }
}