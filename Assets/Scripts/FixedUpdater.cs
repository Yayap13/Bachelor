using System;
using UnityEngine;

public class FixedUpdater
{
    private double timeStepMillisecondsDouble;
    private int deltaTime, previousFixedUpdateTime, previousUpdateTime;
    private Action<int> onFixedUpdate;
    private Action<float> onInterpolateBetweenFixedUpdate;

    public FixedUpdater(int currentTime, int timeStepMilliseconds, Action<int> onFixedUpdate, Action<float> onInterpolateBetweenFixedUpdate = null)
    {
        CurrentTime = previousUpdateTime = previousFixedUpdateTime = currentTime;
        TimeStepMilliseconds = timeStepMilliseconds;
        timeStepMillisecondsDouble = TimeStepMilliseconds;
        this.onFixedUpdate = onFixedUpdate;
        this.onInterpolateBetweenFixedUpdate = onInterpolateBetweenFixedUpdate ?? delegate { };
    }

    public int TimeStepMilliseconds { get; }
    public int CurrentTime { get; private set; }

    public bool Update(int now)
    {
        deltaTime = now - previousUpdateTime;
        var ranUpdate = false;

        while (CurrentTime < now)
        {
            onFixedUpdate(CurrentTime);
            previousFixedUpdateTime = CurrentTime;
            CurrentTime += TimeStepMilliseconds;
            ranUpdate = true;
        }

        if (deltaTime < TimeStepMilliseconds)
        {
            var betweenTime = now - previousFixedUpdateTime;
            var interpolate = betweenTime / timeStepMillisecondsDouble;
            onInterpolateBetweenFixedUpdate((float)interpolate);
        }
        else
        {
            onInterpolateBetweenFixedUpdate(1f);
        }

        previousUpdateTime = now;

        return ranUpdate;
    }

    public void SetTime(int now)
    {
        CurrentTime = now;
    }
}