using DirectoryAnalyser.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DirectoryAnalyser
{
    public class Compare
    {
        private const string COMMON_OUTPUT_NAME = "./common";
        private const string ONLY_IN_A_OUTPUT_NAME = "./a_only";
        private const string ONLY_IN_B_OUTPUT_NAME = "./b_only";

        public static void Main(string[] args)
        {
            if (args.Length != 2 || args.Any(a=>a == null))
            {
                return;
            }

            CompareDirectories(args[0], args[1]);
        }

        public static void CompareDirectories(string source, string destination)
        {
            var sourceContents = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories).Select(s => Path.GetFullPath(s)).ToList();
            var destinationContents = Directory.GetFiles(destination, "*.*", SearchOption.AllDirectories).Select(s => Path.GetFullPath(s)).ToList();

            var hashedSourceDirectory = GetData(sourceContents);
            var hashedDestinationDirectory = GetData(destinationContents);

            var analysisReport = Analyse(hashedSourceDirectory, hashedDestinationDirectory);

            WriteComparisonReport(analysisReport);

        }

        /// <summary>
        /// Returns the content of the folder as a hashedtable of file and checksum
        /// </summary>
        /// <param name="folderContents">A list of files within a folder</param>
        /// <returns>Dictionary of files along with their checksum</returns>
        private static ConcurrentDictionary<string, string> GetData(List<string> folderContents)
        {
            ConcurrentDictionary<string, string> hashedDirectory = new ConcurrentDictionary<string, string>();

            Parallel.ForEach(folderContents, currentFile =>
            {
                using (var stream = File.OpenRead(currentFile))
                {
                    using (var md5 = MD5.Create())
                    {
                        hashedDirectory.TryAdd(currentFile, BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower());
                    }
                }

            });

            return hashedDirectory;
        }

        private static void WriteComparisonReport(AnalysisReport analysisReport)
        {
            //Output files
            using (var stream = new StreamWriter(COMMON_OUTPUT_NAME, true))
            {
                foreach (var item in analysisReport.Common.AsParallel().AsOrdered())
                {
                    stream.WriteLine(item);
                }
            }
            using (var stream = new StreamWriter(ONLY_IN_A_OUTPUT_NAME, true))
            {
                foreach (var item in analysisReport.OnlyInAOutput.AsParallel().AsOrdered())
                {
                    stream.WriteLine(item);
                }
            }
            using (var stream = new StreamWriter(ONLY_IN_B_OUTPUT_NAME, true))
            {
                foreach (var item in analysisReport.OnlyInBOutput.AsParallel().AsOrdered())
                {
                    stream.WriteLine(item);
                }
            }
        }

        private static AnalysisReport Analyse(ConcurrentDictionary<string, string> hashedSourceDirectory, ConcurrentDictionary<string, string> hashedDestinationDirectory)
        {

            ConcurrentBag<string> common = new ConcurrentBag<string>();
            ConcurrentBag<string> onlyInAOutput = new ConcurrentBag<string>();
            ConcurrentBag<string> onlyInBOutput = new ConcurrentBag<string>();

            Parallel.ForEach(hashedSourceDirectory, itemInA =>
            {
                if (hashedDestinationDirectory.Any(a => a.Value == itemInA.Value))
                {
                    common.Add(itemInA.Key);
                }
                else
                {
                    onlyInAOutput.Add(itemInA.Key);
                }
            });

            //Items only in B
            Parallel.ForEach(hashedDestinationDirectory, itemInB =>
            {
                if (hashedSourceDirectory.Any(a => a.Value == itemInB.Value))
                {
                }
                else
                {
                    onlyInBOutput.Add(itemInB.Key);
                }

            });

            return new AnalysisReport(common, onlyInAOutput, onlyInBOutput);
        }
    }
}
