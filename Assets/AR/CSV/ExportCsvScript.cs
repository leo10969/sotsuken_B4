using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MediaPipe.HandPose
{
    public class ExportCsvScript : MonoBehaviour
    {
        public enum Mode
        {
            Experiment,
            Debug
        }

        public Mode currentMode; // モードの選択用

        private List<string[]> csvDatas = new List<string[]>();
        public string lastpath;
        public HandAnimator handAnimator;
        private HandAnimator.HeaderType currentHeaderType;
        private HandAnimator.FilterType currentFilterType;
        
        public int numPhrases; // Number of phrases to type
        public Text idDisplayText; // IDを表示するためのUIテキストオブジェクトの参照

        private void Start()
        {
            currentHeaderType = handAnimator.currentHeaderType;
            currentFilterType = handAnimator.currentFilterType;

            UpdateIdDisplay(); // UIテキストの内容を更新

            switch (currentMode)
            {
                case Mode.Experiment:
                    InitializeCSV();
                    break;
                case Mode.Debug:
                    DeleteAllFilesInPersistentDataPath();
                    InitializeCSV();
                    break;
            }
        }

        private void InitializeCSV()
        {
            string path = GetPath();
            // Debug.Log("path:" + path);

            if (!CheckExistCSV(path))
            {
                string[] header = GetHeaderBasedOnType();
                OverWriteCSV(header, new string[] { }, path);
            }
        }

        public void SaveToCSV(string[] data)
        {
            InitializeCSV();
            AppendCSV(data, GetPath());
        }

        private string GetPath()
        {
            // ファイル名に追加するサフィックスを初期化
            string filterSuffix = "";

            if (currentMode == Mode.Debug)
            {
                // DebugモードのときのみHeaderTypeがFilterかどうかをチェックする
                if (currentHeaderType == HandAnimator.HeaderType.Filter)
                {
                    switch (currentFilterType)
                    {
                        case HandAnimator.FilterType.Kalman:
                            // processNoiseを文字列化し、ファイル名に追加
                            // filterSuffix = "_K" + handAnimator.processNoise.ToString("F2");
                            break;
                        case HandAnimator.FilterType.EMA:
                            // EMAの場合は何も追加しない
                            break;
                        case HandAnimator.FilterType.OneEuro:
                            // betaを文字列化し、ファイル名に追加
                            filterSuffix = "_O" + handAnimator.beta.ToString("F2");
                            break;
                    }
                }
                string baseFileName;
                // Debugモード用のファイル名を生成
                if (string.IsNullOrEmpty(lastpath))
                { 
                    baseFileName = "/Debug_" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                else 
                {
                    baseFileName = "/Debug_" + lastpath;
                }
                return UnityEngine.Application.persistentDataPath + baseFileName + filterSuffix + ".csv";
            }
            else if (currentMode == Mode.Experiment)
            {
                string baseFileName;
                // Experimentモード用のファイル名を生成
                if (string.IsNullOrEmpty(lastpath))
                {
                    baseFileName = "/Study_" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                else
                {
                    baseFileName = "/Study_" + lastpath;
                }
                return UnityEngine.Application.persistentDataPath + baseFileName + ".csv";
            }

            // 他のモードの場合には空の文字列を返すか、別途処理を追加
            return "";
        }


        private string[] GetHeaderBasedOnType()
        {
            
            switch (currentHeaderType)
            {
                case HandAnimator.HeaderType.PhraseSetTest:
                    return GeneratePhraseHeaders();
                case HandAnimator.HeaderType.Filter:
                    return GenerateFilterHeaders();
                case HandAnimator.HeaderType.GestureStroke:
                    return new string[] { "gesturestroke_x", "gesturestroke_y", "gesturestroke_z" };
                default:
                    return new string[] { };
            }
        }

        //UIテキストの内容を更新
        private void UpdateIdDisplay()
        {
            if (idDisplayText != null)
            {
                switch (currentMode)
                {
                    case Mode.Experiment:
                        idDisplayText.text = "Exp_ID: ";
                        break;
                    case Mode.Debug:
                        idDisplayText.text = "Debug_ID: ";
                        break;
                }
            }
        }

        private string[] GeneratePhraseHeaders()
        {
            List<string> headers = new List<string> { "test_id", "input mode", "practice", "keyb_size", "test_date" };
            // Debug.Log("numPhrases:" + numPhrases);
            for (int i = 1; i <= numPhrases; i++)
            {
                //CsvDataHandler.csの113行目から118行目と対応付ける
                headers.Add($"phrase{i}_string");
                headers.Add($"phrase{i}_length");
                
                headers.Add($"input{i}_string");
                headers.Add($"input{i}_length");
                
                headers.Add($"phrase{i}_time");
                headers.Add($"phrase{i}_delete");
                headers.Add(""); // phraseグループの間に空白を追加
            }

            headers.Add("total_phraselength");
            headers.Add("total_time");
            headers.Add("total_delete");


            return headers.ToArray();
        }

        private string[] GenerateFilterHeaders()
        {
            List<string> headers = new List<string> { "frame" };

            foreach (var axis in new string[] { "x", "y", "z" })
            {
                headers.Add(axis);
                switch (currentFilterType)
                {
                    case HandAnimator.FilterType.Kalman:

                        // headers.AddRange(GenerateHeadersForParameters(axis, "kl", handAnimator.kalmanParameters));
                        break;
                    case HandAnimator.FilterType.EMA:
                        // headers.AddRange(GenerateHeadersForParameters(axis, "ema", handAnimator.emaParameters));
                        break;
                    case HandAnimator.FilterType.OneEuro:
                        headers.AddRange(GenerateHeadersForParameters(axis, "oe", new List<float> { handAnimator.beta,handAnimator.fcmin }));
                        break;
                }
                headers.Add(""); // x, y, zグループの間に空白を追加
            }

            return headers.ToArray();
        }


    private IEnumerable<string> GenerateHeadersForParameters(string axis, string filterName, List<float> parameters)
    {
        foreach (var param in parameters)
        {
            yield return $"{axis}_{filterName}_{param:f4}";
        }
    }

        //Start時にディレクトリ内のファイルをすべて削除する
        public void DeleteAllFilesInPersistentDataPath()
        {
            string directoryPath = Application.persistentDataPath;
            string[] files = Directory.GetFiles(directoryPath);

            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log("File deleted: " + file);
            }
        }

        // lastpath内の数字の文字列を更新する関数
        public void UpdateLastPathNumber()
        {
            // 正規表現を使用してlastpath内の数字を検出
            Match match = Regex.Match(lastpath, @"\d+");
            if (match.Success)
            {
                int number = int.Parse(match.Value); // 数字を整数に変換
                number++; // インクリメント
                lastpath = Regex.Replace(lastpath, @"\d+", number.ToString()); // lastpathを更新
            }
            else
            {
                Debug.LogWarning("lastpathに数字が含まれていません。");
            }
        }

        // ファイル書き出し(上書き）
        public void OverWriteCSV(string[] header, string[] data, string path)
        {
            
            StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8"));
            // Debug.Log("OverWriteCSV");
            string h = string.Join(",", header);
            sw.WriteLine(h);

            if (data.Length > 0)
            {
                string d = string.Join(",", data);
                sw.WriteLine(d);
            }

            sw.Close();
            Debug.Log(path);
        }

        // ファイル書き出し（追加）
        public void AppendCSV(string[] data, string path)
        {
            StreamWriter sw = new StreamWriter(path, true, Encoding.GetEncoding("UTF-8"));
            string d = string.Join(",", data);
            sw.WriteLine(d);
            sw.Close();
            Debug.Log(path);
        }

        //csvファイルの読み出し
        public List<string[]> ReadCSV(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                Debug.Log(line);
                csvDatas.Add(line.Split(','));
            }

            sr.Close();
            Debug.Log(path);

            return csvDatas;
        }

        //ファイルが存在するかを確認
        public bool CheckExistCSV(string path)
        {
            if (System.IO.File.Exists(path))
            {
                // Debug.Log("CSVファイルが存在するので追記します");
                return true;
            }
            else
            {
                Debug.Log("CSVファイルが存在しないので作成します");
                return false;
            }
        }    
    }

}