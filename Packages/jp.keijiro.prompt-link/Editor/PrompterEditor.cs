using UnityEngine;
using UnityEditor;

namespace PromptLink.Editor {

[CustomEditor(typeof(Prompter))]
class PrompterEditor : UnityEditor.Editor
{
    AutoProperty _sources;
    AutoProperty BackAction;
    AutoProperty NextAction;

    void OnEnable()
      => AutoProperty.Scan(this);

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(BackAction);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(NextAction);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_sources);

        serializedObject.ApplyModifiedProperties();
    }

    [MenuItem("GameObject/PromptLink/Prompter", false, 10)]
    static void CreatePrompter()
    {
        var path = "Packages/jp.keijiro.prompt-link/Runtime/Prompter.prefab";
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var go = PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(go, "Create Prompter");
        Selection.activeGameObject = (GameObject)go;
    }
}

} // namespace PromptLink.Editor
