using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace MediaPipe.HandPose
{
    public class TouchManager : MonoBehaviour
    {
        [SerializeField] RaycastingScript raycastingScript;
        private GameObject lastHitKey;
        public bool wasTouchedLastFrame = false;
        private Collider vc;
        public float touchAccumulatedTime = 0f; // Accumulated time

        private void Start()
        {
            vc = raycastingScript.VirtualCollider;
            lastHitKey = raycastingScript.LastHitKey;

            // 仮想的なColliderに"IndexRay"タグを設定
            this.gameObject.tag = "IndexRay";
        }

        private void Update()
        {
            touchAccumulatedTime += Time.deltaTime;
            // タッチ入力またはマウスクリックを検出
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                Touch touch = Input.touchCount > 0 ? Input.GetTouch(0) : new Touch();
                if (Input.GetMouseButtonDown(0) || (touch.phase == TouchPhase.Began))
                {
                    HandleTouch(touch.position);
                }
                else if (Input.GetMouseButtonUp(0) || (touch.phase == TouchPhase.Ended))
                {
                    EndTouch();
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    HandleContinuousTouch(touch.position);
                }
            }
            wasTouchedLastFrame = (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0));
        }

        private void EndTouch()
        {
            if (lastHitKey != null)
            {
                KeyController lastKeyController = lastHitKey.GetComponent<KeyController>();
                if (lastKeyController)
                {
                    lastKeyController.OnTriggerExit(vc);
                }
                lastHitKey = null;
            }
        }


        private void HandleTouch(Vector2 position)
        {
            raycastingScript.CastRayFromTouch(position);
        }

        private void HandleContinuousTouch(Vector2 position)
        {
            raycastingScript.CastRayFromTouch(position);
        }
    }
}
