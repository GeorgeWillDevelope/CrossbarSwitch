using System.Collections.Generic;

namespace CrossbarSwitch.Object_Classes
{
    public class Memory
    {
        public int Id;
        public int? TactsLeftToRelease;
        public bool Busy;
        public List<int> WaitingProcessors;

        public Memory(int id, int? tactOfRelease, bool busy, List<int> waitingProcessors)
        {
            Id = id;
            TactsLeftToRelease = tactOfRelease;
            Busy = busy;
            WaitingProcessors = waitingProcessors;
        }
    }
}
