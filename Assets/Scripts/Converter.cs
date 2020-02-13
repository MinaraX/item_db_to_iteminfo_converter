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
    public Text txtCurrentItemDb;
    public Text txtCurrentItemComboDb;

    public Button btnConvert;
    #endregion

    #region delegate subscription
    void Awake()
    {
        ItemDatabase.onItemDbChanged += ItemDatabase_onItemDbChanged;
        ItemDatabase.onItemComboDbChanged += ItemDatabase_onItemComboDbChanged;
    }
    void OnDestroy()
    {
        ItemDatabase.onItemDbChanged -= ItemDatabase_onItemDbChanged;
        ItemDatabase.onItemComboDbChanged -= ItemDatabase_onItemComboDbChanged;
    }
    void ItemDatabase_onItemDbChanged(bool isNull)
    {
        if (isNull)
            txtCurrentItemDb.text = "item_db: Not ready";
        else
            txtCurrentItemDb.text = "item_db: Ready";
    }
    void ItemDatabase_onItemComboDbChanged(bool isNull)
    {
        if (isNull)
            txtCurrentItemComboDb.text = "item_combo_db: Not ready";
        else
            txtCurrentItemComboDb.text = "item_combo_db: Ready";
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
        //Start process
        Log("Converter: Start");

        output.currentOutput = null;

        Log("Converter >> is item_db null: " + string.IsNullOrEmpty(itemDatabase.m_item_db));

        //Parsing item_db to List
        Log("Converter: Parsing item_db to list");
        output.lines = StringToList(itemDatabase.m_item_db);

        //Remove comment from List
        Log("Converter: Remove comment from list");
        for (int i = output.lines.Count - 1; i >= 0; i--)
        {
            if (output.lines[i].Contains("//"))
                output.lines.RemoveAt(i);
        }

        //Remove empty from List
        Log("Converter: Remove empty from list");
        for (int i = output.lines.Count - 1; i >= 0; i--)
        {
            if (string.IsNullOrEmpty(output.lines[i]) || string.IsNullOrWhiteSpace(output.lines[i]))
                output.lines.RemoveAt(i);
        }

        Log("Converter >> is item_combo_db null: " + string.IsNullOrEmpty(itemDatabase.m_item_combo_db));

        //Do nothing for now

        //Convert here
        for (int i = 0; i < output.lines.Count; i++)
            output.ConvertSpecificArrayToItemInfo(i);
        /*{
            Debug.Log("Converter >> Convert index: " + i);
            output.ConvertSpecificArrayToItemInfo(i);
        }*/

        //Finished
        Log("Converter: Done");
    }

    public int targetLines;
    [Button]
    public void ViewAtTargetLines()
    {
        Log(output.lines[targetLines]);
    }

    void Log(object obj)
    {
        Debug.Log(obj);
    }

    List<string> TextAssetToList(TextAsset textAsset)
    {
        return new List<string>(textAsset.text.Split('\n'));
    }
    List<string> StringToList(string data)
    {
        return new List<string>(data.Split('\n'));
    }
}
