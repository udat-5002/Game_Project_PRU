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
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.2f)
            target = target == pointB ? pointA : pointB;

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

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= detectRadius)
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
