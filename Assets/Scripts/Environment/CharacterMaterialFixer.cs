using UnityEngine;

/// <summary>
/// Gán material URP đúng cho nhân vật — tránh bị xanh/hồng do shader lỗi.
/// </summary>
[DefaultExecutionOrder(-100)]
public class CharacterMaterialFixer : MonoBehaviour
{
    static Material sharedCharacterMaterial;

    void Awake()
    {
        ApplyCharacterMaterial();
    }

    public static void ApplyTo(Transform root)
    {
        if (root == null) return;
        var fixer = root.GetComponent<CharacterMaterialFixer>();
        if (fixer == null)
            fixer = root.gameObject.AddComponent<CharacterMaterialFixer>();
        fixer.ApplyCharacterMaterial();
    }

    void ApplyCharacterMaterial()
    {
        var mat = GetCharacterMaterial();
        if (mat == null) return;

        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (smr == null) continue;
            smr.material = new Material(mat);
        }
    }

    static Material GetCharacterMaterial()
    {
        if (sharedCharacterMaterial != null)
            return sharedCharacterMaterial;

        var loaded = Resources.Load<Material>("Materials/ChienSi1");
        if (loaded != null)
        {
            sharedCharacterMaterial = loaded;
            return sharedCharacterMaterial;
        }

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) return null;

        sharedCharacterMaterial = new Material(shader);
        sharedCharacterMaterial.name = "ChienSi1_Runtime";
        sharedCharacterMaterial.SetColor("_BaseColor", Color.white);
        sharedCharacterMaterial.SetColor("_EmissionColor", Color.black);
        sharedCharacterMaterial.DisableKeyword("_EMISSION");
        return sharedCharacterMaterial;
    }
}
