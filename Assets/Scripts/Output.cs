using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using System;
using System.Text;

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
    public void ConvertSpecificArrayToItemInfo(int index)
    {
        targetArray = index;
        ConvertCurrentTargetArrayToItemInfo();
    }

    /// <summary>
    /// Convert string to item info database
    /// </summary>
    /// <param name="input"></param>
    void ConvertCurrentTargetArrayToItemInfo(string input = null)
    {
        if (string.IsNullOrEmpty(input))
            Log("ConvertCurrentTargetArrayToItemInfo: Start");
        else
            Log("DebugConvertStringToItemInfo: Start");

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
            m_currentItemDbs.Add(currentItemDb);

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

        if (string.IsNullOrEmpty(input))
            Log("ConvertCurrentTargetArrayToItemInfo: Done");
        else
            Log("DebugConvertStringToItemInfo: Done");
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
        string sumScriptPath = data;
        int itemId = 0;

        int scriptStartAt = sum.IndexOf("{");
        sum = sum.Substring(0, scriptStartAt - 1);
        sumScriptPath = sumScriptPath.Substring(scriptStartAt);
        string sumSaveScriptPath = sumScriptPath;
        string sumSaveScriptPath2 = sumScriptPath;
        string sumSaveScriptPath3 = sumScriptPath;

        List<string> temp_item_db = new List<string>(sum.Split(','));
        itemId = int.Parse(temp_item_db[0]);

        ItemDbScriptData itemDbScriptData = new ItemDbScriptData();
        itemDbScriptData.id = itemId;

        Log("sumScriptPath: " + sumScriptPath);

        int onEquipStartAt = sumScriptPath.IndexOf(",{");
        string sumScript = sumSaveScriptPath.Substring(0, onEquipStartAt);
        Log("sumScript: " + sumScript);

        sumScriptPath = sumScriptPath.Substring(onEquipStartAt + 1);
        Log("sumScriptPath: " + sumScriptPath);

        int onUnequipStartAt = sumScriptPath.IndexOf(",{");
        string sumOnEquipScript = sumSaveScriptPath2.Substring(onEquipStartAt + 1, onUnequipStartAt);
        Log("sumOnEquipScript: " + sumOnEquipScript);

        int onUnequipEndAt = sumScriptPath.Length;
        sumScriptPath = sumScriptPath.Substring(onUnequipStartAt + 1);
        Log("sumScriptPath: " + sumScriptPath);

        string sumOnUnequipScript = sumScriptPath;
        Log("sumOnUnequipScript: " + sumOnUnequipScript);

        sumScript = RemoveComment(sumScript);
        sumOnEquipScript = RemoveComment(sumOnEquipScript);
        sumOnUnequipScript = RemoveComment(sumOnUnequipScript);

        //Script
        itemDbScriptData.script = sumScript;

        //On equip script
        itemDbScriptData.onEquipScript = sumOnEquipScript;

        //On unequip script
        itemDbScriptData.onUnequipScript = sumOnUnequipScript;

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

        Log("RemoveComment >> sum: " + sum);

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
    [Flags]
    public enum ItemLoc
    {
        UpperHeadgear = 256,
        MiddleHeadgear = 512,
        LowerHeadgear = 1,
        Armor = 16,
        Weapon = 2,
        Shield = 32,
        Garment = 4,
        Footgear = 64,
        AccessoryRight = 8,
        AccessoryLeft = 128,
        CostumeTopHeadgear = 1024,
        CostumeMidHeadgear = 2048,
        CostumeLowHeadgear = 4096,
        CostumeGarmentRobe = 8192,
        Ammo = 32768,
        ShadowArmor = 65536,
        ShadowWeapon = 131072,
        ShadowShield = 262144,
        ShadowShoes = 524288,
        ShadowAccessoryRightEarring = 1048576,
        ShadowAccessoryLeftPendant = 2097152,
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

        ItemLoc itemLoc = (ItemLoc)Enum.Parse(typeof(ItemLoc), loc.ToString());

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(ItemLoc), itemLoc) && !itemLoc.ToString().Contains(","))
            throw new InvalidOperationException($"{loc.ToString("f0")} is not an underlying value of the YourEnum enumeration.");

        return true;
    }
    string GetItemLoc()
    {
        string sum = null;

        int loc = currentItemDb.loc;

        ItemLoc itemLoc = (ItemLoc)Enum.Parse(typeof(ItemLoc), loc.ToString());

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
    /// <summary>
    /// Credit: https://stackoverflow.com/questions/14479981/how-do-i-check-if-bitmask-contains-bit/14480058
    /// </summary>
    [Flags]
    public enum ItemJob : uint
    {
        Novice = 0x00000001,
        Swordman = 0x00000002,
        Magician = 0x00000004,
        Archer = 0x00000008,
        Acolyte = 0x00000010,
        Merchant = 0x00000020,
        Thief = 0x00000040,
        Knight = 0x00000080,
        Priest = 0x00000100,
        Wizard = 0x00000200,
        Blacksmith = 0x00000400,
        Hunter = 0x00000800,
        Assassin = 0x00001000,
        Unused = 0x00002000,
        Crusader = 0x00004000,
        Monk = 0x00008000,
        Sage = 0x00010000,
        Rogue = 0x00020000,
        Alchemist = 0x00040000,
        BardDancer = 0x00080000,
        Unused2 = 0x00100000,
        Taekwon = 0x00200000,
        StarGladiator = 0x00400000,
        SoulLinker = 0x00800000,
        Gunslinger = 0x01000000,
        Ninja = 0x02000000,
        Gangsi = 0x04000000,
        DeathKnight = 0x08000000,
        DarkCollector = 0x10000000,
        KagerouOboro = 0x20000000,
        Rebellion = 0x40000000,
        Summoner = 0x80000000,
    }
    bool IsJobNeeded()
    {
        int sum = 0;

        uint job = currentItemDb.job;

        if (job <= 0)
            return false;

        ItemJob itemJob = (ItemJob)Enum.Parse(typeof(ItemJob), job.ToString());

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

        ItemJob itemJob = (ItemJob)Enum.Parse(typeof(ItemJob), job.ToString("f0"));

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
    /// <summary>
    /// Credit: https://stackoverflow.com/questions/29482/how-to-cast-int-to-enum
    /// </summary>
    [Flags]
    public enum ItemClass
    {
        None = 0,
        NormalClass = 1,
        TranscedentClasses = 2,
        BabyClasses = 4,
        ThirdClasses = 8,
        TranscedentThirdClasses = 16,
        ThirdBabyClasses = 32
    }
    bool IsClassNeeded()
    {
        int sum = 0;

        int _class = currentItemDb._class;

        if (_class <= 0)
            return false;

        ItemClass itemClass = (ItemClass)Enum.Parse(typeof(ItemClass), _class.ToString("f0"));

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

        ItemClass itemClass = (ItemClass)Enum.Parse(typeof(ItemClass), _class.ToString("f0"));

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

#region Item Class
[Serializable]
public class ItemDb
{
    public int id;
    public string aegisName;
    public string name;
    public int type;
    public int buy;
    public int sell;
    public int weight;
    public int atk;
    public int mAtk;
    public int def;
    public int range;
    public int slots;
    public uint job;
    public int _class;
    public string gender;
    public int loc;
    public int wLv;
    public int eLv;
    public int eMaxLv;
    public int refineable;
    public int view;
}

[Serializable]
public class ItemDbScriptData
{
    public Output m_output;

    #region Variable
    public int id;
    public string script;
    public string onEquipScript;
    public string onUnequipScript;

    bool isHadParam1;
    bool isHadParam2;
    bool isHadParam3;
    bool isHadParam4;
    bool isHadParam5;
    bool isHadParam6;
    bool isHadParam7;
    bool isParam1Negative;
    bool isParam2Negative;
    bool isParam3Negative;
    bool isParam4Negative;
    bool isParam5Negative;
    bool isParam6Negative;
    bool isParam7Negative;
    #endregion

    #region Get Description Functions
    string sumScript;
    public string GetScriptDescription()
    {
        sumScript = null;

        string sumDesc = GetDescription(script);
        sumScript = sumDesc;

        return sumScript;
    }
    string sumEquipScript;
    public string GetOnEquipScriptDescription()
    {
        sumEquipScript = null;

        string sumDesc = GetDescription(onEquipScript);
        if (!string.IsNullOrEmpty(sumScript) && !string.IsNullOrEmpty(sumDesc) && !string.IsNullOrWhiteSpace(sumDesc))
            sumEquipScript = "\n[เมื่อสวมใส่]\n" + sumDesc;
        else if (!string.IsNullOrEmpty(sumDesc) && !string.IsNullOrWhiteSpace(sumDesc))
            sumEquipScript = "[เมื่อสวมใส่]\n" + sumDesc;
        else
            sumEquipScript = sumDesc;

        return sumEquipScript;
    }
    string sumUnequipScript;
    public string GetOnUnequipScriptDescription()
    {
        sumUnequipScript = null;

        string sumDesc = GetDescription(onUnequipScript);
        if (!string.IsNullOrEmpty(sumEquipScript) && !string.IsNullOrEmpty(sumDesc) && !string.IsNullOrWhiteSpace(sumDesc))
            sumUnequipScript = "\n[เมื่อถอด]\n" + sumDesc;
        else if (!string.IsNullOrEmpty(sumDesc) && !string.IsNullOrWhiteSpace(sumDesc))
            sumUnequipScript = "[เมื่อถอด]\n" + sumDesc;
        else
            sumUnequipScript = sumDesc;

        return sumUnequipScript;
    }
    #endregion

    List<TempVariables> tempVariables = new List<TempVariables>();
    bool isHadEndIf;
    int toAddEndIfCount;
    /// <summary>
    /// Get description by each item scripts function
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string GetDescription(string data)
    {
        tempVariables = new List<TempVariables>();

        string sum = null;

        string sumData = data;

        Log("GetDescription:" + data);

        //Split all space
        List<string> allCut = StringSplit.GetStringSplit(data, ' ');

    L_Redo:
        #region Merge it again line by line
        for (int i = 0; i < allCut.Count; i++)
        {
            var sumCut = allCut[i];

            Log("(Merging) allCut[" + i + "]: " + sumCut);

            if (sumCut == "if" && allCut[i + 1].Contains("("))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("if(") || sumCut.Contains("if (") || sumCut.Contains("else if(") || sumCut.Contains("else if ("))
            {
                int count1 = 0;
                foreach (char c in sumCut)
                {
                    if (c == '(')
                        count1++;
                }

                int count2 = 0;
                foreach (char c in sumCut)
                {
                    if (c == ')')
                        count2++;
                }

                //Check ()
                if (count1 != count2)
                {
                    MergeItemScripts(allCut, i);
                    goto L_Redo;
                }

                //if not had {}
                if (!allCut[i + 1].Contains("{"))
                {
                    toAddEndIfCount = 0;
                    isHadEndIf = true;
                }

                //if had {
                if (allCut[i + 1].Contains("{") && !allCut[i + 1].Contains("}"))
                {
                    allCut[i + 1] += " " + allCut[i + 2];
                    allCut.RemoveAt(i + 2);
                    goto L_Redo;
                }
                //if had {}
                else if (allCut[i + 1].Contains("{") && allCut[i + 1].Contains("}"))
                {
                    string findTempVar = allCut[i + 1];
                    if (findTempVar.Contains(".@"))
                    {
                        if (findTempVar.Contains(".@") && !findTempVar.Contains(";"))
                        {
                            MergeItemScripts(allCut, i);
                            goto L_Redo;
                        }
                        else
                        {
                            TempVariables newTempVariables = new TempVariables();
                            string tempVariablesName = findTempVar;
                            List<string> allTempVariablesName = StringSplit.GetStringSplit(tempVariablesName, '=');
                            if (allTempVariablesName.Count >= 2)
                            {
                                allTempVariablesName[0] = allTempVariablesName[0].Replace("{", "");
                                int cutStartAt = allTempVariablesName[1].IndexOf(';');
                                allTempVariablesName[1] = allTempVariablesName[1].Substring(0, cutStartAt + 1);
                                newTempVariables.variableName = MergeWhiteSpace.RemoveWhiteSpace(allTempVariablesName[0]);
                                newTempVariables.value = MergeWhiteSpace.RemoveWhiteSpace(allTempVariablesName[1]);

                                bool isFound = false;
                                for (int j = 0; j < tempVariables.Count; j++)
                                {
                                    if (tempVariables[j].variableName == newTempVariables.variableName)
                                    {
                                        isFound = true;
                                        break;
                                    }
                                }
                                if (!isFound)
                                {
                                    Log("newTempVariables.variableName: " + newTempVariables.variableName);
                                    Log("newTempVariables.value: " + newTempVariables.value);
                                    tempVariables.Add(newTempVariables);
                                }
                            }
                        }
                    }
                }
            }
            else if (sumCut.Contains("else"))
            {
                //else not had {}
                if (!allCut[i + 1].Contains("{"))
                    allCut[i] = "TXT_ELSE";
                //else had {}
                if (allCut[i + 1].Contains("{") && !allCut[i + 1].Contains("}"))
                {
                    //Merge
                    allCut[i + 1] += " " + allCut[i + 2];
                    allCut.RemoveAt(i + 2);
                    goto L_Redo;
                }
            }
            else if (sumCut == "bonus" || sumCut == "bonus " || sumCut == " bonus")
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut == "bonus2" || sumCut == "bonus2 " || sumCut == " bonus2")
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut == "bonus3" || sumCut == "bonus3 " || sumCut == " bonus3")
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut == "bonus4" || sumCut == "bonus4 " || sumCut == " bonus4")
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut == "bonus5" || sumCut == "bonus5 " || sumCut == " bonus5")
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains(".@"))
            {
                if (sumCut.Contains(".@") && !sumCut.Contains(";"))
                {
                    MergeItemScripts(allCut, i);
                    goto L_Redo;
                }
                else
                {
                    TempVariables newTempVariables = new TempVariables();
                    string tempVariablesName = sumCut;
                    List<string> allTempVariablesName = StringSplit.GetStringSplit(tempVariablesName, '=');
                    if (allTempVariablesName.Count >= 2)
                    {
                        newTempVariables.variableName = MergeWhiteSpace.RemoveWhiteSpace(allTempVariablesName[0]);
                        newTempVariables.value = MergeWhiteSpace.RemoveWhiteSpace(allTempVariablesName[1]);

                        bool isFound = false;
                        for (int j = 0; j < tempVariables.Count; j++)
                        {
                            if (tempVariables[j].variableName == newTempVariables.variableName)
                            {
                                isFound = true;
                                break;
                            }
                        }
                        if (!isFound)
                        {
                            Log("newTempVariables.variableName: " + newTempVariables.variableName);
                            Log("newTempVariables.value: " + newTempVariables.value);
                            tempVariables.Add(newTempVariables);
                        }
                    }
                }
            }
            else if (sumCut.Contains("itemheal") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("percentheal") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("sc_end") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("sc_start") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("itemskill") && !sumCut.Contains(";"))
            {
                allCut[i] = allCut[i].Replace("itemskill", "itemskillz");
                allCut[i] += " " + allCut[i + 1];
                allCut.RemoveAt(i + 1);
                goto L_Redo;
            }
            else if (sumCut.Contains("skill") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("getrandgroupitem") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("monster") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("produce") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("pet") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("catchpet") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bpet") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("birthpet") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("guildgetexp") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("Zeny") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("RouletteGold") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("RouletteBronze") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("RouletteSilver") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bStr") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bAgi,") || sumCut.Contains("bonus bAgi ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bVit") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bInt,") || sumCut.Contains("bonus bInt ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDex") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bLuk") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAllStats") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAgiVit") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAgiDexStr") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bMaxHP,") || sumCut.Contains("bonus bMaxHP ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMaxHPrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bMaxSP,") || sumCut.Contains("bonus bMaxSP ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMaxSPrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bBaseAtk") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bAtk,") || sumCut.Contains("bonus bAtk ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAtk2") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAtkRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bWeaponAtkRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bMatk,") || sumCut.Contains("bonus bMatk ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMatkRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bWeaponMatkRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bDef,") || sumCut.Contains("bonus bDef ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDefRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bDef2,") || sumCut.Contains("bonus bDef2 ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDef2Rate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bMdef,") || sumCut.Contains("bonus bMdef ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMdefRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bMdef2,") || sumCut.Contains("bonus bMdef2 ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMdef2Rate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bHit,") || sumCut.Contains("bonus bHit ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bHitRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bCritical,") || sumCut.Contains("bonus bCritical ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bCriticalLong") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bCriticalAddRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bCriticalRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bFlee,") || sumCut.Contains("bonus bFlee ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bFleeRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bFlee2,") || sumCut.Contains("bonus bFlee2 ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bFlee2Rate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bPerfectHitRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bPerfectHitAddRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bSpeedRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bSpeedAddRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bAspd,") || sumCut.Contains("bonus bAspd ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAspdRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAtkRange") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAddMaxWeight") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bHPrecovRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bSPrecovRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bHPRegenRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bHPLossRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSPRegenRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSPLossRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bRegenPercentHP") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bRegenPercentSP") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoRegen") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUseSPrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bSkillUseSP,") || sumCut.Contains("bonus2 bSkillUseSP ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSkillUseSPrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSkillAtk") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bLongAtkRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bCritAtkRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bCriticalDef") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bWeaponAtk,") || sumCut.Contains("bonus2 bWeaponAtk ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bWeaponDamageRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNearAtkDef") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bLongAtkDef") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMagicAtkDef") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMiscAtkDef") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoWeaponDamage") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoMagicDamage") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoMiscDamage") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bHealPower,") || sumCut.Contains("bonus bHealPower ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bHealPower2") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bSkillHeal,") || sumCut.Contains("bonus2 bSkillHeal ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSkillHeal2") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAddItemHealRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddItemHealRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddItemGroupHealRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bCastrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bCastrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bFixedCastrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bFixedCastrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bVariableCastrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bVariableCastrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bFixedCast,") || sumCut.Contains("bonus bFixedCast ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSkillFixedCast") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bVariableCast,") || sumCut.Contains("bonus bVariableCast ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSkillVariableCast") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bNoCastCancel;") || sumCut.Contains("bonus bNoCastCancel ;")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoCastCancel2") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDelayrate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSkillDelay") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSkillCooldown") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bAddEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bMagicAddEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSubEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bSubEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSubDefEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bAddRace,") || sumCut.Contains("bonus2 bAddRace ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bMagicAddRace,") || sumCut.Contains("bonus2 bMagicAddRace ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bSubRace,") || sumCut.Contains("bonus2 bSubRace ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bAddClass,") || sumCut.Contains("bonus2 bAddClass ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bMagicAddClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSubClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddSize") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bMagicAddSize") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSubSize") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoSizeFix") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddDamageClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddMagicDamageClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddDefMonster") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddMDefMonster") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddRace2") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSubRace2") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bMagicAddRace2") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSubSkill") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAbsorbDmgMaxHP") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAtkEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDefEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bMagicAtkEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDefRatioAtkRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDefRatioAtkEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDefRatioAtkClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus4 bSetDefRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus4 bSetMDefRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bIgnoreDefEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bIgnoreDefRace,") || sumCut.Contains("bonus bIgnoreDefRace ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bIgnoreDefClass,") || sumCut.Contains("bonus bIgnoreDefClass ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bIgnoreMDefRace,") || sumCut.Contains("bonus bIgnoreMDefRace ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bIgnoreDefRaceRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bIgnoreMdefRaceRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bIgnoreMdefRace2Rate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bIgnoreMDefEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bIgnoreDefClassRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bIgnoreMdefClassRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bExpAddRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bExpAddClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bAddEff,") || sumCut.Contains("bonus2 bAddEff ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddEff2") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddEffWhenHit") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bResEff") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus3 bAddEff,") || sumCut.Contains("bonus3 bAddEff ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus4 bAddEff,") || sumCut.Contains("bonus4 bAddEff ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bAddEffWhenHit") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus4 bAddEffWhenHit") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bAddEffOnSkill") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus4 bAddEffOnSkill") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus5 bAddEffOnSkill") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bComaClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bComaRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bWeaponComaEle") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bWeaponComaClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bWeaponComaRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus3 bAutoSpell,") || sumCut.Contains("bonus3 bAutoSpell ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bAutoSpellWhenHit") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus4 bAutoSpell,") || sumCut.Contains("bonus4 bAutoSpell ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus5 bAutoSpell,") || sumCut.Contains("bonus5 bAutoSpell ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus4 bAutoSpellWhenHit") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus5 bAutoSpellWhenHit") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus4 bAutoSpellOnSkill") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus5 bAutoSpellOnSkill") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bHPDrainValue") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bHPDrainValueRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bHpDrainValueClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bSPDrainValue") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSPDrainValueRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSpDrainValueClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bHPDrainRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSPDrainRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bHPVanishRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bHPVanishRaceRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bHPVanishRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSPVanishRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bSPVanishRaceRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bSPVanishRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bStateNoRecoverRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bHPGainValue") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bSPGainValue") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bSPGainRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bLongHPGainValue") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bLongSPGainValue") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMagicHPGainValue") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMagicSPGainValue") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bShortWeaponDamageReturn") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bLongWeaponDamageReturn") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bMagicDamageReturn") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnstripableWeapon") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnstripableArmor") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnstripableHelm") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnstripableShield") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bUnstripable;") || sumCut.Contains("bonus bUnstripable ;")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnbreakableGarment") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnbreakableWeapon") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnbreakableArmor") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnbreakableHelm") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnbreakableShield") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bUnbreakableShoes") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus bUnbreakable,") || sumCut.Contains("bonus bUnbreakable ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bBreakWeaponRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bBreakArmorRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bDropAddRace") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bDropAddClass") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bAddMonsterIdDropItem") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus2 bAddMonsterDropItem,") || sumCut.Contains("bonus2 bAddMonsterDropItem ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus3 bAddMonsterDropItem,") || sumCut.Contains("bonus3 bAddMonsterDropItem ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if ((sumCut.Contains("bonus3 bAddClassDropItem,") || sumCut.Contains("bonus3 bAddClassDropItem ,")) && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i, true);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddMonsterDropItemGroup") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bAddMonsterDropItemGroup") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus3 bAddClassDropItemGroup") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bGetZenyNum") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddGetZenyNum") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDoubleRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bDoubleAddRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bSplashRange") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bSplashAddRange") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus2 bAddSkillBlow") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoKnockback") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoGemStone") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bIntravision") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bPerfectHide") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bRestartFullRecover") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bClassChange") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bAddStealRate") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoMadoFuel") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
            else if (sumCut.Contains("bonus bNoWalkDelay") && !sumCut.Contains(";"))
            {
                MergeItemScripts(allCut, i);
                goto L_Redo;
            }
        }
        #endregion

        for (int i = 0; i < allCut.Count; i++)
        {
            Log("allCut[" + i + "]: " + allCut[i]);

            data = allCut[i];

            string functionName = "";
            #region if
            if (data.Contains("if(") || data.Contains("if (") || data.Contains("else if(") || data.Contains("else if ("))
            {
                //Remove spacebar
                data = MergeWhiteSpace.RemoveWhiteSpace(data);
                //Remove if(
                data = data.Substring(3);
                //Remove )
                data = data.Substring(0, data.Length - 1);
                //Replace == to คือ
                data = data.Replace("==", " คือ ");
                //Replace != to ไม่เท่ากับ
                data = data.Replace("!=", " ไม่เท่ากับ ");
                //Replace || to หรือ
                data = data.Replace("||", " หรือ ");
                //Replace && to และ
                data = data.Replace("&&", " และ ");
                //Replace Job Name
                data = data.Replace("Job_", "");
                //Replace _
                data = data.Replace("_", " ");
                //Replace getpartnerid()
                data = data.Replace("getpartnerid()", "มีคู่สมรส");

                sum += AddDescription(sum, "[ถ้า " + data + "]");
            }
            #endregion
            #region TXT_ELSE
            functionName = "TXT_ELSE";
            if (data.Contains(functionName))
            {
                sum += AddDescription(sum, "[หากไม่ตรงเงื่อนไข]");
                allCut.Insert(i + 2, "TXT_ENDELSE");
            }
            #endregion
            #region TXT_ENDELSE
            functionName = "TXT_ENDELSE";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "[สิ้นสุดหากไม่ตรงเงื่อนไข]");
            #endregion
            #region TXT_ENDIF
            functionName = "TXT_ENDIF";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "[สิ้นสุดหากตรงเงื่อนไข]");
            #endregion
            #region itemheal
            functionName = "itemheal";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "เสีย " + param1 + " HP");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " HP");
                }
                if (isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "เสีย " + param2 + " SP");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param2 + " SP");
                }
            }
            #endregion
            #region percentheal
            functionName = "percentheal";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "เสีย HP " + param1 + "%");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู HP " + param1 + "%");
                }
                if (isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "เสีย SP " + param2 + "%");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู SP " + param2 + "%");
                }
            }
            #endregion
            #region sc_end
            functionName = "sc_end ";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                    sum += AddDescription(sum, "รักษาสถานะ " + param1);
            }
            #endregion
            #region sc_start
            functionName = "sc_start ";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = "";
                string param2 = "";
                string param3 = "";
                string param4 = "";
                string param5 = "";

                if (allParam.Count > 0)
                    param1 = GetValue(allParam[0], 1);
                if (allParam.Count > 1)
                    param2 = GetValue(allParam[1], 2);
                if (allParam.Count > 2)
                    param3 = GetValue(allParam[2], 3);
                if (allParam.Count > 3)
                    param4 = GetValue(allParam[3], 4);
                if (allParam.Count > 4)
                    param5 = GetValue(allParam[4], 5, true);

                Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);
                Log("isHadParam2: " + isHadParam2 + " | param2: " + param2);
                Log("isHadParam3: " + isHadParam3 + " | param3: " + param3);
                Log("isHadParam4: " + isHadParam4 + " | param4: " + param4);
                Log("isHadParam5: " + isHadParam5 + " | param5: " + param5);

                string timer = "";
                if (param2 == "ตลอดเวลา")
                    timer = " " + param2;
                else
                    timer = " เป็นเวลา " + TimerToStringTimer(float.Parse(param2));

                float rate = 0;
                if (isHadParam4)
                    rate = float.Parse(param4);
                string percent = GetPercent(rate, 100);

                int flag = 0;
                if (isHadParam5)
                    flag = int.Parse(param5);
                else
                    flag = 1;
                string sumFlag = Get_sc_start_Flag(flag);

                if (isHadParam1 && isHadParam2 && isHadParam3
                    || isHadParam1 && isHadParam2)
                    sum += AddDescription(sum, "มีโอกาส " + percent + " ที่จะเกิดสถานะ " + param1 + timer + sumFlag);
                else if (isHadParam1)
                    sum += AddDescription(sum, "มีโอกาส " + percent + " ที่จะเกิดสถานะ " + param1 + sumFlag);
            }
            #endregion
            #region itemskillz
            functionName = "itemskillz";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = "";
                string param2 = "";
                string param3 = "";

                if (allParam.Count > 0)
                    param1 = GetSkillName(allParam[0]);
                if (allParam.Count > 1)
                    param2 = GetValue(allParam[1], 2);
                if (allParam.Count > 2)
                    param3 = allParam[2];

                Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);
                Log("isHadParam2: " + isHadParam2 + " | param2: " + param2);
                Log("isHadParam3: " + isHadParam3 + " | param3: " + param3);

                bool isNeedRequirement = false;
                if (param3 == "true")
                    isNeedRequirement = true;

                if (isNeedRequirement)
                    sum += AddDescription(sum, "สามารถใช้ Lv." + param2 + " " + param1 + "(โดยมีเงื่อนไขการใช้ Skill ดังเดิม)");
                else
                    sum += AddDescription(sum, "สามารถใช้ Lv." + param2 + " " + param1);
            }
            #endregion
            #region skill
            functionName = "skill ";
            if (data.Contains("skill "))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = "";
                string param2 = "";
                string param3 = "";

                if (allParam.Count > 0)
                    param1 = GetSkillName(allParam[0]);
                if (allParam.Count > 1)
                    param2 = GetValue(allParam[1], 2);
                SkillFlag skillFlag = SkillFlag.SKILL_TEMP;
                if (allParam.Count > 2)
                {
                    param3 = allParam[2];

                    skillFlag = (SkillFlag)Enum.Parse(typeof(SkillFlag), param3);

                    // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
                    if (!Enum.IsDefined(typeof(SkillFlag), param3) && !param3.Contains(","))
                        throw new InvalidOperationException($"{param3} is not an underlying value of the YourEnum enumeration.");
                }

                Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);
                Log("isHadParam2: " + isHadParam2 + " | param2: " + param2);
                Log("isHadParam3: " + isHadParam3 + " | param3: " + param3);

                string txtFlag = null;
                if (skillFlag == SkillFlag.SKILL_PERM)
                    txtFlag = "(ถาวร)";
                else if (skillFlag == SkillFlag.SKILL_TEMPLEVEL)
                    txtFlag = "(สามารถทับเลเวลเดิมได้)";

                sum += AddDescription(sum, "สามารถใช้ Lv." + param2 + " " + param1 + txtFlag);

            }
            #endregion
            #region getrandgroupitem
            functionName = "getrandgroupitem";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = "";
                string param2 = "";
                string param3 = "";
                string param4 = "";

                if (allParam.Count > 0)
                    param1 = allParam[0];
                if (allParam.Count > 1)
                    param2 = GetValue(allParam[1], 2, true);
                if (allParam.Count > 2)
                    param3 = allParam[2];
                if (allParam.Count > 3)
                    param4 = allParam[3];

                param1 = param1.Replace("IG_", "");
                param1 = param1.Replace("_", " ");

                Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);
                Log("isHadParam2: " + isHadParam2 + " | param2: " + param2);
                Log("isHadParam3: " + isHadParam3 + " | param3: " + param3);
                Log("isHadParam4: " + isHadParam4 + " | param4: " + param4);

                sum += AddDescription(sum, "กดใช้เพื่อรับ Item ในกลุ่ม " + param1);
            }
            #endregion
            #region monster
            functionName = "monster";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = "";
                string param2 = "";
                string param3 = "";
                string param4 = "";
                string param5 = "";
                string param6 = "";
                string param7 = "";

                if (allParam.Count > 0)
                    param1 = allParam[0];
                if (allParam.Count > 1)
                    param2 = allParam[1];
                if (allParam.Count > 2)
                    param3 = allParam[2];
                if (allParam.Count > 3)
                    param4 = allParam[3];
                if (allParam.Count > 4)
                    param5 = allParam[4];
                if (allParam.Count > 5)
                    param6 = allParam[5];
                if (allParam.Count > 6)
                    param7 = allParam[6];

                int type = 0;//Normal

                if (param5.Contains("rand"))
                    type = 1;//Group

                int paramInt = 0;
                bool isInteger = false;

                isInteger = int.TryParse(param5, out paramInt);

                if (isInteger)
                    type = 2;//Specific

                string finalize = null;

                if (type <= 0)
                {
                    param5 = param5.Replace("-1-", "");
                    param5 = param5.Replace("MOBG_", "");
                    param5 = param5.Replace("_", " ");

                    finalize = "กดใช้เพื่อเรียก Monster ในกลุ่ม " + param5;
                }
                else if (type == 1)
                {
                    if (param1.Contains("rand"))
                        finalize = "กดใช้เพื่อเรียก Monster ในกลุ่ม " + GetMonsterNameGroup(param1);
                    else if (param2.Contains("rand"))
                        finalize = "กดใช้เพื่อเรียก Monster ในกลุ่ม " + GetMonsterNameGroup(param2);
                    else if (param3.Contains("rand"))
                        finalize = "กดใช้เพื่อเรียก Monster ในกลุ่ม " + GetMonsterNameGroup(param3);
                    else if (param4.Contains("rand"))
                        finalize = "กดใช้เพื่อเรียก Monster ในกลุ่ม " + GetMonsterNameGroup(param4);
                    else if (param5.Contains("rand"))
                        finalize = "กดใช้เพื่อเรียก Monster ในกลุ่ม " + GetMonsterNameGroup(param5);
                    else if (param6.Contains("rand"))
                        finalize = "กดใช้เพื่อเรียก Monster ในกลุ่ม " + GetMonsterNameGroup(param6);
                    else if (param7.Contains("rand"))
                        finalize = "กดใช้เพื่อเรียก Monster ในกลุ่ม " + GetMonsterNameGroup(param7);
                }
                else if (type == 2)
                    finalize = "กดใช้เพื่อเรียก Monster " + GetMonsterName(paramInt);

                Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);
                Log("isHadParam2: " + isHadParam2 + " | param2: " + param2);
                Log("isHadParam3: " + isHadParam3 + " | param3: " + param3);
                Log("isHadParam4: " + isHadParam4 + " | param4: " + param4);
                Log("isHadParam5: " + isHadParam5 + " | param5: " + param5);
                Log("isHadParam6: " + isHadParam6 + " | param6: " + param6);
                Log("isHadParam7: " + isHadParam7 + " | param7: " + param7);

                sum += AddDescription(sum, finalize);
            }
            #endregion
            #region produce
            functionName = "produce";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = "";

                if (allParam.Count > 0)
                    param1 = GetValue(allParam[0], 1);

                string finalize = null;

                if (isHadParam1)
                {
                    if (param1 == "1")
                        finalize = "กดใช้เพื่อเปิดหน้าต่าง Craft Level 1 Weapons";
                    else if (param1 == "2")
                        finalize = "กดใช้เพื่อเปิดหน้าต่าง Craft Level 2 Weapons";
                    else if (param1 == "3")
                        finalize = "กดใช้เพื่อเปิดหน้าต่าง Craft Level 3 Weapons";
                    else if (param1 == "21")
                        finalize = "กดใช้เพื่อเปิดหน้าต่าง Craft Blacksmith's Stones and Metals";
                    else if (param1 == "22")
                        finalize = "กดใช้เพื่อเปิดหน้าต่าง Craft Alchemist's Potions, Holy Water, Assassin Cross's Deadly Poison";
                    else if (param1 == "23")
                        finalize = "กดใช้เพื่อเปิดหน้าต่าง Craft Elemental Converters";
                }

                Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);

                sum += AddDescription(sum, finalize);
            }
            #endregion
            #region pet
            functionName = "pet ";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = "";

                if (allParam.Count > 0)
                    param1 = GetValue(allParam[0], 1);

                int paramInt = 0;
                bool isInteger = false;

                isInteger = int.TryParse(param1, out paramInt);

                string finalize = null;

                if (isHadParam1)
                    finalize = "กดใช้เพื่อเริ่มจับ Monster " + GetMonsterName(paramInt);

                Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);

                sum += AddDescription(sum, finalize);
            }
            #endregion
            #region catchpet
            functionName = "catchpet ";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = "";

                if (allParam.Count > 0)
                    param1 = GetValue(allParam[0], 1);

                int paramInt = 0;
                bool isInteger = false;

                isInteger = int.TryParse(param1, out paramInt);

                string finalize = null;

                if (isHadParam1)
                    finalize = "กดใช้เพื่อเริ่มจับ Monster " + GetMonsterName(paramInt);

                Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);

                sum += AddDescription(sum, finalize);
            }
            #endregion
            #region bpet
            functionName = "bpet";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                string finalize = "กดใช้เพื่อฟักไข่ Pet";

                sum += AddDescription(sum, finalize);
            }
            #endregion
            #region birthpet
            functionName = "birthpet";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                string finalize = "กดใช้เพื่อฟักไข่ Pet";

                sum += AddDescription(sum, finalize);
            }
            #endregion
            #region guildgetexp
            functionName = "guildgetexp";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                    sum += AddDescription(sum, "กดใช้เพื่อเพิ่ม EXP ให้ Guild จำนวน " + param1 + " EXP");
            }
            #endregion
            #region Zeny +=
            functionName = "Zeny +=";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                    sum += AddDescription(sum, "กดใช้เพื่อรับ " + param1 + " Zeny");
            }
            #endregion 
            #region Zeny+=
            functionName = "Zeny+=";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                    sum += AddDescription(sum, "กดใช้เพื่อรับ " + param1 + " Zeny");
            }
            #endregion
            #region RouletteGold
            functionName = "RouletteGold";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                    sum += AddDescription(sum, "กดใช้เพื่อรับ " + param1 + " Roulette Gold");
            }
            #endregion
            #region RouletteBronze
            functionName = "RouletteBronze";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                    sum += AddDescription(sum, "กดใช้เพื่อรับ " + param1 + " Roulette Bronze");
            }
            #endregion 
            #region RouletteSilver
            functionName = "RouletteSilver";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                    sum += AddDescription(sum, "กดใช้เพื่อรับ " + param1 + " Roulette Silver");
            }
            #endregion

            #region bonus bStr
            functionName = "bonus bStr";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "STR -" + param1);
                    else
                        sum += AddDescription(sum, "STR +" + param1);
                }
            }
            #endregion
            #region bonus bAgi,
            functionName = "bonus bAgi,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "AGI -" + param1);
                    else
                        sum += AddDescription(sum, "AGI +" + param1);
                }
            }
            #endregion
            #region bonus bVit
            functionName = "bonus bVit";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "VIT -" + param1);
                    else
                        sum += AddDescription(sum, "VIT +" + param1);
                }
            }
            #endregion
            #region bonus bInt,
            functionName = "bonus bInt,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "INT -" + param1);
                    else
                        sum += AddDescription(sum, "INT +" + param1);
                }
            }
            #endregion
            #region bonus bDex
            functionName = "bonus bDex";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "DEX -" + param1);
                    else
                        sum += AddDescription(sum, "DEX +" + param1);
                }
            }
            #endregion
            #region bonus bLuk
            functionName = "bonus bLuk";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "LUK -" + param1);
                    else
                        sum += AddDescription(sum, "LUK +" + param1);
                }
            }
            #endregion
            #region bonus bAllStats
            functionName = "bonus bAllStats";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "All Stats -" + param1);
                    else
                        sum += AddDescription(sum, "All Stats +" + param1);
                }
            }
            #endregion
            #region bonus bAgiVit
            functionName = "bonus bAgiVit";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "AGI, VIT -" + param1);
                    else
                        sum += AddDescription(sum, "AGI, VIT +" + param1);
                }
            }
            #endregion
            #region bonus bAgiDexStr
            functionName = "bonus bAgiDexStr";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "AGI, DEX, STR -" + param1);
                    else
                        sum += AddDescription(sum, "AGI, DEX, STR +" + param1);
                }
            }
            #endregion
            #region bonus bMaxHP,
            functionName = "bonus bMaxHP,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Max HP -" + param1);
                    else
                        sum += AddDescription(sum, "Max HP +" + param1);
                }
            }
            #endregion
            #region bonus bMaxHPrate
            functionName = "bonus bMaxHPrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Max HP -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "Max HP +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bMaxSP,
            functionName = "bonus bMaxSP,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Max SP -" + param1);
                    else
                        sum += AddDescription(sum, "Max SP +" + param1);
                }
            }
            #endregion
            #region bonus bMaxSPrate
            functionName = "bonus bMaxSPrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Max SP -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "Max SP +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bBaseAtk
            functionName = "bonus bBaseAtk";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ATK -" + param1);
                    else
                        sum += AddDescription(sum, "ATK +" + param1);
                }
            }
            #endregion
            #region bonus bAtk,
            functionName = "bonus bAtk,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ATK -" + param1);
                    else
                        sum += AddDescription(sum, "ATK +" + param1);
                }
            }
            #endregion
            #region bonus bAtk2
            functionName = "bonus bAtk2";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ATK -" + param1);
                    else
                        sum += AddDescription(sum, "ATK +" + param1);
                }
            }
            #endregion
            #region bonus bAtkRate
            functionName = "bonus bAtkRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ATK -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "ATK +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bWeaponAtkRate
            functionName = "bonus bWeaponAtkRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ATK จากอาวุธ -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "ATK จากอาวุธ +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bMatk,
            functionName = "bonus bMatk,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "MATK -" + param1);
                    else
                        sum += AddDescription(sum, "MATK +" + param1);
                }
            }
            #endregion
            #region bonus bMatkRate
            functionName = "bonus bMatkRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "MATK -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "MATK +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bWeaponMatkRate
            functionName = "bonus bWeaponMatkRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "MATK จากอาวุธ -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "MATK จากอาวุธ +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bDef,
            functionName = "bonus bDef,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "DEF -" + param1);
                    else
                        sum += AddDescription(sum, "DEF +" + param1);
                }
            }
            #endregion
            #region bonus bDefRate
            functionName = "bonus bDefRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "DEF -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "DEF +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bDef2,
            functionName = "bonus bDef2,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "DEF จาก VIT -" + param1);
                    else
                        sum += AddDescription(sum, "DEF จาก VIT +" + param1);
                }
            }
            #endregion
            #region bonus bDef2Rate
            functionName = "bonus bDef2Rate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "DEF จาก VIT -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "DEF จาก VIT +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bMdef,
            functionName = "bonus bMdef,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "MDEF -" + param1);
                    else
                        sum += AddDescription(sum, "MDEF +" + param1);
                }
            }
            #endregion
            #region bonus bMdefRate
            functionName = "bonus bMdefRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "MDEF -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "MDEF +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bMdef2,
            functionName = "bonus bMdef2,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "MDEF จาก INT -" + param1);
                    else
                        sum += AddDescription(sum, "MDEF จาก INT +" + param1);
                }
            }
            #endregion
            #region bonus bMdef2Rate
            functionName = "bonus bMdef2Rate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "MDEF จาก INT -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "MDEF จาก INT +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bHit,
            functionName = "bonus bHit,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "HIT -" + param1);
                    else
                        sum += AddDescription(sum, "HIT +" + param1);
                }
            }
            #endregion
            #region bonus bHitRate
            functionName = "bonus bHitRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "HIT -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "HIT +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bCritical,
            functionName = "bonus bCritical,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Critical -" + param1);
                    else
                        sum += AddDescription(sum, "Critical +" + param1);
                }
            }
            #endregion
            #region bonus bCriticalLong
            functionName = "bonus bCriticalLong";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "(โจมตีกายภาพระยะไกล) Critical -" + param1);
                    else
                        sum += AddDescription(sum, "(โจมตีกายภาพระยะไกล) Critical +" + param1);
                }
            }
            #endregion
            #region bonus2 bCriticalAddRace
            functionName = "bonus2 bCriticalAddRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Critical -" + param2 + " กับ " + param1);
                    else
                        sum += AddDescription(sum, "Critical +" + param2 + " กับ " + param1);
                }
            }
            #endregion
            #region bonus bCriticalRate
            functionName = "bonus bCriticalRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Critical -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "Critical +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bFlee,
            functionName = "bonus bFlee,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Flee -" + param1);
                    else
                        sum += AddDescription(sum, "Flee +" + param1);
                }
            }
            #endregion
            #region bonus bFleeRate
            functionName = "bonus bFleeRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Flee -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "Flee +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bFlee2,
            functionName = "bonus bFlee2,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Perfect Dodge -" + param1);
                    else
                        sum += AddDescription(sum, "Perfect Dodge +" + param1);
                }
            }
            #endregion
            #region bonus bFlee2Rate
            functionName = "bonus bFlee2Rate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Perfect Dodge -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "Perfect Dodge +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bPerfectHitRate
            functionName = "bonus bPerfectHitRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Perfect Hit -" + param1 + "% (ใช้ค่ามากสุดเท่านั้น)");
                    else
                        sum += AddDescription(sum, "Perfect Hit +" + param1 + "% (ใช้ค่ามากสุดเท่านั้น)");
                }
            }
            #endregion
            #region bonus bPerfectHitAddRate
            functionName = "bonus bPerfectHitAddRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Perfect Hit -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "Perfect Hit +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bSpeedRate
            functionName = "bonus bSpeedRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "เคลื่อนที่ช้าลง " + param1 + "% (ใช้ค่ามากสุดเท่านั้น)");
                    else
                        sum += AddDescription(sum, "เคลื่อนที่เร็วขึ้น " + param1 + "% (ใช้ค่ามากสุดเท่านั้น)");
                }
            }
            #endregion
            #region bonus bSpeedAddRate
            functionName = "bonus bSpeedAddRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "เคลื่อนที่ช้าลง " + param1 + "%");
                    else
                        sum += AddDescription(sum, "เคลื่อนที่เร็วขึ้น " + param1 + "%");
                }
            }
            #endregion
            #region bonus bAspd,
            functionName = "bonus bAspd,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ASPD -" + param1);
                    else
                        sum += AddDescription(sum, "ASPD +" + param1);
                }
            }
            #endregion
            #region bonus bAspdRate
            functionName = "bonus bAspdRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ASPD -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "ASPD +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bAtkRange
            functionName = "bonus bAtkRange";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ระยะโจมตี -" + param1);
                    else
                        sum += AddDescription(sum, "ระยะโจมตี +" + param1);
                }
            }
            #endregion
            #region bonus bAddMaxWeight
            functionName = "bonus bAddMaxWeight";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "แบกของได้น้อยลง -" + param1);
                    else
                        sum += AddDescription(sum, "แบกของได้มากขึ้น +" + param1);
                }
            }
            #endregion   
            #region bonus bHPrecovRate
            functionName = "bonus bHPrecovRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "อัตราการฟื้นฟู HP ปกติ -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "อัตราการฟื้นฟู HP ปกติ +" + param1 + "%");
                }
            }
            #endregion
            #region bonus bSPrecovRate
            functionName = "bonus bSPrecovRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "อัตราการฟื้นฟู SP ปกติ -" + param1 + "%");
                    else
                        sum += AddDescription(sum, "อัตราการฟื้นฟู SP ปกติ +" + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bHPRegenRate
            functionName = "bonus2 bHPRegenRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                    sum += AddDescription(sum, "ฟื้นฟู " + param1 + " HP ทุก ๆ " + TimerToStringTimer(float.Parse(param2)));
            }
            #endregion
            #region bonus2 bHPLossRate
            functionName = "bonus2 bHPLossRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                    sum += AddDescription(sum, "เสีย " + param1 + " HP ทุก ๆ " + TimerToStringTimer(float.Parse(param2)));
            }
            #endregion
            #region bonus2 bSPRegenRate
            functionName = "bonus2 bSPRegenRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                    sum += AddDescription(sum, "ฟื้นฟู " + param1 + " SP ทุก ๆ " + TimerToStringTimer(float.Parse(param2)));
            }
            #endregion
            #region bonus2 bSPLossRate
            functionName = "bonus2 bSPLossRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                    sum += AddDescription(sum, "เสีย " + param1 + " HP ทุก ๆ " + TimerToStringTimer(float.Parse(param2)));
            }
            #endregion
            #region bonus2 bRegenPercentHP
            functionName = "bonus2 bRegenPercentHP";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "เสีย " + param1 + "% จาก MaxHP ทุก ๆ " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + "% จาก MaxHP ทุก ๆ " + TimerToStringTimer(float.Parse(param2)));
                }
            }
            #endregion
            #region bonus2 bRegenPercentSP
            functionName = "bonus2 bRegenPercentSP";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "เสีย " + param1 + "% จาก MaxSP ทุก ๆ " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + "% จาก MaxSP ทุก ๆ " + TimerToStringTimer(float.Parse(param2)));
                }
            }
            #endregion
            #region bonus bNoRegen
            functionName = "bonus bNoRegen";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (param1 == "1")
                        sum += AddDescription(sum, "หยุดการฟื้นฟู HP ตลอดเวลา");
                    else if (param1 == "2")
                        sum += AddDescription(sum, "หยุดการฟื้นฟู SP ตลอดเวลา");
                }
            }
            #endregion
            #region bonus bUseSPrate
            functionName = "bonus bUseSPrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ใช้ SP น้อยลง " + param1 + "%");
                    else
                        sum += AddDescription(sum, "ใช้ SP มากขึ้น " + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bSkillUseSP,
            functionName = "bonus2 bSkillUseSP,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[0], 1);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ใช้ SP มากขึ้น " + param2 + " กับ Skill " + GetSkillName(param1));
                    else
                        sum += AddDescription(sum, "ใช้ SP น้อยลง " + param2 + " กับ Skill " + GetSkillName(param1));
                }
            }
            #endregion
            #region bonus2 bSkillUseSPrate
            functionName = "bonus2 bSkillUseSPrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[0], 1);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ใช้ SP มากขึ้น " + param2 + "% กับ Skill " + GetSkillName(param1));
                    else
                        sum += AddDescription(sum, "ใช้ SP น้อยลง " + param2 + "% กับ Skill " + GetSkillName(param1));
                }
            }
            #endregion
            #region bonus2 bSkillAtk
            functionName = "bonus2 bSkillAtk";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[0], 1);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "Skill " + GetSkillName(param1) + " เบาลง " + param2 + "%");
                    else
                        sum += AddDescription(sum, "Skill " + GetSkillName(param1) + " แรงขึ้น " + param2 + "%");
                }
            }
            #endregion
            #region bonus bLongAtkRate
            functionName = "bonus bLongAtkRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โจมตีไกลเบาลง " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โจมตีไกลแรงขึ้น " + param1 + "%");
                }
            }
            #endregion
            #region bonus bCritAtkRate
            functionName = "bonus bCritAtkRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Critical เบาลง " + param1 + "%");
                    else
                        sum += AddDescription(sum, "Critical แรงขึ้น " + param1 + "%");
                }
            }
            #endregion
            #region bonus bCriticalDef
            functionName = "bonus bCriticalDef";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "เพิ่มโอกาสโดน Critical " + param1 + "%");
                    else
                        sum += AddDescription(sum, "ลดโอกาสโดน Critical " + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bWeaponAtk,
            functionName = "bonus2 bWeaponAtk,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ATK -" + param2 + " ด้วยอาวุธประเภท " + GetWeaponType(param1));
                    else
                        sum += AddDescription(sum, "ATK +" + param2 + " ด้วยอาวุธประเภท " + GetWeaponType(param1));
                }
            }
            #endregion
            #region bonus2 bWeaponDamageRate
            functionName = "bonus2 bWeaponDamageRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีเบาลง " + param2 + "% ด้วยอาวุธประเภท " + GetWeaponType(param1));
                    else
                        sum += AddDescription(sum, "โจมตีแรงขึ้น " + param2 + "% ด้วยอาวุธประเภท " + GetWeaponType(param1));
                }
            }
            #endregion
            #region bonus bNearAtkDef
            functionName = "bonus bNearAtkDef";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โดนโจมตีกายภาพระยะประชิดแรงขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โดนโจมตีกายภาพระยะประชิดเบาลง " + param1 + "%");
                }
            }
            #endregion
            #region bonus bLongAtkDef
            functionName = "bonus bLongAtkDef";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โดนโจมตีกายภาพระยะไกลแรงขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โดนโจมตีกายภาพระยะไกลเบาลง " + param1 + "%");
                }
            }
            #endregion
            #region bonus bMagicAtkDef
            functionName = "bonus bMagicAtkDef";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โดนโจมตีเวทย์แรงขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โดนโจมตีเวทย์เบาลง " + param1 + "%");
                }
            }
            #endregion
            #region bonus bMiscAtkDef
            functionName = "bonus bMiscAtkDef";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โดนโจมตีจากประเภทอื่น ๆ แรงขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โดนโจมตีจากประเภทอื่น ๆ เบาลง " + param1 + "%");
                }
            }
            #endregion
            #region bonus bNoWeaponDamage
            functionName = "bonus bNoWeaponDamage";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โดนโจมตีกายภาพแรงขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โดนโจมตีกายภาพเบาลง " + param1 + "%");
                }
            }
            #endregion
            #region bonus bNoMagicDamage
            functionName = "bonus bNoMagicDamage";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โดนโจมตีเวทยน์แรงขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โดนโจมตีเวทยน์เบาลง " + param1 + "%");
                }
            }
            #endregion
            #region bonus bNoMiscDamage
            functionName = "bonus bNoMiscDamage";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โดนโจมตีจากประเภทอื่น ๆ แรงขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โดนโจมตีจากประเภทอื่น ๆ เบาลง " + param1 + "%");
                }
            }
            #endregion
            #region bonus bHealPower,
            functionName = "bonus bHealPower,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Skill ประเภท Heal เบาลง " + param1 + "%");
                    else
                        sum += AddDescription(sum, "Skill ประเภท Heal แรงขึ้น " + param1 + "%");
                }
            }
            #endregion
            #region bonus bHealPower2
            functionName = "bonus bHealPower2";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "โดน Heal เบาลง " + param1 + "%");
                    else
                        sum += AddDescription(sum, "โดน Heal แรงขึ้น " + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bSkillHeal,
            functionName = "bonus2 bSkillHeal,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "Skill " + GetSkillName(param1) + " Heal เบาลง " + param2 + "%");
                    else
                        sum += AddDescription(sum, "Skill " + GetSkillName(param1) + " Heal แรงขึ้น " + param2 + "%");
                }
            }
            #endregion
            #region bonus2 bSkillHeal2
            functionName = "bonus2 bSkillHeal2";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดน Heal จาก Skill " + GetSkillName(param1) + " เบาลง " + param2 + "%");
                    else
                        sum += AddDescription(sum, "โดน Heal จาก Skill " + GetSkillName(param1) + " แรงขึ้น " + param2 + "%");
                }
            }
            #endregion
            #region bonus bAddItemHealRate
            functionName = "bonus bAddItemHealRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "อัตราการฟื้นฟูจาก Item น้อยลง " + param1 + "%");
                    else
                        sum += AddDescription(sum, "อัตราการฟื้นฟูจาก Item มากขึ้น " + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bAddItemHealRate
            functionName = "bonus2 bAddItemHealRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "อัตราการฟื้นฟูจาก Item " + GetItemName(param1) + " น้อยลง " + param2 + "%");
                    else
                        sum += AddDescription(sum, "อัตราการฟื้นฟูจาก Item " + GetItemName(param1) + " มากขึ้น " + param2 + "%");
                }
            }
            #endregion
            #region bonus2 bAddItemGroupHealRate
            functionName = "bonus2 bAddItemGroupHealRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    param1 = param1.Replace("IG_", "");
                    param1 = param1.Replace("_", " ");

                    if (isParam2Negative)
                        sum += AddDescription(sum, "อัตราการฟื้นฟูจาก Item Group " + param1 + " น้อยลง " + param2 + "%");
                    else
                        sum += AddDescription(sum, "อัตราการฟื้นฟูจาก Item Group " + param1 + " มากขึ้น " + param2 + "%");
                }
            }
            #endregion
            #region bonus bCastrate
            functionName = "bonus bCastrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ร่าย VCAST เร็วขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย VCAST ช้าลง" + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bCastrate
            functionName = "bonus2 bCastrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(param1) + " เร็วขึ้น " + param2 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(param1) + " ช้าลง " + param2 + "%");
                }
            }
            #endregion
            #region bonus bFixedCastrate
            functionName = "bonus bFixedCastrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ร่าย FIXCAST เร็วขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย FIXCAST ช้าลง" + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bFixedCastrate
            functionName = "bonus2 bFixedCastrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย FIXCAST กับ Skill " + GetSkillName(param1) + " เร็วขึ้น " + param2 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย FIXCAST กับ Skill " + GetSkillName(param1) + " ช้าลง " + param2 + "%");
                }
            }
            #endregion
            #region bonus bVariableCastrate
            functionName = "bonus bVariableCastrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ร่าย VCAST เร็วขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย VCAST ช้าลง" + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bVariableCastrate
            functionName = "bonus2 bVariableCastrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(param1) + " เร็วขึ้น " + param2 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(param1) + " ช้าลง " + param2 + "%");
                }
            }
            #endregion
            #region bonus bFixedCast,
            functionName = "bonus bFixedCast,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ร่าย FIXCAST เร็วขึ้น " + TimerToStringTimer(float.Parse(param1)));
                    else
                        sum += AddDescription(sum, "ร่าย FIXCAST ช้าลง" + TimerToStringTimer(float.Parse(param1)));
                }
            }
            #endregion
            #region bonus2 bSkillFixedCast
            functionName = "bonus2 bSkillFixedCast";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย FIXCAST กับ Skill " + GetSkillName(param1) + " เร็วขึ้น " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "ร่าย FIXCAST กับ Skill " + GetSkillName(param1) + " ช้าลง " + TimerToStringTimer(float.Parse(param2)));
                }
            }
            #endregion
            #region bonus bVariableCast,
            functionName = "bonus bVariableCast,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ร่าย VCAST เร็วขึ้น " + TimerToStringTimer(float.Parse(param1)));
                    else
                        sum += AddDescription(sum, "ร่าย VCAST ช้าลง" + TimerToStringTimer(float.Parse(param1)));
                }
            }
            #endregion
            #region bonus2 bSkillVariableCast
            functionName = "bonus2 bSkillVariableCast";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(param1) + " เร็วขึ้น " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(param1) + " ช้าลง " + TimerToStringTimer(float.Parse(param2)));
                }
            }
            #endregion
            #region bonus bNoCastCancel;
            functionName = "bonus bNoCastCancel;";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ร่ายได้โดยไม่ถูกหยุด (ใช้ไม่ได้ใน GvG)");
            #endregion
            #region bonus bNoCastCancel2
            functionName = "bonus bNoCastCancel2";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ร่ายได้โดยไม่ถูกหยุด (ใช้ได้ทุกที่)");
            #endregion
            #region bonus bDelayrate
            functionName = "bonus bDelayrate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "Delay หลังร่ายเร็วขึ้น " + param1 + "%");
                    else
                        sum += AddDescription(sum, "Delay หลังร่ายช้าลง " + param1 + "%");
                }
            }
            #endregion
            #region bonus2 bSkillDelay
            functionName = "bonus2 bSkillDelay";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "Delay หลังร่ายกับ Skill " + GetSkillName(param1) + " เร็วขึ้น " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "Delay หลังร่ายกับ Skill " + GetSkillName(param1) + " ช้าลง " + TimerToStringTimer(float.Parse(param2)));
                }
            }
            #endregion
            #region bonus2 bSkillCooldown
            functionName = "bonus2 bSkillCooldown";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "Cooldown Skill " + GetSkillName(param1) + " เร็วขึ้น " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "Cooldown Skill " + GetSkillName(param1) + " ช้าลง " + TimerToStringTimer(float.Parse(param2)));
                }
            }
            #endregion
            #region bonus2 bAddEle
            functionName = "bonus2 bAddEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับธาตุ " + GetElementName(param1));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(param1));
                }
            }
            #endregion
            #region bonus3 bAddEle
            functionName = "bonus3 bAddEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับธาตุ " + GetElementName(param1) + GetTriggerCriteria(param3));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(param1) + GetTriggerCriteria(param3));
                }
            }
            #endregion
            #region bonus2 bMagicAddEle
            functionName = "bonus2 bMagicAddEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับธาตุ " + GetElementName(param1));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(param1));
                }
            }
            #endregion
            #region bonus2 bSubEle
            functionName = "bonus2 bSubEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(param1));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับธาตุ " + GetElementName(param1));
                }
            }
            #endregion
            #region bonus3 bSubEle
            functionName = "bonus3 bSubEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(param1) + GetTriggerCriteria(param3));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับธาตุ " + GetElementName(param1) + GetTriggerCriteria(param3));
                }
            }
            #endregion
            #region bonus2 bSubDefEle
            functionName = "bonus2 bSubDefEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับศัตรูที่การป้องกันเป็นธาตุ " + GetElementName(param1));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับศัตรูที่การป้องกันเป็นธาตุ " + GetElementName(param1));
                }
            }
            #endregion
            #region bonus2 bAddRace,
            functionName = "bonus2 bAddRace,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับเผ่า " + GetRaceName(param1));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับเผ่า " + GetRaceName(param1));
                }
            }
            #endregion
            #region bMagicAddRace,
            functionName = "bMagicAddRace,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับเผ่า " + GetRaceName(param1));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับเผ่า " + GetRaceName(param1));
                }
            }
            #endregion
            #region bonus2 bSubRace,
            functionName = "bonus2 bSubRace,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับเผ่า " + GetRaceName(param1));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับเผ่า " + GetRaceName(param1));
                }
            }
            #endregion
            #region bonus2 bAddClass,
            functionName = "bonus2 bAddClass,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับ Class " + GetClassName(param1));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับ Class " + GetClassName(param1));
                }
            }
            #endregion
            #region bonus2 bMagicAddClass
            functionName = "bonus2 bMagicAddClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับ Class " + GetClassName(param1));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับ Class " + GetClassName(param1));
                }
            }
            #endregion
            #region bonus2 bSubClass
            functionName = "bonus2 bSubClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับ Class " + GetClassName(param1));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับ Class " + GetClassName(param1));
                }
            }
            #endregion
            #region bonus2 bAddSize
            functionName = "bonus2 bAddSize";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bMagicAddSize
            functionName = "bonus2 bMagicAddSize";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bSubSize
            functionName = "bonus2 bSubSize";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bNoSizeFix
            functionName = "bonus bNoSizeFix";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddDamageClass
            functionName = "bonus2 bAddDamageClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddMagicDamageClass
            functionName = "bonus2 bAddMagicDamageClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddDefMonster
            functionName = "bonus2 bAddDefMonster";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddMDefMonster
            functionName = "bonus2 bAddMDefMonster";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddRace2
            functionName = "bonus2 bAddRace2";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bSubRace2
            functionName = "bonus2 bSubRace2";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bMagicAddRace2
            functionName = "bonus2 bMagicAddRace2";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bSubSkill
            functionName = "bonus2 bSubSkill";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bAbsorbDmgMaxHP
            functionName = "bonus bAbsorbDmgMaxHP";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bAtkEle
            functionName = "bonus bAtkEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bDefEle
            functionName = "bonus bDefEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bMagicAtkEle
            functionName = "bonus2 bMagicAtkEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bDefRatioAtkRace
            functionName = "bonus bDefRatioAtkRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bDefRatioAtkEle
            functionName = "bonus bDefRatioAtkEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bDefRatioAtkClass
            functionName = "bonus bDefRatioAtkClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus4 bSetDefRace
            functionName = "bonus4 bSetDefRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus4 bSetMDefRace
            functionName = "bonus4 bSetMDefRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bIgnoreDefEle
            functionName = "bonus bIgnoreDefEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bIgnoreDefRace,
            functionName = "bonus bIgnoreDefRace,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bIgnoreDefClass,
            functionName = "bonus bIgnoreDefClass,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bIgnoreMDefRace,
            functionName = "bonus bIgnoreMDefRace,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bIgnoreDefRaceRate
            functionName = "bonus2 bIgnoreDefRaceRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bIgnoreMdefRaceRate
            functionName = "bonus2 bIgnoreMdefRaceRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bIgnoreMdefRace2Rate
            functionName = "bonus2 bIgnoreMdefRace2Rate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bIgnoreMDefEle
            functionName = "bonus bIgnoreMDefEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bIgnoreDefClassRate
            functionName = "bonus2 bIgnoreDefClassRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bIgnoreMdefClassRate
            functionName = "bonus2 bIgnoreMdefClassRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bExpAddRace
            functionName = "bonus2 bExpAddRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bExpAddClass
            functionName = "bonus2 bExpAddClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddEff,
            functionName = "bonus2 bAddEff,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddEff2
            functionName = "bonus2 bAddEff2";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddEffWhenHit
            functionName = "bonus2 bAddEffWhenHit";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bResEff
            functionName = "bonus2 bResEff";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAddEff,
            functionName = "bonus3 bAddEff,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus4 bAddEff,
            functionName = "bonus4 bAddEff,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAddEffWhenHit
            functionName = "bonus3 bAddEffWhenHit";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus4 bAddEffWhenHit
            functionName = "bonus4 bAddEffWhenHit";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAddEffOnSkill
            functionName = "bonus3 bAddEffOnSkill";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus4 bAddEffOnSkill
            functionName = "bonus4 bAddEffOnSkill";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus5 bAddEffOnSkill
            functionName = "bonus5 bAddEffOnSkill";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bComaClass
            functionName = "bonus2 bComaClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bComaRace
            functionName = "bonus2 bComaRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bWeaponComaEle
            functionName = "bonus2 bWeaponComaEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bWeaponComaClass
            functionName = "bonus2 bWeaponComaClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bWeaponComaRace
            functionName = "bonus2 bWeaponComaRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAutoSpell,
            functionName = "bonus3 bAutoSpell,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAutoSpellWhenHit
            functionName = "bonus3 bAutoSpellWhenHit";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus4 bAutoSpell,
            functionName = "bonus4 bAutoSpell,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus5 bAutoSpell,
            functionName = "bonus5 bAutoSpell,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus4 bAutoSpellWhenHit
            functionName = "bonus4 bAutoSpellWhenHit";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus5 bAutoSpellWhenHit
            functionName = "bonus5 bAutoSpellWhenHit";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus4 bAutoSpellOnSkill
            functionName = "bonus4 bAutoSpellOnSkill";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus5 bAutoSpellOnSkill
            functionName = "bonus5 bAutoSpellOnSkill";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bHPDrainValue
            functionName = "bonus bHPDrainValue";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bHPDrainValueRace
            functionName = "bonus2 bHPDrainValueRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bHpDrainValueClass
            functionName = "bonus2 bHpDrainValueClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bSPDrainValue
            functionName = "bonus bSPDrainValue";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bSPDrainValueRace
            functionName = "bonus2 bSPDrainValueRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bSpDrainValueClass
            functionName = "bonus2 bSpDrainValueClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bHPDrainRate
            functionName = "bonus2 bHPDrainRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bSPDrainRate
            functionName = "bonus2 bSPDrainRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bHPVanishRate
            functionName = "bonus2 bHPVanishRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bHPVanishRaceRate
            functionName = "bonus3 bHPVanishRaceRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bHPVanishRate
            functionName = "bonus3 bHPVanishRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bSPVanishRate
            functionName = "bonus2 bSPVanishRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bSPVanishRaceRate
            functionName = "bonus3 bSPVanishRaceRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bSPVanishRate
            functionName = "bonus3 bSPVanishRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bStateNoRecoverRace
            functionName = "bonus3 bStateNoRecoverRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bHPGainValue
            functionName = "bonus bHPGainValue";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bSPGainValue
            functionName = "bonus bSPGainValue";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bSPGainRace
            functionName = "bonus2 bSPGainRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bLongHPGainValue
            functionName = "bonus bLongHPGainValue";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bLongSPGainValue
            functionName = "bonus bLongSPGainValue";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bMagicHPGainValue
            functionName = "bonus bMagicHPGainValue";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bMagicSPGainValue
            functionName = "bonus bMagicSPGainValue";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bShortWeaponDamageReturn
            functionName = "bonus bShortWeaponDamageReturn";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bLongWeaponDamageReturn
            functionName = "bonus bLongWeaponDamageReturn";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bMagicDamageReturn
            functionName = "bonus bMagicDamageReturn";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnstripableWeapon
            functionName = "bonus bUnstripableWeapon";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnstripableArmor
            functionName = "bonus bUnstripableArmor";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnstripableHelm
            functionName = "bonus bUnstripableHelm";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnstripableShield
            functionName = "bonus bUnstripableShield";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bUnstripable
            functionName = "bUnstripable";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnbreakableGarment
            functionName = "bonus bUnbreakableGarment";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnbreakableWeapon
            functionName = "bonus bUnbreakableWeapon";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "อาวุธจะไม่มีทางชำรุด");
            #endregion
            #region bonus bUnbreakableArmor
            functionName = "bonus bUnbreakableArmor";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnbreakableHelm
            functionName = "bonus bUnbreakableHelm";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnbreakableShield
            functionName = "bonus bUnbreakableShield";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnbreakableShoes
            functionName = "bonus bUnbreakableShoes";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bUnbreakable,
            functionName = "bonus bUnbreakable,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bBreakWeaponRate
            functionName = "bonus bBreakWeaponRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bBreakArmorRate
            functionName = "bonus bBreakArmorRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bDropAddRace
            functionName = "bonus2 bDropAddRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bDropAddClass
            functionName = "bonus2 bDropAddClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAddMonsterIdDropItem
            functionName = "bonus3 bAddMonsterIdDropItem";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddMonsterDropItem,
            functionName = "bonus2 bAddMonsterDropItem,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAddMonsterDropItem,
            functionName = "bonus3 bAddMonsterDropItem,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAddClassDropItem,
            functionName = "bonus3 bAddClassDropItem,";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddMonsterDropItemGroup
            functionName = "bonus2 bAddMonsterDropItemGroup";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAddMonsterDropItemGroup
            functionName = "bonus3 bAddMonsterDropItemGroup";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus3 bAddClassDropItemGroup
            functionName = "bonus3 bAddClassDropItemGroup";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bGetZenyNum
            functionName = "bonus2 bGetZenyNum";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddGetZenyNum
            functionName = "bonus2 bAddGetZenyNum";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bDoubleRate
            functionName = "bonus bDoubleRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + param1 + "% ที่จะโจมตีสองครั้งภายในครั้งเดียว (ใช้โอกาสมากสุดเท่านั้น)");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + param1 + "% ที่จะโจมตีสองครั้งภายในครั้งเดียว (ใช้โอกาสมากสุดเท่านั้น)");
                }
            }
            #endregion
            #region bonus bDoubleAddRate
            functionName = "bonus bDoubleAddRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bSplashRange
            functionName = "bonus bSplashRange";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bSplashAddRange
            functionName = "bonus bSplashAddRange";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus2 bAddSkillBlow
            functionName = "bonus2 bAddSkillBlow";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bNoKnockback
            functionName = "bonus bNoKnockback";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bNoGemStone
            functionName = "bonus bNoGemStone";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bIntravision
            functionName = "bonus bIntravision";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "มองเห็นศัตรูที่ Hidden, Cloaking ได้ตลอดเวลา");
            #endregion
            #region bonus bPerfectHide
            functionName = "bonus bPerfectHide";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bRestartFullRecover
            functionName = "bonus bRestartFullRecover";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bClassChange
            functionName = "bonus bClassChange";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bAddStealRate
            functionName = "bonus bAddStealRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bNoMadoFuel
            functionName = "bonus bNoMadoFuel";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region bonus bNoWalkDelay
            functionName = "bonus bNoWalkDelay";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
            #region nnnn
            functionName = "nnnn";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    if (isParam1Negative)
                        sum += AddDescription(sum, "nnnn -" + param1);
                    else
                        sum += AddDescription(sum, "nnnn +" + param1);
                }
            }
            #endregion
        }

        return sum;
    }

    /// <summary>
    /// Merge item script from split
    /// </summary>
    /// <param name="allCut"></param>
    /// <param name="i"></param>
    /// <param name="isRemoveWhiteSpace"></param>
    void MergeItemScripts(List<string> allCut, int i, bool isRemoveWhiteSpace = false)
    {
        if (isRemoveWhiteSpace)
        {
            allCut[i] = allCut[i].Replace(" ,", ",");
            allCut[i] = allCut[i].Replace(" ;", ";");
        }
        allCut[i] += " " + allCut[i + 1];
        allCut.RemoveAt(i + 1);
    }

    /// <summary>
    /// Cut function name out of string
    /// </summary>
    /// <param name="toCut"></param>
    /// <param name="functionName"></param>
    /// <returns></returns>
    string CutFunctionName(string toCut, string functionName, int lengthDecrease = 0)
    {
        Log("CutFunctionName >> toCut: " + toCut + " | functionName: " + functionName);

        int cutStartAt = toCut.IndexOf(functionName);

        string cut = toCut.Substring(cutStartAt + functionName.Length - lengthDecrease);

        Log("cut: " + cut);

        if (cut.Contains(";"))
        {
            int cutEndAt = cut.IndexOf(";");

            cut = cut.Substring(1, cutEndAt - 1);
        }

        Log("cut: " + cut);

        return cut;
    }

    /// <summary>
    /// Return all parameter of input parameter
    /// </summary>
    /// <param name="sumCut"></param>
    /// <returns></returns>
    List<string> GetAllParamerters(string sumCut)
    {
        isHadParam1 = false;
        isHadParam2 = false;
        isHadParam3 = false;
        isHadParam4 = false;
        isHadParam5 = false;
        isHadParam6 = false;
        isHadParam7 = false;
        isParam1Negative = false;
        isParam2Negative = false;
        isParam3Negative = false;
        isParam4Negative = false;
        isParam5Negative = false;
        isParam6Negative = false;
        isParam7Negative = false;

        List<string> allParam = new List<string>();
        if (sumCut.Contains("rand"))
        {
            allParam = new List<string>(sumCut.Split(new string[] { "," }, StringSplitOptions.None));

        L_Redo:
            for (int i = 0; i < allParam.Count; i++)
            {
                Log("(Before)allParam[" + i + "]: " + allParam[i]);
                var sumParam = allParam[i];
                if (sumParam.Contains("(") && !sumParam.Contains(")"))
                {
                    allParam[i] += "," + allParam[i + 1];
                    allParam.RemoveAt(i + 1);
                    goto L_Redo;
                }
            }

            for (int i = 0; i < allParam.Count; i++)
                Log("(After)allParam[" + i + "]: " + allParam[i]);
        }
        else if (sumCut.Contains("max"))
        {
            allParam = new List<string>(sumCut.Split(new string[] { "," }, StringSplitOptions.None));

        L_Redo:
            for (int i = 0; i < allParam.Count; i++)
            {
                Log("(Before)allParam[" + i + "]: " + allParam[i]);
                var sumParam = allParam[i];
                if (sumParam.Contains("(") && !sumParam.Contains(","))
                {
                    allParam[i] += "," + allParam[i + 1];
                    allParam.RemoveAt(i + 1);
                    goto L_Redo;
                }
            }

            for (int i = 0; i < allParam.Count; i++)
                Log("(After)allParam[" + i + "]: " + allParam[i]);
        }
        else
        {
            allParam = StringSplit.GetStringSplit(sumCut, ',');
            for (int i = 0; i < allParam.Count; i++)
            {
                if (!allParam[i].Contains("getskilllv"))
                {
                    allParam[i] = allParam[i].Replace("(", "");
                    allParam[i] = allParam[i].Replace(")", "");
                }
                Log("(After)allParam[" + i + "]: " + allParam[i]);
            }
        }
        return allParam;
    }

    /// <summary>
    /// Return value of given parameters
    /// </summary>
    /// <param name="data"></param>
    /// <param name="paramCount"></param>
    /// <returns></returns>
    string GetValue(string data, int paramCount, bool isZeroValueOkay = false)
    {
        string value = data;

        Log("GetValue: " + value);

        if (value == "INFINITE_TICK")
        {
            SetParamCheck(paramCount, true, true);
            return "ตลอดเวลา";
        }

        value = ReplaceAllSpecialValue(value);

        bool isFoundTempVariable = false;
        string tempVarName = null;
        string valueFromTempVar = null;
        for (int i = 0; i < tempVariables.Count; i++)
        {
            if (value.Contains(tempVariables[i].variableName))
            {
                isFoundTempVariable = true;
                tempVarName = tempVariables[i].variableName;
                valueFromTempVar = tempVariables[i].value;
                Log("valueFromTempVar: " + valueFromTempVar);
            }
        }
        if (isFoundTempVariable)
        {
            valueFromTempVar = ReplaceAllSpecialValue(valueFromTempVar);
            value = value.Replace(tempVarName, valueFromTempVar);
            SetParamCheck(paramCount, true, false);
            return "(" + value + ")";
        }
        //rand
        else if (value.Contains("rand"))
        {
            Log("rand");

            int paramStartAt = value.IndexOf("(");
            Log("paramStartAt: " + paramStartAt);

            string sum = value.Substring(paramStartAt);

            Log("GetValue: " + sum);

            int paramEndAt = sum.IndexOf(")");
            Log("paramEndAt: " + paramEndAt);

            sum = sum.Substring(1, paramEndAt - 1);

            Log("GetValue: " + sum);

            List<string> allValue = StringSplit.GetStringSplit(sum, ',');

            if (paramCount == 1)
                isHadParam1 = true;
            else if (paramCount == 2)
                isHadParam2 = true;
            else if (paramCount == 3)
                isHadParam3 = true;
            else if (paramCount == 4)
                isHadParam4 = true;
            else if (paramCount == 5)
                isHadParam5 = true;
            else if (paramCount == 6)
                isHadParam6 = true;
            else if (paramCount == 7)
                isHadParam7 = true;

            return allValue[0] + "~" + allValue[1];
        }
        //max
        else if (value.Contains("max"))
        {
            Log("max");

            int paramStartAt = value.IndexOf("(");
            Log("paramStartAt: " + paramStartAt);

            string sum = value.Substring(paramStartAt);

            Log("GetValue: " + sum);

            sum = sum.Substring(1, sum.Length - 1);

            Log("GetValue: " + sum);

            List<string> allValue = StringSplit.GetStringSplit(sum, ',');
            for (int i = 0; i < allValue.Count; i++)
            {
                Log("allValue[" + i + "]: " + allValue[i]);
                if (allValue[i].Contains("getskilllv"))
                    allValue[i] = GetSkillLv(allValue[i]);
            }

            if (paramCount == 1)
                isHadParam1 = true;
            else if (paramCount == 2)
                isHadParam2 = true;
            else if (paramCount == 3)
                isHadParam3 = true;
            else if (paramCount == 4)
                isHadParam4 = true;
            else if (paramCount == 5)
                isHadParam5 = true;
            else if (paramCount == 6)
                isHadParam6 = true;
            else if (paramCount == 7)
                isHadParam7 = true;

            return allValue[0] + "(มากสุด " + allValue[1] + ")";
        }
        //Normal value
        else
        {
            if (value == "+")
            {
                SetParamCheck(paramCount, true, false);
                return "1";
            }
            else if (value == "-")
            {
                SetParamCheck(paramCount, true, true);
                return "-1";
            }

            int paramInt = 0;
            bool isInteger = false;

            isInteger = int.TryParse(value, out paramInt);

            Log("isInteger: " + isInteger);

            if (!isInteger)
            {
                string newValue = value.ToUpper();

                if (newValue.Contains("SC_"))
                {
                    if (newValue.Contains("SC__"))
                    {
                        newValue = newValue.Substring(4);
                        newValue = newValue.Replace("_", " ");
                        newValue = UpperFirst(newValue);
                    }
                    else
                    {
                        newValue = newValue.Substring(3);
                        newValue = newValue.Replace("_", " ");
                        newValue = UpperFirst(newValue);
                    }

                    SetParamCheck(paramCount, true, false);

                    value = newValue;

                }
                else if (newValue.Contains("RC_"))
                {
                    if (newValue.Contains("RC__"))
                    {
                        newValue = newValue.Substring(4);
                        newValue = newValue.Replace("_", " ");
                        newValue = UpperFirst(newValue);
                    }
                    else
                    {
                        newValue = newValue.Substring(3);
                        newValue = newValue.Replace("_", " ");
                        newValue = UpperFirst(newValue);
                    }

                    SetParamCheck(paramCount, true, false);

                    value = newValue;
                }
            }
            else
            {
                if (paramInt == 0 && !isZeroValueOkay)
                    SetParamCheck(paramCount, false, false);
                else if (paramInt < 0)
                {
                    paramInt = paramInt * -1;
                    value = paramInt.ToString("f0");
                    SetParamCheck(paramCount, true, true);
                }
                else
                    SetParamCheck(paramCount, true, false);
            }

            return value;
        }
    }

    #region Utilities
    /// <summary>
    /// Get percent wording by rate and divisor
    /// </summary>
    /// <param name="rate"></param>
    /// <param name="divisor"></param>
    /// <returns></returns>
    string GetPercent(float rate, int divisor)
    {
        var sumRate = rate / divisor;
        if (rate > divisor)
            return sumRate.ToString("f0") + "%";
        else if (rate > 0)
            return sumRate.ToString("f1") + "%";
        else
            return "100%";
    }

    /// <summary>
    /// Get item name by ID
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetItemName(string data)
    {
        for (int i = 0; i < m_output.m_currentItemDbs.Count; i++)
        {
            var sum = m_output.m_currentItemDbs[i];
            if (sum.id == int.Parse(data))
                return sum.name;
        }
        return null;
    }

    /// <summary>
    /// Get weapon type
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetWeaponType(string data)
    {
        if (string.IsNullOrEmpty(data))
            return "";

        WeaponTypeFlag weaponTypeFlag = (WeaponTypeFlag)Enum.Parse(typeof(WeaponTypeFlag), data);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(WeaponTypeFlag), weaponTypeFlag) && !weaponTypeFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_FIST))
            return "ไม่มีอาวุธ";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_DAGGER))
            return "Daggers";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_1HSWORD))
            return "One-handed swords";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_2HSWORD))
            return "Two-handed swords";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_1HSPEAR))
            return "One-handed spears";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_2HSPEAR))
            return "Two-handed spears";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_1HAXE))
            return "One-handed axes";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_2HAXE))
            return "Two-handed axes";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_MACE))
            return "Maces";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_2HMACE))
            return "Two-handed Maces";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_STAFF))
            return "Staff";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_BOW))
            return "Bows";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_KNUCKLE))
            return "Knuckles";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_MUSICAL))
            return "Musical Instruments";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_WHIP))
            return "Whips";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_BOOK))
            return "Books";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_KATAR))
            return "Katars";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_REVOLVER))
            return "Revolvers";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_RIFLE))
            return "Rifles";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_GATLING))
            return "Gatling guns";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_SHOTGUN))
            return "Shotguns";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_GRENADE))
            return "Grenade launchers";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_HUUMA))
            return "Fuuma Shurikens";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_2HSTAFF))
            return "Two-handed Staff";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.MAX_WEAPON_TYPE))
            return "";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_DOUBLE_DD))
            return "Dual-wield Daggers";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_DOUBLE_SS))
            return "Dual-wield Swords";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_DOUBLE_AA))
            return "Dual-wield Axes";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_DOUBLE_DS))
            return "Dagger + Sword";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_DOUBLE_DA))
            return "Dagger + Axe";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.W_DOUBLE_SA))
            return "Sword + Axe";
        else if (weaponTypeFlag.HasFlag(WeaponTypeFlag.MAX_WEAPON_TYPE_ALL))
            return "";
        return "";
    }

    /// <summary>
    /// Milliseconds to seconds
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="digit"></param>
    /// <returns></returns>
    string TimerToStringTimer(float timer, float divider = 1000)
    {
        string sumDecimal = "f0";

        if (timer % divider != 0)
            sumDecimal = "f1";

        var sumTimer = (timer / divider);

        if (sumTimer >= 31536000)
            return (sumTimer / 31536000).ToString(sumDecimal) + " ปี";
        else if (sumTimer >= 2628000)
            return (sumTimer / 2628000).ToString(sumDecimal) + " เดือน";
        else if (sumTimer >= 604800)
            return (sumTimer / 604800).ToString(sumDecimal) + " สัปดาห์";
        else if (sumTimer >= 86400)
            return (sumTimer / 86400).ToString(sumDecimal) + " วัน";
        else if (sumTimer >= 3600)
            return (sumTimer / 3600).ToString(sumDecimal) + " ชั่วโมง";
        else if (sumTimer >= 60)
            return (sumTimer / 60).ToString(sumDecimal) + " นาที";
        else
            return sumTimer.ToString(sumDecimal) + " วินาที";
    }

    /// <summary>
    /// Get sc_start flag type
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    string Get_sc_start_Flag(int flag)
    {
        ScStartFlag scStartFlag = (ScStartFlag)Enum.Parse(typeof(ScStartFlag), flag.ToString("f0"));
        if (scStartFlag == ScStartFlag.SCSTART_LOADED)
            return "(สถานะจะคงที่)";
        else if (scStartFlag == ScStartFlag.SCSTART_NOAVOID)
            return "(ไม่สามารถยับยั้งการเกิดสถานะนี้ได้)";
        else if (scStartFlag == ScStartFlag.SCSTART_NOICON)
            return "(สถานะจะไม่แสดงเป็น Icon)";
        else if (scStartFlag == ScStartFlag.SCSTART_NORATEDEF)
            return "(โอกาสจะคงที่)";
        else
            return "";
    }

    /// <summary>
    /// Credit: https://answers.unity.com/questions/803672/capitalize-first-letter-in-textfield-only.html
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    string UpperFirst(string text)
    {
        return char.ToUpper(text[0]) +
            ((text.Length > 1) ? text.Substring(1).ToLower() : string.Empty);
    }

    /// <summary>
    /// Replace all special wording in value
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string ReplaceAllSpecialValue(string data)
    {
        string value = data;

        if (IsOneLineIfElse(value))
            value = ConvertOneLineIfElse(value);

        if (value.Contains("getskilllv"))
        {
            while (value.Contains("getskilllv"))
            {
                value = value.Replace("getskilllv", "ตามจำนวนที่เรียนรู้ Skill ");
                int circleStartAt = value.IndexOf("(");
                int circleEndAt = value.IndexOf(")");
                string sumToCut = value.Substring(circleStartAt + 1);
                int sumCircleEndAt = sumToCut.IndexOf(")");
                sumToCut = sumToCut.Substring(0, sumCircleEndAt);
                sumToCut = GetSkillName(sumToCut);
                value = value.Substring(0, circleStartAt) + value.Substring(circleEndAt + 1) + sumToCut;
            }
        }

        value = value.Replace("getrefine();", "(ตามจำนวนตีบวก)");

        value = value.Replace("getrefine", "(ตามจำนวนตีบวก)");

        value = value.Replace("readparambStr", "(ตาม STR ที่ฝึกฝน)");

        value = value.Replace("readparambAgi", "(ตาม AGI ที่ฝึกฝน)");

        value = value.Replace("readparambVit", "(ตาม VIT ที่ฝึกฝน)");

        value = value.Replace("readparambInt", "(ตาม INT ที่ฝึกฝน)");

        value = value.Replace("readparambDex", "(ตาม DEX ที่ฝึกฝน)");

        value = value.Replace("readparambLuk", "(ตาม LUK ที่ฝึกฝน)");

        value = value.Replace("BaseLevel", "(Level)");

        value = value.Replace("JobLevel", "(Job Level)");

        return value;
    }

    /// <summary>
    /// Check is one line if else
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    bool IsOneLineIfElse(string data)
    {
        if (data.Contains("?"))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Convert if else in one line
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string ConvertOneLineIfElse(string data)
    {
        data = data.Replace("getrefine", "หากจำนวนตีบวก");
        data = data.Replace(">=", " มากกว่าหรือเท่ากับ ");
        data = data.Replace("<=", " น้อยกว่าหรือเท่ากับ ");
        data = data.Replace(">", " มากกว่า ");
        data = data.Replace("<", " น้อยกว่า ");
        data = data.Replace("?", " = ");
        data = data.Replace(":", ", หากไม่ตรงเงื่อนไข = ");
        return data;
    }

    /// <summary>
    /// Get monster name from id
    /// </summary>
    /// <returns></returns>
    string GetMonsterName(int id)
    {
        for (int i = 0; i < m_output.m_currentMonsterDatabases.Count; i++)
        {
            if (id == m_output.m_currentMonsterDatabases[i].id)
                return m_output.m_currentMonsterDatabases[i].kROName;
        }
        return null;
    }

    /// <summary>
    /// Get monster name group from first given id
    /// </summary>
    /// <returns></returns>
    string GetMonsterNameGroup(string data)
    {
        List<string> allParam = new List<string>();

        if (data.Contains("rand"))
        {
            string toCut = data;
            toCut = CutFunctionName(toCut, "rand");
            toCut = toCut.Replace("(", "");
            toCut = toCut.Replace(")", "");
            allParam = new List<string>(toCut.Split(new string[] { "," }, StringSplitOptions.None));
            for (int i = 0; i < allParam.Count; i++)
                Log("allParam[" + i + "]: " + allParam[i]);
        }
        else
            return null;

        for (int i = 0; i < m_output.m_currentMonsterDatabases.Count; i++)
        {
            if (int.Parse(allParam[0]) == m_output.m_currentMonsterDatabases[i].id)
            {
                string groupName = m_output.m_currentMonsterDatabases[i].kROName;

                int monsterNameStartAt = groupName.IndexOf("_");

                string cut = groupName.Substring(monsterNameStartAt + groupName.Length);

                return groupName;
            }
        }
        return null;
    }

    /// <summary>
    /// Get wording for skill name levels
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetSkillLv(string data)
    {
        Log("GetSkillLv");

        int paramStartAt = data.IndexOf("(");
        Log("paramStartAt: " + paramStartAt);

        string sum = data.Substring(paramStartAt + 1);

        Log("GetSkillLv: " + sum);

        int paramEndAt = sum.IndexOf(")");
        Log("paramEndAt: " + paramEndAt);

        sum = sum.Substring(1, paramEndAt - 2);

        Log("GetSkillLv: " + sum);

        return " (" + GetSkillName(sum) + " ที่เรียนรู้ ";
    }

    /// <summary>
    /// Get skill name by skill id or skill name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetSkillName(string data)
    {
        int paramInt = 0;
        bool isInteger = false;

        isInteger = int.TryParse(data, out paramInt);

        if (isInteger)
        {
            for (int i = 0; i < m_output.m_currentSkillNames.Count; i++)
            {
                var sumData = m_output.m_currentSkillNames[i];
                if (sumData.id == paramInt)
                    return sumData.desc;
            }
        }
        else
        {
            for (int i = 0; i < m_output.m_currentSkillNames.Count; i++)
            {
                var sumData = m_output.m_currentSkillNames[i];
                if ("\"" + sumData.name + "\"" == data
                    || (sumData.name == data))
                    return sumData.desc;
            }
        }
        return null;
    }

    /// <summary>
    /// Get element name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetElementName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return "";

        Element elementFlag = (Element)Enum.Parse(typeof(Element), data);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(Element), elementFlag) && !elementFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (elementFlag.HasFlag(Element.Ele_Dark))
            return "Dark";
        else if (elementFlag.HasFlag(Element.Ele_Earth))
            return "Earth";
        else if (elementFlag.HasFlag(Element.Ele_Fire))
            return "Fire";
        else if (elementFlag.HasFlag(Element.Ele_Ghost))
            return "Ghost";
        else if (elementFlag.HasFlag(Element.Ele_Holy))
            return "Holy";
        else if (elementFlag.HasFlag(Element.Ele_Neutral))
            return "Neutral";
        else if (elementFlag.HasFlag(Element.Ele_Poison))
            return "Poison";
        else if (elementFlag.HasFlag(Element.Ele_Undead))
            return "Undead";
        else if (elementFlag.HasFlag(Element.Ele_Water))
            return "Water";
        else if (elementFlag.HasFlag(Element.Ele_Wind))
            return "Wind";
        else if (elementFlag.HasFlag(Element.Ele_All))
            return "ทุกธาตุ";
        return "";
    }

    /// <summary>
    /// Get race name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetRaceName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return "";

        Race raceFlag = (Race)Enum.Parse(typeof(Race), data);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(Race), raceFlag) && !raceFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (raceFlag.HasFlag(Race.RC_Angel))
            return "Angel";
        else if (raceFlag.HasFlag(Race.RC_Brute))
            return "Brute";
        else if (raceFlag.HasFlag(Race.RC_DemiHuman))
            return "DemiHuman";
        else if (raceFlag.HasFlag(Race.RC_Demon))
            return "Demon";
        else if (raceFlag.HasFlag(Race.RC_Dragon))
            return "Dragon";
        else if (raceFlag.HasFlag(Race.RC_Fish))
            return "Fish";
        else if (raceFlag.HasFlag(Race.RC_Formless))
            return "Formless";
        else if (raceFlag.HasFlag(Race.RC_Insect))
            return "Insect";
        else if (raceFlag.HasFlag(Race.RC_Plant))
            return "Plant";
        else if (raceFlag.HasFlag(Race.RC_Player))
            return "Player";
        else if (raceFlag.HasFlag(Race.RC_Undead))
            return "Undead";
        else if (raceFlag.HasFlag(Race.RC_All))
            return "ทุกเผ่า";
        return "";
    }

    /// <summary>
    /// Get class name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetClassName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return "";

        Class classFlag = (Class)Enum.Parse(typeof(Class), data);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(Class), classFlag) && !classFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (classFlag.HasFlag(Class.Class_Normal))
            return "Normal";
        else if (classFlag.HasFlag(Class.Class_Boss))
            return "Boss";
        else if (classFlag.HasFlag(Class.Class_Guardian))
            return "Guardian";
        else if (classFlag.HasFlag(Class.Class_All))
            return "ทุก Class";
        return "";
    }

    /// <summary>
    /// Get all trigger criteria from value
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetTriggerCriteria(string data)
    {
        if (string.IsNullOrEmpty(data))
            return "";

        string sum = null;

        List<string> allTriggerCriteria = StringSplit.GetStringSplit(data, '|');

        for (int i = 0; i < allTriggerCriteria.Count; i++)
        {
            TriggerCriteria triggerCriteria = (TriggerCriteria)Enum.Parse(typeof(TriggerCriteria), data);

            // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
            if (!Enum.IsDefined(typeof(Element), triggerCriteria) && !triggerCriteria.ToString().Contains(","))
                throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

            if (triggerCriteria.HasFlag(TriggerCriteria.BF_SHORT))
                sum += ", โจมตีระยะประชิด";
            if (triggerCriteria.HasFlag(TriggerCriteria.BF_LONG))
                sum += ", โจมตีระยะไกล";
            if (triggerCriteria.HasFlag(TriggerCriteria.BF_WEAPON))
                sum += ", อาวุธ";
            if (triggerCriteria.HasFlag(TriggerCriteria.BF_MAGIC))
                sum += ", เวทย์มนต์";
            if (triggerCriteria.HasFlag(TriggerCriteria.BF_MISC))
                sum += ", การโจมตีอื่น ๆ";
            if (triggerCriteria.HasFlag(TriggerCriteria.BF_NORMAL))
                sum += ", โจมตีกายภาพ";
            if (triggerCriteria.HasFlag(TriggerCriteria.BF_SKILL))
                sum += ", โจมตีเวทย์";
            if (triggerCriteria.HasFlag(TriggerCriteria.ATF_SELF))
                sum += ", เป้าหมาย: ตนเอง";
            if (triggerCriteria.HasFlag(TriggerCriteria.ATF_TARGET))
                sum += ", เป้าหมาย: ศัตรู";
            if (triggerCriteria.HasFlag(TriggerCriteria.ATF_SHORT))
                sum += ", โจมตีระยะประชิด";
            if (triggerCriteria.HasFlag(TriggerCriteria.ATF_LONG))
                sum += ", โจมตีระยะไกล";
            if (triggerCriteria.HasFlag(TriggerCriteria.ATF_WEAPON))
                sum += ", อาวุธ";
            if (triggerCriteria.HasFlag(TriggerCriteria.ATF_MAGIC))
                sum += ", เวทย์มนต์";
            if (triggerCriteria.HasFlag(TriggerCriteria.ATF_MISC))
                sum += ", การโจมตีอื่น ๆ";
        }

        Log("GetTriggerCriteria >> sum: " + sum);
        sum = sum.Substring(1);
        Log("GetTriggerCriteria >> sum: " + sum);

        return " เงื่อนไข ( " + sum + " )";
    }
    #endregion

    /// <summary>
    /// Set parameter x to true or false for adding description or not add
    /// </summary>
    /// <param name="paramCount"></param>
    /// <param name="isTrue"></param>
    void SetParamCheck(int paramCount, bool isTrue, bool isNegative)
    {
        if (paramCount == 1)
        {
            isHadParam1 = isTrue;
            isParam1Negative = isNegative;
        }
        else if (paramCount == 2)
        {
            isHadParam2 = isTrue;
            isParam2Negative = isNegative;
        }
        else if (paramCount == 3)
        {
            isHadParam3 = isTrue;
            isParam3Negative = isNegative;
        }
        else if (paramCount == 4)
        {
            isHadParam4 = isTrue;
            isParam4Negative = isNegative;
        }
        else if (paramCount == 5)
        {
            isHadParam5 = isTrue;
            isParam5Negative = isNegative;
        }
        else if (paramCount == 6)
        {
            isHadParam6 = isTrue;
            isParam6Negative = isNegative;
        }
        else if (paramCount == 7)
        {
            isHadParam7 = isTrue;
            isParam7Negative = isNegative;
        }
    }

    /// <summary>
    /// Add description with new line / no new line by string check
    /// </summary>
    /// <param name="data"></param>
    /// <param name="toAdd"></param>
    /// <returns></returns>
    string AddDescription(string data, string toAdd)
    {
        string endIf = null;
        if (isHadEndIf)
        {
            if (toAddEndIfCount >= 1)
            {
                isHadEndIf = false;
                endIf = "\n\"[สิ้นสุดหากตรงเงื่อนไข]\",";
            }
            else
                toAddEndIfCount++;
        }
        if (string.IsNullOrEmpty(data))
            return "\"" + toAdd + "\"," + endIf;
        else
            return "\n\"" + toAdd + "\"," + endIf;
    }

    void Log(object obj)
    {
        if (!Application.isPlaying)
            Debug.Log(obj);
    }
}
#endregion

