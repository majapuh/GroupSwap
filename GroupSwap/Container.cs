using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupSwap
{
    public static class Container
    {
        public static List<StudentCurrentState> StudentStates { get; set; }

        public static List<StudentCurrentState> NewStudentStates { get; set; }

        //sve trenutne grupe u kojima se nalazi student (na svim aktivnostima) - za provjeru preklapanja
        public static Dictionary<ulong, List<ulong>> StudentGroups { get; set; }

        //broj ukupnih trazenih zamjena i broj izvrsenih (prvi param trazene, drugi izvrsene)
        public static Dictionary<ulong, int[]> StudentSwaps { get; set; }

        //trenutna grupa studenta na odredjenoj aktivnosti
        public static Dictionary<Tuple<ulong, ulong>, ulong> StudentActivityGroups { get; set; }

        //zeljene grupe studenta na odr aktivnosti
        public static Dictionary<Tuple<ulong, ulong>, List<ulong>> StudentActivityRequestedGroups { get; set; }

        //broj requestova za ulazak u odredjenu grupu i oznaka za min_max pref
        public static Dictionary<ulong, int[]> GroupRequests { get; set; } 

        public static List<Request> Requests { get; set; }

        public static Dictionary<ulong, List<ulong>> Overlaps { get; set; }

        public static Dictionary<ulong, int[]> Limits { get; set; }

        public static int Evaluation { get; set; }

        public static ulong NumberOfEvaluations { get; set; }
    }
}
