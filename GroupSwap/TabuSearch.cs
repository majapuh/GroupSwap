using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupSwap
{
    public class TabuSearch: Heuristic
    {      
        List<int> neighbourHood;
        int neighbourhoodSize = (int)(0.2*Container.Requests.Count);

        public  List<int> swapWeights;

        public Dictionary<Tuple<ulong, ulong>, ulong> newStudentActivityGroups;
        public Dictionary<ulong, List<ulong>> newStudentGroups;
        public Dictionary<ulong, int[]> newLimits;
        public List<StudentCurrentState> newStudentStates = new List<StudentCurrentState>();
        public Dictionary<ulong, int[]> newStudentSwaps;

        public TabuSearch(List<int> _swapWeights)
        {
            this.swapWeights = _swapWeights;
        }

        public void DoSearch()
        {
            var newListRequests = Container.Requests;

            CopyData();
            CreateNeighbourhood(rand);


            foreach (var index in neighbourHood)
            {
                var request = newListRequests.ElementAt(index);

                var currentGroup = newStudentActivityGroups[Tuple.Create(request.StudentId, request.ActivityId)];

                if (currentGroup != request.RequestedGroupId)
                {
                    if (CheckConstraints(request) == true)
                    {
                        MakeSwap(request);
                 
                    }
                }
            }

           

            Evaluation = CalculatePoints(swapWeights, newLimits, newStudentSwaps);

            if (Evaluation > Container.Evaluation)
            {
                Console.WriteLine("TabuSearch, Current solution points = {0}", Evaluation);
            }               
            
        }

        private void CreateNeighbourhood(Random rand)
        {
            neighbourHood = new List<int>();
            while (neighbourHood.Count < neighbourhoodSize)
            {
                var index = rand.Next(0, Container.Requests.Count());

                if (neighbourHood.Contains(index) == false)
                {
                    neighbourHood.Add(index);
                }
            }
        
        }

        public override bool CheckConstraints(Request request)
        {
            bool requestPossible = true;

            //overlap
            if (Container.Overlaps.ContainsKey(request.RequestedGroupId))
            {
                foreach (var group in newStudentGroups[request.StudentId])
                {
                    bool overLap = Container.Overlaps[request.RequestedGroupId].Contains(group);

                    if (overLap)
                    {
                        requestPossible = false;
                        return requestPossible;
                    }
                }
            }

            //min/max
            if (newLimits[request.RequestedGroupId][0] + 1 > newLimits[request.RequestedGroupId][3])
            {
                requestPossible = false;
                return requestPossible;
            }

            ulong currentGroup = newStudentActivityGroups[Tuple.Create(request.StudentId, request.ActivityId)];
            if (newLimits[currentGroup][0] - 1 < newLimits[currentGroup][1])
            {
                requestPossible = false;
                return requestPossible;
            }
            

            return requestPossible;

        }

        public override void MakeSwap(Request request)
        {
            var index = newStudentStates.FindIndex(x => x.StudentId == request.StudentId && x.ActivityId == request.ActivityId);
            newStudentStates.ElementAt(index).NewGroupId = request.RequestedGroupId;
            //increase swap number
            newStudentStates.ElementAt(index).SwapCnt += 1;
            

            //add swap weight
            if (newStudentStates.ElementAt(index).SwapCnt == 1)
            {
                swapWeights.Add(newStudentStates.ElementAt(index).SwapWeight);
            }
            
            ulong currentGroup = newStudentActivityGroups[Tuple.Create(request.StudentId, request.ActivityId)];
            newLimits[request.RequestedGroupId][0] += 1;
            newLimits[currentGroup][0] -= 1;

            //count swaps
            if (newStudentStates.ElementAt(index).SwapCnt == 1)
            {
                newStudentSwaps[request.StudentId][1] += 1;
            }

            //check current groups
            newStudentGroups[request.StudentId].Remove(currentGroup);
            newStudentGroups[request.StudentId].Add(request.RequestedGroupId);
            //multiple possible swaps
            newStudentActivityGroups[Tuple.Create(request.StudentId, request.ActivityId)] = request.RequestedGroupId;

        }
        

        private void CopyData()
        { 
            newStudentActivityGroups = new Dictionary<Tuple<ulong, ulong>, ulong>();
            newStudentGroups = new Dictionary<ulong, List<ulong>>();
            newLimits = new Dictionary<ulong, int[]>();
            newStudentStates = new List<StudentCurrentState>();
            newStudentSwaps = new Dictionary<ulong, int[]>();

        
            foreach (var state in Container.NewStudentStates)
            {
                StudentCurrentState state_ = new StudentCurrentState(state.StudentId, state.ActivityId, state.SwapWeight, state.CurrentGroupId, state.NewGroupId, state.SwapCnt);
                newStudentStates.Add(state_);
            }

            foreach (var group in Container.StudentActivityGroups)
            {
                newStudentActivityGroups[Tuple.Create(group.Key.Item1, group.Key.Item2)] = group.Value;
            }

            foreach (var group in Container.StudentGroups)
            {
                newStudentGroups[group.Key] = group.Value;
            }

            foreach(var limit in Container.Limits)
            {
                newLimits[limit.Key] = limit.Value;
            }

            foreach (var swap in Container.StudentSwaps)
            {
                newStudentSwaps[swap.Key] = swap.Value;
            }

        }

        public void CopyToContainer()
        {
            Container.StudentActivityGroups = new Dictionary<Tuple<ulong, ulong>, ulong>();
            Container.StudentGroups = new Dictionary<ulong, List<ulong>>();
            Container.Limits = new Dictionary<ulong, int[]>();
            Container.StudentSwaps = new Dictionary<ulong, int[]>();


            foreach (var group in newStudentActivityGroups)
            {
                Container.StudentActivityGroups[Tuple.Create(group.Key.Item1, group.Key.Item2)] = group.Value;
            }

            foreach (var group in newStudentGroups)
            {
                Container.StudentGroups[group.Key] = group.Value;
            }

            foreach (var limit in newLimits)
            {
                Container.Limits[limit.Key] = limit.Value;
            }

            foreach (var swap in newStudentSwaps)
            {
                Container.StudentSwaps[swap.Key] = swap.Value;
            }


        }


    }
}