#region enum
public enum SC
{
    SC_STONE, SC_FREEZE, SC_STUN, SC_SLEEP, SC_POISON, SC_CURSE, SC_SILENCE, SC_CONFUSION, SC_BLIND, SC_BLEEDING, SC_DPOISON, SC_PROVOKE, SC_ENDURE, SC_TWOHANDQUICKEN, SC_CONCENTRATE, SC_HIDING, SC_CLOAKING, SC_ENCPOISON, SC_POISONREACT, SC_QUAGMIRE, SC_ANGELUS, SC_BLESSING, SC_SIGNUMCRUCIS, SC_INCREASEAGI, SC_DECREASEAGI, SC_SLOWPOISON, SC_IMPOSITIO, SC_SUFFRAGIUM, SC_ASPERSIO, SC_BENEDICTIO, SC_KYRIE, SC_MAGNIFICAT, SC_GLORIA, SC_AETERNA, SC_ADRENALINE, SC_WEAPONPERFECTION, SC_OVERTHRUST, SC_MAXIMIZEPOWER, SC_TRICKDEAD, SC_LOUD, SC_ENERGYCOAT, SC_BROKENARMOR, SC_BROKENWEAPON, SC_HALLUCINATION, SC_WEIGHT50, SC_WEIGHT90, SC_ASPDPOTION0, SC_ASPDPOTION1, SC_ASPDPOTION2, SC_ASPDPOTION3, SC_SPEEDUP0, SC_SPEEDUP1, SC_ATKPOTION, SC_MATKPOTION, SC_WEDDING, SC_SLOWDOWN, SC_ANKLE, SC_KEEPING, SC_BARRIER, SC_STRIPWEAPON, SC_STRIPSHIELD, SC_STRIPARMOR, SC_STRIPHELM, SC_CP_WEAPON, SC_CP_SHIELD, SC_CP_ARMOR, SC_CP_HELM, SC_AUTOGUARD, SC_REFLECTSHIELD, SC_SPLASHER, SC_PROVIDENCE, SC_DEFENDER, SC_MAGICROD, SC_SPELLBREAKER, SC_AUTOSPELL, SC_SIGHTTRASHER, SC_AUTOBERSERK, SC_SPEARQUICKEN, SC_AUTOCOUNTER, SC_SIGHT, SC_SAFETYWALL, SC_RUWACH, SC_EXTREMITYFIST, SC_EXPLOSIONSPIRITS, SC_COMBO, SC_BLADESTOP_WAIT, SC_BLADESTOP, SC_FIREWEAPON, SC_WATERWEAPON, SC_WINDWEAPON, SC_EARTHWEAPON, SC_VOLCANO, SC_DELUGE, SC_VIOLENTGALE, SC_WATK_ELEMENT, SC_ARMOR, SC_ARMOR_ELEMENT_WATER, SC_NOCHAT, SC_BABY, SC_AURABLADE, SC_PARRYING, SC_CONCENTRATION, SC_TENSIONRELAX, SC_BERSERK, SC_FURY, SC_GOSPEL, SC_ASSUMPTIO, SC_BASILICA, SC_GUILDAURA, SC_MAGICPOWER, SC_EDP, SC_TRUESIGHT, SC_WINDWALK, SC_MELTDOWN, SC_CARTBOOST, SC_CHASEWALK, SC_REJECTSWORD, SC_MARIONETTE, SC_MARIONETTE2, SC_CHANGEUNDEAD, SC_JOINTBEAT, SC_MINDBREAKER, SC_MEMORIZE, SC_FOGWALL, SC_SPIDERWEB, SC_DEVOTION, SC_SACRIFICE, SC_STEELBODY, SC_ORCISH, SC_READYSTORM, SC_READYDOWN, SC_READYTURN, SC_READYCOUNTER, SC_DODGE, SC_RUN, SC_SHADOWWEAPON, SC_ADRENALINE2, SC_GHOSTWEAPON, SC_KAIZEL, SC_KAAHI, SC_KAUPE, SC_ONEHAND, SC_PRESERVE, SC_BATTLEORDERS, SC_REGENERATION, SC_DOUBLECAST, SC_GRAVITATION, SC_MAXOVERTHRUST, SC_LONGING, SC_HERMODE, SC_SHRINK, SC_SIGHTBLASTER, SC_WINKCHARM, SC_CLOSECONFINE, SC_CLOSECONFINE2, SC_DANCING, SC_ELEMENTALCHANGE, SC_RICHMANKIM, SC_ETERNALCHAOS, SC_DRUMBATTLE, SC_NIBELUNGEN, SC_ROKISWEIL, SC_INTOABYSS, SC_SIEGFRIED, SC_WHISTLE, SC_ASSNCROS, SC_POEMBRAGI, SC_APPLEIDUN, SC_MODECHANGE, SC_HUMMING, SC_DONTFORGETME, SC_FORTUNE, SC_SERVICE4U, SC_STOP, SC_SPURT, SC_SPIRIT, SC_COMA, SC_INTRAVISION, SC_INCALLSTATUS, SC_INCSTR, SC_INCAGI, SC_INCVIT, SC_INCINT, SC_INCDEX, SC_INCLUK, SC_INCHIT, SC_INCHITRATE, SC_INCFLEE, SC_INCFLEERATE, SC_INCMHPRATE, SC_INCMSPRATE, SC_INCATKRATE, SC_INCMATKRATE, SC_INCDEFRATE, SC_STRFOOD, SC_AGIFOOD, SC_VITFOOD, SC_INTFOOD, SC_DEXFOOD, SC_LUKFOOD, SC_HITFOOD, SC_FLEEFOOD, SC_BATKFOOD, SC_WATKFOOD, SC_MATKFOOD, SC_SCRESIST, SC_XMAS, SC_WARM, SC_SUN_COMFORT, SC_MOON_COMFORT, SC_STAR_COMFORT, SC_FUSION, SC_SKILLRATE_UP, SC_SKE, SC_KAITE, SC_SWOO, SC_SKA, SC_EARTHSCROLL, SC_MIRACLE, SC_MADNESSCANCEL, SC_ADJUSTMENT, SC_INCREASING, SC_GATLINGFEVER, SC_TATAMIGAESHI, SC_UTSUSEMI, SC_BUNSINJYUTSU, SC_KAENSIN, SC_SUITON, SC_NEN, SC_KNOWLEDGE, SC_SMA, SC_FLING, SC_AVOID, SC_CHANGE, SC_BLOODLUST, SC_FLEET, SC_SPEED, SC_DEFENCE, SC_INCASPDRATE, SC_INCFLEE2, SC_JAILED, SC_ENCHANTARMS, SC_MAGICALATTACK, SC_ARMORCHANGE, SC_CRITICALWOUND, SC_MAGICMIRROR, SC_SLOWCAST, SC_SUMMER, SC_EXPBOOST, SC_ITEMBOOST, SC_BOSSMAPINFO, SC_LIFEINSURANCE, SC_INCCRI, SC_INCDEF, SC_INCBASEATK, SC_FASTCAST, SC_MDEF_RATE, SC_HPREGEN, SC_INCHEALRATE, SC_PNEUMA, SC_AUTOTRADE, SC_KSPROTECTED, SC_ARMOR_RESIST, SC_SPCOST_RATE, SC_COMMONSC_RESIST, SC_SEVENWIND, SC_DEF_RATE, SC_SPREGEN, SC_WALKSPEED, SC_MERC_FLEEUP, SC_MERC_ATKUP, SC_MERC_HPUP, SC_MERC_SPUP, SC_MERC_HITUP, SC_MERC_QUICKEN, SC_REBIRTH, SC_SKILLCASTRATE, SC_DEFRATIOATK, SC_HPDRAIN, SC_SKILLATKBONUS, SC_ITEMSCRIPT, SC_S_LIFEPOTION, SC_L_LIFEPOTION, SC_JEXPBOOST, SC_IGNOREDEF, SC_HELLPOWER, SC_INVINCIBLE, SC_INVINCIBLEOFF, SC_MANU_ATK, SC_MANU_DEF, SC_SPL_ATK, SC_SPL_DEF, SC_MANU_MATK, SC_SPL_MATK, SC_FOOD_STR_CASH, SC_FOOD_AGI_CASH, SC_FOOD_VIT_CASH, SC_FOOD_DEX_CASH, SC_FOOD_INT_CASH, SC_FOOD_LUK_CASH, SC_FEAR, SC_BURNING, SC_FREEZING, SC_ENCHANTBLADE, SC_DEATHBOUND, SC_MILLENNIUMSHIELD, SC_CRUSHSTRIKE, SC_REFRESH, SC_REUSE_REFRESH, SC_GIANTGROWTH, SC_STONEHARDSKIN, SC_VITALITYACTIVATION, SC_STORMBLAST, SC_FIGHTINGSPIRIT, SC_ABUNDANCE, SC_ADORAMUS, SC_EPICLESIS, SC_ORATIO, SC_LAUDAAGNUS, SC_LAUDARAMUS, SC_RENOVATIO, SC_EXPIATIO, SC_DUPLELIGHT, SC_SECRAMENT, SC_WHITEIMPRISON, SC_MARSHOFABYSS, SC_RECOGNIZEDSPELL, SC_STASIS, SC_SPHERE_1, SC_SPHERE_2, SC_SPHERE_3, SC_SPHERE_4, SC_SPHERE_5, SC_READING_SB, SC_FREEZE_SP, SC_FEARBREEZE, SC_ELECTRICSHOCKER, SC_WUGDASH, SC_BITE, SC_CAMOUFLAGE, SC_ACCELERATION, SC_HOVERING, SC_SHAPESHIFT, SC_INFRAREDSCAN, SC_ANALYZE, SC_MAGNETICFIELD, SC_NEUTRALBARRIER, SC_NEUTRALBARRIER_MASTER, SC_STEALTHFIELD, SC_STEALTHFIELD_MASTER, SC_OVERHEAT, SC_OVERHEAT_LIMITPOINT, SC_VENOMIMPRESS, SC_POISONINGWEAPON, SC_WEAPONBLOCKING, SC_CLOAKINGEXCEED, SC_HALLUCINATIONWALK, SC_HALLUCINATIONWALK_POSTDELAY, SC_ROLLINGCUTTER, SC_TOXIN, SC_PARALYSE, SC_VENOMBLEED, SC_MAGICMUSHROOM, SC_DEATHHURT, SC_PYREXIA, SC_OBLIVIONCURSE, SC_LEECHESEND, SC_REFLECTDAMAGE, SC_FORCEOFVANGUARD, SC_SHIELDSPELL_DEF, SC_SHIELDSPELL_MDEF, SC_SHIELDSPELL_REF, SC_EXEEDBREAK, SC_PRESTIGE, SC_BANDING, SC_BANDING_DEFENCE, SC_EARTHDRIVE, SC_INSPIRATION, SC_SPELLFIST, SC_CRYSTALIZE, SC_STRIKING, SC_WARMER, SC_VACUUM_EXTREME, SC_PROPERTYWALK, SC_SWINGDANCE, SC_SYMPHONYOFLOVER, SC_MOONLITSERENADE, SC_RUSHWINDMILL, SC_ECHOSONG, SC_HARMONIZE, SC_VOICEOFSIREN, SC_DEEPSLEEP, SC_SIRCLEOFNATURE, SC_GLOOMYDAY, SC_GLOOMYDAY_SK, SC_SONGOFMANA, SC_DANCEWITHWUG, SC_SATURDAYNIGHTFEVER, SC_LERADSDEW, SC_MELODYOFSINK, SC_BEYONDOFWARCRY, SC_UNLIMITEDHUMMINGVOICE, SC_SITDOWN_FORCE, SC_NETHERWORLD, SC_CRESCENTELBOW, SC_CURSEDCIRCLE_ATKER, SC_CURSEDCIRCLE_TARGET, SC_LIGHTNINGWALK, SC_RAISINGDRAGON, SC_GT_ENERGYGAIN, SC_GT_CHANGE, SC_GT_REVITALIZE, SC_GN_CARTBOOST, SC_THORNSTRAP, SC_BLOODSUCKER, SC_SMOKEPOWDER, SC_TEARGAS, SC_MANDRAGORA, SC_STOMACHACHE, SC_MYSTERIOUS_POWDER, SC_MELON_BOMB, SC_BANANA_BOMB, SC_BANANA_BOMB_SITDOWN, SC_SAVAGE_STEAK, SC_COCKTAIL_WARG_BLOOD, SC_MINOR_BBQ, SC_SIROMA_ICE_TEA, SC_DROCERA_HERB_STEAMED, SC_PUTTI_TAILS_NOODLES, SC_BOOST500, SC_FULL_SWING_K, SC_MANA_PLUS, SC_MUSTLE_M, SC_LIFE_FORCE_F, SC_EXTRACT_WHITE_POTION_Z, SC_VITATA_500, SC_EXTRACT_SALAMINE_JUICE, SC__REPRODUCE, SC__AUTOSHADOWSPELL, SC__SHADOWFORM, SC__BODYPAINT, SC__INVISIBILITY, SC__DEADLYINFECT, SC__ENERVATION, SC__GROOMY, SC__IGNORANCE, SC__LAZINESS, SC__UNLUCKY, SC__WEAKNESS, SC__STRIPACCESSORY, SC__MANHOLE, SC__BLOODYLUST, SC_CIRCLE_OF_FIRE, SC_CIRCLE_OF_FIRE_OPTION, SC_FIRE_CLOAK, SC_FIRE_CLOAK_OPTION, SC_WATER_SCREEN, SC_WATER_SCREEN_OPTION, SC_WATER_DROP, SC_WATER_DROP_OPTION, SC_WATER_BARRIER, SC_WIND_STEP, SC_WIND_STEP_OPTION, SC_WIND_CURTAIN, SC_WIND_CURTAIN_OPTION, SC_ZEPHYR, SC_SOLID_SKIN, SC_SOLID_SKIN_OPTION, SC_STONE_SHIELD, SC_STONE_SHIELD_OPTION, SC_POWER_OF_GAIA, SC_PYROTECHNIC, SC_PYROTECHNIC_OPTION, SC_HEATER, SC_HEATER_OPTION, SC_TROPIC, SC_TROPIC_OPTION, SC_AQUAPLAY, SC_AQUAPLAY_OPTION, SC_COOLER, SC_COOLER_OPTION, SC_CHILLY_AIR, SC_CHILLY_AIR_OPTION, SC_GUST, SC_GUST_OPTION, SC_BLAST, SC_BLAST_OPTION, SC_WILD_STORM, SC_WILD_STORM_OPTION, SC_PETROLOGY, SC_PETROLOGY_OPTION, SC_CURSED_SOIL, SC_CURSED_SOIL_OPTION, SC_UPHEAVAL, SC_UPHEAVAL_OPTION, SC_TIDAL_WEAPON, SC_TIDAL_WEAPON_OPTION, SC_ROCK_CRUSHER, SC_ROCK_CRUSHER_ATK, SC_LEADERSHIP, SC_GLORYWOUNDS, SC_SOULCOLD, SC_HAWKEYES, SC_ODINS_POWER, SC_RAID, SC_FIRE_INSIGNIA, SC_WATER_INSIGNIA, SC_WIND_INSIGNIA, SC_EARTH_INSIGNIA, SC_PUSH_CART, SC_SPELLBOOK1, SC_SPELLBOOK2, SC_SPELLBOOK3, SC_SPELLBOOK4, SC_SPELLBOOK5, SC_SPELLBOOK6, SC_MAXSPELLBOOK, SC_INCMHP, SC_INCMSP, SC_PARTYFLEE, SC_MEIKYOUSISUI, SC_JYUMONJIKIRI, SC_KYOUGAKU, SC_IZAYOI, SC_ZENKAI, SC_KAGEHUMI, SC_KYOMU, SC_KAGEMUSYA, SC_ZANGETSU, SC_GENSOU, SC_AKAITSUKI, SC_STYLE_CHANGE, SC_TINDER_BREAKER, SC_TINDER_BREAKER2, SC_CBC, SC_EQC, SC_GOLDENE_FERSE, SC_ANGRIFFS_MODUS, SC_OVERED_BOOST, SC_LIGHT_OF_REGENE, SC_ASH, SC_GRANITIC_ARMOR, SC_MAGMA_FLOW, SC_PYROCLASTIC, SC_PARALYSIS, SC_PAIN_KILLER, SC_HANBOK, SC_DEFSET, SC_MDEFSET, SC_DARKCROW, SC_FULL_THROTTLE, SC_REBOUND, SC_UNLIMIT, SC_KINGS_GRACE, SC_TELEKINESIS_INTENSE, SC_OFFERTORIUM, SC_FRIGG_SONG, SC_MONSTER_TRANSFORM, SC_ANGEL_PROTECT, SC_ILLUSIONDOPING, SC_FLASHCOMBO, SC_MOONSTAR, SC_SUPER_STAR, SC_HEAT_BARREL, SC_MAGICALBULLET, SC_P_ALTER, SC_E_CHAIN, SC_C_MARKER, SC_ANTI_M_BLAST, SC_B_TRAP, SC_H_MINE, SC_QD_SHOT_READY, SC_MTF_ASPD, SC_MTF_RANGEATK, SC_MTF_MATK, SC_MTF_MLEATKED, SC_MTF_CRIDAMAGE, SC_OKTOBERFEST, SC_STRANGELIGHTS, SC_DECORATION_OF_MUSIC, SC_QUEST_BUFF1, SC_QUEST_BUFF2, SC_QUEST_BUFF3, SC_ALL_RIDING, SC_TEARGAS_SOB, SC__FEINTBOMB, SC__CHAOS, SC_CHASEWALK2, SC_VACUUM_EXTREME_POSTDELAY, SC_MTF_ASPD2, SC_MTF_RANGEATK2, SC_MTF_MATK2, SC_2011RWC_SCROLL, SC_JP_EVENT04, SC_MTF_MHP, SC_MTF_MSP, SC_MTF_PUMPKIN, SC_MTF_HITFLEE, SC_CRIFOOD, SC_ATTHASTE_CASH, SC_REUSE_LIMIT_A, SC_REUSE_LIMIT_B, SC_REUSE_LIMIT_C, SC_REUSE_LIMIT_D, SC_REUSE_LIMIT_E, SC_REUSE_LIMIT_F, SC_REUSE_LIMIT_G, SC_REUSE_LIMIT_H, SC_REUSE_LIMIT_MTF, SC_REUSE_LIMIT_ASPD_POTION, SC_REUSE_MILLENNIUMSHIELD, SC_REUSE_CRUSHSTRIKE, SC_REUSE_STORMBLAST, SC_ALL_RIDING_REUSE_LIMIT, SC_REUSE_LIMIT_ECL, SC_REUSE_LIMIT_RECALL, SC_PROMOTE_HEALTH_RESERCH, SC_ENERGY_DRINK_RESERCH, SC_NORECOVER_STATE, SC_SUHIDE, SC_SU_STOOP, SC_SPRITEMABLE, SC_CATNIPPOWDER, SC_SV_ROOTTWIST, SC_BITESCAR, SC_ARCLOUSEDASH, SC_TUNAPARTY, SC_SHRIMP, SC_FRESHSHRIMP, SC_ACTIVE_MONSTER_TRANSFORM, SC_CLOUD_KILL, SC_LJOSALFAR, SC_MERMAID_LONGING, SC_HAT_EFFECT, SC_FLOWERSMOKE, SC_FSTONE, SC_HAPPINESS_STAR, SC_MAPLE_FALLS, SC_TIME_ACCESSORY, SC_MAGICAL_FEATHER, SC_GVG_GIANT, SC_GVG_GOLEM, SC_GVG_STUN, SC_GVG_STONE, SC_GVG_FREEZ, SC_GVG_SLEEP, SC_GVG_CURSE, SC_GVG_SILENCE, SC_GVG_BLIND, SC_CLAN_INFO, SC_SWORDCLAN, SC_ARCWANDCLAN, SC_GOLDENMACECLAN, SC_CROSSBOWCLAN, SC_JUMPINGCLAN, SC_GEFFEN_MAGIC1, SC_GEFFEN_MAGIC2, SC_GEFFEN_MAGIC3, SC_MAXPAIN, SC_ARMOR_ELEMENT_EARTH, SC_ARMOR_ELEMENT_FIRE, SC_ARMOR_ELEMENT_WIND, SC_DAILYSENDMAILCNT, SC_DORAM_BUF_01, SC_DORAM_BUF_02, SC_HISS, SC_NYANGGRASS, SC_GROOMING, SC_SHRIMPBLESSING, SC_CHATTERING, SC_DORAM_WALKSPEED, SC_DORAM_MATK, SC_DORAM_FLEE2, SC_DORAM_SVSP, SC_FALLEN_ANGEL, SC_CHEERUP, SC_DRESSUP, SC_GLASTHEIM_ATK, SC_GLASTHEIM_DEF, SC_GLASTHEIM_HEAL, SC_GLASTHEIM_HIDDEN, SC_GLASTHEIM_STATE, SC_GLASTHEIM_ITEMDEF, SC_GLASTHEIM_HPSP, SC_LHZ_DUN_N1, SC_LHZ_DUN_N2, SC_LHZ_DUN_N3, SC_LHZ_DUN_N4, SC_ANCILLA, SC_EARTHSHAKER, SC_WEAPONBLOCK_ON, SC_ENTRY_QUEUE_APPLY_DELAY, SC_ENTRY_QUEUE_NOTIFY_ADMISSION_TIME_OUT, SC_EXTREMITYFIST2
}

