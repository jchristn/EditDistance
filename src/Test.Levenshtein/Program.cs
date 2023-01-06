using System;
using EditDistance;
using GetSomeInput;

namespace Test.Levenshtein
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("Press ENTER to exit");
                string s1 = Inputty.GetString("String 1:", null, true);
                if (String.IsNullOrEmpty(s1)) break;
                string s2 = Inputty.GetString("String 2:", null, true);
                if (String.IsNullOrEmpty(s2)) break;

                Console.WriteLine("");
                Console.WriteLine("Before:");
                WagnerFischer wf = new WagnerFischer(s1, s2);
                Console.WriteLine(wf.DisplayMatrix());

                wf.PopulateMatrix();
                Console.WriteLine("After:");
                Console.WriteLine(wf.DisplayMatrix());

                Console.WriteLine("");
                Console.WriteLine("Edit Distance: " + wf.EditDistance);
            }
        }
    }
}