using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace MediaPipe.HandPose
{
    public class RaycastingScript : MonoBehaviour
    {
        //For Touch
        private GameObject lastHitKey; // 最後に衝突したキーを保存するための変数
        public GameObject LastHitKey{

            get{ return lastHitKey;}
        }
        private Collider virtualCollider; // 仮想的なCollider
        public Collider VirtualCollider{

            get{ return virtualCollider;}
        }
        private List<Vector3> touchgesturePositions = new List<Vector3>();
        private List<Vector3> handTrackgesturePositions = new List<Vector3>();
        [SerializeField] ExportCsvScript csvExporter; // ExportCsvScriptへの参照
        [SerializeField] HandAnimator handAnimator;
        [SerializeField] TouchManager touchManager;
        [SerializeField] PositionFinder positionFinder;


        private void Start()
        {
            // 仮想的なColliderの初期化
            virtualCollider = this.gameObject.AddComponent<SphereCollider>();
            virtualCollider.isTrigger = true;
            virtualCollider.enabled = false; // 実際の衝突判定には使用しないため、無効化

            // 仮想的なColliderに"IndexRay"タグを設定
            this.gameObject.tag = "IndexRay";
        }
    
        public void CastRayFromIndexFinger(Vector3 startPos, Vector3 direction, int index)
        {
            Ray ray = new Ray(startPos, direction);
            int layerMask = 3 << LayerMask.NameToLayer("KeyboardKeys");

            RaycastHit[] hits = Physics.RaycastAll(startPos, direction, Mathf.Infinity, layerMask);

            //レイがいずれかのキーに衝突している場合
            if(hits.Length > 1)
            {
                // hitsを距離でソート
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
                for (int i = 0; i < hits.Length; i++)
                {
                    if (i == 0) // 最初に衝突したオブジェクト = キー
                    {
                        // 衝突しているキーのKeyControllerを取得
                        KeyController keyController = hits[i].collider.gameObject.GetComponent<KeyController>();
                        if(keyController != null)
                        {
                            keyController.keyTextMesh.color = Color.blue;
                        }
                        if(lastHitKey)
                        {
                            // すでに指が何かのキーの上にある状態
                            KeyController lastKeyController = lastHitKey.GetComponent<KeyController>();   
                            //すでに押されたキーと現在のレイが当たっているキーが異なるとき（＝異なるキーに動いた時）
                            if((lastHitKey != hits[i].collider.gameObject || hits[i].collider.gameObject == null)&& lastKeyController)
                            {
                                //前回のキーの色を元に戻す
                                lastKeyController.keyTextMesh.color = Color.red;
                                handAnimator.hasInputTriggered = false;
                            }
                            //すでに押されたキーと現在のレイが当たっているキーが同じとき（＝同じキーから動いていない時）
                            else if(lastHitKey == hits[i].collider.gameObject && lastKeyController)
                            {
                                //前回のキーの色を青にする（＝維持する）
                                lastKeyController.keyTextMesh.color = Color.blue;
                            }
                        }

                        //ーーーーーーーーーーーーーーーーートリガー判定部分ーーーーーーーーーーーーーーーーーー
                        // あるキーに衝突している，親指トリガーオン，かつ，（前回はどのキーにも衝突していない，または，異なるキーに衝突していた）
                        if (keyController && handAnimator.isTriggeredwithThumb && !handAnimator.hasInputTriggered)
                        {
                            // // 前回衝突していたキーのOnTriggerExitを呼び出す
                            // if (lastHitKey != null)
                            // {

                            //     if (lastKeyController)
                            //     {
                            //         lastKeyController.OnTriggerExit(handAnimator.indexTipSphere.GetComponent<Collider>());
                            //     }
                            // }

                            // 衝突しているキーを取得
                            // 新しく衝突したキーのOnTriggerEnterを呼び出す
                            keyController.OnTriggerEnter(handAnimator.indexTipSphere.GetComponent<Collider>());
                            handAnimator.hasInputTriggered = true; // 入力がトリガーされたことを示すフラグを設定
                            
                        }
                        // 同じキーに止まっているが，親指トリガーオフになった場合
                        else if (keyController && !handAnimator.isTriggeredwithThumb)
                        {
                            handAnimator.hasInputTriggered = false;
                            // 衝突しているキーのOnTriggerExitを呼び出す
                            // keyController.OnTriggerExit(handAnimator.indexTipSphere.GetComponent<Collider>());
                        }
                        lastHitKey = hits[i].collider.gameObject;
                    }
                    //ジェスチャストローク取得のための処理
                    else if (i == 1 && handAnimator.isTriggeredwithThumb ) // 次に衝突したオブジェクト = KeyboardBase
                    {
                        //hits[i].pointがキーボードの範囲内にあるかどうかを調べる．
                        Rect rect;
                        rect = new Rect(positionFinder.QTopLeft.x, positionFinder.PRightEdge.x, positionFinder.QTopLeft.y, positionFinder.MBottomEdge.y);
                        if(index == 8 && rect.Contains(new Vector2(hits[i].point.x, hits[i].point.y)))
                        {
                            handTrackgesturePositions.Add(hits[i].point);
                            // Debug.Log(smoothing_position - position);
                            string[] dataForGesture = 
                            {
                                hits[i].point.x.ToString(),
                                hits[i].point.y.ToString(),
                                hits[i].point.z.ToString()
                            };
                            csvExporter.SaveToCSV(dataForGesture);
                        }
                    }
                }
            }
            //レイが衝突していない場合
            else if(hits.Length == 0)
            {
                if (lastHitKey != null)
                {
                    KeyController lastKeyController = lastHitKey.GetComponent<KeyController>();
                    //前回のキーの色を元に戻す
                    lastKeyController.keyTextMesh.color = Color.red;

                    if (lastKeyController && handAnimator.isTriggeredwithThumb)
                    {
                        Debug.Log(lastKeyController.keyTextMesh.color);
                        lastKeyController.OnTriggerExit(handAnimator.indexTipSphere.GetComponent<Collider>());
                        handAnimator.isTriggeredwithThumb = false;
                    }
                    lastHitKey = null;
                }
                handAnimator.isTriggeredwithThumb = false;
            }
        }


        //For Touch
        // private float nextTriggerTime = 0.5f; // 次にOnTriggerEnterを呼び出すべき時刻
        // private bool wasTriggeredRecently = false;
        
        public void CastRayFromTouch(Vector2 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            int layerMask = 3 << LayerMask.NameToLayer("KeyboardKeys");
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

            // レイが衝突した場合
            if (hits.Length > 0)
            {
                // hitsを距離でソート
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                for (int i = 0; i < hits.Length; i++)
                {
                    if (i == 0) // 最初に衝突したオブジェクト = キー
                    {
                        KeyController keyController = hits[i].collider.gameObject.GetComponent<KeyController>();
                        // Debug.Log("wasTouchedLastFrame: " + touchManager.wasTouchedLastFrame);
                        // New touch detected on the same key
                        
                        if (touchManager.wasTouchedLastFrame == false && keyController && lastHitKey == hits[i].collider.gameObject)
                        {
                            keyController.OnTriggerEnter(virtualCollider);
                        }
                        // あるキーに衝突している，かつ，（前回はどのキーにも衝突していない，または，異なるキーに衝突していた）
                        else if (keyController && (lastHitKey == null || lastHitKey != hits[i].collider.gameObject))
                        {
                            // 前回衝突していたキーのOnTriggerExitを呼び出す
                            if (lastHitKey != null)
                            {
                                KeyController lastKeyController = lastHitKey.GetComponent<KeyController>();
                                if (lastKeyController)
                                {
                                    lastKeyController.OnTriggerExit(virtualCollider);
                                }
                            }
                            // 衝突しているキーを取得
                            lastHitKey = hits[i].collider.gameObject;
                            // 新しく衝突したキーのOnTriggerEnterを呼び出す
                            // Debug.Log("Touch:OnTriggerEnter");
                            keyController.OnTriggerEnter(virtualCollider);

                            // // 次回のOnTriggerEnterを呼ぶための時間を設定
                            // nextTriggerTime = Time.time + 0.5f;
                            
                            // // 最近トリガーされたことを記録
                            // wasTriggeredRecently = true;
                        }
                        // // 同じキーに止まっている場合
                        // else if (keyController && lastHitKey == hits[i].collider.gameObject)
                        // {
                        //     if (Time.time >= nextTriggerTime && !wasTriggeredRecently)
                        //     {
                        //         keyController.OnTriggerEnter(virtualCollider);
                        //         nextTriggerTime = Time.time + 0.3f;
                        //     }
                        //     wasTriggeredRecently = false;  // フラグをリセット
                        // }
                        // 新しいタッチが検出された場合、OnTriggerEnterを実行
                        
                    }
                    // 他のオブジェクトとの衝突に関する処理はここに追加
                    else if(i == 1)
                    {
                        //hits[i].pointがキーボードの範囲内にあるかどうかを調べる．
                        Rect rect;
                        rect = new Rect(positionFinder.QTopLeft.x, positionFinder.PRightEdge.x, positionFinder.QTopLeft.y, positionFinder.MBottomEdge.y);
                        if(rect.Contains(new Vector2(hits[i].point.x, hits[i].point.y)))
                        {
                            touchgesturePositions.Add(hits[i].point);
                            // Debug.Log(smoothing_position - position);
                            string[] dataForGesture = 
                            {
                                hits[i].point.x.ToString(),
                                hits[i].point.y.ToString(),
                                hits[i].point.z.ToString()
                            };
                            csvExporter.SaveToCSV(dataForGesture);
                        }
                    }
                }
            }
            // レイが衝突していない場合
            else if (hits.Length == 0)
            {
                if (lastHitKey != null)
                {
                    KeyController lastKeyController = lastHitKey.GetComponent<KeyController>();
                    if (lastKeyController)
                    {
                        lastKeyController.OnTriggerExit(virtualCollider);
                    }
                    lastHitKey = null;
                }
            }
            
        }


    }
}
