using UnityEngine;

/// <summary>
/// Trang phục lính thực dân Pháp thời cũ: áo kaki + mũ cối + chiếc hộp đai đỏ.
/// </summary>
public static class FrenchColonialSoldierLook
{
    static Material cachedUniformMaterial;
    static Material cachedHelmetMaterial;
    static Material cachedLeatherMaterial;
    static Material cachedSashMaterial;

    public static void Apply(GameObject model)
    {
        if (model == null) return;

        ApplyUniform(model);
        AttachPithHelmet(model);
        AttachColonialGear(model);
    }

    static void ApplyUniform(GameObject model)
    {
        var uniform = GetUniformMaterial();
        foreach (var smr in model.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (smr == null) continue;
            smr.material = new Material(uniform);
        }
    }

    static void AttachPithHelmet(GameObject model)
    {
        var head = FindHead(model);
        var anchor = head != null ? head : model.transform;

        var helmetRoot = new GameObject("PithHelmet");
        helmetRoot.transform.SetParent(anchor, false);

        if (head != null)
        {
            helmetRoot.transform.localPosition = new Vector3(0f, 0.07f, 0.02f);
            helmetRoot.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);
            helmetRoot.transform.localScale = Vector3.one;
        }
        else
        {
            helmetRoot.transform.localPosition = new Vector3(0f, 1.58f, 0f);
            helmetRoot.transform.localRotation = Quaternion.identity;
            helmetRoot.transform.localScale = Vector3.one;
        }

        var helmetMat = GetHelmetMaterial();

        var brim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        brim.name = "HelmetBrim";
        brim.transform.SetParent(helmetRoot.transform, false);
        brim.transform.localPosition = new Vector3(0f, 0.01f, 0f);
        brim.transform.localScale = new Vector3(0.34f, 0.018f, 0.34f);
        ApplyMaterial(brim, helmetMat);

        var dome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dome.name = "HelmetDome";
        dome.transform.SetParent(helmetRoot.transform, false);
        dome.transform.localPosition = new Vector3(0f, 0.08f, 0f);
        dome.transform.localScale = new Vector3(0.24f, 0.18f, 0.24f);
        ApplyMaterial(dome, helmetMat);

        var band = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        band.name = "HelmetBand";
        band.transform.SetParent(helmetRoot.transform, false);
        band.transform.localPosition = new Vector3(0f, 0.04f, 0f);
        band.transform.localScale = new Vector3(0.26f, 0.012f, 0.26f);
        ApplyMaterial(band, GetLeatherMaterial());
    }

    static void AttachColonialGear(GameObject model)
    {
        var chest = FindChest(model);
        var anchor = chest != null ? chest : model.transform;

        var belt = GameObject.CreatePrimitive(PrimitiveType.Cube);
        belt.name = "ColonialBelt";
        belt.transform.SetParent(anchor, false);
        belt.transform.localPosition = chest != null ? new Vector3(0f, -0.02f, 0.07f) : new Vector3(0f, 0.95f, 0.08f);
        belt.transform.localScale = new Vector3(0.34f, 0.05f, 0.04f);
        ApplyMaterial(belt, GetLeatherMaterial());

        var sash = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sash.name = "ColonialSash";
        sash.transform.SetParent(anchor, false);
        sash.transform.localPosition = chest != null ? new Vector3(0f, 0.1f, 0.075f) : new Vector3(0f, 1.05f, 0.08f);
        sash.transform.localScale = new Vector3(0.12f, 0.22f, 0.025f);
        ApplyMaterial(sash, GetSashMaterial());
    }

    static Transform FindHead(GameObject model)
    {
        var animator = model.GetComponent<Animator>();
        if (animator != null && animator.isHuman)
        {
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            if (head != null) return head;
        }

        return model.transform.Find("Head")
            ?? model.transform.Find("head")
            ?? model.transform.Find("mixamorig:Head");
    }

    static Transform FindChest(GameObject model)
    {
        var animator = model.GetComponent<Animator>();
        if (animator != null && animator.isHuman)
        {
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            if (spine != null) return spine;
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            if (chest != null) return chest;
        }

        return model.transform;
    }

    static void ApplyMaterial(GameObject go, Material mat)
    {
        var col = go.GetComponent<Collider>();
        if (col != null) Object.Destroy(col);

        var renderer = go.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = mat;
    }

    static Material GetUniformMaterial()
    {
        if (cachedUniformMaterial != null)
            return cachedUniformMaterial;

        cachedUniformMaterial = Resources.Load<Material>("Materials/FrenchColonialUniform");
        if (cachedUniformMaterial != null)
            return cachedUniformMaterial;

        cachedUniformMaterial = CreateLitMaterial(
            "FrenchColonialUniform_Runtime",
            new Color(0.72f, 0.6f, 0.38f),
            0.24f,
            0.02f);
        return cachedUniformMaterial;
    }

    static Material GetHelmetMaterial()
    {
        if (cachedHelmetMaterial != null)
            return cachedHelmetMaterial;

        cachedHelmetMaterial = CreateLitMaterial(
            "FrenchPithHelmet_Runtime",
            new Color(0.93f, 0.89f, 0.78f),
            0.12f,
            0f);
        return cachedHelmetMaterial;
    }

    static Material GetLeatherMaterial()
    {
        if (cachedLeatherMaterial != null)
            return cachedLeatherMaterial;

        cachedLeatherMaterial = CreateLitMaterial(
            "FrenchColonialLeather_Runtime",
            new Color(0.28f, 0.18f, 0.1f),
            0.18f,
            0.05f);
        return cachedLeatherMaterial;
    }

    static Material GetSashMaterial()
    {
        if (cachedSashMaterial != null)
            return cachedSashMaterial;

        cachedSashMaterial = CreateLitMaterial(
            "FrenchColonialSash_Runtime",
            new Color(0.72f, 0.12f, 0.1f),
            0.15f,
            0f);
        return cachedSashMaterial;
    }

    static Material CreateLitMaterial(string name, Color color, float smoothness, float metallic)
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var mat = new Material(shader);
        mat.name = name;
        mat.SetColor("_BaseColor", color);
        mat.color = color;
        mat.SetFloat("_Smoothness", smoothness);
        mat.SetFloat("_Metallic", metallic);
        return mat;
    }
}
