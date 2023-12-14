using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MediaPipe.HandPose
{

    public class TypingScript : MonoBehaviour
    {
        [SerializeField] private string fileName = "phrases2.txt";//file path
        // Start is called before the first frame update
        public List<string> sentences;
        private int sentenceLength;
        public CsvDataHandler csvHandler;
        void Start()
        {
            LoadSentences();
            sentenceLength = sentences.Count;
            Debug.Log("sentences.Count:" + sentences.Count);
        }

        private void LoadSentences()
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);

            if(File.Exists(path))
            {
                sentences = new List<string>(File.ReadAllLines(path));
                Debug.Log(sentences[1]);
            } else
            {
                Debug.LogError("File not found" + path);
            }
        }
        public void ResetSentences()
        {
            sentences.Clear();
            LoadSentences();
        }

        public string DisplaySentence()
        {
            int index = Random.Range(0, sentences.Count);
            sentences.RemoveAt(index);
            return sentences[index];
            
        }

        public void ResetDisplaySentence()
        {
            csvHandler.exampleField.text = "";
        }
    }
}
