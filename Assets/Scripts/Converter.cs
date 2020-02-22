using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyButtons;

public class Converter : MonoBehaviour
{
    #region Variables
    [Header("Database")]
    public ItemDatabase itemDatabase;
    public Output output;

    [Header("UI")]
    public Text txtCurrent_item_db;
    public Text txtCurrent_item_combo_db;
    public Text txtCurrent_itemInfo;
    public GameObject objConvertInProgress;

    public Button btnConvert;
    #endregion

    #region delegate subscription
    void Awake()
    {
        ItemDatabase.onItemDbChanged += ItemDatabase_onItemDbChanged;
        ItemDatabase.onItemComboDbChanged += ItemDatabase_onItemComboDbChanged;
        ItemDatabase.onItemInfoChanged += ItemDatabase_onItemInfoChanged;
    }
    void OnDestroy()
    {
        ItemDatabase.onItemDbChanged -= ItemDatabase_onItemDbChanged;
        ItemDatabase.onItemComboDbChanged -= ItemDatabase_onItemComboDbChanged;
        ItemDatabase.onItemInfoChanged -= ItemDatabase_onItemInfoChanged;
    }
    void ItemDatabase_onItemDbChanged(bool isNull)
    {
        if (isNull)
            txtCurrent_item_db.text = "item_db: Not ready";
        else
            txtCurrent_item_db.text = "item_db: Ready";
    }
    void ItemDatabase_onItemComboDbChanged(bool isNull)
    {
        if (isNull)
            txtCurrent_item_combo_db.text = "item_combo_db: Not ready";
        else
            txtCurrent_item_combo_db.text = "item_combo_db: Ready";
    }
    void ItemDatabase_onItemInfoChanged(bool isNull)
    {
        if (isNull)
            txtCurrent_itemInfo.text = "itemInfo: Not ready";
        else
            txtCurrent_itemInfo.text = "itemInfo: Ready";
    }
    #endregion

    void Start()
    {
        itemDatabase.Initialize();

        btnConvert.onClick.AddListener(Convert);
    }

    [Button]
    public void Convert()
    {
        if (convertProcess != null)
            StopCoroutine(convertProcess);
        convertProcess = ConvertProcess();
        StartCoroutine(convertProcess);
    }

    IEnumerator convertProcess;
    IEnumerator ConvertProcess()
    {
        objConvertInProgress.SetActive(true);

        yield return null;

        //Start process
        Log("Converter: Start");

        output.ParseItemDatabase();

        //Convert here
        for (int i = 0; i < output.m_lines.Count; i++)
            output.ConvertSpecificArrayToItemInfo(i);

        //Finished
        Log("Converter: Done");

        yield return null;

        objConvertInProgress.SetActive(false);

        Log("output.m_lines.Count: " + output.m_lines.Count);
    }

    void Log(object obj)
    {
        Debug.Log(obj);
    }
}
