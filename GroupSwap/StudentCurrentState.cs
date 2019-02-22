using System;

namespace GroupSwap
{
    public class StudentCurrentState
    {
        private ulong studentId;
        private ulong activityId;
        private int swapWeight;
        private ulong currentGroupId;
        private ulong newGroupId;
        private int swapCount;

        public ulong StudentId
        {
            get { return studentId; }
        }

        public ulong ActivityId
        {
            get { return activityId; }
        }

        public int SwapWeight
        {
            get { return swapWeight; }
        }

        public ulong CurrentGroupId
        {
            get { return currentGroupId; }
        }

        public ulong NewGroupId
        {
            get { return newGroupId; }
            set { newGroupId = value; }
        }

        public int SwapCnt
        {
            get { return swapCount; }
            set { swapCount = value; }
        }

        public StudentCurrentState(ulong _studentId, ulong _activityId, int _swapWeight, ulong _currentGroupId, ulong _newGroupId, int _swapCnt = 0)
        {
            this.studentId = _studentId;
            this.activityId = _activityId;
            this.swapWeight = _swapWeight;
            this.currentGroupId = _currentGroupId;
            this.newGroupId = _newGroupId;
            this.swapCount = _swapCnt;
        }

    }
}
