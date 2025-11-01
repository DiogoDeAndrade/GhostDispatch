using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Splines;

public class GhostQueue : MonoBehaviour
{
    [SerializeField, RequireInterface(typeof(IQueueHandler))]
    private Component _queueHandler;

    public IQueueHandler queueHandler => _queueHandler as IQueueHandler;

    SplineContainer spline;

    void Start()
    {
        spline = GetComponent<SplineContainer>();
    }
}
