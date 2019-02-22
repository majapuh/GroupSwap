
namespace GroupSwap
{
    public class TabuItem
    {
        public ulong StudentId { get; set; }
        public int IterationCnt { get; set; }

        public TabuItem(ulong _studentId, int _iterationCnt)
        {
            this.StudentId = _studentId;
            this.IterationCnt = _iterationCnt;
        }
    }
}