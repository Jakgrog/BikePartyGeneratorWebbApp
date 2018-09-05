using System;
using System.Collections.Generic;
using System.Linq;

namespace Generator
{
    public class Member : ICloneable
    {
        public int ID;
        public List<Member> schedule;
        private List<string> names;
        internal string duty;
        internal Member starter;
        internal Member dinner;
        internal Member dessert;

        public Member(int ID, List<string> names, string address, string phone, int association)
        {
            this.ID = ID;
            this.names = names;
            this.address = address;
            this.phone = phone;
            this.association = association;
            schedule = new List<Member>();
            party = new List<int>();
            party.Add(ID);
            allReadyMet = new List<int>();

            this.starter = null;
            this.dinner = null;
            this.dessert = null;
        }

        public object Clone()
        {
            Member newMember = (Member)this.MemberwiseClone();
            newMember.starter = this.starter != null ? (Member)this.starter : null;
            newMember.dinner = this.dinner != null ? (Member)this.dinner : null;
            newMember.dessert = this.dessert != null ? (Member)this.dessert : null;
            newMember.party = new List<int>(this.party);
            newMember.allReadyMet = new List<int>(this.allReadyMet);

            return newMember;
        }
        public string address { get; set; }
        public string phone { get; set; }
        public int association { get; set; }
        public List<int> party { get; set; }
        public List<int> allReadyMet { get; set; }

        public bool isFull()
        {
            return party.Count > 3;
        }

        public bool allDatesFound()
        {
            switch (this.duty)
            {
                case "dinner":
                    return this.dessert != null && this.starter != null;
                case "dessert":
                    return this.dinner != null && this.starter != null;
                case "starter":
                    return this.dessert != null && this.dinner != null;
                default:
                    return false;
            }
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
