using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MBTweenBase), true)]
public class MBTweenBaseEditor : Editor
{
    private bool testInEditor;

    private MBTweenBase tween;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("General Parameters:", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        tween = (MBTweenBase)target;

        tween.duration = EditorGUILayout.FloatField("Duration", tween.duration);
        tween.durationScale = EditorGUILayout.FloatField("Duration Scale", tween.durationScale);

        tween.delay = EditorGUILayout.FloatField("Delay", tween.delay);

        tween.ignoreTimeScale = EditorGUILayout.Toggle("Ignore Time Scale", tween.ignoreTimeScale);

        tween.easingMethod = (EasingMethod)EditorGUILayout.EnumPopup("Easing Method",tween.easingMethod);

        if (tween.easingMethod == EasingMethod.Curve)
        {
            tween.curve = EditorGUILayout.CurveField("Curve", tween.curve);
        }

        if (tween.easingMethod == EasingMethod.EaseIn ||
            tween.easingMethod == EasingMethod.EaseOut ||
            tween.easingMethod == EasingMethod.EaseInOut)
        {
            tween.easingType = (PGR.EasingType)EditorGUILayout.EnumPopup("Easing Type", tween.easingType);
        }

        tween.looping = (LoopType)EditorGUILayout.EnumPopup("Loop Type", tween.looping);

        DrawEvent("OnEndStateSet");
        DrawEvent("OnBeginStateSet");

        DrawDebugButtons(tween);

        bool previousTestInEditor = testInEditor;
        testInEditor = EditorGUILayout.Toggle("Test In Editor", testInEditor);
        if (previousTestInEditor != testInEditor)
        {
            if (testInEditor)
            {
                tween.SubscribeToEditorUpdates();
            }
            else
            {
                tween.UnsubscribeFromEditorUpdates();
            }
        }
    }

    private void OnDisable()
    {
        if (testInEditor)
        {
            tween.UnsubscribeFromEditorUpdates();
        }
        testInEditor = false;
    }

    void DrawEvent(string eventName)
    {
        SerializedProperty onCheck = serializedObject.FindProperty(eventName);

        EditorGUILayout.PropertyField(onCheck);

        if(GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    static void DrawDebugButtons(MBTweenBase tween)
    {
        Color defColor = GUI.color;

        GUI.color = tween.IsInBeginState ? Color.green : defColor;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Set End State"))
        {
            tween.SetEndState();
        }
        EditorGUILayout.EndHorizontal();


        GUI.color = tween.IsInEndState ? Color.green : defColor;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Set Begin State"))
        {
            tween.SetBeginState();
        }
        EditorGUILayout.EndHorizontal();

        GUI.color = defColor;
    }
}
