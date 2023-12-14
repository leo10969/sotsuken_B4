using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MediaPipe.HandPose
{
    public class InputHandler : MonoBehaviour
    {
        public InputField inputField; // InputFieldの参照
        public Text displayText; // 入力テキストを表示するTextオブジェクトの参照
        public ExportCsvScript exportCsvScript; // ExportCsvScriptの参照
        public CsvDataHandler csvDataHandler; // CsvDataHandlerの参照
        public HandAnimator handAnimator; // HandAnimatorの参照

        private void Start()
        {
            // InputFieldのイベントに関数を追加
            inputField.onEndEdit.AddListener(OnInputEnd);
        }

        // InputFieldの入力が終了した時に呼ばれる
        private void OnInputEnd(string input)
        {
            // 入力テキストをTextオブジェクトに表示
            displayText.text += input;

            // 入力テキストをExportCsvScriptの変数に保存
            exportCsvScript.lastpath = input;
            csvDataHandler.TestId = input;
            Debug.Log("ファイルが作成されたので，フィルタリングテストのためのフレームカウントをリセットします．");
            handAnimator.framecount = 0;
        }
    }
}
