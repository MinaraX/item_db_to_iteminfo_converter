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
        currentOutput = null;
        currentItemDbData = new List<string>();
        currentItemDb = new ItemDb();
        m_lines = new List<string>();
        currentResourceNames = new List<ItemResourceName>();
        lines_resourceNames = new List<string>();
        currentItemScriptDatas = new List<ItemDbScriptData>();
        Log("Clear all");
    }
    #endregion

    #region Resource Name
    [Button]
    public void FetchResourceNameFromResourceNames()
    {
        Log("FetchResourceNameFromResourceNames: Start");
        currentResourceNames = new List<ItemResourceName>();
        FetchResourceNamesFromResourceNames(itemDatabase.m_resourceNames);
        Log("FetchResourceNameFromResourceNames: Done");
    }
    void FetchResourceNamesFromResourceNames(string data)
    {
        currentResourceNames = new List<ItemResourceName>();
        lines_resourceNames = new List<string>();
        Log("FetchResourceNamesFromResourceNames >> Parsing txt to database start");
        lines_resourceNames = StringSplit.GetStringSplit(data, '\n');
        Log("FetchResourceNamesFromResourceNames >> Parsing txt to database done");

        for (int i = 0; i < lines_resourceNames.Count; i++)
            Convert_resourceNames_ToList(lines_resourceNames[i]);
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

        currentResourceNames.Add(newCurrentResourceName);
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
        //Log("ConvertCurrentTargetArrayToItemInfo: Start");

        currentItemDbData = new List<string>();
        if (input == null)
            currentItemDbData = ConvertItemDbToListWithoutScript(m_lines[targetArray]);
        else
            currentItemDbData = ConvertItemDbToListWithoutScript(input);
        FetchItemDbScript(m_lines[targetArray]);

        //Test with full parameters
        //currentItemDbData = ConvertItemDbToListWithoutScript("501,Red_Potion,Red Potion,0,10,0,70,15:30,40,5,4,0xFFFFFFFF,63,2,0,4,30:99,1,16,{ itemheal rand(45,65),0; },{},{}");

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

        currentOutput += "[" + currentItemDb.id + "] = {"
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

        //Log("Success convert item_db id: " + currentItemDbData[0]);

        //Log("ConvertCurrentTargetArrayToItemInfo: Done");
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
        //currentItemScriptDatas = new List<ItemDbScriptData>(); //Comment this lines in production

        //Full test
        //data = "19538,Full_Moon,Full Moon,4,0,,0,,0,,0,0xFFFFFFFF,63,2,1024,,1,0,780,{ autobonus \"{ bonus bBaseAtk,50; }\",10,5000,BF_WEAPON,\"{ specialeffect2 EF_POTION_BERSERK; /* showscript */ }\"; autobonus \"{ bonus bMatk,50; }\",5,5000,BF_MAGIC,\"{ specialeffect2 EF_ENERGYCOAT; /* showscript */ }\"; },{ autobonus \"{ bonus bBaseAtk,50; }\",10,5000,BF_WEAPON,\"{ specialeffect2 EF_POTION_BERSERK; /* showscript */ }\"; autobonus \"{ bonus bMatk,50; }\",5,5000,BF_MAGIC,\"{ specialeffect2 EF_ENERGYCOAT; /* showscript */ }\"; },{ autobonus \"{ bonus bBaseAtk,50; }\",10,5000,BF_WEAPON,\"{ specialeffect2 EF_POTION_BERSERK; /* showscript */ }\"; autobonus \"{ bonus bMatk,50; }\",5,5000,BF_MAGIC,\"{ specialeffect2 EF_ENERGYCOAT; /* showscript */ }\"; }";

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

        //Log("sumScriptPath: " + sumScriptPath);

        int onEquipStartAt = sumScriptPath.IndexOf(",{");
        string sumScript = sumSaveScriptPath.Substring(0, onEquipStartAt);
        //Log("sumScript: " + sumScript);

        sumScriptPath = sumScriptPath.Substring(onEquipStartAt + 1);
        //Log("sumScriptPath: " + sumScriptPath);

        int onUnequipStartAt = sumScriptPath.IndexOf(",{");
        string sumOnEquipScript = sumSaveScriptPath2.Substring(onEquipStartAt + 1, onUnequipStartAt);
        //Log("sumOnEquipScript: " + sumOnEquipScript);

        int onUnequipEndAt = sumScriptPath.Length;
        sumScriptPath = sumScriptPath.Substring(onUnequipStartAt + 1);
        //Log("sumScriptPath: " + sumScriptPath);

        string sumOnUnequipScript = sumScriptPath;
        //Log("sumOnUnequipScript: " + sumOnUnequipScript);

        //Script
        itemDbScriptData.script = sumScript;

        //On equip script
        itemDbScriptData.onEquipScript = sumOnEquipScript;

        //On unequip script
        itemDbScriptData.onUnequipScript = sumOnUnequipScript;

        currentItemScriptDatas.Add(itemDbScriptData);
    }

    #region Item Description
    string GetName()
    {
        return currentItemDb.name;
    }
    string GetResourceName()
    {
        string copier = null;

        for (int i = 0; i < currentResourceNames.Count; i++)
        {
            var sumData = currentResourceNames[i];
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

        for (int i = 0; i < currentItemScriptDatas.Count; i++)
        {
            var sumData = currentItemScriptDatas[i];
            if (sumData.id == currentItemDb.id)
            {
                data = sumData;
                break;
            }
        }

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
                return "Staves";
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
                return "Two-handed staves";
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
        currentOutput = null;
        Log("Output cleared");
        ConvertCurrentTargetArrayToItemInfo();
        ClipboardExtension.CopyToClipboard(currentOutput);
    }

    public string _string;
    [Button]
    public void DebugConvertStringToItemInfo()
    {
        currentOutput = null;
        Log("Output cleared");
        ConvertCurrentTargetArrayToItemInfo(_string);
        ClipboardExtension.CopyToClipboard(currentOutput);
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

    //resourceName
    List<ItemResourceName> currentResourceNames = new List<ItemResourceName>();
    public List<ItemResourceName> m_currentResourceNames { get { return currentResourceNames; } set { currentResourceNames = value; } }

    //Item Script
    List<ItemDbScriptData> currentItemScriptDatas = new List<ItemDbScriptData>();
    public List<ItemDbScriptData> m_currentItemScriptDatas { get { return currentItemScriptDatas; } set { currentItemScriptDatas = value; } }

    //lines
    List<string> lines = new List<string>();
    public List<string> m_lines { get { return lines; } set { lines = value; } }
    List<string> lines_resourceNames = new List<string>();
    public List<string> m_lines_resourceNames { get { return lines_resourceNames; } set { lines_resourceNames = value; } }
    #endregion

    void Log(object obj)
    {
        Debug.Log(obj);
    }
}

#region Class
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
    #region Variable
    public int id;
    public string script;
    public string onEquipScript;
    public string onUnequipScript;
    #endregion

    #region Non-Item Related Variable
    bool isHadParam1;
    bool isHadParam2;
    bool isHadParam3;
    bool isHadParam4;
    bool isHadParam5;
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

    /// <summary>
    /// Get description by each item scripts function
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string GetDescription(string data)
    {
        //Debugger.ClearConsole();

        string sum = null;

        if (!Application.isPlaying)
            Log("GetDescription:" + data);

        //Split all space and merge it again line by line
        List<string> allCut = StringSplit.GetStringSplit(data, ' ');
    L_Redo:
        for (int i = 0; i < allCut.Count; i++)
        {
            var sumCut = allCut[i];

            if (!Application.isPlaying)
                Log(sumCut);

            if (sumCut.Contains("itemheal"))
            {
                if (sumCut.Contains("itemheal") && !sumCut.Contains(";"))
                {
                    allCut[i] += " " + allCut[i + 1];
                    allCut.RemoveAt(i + 1);
                    goto L_Redo;
                }
            }
            else if (sumCut.Contains("percentheal"))
            {
                if (sumCut.Contains("percentheal") && !sumCut.Contains(";"))
                {
                    allCut[i] += " " + allCut[i + 1];
                    allCut.RemoveAt(i + 1);
                    goto L_Redo;
                }
            }
            else if (sumCut.Contains("sc_end"))
            {
                if (sumCut.Contains("sc_end") && !sumCut.Contains(";"))
                {
                    allCut[i] += " " + allCut[i + 1];
                    allCut.RemoveAt(i + 1);
                    goto L_Redo;
                }
            }
            else if (sumCut.Contains("sc_start"))
            {
                if (sumCut.Contains("sc_start") && !sumCut.Contains(";"))
                {
                    allCut[i] += " " + allCut[i + 1];
                    allCut.RemoveAt(i + 1);
                    goto L_Redo;
                }
            }
        }

        for (int i = 0; i < allCut.Count; i++)
        {
            if (!Application.isPlaying)
                Log("allCut[" + i + "]: " + allCut[i]);

            data = allCut[i];

            string functionName = "";
            #region itemheal
            functionName = "itemheal";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1)
                    sum += AddDescription(sum, "ฟื้นฟู " + param1 + " HP");
                if (isHadParam2)
                    sum += AddDescription(sum, "ฟื้นฟู " + param2 + " SP");
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
                    sum += AddDescription(sum, "ฟื้นฟู HP " + param1 + "%");
                if (isHadParam2)
                    sum += AddDescription(sum, "ฟื้นฟู SP " + param2 + "%");
            }
            #endregion
            #region sc_end
            functionName = "sc_end";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

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

                if (!Application.isPlaying)
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

                if (!Application.isPlaying)
                {
                    Log("isHadParam1: " + isHadParam1 + " | param1: " + param1);
                    Log("isHadParam2: " + isHadParam2 + " | param2: " + param2);
                    Log("isHadParam3: " + isHadParam3 + " | param3: " + param3);
                    Log("isHadParam4: " + isHadParam4 + " | param4: " + param4);
                    Log("isHadParam5: " + isHadParam5 + " | param5: " + param5);
                }

                string timer = "";
                if (param2 == "ตลอดเวลา")
                    timer = " " + param2;
                else
                    timer = " เป็นเวลา " + SecFromMilliSec(float.Parse(param2));

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
        }

        return sum;
    }

    /// <summary>
    /// Cut function name out of string
    /// </summary>
    /// <param name="toCut"></param>
    /// <param name="functionName"></param>
    /// <returns></returns>
    string CutFunctionName(string toCut, string functionName, int lengthDecrease = 0)
    {
        if (!Application.isPlaying)
            Log("CutFunctionName >> toCut: " + toCut + " | functionName: " + functionName);

        int cutStartAt = toCut.IndexOf(functionName);

        string cut = toCut.Substring(cutStartAt + functionName.Length - lengthDecrease);

        if (!Application.isPlaying)
            Log("cut: " + cut);

        int cutEndAt = cut.IndexOf(";");

        cut = cut.Substring(1, cutEndAt - 1);

        if (!Application.isPlaying)
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

        List<string> allParam = new List<string>();
        if (sumCut.Contains("rand"))
        {
            allParam = new List<string>(sumCut.Split(new string[] { "," }, StringSplitOptions.None));

        L_Redo:
            for (int i = 0; i < allParam.Count; i++)
            {
                //Log("(Before)allParam[" + i + "]: " + allParam[i]);
                var sumParam = allParam[i];
                if (sumParam.Contains("(") && !sumParam.Contains(")"))
                {
                    allParam[i] += "," + allParam[i + 1];
                    allParam.RemoveAt(i + 1);
                    goto L_Redo;
                }
            }

            //for (int i = 0; i < allParam.Count; i++)
            //    Log("(After)allParam[" + i + "]: " + allParam[i]);
        }
        else
        {
            allParam = StringSplit.GetStringSplit(sumCut, ',');
            //for (int i = 0; i < allParam.Count; i++)
            //    Log("(After)allParam[" + i + "]: " + allParam[i]);
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

        //Log("GetValue: " + value);

        if (value == "INFINITE_TICK")
        {
            SetParamCheck(paramCount, true);
            return "ตลอดเวลา";
        }

        //rand
        if (value.Contains("rand"))
        {
            //Log("rand");

            int paramStartAt = value.IndexOf("(");
            //Log("paramStartAt: " + paramStartAt);

            string rand = value.Substring(paramStartAt);

            //Log("GetValue: " + rand);

            int paramEndAt = rand.IndexOf(")");
            //Log("paramEndAt: " + paramEndAt);

            rand = rand.Substring(1, paramEndAt - 1);

            List<string> allRand = StringSplit.GetStringSplit(rand, ',');

            //Log("GetValue: " + rand);

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

            return allRand[0] + "~" + allRand[1];
        }
        //Normal value
        else
        {
            int paramInt = 0;
            bool isInteger = false;

            isInteger = int.TryParse(value, out paramInt);

            //Log("isInteger: " + isInteger);

            if (!isInteger)
            {
                SC sc = (SC)Enum.Parse(typeof(SC), value.ToUpper());
                if (value.ToUpper() == sc.ToString().ToUpper())
                {
                    string newValue = sc.ToString();

                    if (newValue.Contains("SC__"))
                    {
                        newValue = newValue.Substring(4);
                        newValue = UpperFirst(newValue);
                    }
                    else
                    {
                        newValue = newValue.Substring(3);
                        newValue = UpperFirst(newValue);
                    }

                    SetParamCheck(paramCount, true);

                    value = newValue;
                }
            }
            else
            {
                if (paramInt <= 0 && !isZeroValueOkay)
                    SetParamCheck(paramCount, false);
                else
                    SetParamCheck(paramCount, true);
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
    /// Milliseconds to seconds
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="digit"></param>
    /// <returns></returns>
    string SecFromMilliSec(float timer)
    {
        if (timer % 1000 != 0)
            return (timer / 1000).ToString("f1") + " วินาที";
        else
            return (timer / 1000).ToString("f0") + " วินาที";
    }

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
    #endregion

    /// <summary>
    /// Set parameter x to true or false for adding description or not add
    /// </summary>
    /// <param name="paramCount"></param>
    /// <param name="isTrue"></param>
    void SetParamCheck(int paramCount, bool isTrue)
    {
        if (paramCount == 1)
            isHadParam1 = isTrue;
        else if (paramCount == 2)
            isHadParam2 = isTrue;
        else if (paramCount == 3)
            isHadParam3 = isTrue;
        else if (paramCount == 4)
            isHadParam4 = isTrue;
        else if (paramCount == 5)
            isHadParam5 = isTrue;
    }

    /// <summary>
    /// Add description with new line / no new line by string check
    /// </summary>
    /// <param name="data"></param>
    /// <param name="toAdd"></param>
    /// <returns></returns>
    string AddDescription(string data, string toAdd)
    {
        if (string.IsNullOrEmpty(data))
            return "\"" + toAdd + "\",";
        else
            return "\n\"" + toAdd + "\",";
    }

    void Log(object obj)
    {
        Debug.Log(obj);
    }
}

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
public enum ScStartFlag
{
    SCSTART_NOAVOID,
    SCSTART_NOTICKDEF,
    SCSTART_LOADED,
    SCSTART_NORATEDEF,
    SCSTART_NOICON
}

[Serializable]
public class ItemResourceName
{
    public int id;
    public string resourceName;
}
#endregion
