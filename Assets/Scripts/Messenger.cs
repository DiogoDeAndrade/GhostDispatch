using System.Collections;
using UnityEngine;
using UC;

public class Messenger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer3D   demonSR; 
    [SerializeField] private ParticleSystem     hellfirePS;
    [SerializeField] private Light              hellfireLight;

    void Start()
    {
        StartCoroutine(ShowMessageCR());        
    }

    IEnumerator ShowMessageCR()
    {
        hellfireLight.enabled = true;
        float targetIntensity = hellfireLight.intensity;
        hellfireLight.intensity = 0.0f;
        hellfireLight.FadeTo(targetIntensity, 0.1f);

        demonSR.color = Color.white.ChangeAlpha(0.0f);

        hellfirePS.Play();

        yield return new WaitForSeconds(0.2f);

        hellfireLight.FadeTo(0.0f, 0.2f).Done(() => hellfireLight.enabled = false);

        demonSR.FadeTo(Color.white, 0.2f);
    }
}
