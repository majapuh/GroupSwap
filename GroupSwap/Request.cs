using System;

namespace GroupSwap
{
    public class Request
    {
        private ulong studentId;
        private ulong activityId;
        private ulong requestedGroupId;

        public ulong StudentId
        {
            get { return studentId; }
        }

        public ulong ActivityId
        {
            get { return activityId; }
        }

        public ulong RequestedGroupId
        {
            get { return requestedGroupId; }
            set { requestedGroupId = value; }
        }

        public Request(ulong _studentId, ulong _activityId, ulong _requestedGroupId)
        {
            this.studentId = _studentId;
            this.activityId = _activityId;
            this.requestedGroupId = _requestedGroupId;
        }
    }
}
