using System.Collections.Generic;

namespace CrossbarSwitch.Object_Classes
{
    public class Requests
    {
        public int Tact;
        public List<Memory> Memories;
        public List<CPU> CPUs;

        public Requests()
        {

        }
        public Requests(List<CPU> cpus, List<Memory> memories)
        {
            CPUs = cpus;
            Memories = memories;
        }
    }
}
