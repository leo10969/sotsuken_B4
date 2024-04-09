using UnityEngine;
using UnityEngine.XR.ARFoundation; // AR Foundationの名前空間を使用

public class PlaneDetectionController : MonoBehaviour
{
    public ARPlaneManager planeManager; // AR Plane Managerへの参照
    public GameObject keyboard1; // キーボードオブジェクト1への参照
    public GameObject keyboard2; // キーボードオブジェクト2への参照

    private bool isPlaneDetectionEnabled = false; // 平面検出モードの状態

    // 平面検出モードの切り替え
    public void TogglePlaneDetection()
    {
        isPlaneDetectionEnabled = !isPlaneDetectionEnabled;
        planeManager.enabled = isPlaneDetectionEnabled; // ARPlaneManagerの有効/無効を切り替え

        // 全ての既存の平面をアクティブ/非アクティブに切り替え
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(isPlaneDetectionEnabled);
        }

        // キーボードオブジェクトのColliderを平面検出モードに応じて有効/無効に切り替える
        ToggleKeyboardColliders(!isPlaneDetectionEnabled);
    }

    // キーボードオブジェクトのColliderの有効/無効を切り替えるメソッド
    private void ToggleKeyboardColliders(bool isEnabled)
    {
        // キーボードオブジェクトが複数のColliderを持っている可能性があるため、全てのColliderを対象にする
        Collider[] keyboard1Colliders = keyboard1.GetComponentsInChildren<Collider>();
        foreach (var collider in keyboard1Colliders)
        {
            collider.enabled = isEnabled;
        }

        Collider[] keyboard2Colliders = keyboard2.GetComponentsInChildren<Collider>();
        foreach (var collider in keyboard2Colliders)
        {
            collider.enabled = isEnabled;
        }
    }
}
