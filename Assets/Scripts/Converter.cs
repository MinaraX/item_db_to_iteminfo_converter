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

        //Parsing item_db to List
        output.lines = TextAssetToList(itemDatabase.textAsset_item_db);

        //Remove comment from List
        for (int i = output.lines.Count - 1; i >= 0; i--)
        {
            if (output.lines[i].Contains("//"))
            {
                output.lines.RemoveAt(i);
            }
        }

        Log("textAsset_item_combo_db: " + itemDatabase.textAsset_item_combo_db.name);

        Log("Convert finished");

        output.currentOutput = "Finished";
    }

    void Log(object obj)
    {
        Debug.Log(obj);
    }

    List<string> TextAssetToList(TextAsset textAsset)
    {
        return new List<string>(textAsset.text.Split('\n'));
    }
}
