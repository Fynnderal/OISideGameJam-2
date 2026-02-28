using System;
using UnityEngine;


/// <summary>
/// Implements countdown and stopwatch timers.
namespace Utilities
{
    public abstract class Timer
    {
        protected float initialTime;
        protected float Time { get; set; }
        public bool isRunning { get; protected set; }


        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };
        public Action OnTimerForcedStop = delegate { };

        protected Timer(float value)
        {
            initialTime = value;
            isRunning = false;
        }

        public void Start()
        {
            Time = initialTime;
            if (!isRunning)
            {
                isRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public virtual void Stop(bool forced = true)
        {
            if (isRunning)
            {
                isRunning = false;
                if (!forced)
                    OnTimerStop.Invoke();
                else
                    OnTimerForcedStop.Invoke();
            }
        }

        public void Resume() => isRunning = true;
        public void Pause() => isRunning = false;
        public abstract void Tick(float deltaTime);
    }

    public class CountdownTimer : Timer
    {
        public float Progress => 1 - Time / initialTime;
        public CountdownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime)
        {
            if (isRunning && Time > 0)
            {
                Time -= deltaTime; 
            }

            if (isRunning && Time <= 0)
            {
                Stop(false);
            }
        }

        public bool isFinished() => Time <= 0;

        public void Reset() => Time = initialTime;

        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset(); 
        }
    }

    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) { }

        public override void Tick(float deltaTime)
        {
            if (isRunning)
            {
                Time += deltaTime; 
            }
        }

        public void Reset() => Time = 0;
        public float GetTime() => Time; 
    }
}
