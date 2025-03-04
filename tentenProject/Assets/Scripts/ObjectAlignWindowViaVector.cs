using UnityEditor;
using UnityEngine;

public class ObjectAlignWindowViaVector : EditorWindow
{
    private GameObject baseObject;
    private Vector3 spacing;

    [MenuItem("Tools/Bonnate/Object Align Via Vector3 Value")]
    private static void OpenWindow()
    {
        ObjectAlignWindowViaVector window = GetWindow<ObjectAlignWindowViaVector>();
        window.titleContent = new GUIContent("Object Align");
        window.Show();
    }

    private void OnGUI()
    {
        baseObject = EditorGUILayout.ObjectField("기준이 될 오브젝트", baseObject, typeof(GameObject), true) as GameObject;
        spacing = EditorGUILayout.Vector3Field("간격", spacing);

        if (GUILayout.Button("정렬"))
        {
            AlignObjects();
        }

        GUILayout.Label("기준이 될 오브젝트 프로퍼티에 할당합니다.");
        GUILayout.Label("Scene에서 기준이 될 오브젝트로부터 간격마다 정렬할 오브젝트들을 선택합니다.");
        GUILayout.Label("[정렬] 버튼을 눌러 정렬합니다.");        
    }

    private void AlignObjects()
    {
        if (baseObject == null)
        {
            Debug.LogError("기준이 될 오브젝트가 없음!");
            return;
        }

        GameObject[] selectedObjects = Selection.gameObjects;
        int objectCount = selectedObjects.Length;

        if (objectCount == 0)
        {
            Debug.LogError("선택된 오브젝트가 없음!");
            return;
        }

        Undo.RegisterCompleteObjectUndo(selectedObjects, "Align Objects");

        Vector3 currentPosition = baseObject.transform.position + spacing;

        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = selectedObjects[i];

            if (obj == baseObject)
                continue;

            Undo.RecordObject(obj.transform, "Move Object");

            obj.transform.position = currentPosition;
            currentPosition += spacing;
        }
    }
}