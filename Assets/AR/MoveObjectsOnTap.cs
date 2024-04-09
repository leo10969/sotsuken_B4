using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MoveObjectsOnTap : MonoBehaviour
{
    public ARRaycastManager raycastManager; // ARRaycastManagerへの参照
    public GameObject[] objectsToMove; // 移動させるオブジェクトの配列

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    // 配列内の各オブジェクトを移動させる
                    foreach (GameObject objectToMove in objectsToMove)
                    {
                        // オブジェクトが非アクティブ状態でも位置を更新する
                        objectToMove.transform.position = hitPose.position;
                        // 必要に応じてオブジェクトの向きも調整する
                        // objectToMove.transform.rotation = hitPose.rotation;
                    }
                }
            }
        }
    }
}
