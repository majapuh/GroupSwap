using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace GroupSwap
{
    class Program
    {
        static void Main(string[] args)
        {
            //initial settings
            int timeout = 3600;
            int greedyTimeout = 15;
            string awardActivity = "1,2,4";
            int awardStudent = 1;
            int minMaxPenalty = 1;
            string studentsFileName = @"C:\Instances\1\student[1].csv"; 
            string requestsFileName = @"C:\Instances\1\requests[1].csv";
            string overlapsFileName = @"C:\Instances\1\overlaps[1].csv";
            string limitsFileName = @"C:\Instances\1\limits[1].csv";


            for (var i = 0; i < args.Length - 1; ++i)
            {
                switch (args[i])
                {
                    case "-timeout":
                        timeout = int.Parse(args[++i]);
                        break;
                    case "-award-activity":
                        awardActivity = args[++i];
                        break;
                    case "-award-student":
                        awardStudent = int.Parse(args[++i]);
                        break;
                    case "-minmax-penalty":
                        minMaxPenalty = int.Parse(args[++i]);
                        break;
                    case "-students-file":
                        studentsFileName = args[++i];
                        break;
                    case "-requests-file":
                        requestsFileName = args[++i];
                        break;
                    case "-overlaps-file":
                        overlapsFileName = args[++i];
                        break;
                    case "-limits-file":
                        limitsFileName = args[++i];
                        break;
                    default:
                        Console.WriteLine("Please enter valid number of arguments.");
                        return;
                }
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();


            using (var reader = new StreamReader(studentsFileName))
            {
                var headerLine = reader.ReadLine();

                List<StudentCurrentState> _studentStates = new List<StudentCurrentState>();
                Dictionary<ulong, List<ulong>> _studentGroups = new Dictionary<ulong, List<ulong>>();
                Dictionary<Tuple<ulong, ulong>, ulong> _studentActivityGroups = new Dictionary<Tuple<ulong, ulong>, ulong>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');


                    List<int> attributes = new List<int>();

                    foreach (var value in values)
                    {
                        if (int.TryParse(value, out var num) == false)
                        {
                            Console.WriteLine("Error while reading Student File.");
                            return;
                        }
                        else
                        {
                            attributes.Add(num);
                        }
                    }

                    StudentCurrentState studentState = new StudentCurrentState((ulong)attributes[0], (ulong)attributes[1], attributes[2], (ulong)attributes[3], (ulong)attributes[4]);
                    _studentStates.Add(studentState);

                    if (_studentGroups.ContainsKey((ulong)attributes[0]))
                    {
                        _studentGroups[(ulong)attributes[0]].Add((ulong)attributes[(3)]);
                    }
                    else
                    {
                        _studentGroups[(ulong)attributes[0]] = new List<ulong> { (ulong)attributes[(3)] };
                    }

                    _studentActivityGroups[Tuple.Create((ulong)attributes[0], (ulong)attributes[1])] = (ulong)attributes[3];

                    
                }
                
                Container.StudentStates = _studentStates;
                Container.StudentGroups = _studentGroups;
                Container.StudentActivityGroups = _studentActivityGroups;
            }



            using (var reader = new StreamReader(requestsFileName))
            {
                var headerLine = reader.ReadLine();

                List<Request> _requests = new List<Request>();
                Dictionary<ulong, int[]> _groupRequests = new Dictionary<ulong, int[]>();
                Dictionary<Tuple<ulong, ulong>, List<ulong>> _studentActivityRequestedGroups = new Dictionary<Tuple<ulong, ulong>, List<ulong>>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');


                    List<int> attributes = new List<int>();

                    foreach (var value in values)
                    {
                        if (int.TryParse(value, out var num) == false)
                        {
                            Console.WriteLine("Error while reading Requests File.");
                            return;
                        }
                        else
                        {
                            attributes.Add(num);
                        }
                    }
                    
                    if (Container.StudentActivityGroups.ContainsKey(Tuple.Create((ulong)attributes[0], (ulong)attributes[1])))
                    {
                        Request request = new Request((ulong)attributes[0], (ulong)attributes[1], (ulong)attributes[2]);
                        _requests.Add(request);


                        if (_groupRequests.ContainsKey((ulong)attributes[2]))
                        {
                            _groupRequests[(ulong)attributes[2]][0] += 1;
                        }
                        else
                        {
                            _groupRequests[(ulong)attributes[2]] = new int[] { 1, 0};
                        }

                        if (_studentActivityRequestedGroups.ContainsKey(Tuple.Create((ulong)attributes[0], (ulong)attributes[1])))
                        {
                            _studentActivityRequestedGroups[Tuple.Create((ulong)attributes[0], (ulong)attributes[1])].Add((ulong)attributes[2]);
                        }
                        else
                        {
                            _studentActivityRequestedGroups[Tuple.Create((ulong)attributes[0], (ulong)attributes[1])] = new List<ulong> { (ulong)attributes[2] };
                        }

                    }



                }
                Dictionary<ulong, int[]> _studentSwaps = new Dictionary<ulong, int[]>();

                var keys = _studentActivityRequestedGroups.Keys.ToList();
                var keysGrouped = keys.GroupBy(x => x.Item1)
                                            .Select(x => new {
                                                studentId = x.Key,
                                                Count = x.Count() });

                foreach (var student in keysGrouped)
                {
                    if (_studentSwaps.ContainsKey(student.studentId))
                    {
                        _studentSwaps[(student.studentId)][0] = student.Count;
                    }
                    else
                    {
                        _studentSwaps[student.studentId] = new int[] { student.Count, 0 };
                    }
                }

               
                Container.Requests = _requests;
                Container.StudentSwaps = _studentSwaps;
                Container.StudentActivityRequestedGroups = _studentActivityRequestedGroups;
                Container.GroupRequests = _groupRequests;
            }


            using (var reader = new StreamReader(overlapsFileName))
            {
                Dictionary<ulong, List<ulong>> _overlaps = new Dictionary<ulong, List<ulong>>();

                var headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    List<int> attributes = new List<int>();

                    foreach (var value in values)
                    {
                        if (int.TryParse(value, out var num) == false)
                        {
                            Console.WriteLine("Error while reading Overlaps File.");
                            return;
                        }
                        else
                        {
                            attributes.Add(num);
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        if (_overlaps.ContainsKey((ulong)attributes[i]))
                        {
                            _overlaps[(ulong)attributes[i]].Add((ulong)attributes[(i + 1) % 2]);
                        }
                        else
                        {
                            _overlaps[(ulong)attributes[i]] = new List<ulong> { (ulong)attributes[(i + 1) % 2] };
                        }
                    }

                }
                Container.Overlaps = _overlaps;
            }

            using (var reader = new StreamReader(limitsFileName))
            {
                Dictionary<ulong, int[]> _limits = new Dictionary<ulong, int[]>();

                var headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    List<int> attributes = new List<int>();

                    foreach (var value in values)
                    {
                        if (int.TryParse(value, out var num) == false)
                        {
                            Console.WriteLine("Error while reading Limits File.");
                            return;
                        }
                        else
                        {
                            attributes.Add(num);
                        }
                    }

                    
                    //cnt, min, min_p, max, max_p
                    _limits[(ulong)attributes[0]] = new int[] { attributes[1], attributes[2], attributes[3], attributes[4], attributes[5] };

                    if (Container.GroupRequests.ContainsKey((ulong)attributes[0]))
                    {
                        if (attributes[1] < attributes[3])
                        {
                            Container.GroupRequests[(ulong)attributes[0]][1] = attributes[3] - attributes[1];
                          
                        }

                    }

                    
                }
                Container.Limits = _limits;
             }

            var rand = new Random();
            Container.NumberOfEvaluations = 0;
            Container.Evaluation = 0;
            bool doGreedy = true;

            Greedy greedy = new Greedy();
            greedy.Initialize(awardActivity, awardStudent, minMaxPenalty, rand);

            sw.Stop();
            var elapsed = sw.Elapsed.TotalSeconds;
            sw.Start();

            List<int> _swapWeigh = new List<int>();
            bool tabuImproved = false;

            while (elapsed < timeout)
            {                
                while (doGreedy)
                {
                    greedy.ReadRequests();
                    var eval = greedy.ReturnEvaluation();                   

                    if (eval > Container.Evaluation || Container.Evaluation == 0)
                    {
                        Container.Evaluation = eval;
                        Console.WriteLine("Greedy, Current solution points = {0}", eval);
                    }


                    sw.Stop();
                    elapsed = sw.Elapsed.TotalSeconds;
                    sw.Start();

                    if (elapsed > greedyTimeout)
                    {
                        doGreedy = false;
                    }


                }
                Console.WriteLine("Final Greedy, Current solution points = {0}", Container.Evaluation);

                greedy.MakeNewStudentStates(Container.StudentStates);
                

                var _swapWeights = greedy.swapWeights;
                TabuSearch tabu = new TabuSearch(_swapWeights);
                tabu.Initialize(awardActivity, awardStudent, minMaxPenalty, rand);

                sw.Stop();
                elapsed = sw.Elapsed.TotalSeconds;
                sw.Start();

                while (elapsed < timeout)
                {                    
                    tabu.DoSearch();
                    var tabuEvaluation = tabu.ReturnEvaluation();
                

                    if (tabuEvaluation > Container.Evaluation)
                    {
                        tabu.MakeNewStudentStates(tabu.newStudentStates);
                        tabu.CopyToContainer();
                        Container.Evaluation = tabuEvaluation;

                        _swapWeights = tabu.swapWeights;

                        tabuImproved = true;
                        
                    }
                    else
                    {
                        tabu.swapWeights = _swapWeights;
                    }

                    sw.Stop();
                    elapsed = sw.Elapsed.TotalSeconds;
                    sw.Start();


                    _swapWeigh = _swapWeights;

                }
            }

            sw.Stop();

            if (tabuImproved == true) CopyNewStatesToStates();
            
            string newFileName = @"newStudent.csv";
            using (var reader = new StreamReader(studentsFileName))
            {
                using (var writer = new StreamWriter(newFileName))
                {
                    var headerLine = reader.ReadLine();
                    writer.WriteLine(headerLine);

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        var studentId = ulong.Parse(values[0]);
                        var activityId = ulong.Parse(values[1]);
                        var oldGroupId = ulong.Parse(values[3]);

                        var newGroupId = Container.StudentStates.Where(s => s.StudentId == studentId
                        && s.ActivityId == activityId && s.CurrentGroupId == oldGroupId).FirstOrDefault().NewGroupId;
                        values[4] = newGroupId.ToString();

                        if (newGroupId == 0)
                        {
                            values[4] = values[3];
                        }

                        writer.WriteLine(String.Join(",", values));
                    }
                }
            }

             File.Replace(newFileName, studentsFileName, null);
             File.Delete(newFileName);


            Console.WriteLine("Final evaluation: {0}", Container.Evaluation);
            Console.WriteLine("Number of evaluations: {0}", Container.NumberOfEvaluations);
        }

        public static void CopyNewStatesToStates()
        {

            foreach (var state in Container.NewStudentStates)
            {
                if (state.SwapCnt > 0)
                {
                    var index = Container.StudentStates.FindIndex(x => x.StudentId == state.StudentId && x.ActivityId == state.ActivityId);
                    var oldState = Container.StudentStates.ElementAt(index);
                    if (oldState.NewGroupId != state.CurrentGroupId)
                    {
                        Container.StudentStates.ElementAt(index).NewGroupId = state.CurrentGroupId;
                    }

                }
             }
        }

    }
}
