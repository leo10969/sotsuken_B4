using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MediaPipe.HandPose
{
    public class KeyController : MonoBehaviour
    {
        public TextMesh keyTextMesh;
        private Color originalTextColor;
        //実験用のデータを保存するためのクラス
        public CsvDataHandler csvHandler;

        private void Start()
        {
            keyTextMesh = GetComponent<TextMesh>();
            if (keyTextMesh)
            {
                originalTextColor = keyTextMesh.color;
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            
            if (other.gameObject.tag != "IndexRay") return;

            if (csvHandler.InputMode == "Touch" && keyTextMesh)
            {
                keyTextMesh.color = Color.blue;
            }

            switch (this.gameObject.tag)
            {
                case "Space":
                    csvHandler.inputField.text += " ";
                    csvHandler.SetTimeStamps();
                    break;
                case "Delete":
                    HandleDelete(csvHandler.inputField);
                    break;
                case "Delete All":
                    csvHandler.inputField.text = "";
                    // csvHandler.inputTextValue = "";
                    break;
                case "Enter":
                    //フレーズセットが表示されている&&入力されていない
                    if(csvHandler.exampleField.text.Length > 0 && csvHandler.inputField.text.Length == 0) 
                    {
                        break;
                    }
                    //フレーズセットが表示されていない or 表示されていて，入力されている
                    else
                    {
                        HandleEnter();
                        break;
                    }
                default:
                    HandleAlphabet(csvHandler.inputField);
                    break;
            }
            // Debug.Log("csvHandler.inputTextValue: " + csvHandler.inputTextValue);
        }

        private void HandleDelete(TMP_InputField inputField)
        {
            if (inputField.text.Length > 0)
            {
                csvHandler.deleteCount++;
                Debug.Log("Deleted! : " + csvHandler.deleteCount.ToString());
                inputField.text = inputField.text.Remove(inputField.text.Length - 1);
                // Debug.Log("csvHandler.inputTextValue: " + csvHandler.inputTextValue);
                csvHandler.SetTimeStamps();
            }
        }

        private void HandleEnter()
        {
            // Debug.Log("Entered!");
            //例文が表示されているとき（＝2回目以降の）にEnterを押したときにデータを保存する
            // if (csvHandler.firstEnter)
            // {
                csvHandler.ProcessData();
                csvHandler.firstEnter = false;
            // }
            //1番最初とセッション開始時にはEnterを押しても何もしない
            // if (!csvHandler.firstEnter) csvHandler.firstEnter = true;
        }

        private void HandleAlphabet(TMP_InputField inputField)
        {
            //1文字目が入力された時に計測開始
            if (inputField.text.Length == 0 && csvHandler.exampleField.text.Length > 0)
            {
                Debug.Log("StartTyping!");
                csvHandler.StartTyping();
            }
            csvHandler.SetTimeStamps();
            inputField.text += this.gameObject.name.ToLower();
            // Debug.Log("inputField.text: " + inputField.text);
            // Debug.Log("csvHandler.inputTextValue: " + csvHandler.inputTextValue);

        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "IndexRay")
            {
                if (keyTextMesh)
                {
                    keyTextMesh.color = Color.red;
                }
            }
        }
    }

}