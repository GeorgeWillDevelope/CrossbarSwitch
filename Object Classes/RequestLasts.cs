namespace CrossbarSwitch.Object_Classes
{
    public class RequestLasts
    {
        public int Id;
        public int TactsLast;
        public int CPUId;
        public bool Done;

        public RequestLasts()
        {
        }

        public RequestLasts(int id, int tactsLast, int cPUId)
        {
            Id = id;
            TactsLast = tactsLast;
            CPUId = cPUId;
        }

        public RequestLasts(int id, int tactsLast, int cPUId, bool done) : this(id, tactsLast, cPUId)
        {
            this.Done = done;
        }
    }
}
