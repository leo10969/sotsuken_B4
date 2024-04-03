using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeKeyboard : MonoBehaviour
{
    public Transform bigKeyboardTransform;
    public Transform smallKeyboardTransform;
    public string KeyboardSize 
    {   
        get{ return keyboardSizeText;}
        set{ keyboardSizeText = value;}
    }
    private string keyboardSizeText = "Big";
    
    public TextMeshProUGUI keyChangeText;
    // Change Keyboard
    public void changeKeyboard()
    {
        // Change Keyboard big to small
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
            KeyboardSize = "Small";
        }
        // Change keyboard small to big
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
            KeyboardSize = "Big";
        }
    }

}
