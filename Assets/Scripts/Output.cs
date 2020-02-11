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

    [Button]
    public void ConvertSpecificArrayToItemInfo()
    {
        Log("Convert specific array to item info start");

        currentOutput = null;
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
            currentItemDb.job = currentItemDbData[11];
        if (!string.IsNullOrEmpty(currentItemDbData[12]))
            currentItemDb._class = int.Parse(currentItemDbData[12]);
        if (!string.IsNullOrEmpty(currentItemDbData[13]))
            currentItemDb.gender = int.Parse(currentItemDbData[13]);
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

        currentOutput = "[" + currentItemDb.id + "] = {\n"
            + "unidentifiedDisplayName = \"" + GetName() + "\""
            + ",\nunidentifiedResourceName = \"" + GetResourceName() + "\""
            + ",\nunidentifiedDescriptionName = {\"" + GetDescription() + "\""
            + "},"
            + "\nidentifiedDisplayName = \"" + GetName() + "\""
            + ",\nidentifiedResourceName = \"" + GetResourceName() + "\""
            + ",\nidentifiedDescriptionName = {\"" + GetDescription() + "\""
            + "},"
            + "\nslotCount = " + GetSlotCount()
            + ",\nClassNum = " + GetClassNum() + "\n},";

        Log("Convert specific array to item info finished");
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
        return sum;
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
    public string currentOutput;
    public List<string> currentItemDbData = new List<string>();
    public ItemDb currentItemDb = new ItemDb();

    public List<string> lines = new List<string>();

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
    public string job;
    public int _class;
    public int gender;
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
