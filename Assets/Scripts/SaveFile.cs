using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

public class SaveFile : MonoBehaviour, IPointerDownHandler
{
    public Output output;

    string _data
    {
        get
        {
            if (!string.IsNullOrEmpty(output.m_currentOutput))
                Debug.Log("output.m_currentOutput.Length: " + output.m_currentOutput.Length);
            return output.m_currentOutput;
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        var bytes = Encoding.UTF8.GetBytes(_data);
        DownloadFile(gameObject.name, "OnFileDownload", "sample.txt", bytes, bytes.Length);
    }

    // Called from browser
    public void OnFileDownload() {
        output.text = "File Successfully Downloaded";
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    // Listen OnClick event in standlone builds
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Saving to", "", "itemInfo_Sak", "txt");
        if (!string.IsNullOrEmpty(path))
            File.WriteAllText(path, _data);
    }
#endif
}
