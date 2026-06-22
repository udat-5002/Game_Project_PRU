using UnityEngine;
using UnityEditor;

public class PreviewAnimation : EditorWindow
{
    private GameObject character;
    private AnimationClip clip;
    private float time;
    private bool isPlaying = false;
    private double lastTime;

    [MenuItem("Tools/Preview Character Animation")]
    public static void ShowWindow()
    {
        GetWindow<PreviewAnimation>("Preview Animation");
    }

    void OnGUI()
    {
        GUILayout.Label("Animation Previewer", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        // Auto-assign selected object
        if (character == null && Selection.activeGameObject != null)
        {
            character = Selection.activeGameObject;
        }

        character = (GameObject)EditorGUILayout.ObjectField("Character", character, typeof(GameObject), true);
        clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip (Run/Walk)", clip, typeof(AnimationClip), false);

        GUILayout.Space(10);

        if (character != null && clip != null)
        {
            if (GUILayout.Button(isPlaying ? "Stop Preview" : "Play Preview in Scene", GUILayout.Height(30)))
            {
                isPlaying = !isPlaying;
                if (isPlaying)
                {
                    lastTime = EditorApplication.timeSinceStartup;
                    EditorApplication.update += UpdatePreview;
                }
                else
                {
                    EditorApplication.update -= UpdatePreview;
                    AnimationMode.StopAnimationMode();
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please select a Character and an Animation Clip to preview.", MessageType.Info);
        }
    }

    void UpdatePreview()
    {
        if (character == null || clip == null)
        {
            isPlaying = false;
            EditorApplication.update -= UpdatePreview;
            if (AnimationMode.InAnimationMode())
            {
                AnimationMode.StopAnimationMode();
            }
            return;
        }

        if (!AnimationMode.InAnimationMode())
        {
            AnimationMode.StartAnimationMode();
        }

        double currentTime = EditorApplication.timeSinceStartup;
        float deltaTime = (float)(currentTime - lastTime);
        lastTime = currentTime;

        time += deltaTime;
        if (time > clip.length)
        {
            time -= clip.length; // Loop animation
        }

        AnimationMode.BeginSampling();
        AnimationMode.SampleAnimationClip(character, clip, time);
        AnimationMode.EndSampling();
        
        SceneView.RepaintAll();
    }

    void OnDestroy()
    {
        if (isPlaying)
        {
            EditorApplication.update -= UpdatePreview;
            if (AnimationMode.InAnimationMode())
            {
                AnimationMode.StopAnimationMode();
            }
        }
    }
}
