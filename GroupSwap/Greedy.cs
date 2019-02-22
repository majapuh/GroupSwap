using System;
using System.Collections.Generic;
using System.Linq;


namespace GroupSwap
{
    public class Greedy: Heuristic
    {
        private int tabuDuration = 10;
        private Queue<TabuItem> tabuList = new Queue<TabuItem>();

        public List<int> swapWeights = new List<int>();

        public void ReadRequests()
        {
            ulong readIteration = 0;

            var _requests = Container.StudentActivityRequestedGroups.OrderByDescending(x => x.Value.Count).ToList();

            foreach (var requestFromDict in _requests)
            {
                if (tabuList.Where(x => x.StudentId == requestFromDict.Key.Item1).Count() == 0 || tabuList.Count == 0) 
                {
                    var groups = Container.GroupRequests.Where(x => requestFromDict.Value.Contains(x.Key))
                        .OrderByDescending(x => x.Value[1]).ThenBy(x => x.Value[0]);

                    int groupNumber = rand.Next(0, groups.Count());
                    int cnt = 0;

                    foreach (var group in groups)
                    {
                        var request = new Request(requestFromDict.Key.Item1, requestFromDict.Key.Item2, group.Key);

                        var currentGroup = Container.StudentActivityGroups[Tuple.Create(request.StudentId, request.ActivityId)];

                        if (currentGroup != request.RequestedGroupId)
                        {
                            if (CheckConstraints(request) == true)
                            {
                                MakeSwap(request);
                                break;
                            }
                        }
                        cnt += 1;
                        if (cnt == groupNumber) break;
                       
                    }
                }

                //every nth iteration write fitness
                if (readIteration % 10000 == 0)
                {
                    Evaluation = CalculatePoints(swapWeights, Container.Limits, Container.StudentSwaps);
                    Console.WriteLine("Iteration {0}, Current solution points = {1}", readIteration, Evaluation);
                }

                readIteration += 1;

                tabuList.All(x => { x.IterationCnt -= 1; return true; });

                if (tabuList.Count > 0)
                {
                    if (tabuList.Peek().IterationCnt == 0)
                    {
                        tabuList.Dequeue();
                    }
                }

            }
            Evaluation = CalculatePoints(swapWeights, Container.Limits, Container.StudentSwaps);
        }



        public override bool CheckConstraints(Request request)
        {
            bool requestPossible = true;

            //overlap
            if (Container.Overlaps.ContainsKey(request.RequestedGroupId))
            {
                foreach (var group in Container.StudentGroups[request.StudentId])
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
            if (Container.Limits[request.RequestedGroupId][0] + 1 > Container.Limits[request.RequestedGroupId][3])
            {
                requestPossible = false;
                return requestPossible;
            }

            ulong currentGroup = Container.StudentActivityGroups[Tuple.Create(request.StudentId, request.ActivityId)];
            if (Container.Limits[currentGroup][0] - 1 < Container.Limits[currentGroup][1])
            {
                requestPossible = false;
                return requestPossible;
            }
            

            return requestPossible;
        }


        public override void MakeSwap(Request request)
        {
            var index = Container.StudentStates.FindIndex(x => x.StudentId == request.StudentId && x.ActivityId == request.ActivityId);
            Container.StudentStates.ElementAt(index).NewGroupId = request.RequestedGroupId;
            //increase swap number
            Container.StudentStates.ElementAt(index).SwapCnt += 1;

            //add swap weight
            if (Container.StudentStates.ElementAt(index).SwapCnt == 1)
            {
                swapWeights.Add(Container.StudentStates.ElementAt(index).SwapWeight);
            }


            ulong currentGroup = Container.StudentActivityGroups[Tuple.Create(request.StudentId, request.ActivityId)];
            Container.Limits[request.RequestedGroupId][0] += 1;
            Container.Limits[currentGroup][0] -= 1;

            //count swaps
            if (Container.StudentStates.ElementAt(index).SwapCnt == 1)
            {
                Container.StudentSwaps[request.StudentId][1] += 1;
            }

            //check current groups
            Container.StudentGroups[request.StudentId].Remove(currentGroup);
            Container.StudentGroups[request.StudentId].Add(request.RequestedGroupId);
            //multiple possible swaps
            Container.StudentActivityGroups[Tuple.Create(request.StudentId, request.ActivityId)] = request.RequestedGroupId;
           

            Container.GroupRequests[request.RequestedGroupId][1] += 1;

            if (Container.GroupRequests.ContainsKey(currentGroup))
            {
                Container.GroupRequests[currentGroup][1] -= 1;
            }

            //add to tabu list
            var tabuItem = new TabuItem(request.StudentId, tabuDuration);
            tabuList.Enqueue(tabuItem);
        }
       

    }
}
