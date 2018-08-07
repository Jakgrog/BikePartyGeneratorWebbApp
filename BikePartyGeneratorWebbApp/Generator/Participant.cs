using System;
using System.Collections.Generic;

namespace Generator
{
    public class Member
    {
        public int ID;
        public List<Member> schedule;
        private List<string> names;
        private string address;
        internal string duty;
        internal Member starter;
        internal Member dinner;
        internal Member dessert;

        public Member(int ID, List<string> names, string address)
        {
            this.ID = ID;
            this.names = names;
            this.address = address;
            schedule = new List<Member>();
            party = new List<int>();
            party.Add(ID);
            allReadyMet = new List<int>();

            this.starter = null;
            this.dinner = null;
            this.dessert = null;
        }

        public List<int> party { get; set; }
        public List<int> allReadyMet { get; set; }

        public bool PartyNotFull()
        {
            return party.Count < 3;
        }

        public string printNames()
        {
            string str = "";
            foreach (string name in names)
                str = str + name;

            return str;
        }

        public List<string> getNames()
        {
            return names;
        }
    }
}
