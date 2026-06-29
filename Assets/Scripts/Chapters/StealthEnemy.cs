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
    bool patrolActive;
    int stagingIndex;
    Vector3 routePointA;
    Vector3 routePointB;
    
    CharacterController controller;
    float stuckTimer;
    LineRenderer detectRing;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.radius = 0.4f;
            controller.height = 1.8f;
            controller.center = new Vector3(0, 0.9f, 0);
        }

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

        SetupDetectRing();
    }

    public void SetRoute(Vector3 a, Vector3 b)
    {
        routePointA = a;
        routePointB = b;
        pointA = a;
        pointB = b;
        target = b;
    }

    public void SetPatrolActive(bool active)
    {
        patrolActive = active;
        if (detectRing != null)
            detectRing.enabled = active;
    }

    public void SetStagingIndex(int index) => stagingIndex = index;

    public void ActivatePatrolRoute()
    {
        TeleportTo(GroundSnap.SnapCharacter(routePointA));
        pointA = routePointA;
        pointB = routePointB;
        target = pointB;
        SetPatrolActive(true);
    }

    public void StandDownAtRoute()
    {
        if (routePointA.sqrMagnitude > 1f)
            TeleportTo(GroundSnap.SnapCharacter(routePointA));
        pointA = routePointA;
        pointB = routePointB;
        target = pointB;
        SetPatrolActive(false);
    }

    public void MoveToStaging()
    {
        var staging = stagingIndex switch
        {
            0 => new Vector3(-46f, 0f, -62f),
            1 => new Vector3(46f, 0f, -62f),
            2 => new Vector3(-46f, 0f, 8f),
            _ => new Vector3(46f, 0f, 8f)
        };
        TeleportTo(GroundSnap.SnapCharacter(staging));
        SetPatrolActive(false);
    }

    public void MoveAwayFrom(Vector3 avoid, float minDistance)
    {
        var flat = transform.position - avoid;
        flat.y = 0f;
        if (flat.sqrMagnitude < minDistance * minDistance)
        {
            var dir = flat.sqrMagnitude > 0.01f ? flat.normalized : Vector3.right;
            TeleportTo(GroundSnap.SnapCharacter(avoid + dir * minDistance));
        }
    }

    void TeleportTo(Vector3 pos)
    {
        if (controller != null)
            controller.enabled = false;
        transform.position = pos;
        pointA = pos;
        pointB = pos + transform.forward * 6f;
        target = pointB;
        if (controller != null)
            controller.enabled = true;
    }

    void SetupDetectRing()
    {
        var ringObj = new GameObject("DetectRing");
        ringObj.transform.SetParent(transform, false);
        detectRing = ringObj.AddComponent<LineRenderer>();
        detectRing.useWorldSpace = true;
        detectRing.loop = true;
        detectRing.positionCount = 36;
        detectRing.startWidth = 0.1f;
        detectRing.endWidth = 0.1f;
        
        var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
        var mat = new Material(shader);
        mat.color = new Color(1f, 0.2f, 0.1f, 0.6f);
        detectRing.material = mat;
    }

    void Update()
    {
        UpdateDetectRing();

        var dir = target - transform.position;
        dir.y = 0;

        Vector3 moveDir = dir.normalized * moveSpeed;
        if (!controller.isGrounded)
            moveDir.y = Physics.gravity.y * 2f;
        else
            moveDir.y = -2f;

        var flags = controller.Move(moveDir * Time.deltaTime);

        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 360f * Time.deltaTime);

        stuckTimer += Time.deltaTime;

        if (dir.magnitude < 0.3f || stuckTimer > 8f || (flags & CollisionFlags.Sides) != 0)
        {
            stuckTimer = 0f;
            PickNewTarget();
        }

        if (!patrolActive)
        {
            detectTimer = 0f;
            approachWarned = false;
            dangerWarned = false;
            return;
        }

        var player = GameManager.Instance?.player;
        if (player == null) return;

        if (HideSpot.PlayerIsHidden)
        {
            detectTimer = 0f;
            return;
        }

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

    void PickNewTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            var offset = Random.insideUnitSphere * 20f;
            offset.y = 0;
            var potential = pointA + offset; // Tuần tra quanh vị trí ban đầu của lính, không phải quanh điểm hồi sinh của player!
            
            if (GroundSnap.TryGetGroundY(potential, out float y))
            {
                potential.y = y;
                target = potential;
                break;
            }
        }
    }

    void UpdateDetectRing()
    {
        if (detectRing == null) return;
        
        int segments = detectRing.positionCount;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle) * detectRadius, 0, Mathf.Sin(angle) * detectRadius);
            
            if (GroundSnap.TryGetGroundY(pos, out float y))
                pos.y = y + 0.15f; // Nâng lên một chút so với mặt đất
            else
                pos.y = transform.position.y + 0.15f;
                
            detectRing.SetPosition(i, pos);
        }
    }
}
