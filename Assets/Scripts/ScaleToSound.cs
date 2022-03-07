using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToSound : MonoBehaviour
{
    public Vector3 direction = new Vector3(0, 1f, 0);

    public float strength = 100f;

    public float roughness = 10f;

    Vector3 startScale;

    void Start()
    {
        direction.Normalize();
        startScale = transform.localScale;
    }

    void Update()
    {
        Vector3 scale = transform.localScale;

        scale = new Vector3(
            (startScale.x + direction.x * AudioData.s.spectrumAvg * strength),
            (startScale.y + direction.y * AudioData.s.spectrumAvg * strength),
            (startScale.z + direction.z * AudioData.s.spectrumAvg * strength)
        );

        transform.localScale = Vector3.Lerp(transform.localScale, scale, Time.deltaTime * roughness);
    }
}
