using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;
//for Depth
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

namespace MediaPipe.HandPose
{
    public sealed class HandAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject _jointPrefab;

        [SerializeField] private Transform _handParent;

        [SerializeField] CameraImageController _cameraTransfar = null;
        [SerializeField] CaptureXRCamera _CaputureXRCamera;
        [SerializeField] ExportCsvScript csvExporter; // ExportCsvScriptへの参照
        
        // CSVとして保存するデータの種類をenumで定義
        
        public int framecount = 0;

        [SerializeField] RaycastingScript raycastingScript;//RaycastingScriptへの参照

        [SerializeField] ResourceSet _resources = null;
        [SerializeField] bool _useAsyncReadback = true;

        private HandPipeline _pipeline;

        private Dictionary<HandPipeline.KeyPoint, GameObject> _handJoints =
            new Dictionary<HandPipeline.KeyPoint, GameObject>();

        // [SerializeField] ARCameraManager _arCameraManager; // ARCameraManagerの参照を追加
        [SerializeField] AROcclusionManager _arOcclusionManager; // AROcclusionManagerの参照を追加
        
        //人差し指の先端のSphereを参照するための変数を追加
        public GameObject indexTipSphere;
        //入力ジェスチャのための変数を追加
        public GameObject thumbTipSphere;//No.4
        public GameObject thumbSecondJointSphere;//No.3
        public GameObject middleTipSphere;//No.16
        public GameObject middleForthJointSphere;//No.13
        public GameObject littleTipSphere;//No.20
        public GameObject littleForthJointSphere;//No.17
        //Sphereを格納するリスト
        public List<GameObject> fingerTips = new List<GameObject>();


        //入力ジェスチャのフラグを追加
        public bool isTriggeredwithThumb = false;
        public bool isTriggeredwithMiddle = false;
        //isTriggeredwithThumbがtrueになったら一度だけ入力
        public bool hasInputTriggered = false; 

        // 初期状態では透明でないと仮定
        private bool areObjectsTransparent = false;  


        // レイキャスティングの距離
        private const float rayDistance = 5.0f;
        

        public float handAccumulatedTime = 0f; // Accumulated time
        // 人差し指の座標を保存するためのリストを追加
        private List<Vector3> gesturePositions = new List<Vector3>();
        private List<Vector3> smoothedgesturePositions = new List<Vector3>();

        public HeaderType currentHeaderType; // Inspectorから設定できるようにpublic変数として追加
        public enum HeaderType
        {
            GestureStroke,
            Filter,
            PhraseSetTest
        }
        //smoothing
        public FilterType currentFilterType;
        //フィルタの種類をenumで定義
        public enum FilterType
        {
            Kalman,
            EMA,
            OneEuro
        }

        //OneEuroフィルタのベータ係数を設定(遅延)
        public float beta;
        //OneEuroフィルタの最小カットオフ周波数を設定（ジッター）
        public float fcmin;
        //1ユーロフィルタの各パラメータ設定をキーとして，値はそのパラメータを設定したフィルタ
        private Dictionary<HandPipeline.KeyPoint, OneEuroFilter> _oneEuroFilters = new Dictionary<HandPipeline.KeyPoint, OneEuroFilter>(); 

        private void InitializeOneEuroFilters()
        {
            foreach (HandPipeline.KeyPoint point in System.Enum.GetValues(typeof(HandPipeline.KeyPoint)))
            {
                _oneEuroFilters[point] = new OneEuroFilter(beta, fcmin);
                // if (_oneEuroFilters[point] == null)
                //     Debug.LogError(point.ToStlittle() + " is null after assignment!");
                // else
                //     Debug.Log(point.ToStlittle() + " is successfully initialized.");
            }
        }

        void Start()
        {
            InitializeOneEuroFilters();
            _pipeline = new HandPipeline(_resources);
            initalizeHandJoint();
        }

        private void OnDestroy()
        {
            _pipeline.Dispose();
        }

        private void Update()
        {
            _pipeline.UseAsyncReadback = _useAsyncReadback;
            //var cameraTexture = _cameraTransfar.m_Texture;
            var cameraTexture = _CaputureXRCamera._previewTexture;
            if (cameraTexture == null) return;
            _pipeline.ProcessImage(cameraTexture);

            handAccumulatedTime += Time.deltaTime;
            framecount++;
            //手の座標更新
            updateHandPose();
        }

