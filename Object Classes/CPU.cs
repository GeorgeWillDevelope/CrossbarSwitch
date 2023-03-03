namespace CrossbarSwitch.Object_Classes
{
    public class CPU
    {
        public int Id;
        public bool Busy;
        public int? MemoryId;

        public CPU(int id, bool busy, int? memory)
        {
            Id = id;
            Busy = busy;
            MemoryId = memory;
        }
    }
}
