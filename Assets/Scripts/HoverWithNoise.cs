using UnityEngine;

public class HoverWithNoise : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3   direction = Vector3.up;
    [SerializeField] private float     offset;
    [SerializeField] private float     amplitude;
    [SerializeField] private float     frequency;
    [SerializeField] private float     baseOffset = 0.0f;

    Vector3 basePos;
    float   elapsedTime;
    Vector3 maxNoise = Vector3.zero;

    void Start()
    {
        if (target == null) target = this.transform;

        basePos = target.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        var noise = new Vector3(Random.Range(-maxNoise.x, maxNoise.x), Random.Range(-maxNoise.y, maxNoise.y), Random.Range(-maxNoise.z, maxNoise.z));
        target.localPosition =  basePos + direction * (offset + Mathf.Sin(elapsedTime * frequency * Mathf.Deg2Rad + baseOffset) * amplitude) + noise;
    }

    public void SetMaxNoise(Vector3 noise)
    {
        maxNoise = noise;
    }
}
