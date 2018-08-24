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
        public void createDates(List<Member> members)
        {
            tempMembers = new List<Member>(members);
            List<Member> membersAss1 = tempMembers.FindAll(p => p.association == 0);
            List<Member> membersAss2 = tempMembers.FindAll(p => p.association == 1);

            membersAss1.Shuffle();
            membersAss2.Shuffle();

            double numberOfMembersAss1 = membersAss1.Count;
            double numberOfMembersAss2 = membersAss2.Count;

            int numnerOfMembersPerDateAss1 = (int)Math.Round(numberOfMembersAss1 / 3);
            int numnerOfMembersPerDateAss2 = (int)Math.Round(numberOfMembersAss2 / 3);

            List<Member> starterAss1 = new List<Member>(membersAss1.GetRange(0, numnerOfMembersPerDateAss1));
            List<Member> dinnerAss1 = new List<Member>(membersAss1.GetRange(numnerOfMembersPerDateAss1, numnerOfMembersPerDateAss1));
            List<Member> dessertAss1 = new List<Member>(membersAss1.GetRange((2 * numnerOfMembersPerDateAss1), ((int)numberOfMembersAss1 - 2 * numnerOfMembersPerDateAss1)));

            List<Member> starterAss2 = new List<Member>(membersAss2.GetRange(0, numnerOfMembersPerDateAss2));
            List<Member> dinnerAss2 = new List<Member>(membersAss2.GetRange(numnerOfMembersPerDateAss2, numnerOfMembersPerDateAss2));
            List<Member> dessertAss2 = new List<Member>(membersAss2.GetRange((2 * numnerOfMembersPerDateAss2), ((int)numberOfMembersAss1 - 2 * numnerOfMembersPerDateAss2)));

            noDinnerDateFound = new List<Member>();
            noDessertDateFound = new List<Member>();
            noStarterDateFound = new List<Member>();
            
            // Try to find a dates where Ass1 members visits Ass2 members and vice versa 
            foreach(Member m in starterAss1)
            {
                m.duty = "Starter";
                m.dinner = FindDate(m, dinnerAss2, "dinner");
                m.dessert = FindDate(m, dessertAss2, "dessert");
            }
            foreach (Member m in dinnerAss1)
            {
                m.duty = "Dinner";
                m.starter = FindDate(m, starterAss2, "starter");
                m.dessert = FindDate(m, dessertAss2, "dessert");
            }
            foreach (Member m in dessertAss1)
            {
                m.duty = "Dessert";
                m.dinner = FindDate(m, dinnerAss2, "dinner");
                m.starter = FindDate(m, starterAss2, "starter");
            }

            foreach (Member m in starterAss2)
            {
                m.duty = "Starter";
                m.dinner = FindDate(m, dinnerAss1, "dinner");
                m.dessert = FindDate(m, dessertAss1, "dessert");
            }
            foreach (Member m in dinnerAss2)
            {
                m.duty = "Dinner";
                m.starter = FindDate(m, starterAss1, "starter");
                m.dessert = FindDate(m, dessertAss1, "dessert");
            }
            foreach (Member m in dessertAss2)
            {
                m.duty = "Dessert";
                m.dinner = FindDate(m, dinnerAss1, "dinner");
                m.starter = FindDate(m, starterAss1, "starter");
            }

            handleNoDatesFound(starterAss1.Concat(starterAss2).ToList(), dinnerAss1.Concat(dinnerAss2).ToList(), dessertAss1.Concat(dessertAss2).ToList());
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