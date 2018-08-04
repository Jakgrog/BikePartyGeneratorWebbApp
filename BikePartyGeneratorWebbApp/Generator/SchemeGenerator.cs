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
            memberhandler.createDates(members, 0);
            JsonScheme scheme = new JsonScheme();

            foreach (Member m in members)
            { 
                string starter = m.starter != null ? "Starter: " + m.starter.printNames() + ", " : null;
                string dinner = m.dinner != null ? "Dinner: " + m.dinner.printNames() + ", " : null;
                string dessert = m.dessert != null ? "Dessert: " + m.dessert.printNames() : null;

                scheme.dateList.Add(m.printNames() + " " + m.duty + ": " + starter + dinner + dessert);
            }
            return scheme;
        }
    }
}