public enum StatusEffect
{
    Eff_Bleeding, Eff_Blind, Eff_Burning, Eff_Confusion, Eff_Crystalize, Eff_Curse, Eff_DPoison,
    Eff_Fear, Eff_Freeze, Eff_Poison, Eff_Silence, Eff_Sleep, Eff_Stone, Eff_Stun
}

public enum Element
{
    Ele_Dark, Ele_Earth, Ele_Fire, Ele_Ghost, Ele_Holy, Ele_Neutral, Ele_Poison,
    Ele_Undead, Ele_Water, Ele_Wind, Ele_All
}

public enum Race
{
    RC_Angel, RC_Brute, RC_DemiHuman, RC_Demon, RC_Dragon, RC_Fish, RC_Formless,
    RC_Insect, RC_Plant, RC_Player, RC_Undead, RC_All
}

public enum MonsterRace
{
    RC2_Goblin, RC2_Kobold, RC2_Orc, RC2_Golem, RC2_Guardian, RC2_Ninja
}

public enum Class
{
    Class_Normal, Class_Boss, Class_Guardian, Class_All
}

public enum Size
{
    Size_Small, Size_Medium, Size_Large, Size_All
}

public enum TriggerCriteria
{
    BF_SHORT, BF_LONG, BF_WEAPON, BF_MAGIC, BF_MISC, BF_NORMAL, BF_SKILL,
    ATF_SELF, ATF_TARGET, ATF_SHORT, ATF_LONG, ATF_WEAPON, ATF_MAGIC, ATF_MISC
}

