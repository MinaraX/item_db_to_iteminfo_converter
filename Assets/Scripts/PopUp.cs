using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    public static PopUp Instance;
    void Awake()
    {
        Instance = this;
        btnOk.onClick.AddListener(OnOkayButtonTap);
    }
    void OnOkayButtonTap()
    {
        objPopUp.SetActive(false);
    }

    public GameObject objPopUp;
    public Text txtMsg;
    public Button btnOk;

    public void ShowPopUp(string txt)
    {
        txtMsg.text = txt;
        objPopUp.SetActive(true);
    }
}
