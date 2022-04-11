using System;

namespace Crawler.Browser.Common.AccountManagment
{
    public class Interval
    {
        private const int SecondsPerDay = 60 * 60 * 24;

        public Interval(TimeSpan start, TimeSpan end)
        {
            Start = Convert.ToInt32(start.TotalSeconds);
            End = Convert.ToInt32(end.TotalSeconds);
        }

        public Interval(int start, int end)
        {
            Start = start;
            End = end;
        }

        #region Start
        private int _start;
        /// <summary>
        /// Seconds 
        /// <remarks> range [0 - <see cref="SecondsPerDay"/>]</remarks>
        /// </summary>
        public int Start
        {
            get { return _start; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("value < 0");
                if (value > SecondsPerDay)
                    throw new ArgumentException($"value > {SecondsPerDay}");

                _start = value;
            }
        }
        #endregion Start

        #region End
        private int _end;
        /// <summary>
        /// Seconds
        /// </summary>
        public int End
        {
            get { return _end; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("value < 0");
                if (value > SecondsPerDay)
                    throw new ArgumentException($"value > {SecondsPerDay}");

                _end = value;
            }
        }
        #endregion End

        public bool Contains(TimeSpan value)
        {
            return Contains(Convert.ToInt32(value.TotalSeconds));
        }

        private bool Contains(int totalSeconds)
        {
            return totalSeconds >= Start && totalSeconds <= End;
        }

        public bool IsEmpty()
        {
            return Start == End;
        }

        public static Interval Always => new Interval(0, SecondsPerDay);

        public override string ToString()
        {
            return $"Start: {TimeSpan.FromSeconds(Start)}, End: {TimeSpan.FromSeconds(End)}";
        }
    }
}