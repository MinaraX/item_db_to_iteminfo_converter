using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using System;

[CreateAssetMenu(fileName = "Output", menuName = "Start/Output")]
public class Output : ScriptableObject
{
    // ID,AegisName,Name,Type,Buy,Sell,Weight,ATK[:MATK],DEF,Range,Slots,Job,Class,Gender,Loc,wLV,eLV[:maxLevel],Refineable,View,{ Script },{ OnEquip_Script },{ OnUnequip_Script }
    //
    //501,Red_Potion,Red Potion,0,10,,70,,,,,0xFFFFFFFF,63,2,,,,,,{ itemheal rand(45,65),0; },{},{}

    /*
     [501] = {
		unidentifiedDisplayName = "Red Potion",
		unidentifiedResourceName = "»¡°£Æ÷¼Ç",
		unidentifiedDescriptionName = {
		},
		identifiedDisplayName = "Red Potion",
		identifiedResourceName = "»¡°£Æ÷¼Ç",
		identifiedDescriptionName = {
			"^0000CCType:^000000 Restorative",
			"^0000CCHeal:^000000 45 ~ 65 HP",
			"^0000CCWeight:^000000 7"
		},
		slotCount = 0,
		ClassNum = 0
	},
    */

    public void ConvertSpecificArrayToItemInfo(int index)
    {
        targetArrayToConvert = index;
        ConvertCurrentTargetArrayToItemInfo();
    }

    [Button]
    public void ClearAll()
    {
        targetArrayToConvert = 0;
        currentOutput = null;
        currentItemDbData = new List<string>();
        currentItemDb = new ItemDb();
        lines = new List<string>();
        Debug.Log("Clear all");
    }

    [Button]
    public void ClearAndConvertCurrentTargetArrayToItemInfo()
    {
        currentOutput = null;
        Debug.Log("Output cleared");
        ConvertCurrentTargetArrayToItemInfo();
    }

    [Button]
    public void ConvertCurrentTargetArrayToItemInfo()
    {
        //Log("ConvertCurrentTargetArrayToItemInfo: Start");

        currentItemDbData = new List<string>();
        currentItemDbData = ConvertItemDbToListWithoutScript(lines[targetArrayToConvert]);

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
                List<string> sumSplit = StringSplit(sum, ':');
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
                List<string> sumSplit = StringSplit(sum, ':');
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

        //Log("ConvertCurrentTargetArrayToItemInfo: Done");
    }
    public int targetArrayToConvert;

    string GetName()
    {
        return currentItemDb.name;
    }
    string GetResourceName()
    {
        //string copier = null;

        //[TODO] Create a resource name copier from iteminfo_Sak files

        return "»¡°£Æ÷¼Ç";
    }
    string GetDescription()
    {
        string sum = null;
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
        //Only for armor, weapon, shadow items
        bool isLocNeed = false;
        int type = currentItemDb.type;
        if (type == 4 || type == 5 || type == 12)
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
    string GetItemType()
    {
        int type = currentItemDb.type;

        if (type == 0)
            return "ของใช้ฟื้นฟู";
        else if (type == 2)
            return "ของกดใช้";
        else if (type == 3)
            return "ของอื่น ๆ";
        else if (type == 4)
            return "อุปกรณ์สวมใส่";
        else if (type == 5)
            return "อาวุธ";
        else if (type == 6)
            return "Card";
        else if (type == 7)
            return "Pet egg";
        else if (type == 8)
            return "อุปกรณ์สวมใส่ Pet";
        else if (type == 10)
            return "กระสุน";
        else if (type == 11)
            return "ของกดใช้";
        else if (type == 12)
            return "อุปกรณ์สวมใส่ Shadow";
        else if (type == 18)
            return "ของกดใช้";
        else
            return null;
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
        int weight = currentItemDb.weight;
        weight /= 10;
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

    [TextArea]
    [HideInInspector] public string currentOutput;
    public List<string> currentItemDbData = new List<string>();
    public ItemDb currentItemDb = new ItemDb();

    [HideInInspector] public List<string> lines = new List<string>();

    public TextAsset textAsset_someTextFiles;

    List<string> StringSplit(string data, char targetToSplit)
    {
        return new List<string>(data.Split(targetToSplit));
    }

    List<string> ConvertItemDbToListWithoutScript(string data)
    {
        string sum = data;
        int scriptStartAt = sum.IndexOf("{");
        sum = sum.Substring(0, scriptStartAt - 1);
        return new List<string>(sum.Split(','));
    }

    void Log(object obj)
    {
        Debug.Log(obj);
    }
}

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
    public ItemDbScript script;
    public ItemDbScript onEquipScript;
    public ItemDbScript onUnequipScript;
}

[Serializable]
public class ItemDbScript
{
}
