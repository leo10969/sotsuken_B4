using System.Collections.Generic;
using UnityEngine;

namespace MediaPipe.HandPose
{

    public class KeyboardController : MonoBehaviour
    {
        public GameObject keyPrefab;
        public GameObject specialKeyPrefab;
        public GameObject keyboardBasePrefab;
        public GameObject backgroundPrefab;
        public GameObject specialBackgroundPrefab;

        public float keySpacing;
        public float rowSpacing;

        public float customSpacing1;
        public float customSpacing2;

        // Update the keys list to reflect the new layout
        private List<string> keys = new List<string>
        {
        "QWERTYUIOP",
        "ASDFGHJKL",
        "ZXCVBNM",
        ""
        };

        // Update the specialKeys list to reflect the new layout
        private List<List<string>> specialKeys = new List<List<string>>
        {
        new List<string> {"Delete"}, // no special keys in the first row
        new List<string> {}, // no special keys in the second row
        new List<string> {"Capslock"}, // CapsLock at the beginning of the third row
        new List<string> {"Shift", "Shift"}, // Shift at the beginning and end of the fourth row
        new List<string> {"Space", "Enter"} // Space in the fifth row
        };

        public Vector2 specialKeySize = new Vector2(3.0f, 1.0f); 

        void Start()
        {
            CreateKeyboard();
        }

        void CreateKeyboard()
        {
            for (int row = 0; row < keys.Count; row++)
            {
                for (int i = 0; i < keys[row].Length; i++)
                {
                    CreateKey(keys[row][i].ToString(), row, false, i.ToString());
                }
            }

            // // Special keys are placed separately to handle exceptions
            // CreateKey(specialKeys[2][0], 2, true, -1); // CapsLock at the left of A
            // CreateKey(specialKeys[3][0], 3, true, -1); // Left Shift at the left of Z
            // CreateKey(specialKeys[3][1], 3, true, keys[3].Length); // Right Shift at the right of M
            CreateKey(specialKeys[4][0], 4, true, "Space"); // Space at the center of the last row
            CreateKey(specialKeys[0][0], 4, true, "Delete"); // Delete at the center of the last row
            // CreateKey(specialKeys[4][1], 4, true, "DeleteAll"); // Delete All at the center of the last row
            CreateKey(specialKeys[4][1], 4, true, "Enter"); // Enter at the center of the last row
        }

        void CreateKey(string label, int row, bool isSpecial, string keyString = "")
        {
            Vector3 basePosition = keyboardBasePrefab.transform.position;
            float keyWidth = (isSpecial && keyString == "Space") ? specialKeySize.x : keyPrefab.transform.localScale.x;
            float keyHeight = keyPrefab.transform.localScale.y; // same height for all keys

            float totalWidth = (keys[0].Length - 1) * (keyWidth + keySpacing); // assuming the first row has the maximum number of keys
            float totalHeight = (keys.Count - 1) * (keyHeight + rowSpacing);

            float xPos;

            if (isSpecial && keyString == "Space")
            {
                xPos = basePosition.x;
            }
            else if (isSpecial && keyString == "Delete")
            {
                xPos = basePosition.x - totalWidth/3.8f;
            }
            else if (isSpecial && keyString == "Enter")
            {
                xPos = basePosition.x + keySpacing*1.3f + (keys[1].Length + 3) * (keySpacing) - totalWidth / 1.3f;
            }
            else
            {
                int column = int.TryParse(keyString, out int result) ? result : 0;
                xPos = basePosition.x + (column + 2 + row * 0.5f) * (keySpacing) - totalWidth / 2;
            }

            float yPos;

            if (isSpecial && keyString == "Delete")
            {
                yPos = basePosition.y - rowSpacing*1.3f - (row+0.1f) * (keySpacing*1.6f) + totalHeight/1.5f;
            } 
            else if (isSpecial && keyString == "Enter")
            {
                yPos = basePosition.y - rowSpacing*1.3f - (row+0.1f) * (keySpacing*1.6f) + totalHeight/1.5f;
            }
            else if (isSpecial && keyString == "Space")
            {
                yPos = basePosition.y - rowSpacing*1.3f - (row-1) * (keySpacing*1.6f) + totalHeight/1.5f;
            }
            else
            {
                yPos = basePosition.y - rowSpacing*customSpacing1 - row * (keySpacing*customSpacing2) + totalHeight/1.5f;
            }

            Vector3 position = new Vector3(xPos, yPos, basePosition.z - 0.001f);

            GameObject key = Instantiate(keyPrefab, position, Quaternion.identity);
            key.transform.parent = this.transform;
            key.name = label;
            key.GetComponentInChildren<TextMesh>().text = label.ToUpper();

            if (isSpecial)
            {
                BoxCollider collider = key.GetComponent<BoxCollider>();
                if (collider)
                {
                    Vector3 newSize = collider.size;
                    newSize.x *= 3.0f;
                    collider.size = newSize;
                }
            }
            key.tag = label;
        }

    }
}