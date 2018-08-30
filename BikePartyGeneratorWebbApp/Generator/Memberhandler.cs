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
            public int error;
            public int cost;
            public List<Member> members;
            public int x;
            public int y;
            public int z;
            public int Id;
            public Node(int x, int y, int z, int id, List<Member> members, int cost, int error)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.members = members;
                this.cost = cost;
                this.error = error;
                this.Id = id;
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
            tempMembers.Shuffle();

            int numnerOfMembersPerDateAss1 = (int)Math.Round(numberOfMembers / 3);

            List<Member> starter = tempMembers.GetRange(0, numnerOfMembersPerDateAss1);
            List<Member> dinner = tempMembers.GetRange(numnerOfMembersPerDateAss1, numnerOfMembersPerDateAss1);
            List<Member> dessert = tempMembers.GetRange((2 * numnerOfMembersPerDateAss1), ((int)numberOfMembers - 2 * numnerOfMembersPerDateAss1));

            starter.ForEach(p => p.duty = "starter");
            dinner.ForEach(p => p.duty = "dinner");
            dessert.ForEach(p => p.duty = "dessert");

            return Djikstra(tempMembers);
        }

        private List<Member> Djikstra(List<Member> initMembers)
        {
            List<Node> OPEN = new List<Node>();
            List<Node> CLOSE = new List<Node>();

            int z = 0;
            int id = 0;
            Node currentNode = new Node(0, 0, 0, z, initMembers, 10, 0);
            OPEN.Add(currentNode);
            int goal = initMembers.Count * 2;

            while (true)
            {
                if (OPEN.Count > 0)
                    currentNode = getNodeWithLowestCost(OPEN);
                else
                {
                    Debug.WriteLine("Out of nodes");
                    return currentNode.members;
                }
                    
                OPEN.Remove(currentNode);
                CLOSE.Add(currentNode);
                List<Member> members = currentNode.members;
                //Debug.WriteLine("Z: " + currentNode.z + ", Branch " + currentNode.Id + " Error " + currentNode.error + " Cost " + currentNode.cost);
                //Debug.WriteLine("Node: " + currentNode.x + " - " + currentNode.y);

                if (currentNode.z == goal)
                {
                    Debug.WriteLine("Z: " + currentNode.z);
                    Debug.WriteLine("Error: " + currentNode.error);
                    Debug.WriteLine("Cost: " + currentNode.cost);
                    currentNode.members.ForEach(p => p.allReadyMet.ForEach(l => Debug.WriteLine(p.ID + ": " + l)));
                    return currentNode.members;
                }

               OPEN.Remove(currentNode);
                //CLOSE.Add(currentNode);

                int numerOfmembers = members.Count;
                int newZ = currentNode.z + 1;

                for (int i = 0; i < numerOfmembers; i++)
                {
                    for (int j = 0; j < numerOfmembers; j++)
                    {
                        List<Member> membersClone = members.Clone();
                        int groupsize = 2;
                        if (goal - newZ <= (numerOfmembers % (groupsize+1))*2)
                        {
                            groupsize++;
                        }
                        bool possibleMatch = findMatch(membersClone[i], membersClone[j], groupsize);

                        if (!possibleMatch)
                        {
                            continue;
                        }

                        int[] errorAndCost = calculateCost(membersClone[i], membersClone[j], currentNode.error, currentNode.cost, (goal - newZ), groupsize);
                        int cost = errorAndCost[1];
                        int error = errorAndCost[0];
                        registerDate(membersClone[i], membersClone[j], membersClone);

                        if (newZ == goal){ Debug.WriteLine("Cost: " + cost + ", error: " + error + ", Groupsize: " + groupsize); }

                        Node currentNeighbour = new Node(i, j, newZ, id, membersClone, cost, error);
                        OPEN.Add(currentNeighbour);
                        id++;
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

        private int[] calculateCost(Member m, Member date, int initError, int initCost, int distanceToGoal, int groupsize)
        {
            int fullParty = 0;
            int associationCost = m.association != date.association ? 0 : 1;

            if (date.party.Count > groupsize)
            {
                fullParty = date.party.Count - groupsize;
            }
            int returnCost = AllReadyMet(m, date.party) * 40 + fullParty * 40 + distanceToGoal;// + associationCost;
            int returnError = AllReadyMet(m, date.party) + fullParty + initError; //+ associationCost;
            int[] returnArray = new int[] { returnError, returnCost };
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