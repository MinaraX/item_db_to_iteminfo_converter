using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Start/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public TextAsset textAsset_item_db;
    public TextAsset textAsset_item_combo_db;
    string item_db;
    public string m_item_db
    {
        get { return item_db; }
        set
        {
            item_db = value;
            if (!string.IsNullOrEmpty(item_db))
                onItemDbChanged?.Invoke(false);
            else
                onItemDbChanged?.Invoke(true);
        }
    }
    string item_combo_db;
    public string m_item_combo_db
    {
        get { return item_combo_db; }
        set
        {
            item_combo_db = value;
            if (!string.IsNullOrEmpty(item_combo_db))
                onItemComboDbChanged?.Invoke(false);
            else
                onItemComboDbChanged?.Invoke(true);
        }
    }

    public delegate void OnItemDbChanged(bool isNull);
    public static event OnItemDbChanged onItemDbChanged;
    public static event OnItemDbChanged onItemComboDbChanged;

    public void Initialize()
    {
        if (textAsset_item_db)
            m_item_db = textAsset_item_db.text;
        if (textAsset_item_combo_db)
            m_item_combo_db = textAsset_item_combo_db.text;
    }
}
