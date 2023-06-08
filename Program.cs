// Robert Tetreault (rrt2850@g.rit.edu)

/*******************************************************************************************
* File: Program.cs
* -----------------
* This program is my solution to the Project 1 assignment for CSCI251. It is a recreation
* of the UNIX "du" command, which recursively lists the size of all files and directories
* in a given directory.
*
* Note: I run this like 'dotnet run -- -p "C:\"' but it does work when you run the exe
*       when you do 'dotnet run' initially to generate the exe, it will display the help
*       message because it thinks you ran it without giving it arguments
*       on my laptop, it generated the exe file at '.\bin\Debug\net6.0\du.exe'
********************************************************************************************/

using System.Diagnostics;

namespace DiskUsage
{
    public class Program{
        // Initialize console lock so writing to console is protected
        private static readonly object consoleLock = new object();

        static void Main(string[] args){
            // Check if input is valid and then execute methods accordingly
            if(args.Length >= 2 && Directory.Exists(args[1])){
                switch(args[0]){
                    case "-s":
                        // Run in sequential
                        SequentialDU(args[1]);
                        return;
                    case "-p":
                        // Run in parallel
                        ParallelDU(args[1]);
                        return;
                    case "-b":
                        // Run in both parallel and sequential
                        ParallelDU(args[1]);
                        SequentialDU(args[1]);
                        return;
                    default:
                        break; // If args[0] is invalid, print the help messages and return
                }
            }
            // Print the proper argument format and return
            PrintHelp();
        }

        /// <summary>
        /// Recursively lists the size of all files and directories in a given directory sequentially.
        /// </summary>
        /// <param name="directory">The directory to use</param>
        private static void SequentialDU(string directory){
            // Start timer to see how long SequentialDU takes
            var timer = Stopwatch.StartNew();

            long totalBytes = 0;
            int totalFiles = 0;
            int totalFolders = 0;

            /// <summary>
            /// Recursively traverses a directory and all of its subdirectories,
            /// determining the total bytes, files, and folders as it goes.
            /// </summary>
            /// <param name="dir">The directory to traverse</param>
            /// <remarks>
            /// This function is inside SequentialDU so that it can use the
            /// totalBytes, totalFiles, and totalFolders variables
            /// </remarks>
            void MeasureDir(string dir){
                try{
                    string[] subDirectories = Directory.GetDirectories(dir);
                    string[] files = Directory.GetFiles(dir);

                    // Traverse each subdirectory before accessing files incase
                    // there's an access issue causing the method to quit
                    foreach(var subDirectory in subDirectories){
                        totalFolders++;
                        MeasureDir(subDirectory);
                    }
                    foreach(var file in files){
                        totalFiles++;
                        totalBytes += new FileInfo(file).Length;
                    }
                }
                catch{}
            }

            try{
                //  Measure directory disk usage
                MeasureDir(directory);
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
            finally{
                //  Stop the timer after measuring
                timer.Stop();
            }
            lock(consoleLock) {
                Console.WriteLine($"Sequential Calculated in: {timer.Elapsed.TotalSeconds}s");
                Console.WriteLine($"{totalFolders:N0} folders, {totalFiles:N0} files, {totalBytes:N0} bytes");
            }
            
        }

        /// <summary>
        /// Recursively lists the size of all files and directories in a given directory in parallel.
        /// </summary>
        /// <param name="directory">The directory to use</param>
        private static void ParallelDU(string directory){
            // Start timer to see how long ParallelDU takes
            var timer = Stopwatch.StartNew();
            long totalBytes = 0;
            int totalFiles = 0;
            int totalFolders = 0;

            /// <summary>
            /// Recursively traverses a directory and all of its subdirectories,
            /// determining the total bytes, files, and folders as it goes.
            /// </summary>
            /// <param name="dir">The directory to traverse</param>
            /// <remarks>
            /// This function is inside ParallelDU so that it can use the
            /// totalBytes, totalFiles, and totalFolders variables
            /// </remarks>
            void MeasureDir(string dir){
                try{
                    string[] subDirectories = Directory.GetDirectories(dir);
                    string[] files = Directory.GetFiles(dir);

                    // Start thread for each subdirectory before measuring files
                    Parallel.ForEach(subDirectories, subDirectory => {
                        try{
                            Interlocked.Increment(ref totalFolders);
                            MeasureDir(subDirectory);
                        }
                        catch{}
                        
                    });
                    // Start a thread for measuring each file
                    Parallel.ForEach(files, file =>{
                        try{
                            Interlocked.Increment(ref totalFiles);
                            Interlocked.Add(ref totalBytes, new FileInfo(file).Length);
                        }
                        catch{}
                    });
                }
                catch(Exception e){
                    Console.WriteLine(e.Message);
                }
            }

            try{
                //  Measure directory disk usage
                MeasureDir(directory);
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
            finally{
                //  Stop the timer after measuring
                timer.Stop();
            }
            Console.WriteLine($"Parallel Calculated in: {timer.Elapsed.TotalSeconds}s");
            Console.WriteLine($"{totalFolders:N0} folders, {totalFiles:N0} files, {totalBytes:N0} bytes");
           
        }

        /// <summary>
        /// Prints the proper usage of the program, used when input is invalid
        /// </summary>
        /// <remarks>
        /// I was going to do '\n\t' and have all these writes in the same line,
        /// but I chose not to for readability
        /// </remarks>
        private static void PrintHelp(){
            Console.WriteLine("Usage: du [-s] [-p] [-b] <path>");
            Console.WriteLine("Summarize disk usage of the set of FILES, recursively for directories.");
            Console.WriteLine("You MUST specify one of the parameters, -s, -p, or -b");
            Console.WriteLine("\t-s Run in single threaded mode");
            Console.WriteLine("\t-p Run in parallel mode (uses all available processors)");
            Console.WriteLine("\t-b Run in both parallel and single threaded mode. (runs parallel followed by sequential mode)");
        }
    }
}