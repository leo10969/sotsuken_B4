using UnityEngine;
using TMPro;

namespace MediaPipe.HandPose
{
    public class ButtonController : MonoBehaviour
    {
        public TextMeshProUGUI modeButtonText; // Modeボタンのテキスト
        public TextMeshProUGUI transButtonText; // Transボタンのテキスト

        public ScriptController scriptController; // ScriptControllerの参照
        public HandAnimator handAnimator; // TransparencyToggleButtonの場合に必要
        

        private bool modeisOn = true; // 初期状態をONとする
        private bool transparencyisOn = true; // 初期状態をONとする

        // Mode Toggleの処理
        public void ToggleModeButton()
        {
            modeisOn = !modeisOn;
            modeButtonText.text = modeisOn ? "HandTracking ON" : "HandTracking OFF";
            scriptController.ToggleMode();
        }

        // Transparency Toggleの処理
        public void ToggleTransparencyButton()
        {
            transparencyisOn = !transparencyisOn;
            transButtonText.text = transparencyisOn ? "Transparent ON" : "Transparent OFF";

            handAnimator.ToggleTransparency();
        }
    }
}
