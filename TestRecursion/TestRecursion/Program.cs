/* Developer: Isaias Villalobos
 * Date: 9/17/2019
 * Version: 1.0
 * This is a class that handles processing of command line arguments to this program.
 * Will utilize another class to handle processing directory information.
*/
using System;

namespace TestRecursion
{

    public class Program
    {
        /* Input: String arugments passed into the program from STDIN
         * This program is the driver, will make sure that enough arguments are passed inton the program.
         * Will call other functions to properly handle the sequential and parallel processing of a directory. 
         */
        static void Main(string[] args)
        {

            //Create instance of a ProgramOptions class to utilize function stored in this class.
            var options = new ProgramOptions();

            //Make sure to have enough arugments to a program
            if (args.Length < 2)
            {
                Console.Error.WriteLine("\nUsage: du [-s] [-p] [-b] <path>");
                Console.Error.WriteLine("Summarize disk usage of the set of FILES, recursively, for directories.");
                Console.Error.WriteLine("You MUST specify one of the parameters, -s -p, or -b");
                Console.Error.WriteLine("-s\t\t Run in single threaded mode");
                Console.Error.WriteLine("-p\t\t Run in parallel mode (uses all available processors)");
                Console.Error.WriteLine("-b\t\t Run in both parallel and single threaded mode.\n\t\t Runs parallel followed by sequential mode");
                return;
            }
            else
            {
                //Handle the sequential command
                if (args[0].ToString() == "-s")
                {
                    string pathName = args[1].ToString();
                    options.PerformSequential(pathName);

                }
                //Handle the parallel command
                else if (args[0].ToString() == "-p")
                {
                    string pathName = args[1].ToString();
                    options.PerformParallel(pathName);

                }
                //Handle both sequential and parallel processing command
                else if (args[0].ToString() == "-b")
                {
                    string pathName = args[1].ToString();
                    options.PerformSequentialAndParallel(pathName);

                }
                //This is the case where the user enters another cmd line arg that is not valid.
                else
                {
                    Console.Error.WriteLine("Usage: du [-s] [-p] [-b] <path>");
                    Console.Error.WriteLine("You MUST specify one of the parameters, -s -p, or -b");
                    Console.Error.WriteLine("-s\t\t Run in single threaded mode");
                    Console.Error.WriteLine("-p\t\t Run in parallel mode (uses all available processors)");
                    Console.Error.WriteLine("-b\t\t Run in both parallel and single threaded mode.\n\t\tRuns parallel followed by sequential mode");
                    return;
                }
            }

        }
    }
}
