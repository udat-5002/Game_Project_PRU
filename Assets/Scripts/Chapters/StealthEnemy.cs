using UnityEngine;

public class StealthEnemy : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public Vector3 resetPosition;
    public float moveSpeed = 2f;
    public float detectRadius = 5f;
    public float detectSeconds = 1.5f;
    public bool mustHideToPass;
    public string activeQuestId = "stealth_cross";

    Vector3 target;
    float detectTimer;
    bool warned;
    FlashlightVisual flashlight;

    void Start()
    {
        target = pointB;
        if (pointA == Vector3.zero && pointB == Vector3.zero)
        {
            pointA = transform.position;
            pointB = transform.position + Vector3.forward * 5f;
            target = pointB;
        }

        if (resetPosition == Vector3.zero)
            resetPosition = pointA + Vector3.back * 4f;

        // Auto-add flashlight visual representation if must hide to pass (stealth difficulty)
        flashlight = GetComponent<FlashlightVisual>();
        if (flashlight == null && mustHideToPass)
        {
            flashlight = gameObject.AddComponent<FlashlightVisual>();
            flashlight.beamLength = detectRadius;
            flashlight.beamWidth = detectRadius * 0.4f;
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.2f)
        {
            target = target == pointB ? pointA : pointB;
            // Face the walk target
            Vector3 direction = (target - transform.position).normalized;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        if (QuestManager.Instance == null || !QuestManager.Instance.IsStepActive(activeQuestId))
        {
            detectTimer = 0f;
            warned = false;
            return;
        }

        if (mustHideToPass && HideSpot.PlayerIsHidden)
        {
            detectTimer = 0f;
            return;
        }

        var player = GameManager.Instance?.player;
        if (player == null) return;

        bool isDetected = false;
        if (flashlight != null)
        {
            isDetected = flashlight.CheckPlayerInBeam(player.position);
        }
        else
        {
            isDetected = Vector3.Distance(transform.position, player.position) <= detectRadius;
        }

        if (isDetected)
        {
            if (!warned)
            {
                warned = true;
                GameUI.Instance?.ShowNotification(
                    mustHideToPass ? "⚠ Lính tuần tra! Núp vào bụi xanh!" : "⚠ Có lính tuần tra! Tránh xa!",
                    2f);
            }

            detectTimer += Time.deltaTime;
            if (detectTimer >= detectSeconds)
            {
                detectTimer = 0f;
                warned = false;
                GameUI.Instance?.ShowNotification("Bị phát hiện! Rút lui và thử lại.", 3f);
                GameManager.Instance?.TeleportPlayer(resetPosition);
            }
        }
        else
        {
            detectTimer = 0f;
            warned = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}

