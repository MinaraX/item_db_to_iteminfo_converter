using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyButtons;

public class Converter : MonoBehaviour
{
    #region Variables
    [Header("Database")]
    public ItemDatabase itemDatabase;
    public Output output;

    [Header("UI")]
    public Color[] colors;
    public Image imgCurrent_item_db;
    public Image imgCurrent_item_combo_db;
    public Image imgCurrent_resourceNames;
    public Image imgCurrent_skillNames;
    public Image imgCurrent_mob_db;
    public Text txtCurrentProgress;
    public GameObject objConvertInProgress;

    public Button btnSync;
    public Button btnConvert;
    public Button btnSaveAs;
    #endregion

    #region delegate subscription
    void Awake()
    {
        ItemDatabase.onItemDbChanged += ItemDatabase_onItemDbChanged;
        ItemDatabase.onItemComboDbChanged += ItemDatabase_onItemComboDbChanged;
        ItemDatabase.onResourceNamesChanged += ItemDatabase_onResourceNamesChanged;
        ItemDatabase.onSkillNamesChanged += ItemDatabase_onSkillNamesChanged;
        ItemDatabase.onMobDbChanged += ItemDatabase_onMobDbChanged;
    }
    void OnDestroy()
    {
        ItemDatabase.onItemDbChanged -= ItemDatabase_onItemDbChanged;
        ItemDatabase.onItemComboDbChanged -= ItemDatabase_onItemComboDbChanged;
        ItemDatabase.onResourceNamesChanged -= ItemDatabase_onResourceNamesChanged;
        ItemDatabase.onSkillNamesChanged -= ItemDatabase_onSkillNamesChanged;
        ItemDatabase.onMobDbChanged -= ItemDatabase_onMobDbChanged;
    }
    void ItemDatabase_onItemDbChanged(bool isNull)
    {
        if (isNull)
            imgCurrent_item_db.color = colors[0];
        else
            imgCurrent_item_db.color = colors[1];
        CheckButtonState();
    }
    void ItemDatabase_onItemComboDbChanged(bool isNull)
    {
        if (isNull)
            imgCurrent_item_combo_db.color = colors[0];
        else
            imgCurrent_item_combo_db.color = colors[1];
        CheckButtonState();
    }
    void ItemDatabase_onResourceNamesChanged(bool isNull)
    {
        if (isNull)
            imgCurrent_resourceNames.color = colors[0];
        else
            imgCurrent_resourceNames.color = colors[1];
        CheckButtonState();
    }
    void ItemDatabase_onSkillNamesChanged(bool isNull)
    {
        if (isNull)
            imgCurrent_skillNames.color = colors[0];
        else
            imgCurrent_skillNames.color = colors[1];
        CheckButtonState();
    }
    void ItemDatabase_onMobDbChanged(bool isNull)
    {
        if (isNull)
            imgCurrent_mob_db.color = colors[0];
        else
            imgCurrent_mob_db.color = colors[1];
        CheckButtonState();
    }
    void CheckButtonState()
    {
        btnConvert.interactable = false;
        btnSaveAs.interactable = false;

        if (string.IsNullOrEmpty(itemDatabase.m_item_db)
            || string.IsNullOrEmpty(itemDatabase.m_item_combo_db)
            || string.IsNullOrEmpty(itemDatabase.m_resourceNames)
            || string.IsNullOrEmpty(itemDatabase.m_skillNames)
            || string.IsNullOrEmpty(itemDatabase.m_mob_db)
            || output.m_currentResourceNames.Count <= 0
            || output.m_currentSkillNames.Count <= 0
            || output.m_currentMonsterDatabases.Count <= 0)
            btnConvert.interactable = false;
        else
            btnConvert.interactable = true;

        if (string.IsNullOrEmpty(output.m_currentOutput))
            btnSaveAs.interactable = false;
        else
            btnSaveAs.interactable = true;
    }
    #endregion

    void Start()
    {
        Screen.SetResolution(480, 720, false);

        output.ClearAll();

        itemDatabase.Initialize();

        btnSync.onClick.AddListener(Sync);
        btnConvert.onClick.AddListener(Convert);
    }

    /// <summary>
    /// Sync all database
    /// </summary>
    void Sync()
    {
        output.ClearAll();
        output.ParseItemDatabase();
        output.FetchResourceName();
        output.FetchSkillName();
        output.FetchMonsterDatabase();

        //Convert no script here
        for (int i = 0; i < output.m_lines.Count; i++)
            output.ConvertSpecificArrayToItemInfo(i, true);

        //Convert item_combo_db here
        for (int i = 0; i < output.m_lines_combo.Count; i++)
            output.ConvertSpecificArrayToItemComboDB(i);

        CheckButtonState();
    }

    void Convert()
    {
        if (convertProcess != null)
            StopCoroutine(convertProcess);
        convertProcess = ConvertProcess();
        StartCoroutine(convertProcess);
    }

    IEnumerator convertProcess;
    IEnumerator ConvertProcess()
    {
        objConvertInProgress.SetActive(true);

        bool isSlowConvert = true;
        if (Input.GetKey(KeyCode.LeftShift))
            isSlowConvert = false;
        yield return null;

        //Start process
        Log("Converter: Start");

        output.ParseItemDatabase();

        //Convert here
        for (int i = 0; i < output.m_lines.Count; i++)
        {
            output.ConvertSpecificArrayToItemInfo(i);
            txtCurrentProgress.text = "Converted " + i + "/" + output.m_lines.Count;
            if (isSlowConvert)
                yield return null;
        }

        //Finished
        Log("Converter: Done");

        yield return null;

        objConvertInProgress.SetActive(false);

        Log("output.m_lines.Count: " + output.m_lines.Count);

        CheckButtonState();
    }

    void Log(object obj)
    {
        Debug.Log(obj);
    }

    public void Donate()
    {
        Application.OpenURL("https://kanintemsrisukgames.wordpress.com/2019/04/05/support-kt-games/");
    }
}
