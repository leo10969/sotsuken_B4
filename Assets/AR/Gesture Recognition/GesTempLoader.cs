using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GesTempLoader : MonoBehaviour
{
    // CSVファイルが保存されているフォルダのパス
    public string csvFolderPath = "Assets/StreamingAssets/Gesturetemp";

    void Start()
    {
        LoadAllCSVFromFolder(csvFolderPath);
    }

    void LoadAllCSVFromFolder(string folderPath)
    {
        // 指定したフォルダ内のすべてのCSVファイルを取得
        string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");

        // 各CSVファイルを順番に処理
        foreach (string csvFile in csvFiles)
        {
            LoadCSVFromFile(csvFile);
        }
    }

    void LoadCSVFromFile(string filePath)
    {
        // CSVファイルの内容を全て読み込む
        string[] csvLines = File.ReadAllLines(filePath);

        // CSVの各行を処理
        foreach (string line in csvLines)
        {
            // CSVの行をカンマで分割
            string[] values = line.Split(',');

            // ここで必要なデータ処理を行う
            // 例: 最初のカラムがID、次のカラムが名前だとする
            // string id = values[0];
            // string name = values[1];

            // デバッグログに読み込んだデータを表示（デバッグ用）
            Debug.Log("Loaded CSV line from " + filePath + ": " + line);
        }
    }
}
