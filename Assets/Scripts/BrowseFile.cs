using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using UnityEngine.Networking;

public class BrowseFile : MonoBehaviour, IPointerDownHandler
{
#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".txt", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Browse File", "", "txt", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    public ItemDatabase itemDatabase;
    IEnumerator OutputRoutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                //Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                if (fileType == FileType.item_db)
                    itemDatabase.m_item_db = webRequest.downloadHandler.text;
                else if (fileType == FileType.item_combo_db)
                    itemDatabase.m_item_combo_db = webRequest.downloadHandler.text;
                //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }

    public enum FileType { item_db, item_combo_db };
    public FileType fileType;
}
