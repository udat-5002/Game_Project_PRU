using UnityEngine;

public class WeatherAudio : MonoBehaviour
{
    public float windVolume = 0.4f;
    public float rainVolume = 0.5f;

    AudioSource windSource;
    AudioSource rainSource;

    void Start()
    {
        windSource = gameObject.AddComponent<AudioSource>();
        windSource.loop = true;
        windSource.volume = 0f;
        windSource.spatialBlend = 0f;
        windSource.clip = CreateNoiseClip("Wind", 0.35f, 0.97f);
        windSource.Play();

        var rainGo = new GameObject("RainAudio");
        rainGo.transform.SetParent(transform);
        rainSource = rainGo.AddComponent<AudioSource>();
        rainSource.loop = true;
        rainSource.volume = 0f;
        rainSource.spatialBlend = 0f;
        rainSource.clip = CreateNoiseClip("Rain", 0.12f, 0f);
        rainSource.Play();
    }

    void Update()
    {
        if (WeatherController.Instance == null) return;

        float wind = WeatherController.Instance.NormalizedWind;
        float rain = WeatherController.Instance.NormalizedRain;
        float sfx = AudioSettings.SfxScaled;

        if (windSource != null)
            windSource.volume = Mathf.Lerp(windSource.volume, wind * windVolume * sfx, Time.deltaTime * 2f);
        if (rainSource != null)
            rainSource.volume = Mathf.Lerp(rainSource.volume, rain * rainVolume * sfx, Time.deltaTime * 2f);
    }

    AudioClip CreateNoiseClip(string name, float amplitude, float smooth)
    {
        int sampleRate = 44100;
        int length = sampleRate * 3;
        var samples = new float[length];
        float last = 0f;
        for (int i = 0; i < length; i++)
        {
            float white = Random.Range(-1f, 1f);
            if (smooth > 0f)
                last = last * smooth + white * (1f - smooth);
            else
                last = white;
            samples[i] = last * amplitude;
        }
        var clip = AudioClip.Create(name, length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
