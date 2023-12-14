using UnityEngine;

namespace MediaPipe.HandPose
{
    public class HandGesture : MonoBehaviour
    {
        [SerializeField] private HandAnimator handAnimator;
        public CsvDataHandler csvHandler;

        [SerializeField] private float triggerDifferenceThumb; // トリガーとなる座標の差（XまたはZ）
        [SerializeField] private float triggerYDifferenceMiddle; // トリガーとなるY軸の差
        [SerializeField] private float triggerYDifferenceLittle; // トリガーとなるY軸の差 for little finger
        private Vector3 lastThumbDifferenceVector; // 前フレームの親指の座標の差を保存

        private bool wasTriggeredWithMiddle = false;
        private bool wasTriggeredWithLittle = false; // 新しい変数

        private void Start()
        {
            lastThumbDifferenceVector = Vector3.zero;
        }
        private void Update()
        {
            // thumbの2つの点の間のベクトルを計算
            Vector3 thumbDifferenceVector = handAnimator.thumbTipSphere.transform.position - handAnimator.thumbSecondJointSphere.transform.position;
            if (thumbDifferenceVector.x > triggerDifferenceThumb)
            {
                // Debug.Log("thumbDifferenceVector.x: " + thumbDifferenceVector.x);
                handAnimator.isTriggeredwithThumb = true;
                // Debug.Log("thumbDifferenceVector.z: " + thumbDifferenceVector.z);
            }
            else
            {
                handAnimator.isTriggeredwithThumb = false;
            }

            // Vector3 thumbDifferenceVector = handAnimator.thumbTipSphere.transform.position - handAnimator.indexTipSphere.transform.position;
            // if (thumbDifferenceVector.x < triggerDifferenceThumb)
            // {
            //     Debug.Log("thumb" + thumbDifferenceVector.x);
            //     handAnimator.isTriggeredwithThumb = true;
            // }
            // else
            // {
            //     handAnimator.isTriggeredwithThumb = false;
            // }

            // middleの2つの点の間のベクトルを計算
            Vector3 middleDifferenceVector = handAnimator.middleTipSphere.transform.position - handAnimator.middleForthJointSphere.transform.position;
            if (middleDifferenceVector.y < triggerYDifferenceMiddle && !wasTriggeredWithMiddle && !handAnimator.isTriggeredwithThumb && !wasTriggeredWithLittle)
            {
                Debug.Log("middle" + middleDifferenceVector.y);
                SpaceGesture();
                wasTriggeredWithMiddle = true;
            }
            else if (middleDifferenceVector.y >= triggerYDifferenceMiddle)
            {
                wasTriggeredWithMiddle = false;
            }

            // littleの2つの点の間のベクトルを計算
            Vector3 littleDifferenceVector = handAnimator.littleTipSphere.transform.position - handAnimator.littleForthJointSphere.transform.position;
            if (littleDifferenceVector.y < triggerYDifferenceLittle && !wasTriggeredWithLittle && !handAnimator.isTriggeredwithThumb && !wasTriggeredWithMiddle)
            {
                Debug.Log("little" + littleDifferenceVector.y);  
                DeleteGesture();
                wasTriggeredWithLittle = true;
            }
            else if (littleDifferenceVector.y >= triggerYDifferenceLittle)
            {
                wasTriggeredWithLittle = false;
            }
        }

        private void SpaceGesture()
        {
            csvHandler.inputField.text += " ";
            csvHandler.SetTimeStamps();
        }

        private void DeleteGesture()
        {
            if (csvHandler.inputField.text.Length > 0)
            {
                csvHandler.inputField.text = csvHandler.inputField.text.Remove(csvHandler.inputField.text.Length-1); // 最後の文字を削除
                csvHandler.SetTimeStamps();
                csvHandler.deleteCount++;
                Debug.Log("Gesture Deleted! : " + csvHandler.deleteCount.ToString());
            }
        }
    }
}
