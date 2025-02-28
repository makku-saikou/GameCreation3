using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Hmxs.Toolkit
{
    public class TextHandler : OdinEditorWindow
    {
        [MenuItem("Tools/HmxsToolkit/TextHelper")]
        private static void OpenWindow() => GetWindow<TextHandler>().Show();

        [BoxGroup("Text Components Finder")]
        [TableList(IsReadOnly = true)]
        public List<MyTextMeshPro> textMeshProComponents = new();

        [BoxGroup("Text Components Finder")]
        [TableList(IsReadOnly = true)]
        public List<MyText> textComponents = new();


        [BoxGroup("Text Components Finder")]
        [Button(ButtonSizes.Medium, Name = "刷新")]
        public void ReFresh()
        {
            textComponents.Clear();
            textMeshProComponents.Clear();

            foreach (var rootObj in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                var texts = rootObj.GetComponentsInChildren<Text>(true);
                if (texts != null)
                    foreach (var text in texts)
                        textComponents.Add(new MyText(text));

                var tmps = rootObj.GetComponentsInChildren<TextMeshProUGUI>(true);
                if (tmps != null)
                    foreach (var tmp in tmps)
                        textMeshProComponents.Add(new MyTextMeshPro(tmp));
            }
        }

        [Space(15)]

        [InfoBox("导出当前场景中所有的文字资源")]
        [FolderPath, PropertyOrder(10)]
        [InlineButton("Export","导出"), DisableIf("@this.textComponents.Count == 0")]
        public string exportPath = Application.dataPath;

        private void Export()
        {
            var str = new StringBuilder();

            foreach (var text in textComponents)
                str.Append(text.textComponent.text);
            foreach (var text in textMeshProComponents)
                str.Append(text.textComponent.text);

            var stream =
                new StreamWriter(
                    $"{exportPath}\\{SceneManager.GetActiveScene().name} TextSource.txt",
                    false);
            stream.Write(str);
            stream.Close();
            Debug.Log($"{exportPath}\\{SceneManager.GetActiveScene().name} TextSource.txt 导出成功");
        }

        [Serializable]
        public struct MyTextMeshPro
        {
            [ReadOnly, VerticalGroup("Assets")]
            public TextMeshProUGUI textComponent;

            [OnValueChanged("Update"), VerticalGroup("Assets")]
            public TMP_FontAsset fontAsset;

            [ColorPalette, OnValueChanged("Update")]
            public Color fontColor;

            [TextArea, OnValueChanged("Update")]
            public string content;

            public MyTextMeshPro(TextMeshProUGUI component)
            {
                textComponent = component;
                fontAsset = textComponent.font;
                fontColor = component.color;
                content = textComponent.text;
            }

            public void Update()
            {
                if(textComponent == null) return;
                textComponent.text = content;
                if (fontAsset != null) textComponent.font = fontAsset;
                textComponent.color = fontColor;
            }
        }

        [Serializable]
        public struct MyText
        {
            [ReadOnly, VerticalGroup("Assets")]
            public Text textComponent;

            [OnValueChanged("Update"), VerticalGroup("Assets")]
            public Font fontAsset;

            [ColorPalette, OnValueChanged("Update")]
            public Color fontColor;

            [TextArea, OnValueChanged("Update")]
            public string content;

            public MyText(Text component)
            {
                textComponent = component;
                fontAsset = textComponent.font;
                fontColor = component.color;
                content = textComponent.text;
            }

            public void Update()
            {
                if(textComponent == null) return;
                textComponent.text = content;
                if (fontAsset != null) textComponent.font = fontAsset;
                textComponent.color = fontColor;
            }
        }
    }
}