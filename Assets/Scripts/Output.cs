using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using System;

[CreateAssetMenu(fileName = "Output", menuName = "Start/Output")]
public class Output : ScriptableObject
{
    public ItemDatabase itemDatabase;

    #region Clear All Database
    [Button]
    public void ClearAll()
    {
        targetArray = 0;
        m_currentOutput = null;
        currentItemDbData = new List<string>();
        currentItemDb = new ItemDb();
        m_lines = new List<string>();
        m_lines_resourceNames = new List<string>();
        m_lines_SkillName = new List<string>();
        m_lines_MonsterDatabase = new List<string>();
        m_currentItemDbs = new List<ItemDb>();
        m_currentItemWithoutScriptDbs = new List<ItemDb>();
        m_currentItemScriptDatas = new List<ItemDbScriptData>();
        m_currentResourceNames = new List<ItemResourceName>();
        m_currentSkillNames = new List<SkillName>();
        m_currentMonsterDatabases = new List<MonsterDatabase>();
        Log("Clear all");
    }
    #endregion

    #region Parse text asset
    [Button]
    public void ParseTextAsset()
    {
        itemDatabase.Initialize();
    }
    #endregion

    #region Parse item database
    [Button]
    public void ParseItemDatabase()
    {
        //item_db
        m_currentOutput = null;

        Log("Converter >> is item_db null: " + string.IsNullOrEmpty(itemDatabase.m_item_db));

        //Parsing item_db to List
        Log("Converter: Parsing item_db to list");
        m_lines = StringSplit.GetStringSplit(itemDatabase.m_item_db, '\n');

        //Remove comment from List
        Log("Converter: Remove comment from list");
        for (int i = m_lines.Count - 1; i >= 0; i--)
        {
            if (m_lines[i].Contains("//"))
                m_lines.RemoveAt(i);
        }

        //Remove empty from List
        Log("Converter: Remove empty from list");
        for (int i = m_lines.Count - 1; i >= 0; i--)
        {
            if (string.IsNullOrEmpty(m_lines[i]) || string.IsNullOrWhiteSpace(m_lines[i]))
                m_lines.RemoveAt(i);
        }

        //item_combo_db
        Log("Converter >> is item_combo_db null: " + string.IsNullOrEmpty(itemDatabase.m_item_combo_db));

        //Do nothing for now
    }
    #endregion

    #region Resource Name
    [Button]
    public void FetchResourceName()
    {
        Log("FetchResourceName: Start");
        m_currentResourceNames = new List<ItemResourceName>();
        FetchResourceName(itemDatabase.m_resourceNames);
        Log("FetchResourceName: Done");
    }
    void FetchResourceName(string data)
    {
        m_lines_resourceNames = new List<string>();
        Log("FetchResourceName >> Parsing txt to database start");
        m_lines_resourceNames = StringSplit.GetStringSplit(data, '\n');
        Log("FetchResourceName >> Parsing txt to database done");

        for (int i = 0; i < m_lines_resourceNames.Count; i++)
            Convert_resourceNames_ToList(m_lines_resourceNames[i]);
    }
    void Convert_resourceNames_ToList(string data)
    {
        Log(data);

        List<string> sumSplit = StringSplit.GetStringSplit(data, '=');

        ItemResourceName newCurrentResourceName = new ItemResourceName();
        newCurrentResourceName.id = int.Parse(sumSplit[0]);
        string sumResourceName = sumSplit[1];
        sumResourceName = sumResourceName.Substring(0, sumResourceName.Length - 1);
        newCurrentResourceName.resourceName = sumResourceName;

        m_currentResourceNames.Add(newCurrentResourceName);
    }
    #endregion

    #region Skill Name
    [Button]
    public void FetchSkillName()
    {
        Log("FetchSkillName: Start");
        m_currentSkillNames = new List<SkillName>();
        FetchSkillName(itemDatabase.m_skillNames);
        Log("FetchSkillName: Done");
    }
    void FetchSkillName(string data)
    {
        m_lines_SkillName = new List<string>();
        Log("FetchSkillName >> Parsing txt to database start");
        m_lines_SkillName = StringSplit.GetStringSplit(data, '\n');
        Log("FetchSkillName >> Parsing txt to database done");

        for (int i = 0; i < m_lines_SkillName.Count; i++)
            Convert_SkillName_ToList(m_lines_SkillName[i]);
    }
    void Convert_SkillName_ToList(string data)
    {
        Log(data);

        List<string> sumSplit = StringSplit.GetStringSplit(data, '=');

        SkillName newSkillName = new SkillName();
        newSkillName.id = int.Parse(sumSplit[0]);
        newSkillName.name = sumSplit[1];
        string sumDesc = sumSplit[2];
        sumDesc = sumDesc.Substring(0, sumDesc.Length - 1);
        newSkillName.desc = sumDesc;

        m_currentSkillNames.Add(newSkillName);
    }
    #endregion