[Flags]
public enum SkillFlag
{
    SKILL_PERM = 0,
    SKILL_TEMP = 1,
    SKILL_TEMPLEVEL = 2,
    SKILL_PERM_GRANT = 3
}

[Flags]
public enum WeaponTypeFlag
{
    W_FIST = 0,
    W_DAGGER = 1,
    W_1HSWORD = 2,
    W_2HSWORD = 3,
    W_1HSPEAR = 4,
    W_2HSPEAR = 5,
    W_1HAXE = 6,
    W_2HAXE = 7,
    W_MACE = 8,
    W_2HMACE = 9,
    W_STAFF = 10,
    W_BOW = 11,
    W_KNUCKLE = 12,
    W_MUSICAL = 13,
    W_WHIP = 14,
    W_BOOK = 15,
    W_KATAR = 16,
    W_REVOLVER = 17,
    W_RIFLE = 18,
    W_GATLING = 19,
    W_SHOTGUN = 20,
    W_GRENADE = 21,
    W_HUUMA = 22,
    W_2HSTAFF = 23,
    MAX_WEAPON_TYPE,
    W_DOUBLE_DD, // 2 daggers
    W_DOUBLE_SS, // 2 swords
    W_DOUBLE_AA, // 2 axes
    W_DOUBLE_DS, // dagger + sword
    W_DOUBLE_DA, // dagger + axe
    W_DOUBLE_SA, // sword + axe
    MAX_WEAPON_TYPE_ALL,
}

[Flags]
public enum ScStartFlag
{
    SCSTART_NOAVOID,
    SCSTART_NOTICKDEF,
    SCSTART_LOADED,
    SCSTART_NORATEDEF,
    SCSTART_NOICON
}
#endregion

[Serializable]
public class TempVariables
{
    public string variableName;
    public string value;
}

[Serializable]
public class ItemResourceName
{
    public int id;
    public string resourceName;
}

[Serializable]
public class SkillName
{
    public int id;
    public string name;
    public string desc;
}

[Serializable]
public class MonsterDatabase
{
    public int id;
    public string name;
    public string kROName;
}

[Serializable]
public class IfElse
{
    public string _if;
    public string _else;
}