using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Simple class using the high percision timer to measure elapsed time
    /// </summary>
    public class HighSpeedTimer
    {
        private double _tickFrequency;
        private double _tickLength;
        private long _timerStartedTick;

        public HighSpeedTimer()
        {
            _tickFrequency = (double)Stopwatch.Frequency;
            _tickLength = 1.0 / _tickFrequency;
            _timerStartedTick = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Resets the timer
        /// </summary>
        public void Reset()
        {
            _timerStartedTick = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Gets the time in seconds since the last time the timer was reset
        /// </summary>
        public double ElapsedTime
        {
            get
            {
                var currentTick = Stopwatch.GetTimestamp();
                var deltaTicks = currentTick - _timerStartedTick;
                return (double)deltaTicks * _tickLength;
            }
        }

        /// <summary>
        /// Gets the time in seconds since the last time the timer was reset, and imeditatly resets the timer
        /// </summary>
        /// <returns></returns>
        public double GetElapsedTimeAndReset()
        {
            var currentTick = Stopwatch.GetTimestamp();
            var deltaTicks = currentTick - _timerStartedTick;
            _timerStartedTick = currentTick;
            return (double)deltaTicks * _tickLength;
        }

        /// <summary>
        /// Gets the smallest time interval in seconds the timer can mesure
        /// </summary>
        public double MaximumPercision
        {
            get
            {
                return _tickLength;
            }
        }

        /// <summary>
        /// Gets the smallest time interval in microseconds that the timer can mesure
        /// </summary>
        public double MaximumPercisionMicroseconds
        {
            get
            {
                return _tickLength * 1000.0 * 1000.0;
            }
        }
    }
}