using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData : MonoBehaviour
{
    // Singleton is very useful for Manager objects
    private static AudioData singleton;
    public static AudioData s { get { return singleton; } }
    protected void Awake()
    {
        //On Awake this manager object references itself.
        //To reference this script simply type AudioData.s.yourVariable
        singleton = this;
    }

    // the number we get when we average out the spectrum of the song at any given time
    // we use this number to animate the scene
    public float spectrumAvg = 0f;
    public float bassAvg = 0f;
    public float midAvg = 0f;
    public float trebleAvg = 0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float[] spectrum = new float[256];

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        spectrumAvg = 0f;
        bassAvg = 0f;
        midAvg = 0f;
        trebleAvg = 0f;

        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            spectrumAvg += spectrum[i];

            if (i < 3)
            {
                bassAvg += spectrum[i];
            }
            if (i > 3 && i < 6)
            {
                midAvg += spectrum[i];
            }
            if (i > 6 && i < 9)
            {
                trebleAvg += spectrum[i];
            }
        }

        spectrumAvg /= 256f;
        bassAvg /= 3f;
        midAvg /= 3f;
        trebleAvg /= 3f;

        Debug.Log("spectrum : " + spectrumAvg + " - bass : " + bassAvg + " - mid : " + midAvg + " - treble : " + trebleAvg);
    }
}
