using UnityEngine;
using UnityEditor;

public class RandomScaleTool : EditorWindow
{
    private float minScale = 0.8f;
    private float maxScale = 1.2f;
    private bool uniformScale = true;

    // Các biến cho Non-uniform scale (scale riêng lẻ từng trục)
    private Vector3 minScaleVector = new Vector3(0.8f, 0.8f, 0.8f);
    private Vector3 maxScaleVector = new Vector3(1.2f, 1.2f, 1.2f);

    [MenuItem("Tools/Random Scale Tool")]
    public static void ShowWindow()
    {
        GetWindow<RandomScaleTool>("Random Scale");
    }

    private void OnGUI()
    {
        GUILayout.Label("Randomize Scale of Selected Objects", EditorStyles.boldLabel);
        GUILayout.Space(5);

        uniformScale = EditorGUILayout.Toggle("Uniform Scale (Scale đều)", uniformScale);
        GUILayout.Space(5);

        if (uniformScale)
        {
            minScale = EditorGUILayout.FloatField("Min Scale", minScale);
            maxScale = EditorGUILayout.FloatField("Max Scale", maxScale);

            if (minScale > maxScale) minScale = maxScale;
        }
        else
        {
            minScaleVector = EditorGUILayout.Vector3Field("Min Scale", minScaleVector);
            maxScaleVector = EditorGUILayout.Vector3Field("Max Scale", maxScaleVector);
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Apply Random Scale", GUILayout.Height(30)))
        {
            RandomizeScale();
        }
        
        GUILayout.Space(5);
        GUILayout.Label($"Đang chọn: {Selection.gameObjects.Length} vật thể", EditorStyles.helpBox);
    }

    private void RandomizeScale()
    {
        if (Selection.gameObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Lưu ý", "Vui lòng chọn ít nhất một vật thể trong Hierarchy để thay đổi kích thước.", "OK");
            return;
        }

        foreach (GameObject go in Selection.gameObjects)
        {
            // Ghi lại trạng thái để có thể Undo (Ctrl+Z)
            Undo.RecordObject(go.transform, "Random Scale");

            if (uniformScale)
            {
                float randomScale = Random.Range(minScale, maxScale);
                go.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            }
            else
            {
                float randX = Random.Range(minScaleVector.x, maxScaleVector.x);
                float randY = Random.Range(minScaleVector.y, maxScaleVector.y);
                float randZ = Random.Range(minScaleVector.z, maxScaleVector.z);
                go.transform.localScale = new Vector3(randX, randY, randZ);
            }
        }
    }
}
