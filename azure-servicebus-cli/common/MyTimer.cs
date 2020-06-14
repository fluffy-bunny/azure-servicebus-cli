using System.Diagnostics;

namespace Common
{
    public class MyTimer
    {
        private Stopwatch _watch;

        public void Start()
        {
            _watch = Stopwatch.StartNew();
        }

        public long Stop()
        {
            _watch.Stop();
            var elapsedMs = _watch.ElapsedMilliseconds;
            return elapsedMs;
        }
    }
}
