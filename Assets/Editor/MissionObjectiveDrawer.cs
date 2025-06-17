using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomPropertyDrawer(typeof(MissionInfo.MissionObjective))]
public class MissionObjectiveDrawer : PropertyDrawer
{
    private const float ButtonWidth = 20f;
    private const float AddButtonWidth = 120f;
    private const float DropdownLabelWidth = 150f;

    private Dictionary<string, int> _propertySelectedIndex = new Dictionary<string, int>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 3;
        height += EditorGUIUtility.standardVerticalSpacing * 3;

        SerializedProperty requirementsList = property.FindPropertyRelative("Requirements");
        height += EditorGUIUtility.singleLineHeight;

        if (requirementsList.isExpanded)
        {
            height += EditorGUIUtility.standardVerticalSpacing;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.standardVerticalSpacing;

            if (requirementsList.arraySize == 0)
            {
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                for (int i = 0; i < requirementsList.arraySize; i++)
                {
                    SerializedProperty element = requirementsList.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(element, true);
                    height += EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }
        height += EditorGUIUtility.standardVerticalSpacing;
        return height;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect currentRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("ObjectiveID"));
        currentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("ObjectiveName"));
        currentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(currentRect, property.FindPropertyRelative("isCompleted"));
        currentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        currentRect.y += EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty requirementsList = property.FindPropertyRelative("Requirements");
        requirementsList.isExpanded = EditorGUI.Foldout(currentRect, requirementsList.isExpanded, "Requirements", true);
        currentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (requirementsList.isExpanded)
        {
            EditorGUI.indentLevel++;

            string propertyPath = property.propertyPath;
            if (!_propertySelectedIndex.ContainsKey(propertyPath))
            {
                _propertySelectedIndex[propertyPath] = -1;
            }

            float availableWidth = currentRect.width;
            float dropdownWidth = availableWidth - AddButtonWidth - 5;
            if (dropdownWidth < DropdownLabelWidth + 50)
            {
                dropdownWidth = availableWidth;
            }


            Rect dropdownRect = new Rect(currentRect.x, currentRect.y, dropdownWidth, EditorGUIUtility.singleLineHeight);
            Rect actualAddButtonRect = new Rect(dropdownRect.xMax + 5, currentRect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight);

            if (ObjectiveTypeCache.RequirementTypes.Count > 0)
            {
                _propertySelectedIndex[propertyPath] = EditorGUI.Popup(dropdownRect, "Add Requirement Type:", _propertySelectedIndex[propertyPath], ObjectiveTypeCache.RequirementTypeNames);

                bool canAdd = _propertySelectedIndex[propertyPath] >= 0;
                EditorGUI.BeginDisabledGroup(!canAdd);
                if (GUI.Button(actualAddButtonRect, "Add Selected") && canAdd)
                {
                    Type typeToAdd = ObjectiveTypeCache.RequirementTypes[_propertySelectedIndex[propertyPath]];
                    requirementsList.arraySize++;
                    SerializedProperty newElement = requirementsList.GetArrayElementAtIndex(requirementsList.arraySize - 1);
                    newElement.managedReferenceValue = Activator.CreateInstance(typeToAdd);
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUI.LabelField(dropdownRect, "No ObjectiveRequirement types found.");
            }
            currentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


            if (requirementsList.arraySize == 0)
            {
                EditorGUI.LabelField(currentRect, "List is empty.");
                currentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                for (int i = 0; i < requirementsList.arraySize; i++)
                {
                    SerializedProperty element = requirementsList.GetArrayElementAtIndex(i);
                    float elementHeight = EditorGUI.GetPropertyHeight(element, true);
                    Rect elementRect = new Rect(currentRect.x, currentRect.y, currentRect.width - ButtonWidth - 5, elementHeight);
                    EditorGUI.PropertyField(elementRect, element, true);

                    Rect removeButtonRect = new Rect(elementRect.xMax + 5, currentRect.y, ButtonWidth, EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(removeButtonRect, "-"))
                    {
                        element.managedReferenceValue = null;
                        requirementsList.DeleteArrayElementAtIndex(i);
                    }
                    currentRect.y += elementHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}