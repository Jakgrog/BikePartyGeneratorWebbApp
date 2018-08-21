using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Generator
{
    public class Memberhandler
    {
        //Private functions
        private List<Member> members;
        private static List<Member> tempMembers;

        //Public Functions

        public Memberhandler(List<Member> members)
        {
            this.members = members;
        }

        public List<Member> getMembers()
        {
            return members;
        }

        public void addMember(Member m)
        {
            members.Add(m);
        }

        public void printAllMembers()
        {
            //Console.WriteLine(members.Count.ToString());
            foreach (Member member in members)
                Debug.WriteLine(member.printNames());
        }

        /// <summary>
        /// Tries to create a scheme where no group meets the same group twice.
        /// This is done by shuffling the members list and matching the groups
        /// according to a very specific pattern (hence all the if-statements).
        /// </summary>
        /// <param name="members"></param>
        /// <param name="count"></param>

        private List<Member> noStarterDateFound;
        private List<Member> noDessertDateFound;
        private List<Member> noDinnerDateFound;
        public void createDates(List<Member> members, int count)
        {
            tempMembers = new List<Member>(members);
            tempMembers.Shuffle();
            double numberOfMembers = tempMembers.Count;
            int numnerOfMembersPerDate = (int)Math.Round(numberOfMembers / 3);

            List<Member> starter = new List<Member>(tempMembers.GetRange(0, numnerOfMembersPerDate));
            List<Member> dinner = new List<Member>(tempMembers.GetRange(numnerOfMembersPerDate, numnerOfMembersPerDate));
            List<Member> dessert = new List<Member>(tempMembers.GetRange((2 * numnerOfMembersPerDate), (tempMembers.Count - 2 * numnerOfMembersPerDate)));
            noDinnerDateFound = new List<Member>();
            noDessertDateFound = new List<Member>();
            noStarterDateFound = new List<Member>();

            foreach(Member m in starter)
            {
                m.duty = "Starter";
                m.dinner = FindDate(m, dinner, "dinner");
                m.dessert = FindDate(m, dessert, "dessert");
            }
            foreach (Member m in dinner)
            {
                m.duty = "Dinner";
                m.starter = FindDate(m, starter, "starter");
                m.dessert = FindDate(m, dessert, "dessert");
            }
            foreach (Member m in dessert)
            {
                m.duty = "Dessert";
                m.dinner = FindDate(m, dinner, "dinner");
                m.starter = FindDate(m, starter, "starter");
            }

            handleNoDatesFound(starter, dinner, dessert);
        }

        private Member FindDate(Member m, List<Member> list, string dateType)
        {
            Member date = list.Where(x => !AllReadyMet(m, x.party) && x.PartyNotFull()).FirstOrDefault();
            if (date != null)
            {
                switch (dateType)
                {
                    case "starter":
                        m.starter = date;
                        registerDate(m, date);
                        break;
                    case "dinner":
                        m.dinner = date;
                        registerDate(m, date);
                        break;
                    case "dessert":
                        m.dessert = date;
                        registerDate(m, date);
                        break;
                }
            }
            else
            {
                switch (dateType)
                {
                    case "starter":
                        this.noStarterDateFound.Add(m);
                        break;
                    case "dinner":
                        this.noDinnerDateFound.Add(m);
                        break;
                    case "dessert":
                        this.noDessertDateFound.Add(m);
                        break;
                }
            }
            return date;
        }

        private bool AllReadyMet(Member memb, List<int> party)
        {
            foreach (int membID in party)
            {
                if (memb.allReadyMet.FirstOrDefault(x => x == membID) != 0)
                {
                    return true;
                }
            }
            return false;
        }
        private void registerDate(Member m, Member date)
        {
            m.allReadyMet.Add(date.ID);
            date.allReadyMet.Add(m.ID);
            date.party.Add(m.ID);
        }

        private void handleNoDatesFound(List<Member> starter, List<Member> dinner, List<Member> dessert)
        {
            List<Member> tempList;
            int index;

            if (this.noStarterDateFound != null)
            {
                tempList = new List<Member>(starter);
                tempList.Shuffle();
                index = 0;
                foreach (Member m in this.noStarterDateFound)
                {
                    m.starter = starter[index];
                    index = index < starter.Count-1 ? index + 1 : 0;
                }
            }

            if (this.noDinnerDateFound != null)
            {
                tempList = new List<Member>(dinner);
                tempList.Shuffle();
                index = 0;
                foreach (Member m in this.noDinnerDateFound)
                {
                    m.dinner = dinner[index];
                    index = index < dinner.Count-1 ? index + 1 : 0;
                }
            }

            if (this.noDessertDateFound != null)
            {
                tempList = new List<Member>(dessert);
                tempList.Shuffle();
                index = 0;
                foreach (Member m in this.noDessertDateFound)
                {
                    m.dessert = dessert[index];
                    index = index < dessert.Count-1 ? index + 1 : 0;
                }
            }
        }
        
    }

    static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
} 