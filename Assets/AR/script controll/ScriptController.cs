using UnityEngine;

namespace MediaPipe.HandPose
{
    public class ScriptController : MonoBehaviour
    {
        public CsvDataHandler csvHandler; // ModeToggleButtonの場合に必要
        public bool runHandAnimator = false;
        // public bool runTouchManager = false;

        public HandAnimator handAnimator;
        // public TouchManager touchManager;

        //モード切り替え
        public void ToggleMode()
        {
            runHandAnimator = !runHandAnimator;
            // runTouchManager = !runTouchManager;
            UpdateScripts();
            csvHandler.InputMode = handAnimator.enabled ? "HandTracking" : "Touch";
        }

        public void UpdateScripts()
        {
            if (handAnimator)
                handAnimator.enabled = runHandAnimator;
                handAnimator.UnenableHandObjects(runHandAnimator);

            // if (touchManager)
            //     touchManager.enabled = runTouchManager;
        }
    }

}