    #region Monster Database
    [Button]
    public void FetchMonsterDatabase()
    {
        Log("FetchMonsterDatabase: Start");
        m_currentMonsterDatabases = new List<MonsterDatabase>();
        FetchMonsterDatabase(itemDatabase.m_mob_db);
        Log("FetchMonsterDatabase: Done");
    }
    void FetchMonsterDatabase(string data)
    {
        m_lines_MonsterDatabase = new List<string>();
        Log("FetchMonsterDatabase >> Parsing txt to database start");
        m_lines_MonsterDatabase = StringSplit.GetStringSplit(data, '\n');
        Log("FetchMonsterDatabase >> Parsing txt to database done");

        for (int i = 0; i < m_lines_MonsterDatabase.Count; i++)
            Convert_mob_db_ToList(m_lines_MonsterDatabase[i]);
    }
    void Convert_mob_db_ToList(string data)
    {
        Log(data);

        if (!data.Contains("//") && !string.IsNullOrEmpty(data) && !string.IsNullOrWhiteSpace(data))
        {
            List<string> sumSplit = StringSplit.GetStringSplit(data, ',');

            MonsterDatabase newMonsterDatabase = new MonsterDatabase();
            newMonsterDatabase.id = int.Parse(sumSplit[0]);
            newMonsterDatabase.name = sumSplit[1];
            newMonsterDatabase.kROName = sumSplit[2];

            m_currentMonsterDatabases.Add(newMonsterDatabase);
        }
    }
    #endregion

    public int targetArray;
    /// <summary>
    /// Start convert specific lines to item info database
    /// </summary>
    /// <param name="index"></param>
    public void ConvertSpecificArrayToItemInfo(int index, bool isNoScript = false)
    {
        targetArray = index;
        ConvertCurrentTargetArrayToItemInfo(null, isNoScript);
    }

    /// <summary>
    /// Convert string to item info database
    /// </summary>
    /// <param name="input"></param>
    void ConvertCurrentTargetArrayToItemInfo(string input = null, bool isExcludeItemScripts = false)
    {
        currentItemDbData = new List<string>();
        if (string.IsNullOrEmpty(input))
        {
            currentItemDbData = ConvertItemDbToListWithoutScript(m_lines[targetArray]);
            FetchItemDbScript(m_lines[targetArray]);
        }
        else
        {
            currentItemDbData = ConvertItemDbToListWithoutScript(input);
            FetchItemDbScript(input);
        }

        currentItemDb = new ItemDb();
        if (!string.IsNullOrEmpty(currentItemDbData[0]))
            currentItemDb.id = int.Parse(currentItemDbData[0]);
        if (!string.IsNullOrEmpty(currentItemDbData[1]))
            currentItemDb.aegisName = currentItemDbData[1];
        if (!string.IsNullOrEmpty(currentItemDbData[2]))
            currentItemDb.name = currentItemDbData[2];
        if (!string.IsNullOrEmpty(currentItemDbData[3]))
            currentItemDb.type = int.Parse(currentItemDbData[3]);
        if (!string.IsNullOrEmpty(currentItemDbData[4]))
            currentItemDb.buy = int.Parse(currentItemDbData[4]);
        if (!string.IsNullOrEmpty(currentItemDbData[5]))
            currentItemDb.sell = int.Parse(currentItemDbData[5]);
        if (!string.IsNullOrEmpty(currentItemDbData[6]))
            currentItemDb.weight = int.Parse(currentItemDbData[6]);
        if (!string.IsNullOrEmpty(currentItemDbData[7]))
        {
            string sum = currentItemDbData[7];
            if (sum.Contains(":"))
            {
                List<string> sumSplit = StringSplit.GetStringSplit(sum, ':');
                currentItemDb.atk = int.Parse(sumSplit[0]);
                currentItemDb.mAtk = int.Parse(sumSplit[1]);
            }
            else
            {
                currentItemDb.atk = int.Parse(currentItemDbData[7]);
                currentItemDb.mAtk = 0;
            }
        }
        if (!string.IsNullOrEmpty(currentItemDbData[8]))
            currentItemDb.def = int.Parse(currentItemDbData[8]);
        if (!string.IsNullOrEmpty(currentItemDbData[9]))
            currentItemDb.range = int.Parse(currentItemDbData[9]);
        if (!string.IsNullOrEmpty(currentItemDbData[10]))
            currentItemDb.slots = int.Parse(currentItemDbData[10]);
        if (!string.IsNullOrEmpty(currentItemDbData[11]))
            currentItemDb.job = Convert.ToUInt32(currentItemDbData[11], 16);
        if (!string.IsNullOrEmpty(currentItemDbData[12]))
            currentItemDb._class = int.Parse(currentItemDbData[12]);
        if (!string.IsNullOrEmpty(currentItemDbData[13]))
            currentItemDb.gender = currentItemDbData[13];
        if (!string.IsNullOrEmpty(currentItemDbData[14]))
            currentItemDb.loc = int.Parse(currentItemDbData[14]);
        if (!string.IsNullOrEmpty(currentItemDbData[15]))
            currentItemDb.wLv = int.Parse(currentItemDbData[15]);
        if (!string.IsNullOrEmpty(currentItemDbData[16]))
        {
            string sum = currentItemDbData[16];
            if (sum.Contains(":"))
            {
                List<string> sumSplit = StringSplit.GetStringSplit(sum, ':');
                currentItemDb.eLv = int.Parse(sumSplit[0]);
                currentItemDb.eMaxLv = int.Parse(sumSplit[1]);
            }
            else
            {
                currentItemDb.eLv = int.Parse(currentItemDbData[16]);
                currentItemDb.eMaxLv = 0;
            }
        }
        if (!string.IsNullOrEmpty(currentItemDbData[17]))
            currentItemDb.refineable = int.Parse(currentItemDbData[17]);
        if (!string.IsNullOrEmpty(currentItemDbData[18]))
            currentItemDb.view = int.Parse(currentItemDbData[18]);

        if (Application.isPlaying)
        {
            if (!isExcludeItemScripts)
                m_currentItemDbs.Add(currentItemDb);
            else
                m_currentItemWithoutScriptDbs.Add(currentItemDb);
        }

        if (!isExcludeItemScripts)
            m_currentOutput += "[" + currentItemDb.id + "] = {"
                + "\nunidentifiedDisplayName = \"" + GetName() + "\","
                + "\nunidentifiedResourceName = \"" + GetResourceName() + "\","
                + "\nunidentifiedDescriptionName = {" + GetDescription() + ""
                + "\n},"
                + "\nidentifiedDisplayName = \"" + GetName() + "\","
                + "\nidentifiedResourceName = \"" + GetResourceName() + "\","
                + "\nidentifiedDescriptionName = {" + GetDescription() + ""
                + "\n},"
                + "\nslotCount = " + GetSlotCount() + ","
                + "\nClassNum = " + GetClassNum() + "\n},\n";

        Log("Success convert item_db id: " + currentItemDbData[0]);
    }

