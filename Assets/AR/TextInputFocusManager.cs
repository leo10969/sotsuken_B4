using UnityEngine;
using TMPro; // TextMeshProの名前空間を使用
using UnityEngine.EventSystems; // EventSystemの名前空間を使用

public class TextInputFocusManager : MonoBehaviour
{
    public TMP_InputField inputField; // TMP_InputFieldへの参照
    private EventSystem eventSystem; // EventSystemへの参照

    void Start()
    {
        // EventSystemの参照を取得
        eventSystem = EventSystem.current;

        // 初期状態で入力フィールドにフォーカスを設定
        FocusOnInputField();
    }

    public void FocusOnInputField()
    {
        // EventSystemを使用して入力フィールドにフォーカスを設定
        eventSystem.SetSelectedGameObject(inputField.gameObject, null);

        //iosビルトインキーボードを出さないようにする
        inputField.readOnly = true; // キーボードを表示させない
        // キャレットをアクティブにする
        inputField.ActivateInputField();

    }

    // テキストを更新し、キャレットの位置を調整する
    public void UpdateInputFieldTextAndCaret(string newText)
    {
        if (inputField != null)
        {
            // テキストを更新
            inputField.text = newText;

            // キャレットの位置をテキストの末尾に設定
            inputField.caretPosition = newText.Length;

            // キャレットを再度アクティブにする（必要に応じて）
            FocusOnInputField();
        }
    }
}
