
using UC;
using UnityEngine;

public class Pentagram : MonoBehaviour
{
    [SerializeField] private float radius = 1.0f;
    [SerializeField] private float maxSpeed = 1.0f;
    [SerializeField] private Light bloodLight;
    [SerializeField] private float intensitySpeed = 5.0f;

    int                 nPoint = 0;
    TrailRenderer       trailRenderer;
    ParticleSystem      ps;
    Vector3             centerPos;
    float               targetLightIntensity;

    private void OnEnable()
    {
        centerPos = transform.position;
        nPoint = 0;
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer) trailRenderer.emitting = false;
        ps = GetComponent<ParticleSystem>();
        if (ps) ps.SetEmission(false);
        transform.position = GetPoint(nPoint);
        nPoint = 1;
        targetLightIntensity = bloodLight.intensity;
        bloodLight.intensity = 0.0f;
        bloodLight.enabled = true;
    }

    Vector3 GetPoint(int index)
    {
        float angle = Mathf.Deg2Rad * (360.0f / 5.0f) * ((index * 2) % 5);
        return new Vector3(centerPos.x + radius * Mathf.Cos(angle), centerPos.y + radius * Mathf.Sin(angle), centerPos.z);
    }

    void Update()
    {
        if (nPoint >= 6)
        {
            bloodLight.intensity = Mathf.MoveTowards(bloodLight.intensity, 0, Time.deltaTime * intensitySpeed);
            return;
        }

        bloodLight.intensity = Mathf.MoveTowards(bloodLight.intensity, targetLightIntensity, Time.deltaTime * intensitySpeed);

        if (trailRenderer) trailRenderer.enabled = true;
        if (ps) ps.SetEmission(true);

        Vector3 targetPoint = GetPoint(nPoint);
        float   dist = Vector3.Distance(transform.position, targetPoint);
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, maxSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPoint) < 1e-3)
        {
            nPoint++;
        }
    }
}