    List<string> ConvertItemDbToListWithoutScript(string data)
    {
        string sum = data;
        int scriptStartAt = sum.IndexOf("{");
        sum = sum.Substring(0, scriptStartAt - 1);
        return new List<string>(sum.Split(','));
    }

    void FetchItemDbScript(string data)
    {
        string sum = data;
        string itemScript = data;
        int itemId = 0;

        int scriptStartAt = sum.IndexOf("{");
        sum = sum.Substring(0, scriptStartAt - 1);
        itemScript = itemScript.Substring(scriptStartAt);
        string sumSaveScriptPath = itemScript;
        string sumSaveScriptPath2 = itemScript;
        string sumSaveScriptPath3 = itemScript;

        List<string> temp_item_db = new List<string>(sum.Split(','));
        itemId = int.Parse(temp_item_db[0]);

        ItemDbScriptData itemDbScriptData = new ItemDbScriptData();
        itemDbScriptData.id = itemId;

        Log("itemScript:\n" + itemScript);

        int onEquipStartAt = itemScript.IndexOf(",{");
        string script = sumSaveScriptPath.Substring(0, onEquipStartAt);
        Log("script:\n" + script);

        itemScript = itemScript.Substring(onEquipStartAt + 1);

        int onUnequipStartAt = itemScript.IndexOf(",{");
        string onEquipScript = sumSaveScriptPath2.Substring(onEquipStartAt + 1, onUnequipStartAt);
        Log("onEquipScript:\n" + onEquipScript);

        int onUnequipEndAt = itemScript.Length;
        itemScript = itemScript.Substring(onUnequipStartAt + 1);

        string onUnequipScript = itemScript;
        Log("onUnequipScript:\n" + onUnequipScript);

        script = RemoveComment(script);
        onEquipScript = RemoveComment(onEquipScript);
        onUnequipScript = RemoveComment(onUnequipScript);

        //Script
        itemDbScriptData.script = script;

        //On equip script
        itemDbScriptData.onEquipScript = onEquipScript;

        //On unequip script
        itemDbScriptData.onUnequipScript = onUnequipScript;

        m_currentItemScriptDatas.Add(itemDbScriptData);
    }

    string RemoveComment(string data)
    {
        string sum = data;

        while (sum.Contains("/*") && sum.Contains("*/"))
        {
            int commentStartAt = sum.IndexOf("/*");
            int commentEndAt = sum.IndexOf("*/");
            sum = sum.Substring(0, commentStartAt) + sum.Substring(commentEndAt + 2);
        }

        Log("Comment removed:\n" + sum);

        return sum;
    }

