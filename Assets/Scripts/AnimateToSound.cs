using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateToSound : MonoBehaviour
{
    public Vector3 direction = new Vector3(0, 1f, 0);

    public float strength = 100f;

    public float roughness = 10f;

    Vector3 startPos;

    void Start()
    {
        direction.Normalize();
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        pos = new Vector3(
            (startPos.x + direction.x * AudioData.s.spectrumAvg * strength),
            (startPos.y + direction.y * AudioData.s.spectrumAvg * strength),
            (startPos.z + direction.z * AudioData.s.spectrumAvg * strength)
        );
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * roughness);
    }
}
