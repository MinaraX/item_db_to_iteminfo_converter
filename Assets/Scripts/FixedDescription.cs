using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleSheetsToUnity;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;
using EasyButtons;
using com.spacepuppy.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "FixedDescription", menuName = "Start/FixedDescription")]
public class FixedDescription : ScriptableObject
{
    public string associatedSheet = "1tVrEtp2IAf_cGmMKZVzxL9-Aaq86Vc4BMqB_SxdLNNA";
    public string associatedWorksheet = "Description";

    public VisibleData datas = new VisibleData();

    public Data GetData(int id)
    {
        if (datas.ContainsKey(id))
            return datas[id];
        else
            return null;
    }

    [Serializable]
    public class VisibleData : SerializableDictionaryBase<int, Data>
    {
    }

    [Serializable]
    public class Data
    {
        public int id;
        public string desc;
        public string resName;

        public void UpdateStats(List<GSTU_Cell> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                switch (list[i].columnId)
                {
                    case "ID":
                        {
                            id = int.Parse(list[i].value);
                            break;
                        }
                    case "DESC":
                        {
                            desc = list[i].value;
                            break;
                        }
                    case "RES":
                        {
                            resName = list[i].value;
                            break;
                        }
                }
            }
        }
    }

    [Button]
    public void Sync()
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), OnSheetLoaded);
    }

    void OnSheetLoaded(GstuSpreadSheet ss)
    {
        datas = new VisibleData();

        for (int i = 1; i < ss.columns["A"].Count; i++)
        {
            int id = int.Parse(ss.rows[i + 1][0].value);
            datas.Add(id, new Data());
            for (int j = 0; j < ss.rows[i + 1].Count; j++)
            {
                //Debug.Log("ss.rows[i]: " + ss.rows[i + 1][j].value);
                datas[id].UpdateStats(ss.rows[i + 1]);
            }
        }

        EditorUtility.SetDirty(this);

        onGoogleSynced?.Invoke();
    }
    public delegate void NoArg();
    public static event NoArg onGoogleSynced;
}
