/* Developer: Isaias Villalobos
 * Date: 9/17/2019
 * Version: 1.0
 * This is a class that handles processing a directory in a sequential manner and parallel manner
 * This class makes use of the Parallel library to concurrently find files in a directory.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TestRecursion
{
    public class ProgramOptions
    {
        //Variables used for thread-safe code with regards to ParallelProcessDirectory() function
        static object locker = new object();
        static object locker2 = new object();

        //General Variables used for calcualtions of functions
        public static long seqTotalFiles = 0;
        public static long seqTotalSize = 0;
        public static long seqTotalFolders = 0;

        public static long parTotalFiles = 0;
        public static long parTotalSize = 0;
        public static long parTotalFolders = 0;

        /*
         * Input: A string representing the name(path) of a directory
         * Recursive function that will make sure that the count total files, total size, total folders.
         * To be used for the sequential implementation only.
         * Return: Void
         */
        public static void ProcessDirectory(string targetDirectory)
        {
            try
            {
                var dirInfo = new DirectoryInfo(targetDirectory);
                //dirInfo.Refresh();

                // List all files in the current directory.
                foreach (var fileName in dirInfo.GetFiles())
                {
                    seqTotalFiles += 1;
                    seqTotalSize += fileName.Length;
                }

                // Recurse into subdirectories of this directory.
                foreach (var subdirectory in dirInfo.GetDirectories())
                {
                    seqTotalFolders += 1;

                    //Pass path of subdirectory to the recursive function.
                    ProcessDirectory(subdirectory.FullName);
                }
            }
            catch (UnauthorizedAccessException) { }
        }


        /*
         * Input: A string representing the name(path) of a directory
         * Recursive function that will make sure that the count total files, total size, total folders.
         * To be used for the parallel implementation only.
         * Return: Void
        */
        public static void ParallelProcessDirectory(string targetDirectory)
        {
            try

            {
                // List all files in the current directory.
                var dirInfo = new DirectoryInfo(targetDirectory);
                //dirInfo.Refresh();

                Parallel.ForEach(dirInfo.GetFiles(), (fileEntry) =>
                {
                    lock (locker)
                    {
                        parTotalFiles += 1;
                        parTotalSize += fileEntry.Length;
                    }
                });
            }
            catch (UnauthorizedAccessException) { } //getfiles()
            catch (DirectoryNotFoundException) { } //getfiles() throws

            try
            {
                var myinfo = new DirectoryInfo(targetDirectory);
                Parallel.ForEach(myinfo.GetDirectories(), (subdirectory) =>
                {
                    //Needs a seperate locker for keeping track of this variable.
                    lock (locker2)
                    {
                        parTotalFolders += 1;
                    }
                    //Pass path of subdirectory to the recursive function.
                    ParallelProcessDirectory(subdirectory.FullName);
                });
            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { } //getfiles() throws

        }

        /*
        * Input: A string representing the path of a directory
        * This function wil set directory, will call a recursive function to process a directory
        * This function will time how long it takes to process a directory sequentially
        * Will only execute given the correct cmd line arguments
        * Return: Void
        */
        public void PerformSequential(string pathName)
        {

            //Set current directory
            Directory.SetCurrentDirectory(pathName);

            //Create new stopwatch to time the program
            var watch = Stopwatch.StartNew();

            //Recursively find total size and folder count
            ProcessDirectory(pathName);
            watch.Stop();

            var elapsedTime = watch.Elapsed.TotalSeconds;

            //Format of the time NEEDS to be fixed
            Console.WriteLine($"Sequential Calculated in: {elapsedTime}s");

            //This printing needs to be formatted so that the commas are placed correctly.
            Console.WriteLine("{0:N0} folders, {1:N0} files, {2:N0} bytes", seqTotalFolders, seqTotalFiles, seqTotalSize);

        }

        /* Input: A string pathname that is given to this program
         * Input: A string representing the path of a directory
         * This function wil set directory, will call a recursive function to process a directory
         * This function will time how long it takes to process a directory concurrently aka in parallel
         * Will only execute given the correct cmd line arguments
         * Return: Void
        */
        public void PerformParallel(string pathName)
        {

            //Set to the correct directory
            Directory.SetCurrentDirectory(pathName);

            //Create new stopwatch and begin timing
            var watch = Stopwatch.StartNew();

            ParallelProcessDirectory(pathName);

            watch.Stop();
            var elapsedTime = watch.Elapsed.TotalSeconds;

            //Printing needs to be formatted to included commans
            Console.WriteLine($"Parallel Calculated in: {elapsedTime}s");

            //Format the printing of files, add commas to printout statements
            Console.WriteLine("{0:N0} folders, {1:N0} files, {2:N0} bytes", parTotalFolders, parTotalFiles, parTotalSize);

        }

        /* Input: String pathname of directory
         * This function is called when the correct argument to the cmd line arguemnts are given
         * Return: Void
         */
        public void PerformSequentialAndParallel(string pathName)
        {
            //Set to the correct directory
            Directory.SetCurrentDirectory(pathName);

            //Call parallel directory processing function
            PerformParallel(pathName);

            //Formatting 
            Console.WriteLine("\n");

            //Call sequential directory processing function
            PerformSequential(pathName);
        }
    };
}

