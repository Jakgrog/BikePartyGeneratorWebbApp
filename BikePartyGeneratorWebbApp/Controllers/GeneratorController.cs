using Generator;
using BikePartyGeneratorWebbApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;

namespace BikePartyGeneratorWebbApp.Controllers
{
    public class GeneratorController : ApiController
    {
        public IHttpActionResult GetScheme()
        {
            List<JsonMember> members = (new MembersController()).GetAllMembers();
            GeneratorClass generator = new GeneratorClass(members);
            JsonScheme scheme = generator.generate();

            if (scheme == null)
            {
                return NotFound();
            }
            return Ok(scheme);
        }
    }
}
