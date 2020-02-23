using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using System.Text;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Start/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public TextAsset textAsset_item_db;
    public TextAsset textAsset_item_combo_db;
    public TextAsset textAsset_resourceNames;
    public TextAsset textAsset_skillNames;
    public TextAsset textAsset_mob_db;
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
    string resourceNames;
    public string m_resourceNames
    {
        get
        {
            return resourceNames;
        }
        set
        {
            resourceNames = value;
            if (!string.IsNullOrEmpty(resourceNames))
                onResourceNamesChanged?.Invoke(false);
            else
                onResourceNamesChanged?.Invoke(true);
        }
    }
    string skillNames;
    public string m_skillNames
    {
        get
        {
            return skillNames;
        }
        set
        {
            skillNames = value;
            if (!string.IsNullOrEmpty(skillNames))
                onSkillNamesChanged?.Invoke(false);
            else
                onSkillNamesChanged?.Invoke(true);
        }
    }
    string mob_db;
    public string m_mob_db
    {
        get
        {
            return mob_db;
        }
        set
        {
            mob_db = value;
            if (!string.IsNullOrEmpty(mob_db))
                onMobDbChanged?.Invoke(false);
            else
                onMobDbChanged?.Invoke(true);
        }
    }

    public delegate void OnItemDbChanged(bool isNull);
    public static event OnItemDbChanged onItemDbChanged;
    public static event OnItemDbChanged onItemComboDbChanged;
    public static event OnItemDbChanged onResourceNamesChanged;
    public static event OnItemDbChanged onSkillNamesChanged;
    public static event OnItemDbChanged onMobDbChanged;

    public void Initialize()
    {
        item_db = null;
        item_combo_db = null;
        resourceNames = null;
        skillNames = null;
        mob_db = null;

        if (textAsset_item_db)
            m_item_db = textAsset_item_db.text;
        if (textAsset_item_combo_db)
            m_item_combo_db = textAsset_item_combo_db.text;
        if (textAsset_resourceNames)
            m_resourceNames = textAsset_resourceNames.text;
        if (textAsset_skillNames)
            m_skillNames = textAsset_skillNames.text;
        if (textAsset_mob_db)
            m_mob_db = textAsset_mob_db.text;
    }
}
