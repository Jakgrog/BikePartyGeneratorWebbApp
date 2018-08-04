using System;
using System.Collections.Generic;

class Generator
{
  private static Memberhandler memberhandler;
  private static Scheme scheme;
  private static int j;

  static void Main()
  {
    memberhandler = new Memberhandler();
    readInput();
    j = 0;
  }

  private static void readInput()
  {
    string input = Console.ReadLine();
    string[] inputArray = input.Split(null);
    string register = inputArray[0].ToLower();
    List<string> names = new List<string>();

    if(register != "quit")
    {
      foreach(string i in inputArray)
        names.Add(i);

      memberhandler.addMember(new Member(j,names,"Rydsvägen 72B"));
      Console.WriteLine(j.ToString());
      j = j + 1;
      readInput();
    }
    else
    {
      //memberhandler.printAllMembers();
      scheme = new Scheme(memberhandler);
    }
  }
}
