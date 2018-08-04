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
        private static int j;

        public GeneratorClass(List<JsonMember> memberList)
        {
            List<Member> members = new List<Member>();
            foreach (var member in memberList)
            {
                List<string> names = new List<string>(member.Name.Split(','));
                members.Add(new Member(member.Id, names, "Ryd"));
            }
            memberhandler = new Memberhandler(members);
        }


        public JsonScheme generate()
        {
            memberhandler.printAllMembers();
            scheme = new Scheme();
            JsonScheme resultingScheme = scheme.createScheme(memberhandler);
            return resultingScheme;
        }
    }
}