        /// <summary>
        /// 手のパーツの初期化
        /// </summary>
        private void initalizeHandJoint()
        {
            _jointPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Renderer rend = _jointPrefab.GetComponent<Renderer>();

            // マテリアルのレンダリングモードをTransparentに設定
            Material material = rend.material;

            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;

            // 任意でマテリアルの色も変更
            material.color = new Color(0, 1, 0, 1.0f);

            // // SmoothnessとMetallicの値を0に設定
            // material.SetFloat("_Metallic", 0f);
            // material.SetFloat("_Glossiness", 0f); // "_Glossiness" はSmoothnessのこと

            for (int i = 0; i < HandPipeline.KeyPointCount; i++)
            {
                var go = Instantiate(_jointPrefab, _handParent);
                go.SetActive(true);
                // var _renderer = go.GetComponent<Renderer>();
                // _renderer.material.color = new Color(0.0f, 1.0f, 0.0f, 0.0f);
                // Debug.Log(_renderer.material.color);
                var keyPoint = (HandPipeline.KeyPoint)i;
                _handJoints.Add(keyPoint, go);
                //親指の第2関節
                if (i == 3)
                {
                    thumbSecondJointSphere = go;
                    fingerTips.Add(thumbSecondJointSphere);
                }
                else if (i == 4)
                {
                    thumbTipSphere = go;
                    fingerTips.Add(thumbTipSphere);
                }
                //中指の第4関節
                else if (i == 9)
                {
                    
                    middleForthJointSphere = go;
                    fingerTips.Add(middleForthJointSphere);

                }
                //中指の先端
                else if (i == 12)
                {
                    
                    middleTipSphere = go;
                    fingerTips.Add(middleTipSphere);
                }
                //薬指の第4関節
                else if (i == 17)
                {
                    littleForthJointSphere = go;
                    fingerTips.Add(littleForthJointSphere);
                }
                //薬指の先端
                else if (i ==20)
                {
                    littleTipSphere = go;
                    fingerTips.Add(littleTipSphere);

                }
                //人差し指 if keyPoint corresponds to the tip of the index finger, add a specific tag to the GameObject
                else if (i == 8)
                {
                    
                    go.tag = "IndexRay";
                    indexTipSphere = go;
                    fingerTips.Add(indexTipSphere);
                }
                else
                {
                    go.SetActive(false);
                }
                
            }
            // このオブジェクトをシーンから遠ざける（例：ワールド座標の原点から非常に遠い位置へ移動）
            _jointPrefab.transform.position = new Vector3(10000, 10000, 10000);
        }

        /// <summary>
        /// 手の座標更新
        /// </summary>
        private void updateHandPose()
        {   
            for (int i = 0; i < HandPipeline.KeyPointCount; i++)
            {
                if(i == 3 || i == 4 ||i == 9 || i == 12 || i == 17 || i == 20 || i == 8)
                {
                    var position = _pipeline.GetKeyPoint(i);
                    // Debug.Log("position: " + position);
                    var keyPoint = (HandPipeline.KeyPoint)i;
                    var smoothing_position = _oneEuroFilters[keyPoint].UpdateEstimate(position); //フィルタリング適用前の座標
                    
                    //ワールド座標に変換する
                    float xPos = Screen.width * normalize(smoothing_position.x, -0.5f, 0.5f);
                    float yPos = Screen.height * normalize(smoothing_position.y, -0.5f, 0.5f);
                    float zPos = 0.3f + smoothing_position.z;

                    Vector3 cameraPos = new Vector3(xPos, yPos, zPos);
                    var screenPosition = Camera.main.ScreenToWorldPoint(cameraPos);
                    // それぞれの手のパーツに座標を代入
                    _handJoints[keyPoint].transform.position = screenPosition;

                    // // ここで、zPosに基づいて、関節の大きさを調整します
                    // float scaleFactor = 1.0f / (zPos + 1); 
                    // _handJoints[keyPoint].transform.localScale = Vector3.one * scaleFactor;
                    //  人差し指のSphereの位置と向きを基にレイを生成
                    if (indexTipSphere)
                    {   
                        Vector3 currentPosition = indexTipSphere.transform.position;
                        Vector3 direction = (currentPosition - Camera.main.transform.position).normalized;
                        Vector3 endPos = currentPosition + direction * rayDistance; 
                        Debug.DrawLine(currentPosition, endPos, Color.green, 2f);

                        // 人差し指の先端からのレイキャスティングを行う
                        // Debug.Log(isTriggeredwithThumb);
                        raycastingScript.CastRayFromIndexFinger(currentPosition, direction, i);

                    }
                }
                
            }
        }
        //ローカル関数:座標の正規化
        public float normalize(float value, float min, float max)
        {
            float cValue = Mathf.Clamp(value, min, max);
            return (cValue - min) / (max - min);
        }

        public void UnenableHandObjects(bool hide)
        {
            foreach (var tip in fingerTips)
            {
                tip.SetActive(hide);
            }
        }

        public void ToggleTransparency()
        {
            foreach (var tip in fingerTips)
            {
                var renderer = tip.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Color currentColor = renderer.material.color;
                    renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, (currentColor.a == 1.0f) ? 0.0f : 1.0f);   
                }
            }
            Debug.Log("changed color: " + fingerTips[0].GetComponent<Renderer>().material.color);
        }
    }
} // namespace MediaPipe.HandPose


