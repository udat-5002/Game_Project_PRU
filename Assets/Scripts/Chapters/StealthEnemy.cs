using UnityEngine;

public class StealthEnemy : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public Vector3 resetPosition;
    public float moveSpeed = 2f;
    public float detectRadius = 5f;
    public float approachRadius = 10f;
    public float detectSeconds = 1.5f;
    public bool mustHideToPass;
    public string activeQuestId = "stealth_cross";

    Vector3 target;
    float detectTimer;
    bool approachWarned;
    bool dangerWarned;

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

        if (approachRadius <= detectRadius)
            approachRadius = detectRadius * 2f;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.2f)
            target = target == pointB ? pointA : pointB;



        if (HideSpot.PlayerIsHidden)
        {
            detectTimer = 0f;
            return;
        }

        var player = GameManager.Instance?.player;
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= approachRadius && dist > detectRadius)
        {
            if (!approachWarned)
            {
                approachWarned = true;
                GameUI.Instance?.ShowNotification(
                    mustHideToPass
                        ? "⚠ Sắp gặp lính tuần tra! Chuẩn bị núp vào bụi xanh!"
                        : "⚠ Sắp gặp lính tuần tra! Tránh xa!",
                    3f,
                    GameUI.PatrolWarningColor);
            }

            detectTimer = 0f;
            dangerWarned = false;
            return;
        }

        if (dist <= detectRadius)
        {
            if (!dangerWarned)
            {
                dangerWarned = true;
                GameUI.Instance?.ShowNotification(
                    mustHideToPass ? "⚠ Lính tuần tra! Núp vào bụi xanh!" : "⚠ Có lính tuần tra! Tránh xa!",
                    2.5f,
                    GameUI.PatrolWarningColor);
            }

            detectTimer += Time.deltaTime;
            if (detectTimer >= detectSeconds)
            {
                detectTimer = 0f;
                dangerWarned = false;
                GameUI.Instance?.ShowNotification("Bị phát hiện! Rút lui và thử lại.", 3f, GameUI.PatrolWarningColor);
                GameManager.Instance?.TeleportPlayer(resetPosition);
            }
        }
        else
        {
            ResetWarnings();
        }
    }

    void ResetWarnings()
    {
        detectTimer = 0f;
        approachWarned = false;
        dangerWarned = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.55f, 0.1f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, approachRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
