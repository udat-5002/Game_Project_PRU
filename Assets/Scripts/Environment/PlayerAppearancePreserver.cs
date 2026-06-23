using UnityEngine;

/// <summary>
/// Đèn fill nhẹ khi trời tối — KHÔNG sửa material (tránh nhân vật xanh lè).
/// </summary>
public class PlayerAppearancePreserver : MonoBehaviour
{
    Light fillLight;

    void Start()
    {
        SetupFillLight();
    }

    void SetupFillLight()
    {
        var go = new GameObject("PlayerFillLight");
        go.transform.SetParent(transform);
        go.transform.localPosition = new Vector3(0f, 1.6f, 0.3f);

        fillLight = go.AddComponent<Light>();
        fillLight.type = LightType.Point;
        fillLight.range = 4f;
        fillLight.shadows = LightShadows.None;
        fillLight.color = new Color(1f, 0.96f, 0.9f);
        fillLight.intensity = 0f;
    }

    void LateUpdate()
    {
        if (fillLight == null) return;
        float dark = GetWeatherDarkFactor();
        fillLight.intensity = Mathf.Lerp(0f, 0.55f, dark);
    }

    float GetWeatherDarkFactor()
    {
        if (WeatherController.Instance == null) return 0f;
        if (WeatherController.Instance.IsStormActive) return 1f;

        return WeatherController.Instance.CurrentPreset switch
        {
            WeatherController.WeatherPreset.DarkForest => 0.7f,
            WeatherController.WeatherPreset.Storm => 1f,
            WeatherController.WeatherPreset.Battlefield => 0.5f,
            WeatherController.WeatherPreset.Overcast => 0.3f,
            _ => 0f
        };
    }
}
