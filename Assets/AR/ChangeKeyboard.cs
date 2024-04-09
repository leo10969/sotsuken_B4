using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeKeyboard : MonoBehaviour
{
    public Transform bigKeyboardTransform;
    public Transform smallKeyboardTransform;
<<<<<<< HEAD
    public string KeyboardSize 
    {   
        get{ return keyboardSizeText;}
        set{ keyboardSizeText = value;}
    }
    private string keyboardSizeText = "Big";
=======
>>>>>>> 3d8ed57 (gesture-keyboard)
    
    public TextMeshProUGUI keyChangeText;
    // Change Keyboard
    public void changeKeyboard()
    {
<<<<<<< HEAD
        // Change Keyboard big to small
=======
>>>>>>> 3d8ed57 (gesture-keyboard)
        if (bigKeyboardTransform.gameObject.activeSelf)
        {
            bigKeyboardTransform.gameObject.SetActive(false);
            foreach (Transform child in bigKeyboardTransform)
            {
                child.gameObject.SetActive(false);
            }
            smallKeyboardTransform.gameObject.SetActive(true);
            foreach (Transform child in smallKeyboardTransform)
            {
                child.gameObject.SetActive(true);
            }
            keyChangeText.text = "Small";
<<<<<<< HEAD
            KeyboardSize = "Small";
        }
        // Change keyboard small to big
=======
        }
>>>>>>> 3d8ed57 (gesture-keyboard)
        else
        {
            bigKeyboardTransform.gameObject.SetActive(true);
            foreach (Transform child in bigKeyboardTransform)
            {
                child.gameObject.SetActive(true);
            }
            smallKeyboardTransform.gameObject.SetActive(false);
            foreach (Transform child in smallKeyboardTransform)
            {
                child.gameObject.SetActive(false);
            }
            keyChangeText.text = "Big";
<<<<<<< HEAD
            KeyboardSize = "Big";
=======
>>>>>>> 3d8ed57 (gesture-keyboard)
        }
    }

}
