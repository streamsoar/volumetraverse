using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorToSound : MonoBehaviour
{
    public Vector3 rgb = new Vector3(0, 1f, 0);

    public float strength = 100f;

    public float roughness = 10f;

    Color startColor;

    public bool colorSpectrum = true;

    Material mat;

    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
        startColor = mat.color;
        rgb.Normalize();
    }

    void Update()
    {
        if (colorSpectrum)
        {
            rgb.x = AudioData.s.bassAvg * 10f;
            rgb.y = AudioData.s.midAvg * 10f;
            rgb.z = AudioData.s.trebleAvg * 10f;
        }
        rgb.Normalize();

        Color col = mat.color;

        col = new Color(
            (startColor.r + rgb.x * AudioData.s.spectrumAvg * strength),
            (startColor.g + rgb.y * AudioData.s.spectrumAvg * strength),
            (startColor.b + rgb.z * AudioData.s.spectrumAvg * strength)
        );

        col = Color.Lerp(mat.color, col, Time.deltaTime * roughness);

        mat.color = col;
    }
}
