using System.Timers;

namespace MemoryCacheT.Ex
{
    internal interface ITimer
    {
        void Start();
        void Stop();
        double Interval { get; set; }
        event ElapsedEventHandler Elapsed;
    }
}