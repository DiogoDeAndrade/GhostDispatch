using UnityEngine;

public interface IQueueHandler 
{
    GhostQueue GetNextQueue();

    bool IsGoal() => false;
    void ReachGoal(Ghost ghost) { }
}

