using UnityEngine;

[RequireComponent(typeof(Collider))]
public class QuestZone : MonoBehaviour
{
    public string questStepId;
    public bool oneShot = true;

    bool triggered;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered && oneShot) return;
        if (!other.GetComponent<ThirdPersonController>() && !other.GetComponentInParent<ThirdPersonController>())
            return;
        if (QuestManager.Instance == null || !QuestManager.Instance.IsStepActive(questStepId))
            return;

        QuestManager.Instance.CompleteStep(questStepId);
        triggered = true;
    }
}
