using System;
using System.Collections.Generic;

namespace Generator
{
    public class Member
    {
        public int ID;
        public List<Member> schedule;
        private List<int> allreadyMet;
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
            allreadyMet = new List<int>();
            addAllreadyMet(ID);
            schedule = new List<Member>();

            this.starter = null;
            this.dinner = null;
            this.dessert = null;
        }

        public void addAllreadyMet(int ID)
        {
            allreadyMet.Add(ID);

        }
        public List<int> getAllreadyMet()
        {
            return allreadyMet;
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
