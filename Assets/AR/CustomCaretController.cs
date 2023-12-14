using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MediaPipe.HandPose
{
    public class CustomCaretController : MonoBehaviour
    {
        public TMP_InputField inputField;
        public RectTransform caretTransform;
        public Image caretImage; // キャレットのImageコンポーネントへの参照
        public float blinkRate = 0.5f;
        private float nextBlink;
        private Vector2 lastCaretPosition; // 最後のキャレットの位置
        private float lastCharWidth; // 最後に追加または削除された文字の幅
        public float fixedSpaceWidth = 20f; // 空文字の幅
        private Vector2 initialPosition = new Vector2(-370, 805);
        private float textSizeOffset = 20f; // テキストサイズから引くオフセット値

        private void Update()
        {
            // キャレットの点滅アニメーション
            if (Time.time >= nextBlink)
            {
                caretImage.enabled = !caretImage.enabled; // Imageコンポーネントの有効/無効を切り替える
                nextBlink = Time.time + blinkRate;
            }
        }

        // キャレット位置を更新するメソッド
        public void UpdateCaretPosition()
        {
            string currentText = inputField.text;

            // テキストが空の場合は初期位置に設定
            if (string.IsNullOrEmpty(currentText))
            {
                caretTransform.anchoredPosition = initialPosition;
                return;
            }
            // テキスト全体の幅を取得
            Vector2 textSize = inputField.textComponent.GetPreferredValues(currentText);

            // キャレットの新しい位置を計算
            Vector2 caretPosition = initialPosition + new Vector2(textSize.x - textSizeOffset, 0);
            caretTransform.anchoredPosition = caretPosition;
        }

        // キャレット位置を初期位置にリセットするメソッド
        public void ResetCaretPosition()
        {
            caretTransform.anchoredPosition = initialPosition;
        }

        public void UpdateCaretPositionForSpace()
        {
            Vector2 caretPosition = caretTransform.anchoredPosition;
            caretPosition.x += fixedSpaceWidth; // 空文字の幅だけキャレットを右に移動
            caretTransform.anchoredPosition = caretPosition;
        }

        public void MoveCaretToPreviousPosition()
        {
            string currentText = inputField.text;

            // テキストが1文字の場合は初期位置に設定
            if (currentText.Length == 1)
            {
                caretTransform.anchoredPosition = initialPosition;
                return;
            }

            // 最後の文字が空文字であるかどうかをチェック
            bool isLastCharSpace = string.IsNullOrWhiteSpace(currentText.Substring(currentText.Length - 1));
            string textWithoutLastChar = currentText.Substring(0, currentText.Length - 1);
            Vector2 textSize = inputField.textComponent.GetPreferredValues(textWithoutLastChar);

            // 最後の文字が空文字の場合は、固定の空文字の幅を追加
            if (isLastCharSpace)
            {
                textSize.x += fixedSpaceWidth;
            }
            Vector2 caretPosition = initialPosition + new Vector2(textSize.x, 0);
            caretTransform.anchoredPosition = caretPosition;
        }
    }
}