    #region Item Description
    string GetName()
    {
        return currentItemDb.name;
    }
    string GetResourceName()
    {
        string copier = null;

        for (int i = 0; i < m_currentResourceNames.Count; i++)
        {
            var sumData = m_currentResourceNames[i];
            if (sumData.id == currentItemDb.id)
            {
                copier = sumData.resourceName;
                break;
            }
        }

        return copier;
    }
    string GetDescription()
    {
        string sum = null;
        string sumItemScripts = GetItemScripts();
        if (!isItemScriptNull)
            sum += "\n" + GetItemScripts();
        sum += "\n\"^0000CCประเภท:^000000 " + GetItemType() + "\",";
        if (IsLocNeeded())
            sum += "\n\"^0000CCตำแหน่ง:^000000 " + GetItemLoc() + "\",";
        if (IsAtkNeeded())
            sum += "\n\"^0000CCAtk:^000000 " + GetItemAtk() + "\",";
        if (IsMAtkNeeded())
            sum += "\n\"^0000CCMAtk:^000000 " + GetItemMAtk() + "\",";
        if (IsDefNeeded())
            sum += "\n\"^0000CCDef:^000000 " + GetItemDef() + "\",";
        if (IsRangeNeeded())
            sum += "\n\"^0000CCระยะโจมตี:^000000 " + GetItemRange() + "\",";
        if (IsJobNeeded())
            sum += "\n\"^0000CCอาชีพที่ใช้ได้:^000000 " + GetItemJob() + "\",";
        if (IsClassNeeded())
            sum += "\n\"^0000CCClass ที่ใช้ได้:^000000 " + GetItemClass() + "\",";
        if (IsGenderNeeded())
            sum += "\n\"^0000CCเพศที่ใช้ได้:^000000 " + GetItemGender() + "\",";
        if (IsWeaponLevelNeeded())
            sum += "\n\"^0000CCLv. อาวุธ:^000000 " + GetItemWeaponLevel() + "\",";
        if (IsEquipMaxLevelNeeded())
            sum += "\n\"^0000CCLv. ที่ต้องการ:^000000 " + GetItemEquipLevel() + "\",";
        if (IsEquipMaxLevelNeeded())
            sum += "\n\"^0000CCLv. ห้ามเกิน:^000000 " + GetItemEquipMaxLevel() + "\",";
        if (IsRefineableNeeded())
            sum += "\n\"^0000CCตีบวก:^000000 " + GetItemRefineable() + "\",";
        sum += "\n\"^0000CCน้ำหนัก:^000000 " + GetItemWeight() + "\"";
        return sum;
    }
    bool isItemScriptNull;
    string GetItemScripts()
    {
        string sum = null;

        ItemDbScriptData data = new ItemDbScriptData();

        for (int i = 0; i < m_currentItemScriptDatas.Count; i++)
        {
            var sumData = m_currentItemScriptDatas[i];
            if (sumData.id == currentItemDb.id)
            {
                data = sumData;
                break;
            }
        }

        data.m_output = this;

        sum += data.GetScriptDescription();

        sum += data.GetOnEquipScriptDescription();

        sum += data.GetOnUnequipScriptDescription();

        isItemScriptNull = string.IsNullOrEmpty(sum);

        return sum;
    }
    string GetItemType()
    {
        int type = currentItemDb.type;
        int view = currentItemDb.view;

        if (type == 0)
            return "ของใช้ฟื้นฟู";
        else if (type == 2)
            return "ของกดใช้";
        else if (type == 3)
            return "ของอื่น ๆ";
        else if (type == 4)
            return "อุปกรณ์สวมใส่";
        else if (type == 5)
        {
            if (view == 0)
                return "bare fist";
            else if (view == 1)
                return "Daggers";
            else if (view == 2)
                return "One-handed swords";
            else if (view == 3)
                return "Two-handed swords";
            else if (view == 4)
                return "One-handed spears";
            else if (view == 5)
                return "Two-handed spears";
            else if (view == 6)
                return "One-handed axes";
            else if (view == 7)
                return "Two-handed axes";
            else if (view == 8)
                return "Maces";
            else if (view == 9)
                return "Unused";
            else if (view == 10)
                return "Staff";
            else if (view == 11)
                return "Bows";
            else if (view == 12)
                return "Knuckles";
            else if (view == 13)
                return "Musical Instruments";
            else if (view == 14)
                return "Whips";
            else if (view == 15)
                return "Books";
            else if (view == 16)
                return "Katars";
            else if (view == 17)
                return "Revolvers";
            else if (view == 18)
                return "Rifles";
            else if (view == 19)
                return "Gatling guns";
            else if (view == 20)
                return "Shotguns";
            else if (view == 21)
                return "Grenade launchers";
            else if (view == 22)
                return "Fuuma Shurikens";
            else if (view == 23)
                return "Two-handed staff";
            else if (view == 24)
                return "Max Type";
            else if (view == 25)
                return "Dual-wield Daggers";
            else if (view == 26)
                return "Dual-wield Swords";
            else if (view == 27)
                return "Dual-wield Axes";
            else if (view == 28)
                return "Dagger + Sword";
            else if (view == 29)
                return "Dagger + Axe";
            else if (view == 30)
                return "Sword + Axe";
            else
                return "อาวุธ";
        }
        else if (type == 6)
            return "Card";
        else if (type == 7)
            return "Pet egg";
        else if (type == 8)
            return "อุปกรณ์สวมใส่ Pet";
        else if (type == 10)
        {
            if (view == 1)
                return "ลูกธนู";
            else if (view == 2)
                return "มีดปา";
            else if (view == 3)
                return "ลูกกระสุน";
            else if (view == 4)
                return "ลูกปืนใหญ่";
            else if (view == 5)
                return "ระเบิด";
            else if (view == 6)
                return "ดาวกระจาย";
            else if (view == 7)
                return "คุไน";
            else if (view == 8)
                return "ลูกกระสุนปืนใหญ่";
            else if (view == 9)
                return "ของปา";
            else
                return "กระสุน";
        }
        else if (type == 11)
            return "ของกดใช้";
        else if (type == 12)
            return "อุปกรณ์สวมใส่ Shadow";
        else if (type == 18)
            return "ของกดใช้";
        else
            return null;
    }
    bool IsLocNeeded()
    {
        //Only for armor, weapon, shadow items, ammo
        bool isLocNeed = false;
        int type = currentItemDb.type;
        if (type == 4 || type == 5 || type == 6 || type == 12)
            isLocNeed = true;

        if (!isLocNeed)
            return false;

        int loc = currentItemDb.loc;

        if (loc <= 0)
            return false;

        ItemLoc itemLoc = (ItemLoc)Enum.Parse(typeof(ItemLoc), loc.ToString(), true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(ItemLoc), itemLoc) && !itemLoc.ToString().Contains(","))
            throw new InvalidOperationException($"{loc.ToString("f0")} is not an underlying value of the YourEnum enumeration.");

