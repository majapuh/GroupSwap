using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupSwap
{
    public static class Container
    {
        public static List<StudentCurrentState> StudentStates { get; set; }

        public static List<StudentCurrentState> NewStudentStates { get; set; }

        //all curent student groups (on all activities) - for overlaps check
        public static Dictionary<ulong, List<ulong>> StudentGroups { get; set; }

        //count of wanted and made swaps 
        public static Dictionary<ulong, int[]> StudentSwaps { get; set; }

        //current student group on specific activity
        public static Dictionary<Tuple<ulong, ulong>, ulong> StudentActivityGroups { get; set; }

        //wanted groups on specific activity
        public static Dictionary<Tuple<ulong, ulong>, List<ulong>> StudentActivityRequestedGroups { get; set; }

        //number of all requests made for specific group 
        public static Dictionary<ulong, int[]> GroupRequests { get; set; } 

        public static List<Request> Requests { get; set; }

        public static Dictionary<ulong, List<ulong>> Overlaps { get; set; }

        public static Dictionary<ulong, int[]> Limits { get; set; }

        public static int Evaluation { get; set; }

        public static ulong NumberOfEvaluations { get; set; }
    }
}
