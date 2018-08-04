using BikePartyGeneratorWebbApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BikePartyGeneratorWebbApp.Controllers
{
    public class MembersController : ApiController
    {
        private static List<JsonMember> members = new List<JsonMember>();

        public List<JsonMember> GetAllMembers()
        {
            return members;
        }

        public IHttpActionResult GetMember(int Id)
        {
            var member = members.FirstOrDefault((p) => p.Id == Id);
            if (member == null)
            {
                return NotFound();
            }
            return Ok(member);
        }

        [HttpPost]
        public IHttpActionResult AddMember([FromBody] JsonMember member)
        {
            if (member != null)
            {
                members.Add(member);
            }
            return Ok(member);

        }
    }
}
