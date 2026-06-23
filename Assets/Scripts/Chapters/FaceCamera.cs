using UnityEngine;

/// <summary>Giữ biển báo 3D luôn quay về camera.</summary>
public class FaceCamera : MonoBehaviour
{
    void LateUpdate()
    {
        var cam = Camera.main;
        if (cam == null) return;
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }
}
