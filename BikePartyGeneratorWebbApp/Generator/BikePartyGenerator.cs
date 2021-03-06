using System;
using System.Net.Http;
using System.Collections.Generic;
using BikePartyGeneratorWebbApp.Models;

namespace Generator
{
    public class GeneratorClass
    {
        private static Memberhandler memberhandler;
        private static Scheme scheme;

        public GeneratorClass(List<JsonMember> memberList)
        {
            List<Member> members = new List<Member>();
            foreach (var member in memberList)
            {
                members.Add(new Member(member.Id, member.names, member.address, member.phone, member.association));
            }
            memberhandler = new Memberhandler(members);
        }

        public JsonScheme generate()
        {
            scheme = new Scheme();
            JsonScheme resultingScheme = scheme.createScheme(memberhandler);
            return resultingScheme;
        }
    }
}
