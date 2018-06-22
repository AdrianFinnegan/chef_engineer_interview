using System.Collections.Concurrent;

namespace DirectoryAnalyser.Models
{
    public class AnalysisReport
    {
        public ConcurrentBag<string> Common;
        public ConcurrentBag<string> OnlyInAOutput;
        public ConcurrentBag<string> OnlyInBOutput;

        public AnalysisReport(ConcurrentBag<string> common, ConcurrentBag<string> onlyInA, ConcurrentBag<string> onlyInB)
        {
            Common = common;
            OnlyInAOutput = onlyInA;
            OnlyInBOutput = onlyInB;
        }
    }
}
