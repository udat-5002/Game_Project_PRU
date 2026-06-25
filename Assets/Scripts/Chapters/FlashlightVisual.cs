using UnityEngine;

public class FlashlightVisual : MonoBehaviour
{
    public float sweepAngle = 40f;
    public float sweepSpeed = 1.8f;
    public float beamLength = 6f;
    public float beamWidth = 2f;

    Transform beamTransform;
    float startYaw;

    void Start()
    {
        startYaw = transform.localEulerAngles.y;
        BuildBeam();
    }

    void BuildBeam()
    {
        var beamGo = new GameObject("BeamVisual");
        beamGo.transform.SetParent(transform, false);
        beamGo.transform.localPosition = new Vector3(0f, 1.2f, 0.5f);
        beamTransform = beamGo.transform;

        var cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cone.name = "Cone";
        cone.transform.SetParent(beamTransform, false);
        cone.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        cone.transform.localPosition = new Vector3(0f, 0f, beamLength * 0.5f);
        cone.transform.localScale = new Vector3(beamWidth, beamLength * 0.5f, beamWidth);

        var col = cone.GetComponent<Collider>();
        if (col != null) Destroy(col);

        var renderer = cone.GetComponent<Renderer>();
        if (renderer != null)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader);
            var color = new Color(1f, 0.92f, 0.45f, 0.15f);
            mat.SetColor("_BaseColor", color);
            mat.color = color;

            // Setup transparency (handles standard & URP shaders fallback)
            mat.SetFloat("_Surface", 1f); 
            mat.SetFloat("_Blend", 0f); 
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
            
            renderer.material = mat;
        }
    }

    void Update()
    {
        if (beamTransform == null) return;

        float angle = Mathf.Sin(Time.time * sweepSpeed) * sweepAngle;
        beamTransform.localRotation = Quaternion.Euler(0f, startYaw + angle, 0f);
    }

    public bool CheckPlayerInBeam(Vector3 playerPos)
    {
        if (beamTransform == null) return false;

        Vector3 origin = beamTransform.position;
        Vector3 dirToPlayer = playerPos - origin;
        dirToPlayer.y = 0f; 

        float dist = dirToPlayer.magnitude;
        if (dist > beamLength) return false;

        float angle = Vector3.Angle(beamTransform.forward, dirToPlayer.normalized);
        // Flashlight beam spread angle is roughly 30 degrees
        return angle <= 30f;
    }
}
