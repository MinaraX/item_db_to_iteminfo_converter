using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class Converter : MonoBehaviour
{
    #region Variables
    public ItemDatabase itemDatabase;
    public Output output;
    #endregion

    void Start()
    {
        output.currentOutput = "Ready";
    }

    [Button]
    public void Convert()
    {
        Log("Convert start");

        output.currentOutput = null;

        Log("textAsset_item_db: " + itemDatabase.textAsset_item_db.name);

        output.lines = itemDatabase.textAsset_item_db.text.Split("\n"[0]);

        Log("textAsset_item_combo_db: " + itemDatabase.textAsset_item_combo_db.name);

        Log("Convert finished");

        output.currentOutput = "Finished";
    }

    void Log(object obj)
    {
        Debug.Log(obj);
    }
}
