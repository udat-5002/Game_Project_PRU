using UnityEngine;

/// <summary>
/// Trang phục áo bà ba: giảm độ bóng da/kim loại, giữ texture vải mềm.
/// </summary>
public static class AoBaBaLook
{
    public static void Apply(GameObject model)
    {
        if (model == null) return;

        foreach (var smr in model.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (smr == null) continue;

            var mats = smr.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == null) continue;
                var mat = new Material(mats[i]);
                SoftenCloth(mat);
                mats[i] = mat;
            }
            smr.materials = mats;
        }
    }

    static void SoftenCloth(Material mat)
    {
        if (mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", 0.12f);
        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", 0f);
        if (mat.HasProperty("_SpecularHighlights"))
            mat.SetFloat("_SpecularHighlights", 0f);
        if (mat.HasProperty("_BaseColor"))
        {
            // Giữ albedo texture; chỉ chỉnh nhẹ tông cho gần vải bà ba đen
            var c = mat.GetColor("_BaseColor");
            mat.SetColor("_BaseColor", Color.Lerp(c, new Color(0.92f, 0.94f, 0.96f, 1f), 0.08f));
        }
    }
}
