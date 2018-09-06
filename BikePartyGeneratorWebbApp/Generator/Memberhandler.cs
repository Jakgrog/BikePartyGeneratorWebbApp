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

        private class Node
        {
            public int allreadyMet = 0;
            public int cost;
            public List<Member> members;
            public List<Tuple<int, int>> datesFound;
            public int x;
            public int z;
            public int Id;
            internal int associationCost = 0;

            public Node(int x, int z, int id, List<Member> members, List<Tuple<int, int>> dates, int cost)
            {
                this.x = x;
                this.z = z;
                this.members = members;
                this.cost = cost;
                this.Id = id;
                this.datesFound = dates;
            }
        }

        /// <summary>
        /// Tries to create a scheme where no group meets the same group twice.
        /// This is done by shuffling the members list and matching the groups.
        /// The function also tries to mix the associations as much as possible
        /// so that members in association 1 goes to members in ass2 and vice versa.
        /// </summary>
        /// <param name="members"></param>
        /// 

        public List<Member> createDates(List<Member> members)
        {
            tempMembers = new List<Member>(members);
            double numberOfMembers = tempMembers.Count;
            List<Member> ass1List = tempMembers.FindAll(p => p.association == 0);
            List<Member> ass2List = tempMembers.FindAll(p => p.association == 1);
            double numberOfMembersAss1 = ass1List.Count;
            double numberOfMembersAss2 = ass2List.Count;

            //ass1List.Shuffle();
            //ass2List.Shuffle();

            int numnerOfMembersPerDate = (int)Math.Round(numberOfMembers / 3);
            int numnerOfMembersPerDateAss1 = (int)Math.Round(numberOfMembersAss1 / 3);
            int numnerOfMembersPerDateAss2 = (int)Math.Round(numberOfMembersAss2 / 3);

            List<Member> starterAss1 = ass1List.GetRange(0, numnerOfMembersPerDateAss1);
            List<Member> dinnerAss1 = ass1List.GetRange(numnerOfMembersPerDateAss1, numnerOfMembersPerDateAss1);
            List<Member> dessertAss1 = ass1List.GetRange((2 * numnerOfMembersPerDateAss1), ((int)numberOfMembersAss1 - 2 * numnerOfMembersPerDateAss1));

            List<Member> dessertAss2 = ass2List.GetRange(0, numnerOfMembersPerDateAss2);
            List<Member> dinnerAss2 = ass2List.GetRange(numnerOfMembersPerDateAss2, numnerOfMembersPerDateAss2);
            List<Member> starterAss2 = ass2List.GetRange((2 * numnerOfMembersPerDateAss2), ((int)numberOfMembersAss2 - 2 * numnerOfMembersPerDateAss2));

            starterAss1.ForEach(p => p.duty = "starter");
            dinnerAss1.ForEach(p => p.duty = "dinner");
            dessertAss1.ForEach(p => p.duty = "dessert");

            starterAss2.ForEach(p => p.duty = "starter");
            dinnerAss2.ForEach(p => p.duty = "dinner");
            dessertAss2.ForEach(p => p.duty = "dessert");

            List<Member> SortedMembers = tempMembers.OrderBy(p => p.association).ToList();

            return Djikstra(SortedMembers, starterAss1.Concat(starterAss2).Count(), dinnerAss1.Concat(dinnerAss2).Count(), dessertAss1.Concat(dessertAss2).Count(), numnerOfMembersPerDate);
        }

        private List<Member> Djikstra(List<Member> initMembers, int starterNum, int dinnerNum, int dessertNum, int numnerOfMembersPerDateAss1)
        {
            List<Node> OPEN = new List<Node>();
            List<Node> CLOSE = new List<Node>();

            int groupsizeStarter = starterNum % numnerOfMembersPerDateAss1 == 0 ? 2 : (int)Math.Floor((double)((dinnerNum + dessertNum) / starterNum));
            int groupsizeDinner = dinnerNum % numnerOfMembersPerDateAss1 == 0 ? 2 : (int)Math.Floor((double)((starterNum + dessertNum) / dinnerNum));
            int groupsizeDessert = dessertNum % numnerOfMembersPerDateAss1 == 0 ? 2 : (int)Math.Floor((double)((dinnerNum + starterNum) / dessertNum));

            int highestZ = 0;

            int z = 0;
            int id = 0;
            int goal = initMembers.Count * 2;
            int backupcost = 500;
            Node backupNode = new Node(0, 0, z, initMembers, new List<Tuple<int, int>>(), 500);
            Node currentNode = new Node(0, 0, z, initMembers, new List<Tuple<int, int>>(), 500);
            OPEN.Add(currentNode);

            while (true)
            {
                if (OPEN.Count > 0 && currentNode.datesFound.Count != goal)
                {
                    currentNode = getNodeWithLowestCost(OPEN);
                    OPEN.Remove(currentNode);
                }
                else if(currentNode.datesFound.Count != goal)
                {
                    currentNode = getNodeWithLowestCost(CLOSE);
                    Debug.WriteLine("Ops");
                    currentNode.cost = 10000;
                }

                if(OPEN.Count > 8000)
                {
                    OPEN = new List<Node>();
                }
                List<Member> members = currentNode.members;

                if (currentNode.datesFound.Count == goal)
                {
                    Debug.WriteLine("Z: " + currentNode.z);
                    Debug.WriteLine("Allready met: " + currentNode.allreadyMet);
                    Debug.WriteLine("Association error: " + currentNode.associationCost);
                    Debug.WriteLine("Cost: " + currentNode.cost);
                    currentNode.members.ForEach(p => p.allReadyMet.ForEach(l => Debug.WriteLine(p.ID + ": " + l)));
                    return currentNode.members;
                }

                OPEN.Remove(currentNode);

                int numerOfmembers = members.Count;
                for (int i = 0; i < numerOfmembers; i++)
                {
                    List<Member> membersClone = members.Clone();
                    if (membersClone[i].allDatesFound())
                        continue;

                    List<Tuple<int, int>> dates = new List<Tuple<int, int>>(currentNode.datesFound);
                    int cost = 0;
                    int allreadyMet = 0;
                    int associationcost = 0;
                    for (int j = 0; j < numerOfmembers; j++)
                    {
                        bool allreadyRegistered = currentNode.datesFound.FirstOrDefault(n => n.Item1 == i && n.Item2 == j) != null;
                        if (allreadyRegistered || membersClone[i].allDatesFound())
                        {
                            continue;
                        }

                        int groupsize = 2;
                        if (membersClone[j].duty == "starter")
                            groupsize = groupsizeStarter;
                        else if (membersClone[j].duty == "dinner")
                            groupsize = groupsizeDinner;
                        else if(membersClone[j].duty == "dessert")
                            groupsize = groupsizeDessert;

                        int release = 0;
                        int whenToIncreaseGroupsize = 2 * numerOfmembers - groupsizeDinner * dinnerNum - groupsizeDessert * dessertNum - groupsizeStarter * starterNum;
                        if(whenToIncreaseGroupsize == 0)
                            groupsize = (goal - dates.Count) <= (numerOfmembers % (groupsize + 1)) * 2 ? groupsize + 1 : groupsize;
                        else
                            groupsize = (goal - dates.Count) <= whenToIncreaseGroupsize+release ? groupsize + 1 : groupsize;

                        bool possibleMatch = findMatch(membersClone[i], membersClone[j], groupsize);
                        if (!possibleMatch)
                        {
                            continue;
                        }

                        Tuple<int, int> date = new Tuple<int, int>(i, j);
                        dates.Add(date);

                        int[] errorAndCost = calculateCost(membersClone[i], membersClone[j], goal, (goal - dates.Count), associationcost);
                        cost = cost + errorAndCost[0];
                        allreadyMet = allreadyMet + errorAndCost[3];
                        associationcost = associationcost + errorAndCost[2];
                        registerDate(membersClone[i], membersClone[j], membersClone);
                    }

                    int datecount = dates.Count;
                    Node currentNeighbour = new Node(i, datecount, id, membersClone, dates, cost+associationcost);
                    currentNeighbour.allreadyMet = currentNode.allreadyMet + allreadyMet;
                    currentNeighbour.associationCost = currentNode.associationCost + associationcost;

                    if (cost < currentNode.cost)
                    {
                        OPEN.Insert(0, currentNeighbour);
                        //Debug.WriteLine("Cost: " + cost + ", Z: " + datecount);
                        id++;
                    }
                    else if (datecount >= highestZ && cost < backupcost)
                    {
                        if(datecount > highestZ)
                        {
                            //CLOSE = new List<Node>();
                            highestZ = datecount;
                        }
                        CLOSE.Add(currentNeighbour);
                        backupcost = cost;
                    }
                }
            }
        }

        private Node getNodeWithLowestCost(List<Node> nodes)
        {
            Node minCostNode = nodes[0];
            foreach (var node in nodes)
            {
                if (node.cost < minCostNode.cost)
                    minCostNode = node;
            }
            return minCostNode;
        }

        private int[] calculateCost(Member m, Member date, int goal, int distanceToGoal, int associsationcost)
        {
            int associationCost = m.association == date.association ? associsationcost + 1 : 0;
            int associationError = m.association == date.association ? associsationcost + 1 : 0;

            int allReadyMet = AllReadyMet(m, date.party);
            int allReadyMetCost = allReadyMet * goal * 4 ;

            int returnCost = allReadyMetCost + distanceToGoal*2;
            int[] returnArray = new int[] { returnCost, allReadyMetCost, ((int)Math.Pow((double)associationCost, (double)8)), allReadyMet};

            return returnArray;
        }

        private void registerDate(Member visitor, Member host, List<Member> members)
        {
            foreach(int membID in host.party)
            {
                Member partymemb = members.FirstOrDefault(m => m.ID == membID);
                partymemb.allReadyMet.Add(visitor.ID);
                visitor.allReadyMet.Add(membID);
            }
            host.party.Add(visitor.ID);
        }

        private int AllReadyMet(Member memb, List<int> party)
        {
            int numberOfMembersAllreadyMet = 0;
            foreach (int membID in party)
            {
                numberOfMembersAllreadyMet = memb.allReadyMet.FirstOrDefault(x => x == membID) != 0 ? numberOfMembersAllreadyMet + 2 : numberOfMembersAllreadyMet;
            }
            return numberOfMembersAllreadyMet;
        }

        private bool findMatch(Member visitor, Member host, int groupsize)
        {
            if (host.party.Count > groupsize)
            {
                return false;
            }
            switch (visitor.duty)
            {
                case "dinner":
                    if(visitor.starter == null && host.duty == "starter")
                    {
                        visitor.starter = host;
                        return true;
                    } else if(visitor.dessert == null && host.duty == "dessert")
                    {
                        visitor.dessert = host;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "dessert":
                    if (visitor.starter == null && host.duty == "starter")
                    {
                        visitor.starter = host;
                        return true;
                    }
                    else if (visitor.dinner == null && host.duty == "dinner")
                    {
                        visitor.dinner = host;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "starter":
                    if (visitor.dinner == null && host.duty == "dinner")
                    {
                        visitor.dinner = host;
                        return true;
                    }
                    else if (visitor.dessert == null && host.duty == "dessert")
                    {
                        visitor.dessert = host;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }
    }

    static class Extensions
    {
        public static List<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
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