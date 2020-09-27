using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleSheetsToUnity;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;
using EasyButtons;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "FixedDescription", menuName = "Start/FixedDescription")]
public class FixedDescription : ScriptableObject
{
    public string associatedSheet = "1tVrEtp2IAf_cGmMKZVzxL9-Aaq86Vc4BMqB_SxdLNNA";
    public string associatedWorksheet = "Description";

    public List<Data> datas = new List<Data>();

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
    public void UpdateMethodOne()
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), OnSheetLoaded);
    }

    void OnSheetLoaded(GstuSpreadSheet ss)
    {
        datas = new List<Data>();
        for (int i = 1; i < ss.columns["A"].Count; i++)
        {
            datas.Add(new Data());
            for (int j = 0; j < ss.rows[i + 1].Count; j++)
            {
                //Debug.Log("ss.rows[i]: " + ss.rows[i + 1][j].value);
                datas[i - 1].UpdateStats(ss.rows[i + 1]);
            }
        }

        EditorUtility.SetDirty(this);
    }
}
