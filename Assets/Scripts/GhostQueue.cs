using System.Collections.Generic;
using UC;
using UnityEngine;
using UnityEngine.Splines;

public class GhostQueue : MonoBehaviour
{
    [SerializeField, RequireInterface(typeof(IQueueHandler))]
    private Component _queueHandler;

    public IQueueHandler queueHandler => _queueHandler as IQueueHandler;

    SplineContainer spline;
    List<Ghost>     ghosts;
    List<Ghost>     toRemove;

    void Start()
    {
        spline = GetComponent<SplineContainer>();
        ghosts = new();
        toRemove = new();
    }

    public void Add(Ghost ghost)
    {
        ghosts.Add(ghost);
    }

    private void Update()
    {
        // Move ghost on queue to position

        float currentT = 1.0f;
        float incT = Mathf.Min(1.0f / ghosts.Count, 0.1f);
        foreach (var ghost in ghosts)
        {
            Vector3 targetPosition = GetPosition(currentT);
            float dist = Vector3.Distance(ghost.transform.position.xz(), targetPosition.xz());
            if (dist > 1e-3)
            {
                Vector2 newPos2d = Vector2.MoveTowards(ghost.transform.position.xz(), targetPosition.xz(), Time.deltaTime * LevelManager.ghostMoveSpeed);
                ghost.transform.position = new Vector3(newPos2d.x, ghost.transform.position.y, newPos2d.y);
            }
            else
            {
                if (currentT == 1.0f)
                {
                    // First in the queue, check if we can move on to the next queue
                    if (queueHandler != null)
                    {
                        if (queueHandler.IsGoal())
                        {
                            toRemove.Add(ghost);
                            queueHandler.ReachGoal(ghost);
                        }
                        else
                        {
                            var nextQueue = queueHandler.GetNextQueue();
                            if (nextQueue != null)
                            {
                                toRemove.Add(ghost);
                                nextQueue.Add(ghost);
                            }
                        }
                    }
                }
            }

            currentT -= incT;
        }

        foreach (var ghost in toRemove) ghosts.Remove(ghost);
        toRemove.Clear();
    }

    Vector3 GetPosition(float t)
    {
        if (spline.Evaluate(t, out var position, out var tangenty, out var up))
        {
            return position;
        }
        return Vector3.zero;
    }
}
