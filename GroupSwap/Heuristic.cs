using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupSwap
{
    public abstract class Heuristic
    {
        public int Evaluation { get; set; }

        public int AwardStudent { get; set; }
        public List<int> AwardActivity = new List<int>();
        public int MinMaxPenalty { get; set; }

        public Random rand;

        public void Initialize(string _awardActivity, int _awardStudent, int _minMaxPenalty, Random _rnd)
        {
            this.AwardActivity = _awardActivity.Split(',').ToList().Select(int.Parse).ToList();
            this.AwardStudent = _awardStudent;
            this.MinMaxPenalty = _minMaxPenalty;
            this.rand = _rnd;
        }

        public int CalculatePoints(List<int> _swapWeights, Dictionary<ulong, int[]> _limits,  Dictionary<ulong, int[]> _studentSwaps)
        {
            var pointsA = _swapWeights.Sum();

            //awardActivity - calculate for every student
            var pointsB = 0;
            var studentsWithSwaps = _studentSwaps.Where(x => x.Value[1] > 0);
            var n = AwardActivity.Count;
            for (int i = 1; i < n; i++)
            {
                pointsB += studentsWithSwaps.Where(x => x.Value[1] == i).Count() * AwardActivity[i - 1];
            }
            pointsB += studentsWithSwaps.Where(x => x.Value[1] >= n).Count() * AwardActivity[n - 1];

            //just for students with all swaps made
            var studentsCnt = _studentSwaps.Where(x => x.Value[0] == x.Value[1]).Count();
            var pointsC = studentsCnt * AwardStudent;

 
            var minPenalty = _limits.Where(x => x.Value[2] > x.Value[0]).Sum(x => x.Value[2] - x.Value[0]);
            var pointsD = minPenalty * MinMaxPenalty;

            var maxPenalty = _limits.Where(x => x.Value[0] > x.Value[4]).Sum(x => x.Value[0] - x.Value[4]);
            var pointsE = maxPenalty * MinMaxPenalty;

            var totalPoints = pointsA + pointsB + pointsC - pointsD - pointsE;

            Container.NumberOfEvaluations += 1;

            return totalPoints;
        }

        public int ReturnEvaluation()
        {
            return Evaluation;
        }

        public void MakeNewStudentStates(List<StudentCurrentState> _studentStates)
        {
            var _newStudentStates = new List<StudentCurrentState>();
            foreach (var state in _studentStates)
            {
                if (state.SwapCnt > 0 && state.NewGroupId != 0)  
                {
                    StudentCurrentState newState = new StudentCurrentState(state.StudentId, state.ActivityId, state.SwapWeight, state.NewGroupId, 0, state.SwapCnt);
                   _newStudentStates.Add(newState);
                }
                else
                {
                    _newStudentStates.Add(state);
                }
               

            }
            Container.NewStudentStates = _newStudentStates;
        }


        public abstract bool CheckConstraints(Request request);

        public abstract void MakeSwap(Request request);


    }
}
