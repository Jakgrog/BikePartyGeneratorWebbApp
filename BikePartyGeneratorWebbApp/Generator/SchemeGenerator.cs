using System;
using BikePartyGeneratorWebbApp.Models;
using System.Collections.Generic;

namespace Generator
{
    public class Scheme
    {
        public JsonScheme createScheme(Memberhandler memberhandler)
        {
            List<Member> members = memberhandler.getMembers();
            JsonScheme scheme = new JsonScheme();

            if (members.Count < 3)
            {
                scheme.message = "Too few participants :(, get more friends!";
                return scheme;
            }
            members = memberhandler.createDates(members);
            foreach (Member m in members)
            {
                List<string> starter = m.starter != null ? m.starter.getNames() : null;
                List<string> dinner = m.dinner != null ? m.dinner.getNames() : null;
                List<string> dessert = m.dessert != null ? m.dessert.getNames() : null;

                JsonMember member = new JsonMember();
                member.names = m.getNames();
                member.duty = m.duty;
                member.starter = starter;
                member.dessert = dessert;
                member.dinner = dinner;

                scheme.dateList.Add(member);
            }
            return scheme;
        }
    }
}
