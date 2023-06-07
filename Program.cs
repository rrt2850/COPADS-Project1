// Robert Tetreault (rrt2850@g.rit.edu)

/*******************************************************************************************
* File: Program.cs
* -----------------
* This program is my solution to the Project 1 assignment for CSCI251. It is a recreation
* of the UNIX "du" command, which recursively lists the size of all files and directories
* in a given directory.
********************************************************************************************/

using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DiskUsage{
    public class Program{
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
            return;
        }

        /// <summary>
        /// Recursively lists the size of all files and directories in a given directory sequentially.
        /// </summary>
        /// <param name="directory">The directory to use</param>
        private static void SequentialDU(string directory){

        }

        /// <summary>
        /// Recursively lists the size of all files and directories in a given directory in parallel.
        /// </summary>
        /// <param name="directory">The directory to use</param>
        private static void ParallelDU(string directory){

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
            return;
        }
    }
}