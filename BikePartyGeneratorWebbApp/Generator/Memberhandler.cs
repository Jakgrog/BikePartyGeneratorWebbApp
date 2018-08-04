using System;
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
        public void createDates(List<Member> members, int count)
        {
            tempMembers = new List<Member>(members);
            tempMembers.Shuffle();
            double numberOfMembers = tempMembers.Count;
            int numnerOfMembersPerDate = (int)Math.Floor(numberOfMembers / 3);

            List<Member> starter = new List<Member>(tempMembers.GetRange(0, numnerOfMembersPerDate));
            List<Member> dinner = new List<Member>(tempMembers.GetRange(numnerOfMembersPerDate, numnerOfMembersPerDate));
            List<Member> dessert = new List<Member>(tempMembers.GetRange((2 * numnerOfMembersPerDate), (tempMembers.Count - 2 * numnerOfMembersPerDate)));

            int v = starter.Count - 1;
            int i = 0;
            int t = 0;
            int s = v;
            foreach (Member m in starter)
            {
                m.duty = "starter";
                if(i == 0)
                {
                    m.dessert = dessert[v/2];
                    m.dinner = dinner[i];
                }
                else if (i > 0 && i <= v/2)
                {
                    m.dessert = dessert[t];
                    m.dinner = dinner[i];
                    t++;
                }
                else if (i > v / 2)
                {
                    m.dinner = dinner[s];
                    m.dessert = dessert[s];
                }
                i++;
                s--;
            }

            v = dinner.Count - 1;
            i = 0;
            t = v/2;
            s = v;
            foreach (Member m in dinner)
            {
                m.duty = "dinner";
                if (i < v / 2)
                {
                    m.starter = starter[t];
                    m.dessert = dessert[s];
                }
                else if (i == v / 2)
                {
                    m.starter = starter[t];
                    s = v;
                    m.dessert = dessert[s];
                }
                else if (i > v/2)
                {
                    t = v / 2;
                    m.starter = starter[t];
                    m.dessert = dessert[s];
                }
                t++;
                i++;
                s--;
            }

            v = numnerOfMembersPerDate - 1;
            i = 0;
            t = 0;
            s = v;
            foreach (Member m in dessert)
            {
                m.duty = "dessert";
                if(i > v)
                {
                    m.dinner = dinner[0];
                    m.starter = starter[0];
                }
                else if (i < v / 2)
                {
                    m.dinner = dinner[v - t];
                    m.starter = starter[t];
                }
                else if(i == v / 2)
                {
                    m.dinner = dinner[v/2];
                    m.starter = starter[v];
                }
                else if(i > v / 2)
                {
                    m.dinner = dinner[t];
                    m.starter = starter[s];
                }
                i++;
                t++;
                s--;
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