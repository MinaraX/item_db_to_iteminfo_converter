using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Start/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public TextAsset textAsset_item_db;
    public TextAsset textAsset_item_combo_db;
}
