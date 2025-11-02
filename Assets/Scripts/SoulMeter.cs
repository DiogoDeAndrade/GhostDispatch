using UnityEngine;

public class SoulMeter : MonoBehaviour
{
    [SerializeField] private Vector2        heightRange;
    [SerializeField] private RectTransform  meter;

    void Update()
    {
        float p = LevelManager.soulPercentage;
        float h = Mathf.Lerp(heightRange.x, heightRange.y, p);

        meter.sizeDelta = new Vector2(meter.sizeDelta.x, Mathf.MoveTowards(meter.sizeDelta.y, h, 20.0f * Time.deltaTime));
        if ((p == 0.0f) && (Mathf.Abs(meter.sizeDelta.y - h) < 1.0f))
        {
            meter.gameObject.SetActive(false);
        }
        else
        {
            meter.gameObject.SetActive(true);
        }
    }
}
