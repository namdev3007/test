using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ComboboxData))]
public class ComboboxDataEditor : Editor
{
    SerializedObject m_Object;

    SerializedProperty listView;
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        m_Object = new SerializedObject(target);

        listView = serializedObject.FindProperty("listView");
        reorderableList = new ReorderableList(m_Object,
                                              m_Object.FindProperty("data"),
                                              true,   // draggable (kéo thả được)
                                              true,   // displayHeader (hiện tiêu đề)
                                              true,   // displayAddButton
                                              true    // displayRemoveButton
                                              );


        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Combobox data");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            float fieldHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 2f;

            // Tăng chiều cao nếu muốn hiển thị cả nhiều field
            EditorGUI.PropertyField(
                new Rect(rect.x + 10, rect.y, rect.width, fieldHeight),
                element, true); // 👈 TRUE để hiện phần tử con (expandable)
        };

        // 👇 Quan trọng: tự động tính lại chiều cao nếu có nhiều field con
        reorderableList.elementHeightCallback = (int index) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true) + 4;
        };
    }

    public override void OnInspectorGUI()
    {
        m_Object.Update();

        EditorGUILayout.PropertyField(listView);
        EditorGUILayout.Space();
        reorderableList.DoLayoutList();

        m_Object.ApplyModifiedProperties();
        
    }
}