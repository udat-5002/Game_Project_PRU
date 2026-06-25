using UnityEngine;

/// <summary>
/// Xoay model lính tuần tra theo hướng đi và phát animation đi bộ.
/// </summary>
public class NpcPatrolAnimator : MonoBehaviour
{
    public Transform modelRoot;

    Vector3 lastPosition;

    void Start() => lastPosition = transform.position;

    void LateUpdate()
    {
        if (modelRoot == null) return;

        var delta = transform.position - lastPosition;
        lastPosition = transform.position;

        var flat = new Vector3(delta.x, 0f, delta.z);
        if (flat.sqrMagnitude > 0.0004f)
            modelRoot.rotation = Quaternion.LookRotation(flat.normalized, Vector3.up);

        var animator = modelRoot.GetComponent<Animator>();
        if (animator == null) return;

        float speed = flat.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        animator.SetFloat("Speed", speed);
        animator.SetBool("Grounded", true);
    }
}
