using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Text;

public class SaveFile : MonoBehaviour, IPointerDownHandler
{
    public Output output;

    string _data { get { return output.m_currentOutput; } }

    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        string path = Application.dataPath + "/Output/itemInfo_true.txt";
        File.WriteAllText(path, _data, Encoding.UTF8);
        PopUp.Instance.ShowPopUp("ไฟล์ถูก Save ไว้ที่ " + path);
    }
}
