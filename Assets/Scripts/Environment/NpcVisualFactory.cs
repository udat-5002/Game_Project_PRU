using UnityEngine;

/// <summary>
/// Gắn model ChienSi1 lên NPC / lính tuần tra thay cho cột marker.
/// </summary>
public static class NpcVisualFactory
{
    public enum NpcRole
    {
        Civilian,
        Soldier,
        Enemy
    }

    const float FallbackScale = 3f;
    const float FallbackFootOffset = -1.5f;
    const float FallbackModelYaw = 90f;

    static GameObject cachedModelTemplate;
    static RuntimeAnimatorController cachedAnimController;
    static Transform cachedPlayerModelTransform;

    public static Transform Attach(Transform npcRoot, NpcRole role, string overrideModelPath = null)
    {
        if (npcRoot == null) return null;

        var template = GetModelTemplate(overrideModelPath);
        if (template == null)
        {
            Debug.LogWarning("[NpcVisualFactory] Không tìm thấy model ChienSi1 — giữ marker mặc định.");
            return null;
        }

        var model = Object.Instantiate(template, npcRoot);
        model.name = "NpcModel";
        ApplyPlayerModelTransform(model.transform);

        StripGameplayComponents(model);
        SetupAnimator(model);
        ApplyRoleAppearance(model, role);

        return model.transform;
    }

    static void ApplyRoleAppearance(GameObject model, NpcRole role)
    {
        if (role == NpcRole.Enemy)
        {
            FrenchColonialSoldierLook.Apply(model);
            return;
        }

        CharacterMaterialFixer.ApplyTo(model.transform);
        ApplyRoleTint(model, role);
    }

    static void ApplyPlayerModelTransform(Transform modelTransform)
    {
        var reference = GetPlayerModelTransform();
        if (reference != null)
        {
            modelTransform.localScale = reference.localScale;
            modelTransform.localPosition = reference.localPosition;
            modelTransform.localRotation = reference.localRotation;
            return;
        }

        modelTransform.localScale = Vector3.one * FallbackScale;
        modelTransform.localPosition = new Vector3(0f, FallbackFootOffset, 0f);
        modelTransform.localRotation = Quaternion.Euler(0f, FallbackModelYaw, 0f);
    }

    static Transform GetPlayerModelTransform()
    {
        if (cachedPlayerModelTransform != null)
            return cachedPlayerModelTransform;

        var player = Object.FindFirstObjectByType<ThirdPersonController>();
        if (player == null)
            return null;

        foreach (Transform child in player.transform)
        {
            if (child.GetComponentInChildren<SkinnedMeshRenderer>(true) == null)
                continue;

            cachedPlayerModelTransform = child;
            return cachedPlayerModelTransform;
        }

        return null;
    }

    static GameObject GetModelTemplate(string overrideModelPath)
    {
        if (!string.IsNullOrEmpty(overrideModelPath))
        {
            var overrideTemplate = Resources.Load<GameObject>(overrideModelPath);
            if (overrideTemplate != null) return overrideTemplate;
        }
        if (cachedModelTemplate != null)
            return cachedModelTemplate;

        var playerModel = GetPlayerModelTransform();
        if (playerModel != null)
        {
            cachedModelTemplate = playerModel.gameObject;
            return cachedModelTemplate;
        }

        cachedModelTemplate = Resources.Load<GameObject>("NhanVat/ChienSi1");
        return cachedModelTemplate;
    }

    static RuntimeAnimatorController GetAnimController()
    {
        if (cachedAnimController != null)
            return cachedAnimController;

        cachedAnimController = Resources.Load<RuntimeAnimatorController>("Animation/PlayerAnimController");
        if (cachedAnimController != null)
            return cachedAnimController;

        var player = Object.FindFirstObjectByType<ThirdPersonController>();
        if (player != null && player.animator != null)
            cachedAnimController = player.animator.runtimeAnimatorController;

        return cachedAnimController;
    }

    static void SetupAnimator(GameObject model)
    {
        var controller = GetAnimController();
        if (controller == null) return;

        var animator = model.GetComponent<Animator>();
        if (animator == null)
            animator = model.AddComponent<Animator>();

        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
        animator.SetFloat("Speed", 0f);
        animator.SetBool("Grounded", true);
    }

    static void StripGameplayComponents(GameObject model)
    {
        foreach (var cc in model.GetComponentsInChildren<CharacterController>(true))
            Object.Destroy(cc);
        foreach (var controller in model.GetComponentsInChildren<ThirdPersonController>(true))
            Object.Destroy(controller);
        foreach (var interaction in model.GetComponentsInChildren<PlayerInteraction>(true))
            Object.Destroy(interaction);
        foreach (var inventory in model.GetComponentsInChildren<MailInventory>(true))
            Object.Destroy(inventory);
    }

    static void ApplyRoleTint(GameObject model, NpcRole role)
    {
        Color tint = role switch
        {
            NpcRole.Soldier => new Color(0.82f, 0.88f, 1f),
            _ => Color.white
        };

        if (tint == Color.white) return;

        foreach (var smr in model.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (smr == null || smr.material == null) continue;
            smr.material.color = tint;
        }
    }
}
