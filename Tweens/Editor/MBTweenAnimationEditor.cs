using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MBTweenAnimation))]
public class MBTweenAnimationEditor : Editor
{
    private bool testInEditor;

    private MBTweenAnimation anim;

    public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		anim = target as MBTweenAnimation;
		DrawDebugButtons(anim);

	    bool previousTestInEditor = testInEditor;
	    testInEditor = EditorGUILayout.Toggle("Test In Editor", testInEditor);
	    if (previousTestInEditor != testInEditor)
	    {
	        if (testInEditor)
	        {
	            anim.SubscribeToEditorUpdates();
	        }
	        else
	        {
	            anim.UnsubscribeFromEditorUpdates();
	        }
	    }
	}

    private void OnDisable()
    {
        if (testInEditor)
        {
             anim.UnsubscribeFromEditorUpdates();
        }
        testInEditor = false;
    }

    static void DrawDebugButtons(MBTweenAnimation animation)
	{
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Set End State"))
		{
			animation.SetEndState(0);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Set Begin State"))
		{
			animation.SetBeginState(0);
		}
		EditorGUILayout.EndHorizontal();
	}

}