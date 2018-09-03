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
            public int y;
            public int z;
            public int Id;
            internal int associationCost = 0;

            public Node(int x, int y, int z, int id, List<Member> members, List<Tuple<int, int>> dates, int cost)
            {
                this.x = x;
                this.y = y;
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

            int highestZ = 0;

            int z = 0;
            int id = 0;
            int releaseCounter = 0;
            int release = 0;
            int goal = initMembers.Count * 2;
            Node backupNode = new Node(0, 0, 0, z, initMembers, new List<Tuple<int, int>>(), 500);
            Node currentNode = new Node(0, 0, 0, z, initMembers, new List<Tuple<int, int>>(), 500);
            OPEN.Add(currentNode);

            while (true)
            {
                if (OPEN.Count > 0)
                {
                    currentNode = getNodeWithLowestCost(OPEN);
                    OPEN.Remove(currentNode);
                }
                else
                {
                    currentNode = backupNode;
                    Debug.WriteLine("Out of nodes: " + ", Z: " + currentNode.z +" Error " + currentNode.allreadyMet + " Cost " + currentNode.cost);
                }
                    

                List<Member> members = currentNode.members;
                //Debug.WriteLine("Z: " + currentNode.z + ", Branch " + currentNode.Id + " Error " + currentNode.error + " Cost " + currentNode.cost);
                //Debug.WriteLine("Node: " + currentNode.x + " - " + currentNode.y);

                if (currentNode.z == goal)
                {
                    Debug.WriteLine("Z: " + currentNode.z);
                    Debug.WriteLine("Allready met: " + currentNode.allreadyMet);
                    Debug.WriteLine("Association error: " + currentNode.associationCost);
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
                        bool allreadyRegistered = currentNode.datesFound.FirstOrDefault(n => n.Item1 == i && n.Item2 == j) != null;
                        if (allreadyRegistered)
                        {
                            continue;
                        }

                        List<Member> membersClone = members.Clone();
                        int groupsize = 2;
                        groupsize = (goal - newZ) < (numerOfmembers % (groupsize + 1)) * 2 ? groupsize + 1 : groupsize;

                        bool possibleMatch = findMatch(membersClone[i], membersClone[j], groupsize);
                        if (!possibleMatch)
                        {
                            continue;
                        }

                        if (releaseCounter == 5000)
                        {
                            release++;
                            releaseCounter = 0;
                        }

                        if (newZ > highestZ)
                            releaseCounter = 0;
                        else
                            releaseCounter++;

                        Tuple<int, int> date = new Tuple<int, int>(i, j);
                        int[] errorAndCost = calculateCost(membersClone[i], membersClone[j], goal, (goal - newZ), groupsize, release);
                        int cost = errorAndCost[0];
                        int allreadyMet = errorAndCost[1];
                        List<Tuple<int, int>> dates = new List<Tuple<int, int>>(currentNode.datesFound);
                        dates.Add(date);
                        Node currentNeighbour = new Node(i, j, newZ, id, membersClone, dates, cost);
                        currentNeighbour.allreadyMet = currentNode.allreadyMet + errorAndCost[1];
                        currentNeighbour.associationCost = currentNode.associationCost + errorAndCost[2];
                        registerDate(membersClone[i], membersClone[j], membersClone);

                        if (cost < currentNode.cost)
                        {
                            //Debug.WriteLine("Z: " + newZ + ", Cost: " + cost + ", error: " + error + ", currentCost: " + currentNode.cost + ", groupsize: " + groupsize);
                            OPEN.Add(currentNeighbour);
                            id++;
                        }
                        else if(newZ > highestZ)
                        {
                            backupNode = currentNeighbour;
                            highestZ = newZ;
                            //Debug.WriteLine("Added to closed: " + ", Z: " + newZ + ", cost: " + cost + ", old cost: " + currentNode.cost);
                        }
                        else if(newZ == highestZ && cost < backupNode.cost)
                        {
                            backupNode = currentNeighbour;
                        }
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

        private int[] calculateCost(Member m, Member date, int goal, int distanceToGoal, int groupsize, int release)
        {
            int associationCost = m.association != date.association || release > 1 ? 0 : 1;
            int associationError = m.association != date.association ? 0 : 1;

            int emptyParty = date.party.Count < 2 && release < date.party.Count ? (2 - date.party.Count - release) * goal * 2 : 0;
            int allReadyMet = AllReadyMet(m, date.party);
            int allReadyMetCost = allReadyMet > release ? (allReadyMet - release)* goal * 2 : 0;

            int returnCost = allReadyMetCost + emptyParty + distanceToGoal + associationCost;
            int[] returnArray = new int[] { returnCost, allReadyMet, associationError };
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