        return true;
    }
    string GetItemLoc()
    {
        string sum = null;

        int loc = currentItemDb.loc;

        ItemLoc itemLoc = (ItemLoc)Enum.Parse(typeof(ItemLoc), loc.ToString(), true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(ItemLoc), itemLoc) && !itemLoc.ToString().Contains(","))
            throw new InvalidOperationException($"{loc.ToString("f0")} is not an underlying value of the YourEnum enumeration.");

        if (itemLoc.HasFlag(ItemLoc.UpperHeadgear))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Upper Headgear";
            else
                sum += ", Upper Headgear";
        }
        if (itemLoc.HasFlag(ItemLoc.MiddleHeadgear))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Middle Headgear";
            else
                sum += ", Middle Headgear";
        }
        if (itemLoc.HasFlag(ItemLoc.LowerHeadgear))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Lower Headgear";
            else
                sum += ", Lower Headgear";
        }
        if (itemLoc.HasFlag(ItemLoc.Armor))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Armor";
            else
                sum += ", Armor";
        }
        if (itemLoc.HasFlag(ItemLoc.Weapon) && itemLoc.HasFlag(ItemLoc.Shield))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Two-Handed Weapon";
            else
                sum += ", Two-Handed Weapon";
        }
        else
        {
            if (itemLoc.HasFlag(ItemLoc.Weapon))
            {
                if (string.IsNullOrEmpty(sum))
                    sum += "Weapon";
                else
                    sum += ", Weapon";
            }
            if (itemLoc.HasFlag(ItemLoc.Shield))
            {
                if (string.IsNullOrEmpty(sum))
                    sum += "Shield";
                else
                    sum += ", Shield";
            }
        }
        if (itemLoc.HasFlag(ItemLoc.Garment))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Garment";
            else
                sum += ", Garment";
        }
        if (itemLoc.HasFlag(ItemLoc.Footgear))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Footgear";
            else
                sum += ", Footgear";
        }
        if (itemLoc.HasFlag(ItemLoc.AccessoryRight) && itemLoc.HasFlag(ItemLoc.AccessoryLeft))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Accessory Left or Right";
            else
                sum += ", Accessory Left or Right";
        }
        else
        {
            if (itemLoc.HasFlag(ItemLoc.AccessoryRight))
            {
                if (string.IsNullOrEmpty(sum))
                    sum += "Accessory Right";
                else
                    sum += ", Accessory Right";
            }
            if (itemLoc.HasFlag(ItemLoc.AccessoryLeft))
            {
                if (string.IsNullOrEmpty(sum))
                    sum += "Accessory Left";
                else
                    sum += ", Accessory Left";
            }
        }
        if (itemLoc.HasFlag(ItemLoc.CostumeTopHeadgear))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Costume Top Headgear";
            else
                sum += ", Costume Top Headgear";
        }
        if (itemLoc.HasFlag(ItemLoc.CostumeMidHeadgear))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Costume Mid Headgear";
            else
                sum += ", Costume Mid Headgear";
        }
        if (itemLoc.HasFlag(ItemLoc.CostumeLowHeadgear))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Costume Low Headgear";
            else
                sum += ", Costume Low Headgear";
        }
        if (itemLoc.HasFlag(ItemLoc.CostumeGarmentRobe))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Costume Garment";
            else
                sum += ", Costume Garment";
        }
        if (itemLoc.HasFlag(ItemLoc.Ammo))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Ammo";
            else
                sum += ", Ammo";
        }
        if (itemLoc.HasFlag(ItemLoc.ShadowArmor))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Shadow Armor";
            else
                sum += ", Shadow Armor";
        }
        if (itemLoc.HasFlag(ItemLoc.ShadowWeapon))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Shadow Weapon";
            else
                sum += ", Shadow Weapon";
        }
        if (itemLoc.HasFlag(ItemLoc.ShadowShield))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Shadow Shield";
            else
                sum += ", Shadow Shield";
        }
        if (itemLoc.HasFlag(ItemLoc.ShadowShoes))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Shadow Shoes";
            else
                sum += ", Shadow Shoes";
        }
        if (itemLoc.HasFlag(ItemLoc.ShadowAccessoryRightEarring) && itemLoc.HasFlag(ItemLoc.ShadowAccessoryLeftPendant))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Shadow Accessory Left or Right";
            else
                sum += ", Shadow Accessory Left or Right";
        }
        else
        {
            if (itemLoc.HasFlag(ItemLoc.ShadowAccessoryRightEarring))
            {
                if (string.IsNullOrEmpty(sum))
                    sum += "Shadow Accessory Right";
                else
                    sum += ", Shadow Accessory Right";
            }
            if (itemLoc.HasFlag(ItemLoc.ShadowAccessoryLeftPendant))
            {
                if (string.IsNullOrEmpty(sum))
                    sum += "Shadow Accessory Left";
                else
                    sum += ", Shadow Accessory Left";
            }
        }

        return sum;
    }
    bool IsAtkNeeded()
    {
        int atk = currentItemDb.atk;
        if (atk > 0)
            return true;
        else
            return false;
    }
    string GetItemAtk()
    {
        int atk = currentItemDb.atk;
        return atk.ToString("f0");
    }
    bool IsMAtkNeeded()
    {
        int mAtk = currentItemDb.mAtk;
        if (mAtk > 0)
            return true;
        else
            return false;
    }
    string GetItemMAtk()
    {
        int mAtk = currentItemDb.mAtk;
        return mAtk.ToString("f0");
    }
    bool IsDefNeeded()
    {
        int def = currentItemDb.def;
        if (def > 0)
            return true;
        else
            return false;
    }
    string GetItemDef()
    {
        int def = currentItemDb.def;
        return def.ToString("f0");
    }
    bool IsRangeNeeded()
    {
        return IsWeaponLevelNeeded();
    }
    string GetItemRange()
    {
        int range = currentItemDb.range;
        return range.ToString("f0");
    }
    bool IsJobNeeded()
    {
        int sum = 0;

        uint job = currentItemDb.job;

        if (job <= 0)
            return false;

        ItemJob itemJob = (ItemJob)Enum.Parse(typeof(ItemJob), job.ToString(), true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(ItemJob), itemJob) && !itemJob.ToString().Contains(","))
            throw new InvalidOperationException($"{job.ToString("f0")} is not an underlying value of the YourEnum enumeration.");

        if (itemJob.HasFlag(ItemJob.Novice))
            sum++;
        if (itemJob.HasFlag(ItemJob.Swordman))
            sum++;
        if (itemJob.HasFlag(ItemJob.Magician))
            sum++;
        if (itemJob.HasFlag(ItemJob.Archer))
            sum++;
        if (itemJob.HasFlag(ItemJob.Acolyte))
            sum++;
        if (itemJob.HasFlag(ItemJob.Merchant))
            sum++;
        if (itemJob.HasFlag(ItemJob.Thief))
            sum++;
        if (itemJob.HasFlag(ItemJob.Knight))
            sum++;
        if (itemJob.HasFlag(ItemJob.Priest))
            sum++;
        if (itemJob.HasFlag(ItemJob.Wizard))
            sum++;
        if (itemJob.HasFlag(ItemJob.Blacksmith))
            sum++;
        if (itemJob.HasFlag(ItemJob.Hunter))
            sum++;
        if (itemJob.HasFlag(ItemJob.Assassin))
            sum++;
        if (itemJob.HasFlag(ItemJob.Unused))
            sum++;
        if (itemJob.HasFlag(ItemJob.Crusader))
            sum++;
        if (itemJob.HasFlag(ItemJob.Monk))
            sum++;
        if (itemJob.HasFlag(ItemJob.Sage))
            sum++;
        if (itemJob.HasFlag(ItemJob.Rogue))
            sum++;
        if (itemJob.HasFlag(ItemJob.Alchemist))
            sum++;
        if (itemJob.HasFlag(ItemJob.BardDancer))
            sum++;
        if (itemJob.HasFlag(ItemJob.Unused2))
            sum++;
        if (itemJob.HasFlag(ItemJob.Taekwon))
            sum++;
        if (itemJob.HasFlag(ItemJob.StarGladiator))
            sum++;
        if (itemJob.HasFlag(ItemJob.SoulLinker))
            sum++;
        if (itemJob.HasFlag(ItemJob.Gunslinger))
            sum++;
        if (itemJob.HasFlag(ItemJob.Ninja))
            sum++;
        if (itemJob.HasFlag(ItemJob.Gangsi))
            sum++;
        if (itemJob.HasFlag(ItemJob.DeathKnight))
            sum++;
        if (itemJob.HasFlag(ItemJob.DarkCollector))
            sum++;
        if (itemJob.HasFlag(ItemJob.KagerouOboro))
            sum++;
        if (itemJob.HasFlag(ItemJob.Rebellion))
            sum++;
        if (itemJob.HasFlag(ItemJob.Summoner))
            sum++;

        if (sum >= 32)
            return false;
        else
            return true;
    }
    string GetItemJob()
    {
        string sum = null;

        uint job = currentItemDb.job;

        ItemJob itemJob = (ItemJob)Enum.Parse(typeof(ItemJob), job.ToString("f0"), true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(ItemJob), itemJob) && !itemJob.ToString().Contains(","))
            throw new InvalidOperationException($"{job.ToString("f0")} is not an underlying value of the YourEnum enumeration.");

        if (itemJob.HasFlag(ItemJob.Novice))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Novice";
            else
                sum += ", Novice";
        }
        if (itemJob.HasFlag(ItemJob.Swordman))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Swordman";
            else
                sum += ", Swordman";
        }
        if (itemJob.HasFlag(ItemJob.Magician))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Magician";
            else
                sum += ", Magician";
        }
        if (itemJob.HasFlag(ItemJob.Archer))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Archer";
            else
                sum += ", Archer";
        }
        if (itemJob.HasFlag(ItemJob.Acolyte))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Acolyte";
            else
                sum += ", Acolyte";
        }
        if (itemJob.HasFlag(ItemJob.Merchant))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Merchant";
            else
                sum += ", Merchant";
        }
        if (itemJob.HasFlag(ItemJob.Thief))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Thief";
            else
                sum += ", Thief";
        }
        if (itemJob.HasFlag(ItemJob.Knight))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Knight";
            else
                sum += ", Knight";
        }
        if (itemJob.HasFlag(ItemJob.Priest))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Priest";
            else
                sum += ", Priest";
        }
        if (itemJob.HasFlag(ItemJob.Wizard))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Wizard";
            else
                sum += ", Wizard";
        }
        if (itemJob.HasFlag(ItemJob.Blacksmith))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Blacksmith";
            else
                sum += ", Blacksmith";
        }
        if (itemJob.HasFlag(ItemJob.Hunter))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Hunter";
            else
                sum += ", Hunter";
        }
        if (itemJob.HasFlag(ItemJob.Assassin))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Assassin";
            else
                sum += ", Assassin";
        }
        if (itemJob.HasFlag(ItemJob.Crusader))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Crusader";
            else
                sum += ", Crusader";
        }
        if (itemJob.HasFlag(ItemJob.Monk))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Monk";
            else
                sum += ", Monk";
        }
        if (itemJob.HasFlag(ItemJob.Sage))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Sage";
            else
                sum += ", Sage";
        }
        if (itemJob.HasFlag(ItemJob.Rogue))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Rogue";
            else
                sum += ", Rogue";
        }
        if (itemJob.HasFlag(ItemJob.Alchemist))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Alchemist";
            else
                sum += ", Alchemist";
        }
        if (itemJob.HasFlag(ItemJob.BardDancer))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Bard & Dancer";
            else
                sum += ", Bard & Dancer";
        }
        if (itemJob.HasFlag(ItemJob.Taekwon))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Taekwon";
            else
                sum += ", Taekwon";
        }
        if (itemJob.HasFlag(ItemJob.StarGladiator))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Star Gladiator";
            else
                sum += ", Star Gladiator";
        }
        if (itemJob.HasFlag(ItemJob.SoulLinker))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Soul Linker";
            else
                sum += ", Soul Linker";
        }
        if (itemJob.HasFlag(ItemJob.Gunslinger))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Gunslinger";
            else
                sum += ", Gunslinger";
        }
        if (itemJob.HasFlag(ItemJob.Ninja))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Ninja";
            else
                sum += ", Ninja";
        }
        if (itemJob.HasFlag(ItemJob.Gangsi))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Gangsi";
            else
                sum += ", Gangsi";
        }
        if (itemJob.HasFlag(ItemJob.DeathKnight))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Death Knight";
            else
                sum += ", Death Knight";
        }
        if (itemJob.HasFlag(ItemJob.DarkCollector))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Dark Collector";
            else
                sum += ", Dark Collector";
        }
        if (itemJob.HasFlag(ItemJob.KagerouOboro))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Kagerou & Oboro";
            else
                sum += ", Kagerou & Oboro";
        }
        if (itemJob.HasFlag(ItemJob.Rebellion))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Rebellion";
            else
                sum += ", Rebellion";
        }
        if (itemJob.HasFlag(ItemJob.Summoner))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Summoner";
            else
                sum += ", Summoner";
        }

        return sum;
    }
    bool IsClassNeeded()
    {
        int sum = 0;

        int _class = currentItemDb._class;

        if (_class <= 0)
            return false;

        ItemClass itemClass = (ItemClass)Enum.Parse(typeof(ItemClass), _class.ToString("f0"), true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(ItemClass), itemClass) && !itemClass.ToString().Contains(","))
            throw new InvalidOperationException($"{_class.ToString("f0")} is not an underlying value of the YourEnum enumeration.");

        if (itemClass.HasFlag(ItemClass.NormalClass))
            sum++;
        if (itemClass.HasFlag(ItemClass.TranscedentClasses))
            sum++;
        if (itemClass.HasFlag(ItemClass.BabyClasses))
            sum++;
        if (itemClass.HasFlag(ItemClass.ThirdClasses))
            sum++;
        if (itemClass.HasFlag(ItemClass.TranscedentThirdClasses))
            sum++;
        if (itemClass.HasFlag(ItemClass.ThirdBabyClasses))
            sum++;

        if (sum >= 6)
            return false;
        else
            return true;
    }
    string GetItemClass()
    {
        string sum = null;

        int _class = currentItemDb._class;

        ItemClass itemClass = (ItemClass)Enum.Parse(typeof(ItemClass), _class.ToString("f0"), true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(ItemClass), itemClass) && !itemClass.ToString().Contains(","))
            throw new InvalidOperationException($"{_class.ToString("f0")} is not an underlying value of the YourEnum enumeration.");

        if (itemClass.HasFlag(ItemClass.NormalClass))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Class 1 และ 2";
            else
                sum += ", Class 1 และ 2";
        }
        if (itemClass.HasFlag(ItemClass.TranscedentClasses))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Class 1 และ 2";
            else
                sum += ", Class 1 และ 2";
        }
        if (itemClass.HasFlag(ItemClass.BabyClasses))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Baby Class 1 และ 2";
            else
                sum += ", Baby Class 1 และ 2";
        }
        if (itemClass.HasFlag(ItemClass.ThirdClasses))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Class 3";
            else
                sum += ", Class 3";
        }
        if (itemClass.HasFlag(ItemClass.TranscedentThirdClasses))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Trans Class 3";
            else
                sum += ", Trans Class 3";
        }
        if (itemClass.HasFlag(ItemClass.ThirdBabyClasses))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "Baby Class 3";
            else
                sum += ", Baby Class 3";
        }
        return sum;
    }
    bool IsGenderNeeded()
    {
        string gender = currentItemDb.gender;
        if (string.IsNullOrEmpty(gender))
            return false;
        if (int.Parse(gender) != 2)
            return true;
        else
            return false;
    }
    string GetItemGender()
    {
        int gender = int.Parse(currentItemDb.gender);
        if (gender == 0)
            return "หญิง";
        else if (gender == 1)
            return "ชาย";
        else
            return null;
    }
    bool IsWeaponLevelNeeded()
    {
        int type = currentItemDb.type;
        if (type == 5)
            return true;
        else
            return false;
    }
    string GetItemWeaponLevel()
    {
        int wLv = currentItemDb.wLv;
        return wLv.ToString("f0");
    }
    bool IsEquipLevelNeeded()
    {
        int eLv = currentItemDb.eLv;
        if (eLv > 0)
            return true;
        else
            return false;
    }
    string GetItemEquipLevel()
    {
        int eLv = currentItemDb.eLv;
        return eLv.ToString("f0");
    }
    bool IsEquipMaxLevelNeeded()
    {
        int eMaxLv = currentItemDb.eMaxLv;
        if (eMaxLv > 0)
            return true;
        else
            return false;
    }
    string GetItemEquipMaxLevel()
    {
        int eMaxLv = currentItemDb.eMaxLv;
        return eMaxLv.ToString("f0");
    }
    bool IsRefineableNeeded()
    {
        int type = currentItemDb.type;
        if (type == 4
            || type == 5
            || type == 12)
            return true;
        else
            return false;
    }
    string GetItemRefineable()
    {
        int refineable = currentItemDb.refineable;
        if (refineable >= 1)
            return "ได้";
        else
            return "ไม่ได้";
    }
    string GetItemWeight()
    {
        float weight = currentItemDb.weight;
        weight /= 10;
        if (weight <= 0)
            return weight.ToString("f0");
        else if (weight < 1)
            return weight.ToString("f1");
        else
            return weight.ToString("f0");
    }
    string GetSlotCount()
    {
        return currentItemDb.slots.ToString("f0");
    }
    string GetClassNum()
    {
        return currentItemDb.view.ToString("f0");
    }
    #endregion

    #region Debug / View Database
    [Button]
    public void DebugConvertCurrentTargetArrayToItemInfo()
    {
        m_currentOutput = null;
        Log("Output cleared");
        ConvertCurrentTargetArrayToItemInfo();
        ClipboardExtension.CopyToClipboard(m_currentOutput);
    }

    public string _string;
    [Button]
    public void DebugConvertStringToItemInfo()
    {
        m_currentOutput = null;
        Log("Output cleared");
        ConvertCurrentTargetArrayToItemInfo(_string);
        ClipboardExtension.CopyToClipboard(m_currentOutput);
    }

    [Button]
    public void ViewTargetArrayLines()
    {
        Log(m_lines[targetArray]);
    }

    [TextArea] string currentOutput;
    public string m_currentOutput { get { return currentOutput; } set { currentOutput = value; } }

    //item_db
    List<string> currentItemDbData = new List<string>();

    //itemInfo
    ItemDb currentItemDb = new ItemDb();
    List<ItemDb> currentItemDbs = new List<ItemDb>();
    public List<ItemDb> m_currentItemDbs { get { return currentItemDbs; } set { currentItemDbs = value; } }
    List<ItemDb> currentItemWithoutScriptDbs = new List<ItemDb>();
    public List<ItemDb> m_currentItemWithoutScriptDbs { get { return currentItemWithoutScriptDbs; } set { currentItemWithoutScriptDbs = value; } }

    //resourceName
    List<ItemResourceName> currentResourceNames = new List<ItemResourceName>();
    public List<ItemResourceName> m_currentResourceNames { get { return currentResourceNames; } set { currentResourceNames = value; } }

    //Skill Name
    List<SkillName> currentSkillNames = new List<SkillName>();
    public List<SkillName> m_currentSkillNames { get { return currentSkillNames; } set { currentSkillNames = value; } }

    //Monster Database
    List<MonsterDatabase> currentMonsterDatabases = new List<MonsterDatabase>();
    public List<MonsterDatabase> m_currentMonsterDatabases { get { return currentMonsterDatabases; } set { currentMonsterDatabases = value; } }

    //Item Script
    List<ItemDbScriptData> currentItemScriptDatas = new List<ItemDbScriptData>();
    public List<ItemDbScriptData> m_currentItemScriptDatas { get { return currentItemScriptDatas; } set { currentItemScriptDatas = value; } }

    //lines
    List<string> lines = new List<string>();
    public List<string> m_lines { get { return lines; } set { lines = value; } }
    List<string> lines_resourceNames = new List<string>();
    public List<string> m_lines_resourceNames { get { return lines_resourceNames; } set { lines_resourceNames = value; } }
    List<string> lines_SkillName = new List<string>();
    public List<string> m_lines_SkillName { get { return lines_SkillName; } set { lines_SkillName = value; } }
    List<string> lines_MonsterDatabase = new List<string>();
    public List<string> m_lines_MonsterDatabase { get { return lines_MonsterDatabase; } set { lines_MonsterDatabase = value; } }
    #endregion

    void Log(object obj)
    {
        if (!Application.isPlaying)
            Debug.Log(obj);
    }
}
