using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MediaPipe.HandPose
{
    public class CsvDataHandler : MonoBehaviour
    {
        public ExportCsvScript exportCsvScript;
        public TypingScript typingScript;
        public HandAnimator handAnimator;
        public TouchManager touchManager;
        public string InputMode { get; set; }
        public TextMeshProUGUI exampleField; // ExampleTextUIへの参照
        public TMP_InputField inputField; // InputTextUIへの参照
        private string testId; // Inspectorから設定するためのtest_id
        public string TestId
        {
            get { return testId; }
            set { testId = value; }
        }

        public List<float> TimeStamps = new List<float>();
        public List<float> PhraseTimes = new List<float>();
        //フレーズセットのテキストをデータに保存
        public List<string> PhraseTexts = new List<string>();
        public List<int> PhraseLength = new List<int>();
        //入力した文字列もデータに保存
        public List<string> InputTexts = new List<string>();
        public List<int> InputLength = new List<int>();
        public List<int> DeleteCounts = new List<int>();
        private float duration;
        public int deleteCount = 0; // 削除回数をカウントするための変数,KeyControllerから参照される
        public bool firstEnter = false;
        private bool isPractice = false;
        private float onePhraseTime = 0f;

        //2回目以降にEnterがおされたときの処理
        public void ProcessData()
        {
            handAnimator.framecount = 0;
            // InputMode = handAnimator.enabled ? "HandTracking" : "Touch";
            //セッション中
            if(PhraseLength.Count != exportCsvScript.numPhrases) 
            {
                exampleField.text = typingScript.DisplaySentence();

                if(PhraseLength.Count != 0)
                {
                    Debug.Log("count:"+TimeStamps.Count.ToString());
                    onePhraseTime = TimeStamps[TimeStamps.Count-1] - TimeStamps[0];
                    Debug.Log("onePhraseTime: " + onePhraseTime.ToString());
                    InputLength.Add(inputField.text.Length);
                    InputTexts.Add(inputField.text);
                    DeleteCounts.Add(deleteCount);
                    PhraseTimes.Add(onePhraseTime);
                    Debug.Log("length: " + PhraseLength.Count.ToString());
                }
                //全てのフレーズ（numPhrases分）を入力し終えたら，フレーズを表示しない
                PhraseLength.Add(exampleField.text.Length);
                PhraseTexts.Add(exampleField.text);
            }
            //1セッションが終了
            else if (PhraseLength.Count == exportCsvScript.numPhrases)
            {   

                onePhraseTime = TimeStamps[TimeStamps.Count-1] - TimeStamps[0];
                InputLength.Add(inputField.text.Length);
                PhraseTimes.Add(onePhraseTime);
                InputTexts.Add(inputField.text);
                DeleteCounts.Add(deleteCount);

                exampleField.text = "";
                // ここでCSVへの保存処理を行う
                string[] dataToSave = GenerateDataToSave();
                // 保存メソッドを呼び出してデータをCSVに保存する
                exportCsvScript.SaveToCSV(dataToSave); 
                //1セッションが終了したので，フレーズを非表示にする
                Debug.Log("task finished!");
                print(onePhraseTime);
                // typingScript.ResetDisplaySentence();
                PhraseTexts.Clear();
                PhraseLength.Clear();
                InputTexts.Clear();
                InputLength.Clear();
                TimeStamps.Clear();
                PhraseTimes.Clear();
                DeleteCounts.Clear();
            }
            deleteCount = 0;
            onePhraseTime = 0f;
            inputField.text = "";
            inputField.text = "";
        }

        public string[] GenerateDataToSave()
        {
            List<string> dataToSave = new List<string>();
            dataToSave.Add(TestId);
            dataToSave.Add(InputMode);
            dataToSave.Add(isPractice.ToString());
            dataToSave.Add(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            int totalDeleteCount = 0;
            int totalPhraseLength  = 0;
            float totalTime = 0f;
            
            for (int i = 0; i < exportCsvScript.numPhrases; i++)
            {
                //ExportCsvScript.csの146行目から160行目と対応付ける
                if(i < PhraseLength.Count)
                {
                    dataToSave.Add(PhraseTexts[i]);
                    dataToSave.Add(PhraseLength[i].ToString());
                    dataToSave.Add(InputTexts[i]);
                    dataToSave.Add(InputLength[i].ToString());
                    Debug.Log(i.ToString());
                    dataToSave.Add(PhraseTimes[i].ToString());
                    dataToSave.Add(DeleteCounts[i].ToString());
                    totalPhraseLength += PhraseLength[i];
                    totalTime += PhraseTimes[i];
                    totalDeleteCount += DeleteCounts[i];
                }
                else
                {
                    dataToSave.Add("");
                    dataToSave.Add("0");

                    dataToSave.Add("");
                    dataToSave.Add("0");

                    dataToSave.Add("0");
                    dataToSave.Add("0");
                }
                dataToSave.Add("");// phraseグループの間に空白を追加
            }
            dataToSave.Add(totalPhraseLength.ToString());
            dataToSave.Add(totalTime.ToString());
            dataToSave.Add(totalDeleteCount.ToString());
            return dataToSave.ToArray();
        }

        public void StartTyping()
        {
            handAnimator.handAccumulatedTime = 0f; 
            touchManager.touchAccumulatedTime = 0f; 
        }

        public void SetTimeStamps()
        {
            Debug.Log("input mode" + InputMode);

            if(InputMode == "HandTracking")
            {
                TimeStamps.Add(handAnimator.handAccumulatedTime);
                Debug.Log("Stamped");
            }
            else if(InputMode == "Touch")
            {
                TimeStamps.Add(touchManager.touchAccumulatedTime);
            }
        }

        public void SetIsPractice()
        {
            if(isPractice) isPractice = false;
            else isPractice = true;
            Debug.Log("isPractice: " + isPractice.ToString());
        }
    }
}
