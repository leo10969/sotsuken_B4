using UnityEngine;
using UnityEditor;
using MediaPipe.HandPose;

[CustomEditor(typeof(HandAnimator))]
public class HandAnimatorEditor : Editor
{
    private HandAnimator.HeaderType lastHeaderType; // 以前のHeaderTypeの値を保存するための変数

    void OnEnable()
    {
        HandAnimator handAnimator = (HandAnimator)target;
        lastHeaderType = handAnimator.currentHeaderType; // OnEnable時の値を保存
    }

    public override void OnInspectorGUI()
    {
        HandAnimator handAnimator = (HandAnimator)target;

        // Draw default inspector
        DrawDefaultInspector();

        // Check if ExportCsvScript's mode is not Debug when FilterType is set
        if (handAnimator.currentHeaderType == HandAnimator.HeaderType.Filter || handAnimator.currentHeaderType == HandAnimator.HeaderType.GestureStroke)
        {
            ExportCsvScript exportCsvScript = FindObjectOfType<ExportCsvScript>();
            if (exportCsvScript && exportCsvScript.currentMode != ExportCsvScript.Mode.Debug)
            {
                EditorGUILayout.HelpBox("FilterType should only be set in Debug mode of ExportCsvScript!", MessageType.Warning);
                handAnimator.currentHeaderType = lastHeaderType; // 以前の値に戻す
            }
            else
            {
                lastHeaderType = handAnimator.currentHeaderType; // 現在の値を保存
            }
        }
        else
        {
            lastHeaderType = handAnimator.currentHeaderType; // 現在の値を保存
        }
    }
}
