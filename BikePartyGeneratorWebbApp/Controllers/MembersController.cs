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
        private static int id = 0;

        public List<JsonMember> GetAllMembers()
        {
            return members;
        }

        [ActionName("GetMember")]
        public IHttpActionResult GetMember(int Id)
        {
            var member = members.FirstOrDefault((p) => p.Id == Id);
            if (member == null)
            {
                var message = string.Format("Member with id = {0} not found", id);
                return Content(HttpStatusCode.NotFound, message);
            }
            return Ok(member);
        }

        [ActionName("RemoveMember")]
        public IHttpActionResult RemoveMember(int Id)
        {
            var memberToRemove = members.FirstOrDefault(m => m.Id == Id);
            if(memberToRemove == null)
            {
                var message = string.Format("Member with id = {0} not found", id);
                return Content(HttpStatusCode.NotFound, message);
            }
            members.Remove(memberToRemove);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult AddMember([FromBody] JsonMember member)
        {
            member.Id = id;
            id = id + 1;

            if (member != null)
            {
                members.Add(member);
            }
            return Ok(member);

        }
    }
}
