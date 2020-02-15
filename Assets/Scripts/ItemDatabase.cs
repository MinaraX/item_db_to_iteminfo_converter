using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Start/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public TextAsset textAsset_item_db;
    public TextAsset textAsset_item_combo_db;
    public TextAsset textAsset_itemInfo;
    public TextAsset textAsset_resourceNames;
    string item_db;
    public string m_item_db
    {
        get
        {
            return item_db;
        }
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
        get
        {
            return item_combo_db;
        }
        set
        {
            item_combo_db = value;
            if (!string.IsNullOrEmpty(item_combo_db))
                onItemComboDbChanged?.Invoke(false);
            else
                onItemComboDbChanged?.Invoke(true);
        }
    }
    string itemInfo;
    public string m_itemInfo
    {
        get
        {
            return itemInfo;
        }
        set
        {
            itemInfo = value;
            if (!string.IsNullOrEmpty(itemInfo))
                onItemInfoChanged?.Invoke(false);
            else
                onItemInfoChanged?.Invoke(true);
        }
    }
    //string resourceNames;
    public string m_resourceNames
    {
        get
        {
            return textAsset_resourceNames.text;
        }
    }

    public delegate void OnItemDbChanged(bool isNull);
    public static event OnItemDbChanged onItemDbChanged;
    public static event OnItemDbChanged onItemComboDbChanged;
    public static event OnItemDbChanged onItemInfoChanged;

    public void Initialize()
    {
        if (textAsset_item_db)
            m_item_db = textAsset_item_db.text;
        if (textAsset_item_combo_db)
            m_item_combo_db = textAsset_item_combo_db.text;
        if (textAsset_itemInfo)
            m_itemInfo = textAsset_itemInfo.text;
    }
}
