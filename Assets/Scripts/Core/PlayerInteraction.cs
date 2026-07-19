using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public static bool HasActiveTarget { get; private set; }

    public float interactRange = 5f;
    public float clueInteractRange = 7.5f;
    public LayerMask interactLayers = ~0;

    Interactable currentTarget;
    InputAction interactAction;

    void Start()
    {
        interactAction = new InputAction("Interact", binding: "<Keyboard>/e");
        interactAction.Enable();
    }

    void OnDestroy()
    {
        interactAction?.Disable();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.InputLocked)
        {
            HasActiveTarget = false;
            GameUI.Instance?.SetInteractPrompt(false);
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsShowing)
        {
            HasActiveTarget = false;
            GameUI.Instance?.SetInteractPrompt(false);
            return;
        }

        FindTarget();

        if (currentTarget != null && WasInteractPressed())
        {
            if (currentTarget.GetComponent<Collider>() != null && 
                !currentTarget.name.Contains("Clue") &&
                !currentTarget.name.Contains("TramLienLac") &&
                !currentTarget.name.Contains("FallenLog") &&
                !currentTarget.name.Contains("Khu gỗ đổ"))
            {
                var toPlayer = transform.position - currentTarget.transform.position;
                toPlayer.y = 0;
                if (toPlayer.sqrMagnitude > 0.1f)
                    currentTarget.transform.rotation = Quaternion.LookRotation(toPlayer);
            }

            currentTarget.Interact();
        }
    }

    bool WasInteractPressed()
    {
        if (interactAction != null && interactAction.WasPressedThisFrame())
            return true;
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
    }

    void FindTarget()
    {
        Interactable best = null;
        float bestDist = float.MaxValue;
        float searchRange = Mathf.Max(interactRange, clueInteractRange);

        var hits = Physics.OverlapSphere(transform.position, searchRange, interactLayers, QueryTriggerInteraction.Collide);
        foreach (var col in hits)
        {
            var interactable = col.GetComponent<Interactable>() ?? col.GetComponentInParent<Interactable>();
            if (interactable == null || !interactable.CanInteract()) continue;

            float maxRange = interactable is ClueInteractable ? clueInteractRange : interactRange;
            float dist = HorizontalDistance(transform.position, interactable.transform.position);
            if (dist > maxRange) continue;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = interactable;
            }
        }

        if (best == null)
        {
            foreach (var interactable in FindObjectsByType<Interactable>(FindObjectsSortMode.None))
            {
                if (interactable == null || !interactable.CanInteract()) continue;
                float maxRange = interactable is ClueInteractable ? clueInteractRange : interactRange;
                float dist = HorizontalDistance(transform.position, interactable.transform.position);
                if (dist > maxRange || dist >= bestDist) continue;
                bestDist = dist;
                best = interactable;
            }
        }

        if (currentTarget != best)
        {
            currentTarget = best;
            HasActiveTarget = best != null;
            GameUI.Instance?.SetInteractPrompt(best != null, best != null ? best.PromptText : "");
        }
        else
        {
            HasActiveTarget = currentTarget != null;
        }
    }

    static float HorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
