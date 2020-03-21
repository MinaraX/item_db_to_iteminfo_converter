using System.Collections.Generic;
using UnityEngine;
using System;

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

    bool isUseAkaTempVar = false;
    List<TempVariables> tempVariables = new List<TempVariables>();
    #endregion

    #region Get Description Functions
    string sumScript;
    public string GetScriptDescription()
    {
        sumScript = null;

        script = CorrectScriptToConvert(script);

        string sumDesc = GetScriptsDescription(script);
        sumScript = sumDesc;

        if (!string.IsNullOrEmpty(sumScript))
        {
            sumScript = sumScript.Replace("[", "(");
            sumScript = sumScript.Replace("]", ")");
        }

        return sumScript;
    }
    string sumEquipScript;
    public string GetOnEquipScriptDescription()
    {
        sumEquipScript = null;

        onEquipScript = CorrectScriptToConvert(onEquipScript);

        string sumDesc = GetScriptsDescription(onEquipScript);
        if (!string.IsNullOrEmpty(sumScript) && !string.IsNullOrEmpty(sumDesc) && !string.IsNullOrWhiteSpace(sumDesc))
            sumEquipScript = "\n[เมื่อสวมใส่]\n" + sumDesc;
        else if (!string.IsNullOrEmpty(sumDesc) && !string.IsNullOrWhiteSpace(sumDesc))
            sumEquipScript = "[เมื่อสวมใส่]\n" + sumDesc;
        else
            sumEquipScript = sumDesc;

        if (!string.IsNullOrEmpty(sumEquipScript))
        {
            sumEquipScript = sumEquipScript.Replace("[", "(");
            sumEquipScript = sumEquipScript.Replace("]", ")");
        }

        return sumEquipScript;
    }
    string sumUnequipScript;
    public string GetOnUnequipScriptDescription()
    {
        sumUnequipScript = null;

        onUnequipScript = CorrectScriptToConvert(onUnequipScript);

        string sumDesc = GetScriptsDescription(onUnequipScript);
        if (!string.IsNullOrEmpty(sumEquipScript) && !string.IsNullOrEmpty(sumDesc) && !string.IsNullOrWhiteSpace(sumDesc))
            sumUnequipScript = "\n[เมื่อถอด]\n" + sumDesc;
        else if (!string.IsNullOrEmpty(sumDesc) && !string.IsNullOrWhiteSpace(sumDesc))
            sumUnequipScript = "[เมื่อถอด]\n" + sumDesc;
        else
            sumUnequipScript = sumDesc;

        if (!string.IsNullOrEmpty(sumUnequipScript))
        {
            sumUnequipScript = sumUnequipScript.Replace("[", "(");
            sumUnequipScript = sumUnequipScript.Replace("]", ")");
        }

        return sumUnequipScript;
    }

    /// <summary>
    /// Fix non-space, etc. to start covnerting
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string CorrectScriptToConvert(string data)
    {
        string sum = data;
        sum = sum.Replace("{", " { ");
        sum = sum.Replace("}", " } ");
        sum = sum.Replace("else", "else ");
        sum = sum.Replace(";", "; ");
        while (sum.Contains("  "))
            sum = sum.Replace("  ", " ");
        sum = sum.Replace("bonus bDelayrate", "bonus bDelayRate");
        sum = sum.Replace("bonus buseSPRate", "bonus bUseSPrate");

        //Replace temporary variables in autobonus or bonus_script
        while (sum.Contains("\"+.@") && (sum.Contains("autobonus") || sum.Contains("bonus_script")))
        {
            sum = sum.Replace("\"+", "");
            sum = sum.Replace("+\"", "");
        }
        return sum;
    }
    #endregion

    /// <summary>
    /// Get description by each item scripts function
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string GetScriptsDescription(string data, bool isNotClearTempVar = false)
    {
        isUseAkaTempVar = false;

        if (!isNotClearTempVar)
            tempVariables = new List<TempVariables>();

        string sum = null;

        string scripts = data;

        Log("scripts:\n" + scripts);

        #region Remove specialeffect & specialeffect2
        while (scripts.Contains("specialeffect"))
        {
            if (scripts.Contains("specialeffect2"))
            {
                int specialEffectStartAt = scripts.IndexOf("specialeffect2");
                if (specialEffectStartAt != -1)
                {
                    int specialEffectEndAt = scripts.IndexOf(";", specialEffectStartAt);
                    if (specialEffectEndAt != -1)
                    {
                        string leftSideString = scripts.Substring(0, specialEffectStartAt);
                        //Log("Remove specialeffect >> leftSideString: " + leftSideString);
                        string rightSideString = scripts.Substring(specialEffectEndAt + 1);
                        //Log("Remove specialeffect >> rightSideString: " + rightSideString);
                        scripts = leftSideString + rightSideString;
                        //Log("Remove specialeffect >> data: " + data);
                    }
                }
            }
            else if (scripts.Contains("specialeffect"))
            {
                int specialEffectStartAt = scripts.IndexOf("specialeffect");
                if (specialEffectStartAt != -1)
                {
                    int specialEffectEndAt = scripts.IndexOf(";", specialEffectStartAt);
                    if (specialEffectEndAt != -1)
                    {
                        string leftSideString = scripts.Substring(0, specialEffectStartAt);
                        //Log("Remove specialeffect >> leftSideString: " + leftSideString);
                        string rightSideString = scripts.Substring(specialEffectEndAt + 1);
                        //Log("Remove specialeffect >> rightSideString: " + rightSideString);
                        scripts = leftSideString + rightSideString;
                        //Log("Remove specialeffect >> data: " + data);
                    }
                }
            }
        }
        #endregion

        Log("scripts:\n" + scripts);

        #region First phase merge
        //Split all space
        List<string> lines = StringSplit.GetStringSplit(scripts, ' ');

    L_MergeAutoBonusLines:
        for (int i = 0; i < lines.Count; i++)
        {
            Log("Lines #" + i + ":\n" + lines[i], true, "#D0EADE");

            string currentLine = lines[i];

            //Merge autobonus / bonus_script
            if ((currentLine.Contains("autobonus") || currentLine.Contains("bonus_script")) && !currentLine.Contains("[END_MERGE]"))
            {
                Log("Found autobonus or bonus_script", true, "yellow");

                if (!currentLine.Contains("}"))
                {
                    lines[i] = lines[i] + lines[i + 1];
                    lines.RemoveAt(i + 1);
                    goto L_MergeAutoBonusLines;
                }
                else
                {
                    bool isRemerge = true;

                    bool isAutoBonus3Check = false;
                    bool isContainAutoBonus3 = currentLine.Contains("autobonus3");

                    int count = 0;

                    int toCount = 2;
                    if (isContainAutoBonus3)
                        toCount = 3;

                    var targetToCount = '"';

                    foreach (var c in currentLine)
                    {
                        if (c == targetToCount)
                        {
                            count++;
                            if (count >= 2 && targetToCount == '"' && !isRemerge && !isAutoBonus3Check)
                            {
                                count = 0;
                                targetToCount = ',';
                            }
                            else if (count >= 2 && targetToCount == '"' && isRemerge)
                            {
                                if (isContainAutoBonus3 && !isAutoBonus3Check)
                                {
                                    isAutoBonus3Check = true;
                                    isRemerge = true;
                                    count = 0;
                                    targetToCount = '"';
                                }
                                else if (isContainAutoBonus3 && isAutoBonus3Check)
                                {
                                    isRemerge = false;
                                    break;
                                }
                                else if (!isContainAutoBonus3)
                                    isRemerge = false;
                            }
                            else if (count >= toCount && targetToCount == ',')
                            {
                                isRemerge = false;
                                count = 0;
                                targetToCount = '"';
                            }
                        }
                    }

                    Log("isRemerge: " + isRemerge, true);

                    if (isRemerge || currentLine[currentLine.Length - 1] != ';')
                    {
                        Log("currentLine last char != ;", true);

                        if (i + 1 < lines.Count)
                        {
                            lines[i] = lines[i] + lines[i + 1];
                            lines.RemoveAt(i + 1);
                            goto L_MergeAutoBonusLines;
                        }
                    }
                    else
                    {
                        lines[i] = lines[i].Replace("\"", "");
                        lines[i] = lines[i].Replace("{", "[");
                        lines[i] = lines[i].Replace("}", "]");
                        lines[i] = "[END_MERGE]" + lines[i] + "[/END_MERGE]";
                    }
                }
            }

            lines[i] = lines[i].Replace("end;", "[TXT_END];");

            Log("Lines #" + i + ":\n" + lines[i], true, "#4FEDA2");
        }
        #endregion

        //Check temporary variables in merged autobonus
        for (int i = 0; i < lines.Count; i++)
        {
            if ((lines[i].Contains("autobonus") || lines[i].Contains("bonus_script")) && lines[i].Contains("[END_MERGE]"))
                SearchForTemporaryVariables(lines[i]);
        }

    #region Merge it again line by line
    L_MergeLines:
        for (int i = 0; i < lines.Count; i++)
        {
            var currentLine = lines[i];
            string nextLine = null;
            if (i + 1 < lines.Count)
                nextLine = lines[i + 1];

            //Remove null array
            if (string.IsNullOrEmpty(currentLine) || string.IsNullOrWhiteSpace(currentLine))
            {
                lines.RemoveAt(i);
                goto L_MergeLines;
            }

            if (i == 0)
                Log("_________________", false, "red");

            Log("Lines #" + i + ":\n" + currentLine, false, "#DEC9FF");

            if (currentLine == "if" && !currentLine.Contains("(") && nextLine.Contains("("))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("if(") || currentLine.Contains("if (") || currentLine.Contains("else if(") || currentLine.Contains("else if ("))
            {
                if (!IsEven.IsCircleEven(currentLine))
                {
                    MergeItemScripts(lines, i);
                    goto L_MergeLines;
                }

                if (currentLine.Contains("{") && currentLine.Contains("}") && currentLine.Contains(";"))
                {
                    Log("currentLine contains {} and ;", true);

                    if (IsContainsTemporaryVariables(lines[i], lines, i + 1))
                        goto L_MergeLines;
                }
                else if (!nextLine.Contains("{") && !nextLine.Contains("}") && !currentLine.Contains(";"))
                {
                    Log("nextLine not contains {} and currentLine not contains ;", true);

                    //Remove this line when next 2 arrays is empty
                    if (i + 1 < lines.Count && (string.IsNullOrEmpty(nextLine) || string.IsNullOrWhiteSpace(nextLine)))
                    {
                        if (i + 2 < lines.Count && (string.IsNullOrEmpty(lines[i + 2]) || string.IsNullOrWhiteSpace(lines[i + 2])))
                        {
                            lines[i] = "";
                            goto L_MergeLines;
                        }
                    }

                    //Merge next line first
                    if (!nextLine.Contains(";") && !string.IsNullOrEmpty(nextLine))
                    {
                        MergeItemScripts(lines, i + 1);
                        goto L_MergeLines;
                    }
                    else if (string.IsNullOrEmpty(nextLine))
                    {
                        lines[i] = "";
                        goto L_MergeLines;
                    }

                    int loop = 1;
                    while (!lines[i + loop].Contains(";"))
                        loop++;

                    lines[i] = lines[i] + ";";

                    bool isNoAdd = false;
                    int indexToFindElse = i + loop + 1;
                    if (indexToFindElse < lines.Count)
                    {
                        if (lines[indexToFindElse].Contains("else"))
                            isNoAdd = true;
                    }

                    if (!isNoAdd)
                        lines.Insert(i + 2 + loop, "[TXT_END_IF];");
                }
                else if (!nextLine.Contains("{") && !nextLine.Contains("}") && currentLine.Contains(";"))
                {
                    Log("nextLine not contains {} and currentLine contains ;", true);

                    if (IsContainsTemporaryVariables(nextLine, lines, i + 1))
                        goto L_MergeLines;
                }
                else if (nextLine.Contains("{") && !nextLine.Contains("}"))
                {
                    if (!currentLine.Contains(";"))
                        lines[i] = lines[i] + ";";
                    lines[i + 1] += " " + lines[i + 2];
                    lines.RemoveAt(i + 2);
                    goto L_MergeLines;
                }
                else if (nextLine.Contains("{") && nextLine.Contains("}"))
                {
                    Log("nextLine Contains {:\n" + nextLine, true);

                    if (!IsEven.IsCurlyEven(nextLine))
                    {
                        lines[i + 1] += " " + lines[i + 2];
                        lines.RemoveAt(i + 2);
                        goto L_MergeLines;
                    }
                    else
                    {
                        if (IsContainsTemporaryVariables(nextLine, lines, i))
                            goto L_MergeLines;

                        //Remove start { and end }
                        lines[i + 1] = lines[i + 1].Substring(1, lines[i + 1].Length - 2);

                        lines[i + 1] = MergeWhiteSpace.RemoveWhiteSpace(lines[i + 1]);

                        bool isNeedRedo = false;
                        int additionalEndIfIndex = 0;
                        string cutRemains = null;

                        //Find if in if and extract it out from this array to solve conflict
                        if (lines[i + 1].Contains("if("))
                        {
                            int ifStartAt = lines[i + 1].IndexOf("if(");

                            string cut = lines[i + 1].Substring(ifStartAt);
                            Log("cut:\n" + cut, true);

                            lines[i + 1] = lines[i + 1].Substring(0, ifStartAt);
                            Log("Lines #" + (i + 1) + ":\n" + lines[i + 1], true, "yellow");

                            //Contains {
                            if (cut.Contains("{"))
                            {
                                //Count { and save first { index
                                int countCutLeftCurly = 0;
                                int cutleftCurlyIndex = cut.IndexOf("{");
                                for (int j = 0; j < cut.Length; j++)
                                {
                                    if (cut[j] == '{')
                                        countCutLeftCurly++;
                                    else if (cut[j] == '}')
                                        break;
                                }

                                //Find } by { count and save last } index
                                int countCutRightCurly = 0;
                                int cutRightCurlyIndex = 0;
                                for (int j = 0; j < cut.Length; j++)
                                {
                                    if (cut[j] == '}')
                                        countCutRightCurly++;
                                    if (countCutRightCurly == countCutLeftCurly)
                                    {
                                        cutRightCurlyIndex = j;
                                        break;
                                    }
                                }

                                string cutConditions = cut.Substring(0, cutleftCurlyIndex);

                                string cutScripts = cut.Substring(cutleftCurlyIndex);

                                //Find } by { count
                                int countRoomRightCurly = 0;
                                int roomRightCurlyIndex = 0;
                                for (int j = 0; j < cutScripts.Length; j++)
                                {
                                    if (cutScripts[j] == '}')
                                        countRoomRightCurly++;
                                    if (countRoomRightCurly == countCutLeftCurly)
                                    {
                                        roomRightCurlyIndex = j;
                                        break;
                                    }
                                }

                                cutScripts = cutScripts.Substring(0, roomRightCurlyIndex + 1);

                                cutRemains = cut.Substring(cutRightCurlyIndex + 1);

                                Log("cutConditions:\n" + cutConditions, true);
                                Log("cutScripts:\n" + cutScripts, true);
                                Log("cutRemains:\n" + cutRemains, true);

                                lines.Insert(i + 2, "[TXT_END_IF];");

                                //Split it out and insert it
                                if (cutRemains.Contains("if("))
                                {
                                    var cutRemainLines = StringSplit.GetStringSplitMoreThanOneChar(cutRemains, new string[] { "if(" });
                                L_RemergeRemainLines1:
                                    for (int j = 0; j < cutRemainLines.Count; j++)
                                    {
                                        if (!string.IsNullOrEmpty(cutRemainLines[j]) && !string.IsNullOrWhiteSpace(cutRemainLines[j]))
                                            cutRemainLines[j] = "if(" + cutRemainLines[j];
                                        else
                                        {
                                            cutRemainLines.RemoveAt(j);
                                            goto L_RemergeRemainLines1;
                                        }
                                    }

                                L_RemergeRemainLines2:
                                    for (int j = 0; j < cutRemainLines.Count; j++)
                                    {
                                        //Log(cutRemainLines[j],false,"yellow");
                                        if (!string.IsNullOrEmpty(cutRemainLines[j]) && !string.IsNullOrWhiteSpace(cutRemainLines[j]))
                                        {
                                            //Example: if(readparam(bInt)>=90){bonusbMatk,(.@r>=9)?50:30;}

                                            //Split if and {}
                                            if (cutRemainLines[j].Contains("if(") && cutRemainLines[j].Contains("{"))
                                            {
                                                //Count {}
                                                if (!IsEven.IsCurlyEven(cutRemainLines[j]))
                                                {
                                                    cutRemainLines[j] += cutRemainLines[j + 1];
                                                    cutRemainLines.RemoveAt(j + 1);
                                                    goto L_RemergeRemainLines2;
                                                }
                                                else
                                                {
                                                    //Split now
                                                    //Log("Before:\n" + cutRemainLines[j]);
                                                    int firstCurlyIndex = cutRemainLines[j].IndexOf('{');
                                                    string saveTxt = cutRemainLines[j];
                                                    cutRemainLines[j] = cutRemainLines[j].Substring(0, firstCurlyIndex);

                                                    //Find and split if inside if?
                                                    string toInsert = saveTxt.Substring(firstCurlyIndex);
                                                    if (toInsert.Contains("if("))
                                                    {
                                                        int startIfIndex = 0;
                                                        for (int k = 0; k < toInsert.Length; k++)
                                                        {
                                                            if (toInsert[k] == 'i' && toInsert[k + 1] == 'f' && toInsert[k + 2] == '(')
                                                            {
                                                                startIfIndex = k;
                                                                break;
                                                            }
                                                        }
                                                        string foundIf = toInsert.Substring(startIfIndex);
                                                        foundIf = foundIf.Substring(0, foundIf.IndexOf('}') + 1);
                                                        cutRemainLines.Insert(j + 1, "[TXT_END_IF];");
                                                        cutRemainLines.Insert(j + 1, foundIf);
                                                        toInsert = toInsert.Replace(foundIf, "");
                                                        cutRemainLines.Insert(j + 1, toInsert + "[TXT_NO_END_IF_BELOW]");
                                                    }
                                                    else
                                                        cutRemainLines.Insert(j + 1, toInsert);

                                                    //Log("After:\n" + cutRemainLines[j]);
                                                    //Log("Insert:\n" + cutRemainLines[j + 1]);

                                                    goto L_RemergeRemainLines2;
                                                }
                                            }
                                            //Example: if(readparam(bInt)>=90)bonusbMatk,(.@r>=9)?50:30;

                                            //Split if and last ) +1
                                            else if (cutRemainLines[j].Contains("if(") && cutRemainLines[j][cutRemainLines[j].Length - 1] != ')')
                                            {
                                                //Split now
                                                Log("(Before):\n" + cutRemainLines[j]);
                                                int firstCircleIndex = cutRemainLines[j].IndexOf('(');
                                                string saveTxt = cutRemainLines[j];
                                                cutRemainLines[j] = cutRemainLines[j].Substring(0, firstCircleIndex);
                                                cutRemainLines.Insert(j + 1, saveTxt.Substring(firstCircleIndex));
                                                Log("(After):\n" + cutRemainLines[j]);
                                                Log("(Insert):\n" + cutRemainLines[j + 1]);
                                                goto L_RemergeRemainLines2;
                                            }
                                            //Split if and last ) +1
                                            else if (cutRemainLines[j].Contains("if(") && !cutRemainLines[j].Contains(";"))
                                                cutRemainLines[j] = cutRemainLines[j] + ";";
                                        }
                                        else
                                        {
                                            cutRemainLines.RemoveAt(j);
                                            goto L_RemergeRemainLines2;
                                        }
                                    }

                                    cutRemainLines.Reverse();

                                    for (int j = 0; j < cutRemainLines.Count; j++)
                                        lines.Insert(i + 2, cutRemainLines[j]);
                                }
                                else
                                    lines.Insert(i + 2, cutRemains);
                                lines.Insert(i + 2, cutScripts);
                                lines.Insert(i + 2, cutConditions + ";");
                                additionalEndIfIndex += 2;
                            }
                            //Otherwise use old code
                            else
                            {
                                lines.Insert(i + 2, "[TXT_END_IF];");
                                lines.Insert(i + 2, cut);
                                additionalEndIfIndex++;
                            }
                            isNeedRedo = true;
                        }
                        else
                            lines.Insert(i + 2, "[TXT_END_IF];");

                        var splitBonus = new List<string>();

                        int index = 1;

                        //Do not spilt if first word is [END_MERGE] and last word is [/END_MERGE]  
                        bool isFoundEndMergeStart = false;
                        bool isFoundEndMergeEnd = false;
                        if (lines[i + index].Contains("[END_MERGE]"))
                        {
                            string findWord = null;
                            foreach (var c in lines[i + index])
                            {
                                findWord += c;
                                if (findWord == "[END_MERGE]")
                                {
                                    isFoundEndMergeStart = true;
                                    break;
                                }
                            }
                            findWord = null;
                            for (int j = lines[i + index].Length - 1; j > 0; j--)
                            {
                                findWord = lines[i + index][j] + findWord;
                                if (findWord == "[/END_MERGE]")
                                {
                                    isFoundEndMergeEnd = true;
                                    break;
                                }
                            }
                        }

                        if (!isFoundEndMergeStart && !isFoundEndMergeEnd)
                        {
                            while (lines[i + index].Contains("autobonus") || lines[i + index].Contains("bonus_script"))
                            {
                                string cutAutoBonus = null;

                                lines[i + index] = lines[i + index].Replace("[/END_MERGE]", "[/END_MERGE] ");

                                int startAutoBonusIndex = lines[i + index].IndexOf("[END_MERGE]");
                                int endAutoBonusIndex = lines[i + index].IndexOf("[/END_MERGE]");

                                cutAutoBonus = lines[i + index].Substring(startAutoBonusIndex + 11);
                                Log("cutAutoBonus:\n" + cutAutoBonus, true);

                                cutAutoBonus = cutAutoBonus.Substring(0, cutAutoBonus.IndexOf("[/END_MERGE]"));
                                Log("cutAutoBonus:\n" + cutAutoBonus, true);

                                lines.Insert(i + index + additionalEndIfIndex, cutAutoBonus);//TODO >> Change index to index -1?

                                Log("Lines #" + (i + index) + ":\n" + lines[i + index], true);

                                lines[i + index] = lines[i + index].Substring(0, startAutoBonusIndex) + lines[i + index].Substring(endAutoBonusIndex + 12);

                                Log("Lines #" + (i + index) + ":\n" + lines[i + index], true);

                                index++;
                            }
                        }

                        if (isFoundEndMergeStart && isFoundEndMergeEnd)
                            splitBonus.Add(lines[i + index]);
                        else
                        {
                            splitBonus = StringSplit.GetStringSplit(lines[i + index], ';');

                        //Then re-add ';'
                        L_MergeSplitBonus:
                            for (int j = 0; j < splitBonus.Count; j++)
                            {
                                if (splitBonus[j] == "" || splitBonus[j] == " " || string.IsNullOrEmpty(splitBonus[j]) || string.IsNullOrWhiteSpace(splitBonus[j]))
                                {
                                    splitBonus.RemoveAt(j);
                                    goto L_MergeSplitBonus;
                                }
                                else
                                {
                                    if (splitBonus[j].Contains("[END_MERGE]") && !splitBonus[j].Contains("[/END_MERGE]"))
                                    {
                                        if (j + 1 < splitBonus.Count)
                                        {
                                            splitBonus[j] = splitBonus[j] + ";" + splitBonus[j + 1];
                                            splitBonus.RemoveAt(j + 1);
                                            goto L_MergeSplitBonus;
                                        }
                                    }
                                    if (splitBonus[j][splitBonus[j].Length - 1] != ';')
                                        splitBonus[j] = splitBonus[j] + ";";
                                }
                            }
                        }

                        foreach (var bonus in splitBonus)
                            Log("splitBonus:\n" + bonus, false, "yellow");

                        //Set next index to splitBonus[0]
                        if (splitBonus.Count > 0)
                            lines[i + index] = splitBonus[0];

                        if (splitBonus.Count > 1)
                        {
                            for (int j = 1; j < splitBonus.Count; j++)
                                lines.Insert(i + index + j, splitBonus[j]);
                        }

                        //foreach (var item in lines)
                        //    Log(item, false, "orange");

                        if (isNeedRedo)
                            goto L_MergeLines;
                    }
                }
            }
            else if (currentLine.Contains("else"))
            {
                if (nextLine.Contains("if"))
                {
                    MergeItemScripts(lines, i);
                    goto L_MergeLines;
                }
                else if (currentLine.Contains("if") && nextLine.Contains("("))
                {
                    MergeItemScripts(lines, i);
                    goto L_MergeLines;
                }
                else if (!nextLine.Contains("{") && !nextLine.Contains("}") && !currentLine.Contains(";"))
                {
                    Log("[ELSE] nextLine not contains {} and currentLine not contains ;", true);

                    lines[i] = "[TXT_ELSE];";

                    //Remove this line when next 2 arrays is empty
                    if (i + 1 < lines.Count && (string.IsNullOrEmpty(nextLine) || string.IsNullOrWhiteSpace(nextLine)))
                    {
                        if (i + 2 < lines.Count && (string.IsNullOrEmpty(lines[i + 2]) || string.IsNullOrWhiteSpace(lines[i + 2])))
                        {
                            lines[i] = "";
                            goto L_MergeLines;
                        }
                    }

                    //Merge next line first
                    if (!nextLine.Contains(";") && !string.IsNullOrEmpty(nextLine))
                    {
                        MergeItemScripts(lines, i + 1);
                        goto L_MergeLines;
                    }
                    else if (string.IsNullOrEmpty(nextLine))
                    {
                        lines[i] = "";
                        goto L_MergeLines;
                    }

                    int loop = 1;
                    while (!lines[i + loop].Contains(";"))
                        loop++;

                    lines[i] = lines[i] + ";";

                    bool isNoAdd = false;
                    int indexToFindElse = i + loop + 1;
                    if (indexToFindElse < lines.Count)
                    {
                        if (lines[indexToFindElse].Contains("else"))
                            isNoAdd = true;
                    }

                    if (!isNoAdd)
                        lines.Insert(i + 2 + loop, "[TXT_END_ELSE];");
                }
                else if (!nextLine.Contains("{") && !nextLine.Contains("}") && currentLine.Contains(";"))
                {
                    Log("[ELSE] nextLine not contains {} and currentLine contains ;", true);

                    if (IsContainsTemporaryVariables(nextLine, lines, i + 1))
                        goto L_MergeLines;
                }
                else if (nextLine.Contains("{") && !nextLine.Contains("}"))
                {
                    if (!currentLine.Contains(";"))
                        lines[i] = lines[i] + ";";
                    lines[i + 1] += " " + lines[i + 2];
                    lines.RemoveAt(i + 2);
                    goto L_MergeLines;
                }
                else if (nextLine.Contains("{") && nextLine.Contains("}"))
                {
                    Log("[ELSE] nextLine Contains {:\n" + nextLine, true);

                    if (!IsEven.IsCurlyEven(nextLine))
                    {
                        lines[i + 1] += " " + lines[i + 2];
                        lines.RemoveAt(i + 2);
                        goto L_MergeLines;
                    }
                    else
                    {
                        if (IsContainsTemporaryVariables(nextLine, lines, i))
                            goto L_MergeLines;

                        //Remove first '{' and end '}'
                        lines[i + 1] = lines[i + 1].Substring(1, lines[i + 1].Length - 2);

                        lines[i + 1] = MergeWhiteSpace.RemoveWhiteSpace(lines[i + 1]);

                        string cutRemains = null;
                        //Find if in if and extract it out from this array to solve conflict
                        if (lines[i + 1].Contains("if("))
                        {
                            int ifStartAt = lines[i + 1].IndexOf("if(");

                            string cut = lines[i + 1].Substring(ifStartAt);
                            Log("cut:\n" + cut, true);

                            lines[i + 1] = lines[i + 1].Substring(0, ifStartAt);
                            Log("Lines #" + (i + 1) + ":\n" + lines[i + 1], true, "yellow");

                            //Contains {
                            if (cut.Contains("{"))
                            {
                                //Count { and save first { index
                                int countCutLeftCurly = 0;
                                int cutleftCurlyIndex = cut.IndexOf("{");
                                for (int j = 0; j < cut.Length; j++)
                                {
                                    if (cut[j] == '{')
                                        countCutLeftCurly++;
                                    else if (cut[j] == '}')
                                        break;
                                }

                                //Find } by { count and save last } index
                                int countCutRightCurly = 0;
                                int cutRightCurlyIndex = 0;
                                for (int j = 0; j < cut.Length; j++)
                                {
                                    if (cut[j] == '}')
                                        countCutRightCurly++;
                                    if (countCutRightCurly == countCutLeftCurly)
                                    {
                                        cutRightCurlyIndex = j;
                                        break;
                                    }
                                }

                                string cutConditions = cut.Substring(0, cutleftCurlyIndex);

                                string cutScripts = cut.Substring(cutleftCurlyIndex);

                                //Find } by { count
                                int countRoomRightCurly = 0;
                                int roomRightCurlyIndex = 0;
                                for (int j = 0; j < cutScripts.Length; j++)
                                {
                                    if (cutScripts[j] == '}')
                                        countRoomRightCurly++;
                                    if (countRoomRightCurly == countCutLeftCurly)
                                    {
                                        roomRightCurlyIndex = j;
                                        break;
                                    }
                                }

                                cutScripts = cutScripts.Substring(0, roomRightCurlyIndex + 1);

                                cutRemains = cut.Substring(cutRightCurlyIndex + 1);

                                Log("cutConditions:\n" + cutConditions, true);
                                Log("cutScripts:\n" + cutScripts, true);
                                Log("cutRemains:\n" + cutRemains, true);

                                lines.Insert(i + 2, "[TXT_END_ELSE];");

                                //Split it out and insert it
                                if (cutRemains.Contains("if("))
                                {
                                    var cutRemainLines = StringSplit.GetStringSplitMoreThanOneChar(cutRemains, new string[] { "if(" });
                                L_RemergeRemainLines1:
                                    for (int j = 0; j < cutRemainLines.Count; j++)
                                    {
                                        if (!string.IsNullOrEmpty(cutRemainLines[j]) && !string.IsNullOrWhiteSpace(cutRemainLines[j]))
                                            cutRemainLines[j] = "if(" + cutRemainLines[j];
                                        else
                                        {
                                            cutRemainLines.RemoveAt(j);
                                            goto L_RemergeRemainLines1;
                                        }
                                    }

                                L_RemergeRemainLines2:
                                    for (int j = 0; j < cutRemainLines.Count; j++)
                                    {
                                        //Log(cutRemainLines[j],false,"yellow");
                                        if (!string.IsNullOrEmpty(cutRemainLines[j]) && !string.IsNullOrWhiteSpace(cutRemainLines[j]))
                                        {
                                            //Example: if(readparam(bInt)>=90){bonusbMatk,(.@r>=9)?50:30;}

                                            //Split if and {}
                                            if (cutRemainLines[j].Contains("if(") && cutRemainLines[j].Contains("{"))
                                            {
                                                //Count {}
                                                if (!IsEven.IsCurlyEven(cutRemainLines[j]))
                                                {
                                                    cutRemainLines[j] += cutRemainLines[j + 1];
                                                    cutRemainLines.RemoveAt(j + 1);
                                                    goto L_RemergeRemainLines2;
                                                }
                                                else
                                                {
                                                    //Split now
                                                    //Log("Before:\n" + cutRemainLines[j]);
                                                    int firstCurlyIndex = cutRemainLines[j].IndexOf('{');
                                                    string saveTxt = cutRemainLines[j];
                                                    cutRemainLines[j] = cutRemainLines[j].Substring(0, firstCurlyIndex);

                                                    //Find and split if inside if?
                                                    string toInsert = saveTxt.Substring(firstCurlyIndex);
                                                    if (toInsert.Contains("if("))
                                                    {
                                                        int startIfIndex = 0;
                                                        for (int k = 0; k < toInsert.Length; k++)
                                                        {
                                                            if (toInsert[k] == 'i' && toInsert[k + 1] == 'f' && toInsert[k + 2] == '(')
                                                            {
                                                                startIfIndex = k;
                                                                break;
                                                            }
                                                        }
                                                        string foundIf = toInsert.Substring(startIfIndex);
                                                        foundIf = foundIf.Substring(0, foundIf.IndexOf('}') + 1);
                                                        cutRemainLines.Insert(j + 1, "[TXT_END_IF];");
                                                        cutRemainLines.Insert(j + 1, foundIf);
                                                        toInsert = toInsert.Replace(foundIf, "");
                                                        cutRemainLines.Insert(j + 1, toInsert + "[TXT_NO_END_IF_BELOW]");
                                                    }
                                                    else
                                                        cutRemainLines.Insert(j + 1, toInsert);

                                                    //Log("After:\n" + cutRemainLines[j]);
                                                    //Log("Insert:\n" + cutRemainLines[j + 1]);

                                                    goto L_RemergeRemainLines2;
                                                }
                                            }
                                            //Example: if(readparam(bInt)>=90)bonusbMatk,(.@r>=9)?50:30;

                                            //Split if and last ) +1
                                            else if (cutRemainLines[j].Contains("if(") && cutRemainLines[j][cutRemainLines[j].Length - 1] != ')')
                                            {
                                                //Split now
                                                Log("(Before):\n" + cutRemainLines[j]);
                                                int firstCircleIndex = cutRemainLines[j].IndexOf('(');
                                                string saveTxt = cutRemainLines[j];
                                                cutRemainLines[j] = cutRemainLines[j].Substring(0, firstCircleIndex);
                                                cutRemainLines.Insert(j + 1, saveTxt.Substring(firstCircleIndex));
                                                Log("(After):\n" + cutRemainLines[j]);
                                                Log("(Insert):\n" + cutRemainLines[j + 1]);
                                                goto L_RemergeRemainLines2;
                                            }
                                            //Split if and last ) +1
                                            else if (cutRemainLines[j].Contains("if(") && !cutRemainLines[j].Contains(";"))
                                                cutRemainLines[j] = cutRemainLines[j] + ";";
                                        }
                                        else
                                        {
                                            cutRemainLines.RemoveAt(j);
                                            goto L_RemergeRemainLines2;
                                        }
                                    }

                                    cutRemainLines.Reverse();

                                    for (int j = 0; j < cutRemainLines.Count; j++)
                                        lines.Insert(i + 2, cutRemainLines[j]);
                                }
                                else
                                    lines.Insert(i + 2, cutRemains);
                                lines.Insert(i + 2, cutScripts);
                                lines.Insert(i + 2, cutConditions + ";");
                            }
                            //Otherwise use old code
                            else
                            {
                                lines.Insert(i + 2, "[TXT_END_ELSE];");
                                lines.Insert(i + 2, cut);
                            }
                        }
                        else
                            lines.Insert(i + 2, "[TXT_END_ELSE];");

                        var splitBonus = new List<string>();

                        int index = 1;

                        //Do not spilt if first word is [END_MERGE] and last word is [/END_MERGE]  
                        bool isFoundEndMergeStart = false;
                        bool isFoundEndMergeEnd = false;
                        if (lines[i + index].Contains("[END_MERGE]"))
                        {
                            string findWord = null;
                            foreach (var c in lines[i + index])
                            {
                                findWord += c;
                                if (findWord == "[END_MERGE]")
                                {
                                    isFoundEndMergeStart = true;
                                    break;
                                }
                            }
                            findWord = null;
                            for (int j = lines[i + index].Length - 1; j > 0; j--)
                            {
                                findWord = lines[i + index][j] + findWord;
                                if (findWord == "[/END_MERGE]")
                                {
                                    isFoundEndMergeEnd = true;
                                    break;
                                }
                            }
                        }

                        if (!isFoundEndMergeStart && !isFoundEndMergeEnd)
                        {
                            while (lines[i + index].Contains("autobonus") || lines[i + index].Contains("bonus_script"))
                            {
                                string cutAutoBonus = null;

                                lines[i + index] = lines[i + index].Replace("[/END_MERGE]", "[/END_MERGE] ");

                                int startAutoBonusIndex = lines[i + index].IndexOf("[END_MERGE]");
                                int endAutoBonusIndex = lines[i + index].IndexOf("[/END_MERGE]");

                                cutAutoBonus = lines[i + index].Substring(startAutoBonusIndex + 11);
                                Log("cutAutoBonus:\n" + cutAutoBonus, true);

                                cutAutoBonus = cutAutoBonus.Substring(0, cutAutoBonus.IndexOf("[/END_MERGE]"));
                                Log("cutAutoBonus:\n" + cutAutoBonus, true);

                                lines.Insert(i + index, cutAutoBonus);//TODO >> Change index to index -1?

                                Log("Lines #" + (i + index) + ":\n" + lines[i + index], true);

                                lines[i + index] = lines[i + index].Substring(0, startAutoBonusIndex) + lines[i + index].Substring(endAutoBonusIndex + 12);

                                Log("Lines #" + (i + index) + ":\n" + lines[i + index], true);

                                index++;
                            }
                        }

                        if (isFoundEndMergeStart && isFoundEndMergeEnd)
                            splitBonus.Add(lines[i + index]);
                        else
                        {
                            splitBonus = StringSplit.GetStringSplit(lines[i + index], ';');

                        //Then re-add ';'
                        L_MergeSplitBonus:
                            for (int j = 0; j < splitBonus.Count; j++)
                            {
                                if (splitBonus[j] == "" || splitBonus[j] == " " || string.IsNullOrEmpty(splitBonus[j]) || string.IsNullOrWhiteSpace(splitBonus[j]))
                                {
                                    splitBonus.RemoveAt(j);
                                    goto L_MergeSplitBonus;
                                }
                                else
                                {
                                    if (splitBonus[j].Contains("[END_MERGE]") && !splitBonus[j].Contains("[/END_MERGE]"))
                                    {
                                        if (j + 1 < splitBonus.Count)
                                        {
                                            splitBonus[j] = splitBonus[j] + ";" + splitBonus[j + 1];
                                            splitBonus.RemoveAt(j + 1);
                                            goto L_MergeSplitBonus;
                                        }
                                    }
                                    if (splitBonus[j][splitBonus[j].Length - 1] != ';')
                                        splitBonus[j] = splitBonus[j] + ";";
                                }
                            }
                        }

                        foreach (var bonus in splitBonus)
                            Log("splitBonus: " + bonus, false, "yellow");

                        //Set current index to [TXT_ELSE];
                        lines[i] = "[TXT_ELSE];";

                        //Set next index to splitBonus[0]
                        if (splitBonus.Count > 0)
                            lines[i + index] = splitBonus[0];

                        if (splitBonus.Count > 1)
                        {
                            for (int j = 1; j < splitBonus.Count; j++)
                                lines.Insert(i + index + j, splitBonus[j]);
                        }

                        //foreach (var item in lines)
                        //   Log(item, false, "orange");
                    }
                }
            }
            else if (currentLine == "bonus" || currentLine == "bonus " || currentLine == " bonus")
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine == "bonus2" || currentLine == "bonus2 " || currentLine == " bonus2")
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine == "bonus3" || currentLine == "bonus3 " || currentLine == " bonus3")
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine == "bonus4" || currentLine == "bonus4 " || currentLine == " bonus4")
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine == "bonus5" || currentLine == "bonus5 " || currentLine == " bonus5")
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (IsContainsTemporaryVariables(currentLine, lines, i, true))
                goto L_MergeLines;
            else if (currentLine.Contains("itemheal") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("percentheal") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("heal") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("sc_end") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("sc_start") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("itemskill") && !currentLine.Contains(";"))
            {
                lines[i] = lines[i].Replace("itemskill", "itemskillz");
                lines[i] += " " + lines[i + 1];
                lines.RemoveAt(i + 1);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("skill") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("getrandgroupitem") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("monster") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("produce") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("pet") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("catchpet") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bpet") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("birthpet") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("guildgetexp") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("Zeny") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("RouletteGold") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("RouletteBronze") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("RouletteSilver") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("autobonus3") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("autobonus2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("autobonus") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus_script") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bStr") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bAgi,") || currentLine.Contains("bonus bAgi ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bVit") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bInt,") || currentLine.Contains("bonus bInt ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDex") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bLuk") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAllStats") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAgiVit") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAgiDexStr") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bMaxHP,") || currentLine.Contains("bonus bMaxHP ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMaxHPrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bMaxSP,") || currentLine.Contains("bonus bMaxSP ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMaxSPrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bBaseAtk") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bAtk,") || currentLine.Contains("bonus bAtk ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAtk2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAtkRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bWeaponAtkRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bMatk,") || currentLine.Contains("bonus bMatk ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMatkRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bWeaponMatkRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bDef,") || currentLine.Contains("bonus bDef ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDefRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bDef2,") || currentLine.Contains("bonus bDef2 ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDef2Rate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bMdef,") || currentLine.Contains("bonus bMdef ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMdefRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bMdef2,") || currentLine.Contains("bonus bMdef2 ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMdef2Rate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bHit,") || currentLine.Contains("bonus bHit ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bHitRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bCritical,") || currentLine.Contains("bonus bCritical ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bCriticalLong") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bCriticalAddRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bCriticalRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bFlee,") || currentLine.Contains("bonus bFlee ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bFleeRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bFlee2,") || currentLine.Contains("bonus bFlee2 ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bFlee2Rate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bPerfectHitRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bPerfectHitAddRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bSpeedRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bSpeedAddRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bAspd,") || currentLine.Contains("bonus bAspd ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAspdRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAtkRange") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAddMaxWeight") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bHPrecovRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bSPrecovRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bHPRegenRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bHPLossRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSPRegenRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSPLossRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bRegenPercentHP") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bRegenPercentSP") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoRegen") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUseSPrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bSkillUseSP,") || currentLine.Contains("bonus2 bSkillUseSP ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSkillUseSPrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSkillAtk") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bLongAtkRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bCritAtkRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bCriticalDef") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bWeaponAtk,") || currentLine.Contains("bonus2 bWeaponAtk ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bWeaponDamageRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNearAtkDef") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bLongAtkDef") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMagicAtkDef") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMiscAtkDef") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoWeaponDamage") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoMagicDamage") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoMiscDamage") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bHealPower,") || currentLine.Contains("bonus bHealPower ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bHealPower2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bSkillHeal,") || currentLine.Contains("bonus2 bSkillHeal ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSkillHeal2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAddItemHealRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddItemHealRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddItemGroupHealRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bCastrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bCastrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bFixedCastrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bFixedCastrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bVariableCastrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bVariableCastrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bFixedCast,") || currentLine.Contains("bonus bFixedCast ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSkillFixedCast") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bVariableCast,") || currentLine.Contains("bonus bVariableCast ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSkillVariableCast") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bNoCastCancel;") || currentLine.Contains("bonus bNoCastCancel ;")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoCastCancel2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDelayrate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSkillDelay") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSkillCooldown") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bAddEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bMagicAddEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSubEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bSubEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSubDefEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bAddRace,") || currentLine.Contains("bonus2 bAddRace ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bMagicAddRace,") || currentLine.Contains("bonus2 bMagicAddRace ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bSubRace,") || currentLine.Contains("bonus2 bSubRace ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bAddClass,") || currentLine.Contains("bonus2 bAddClass ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bMagicAddClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSubClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddSize") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bMagicAddSize") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSubSize") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoSizeFix") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddDamageClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddMagicDamageClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddDefMonster") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddMDefMonster") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddRace2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSubRace2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bMagicAddRace2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSubSkill") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAbsorbDmgMaxHP") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAtkEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDefEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bMagicAtkEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDefRatioAtkRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDefRatioAtkEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDefRatioAtkClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus4 bSetDefRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus4 bSetMDefRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bIgnoreDefEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bIgnoreDefRace,") || currentLine.Contains("bonus bIgnoreDefRace ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bIgnoreDefClass,") || currentLine.Contains("bonus bIgnoreDefClass ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bIgnoreMDefRace,") || currentLine.Contains("bonus bIgnoreMDefRace ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bIgnoreDefRaceRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bIgnoreMdefRaceRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bIgnoreMdefRace2Rate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bIgnoreMDefEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bIgnoreDefClassRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bIgnoreMdefClassRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bExpAddRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bExpAddClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bAddEff,") || currentLine.Contains("bonus2 bAddEff ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddEff2") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddEffWhenHit") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bResEff") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus3 bAddEff,") || currentLine.Contains("bonus3 bAddEff ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus4 bAddEff,") || currentLine.Contains("bonus4 bAddEff ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bAddEffWhenHit") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus4 bAddEffWhenHit") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bAddEffOnSkill") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus4 bAddEffOnSkill") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus5 bAddEffOnSkill") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bComaClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bComaRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bWeaponComaEle") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bWeaponComaClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bWeaponComaRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus3 bAutoSpell,") || currentLine.Contains("bonus3 bAutoSpell ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bAutoSpellWhenHit") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus4 bAutoSpell,") || currentLine.Contains("bonus4 bAutoSpell ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus5 bAutoSpell,") || currentLine.Contains("bonus5 bAutoSpell ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus4 bAutoSpellWhenHit") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus5 bAutoSpellWhenHit") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus4 bAutoSpellOnSkill") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus5 bAutoSpellOnSkill") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bHPDrainValue") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bHPDrainValueRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bHpDrainValueClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bSPDrainValue") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSPDrainValueRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSpDrainValueClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bHPDrainRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSPDrainRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bHPVanishRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bHPVanishRaceRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bHPVanishRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSPVanishRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bSPVanishRaceRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bSPVanishRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bStateNoRecoverRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bHPGainValue") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bSPGainValue") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bSPGainRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bLongHPGainValue") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bLongSPGainValue") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMagicHPGainValue") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMagicSPGainValue") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bShortWeaponDamageReturn") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bLongWeaponDamageReturn") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bMagicDamageReturn") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnstripableWeapon") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnstripableArmor") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnstripableHelm") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnstripableShield") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bUnstripable;") || currentLine.Contains("bonus bUnstripable ;")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnbreakableGarment") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnbreakableWeapon") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnbreakableArmor") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnbreakableHelm") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnbreakableShield") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bUnbreakableShoes") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus bUnbreakable,") || currentLine.Contains("bonus bUnbreakable ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bBreakWeaponRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bBreakArmorRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bDropAddRace") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bDropAddClass") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bAddMonsterIdDropItem") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus2 bAddMonsterDropItem,") || currentLine.Contains("bonus2 bAddMonsterDropItem ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus3 bAddMonsterDropItem,") || currentLine.Contains("bonus3 bAddMonsterDropItem ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if ((currentLine.Contains("bonus3 bAddClassDropItem,") || currentLine.Contains("bonus3 bAddClassDropItem ,")) && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i, true);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddMonsterDropItemGroup") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bAddMonsterDropItemGroup") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus3 bAddClassDropItemGroup") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bGetZenyNum") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddGetZenyNum") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDoubleRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bDoubleAddRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bSplashRange") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bSplashAddRange") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus2 bAddSkillBlow") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoKnockback") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoGemStone") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bIntravision") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bPerfectHide") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bRestartFullRecover") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bClassChange") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bAddStealRate") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoMadoFuel") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("bonus bNoWalkDelay") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
            else if (currentLine.Contains("getitem") && !currentLine.Contains(";"))
            {
                MergeItemScripts(lines, i);
                goto L_MergeLines;
            }
        }
        #endregion

        #region Replace temporary variables
        for (int i = 0; i < lines.Count; i++)
        {
            //Log("Lines #" + i + ":\n" + lines[i], true, "#F3FFAE");

            #region Scripts
            string findTempVar = lines[i];
            if (findTempVar.Contains(".@"))
            {
                // +=, -=, *=, /=
                if (findTempVar.Contains("+=") || findTempVar.Contains("-=") || findTempVar.Contains("*=") || findTempVar.Contains("/="))
                {
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (findTempVar.Contains(tempVariables[j].variableName))
                        {
                            findTempVar = findTempVar.Replace(tempVariables[j].variableName, tempVariables[j].aka);
                            isUseAkaTempVar = true;
                        }
                    }
                    lines[i] = "[TEMP_VAR]" + findTempVar;
                }
                // ++, --
                else if (findTempVar.Contains("++") || findTempVar.Contains("--"))
                {
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (findTempVar.Contains(tempVariables[j].variableName))
                        {
                            findTempVar = findTempVar.Replace(tempVariables[j].variableName, tempVariables[j].aka);
                            isUseAkaTempVar = true;
                        }
                    }
                    findTempVar = findTempVar.Replace("++", "เพิ่ม 1");
                    findTempVar = findTempVar.Replace("--", "ลด 1");
                    lines[i] = "[TEMP_VAR]" + findTempVar;
                }
                else
                {
                    Log("findTempVar: " + findTempVar);
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        Log(" tempVariables[" + j + "].txtDefault: " + tempVariables[j].txtDefault);
                        if (findTempVar == tempVariables[j].txtDefault && !findTempVar.Contains("[DONT_DECLARE]"))
                        {
                            lines[i] = "[TEMP_VAR_DECLARE]" + tempVariables[j].aka + "꞉ " + tempVariables[j].value;
                            break;
                        }
                        else if (findTempVar.Contains("[DONT_DECLARE]"))
                            lines[i] = lines[i].Replace("[DONT_DECLARE]", "");
                    }
                }
            }
            #endregion

            Log("Lines #" + i + ":\n" + lines[i], true, "#F3FFAE");
        }
        #endregion

        for (int i = 0; i < lines.Count; i++)
        {
            Log("Lines #" + i + ":\n" + lines[i]);

            char sumFirstChar = ' ';
            if (!string.IsNullOrEmpty(lines[i]))
                sumFirstChar = lines[i][0];

            if (lines[i].Contains("TXT_NO_END_IF_BELOW"))
            {
                lines[i] = lines[i].Replace("[TXT_NO_END_IF_BELOW]", "");
                lines[i] = lines[i].Replace("[TXT_NO_END_IF_BELOW", "");
                lines[i] = lines[i].Replace("TXT_NO_END_IF_BELOW}", "");
                lines[i] = lines[i].Replace("TXT_NO_END_IF_BELOW", "");
                if (i + 1 < lines.Count && lines[i + 1].Contains("[TXT_END_IF]"))
                    lines[i + 1] = "";
            }
            lines[i] = lines[i].Replace("[END_MERGE]", "");
            lines[i] = lines[i].Replace("[/END_MERGE]", "");
            lines[i] = lines[i].Replace("bonus5", "bonus5 ");
            lines[i] = lines[i].Replace("bonus4", "bonus4 ");
            lines[i] = lines[i].Replace("bonus3", "bonus3 ");
            lines[i] = lines[i].Replace("bonus2", "bonus2 ");
            lines[i] = lines[i].Replace("bonus", "bonus ");
            lines[i] = lines[i].Replace("bonus 5", "bonus5");
            lines[i] = lines[i].Replace("bonus 4", "bonus4");
            lines[i] = lines[i].Replace("bonus 3", "bonus3");
            lines[i] = lines[i].Replace("bonus 2", "bonus2");
            lines[i] = lines[i].Replace("heal", "heal ");
            lines[i] = lines[i].Replace("  ", " ");

            data = lines[i];

            string functionName = "";
            #region if
            if (sumFirstChar != '[' && data.Contains("if(") || data.Contains("if (") || data.Contains("else if(") || data.Contains("else if ("))
            {
                data = data.Replace("[TEMP_VAR]", "");
                data = data.Replace("[TEMP_VAR_DECLARE]", "");

                string saveData = data;

                List<string> toReplace = new List<string>();
                List<string> toReplaceValue = new List<string>();

                //bonus_script inside room
                while (data.Contains("bonus_script"))
                {
                    //Loop all char
                    for (int j = 0; j < data.Length; j++)
                    {
                        int declareAt1 = data.IndexOf("bonus_script");
                        if (declareAt1 == -1)
                            break;
                        string sub = data.Substring(declareAt1);
                        int declareAt2 = sub.IndexOf(";");
                        if (declareAt2 == -1)
                            break;
                        string toConvert = sub.Substring(0, declareAt2 + 1);
                        Log("bonus_script >> toConvert: " + toConvert);
                        string convert = GetScriptsDescription(toConvert, true);
                        convert = convert.Replace("\"", " ");
                        if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                            convert = convert.Substring(0, convert.Length - 1);
                        Log("bonus_script >> convert: " + toConvert);
                        toReplace.Add(toConvert);
                        toReplaceValue.Add(convert + "\",\n\"");
                        data = data.Replace(toConvert, convert);
                        Log("bonus_script >> data: " + data);
                    }
                }

                //autobonus inside room
                while (data.Contains("autobonus"))
                {
                    //Loop all char
                    for (int j = 0; j < data.Length; j++)
                    {
                        int declareAt1 = data.IndexOf("autobonus");
                        if (declareAt1 == -1)
                            break;
                        string sub = data.Substring(declareAt1);
                        int declareAt2 = sub.IndexOf(";");
                        if (declareAt2 == -1)
                            break;
                        string toConvert = sub.Substring(0, declareAt2 + 1);
                        Log("autobonus >> toConvert: " + toConvert);
                        string convert = GetScriptsDescription(toConvert, true);
                        convert = convert.Replace("\"", " ");
                        if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                            convert = convert.Substring(0, convert.Length - 1);
                        Log("autobonus >> convert: " + toConvert);
                        toReplace.Add(toConvert);
                        toReplaceValue.Add(convert + "\",\n\"");
                        data = data.Replace(toConvert, convert);
                        Log("autobonus >> data: " + data);
                    }
                }

                //itemheal inside room
                while (data.Contains("itemheal"))
                {
                    //Loop all char
                    for (int j = 0; j < data.Length; j++)
                    {
                        int declareAt1 = data.IndexOf("itemheal");
                        if (declareAt1 == -1)
                            break;
                        string sub = data.Substring(declareAt1);
                        int declareAt2 = sub.IndexOf(";");
                        if (declareAt2 == -1)
                            break;
                        string toConvert = sub.Substring(0, declareAt2 + 1);
                        Log("autobonus >> itemheal#1: " + toConvert);
                        string convert = GetScriptsDescription(toConvert, true);
                        convert = convert.Replace("\"", " ");
                        if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                            convert = convert.Substring(0, convert.Length - 1);
                        Log("autobonus >> itemheal#2: " + toConvert);
                        toReplace.Add(toConvert);
                        toReplaceValue.Add(convert + "\",\n\"");
                        data = data.Replace(toConvert, convert);
                        Log("autobonus >> itemheal#3: " + data);
                    }
                }

                //percentheal inside room
                while (data.Contains("percentheal"))
                {
                    //Loop all char
                    for (int j = 0; j < data.Length; j++)
                    {
                        int declareAt1 = data.IndexOf("percentheal");
                        if (declareAt1 == -1)
                            break;
                        string sub = data.Substring(declareAt1);
                        int declareAt2 = sub.IndexOf(";");
                        if (declareAt2 == -1)
                            break;
                        string toConvert = sub.Substring(0, declareAt2 + 1);
                        Log("autobonus >> percentheal#1: " + toConvert);
                        string convert = GetScriptsDescription(toConvert, true);
                        convert = convert.Replace("\"", " ");
                        if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                            convert = convert.Substring(0, convert.Length - 1);
                        Log("autobonus >> percentheal#2: " + toConvert);
                        toReplace.Add(toConvert);
                        toReplaceValue.Add(convert + "\",\n\"");
                        data = data.Replace(toConvert, convert);
                        Log("autobonus >> percentheal#3: " + data);
                    }
                }

                //heal inside room
                while (data.Contains("heal"))
                {
                    //Loop all char
                    for (int j = 0; j < data.Length; j++)
                    {
                        int declareAt1 = data.IndexOf("heal");
                        if (declareAt1 == -1)
                            break;
                        string sub = data.Substring(declareAt1);
                        int declareAt2 = sub.IndexOf(";");
                        if (declareAt2 == -1)
                            break;
                        string toConvert = sub.Substring(0, declareAt2 + 1);
                        Log("autobonus >> heal#1: " + toConvert);
                        string convert = GetScriptsDescription(toConvert, true);
                        convert = convert.Replace("\"", " ");
                        if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                            convert = convert.Substring(0, convert.Length - 1);
                        Log("autobonus >> heal#2: " + toConvert);
                        toReplace.Add(toConvert);
                        toReplaceValue.Add(convert + "\",\n\"");
                        data = data.Replace(toConvert, convert);
                        Log("autobonus >> heal#3: " + data);
                    }
                }

                //sc_start inside room
                while (data.Contains("sc_start"))
                {
                    //Loop all char
                    for (int j = 0; j < data.Length; j++)
                    {
                        int declareAt1 = data.IndexOf("sc_start");
                        if (declareAt1 == -1)
                            break;
                        string sub = data.Substring(declareAt1);
                        int declareAt2 = sub.IndexOf(";");
                        if (declareAt2 == -1)
                            break;
                        string toConvert = sub.Substring(0, declareAt2 + 1);
                        Log("autobonus >> sc_start#1: " + toConvert);
                        string convert = GetScriptsDescription(toConvert, true);
                        convert = convert.Replace("\"", " ");
                        if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                            convert = convert.Substring(0, convert.Length - 1);
                        Log("autobonus >> sc_start#2: " + toConvert);
                        toReplace.Add(toConvert);
                        toReplaceValue.Add(convert + "\",\n\"");
                        data = data.Replace(toConvert, convert);
                        Log("autobonus >> sc_start#3: " + data);
                    }
                }

                //sc_end inside room
                while (data.Contains("sc_end"))
                {
                    //Loop all char
                    for (int j = 0; j < data.Length; j++)
                    {
                        int declareAt1 = data.IndexOf("sc_end");
                        if (declareAt1 == -1)
                            break;
                        string sub = data.Substring(declareAt1);
                        int declareAt2 = sub.IndexOf(";");
                        if (declareAt2 == -1)
                            break;
                        string toConvert = sub.Substring(0, declareAt2 + 1);
                        Log("autobonus >> sc_end#1: " + toConvert);
                        string convert = GetScriptsDescription(toConvert, true);
                        convert = convert.Replace("\"", " ");
                        if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                            convert = convert.Substring(0, convert.Length - 1);
                        Log("autobonus >> sc_end#2: " + toConvert);
                        toReplace.Add(toConvert);
                        toReplaceValue.Add(convert + "\",\n\"");
                        data = data.Replace(toConvert, convert);
                        Log("autobonus >> sc_end#3: " + data);
                    }
                }

                //Bonus inside room
                while (data.Contains("bonus"))
                {
                    //Loop all char
                    for (int j = 0; j < data.Length; j++)
                    {
                        int declareAt1 = data.IndexOf("bonus");
                        if (declareAt1 == -1)
                            break;
                        string sub = data.Substring(declareAt1);
                        int declareAt2 = sub.IndexOf(";");
                        if (declareAt2 == -1)
                            break;
                        string toConvert = sub.Substring(0, declareAt2 + 1);
                        Log("autobonus >> bonus#1: " + toConvert);
                        string convert = GetScriptsDescription(toConvert, true);
                        convert = convert.Replace("\"", " ");
                        if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                            convert = convert.Substring(0, convert.Length - 1);
                        Log("autobonus >> bonus#2: " + toConvert);
                        toReplace.Add(toConvert);
                        toReplaceValue.Add(convert + "\",\n\"");
                        data = data.Replace(toConvert, convert);
                        Log("autobonus >> bonus#3: " + data);
                    }
                }

                if (toReplaceValue.Count > 0)
                    toReplaceValue[toReplaceValue.Count - 1] = toReplaceValue[toReplaceValue.Count - 1].Replace("\",\n\"", "");

                data = saveData;

                //Remove spacebar
                data = MergeWhiteSpace.RemoveWhiteSpace(data);

                for (int j = 0; j < toReplace.Count; j++)
                    data = data.Replace(MergeWhiteSpace.RemoveWhiteSpace(toReplace[j]), toReplaceValue[j]);

                data = data.Replace("elseif(", " หรือถ้า ");
                data = data.Replace("if(", " ถ้า ");
                data = ReplaceAllSpecialValue(data);
                data = data.Replace("(", "");
                data = data.Replace(")", "");
                data = data.Replace(";", "");

                data = data.Replace("W_FIST", " ไม่มีอาวุธ ");
                data = data.Replace("W_DAGGER", " Dagger ");
                data = data.Replace("W_1HSWORD", " One-handed swords ");
                data = data.Replace("W_2HSWORD", " Two-handed swords ");
                data = data.Replace("W_1HSPEAR", " One-handed spears ");
                data = data.Replace("W_2HSPEAR", " Two-handed spears ");
                data = data.Replace("W_1HAXE", " One-handed axes ");
                data = data.Replace("W_2HAXE", " Two-handed axes ");
                data = data.Replace("W_MACE", " Maces ");
                data = data.Replace("W_2HMACE", " Two-handed Maces ");
                data = data.Replace("W_STAFF", " Staff ");
                data = data.Replace("W_BOW", " Bows ");
                data = data.Replace("W_KNUCKLE", " Knuckles ");
                data = data.Replace("W_MUSICAL", " Musical Instruments ");
                data = data.Replace("W_WHIP", " Whips ");
                data = data.Replace("W_BOOK", " Book ");
                data = data.Replace("W_KATAR", " Katars ");
                data = data.Replace("W_REVOLVER", " Revolvers ");
                data = data.Replace("W_RIFLE", " Rifles ");
                data = data.Replace("W_GATLING", " Gatling guns ");
                data = data.Replace("W_SHOTGUN", " Shotguns ");
                data = data.Replace("W_GRENADE", " Grenade launchers ");
                data = data.Replace("W_HUUMA", " Fuuma Shurikens ");
                data = data.Replace("W_2HSTAFF", " Two-handed Staff ");
                data = data.Replace("MAX_WEAPON_TYPE", "");
                data = data.Replace("W_DOUBLE_DD", " Dual-wield Daggers ");
                data = data.Replace("W_DOUBLE_SS", " Dual-wield Swords ");
                data = data.Replace("W_DOUBLE_AA", " Dual-wield Axes ");
                data = data.Replace("W_DOUBLE_DS", " Dagger + Sword ");
                data = data.Replace("W_DOUBLE_DA", " Dagger + Axe ");
                data = data.Replace("W_DOUBLE_SA", " Sword + Axe ");
                data = data.Replace("MAX_WEAPON_TYPE_ALL", "");

                data = data.Replace("==", " คือ ");
                data = data.Replace("!=", " ไม่เท่ากับ ");
                data = data.Replace("||", " หรือ ");
                data = data.Replace("&&", " และ ");
                data = data.Replace(">=", " มากกว่าหรือเท่ากับ ");
                data = data.Replace(">", " มากกว่า ");
                data = data.Replace("<=", " น้อยกว่าหรือเท่ากับ ");
                data = data.Replace("<", " น้อยกว่า ");
                data = data.Replace("Job_", "");
                data = data.Replace("_", " ");
                data = data.Replace("getpartnerid()", "มีคู่สมรส");
                data = data.Replace("{", " รับ ");
                data = data.Replace("}", " ");
                data = data.Replace("ค่าที่", " ค่าที่ ");
                data = data.Replace(" , ", ", ");

                //Use store temporary variables if found in this value
                bool isFoundTempVariable = false;
                List<string> tempVarName = new List<string>();
                List<string> valueFromTempVar = new List<string>();
                for (int j = 0; j < tempVariables.Count; j++)
                {
                    if (data.Contains(tempVariables[j].variableName))
                    {
                        isFoundTempVariable = true;

                        tempVarName.Add(tempVariables[j].variableName);
                        Log("GetDescription >> Found variableName: " + tempVariables[j].variableName);

                        string value = tempVariables[j].value;
                        bool isContainRand = value.Contains("rand");
                        if (isContainRand)
                        {
                            value = GetValue(value);
                            valueFromTempVar.Add("สุ่ม " + value);
                        }
                        else
                            valueFromTempVar.Add(value);
                        Log("GetDescription >> Found value: " + tempVariables[j].value);
                    }
                }

                //Replace temporary variables
                if (isFoundTempVariable)
                {
                    for (int j = 0; j < tempVarName.Count; j++)
                        data = data.Replace(tempVarName[j], valueFromTempVar[j]);
                }

                //Replace special variables
                data = ReplaceAllSpecialValue(data);

                sum += AddDescription(sum, "[" + data + "]");
            }
            #endregion
            #region [TXT_ELSE]
            functionName = "[TXT_ELSE]";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "[หากไม่ตรงเงื่อนไข]");
            #endregion
            #region [TXT_END_ELSE]
            functionName = "[TXT_END_ELSE]";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "[สิ้นสุดหากไม่ตรงเงื่อนไข]");
            #endregion
            #region [TXT_END_IF]
            functionName = "[TXT_END_IF]";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "[สิ้นสุดหากตรงเงื่อนไข]");
            #endregion 
            #region [TXT_END]
            functionName = "[TXT_END]";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "[ว่างเปล่า]");
            #endregion

            #region autobonus3
            functionName = "autobonus3";
            if (data.Contains(functionName))
            {
                string sumCut = data;

                sumCut = sumCut.Replace("[END_MERGE]", "");
                sumCut = sumCut.Replace(functionName, "");
                sumCut = sumCut.Replace("[", "{");
                sumCut = sumCut.Replace("]", "}");

                List<string> allParam = GetAllParamerters(sumCut, true);

            L_ReMerge:
                for (int j = 0; j < allParam.Count; j++)
                {
                    //Count { and }
                    if (allParam[j].Contains("{"))
                    {
                        int count1 = 0;
                        int count2 = 0;
                        foreach (var c in allParam[j])
                        {
                            if (c == '{')
                                count1++;
                            else if (c == '}')
                                count2++;
                        }
                        if (count1 != count2 && (j + 1) < allParam.Count)
                        {
                            allParam[j] = allParam[j] + allParam[j + 1];
                            allParam.RemoveAt(j + 1);
                            goto L_ReMerge;
                        }
                    }
                    if (allParam[j][0] == ',')
                        allParam[j] = allParam[j].Substring(1, allParam[j].Length - 1);
                }

                if (allParam.Count > 3)
                {
                    allParam[2] = allParam[2].Replace("{", "");
                    allParam[2] = allParam[2].Replace("}", "");
                }
                if (allParam.Count > 4)
                {
                    allParam[3] = allParam[3].Replace(";", "");
                    allParam[3] = allParam[3].Replace("}", "");
                }

                //for (int j = 0; j < allParam.Count; j++)
                //    Log("allParam[" + j + "]: " + allParam[j]);

                string param1 = GetBonusScript(allParam[0]);
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (allParam.Count > 3)
                    param4 = allParam[3];

                if (!string.IsNullOrEmpty(param3) || !string.IsNullOrWhiteSpace(param3))
                {
                    param3 = param3.Replace("{", "");
                    param3 = param3.Replace("}", "");
                }
                if (!string.IsNullOrEmpty(param4) || !string.IsNullOrWhiteSpace(param4))
                {
                    param4 = param4.Replace(";", "");
                    param4 = param4.Replace("}", "");
                }

                sum += AddDescription(sum, "เมื่อร่าย Skill " + GetSkillName(param4) + " มีโอกาส " + GetRateByDivider(param2, 10) + "% ที่จะ " + param1 + " เป็นเวลา " + UseFunctionWithString(param3, 0));

                data = "";
            }
            #endregion
            #region autobonus2
            functionName = "autobonus2";
            if (data.Contains(functionName))
            {
                string sumCut = data;

                sumCut = sumCut.Replace("[END_MERGE]", "");
                sumCut = sumCut.Replace(functionName, "");
                sumCut = sumCut.Replace("[", "{");
                sumCut = sumCut.Replace("]", "}");

                List<string> allParam = GetAllParamerters(sumCut, true);

            L_ReMerge:
                for (int j = 0; j < allParam.Count; j++)
                {
                    if ((j + 1) < allParam.Count && allParam[j].Contains("{") && !allParam[j].Contains("}"))
                    {
                        allParam[j] = allParam[j] + allParam[j + 1];
                        allParam.RemoveAt(j + 1);
                        goto L_ReMerge;
                    }
                    else
                    {
                        if (allParam[j][0] == ',')
                            allParam[j] = allParam[j].Substring(1, allParam[j].Length - 1);
                    }
                }

                if (allParam.Count > 3)
                {
                    allParam[2] = allParam[2].Replace("{", "");
                    allParam[2] = allParam[2].Replace("}", "");
                }
                if (allParam.Count > 4)
                {
                    allParam[3] = allParam[3].Replace(";", "");
                    allParam[3] = allParam[3].Replace("}", "");
                }

                string param1 = GetBonusScript(allParam[0]);
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = null;

                if (allParam.Count > 3)
                    param4 = allParam[3];

                if (!string.IsNullOrEmpty(param3) || !string.IsNullOrWhiteSpace(param3))
                {
                    param3 = param3.Replace("{", "");
                    param3 = param3.Replace("}", "");
                }
                if (!string.IsNullOrEmpty(param4) || !string.IsNullOrWhiteSpace(param4))
                {
                    param4 = param4.Replace(";", "");
                    param4 = param4.Replace("}", "");
                }

                if (!string.IsNullOrEmpty(param4) || !string.IsNullOrWhiteSpace(param4))
                    sum += AddDescription(sum, "เมื่อโดนโจมตีกายภาพมีโอกาส " + GetRateByDivider(param2, 10) + "% ที่จะ " + param1 + " เป็นเวลา " + UseFunctionWithString(param3, 0) + GetTriggerCriteria(param4));
                else
                    sum += AddDescription(sum, "เมื่อโดนโจมตีกายภาพมีโอกาส " + GetRateByDivider(param2, 10) + "% ที่จะ " + param1 + " เป็นเวลา " + UseFunctionWithString(param3, 0));

                data = "";
            }
            #endregion
            #region autobonus
            functionName = "autobonus";
            if (data.Contains(functionName))
            {
                string sumCut = data;

                sumCut = sumCut.Replace("[END_MERGE]", "");
                sumCut = sumCut.Replace(functionName, "");
                sumCut = sumCut.Replace("[", "{");
                sumCut = sumCut.Replace("]", "}");

                List<string> allParam = GetAllParamerters(sumCut, true);

            L_ReMerge:
                for (int j = 0; j < allParam.Count; j++)
                {
                    if ((j + 1) < allParam.Count && allParam[j].Contains("{") && !allParam[j].Contains("}"))
                    {
                        allParam[j] = allParam[j] + allParam[j + 1];
                        allParam.RemoveAt(j + 1);
                        goto L_ReMerge;
                    }
                    else
                    {
                        if (allParam[j][0] == ',')
                            allParam[j] = allParam[j].Substring(1, allParam[j].Length - 1);
                    }
                }

                if (allParam.Count > 3)
                {
                    allParam[2] = allParam[2].Replace("{", "");
                    allParam[2] = allParam[2].Replace("}", "");
                }
                if (allParam.Count > 4)
                {
                    allParam[3] = allParam[3].Replace(";", "");
                    allParam[3] = allParam[3].Replace("}", "");
                }

                string param1 = GetBonusScript(allParam[0]);
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = null;

                if (allParam.Count > 3)
                    param4 = allParam[3];

                if (!string.IsNullOrEmpty(param3) || !string.IsNullOrWhiteSpace(param3))
                {
                    param3 = param3.Replace("{", "");
                    param3 = param3.Replace("}", "");
                }
                if (!string.IsNullOrEmpty(param4) || !string.IsNullOrWhiteSpace(param4))
                {
                    param4 = param4.Replace(";", "");
                    param4 = param4.Replace("}", "");
                }

                if (!string.IsNullOrEmpty(param4) || !string.IsNullOrWhiteSpace(param4))
                    sum += AddDescription(sum, "เมื่อโจมตีกายภาพมีโอกาส " + GetRateByDivider(param2, 10) + "% ที่จะ " + param1 + " เป็นเวลา " + UseFunctionWithString(param3, 0) + GetTriggerCriteria(param4));
                else
                    sum += AddDescription(sum, "เมื่อโจมตีกายภาพมีโอกาส " + GetRateByDivider(param2, 10) + "% ที่จะ " + param1 + " เป็นเวลา " + UseFunctionWithString(param3, 0));

                data = "";
            }
            #endregion
            #region bonus_script
            functionName = "bonus_script";
            if (data.Contains(functionName))
            {
                string sumCut = data;

                sumCut = sumCut.Replace("[END_MERGE]", "");
                sumCut = sumCut.Replace(functionName, "");
                sumCut = sumCut.Replace("[", "{");
                sumCut = sumCut.Replace("]", "}");
                Log("sumCut: " + sumCut);
                List<string> allParam = GetAllParamerters(sumCut, true);

            L_ReMerge:
                for (int j = 0; j < allParam.Count; j++)
                {
                    //Log("allParam[j]: " + allParam[j]);
                    if ((j + 1) < allParam.Count && allParam[j].Contains("{") && !allParam[j].Contains("}"))
                    {
                        allParam[j] = allParam[j] + allParam[j + 1];
                        allParam.RemoveAt(j + 1);
                        //Log("allParam[j]: " + allParam[j]);
                        goto L_ReMerge;
                    }
                    else
                    {
                        if (allParam[j][0] == ',')
                            allParam[j] = allParam[j].Substring(1, allParam[j].Length - 1);
                    }
                }

                string param1 = GetBonusScript(allParam[0]);
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (allParam.Count > 3)
                    param4 = allParam[3];

                Log("param1: " + param1);
                Log("param2: " + param2);
                Log("param3: " + param3);
                Log("param4: " + param4);

                if (isHadParam1 && isHadParam2)
                {
                    string sumFlag = null;
                    string sumType = null;
                    if (isHadParam3)
                        sumFlag = param3;
                    if (isHadParam4)
                    {
                        sumType = param4;
                        if (sumType == "1")
                            sumType = "(เป็น Buff)";
                        else if (sumType == "1")
                            sumType = "(เป็น Debuff)";
                    }

                    sum += AddDescription(sum, "เมื่อสวมใส่จะ " + param1 + " เป็นเวลา " + TimerToStringTimer(float.Parse(param2)) + GetBonusScriptFlag(sumFlag) + sumType);
                }

                data = "";
            }
            #endregion

            #region getitem
            functionName = "getitem ";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                //Special function
                bool isSpecialFunction = false;
                List<string> allItem = new List<string>();
                if (sumCut.Contains("callfunc"))
                {
                    isSpecialFunction = true;

                    sumCut = sumCut.Replace("callfunc", "");

                    //Split and save it for later (Don't forget to replace item id to 0)
                    if (sumCut.Contains("\"F_Rand\""))
                    {
                        sumCut = sumCut.Replace("\"F_Rand\"", "");

                        //Find first ( and last and extract it out
                        int firstCircleIndex = sumCut.IndexOf("(");
                        int firstCircleEndIndex = sumCut.IndexOf(")");
                        string cut = sumCut.Substring(firstCircleIndex + 1);
                        int cutCircleEndAt = cut.IndexOf(")");
                        cut = cut.Substring(0, cutCircleEndAt);
                        allItem = StringSplit.GetStringSplit(cut, ',');
                        sumCut = sumCut.Substring(0, firstCircleIndex) + "0" + sumCut.Substring(firstCircleEndIndex + 1);
                    }
                }

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1, true);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isSpecialFunction)
                    {
                        string allItemName = null;
                        for (int j = 0; j < allItem.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(allItem[j]) && !string.IsNullOrWhiteSpace(allItem[j]))
                                allItemName = allItemName + ", " + GetItemName(allItem[j]);
                        }
                        allItemName = allItemName.Substring(2);
                        sum += AddDescription(sum, "รับชิ้นใดชิ้นนึงจาก (" + allItemName + ") จำนวน " + param2 + " ชิ้น");
                    }
                    else
                        sum += AddDescription(sum, "รับ " + GetItemName(allParam[0]) + " จำนวน " + param2 + " ชิ้น");
                }

                data = "";
            }
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

                data = "";
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

                data = "";
            }
            #endregion
            #region heal
            functionName = "heal";
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

                data = "";
            }
            #endregion
            #region sc_end
            functionName = "sc_end";
            if (data.Contains(functionName) && isNotClearTempVar)
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                string param1 = sumCut;
                param1 = param1.Replace("sc_", "");
                param1 = param1.Replace("SC_", "");
                param1 = UpperFirst(param1);
                sum += AddDescription(sum, "รักษาสถานะ " + param1);
            }
            #endregion
            #region sc_end
            functionName = "sc_end ";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = allParam[0];
                param1 = param1.Replace("sc_", "");
                param1 = param1.Replace("SC_", "");
                param1 = UpperFirst(param1);
                sum += AddDescription(sum, "รักษาสถานะ " + param1);
            }
            #endregion
            #region sc_start
            functionName = "sc_start ";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = allParam[0];
                param1 = param1.Replace("sc_", "");
                param1 = param1.Replace("SC_", "");
                param1 = UpperFirst(param1);
                string param2 = "";
                string param3 = "";
                string param4 = "";
                string param5 = "";

                if (allParam.Count > 1)
                    param2 = GetValue(allParam[1], 2);
                if (allParam.Count > 2)
                    param3 = GetValue(allParam[2], 3);
                if (allParam.Count > 3)
                    param4 = GetValue(allParam[3], 4);
                if (allParam.Count > 4)
                    param5 = GetValue(allParam[4], 5, true);

                Log("isHadParam2: " + isHadParam2 + " | param2: " + param2);
                Log("isHadParam3: " + isHadParam3 + " | param3: " + param3);
                Log("isHadParam4: " + isHadParam4 + " | param4: " + param4);
                Log("isHadParam5: " + isHadParam5 + " | param5: " + param5);

                string timer = "";
                if (param2 == "ตลอดเวลา")
                    timer = " " + param2;
                else
                    timer = " เป็นเวลา " + TimerToStringTimer(float.Parse(param2));

                string percent = "0";
                if (isHadParam4)
                    percent = GetRateByDivider(param4, 100);

                int flag = 0;
                if (isHadParam5)
                    flag = int.Parse(param5);
                else
                    flag = 1;
                string sumFlag = Get_sc_start_Flag(flag);

                if (isHadParam2 && isHadParam3
                    || isHadParam2)
                    sum += AddDescription(sum, "มีโอกาส " + percent + "% ที่จะเกิดสถานะ " + param1 + timer + sumFlag);
                else
                    sum += AddDescription(sum, "มีโอกาส " + percent + "% ที่จะเกิดสถานะ " + param1 + sumFlag);
            }
            #endregion
            #region itemskillz
            functionName = "itemskillz";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

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
                    sum += AddDescription(sum, "สามารถใช้ Lv. " + param2 + " " + param1 + "(โดยมีเงื่อนไขการใช้ Skill ดังเดิม)");
                else
                    sum += AddDescription(sum, "สามารถใช้ Lv. " + param2 + " " + param1);
            }
            #endregion
            #region skill
            functionName = "skill ";
            if (data.Contains("skill "))
            {
                string sumCut = CutFunctionName(data, functionName, 1);

                List<string> allParam = GetAllParamerters(sumCut);

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

                    skillFlag = (SkillFlag)Enum.Parse(typeof(SkillFlag), param3, true);

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

                sum += AddDescription(sum, "สามารถใช้ Lv. " + param2 + " " + param1 + txtFlag);

            }
            #endregion
            #region getrandgroupitem
            functionName = "getrandgroupitem";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                Log("allParam.Count: " + allParam.Count);

                string param1 = GetValue(allParam[0], 1, true);

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

                if (isHadParam2)
                    sum += AddDescription(sum, "กดใช้เพื่อรับ Item ในกลุ่ม " + param1 + " " + param2 + " ชิ้น");
                else
                    sum += AddDescription(sum, "กดใช้เพื่อรับ Item ในกลุ่ม " + param1);
            }
            #endregion
            #region monster
            functionName = "monster";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

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
                        finalize = "กดใช้เพื่อเปิดหน้าต่าง Craft Blacksmith's Stones และ Metals";
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
                sum += AddDescription(sum, "กดใช้เพื่อฟักไข่ Pet");
            #endregion
            #region birthpet
            functionName = "birthpet";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "กดใช้เพื่อฟักไข่ Pet");
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
                string sumCut = CutFunctionName(data, functionName, 1);

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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ใช้ SP มากขึ้น " + param2 + " กับ Skill " + GetSkillName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ใช้ SP น้อยลง " + param2 + " กับ Skill " + GetSkillName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ใช้ SP มากขึ้น " + param2 + "% กับ Skill " + GetSkillName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ใช้ SP น้อยลง " + param2 + "% กับ Skill " + GetSkillName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "Skill " + GetSkillName(allParam[0]) + " เบาลง " + param2 + "%");
                    else
                        sum += AddDescription(sum, "Skill " + GetSkillName(allParam[0]) + " แรงขึ้น " + param2 + "%");
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
                        sum += AddDescription(sum, "Skill " + GetSkillName(allParam[0]) + " Heal เบาลง " + param2 + "%");
                    else
                        sum += AddDescription(sum, "Skill " + GetSkillName(allParam[0]) + " Heal แรงขึ้น " + param2 + "%");
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
                        sum += AddDescription(sum, "โดน Heal จาก Skill " + GetSkillName(allParam[0]) + " เบาลง " + param2 + "%");
                    else
                        sum += AddDescription(sum, "โดน Heal จาก Skill " + GetSkillName(allParam[0]) + " แรงขึ้น " + param2 + "%");
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param1.Contains(tempVariables[j].aka) && param1.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                        else if (allParam[0] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                    }
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param2.Contains(tempVariables[j].aka) && param2.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                        else if (allParam[1] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                    }
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(allParam[0]) + " เร็วขึ้น " + param2 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(allParam[0]) + " ช้าลง " + param2 + "%");
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param1.Contains(tempVariables[j].aka) && param1.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                        else if (allParam[0] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                    }
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param2.Contains(tempVariables[j].aka) && param2.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                        else if (allParam[1] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                    }
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย FIXCAST กับ Skill " + GetSkillName(allParam[0]) + " เร็วขึ้น " + param2 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย FIXCAST กับ Skill " + GetSkillName(allParam[0]) + " ช้าลง " + param2 + "%");
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param1.Contains(tempVariables[j].aka) && param1.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                        else if (allParam[0] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                    }
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param2.Contains(tempVariables[j].aka) && param2.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                        else if (allParam[1] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                    }
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(allParam[0]) + " เร็วขึ้น " + param2 + "%");
                    else
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(allParam[0]) + " ช้าลง " + param2 + "%");
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param1.Contains(tempVariables[j].aka) && param1.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                        else if (allParam[0] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                    }
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ร่าย FIXCAST เร็วขึ้น " + UseFunctionWithString(param1, 0));
                    else
                        sum += AddDescription(sum, "ร่าย FIXCAST ช้าลง" + UseFunctionWithString(param1, 0));
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param2.Contains(tempVariables[j].aka) && param2.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                        else if (allParam[1] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                    }
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย FIXCAST กับ Skill " + GetSkillName(allParam[0]) + " เร็วขึ้น " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "ร่าย FIXCAST กับ Skill " + GetSkillName(allParam[0]) + " ช้าลง " + TimerToStringTimer(float.Parse(param2)));
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param1.Contains(tempVariables[j].aka) && param1.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                        else if (allParam[0] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                    }
                    if (isParam1Negative)
                        sum += AddDescription(sum, "ร่าย VCAST เร็วขึ้น " + UseFunctionWithString(param1, 0));
                    else
                        sum += AddDescription(sum, "ร่าย VCAST ช้าลง" + UseFunctionWithString(param1, 0));
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param2.Contains(tempVariables[j].aka) && param2.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                        else if (allParam[1] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                    }
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(allParam[0]) + " เร็วขึ้น " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "ร่าย VCAST กับ Skill " + GetSkillName(allParam[0]) + " ช้าลง " + TimerToStringTimer(float.Parse(param2)));
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
            #region bonus bDelayRate
            functionName = "bonus bDelayRate";
            if (data.Contains(functionName))
            {
                Log(functionName);

                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);

                if (isHadParam1)
                {
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param1.Contains(tempVariables[j].aka) && param1.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                        else if (allParam[0] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam1Negative = true;
                            param1 = param1.Replace("-", "");
                        }
                    }
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param2.Contains(tempVariables[j].aka) && param2.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                        else if (allParam[1] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                    }
                    if (isParam2Negative)
                        sum += AddDescription(sum, "Delay หลังร่ายกับ Skill " + GetSkillName(allParam[0]) + " เร็วขึ้น " + TimerToStringTimer(float.Parse(param2)));
                    else
                        sum += AddDescription(sum, "Delay หลังร่ายกับ Skill " + GetSkillName(allParam[0]) + " ช้าลง " + TimerToStringTimer(float.Parse(param2)));
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
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        if (param2.Contains(tempVariables[j].aka) && param2.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                        else if (allParam[1] == tempVariables[j].variableName && tempVariables[j].value.Contains("-"))
                        {
                            isParam2Negative = true;
                            param2 = param2.Replace("-", "");
                        }
                    }
                    if (isParam2Negative)
                        sum += AddDescription(sum, "Cooldown Skill " + GetSkillName(allParam[0]) + " เร็วขึ้น " + UseFunctionWithString(param2, 0));
                    else
                        sum += AddDescription(sum, "Cooldown Skill " + GetSkillName(allParam[0]) + " ช้าลง " + UseFunctionWithString(param2, 0));
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
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับธาตุ " + GetElementName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(allParam[0]));
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
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับธาตุ " + GetElementName(allParam[0]) + GetTriggerCriteria(allParam[2]));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(allParam[0]) + GetTriggerCriteria(allParam[2]));
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
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับธาตุ " + GetElementName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(allParam[0]));
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
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับธาตุ " + GetElementName(allParam[0]));
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
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับธาตุ " + GetElementName(allParam[0]) + GetTriggerCriteria(allParam[2]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับธาตุ " + GetElementName(allParam[0]) + GetTriggerCriteria(allParam[2]));
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
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับศัตรูที่การป้องกันเป็นธาตุ " + GetElementName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับศัตรูที่การป้องกันเป็นธาตุ " + GetElementName(allParam[0]));
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
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
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
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
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
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
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
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับ Class " + GetClassName(allParam[0]));
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
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับ Class " + GetClassName(allParam[0]));
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
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับ Class " + GetClassName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับ Size " + GetSizeName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับ Size " + GetSizeName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับ Size " + GetSizeName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับ Size " + GetSizeName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับ Size " + GetSizeName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับ Size " + GetSizeName(allParam[0]));
                }
            }
            #endregion
            #region bonus bNoSizeFix
            functionName = "bonus bNoSizeFix";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ไม่สนใจ Size ของศัตรูในการคำนวณ Damage");
            #endregion
            #region bonus2 bAddDamageClass
            functionName = "bonus2 bAddDamageClass";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                Log(functionName + " >> allParam[0]: " + allParam[0]);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับ " + GetMIDName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับ " + GetMIDName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับ " + GetMIDName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับ " + GetMIDName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีกายภาพแรงขึ้น " + param2 + "% กับ " + GetMIDName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีกายภาพเบาลง " + param2 + "% กับ " + GetMIDName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีเวทย์แรงขึ้น " + param2 + "% กับ " + GetMIDName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเวทย์เบาลง " + param2 + "% กับ " + GetMIDName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีกายภาพเบาลง " + param2 + "% กับเผ่า " + GetMonsterRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีกายภาพแรงขึ้น " + param2 + "% กับเผ่า " + GetMonsterRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีแรงขึ้น " + param2 + "% กับเผ่า " + GetMonsterRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โดนโจมตีเบาลง " + param2 + "% กับเผ่า " + GetMonsterRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีเวทย์เบาลง " + param2 + "% กับเผ่า " + GetMonsterRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีเวทย์แรงขึ้น " + param2 + "% กับเผ่า " + GetMonsterRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โดนโจมตีจาก Skill " + GetSkillName(allParam[0]) + " แรงขึ้น " + param2 + "%");
                    else
                        sum += AddDescription(sum, "โดนโจมตีจาก Skill " + GetSkillName(allParam[0]) + " เบาลง " + param2 + "%");
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
                        sum += AddDescription(sum, "ถ้า Damage ที่ได้รับมากกว่า " + param1 + "% ของ MaxHP จะใช้การคำนวณใหม่ (Damage เดิม + " + param1 + "% MaxHP) (ใช้ค่ามากสุดเท่านั้น)");
                    else
                        sum += AddDescription(sum, "ถ้า Damage ที่ได้รับมากกว่า " + param1 + "% ของ MaxHP จะใช้การคำนวณใหม่ (Damage เดิม - " + param1 + "% MaxHP) (ใช้ค่ามากสุดเท่านั้น)");
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
                    sum += AddDescription(sum, "การโจมตีเป็นธาตุ " + GetElementName(allParam[0]));
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
                    sum += AddDescription(sum, "การป้องกันเป็นธาตุ " + GetElementName(allParam[0]));
            }
            #endregion
            #region bonus2 bMagicAtkEle
            functionName = "bonus2 bMagicAtkEle";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "โจมตีเบาลง " + param2 + "% กับเวทย์มนต์ธาตุ " + GetElementName(allParam[0]));
                    else
                        sum += AddDescription(sum, "โจมตีแรงขึ้น " + param2 + "% กับเวทย์มนต์ธาตุ " + GetElementName(allParam[0]));
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
                    sum += AddDescription(sum, "โจมตีแรงขึ้นตาม DEF ของศัตรู กับเผ่า " + GetRaceName(allParam[0]));
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
                    sum += AddDescription(sum, "โจมตีแรงขึ้นตาม DEF ของศัตรู กับธาตุ " + GetElementName(allParam[0]));
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
                    sum += AddDescription(sum, "โจมตีแรงขึ้นตาม DEF ของศัตรู กับ Class " + GetClassName(allParam[0]));
            }
            #endregion
            #region bonus4 bSetDefRace
            functionName = "bonus4 bSetDefRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4)
                    sum += AddDescription(sum, "เมื่อโจมตีกายภาพมีโอกาส " + param2 + "% ทำให้ศัตรูเหลือ DEF " + param4 + " กับเผ่า " + GetRaceName(allParam[0]) + " เป็นเวลา " + TimerToStringTimer(float.Parse(param3)));
            }
            #endregion
            #region bonus4 bSetMDefRace
            functionName = "bonus4 bSetMDefRace";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4)
                    sum += AddDescription(sum, "เมื่อโจมตีกายภาพมีโอกาส " + param2 + "% ทำให้ศัตรูเหลือ MDEF " + param4 + " กับเผ่า " + GetRaceName(allParam[0]) + " เป็นเวลา " + TimerToStringTimer(float.Parse(param3)));
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
                    sum += AddDescription(sum, "ไม่สนใจ DEF กับธาตุ " + GetElementName(allParam[0]));
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
                    sum += AddDescription(sum, "ไม่สนใจ DEF กับเผ่า " + GetRaceName(allParam[0]));
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
                    sum += AddDescription(sum, "ไม่สนใจ DEF กับ Class " + GetClassName(allParam[0]));
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
                    sum += AddDescription(sum, "ไม่สนใจ MDEF กับเผ่า " + GetRaceName(allParam[0]));
            }
            #endregion
            #region bonus2 bIgnoreDefRaceRate
            functionName = "bonus2 bIgnoreDefRaceRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ไม่สนใจ DEF น้อยลง " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ไม่สนใจ DEF " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ไม่สนใจ MDEF น้อยลง " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ไม่สนใจ MDEF " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ไม่สนใจ MDEF น้อยลง " + param2 + "% กับเผ่า " + GetMonsterRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ไม่สนใจ MDEF " + param2 + "% กับเผ่า " + GetMonsterRaceName(allParam[0]));
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
                    sum += AddDescription(sum, "ไม่สนใจ MDEF กับธาตุ " + GetElementName(allParam[0]));
            }
            #endregion
            #region bonus2 bIgnoreDefClassRate
            functionName = "bonus2 bIgnoreDefClassRate";
            if (data.Contains(functionName))
            {
                string sumCut = CutFunctionName(data, functionName);

                List<string> allParam = GetAllParamerters(sumCut);

                string param1 = GetValue(allParam[0], 1);
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ไม่สนใจ DEF น้อยลง " + param2 + "% กับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ไม่สนใจ DEF " + param2 + "% กับ Class " + GetClassName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ไม่สนใจ MDEF น้อยลง " + param2 + "% กับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ไม่สนใจ MDEF " + param2 + "% กับ Class " + GetClassName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ได้รับ EXP น้อยลง " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ได้รับ EXP มากขึ้น " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ได้รับ EXP น้อยลง " + param2 + "% กับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ได้รับ EXP มากขึ้น " + param2 + "% กับ Class " + GetClassName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับตนเองเมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับตนเองเมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโดนโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโดนโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะไม่เกิด " + GetEffectName(allParam[0]) + " กับตนเอง");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะไม่เกิด " + GetEffectName(allParam[0]) + " กับตนเอง");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโจมตีกายภาพ" + GetTriggerCriteria(allParam[2]));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโจมตีกายภาพ" + GetTriggerCriteria(allParam[2]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโจมตีกายภาพ" + GetTriggerCriteria(allParam[2]) + " เป็นเวลา " + TimerToStringTimer(float.Parse(param4)));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโจมตีกายภาพ" + GetTriggerCriteria(allParam[2]) + " เป็นเวลา " + TimerToStringTimer(float.Parse(param4)));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโดนโจมตีกายภาพ" + GetTriggerCriteria(allParam[2]));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโดนโจมตีกายภาพ" + GetTriggerCriteria(allParam[2]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโดนโจมตีกายภาพ" + GetTriggerCriteria(allParam[2]) + " เป็นเวลา " + TimerToStringTimer(float.Parse(param4)));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด " + GetEffectName(allParam[0]) + " กับศัตรูเมื่อโดนโจมตีกายภาพ" + GetTriggerCriteria(allParam[2]) + " เป็นเวลา " + TimerToStringTimer(float.Parse(param4)));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะเกิด " + GetEffectName(allParam[1]) + " กับศัตรูเมื่อร่าย Skill " + GetSkillName(allParam[0]));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะเกิด " + GetEffectName(allParam[1]) + " กับศัตรูเมื่อร่าย Skill " + GetSkillName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะเกิด " + GetEffectName(allParam[1]) + " กับศัตรูเมื่อร่าย Skill" + GetTriggerCriteria(allParam[0]) + GetTriggerCriteria(allParam[3]));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะเกิด " + GetEffectName(allParam[1]) + " กับศัตรูเมื่อร่าย Skill" + GetTriggerCriteria(allParam[0]) + GetTriggerCriteria(allParam[3]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);
                string param5 = GetValue(allParam[4], 5);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4 && isHadParam5)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะเกิด " + GetEffectName(allParam[1]) + " กับศัตรูเมื่อร่าย Skill" + GetTriggerCriteria(allParam[0]) + GetTriggerCriteria(allParam[3]) + " เป็นเวลา " + TimerToStringTimer(float.Parse(param5)));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะเกิด " + GetEffectName(allParam[1]) + " กับศัตรูเมื่อร่าย Skill" + GetTriggerCriteria(allParam[0]) + GetTriggerCriteria(allParam[3]) + " เป็นเวลา " + TimerToStringTimer(float.Parse(param5)));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับ Class " + GetClassName(allParam[0]) + " เมื่อโจมตี");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับ Class " + GetClassName(allParam[0]) + " เมื่อโจมตี");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับเผ่า " + GetRaceName(allParam[0]) + " เมื่อโจมตี");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับเผ่า " + GetRaceName(allParam[0]) + " เมื่อโจมตี");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับธาตุ " + GetElementName(allParam[0]) + " เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับธาตุ " + GetElementName(allParam[0]) + " เมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับ Class " + GetClassName(allParam[0]) + " เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับ Class " + GetClassName(allParam[0]) + " เมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับเผ่า " + GetRaceName(allParam[0]) + " เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะเกิด Coma กับเผ่า " + GetRaceName(allParam[0]) + " เมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " เมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " เมื่อโดนโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " เมื่อโดนโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " " + GetAutoSpellFlagName(allParam[3]) + " เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " " + GetAutoSpellFlagName(allParam[3]) + " เมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);
                string param5 = GetValue(allParam[4], 5);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4 && isHadParam5)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " " + GetAutoSpellFlagName(allParam[4]) + " เมื่อโจมตีกายภาพ" + GetTriggerCriteria(allParam[3]));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " " + GetAutoSpellFlagName(allParam[4]) + " เมื่อโจมตีกายภาพ" + GetTriggerCriteria(allParam[3]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " " + GetAutoSpellFlagName(allParam[3]) + " เมื่อโดนโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " " + GetAutoSpellFlagName(allParam[3]) + " เมื่อโดนโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);
                string param5 = GetValue(allParam[4], 5);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4 && isHadParam5)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " " + GetAutoSpellFlagName(allParam[4]) + " เมื่อโดนโจมตีกายภาพ" + GetTriggerCriteria(allParam[3]));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param3, 10) + "% ที่จะร่าย Lv. " + param2 + " " + GetSkillName(allParam[0]) + " " + GetAutoSpellFlagName(allParam[4]) + " เมื่อโดนโจมตีกายภาพ" + GetTriggerCriteria(allParam[3]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4)
                {
                    if (isParam4Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param4, 10) + "% ที่จะร่าย Lv. " + param3 + " " + GetSkillName(allParam[0]) + " เมื่อร่าย Skill " + GetSkillName(allParam[1]));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param4, 10) + "% ที่จะร่าย Lv. " + param3 + " " + GetSkillName(allParam[0]) + " เมื่อร่าย Skill " + GetSkillName(allParam[1]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);
                string param4 = GetValue(allParam[3], 4);
                string param5 = GetValue(allParam[4], 5);

                if (isHadParam1 && isHadParam2 && isHadParam3 && isHadParam4 && isHadParam5)
                {
                    if (isParam4Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param4, 10) + "% ที่จะร่าย Lv. " + param3 + " " + GetSkillName(allParam[0]) + GetAutoSpellOnSkillFlagName(param5, param3) + " เมื่อร่าย Skill " + GetSkillName(allParam[1]));
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param4, 10) + "% ที่จะร่าย Lv. " + param3 + " " + GetSkillName(allParam[0]) + GetAutoSpellOnSkillFlagName(param5, param3) + " เมื่อร่าย Skill " + GetSkillName(allParam[1]));
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
                        sum += AddDescription(sum, "เสีย " + param1 + " HP เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " HP เมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "เสีย " + param2 + " HP เมื่อโจมตีกายภาพกับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param2 + " HP เมื่อโจมตีกายภาพกับเผ่า " + GetRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "เสีย " + param2 + " HP เมื่อโจมตีกายภาพกับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param2 + " HP เมื่อโจมตีกายภาพกับ Class " + GetClassName(allParam[0]));
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
                        sum += AddDescription(sum, "เสีย " + param1 + " SP เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " SP เมื่อโจมตีกายภาพ");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "เสีย " + param2 + " SP เมื่อโจมตีกายภาพกับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param2 + " SP เมื่อโจมตีกายภาพกับเผ่า " + GetRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "เสีย " + param2 + " SP เมื่อโจมตีกายภาพกับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param2 + " SP เมื่อโจมตีกายภาพกับ Class " + GetClassName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    string sumParam1 = null;
                    string sumParam2 = null;

                    if (isParam1Negative)
                        sumParam1 = "ลดโอกาส";
                    else
                        sumParam1 = "มีโอกาส";

                    if (isParam2Negative)
                        sumParam2 = "เสีย";
                    else
                        sumParam2 = "ฟื้นฟู";

                    sum += AddDescription(sum, sumParam1 + " " + param1 + "% ที่จะ" + sumParam2 + " HP " + GetRateByDivider(param2, 10) + "% จาก Damage การโจมตีกายภาพที่ทำไป");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    string sumParam1 = null;
                    string sumParam2 = null;

                    if (isParam1Negative)
                        sumParam1 = "ลดโอกาส";
                    else
                        sumParam1 = "มีโอกาส";

                    if (isParam2Negative)
                        sumParam2 = "เสีย";
                    else
                        sumParam2 = "ฟื้นฟู";

                    sum += AddDescription(sum, sumParam1 + " " + param1 + "% ที่จะ" + sumParam2 + " SP " + GetRateByDivider(param2, 10) + "% จาก Damage การโจมตีกายภาพที่ทำไป");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    string sumParam1 = null;
                    string sumParam2 = null;

                    if (isParam1Negative)
                        sumParam1 = "ลดโอกาส";
                    else
                        sumParam1 = "มีโอกาส";

                    if (isParam2Negative)
                        sumParam2 = "ฟื้นฟู";
                    else
                        sumParam2 = "เสีย";

                    sum += AddDescription(sum, sumParam1 + " " + param1 + "% ที่ศัตรูจะ" + sumParam2 + " HP " + GetRateByDivider(param2, 10) + "% จาก Damage การโจมตีกายภาพที่ทำไป");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    string sumParam2 = null;
                    string sumParam3 = null;

                    if (isParam2Negative)
                        sumParam2 = "ลดโอกาส";
                    else
                        sumParam2 = "มีโอกาส";

                    if (isParam3Negative)
                        sumParam3 = "ฟื้นฟู";
                    else
                        sumParam3 = "เสีย";

                    sum += AddDescription(sum, sumParam2 + " " + param2 + "% ที่ศัตรูจะ" + sumParam3 + " HP " + GetRateByDivider(param3, 10) + "% กับเผ่า " + GetRaceName(allParam[0]) + " จาก Damage การโจมตีกายภาพที่ทำไป");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    string sumParam1 = null;
                    string sumParam2 = null;

                    if (isParam1Negative)
                        sumParam1 = "ลดโอกาส";
                    else
                        sumParam1 = "มีโอกาส";

                    if (isParam2Negative)
                        sumParam2 = "ฟื้นฟู";
                    else
                        sumParam2 = "เสีย";

                    sum += AddDescription(sum, sumParam1 + " " + param1 + "% ที่ศัตรูจะ" + sumParam2 + " HP " + GetRateByDivider(param2, 10) + "% จาก Damage การโจมตีกายภาพที่ทำไป" + GetTriggerCriteria(allParam[2]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    string sumParam1 = null;
                    string sumParam2 = null;

                    if (isParam1Negative)
                        sumParam1 = "ลดโอกาส";
                    else
                        sumParam1 = "มีโอกาส";

                    if (isParam2Negative)
                        sumParam2 = "ฟื้นฟู";
                    else
                        sumParam2 = "เสีย";

                    sum += AddDescription(sum, sumParam1 + " " + param1 + "% ที่ศัตรูจะ" + sumParam2 + " SP " + GetRateByDivider(param2, 10) + "% จาก Damage การโจมตีกายภาพที่ทำไป");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    string sumParam2 = null;
                    string sumParam3 = null;

                    if (isParam2Negative)
                        sumParam2 = "ลดโอกาส";
                    else
                        sumParam2 = "มีโอกาส";

                    if (isParam3Negative)
                        sumParam3 = "ฟื้นฟู";
                    else
                        sumParam3 = "เสีย";

                    sum += AddDescription(sum, sumParam2 + " " + param2 + "% ที่ศัตรูจะ" + sumParam3 + " SP " + GetRateByDivider(param3, 10) + "% กับเผ่า " + GetRaceName(allParam[0]) + " จาก Damage การโจมตีกายภาพที่ทำไป");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    string sumParam1 = null;
                    string sumParam2 = null;

                    if (isParam1Negative)
                        sumParam1 = "ลดโอกาส";
                    else
                        sumParam1 = "มีโอกาส";

                    if (isParam2Negative)
                        sumParam2 = "ฟื้นฟู";
                    else
                        sumParam2 = "เสีย";

                    sum += AddDescription(sum, sumParam1 + " " + param1 + "% ที่ศัตรูจะ" + sumParam2 + " SP " + GetRateByDivider(param2, 10) + "% จาก Damage การโจมตีกายภาพที่ทำไป" + GetTriggerCriteria(allParam[2]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะทำให้ศัตรูเผ่า " + GetRaceName(allParam[0]) + " ไม่สามารถฟื้นฟู HP,SP ได้เป็นเวลา " + TimerToStringTimer(float.Parse(param3)) + " เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะทำให้ศัตรูเผ่า " + GetRaceName(allParam[0]) + " ไม่สามารถฟื้นฟู HP,SP ได้เป็นเวลา " + TimerToStringTimer(float.Parse(param3)) + " เมื่อโจมตีกายภาพ");
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
                        sum += AddDescription(sum, "เสีย " + param1 + " HP เมื่อกำจัดศัตรูด้วยการโจมตีกายภาพระยะประชิด");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " HP เมื่อกำจัดศัตรูด้วยการโจมตีกายภาพระยะประชิด");
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
                        sum += AddDescription(sum, "เสีย " + param1 + " SP เมื่อกำจัดศัตรูด้วยการโจมตีกายภาพระยะประชิด");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " SP เมื่อกำจัดศัตรูด้วยการโจมตีกายภาพระยะประชิด");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "เสีย " + param2 + " SP เมื่อกำจัดศัตรูเผ่า " + GetRaceName(allParam[0]) + " ด้วยการโจมตีกายภาพระยะประชิด");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param2 + " SP เมื่อกำจัดศัตรูเผ่า " + GetRaceName(allParam[0]) + " ด้วยการโจมตีกายภาพระยะประชิด");
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
                        sum += AddDescription(sum, "เสีย " + param1 + " HP เมื่อกำจัดศัตรูด้วยการโจมตีกายภาพระยะไกล");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " HP เมื่อกำจัดศัตรูด้วยการโจมตีกายภาพระยะไกล");
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
                        sum += AddDescription(sum, "เสีย " + param1 + " SP เมื่อกำจัดศัตรูด้วยการโจมตีกายภาพระยะไกล");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " SP เมื่อกำจัดศัตรูด้วยการโจมตีกายภาพระยะไกล");
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
                        sum += AddDescription(sum, "เสีย " + param1 + " HP เมื่อกำจัดศัตรูด้วยการโจมตีเวทย์");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " HP เมื่อกำจัดศัตรูด้วยการโจมตีเวทย์");
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
                        sum += AddDescription(sum, "เสีย " + param1 + " SP เมื่อกำจัดศัตรูด้วยการโจมตีเวทย์");
                    else
                        sum += AddDescription(sum, "ฟื้นฟู " + param1 + " SP เมื่อกำจัดศัตรูด้วยการโจมตีเวทย์");
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
                        sum += AddDescription(sum, "ลดการสะท้อนการโจมตีระยะประชิด " + param1 + "%");
                    else
                        sum += AddDescription(sum, "สะท้อนการโจมตีระยะประชิด " + param1 + "%");
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
                        sum += AddDescription(sum, "ลดการสะท้อนการโจมตีระยะไกล " + param1 + "%");
                    else
                        sum += AddDescription(sum, "สะท้อนการโจมตีระยะไกล " + param1 + "%");
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
                        sum += AddDescription(sum, "ลดการสะท้อนการโจมตีเวทย์ " + param1 + "%");
                    else
                        sum += AddDescription(sum, "สะท้อนการโจมตีเวทย์ " + param1 + "%");
                }
            }
            #endregion
            #region bonus bUnstripableWeapon
            functionName = "bonus bUnstripableWeapon";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "อาวุธจะไม่มีทางถูกปลดโดยศัตรูได้");
            #endregion
            #region bonus bUnstripableArmor
            functionName = "bonus bUnstripableArmor";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ชุดเกราะจะไม่มีทางถูกปลดโดยศัตรูได้");
            #endregion
            #region bonus bUnstripableHelm
            functionName = "bonus bUnstripableHelm";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "หมวกจะไม่มีทางถูกปลดโดยศัตรูได้");
            #endregion
            #region bonus bUnstripableShield
            functionName = "bonus bUnstripableShield";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "โล่จะไม่มีทางถูกปลดโดยศัตรูได้");
            #endregion
            #region bUnstripable
            functionName = "bUnstripable";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "อุปกรณ์สวมใส่ทุกชนิดจะไม่มีทางถูกปลดโดยศัตรูได้");
            #endregion
            #region bonus bUnbreakableGarment
            functionName = "bonus bUnbreakableGarment";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ผ้าคลุมจะไม่มีทางชำรุด");
            #endregion
            #region bonus bUnbreakableWeapon
            functionName = "bonus bUnbreakableWeapon";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "อาวุธจะไม่มีทางชำรุด");
            #endregion
            #region bonus bUnbreakableArmor
            functionName = "bonus bUnbreakableArmor";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ชุดเกราะจะไม่มีทางชำรุด");
            #endregion
            #region bonus bUnbreakableHelm
            functionName = "bonus bUnbreakableHelm";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "หมวกจะไม่มีทางชำรุด");
            #endregion
            #region bonus bUnbreakableShield
            functionName = "bonus bUnbreakableShield";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "โล่จะไม่มีทางชำรุด");
            #endregion
            #region bonus bUnbreakableShoes
            functionName = "bonus bUnbreakableShoes";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "รองเท้าจะไม่มีทางชำรุด");
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
                        sum += AddDescription(sum, "ลดโอกาส " + param1 + "% ที่อุปกรณ์สวมใส่ทุกชนิดจะไม่มีทางชำรุด");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + param1 + "% ที่อุปกรณ์สวมใส่ทุกชนิดจะไม่มีทางชำรุด");
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
                        sum += AddDescription(sum, "ลดโอกาส " + param1 + "% ที่จะทำให้อาวุธศัตรูชำรุด");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + param1 + "% ที่จะทำให้อาวุธศัตรูชำรุด");
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
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param1, 100) + "% ที่จะทำให้ชุดเกราะศัตรูชำรุด");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param1, 100) + "% ที่จะทำให้ชุดเกราะศัตรูชำรุด");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดอัตรา Drop " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
                    else
                        sum += AddDescription(sum, "เพิ่มอัตรา Drop " + param2 + "% กับเผ่า " + GetRaceName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดอัตรา Drop " + param2 + "% กับ Class " + GetClassName(allParam[0]));
                    else
                        sum += AddDescription(sum, "เพิ่มอัตรา Drop " + param2 + "% กับ Class " + GetClassName(allParam[0]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop " + GetItemName(param1) + " เมื่อกำจัด " + GetMIDName(allParam[1]));
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop " + GetItemName(param1) + " เมื่อกำจัด " + GetMIDName(allParam[1]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะ Drop " + GetItemName(param1) + " เมื่อกำจัด Monster");
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะ Drop " + GetItemName(param1) + " เมื่อกำจัด Monster");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop " + GetItemName(param1) + " เมื่อกำจัด Monster เผ่า " + GetRaceName(allParam[1]));
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop " + GetItemName(param1) + " เมื่อกำจัด Monster เผ่า" + GetRaceName(allParam[1]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop " + GetItemName(param1) + " เมื่อกำจัด Monster Class " + GetClassName(allParam[1]));
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop " + GetItemName(param1) + " เมื่อกำจัด Monster Class" + GetClassName(allParam[1]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะ Drop Item Group " + param1 + " เมื่อกำจัด Monster");
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + GetRateByDivider(param2, 100) + "% ที่จะ Drop Item Group " + param1 + " เมื่อกำจัด Monster");
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop Item Group " + param1 + " เมื่อกำจัด Monster เผ่า " + GetRaceName(allParam[1]));
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop Item Group " + param1 + " เมื่อกำจัด Monster เผ่า " + GetRaceName(allParam[1]));
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
                string param2 = GetValue(allParam[1], 2);
                string param3 = GetValue(allParam[2], 3);

                if (isHadParam1 && isHadParam2 && isHadParam3)
                {
                    if (isParam3Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop Item Group " + param1 + " เมื่อกำจัด Monster Class " + GetClassName(allParam[1]));
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + GetRateByDivider(param3, 100) + "% ที่จะ Drop Item Group " + param1 + " เมื่อกำจัด Monster Class " + GetClassName(allParam[1]));
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + param2 + "% ที่จะได้รับ 1~" + param1 + " Zeny (ใช้ค่ามากสุดเท่านั้น)");
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + param2 + "% ที่จะได้รับ 1~" + param1 + " Zeny (ใช้ค่ามากสุดเท่านั้น)");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "ลดโอกาส " + param2 + "% ที่จะได้รับ 1~" + param1 + " Zeny");
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + param2 + "% ที่จะได้รับ 1~" + param1 + " Zeny");
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
                        sum += AddDescription(sum, "ลดโอกาส " + param1 + "% ที่จะโจมตีสองครั้งภายในครั้งเดียว");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + param1 + "% ที่จะโจมตีสองครั้งภายในครั้งเดียว");
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
                    int splash = int.Parse(param1);
                    int sumSplash = 1 + (2 * splash);
                    sum += AddDescription(sum, "โจมตีกระจาย " + sumSplash + "x" + sumSplash + " ช่อง (ใช้ค่ามากสุดเท่านั้น)");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    int splash = int.Parse(param1);
                    int sumSplash = 1 + (2 * splash);
                    sum += AddDescription(sum, "โจมตีกระจาย " + sumSplash + "x" + sumSplash + " ช่อง");
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
                string param2 = GetValue(allParam[1], 2);

                if (isHadParam1 && isHadParam2)
                {
                    if (isParam2Negative)
                        sum += AddDescription(sum, "เมื่อร่าย Skill " + GetSkillName(allParam[0]) + " จะลดการทำให้ศัตรูกระเด็น " + param2 + " ช่อง ");
                    else
                        sum += AddDescription(sum, "เมื่อร่าย Skill " + GetSkillName(allParam[0]) + " จะทำให้ศัตรูกระเด็น " + param2 + " ช่อง ");
                }
            }
            #endregion
            #region bonus bNoKnockback
            functionName = "bonus bNoKnockback";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ไม่มีทางที่จะถูกทำให้กระเด็น");
            #endregion
            #region bonus bNoGemStone
            functionName = "bonus bNoGemStone";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ร่าย Skill ไม่ต้องใช้ Gemstones");
            #endregion
            #region bonus bIntravision
            functionName = "bonus bIntravision";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "มองเห็นศัตรูที่ Hidden, Cloaking ได้ตลอดเวลา");
            #endregion
            #region bonus bPerfectHide
            functionName = "bonus bPerfectHide";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "Monster จะไม่สามารถใช้ Skill Detect เพื่อตรวจจับคุณได้ขณะ Hidden, Cloaking");
            #endregion
            #region bonus bRestartFullRecover
            functionName = "bonus bRestartFullRecover";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "HP และ SP เต็ม เมื่อได้สติกลับมาอีกครั้ง");
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
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param1, 100) + "% ที่จะเปลี่ยนร่าง Monster เมื่อโจมตีกายภาพ");
                    else
                        sum += AddDescription(sum, "มีโอกาส " + GetRateByDivider(param1, 100) + "% ที่จะเปลี่ยนร่าง Monster เมื่อโจมตีกายภาพ");
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
                        sum += AddDescription(sum, "ลดโอกาส " + GetRateByDivider(param1, 100) + "% ที่ Skill Steal จะสำเร็จ");
                    else
                        sum += AddDescription(sum, "เพิ่มโอกาส " + GetRateByDivider(param1, 100) + "% ที่ Skill Steal จะสำเร็จ");
                }
            }
            #endregion
            #region bonus bNoMadoFuel
            functionName = "bonus bNoMadoFuel";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "Skill ของ Mado Gear ไม่ต้องใช้ Fuel อีกต่อไป");
            #endregion
            #region bonus bNoWalkDelay
            functionName = "bonus bNoWalkDelay";
            if (data.Contains(functionName))
                sum += AddDescription(sum, "ไม่มีอาการชะงักเมื่อโดนโจมตี");
            #endregion
            #region [TEMP_VAR]
            functionName = "[TEMP_VAR]";
            if (data.Contains(functionName))
            {
                data = data.Replace("[TEMP_VAR]", "");
                data = GetValue(data, -1, true);
                data = data.Replace("ค่าที่", "ค่าที่ ");
                data = data.Replace("+=", " เพิ่ม ");
                data = data.Replace("-=", " ลด ");
                data = data.Replace("*=", " คูณ ");
                data = data.Replace("/=", " หาร ");
                sum += AddDescription(sum, data);
            }
            #endregion
            #region [TEMP_VAR_DECLARE]
            functionName = "[TEMP_VAR_DECLARE]";
            if (data.Contains(functionName))
            {
                data = data.Replace("[TEMP_VAR_DECLARE]", "");
                data = GetValue(data, -1, true, true);
                data = data.Replace("ค่าที่", "ค่าที่ ");
                data = data.Replace("+=", " เพิ่ม ");
                data = data.Replace("-=", " ลด ");
                data = data.Replace("*=", " คูณ ");
                data = data.Replace("/=", " หาร ");
                data = data.Replace(":", ": ");
                data = data.Replace("꞉", "꞉ ");
                sum += AddDescription(sum, "● " + data);
            }
            #endregion
            #region Set Temporary Variables
            for (int j = 0; j < tempVariables.Count; j++)
            {
                functionName = tempVariables[j].variableName;
                if (data.Contains(functionName))
                {
                    if ((i - 1) < 0)
                        break;
                    if (lines[i - 1] == tempVariables[j].toCheckMatching)
                    {
                        string tempVarValue = tempVariables[j].value;
                        tempVarValue = tempVarValue.Replace(";", "");
                        sum += AddDescription(sum, "เปลี่ยนค่าที่ได้รับเป็น " + tempVarValue);
                    }
                }
            }
            #endregion
        }

        return sum;
    }

    string GetBonusScript(string data)
    {
        data = data.Replace("[TEMP_VAR]", "");
        data = data.Replace("[TEMP_VAR_DECLARE]", "");

        Log("GetBonusScript >> data: " + data);

        string saveData = data;

        List<string> toReplace = new List<string>();
        List<string> toReplaceValue = new List<string>();

        //itemheal inside room
        while (data.Contains("itemheal"))
        {
            //Loop all char
            for (int j = 0; j < data.Length; j++)
            {
                int declareAt1 = data.IndexOf("itemheal");
                if (declareAt1 == -1)
                    break;
                string sub = data.Substring(declareAt1);
                int declareAt2 = sub.IndexOf(";");
                if (declareAt2 == -1)
                    break;
                string toConvert = sub.Substring(0, declareAt2 + 1);
                Log("GetBonusScript >> itemheal#1: " + toConvert);
                string convert = GetScriptsDescription(toConvert, true);
                convert = convert.Replace("\"", " ");
                if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                    convert = convert.Substring(0, convert.Length - 1);
                Log("GetBonusScript >> itemheal#2: " + toConvert);
                toReplace.Add(toConvert);
                toReplaceValue.Add(convert + "\",\n\"");
                data = data.Replace(toConvert, convert);
                Log("GetBonusScript >> itemheal#3: " + data);
            }
        }

        //percentheal inside room
        while (data.Contains("percentheal"))
        {
            //Loop all char
            for (int j = 0; j < data.Length; j++)
            {
                int declareAt1 = data.IndexOf("percentheal");
                if (declareAt1 == -1)
                    break;
                string sub = data.Substring(declareAt1);
                int declareAt2 = sub.IndexOf(";");
                if (declareAt2 == -1)
                    break;
                string toConvert = sub.Substring(0, declareAt2 + 1);
                Log("GetBonusScript >> percentheal#1: " + toConvert);
                string convert = GetScriptsDescription(toConvert, true);
                convert = convert.Replace("\"", " ");
                if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                    convert = convert.Substring(0, convert.Length - 1);
                Log("GetBonusScript >> percentheal#2: " + toConvert);
                toReplace.Add(toConvert);
                toReplaceValue.Add(convert + "\",\n\"");
                data = data.Replace(toConvert, convert);
                Log("GetBonusScript >> percentheal#3: " + data);
            }
        }

        //heal inside room
        while (data.Contains("heal"))
        {
            //Loop all char
            for (int j = 0; j < data.Length; j++)
            {
                int declareAt1 = data.IndexOf("heal");
                if (declareAt1 == -1)
                    break;
                string sub = data.Substring(declareAt1);
                int declareAt2 = sub.IndexOf(";");
                if (declareAt2 == -1)
                    break;
                string toConvert = sub.Substring(0, declareAt2 + 1);
                Log("GetBonusScript >> heal#1: " + toConvert);
                string convert = GetScriptsDescription(toConvert, true);
                convert = convert.Replace("\"", " ");
                if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                    convert = convert.Substring(0, convert.Length - 1);
                Log("GetBonusScript >> heal#2: " + toConvert);
                toReplace.Add(toConvert);
                toReplaceValue.Add(convert + "\",\n\"");
                data = data.Replace(toConvert, convert);
                Log("GetBonusScript >> heal#3: " + data);
            }
        }

        //sc_start inside room
        while (data.Contains("sc_start"))
        {
            //Loop all char
            for (int j = 0; j < data.Length; j++)
            {
                int declareAt1 = data.IndexOf("sc_start");
                if (declareAt1 == -1)
                    break;
                string sub = data.Substring(declareAt1);
                int declareAt2 = sub.IndexOf(";");
                if (declareAt2 == -1)
                    break;
                string toConvert = sub.Substring(0, declareAt2 + 1);
                Log("GetBonusScript >> sc_start#1: " + toConvert);
                string convert = GetScriptsDescription(toConvert, true);
                convert = convert.Replace("\"", " ");
                if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                    convert = convert.Substring(0, convert.Length - 1);
                Log("GetBonusScript >> sc_start#2: " + toConvert);
                toReplace.Add(toConvert);
                toReplaceValue.Add(convert + "\",\n\"");
                data = data.Replace(toConvert, convert);
                Log("GetBonusScript >> sc_start#3: " + data);
            }
        }

        //sc_end inside room
        while (data.Contains("sc_end"))
        {
            //Loop all char
            for (int j = 0; j < data.Length; j++)
            {
                int declareAt1 = data.IndexOf("sc_end");
                if (declareAt1 == -1)
                    break;
                string sub = data.Substring(declareAt1);
                int declareAt2 = sub.IndexOf(";");
                if (declareAt2 == -1)
                    break;
                string toConvert = sub.Substring(0, declareAt2 + 1);
                Log("GetBonusScript >> sc_end#1: " + toConvert);
                string convert = GetScriptsDescription(toConvert, true);
                convert = convert.Replace("\"", " ");
                if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                    convert = convert.Substring(0, convert.Length - 1);
                Log("GetBonusScript >> sc_end#2: " + toConvert);
                toReplace.Add(toConvert);
                toReplaceValue.Add(convert + "\",\n\"");
                data = data.Replace(toConvert, convert);
                Log("GetBonusScript >> sc_end#3: " + data);
            }
        }

        //Bonus inside room
        while (data.Contains("bonus"))
        {
            //Loop all char
            for (int j = 0; j < data.Length; j++)
            {
                int declareAt1 = data.IndexOf("bonus");
                if (declareAt1 == -1)
                    break;
                string sub = data.Substring(declareAt1);
                int declareAt2 = sub.IndexOf(";");
                if (declareAt2 == -1)
                    break;
                string toConvert = sub.Substring(0, declareAt2 + 1);
                Log("GetBonusScript >> bonus#1: " + toConvert);
                string convert = GetScriptsDescription(toConvert, true);
                convert = convert.Replace("\"", " ");
                if (convert.Length > 0 && convert[convert.Length - 1] == ',')
                    convert = convert.Substring(0, convert.Length - 1);
                Log("GetBonusScript >> bonus#2: " + toConvert);
                toReplace.Add(toConvert);
                toReplaceValue.Add(convert + "\",\n\"");
                data = data.Replace(toConvert, convert);
                Log("GetBonusScript >> bonus#3: " + data);
            }
        }

        if (toReplaceValue.Count > 0)
            toReplaceValue[toReplaceValue.Count - 1] = toReplaceValue[toReplaceValue.Count - 1].Replace("\",\n\"", "");

        data = saveData;

        //Remove spacebar
        data = MergeWhiteSpace.RemoveWhiteSpace(data);

        for (int j = 0; j < toReplace.Count; j++)
            data = data.Replace(MergeWhiteSpace.RemoveWhiteSpace(toReplace[j]), toReplaceValue[j]);

        data = data.Replace("elseif(", " หรือถ้า ");
        data = data.Replace("if(", " ถ้า ");
        data = ReplaceAllSpecialValue(data);
        data = data.Replace("(", "");
        data = data.Replace(")", "");
        data = data.Replace(";", "");

        data = data.Replace("==", " คือ ");
        data = data.Replace("!=", " ไม่เท่ากับ ");
        data = data.Replace("||", " หรือ ");
        data = data.Replace("&&", " และ ");
        data = data.Replace(">=", " มากกว่าหรือเท่ากับ ");
        data = data.Replace(">", " มากกว่า ");
        data = data.Replace("<=", " น้อยกว่าหรือเท่ากับ ");
        data = data.Replace("<", " น้อยกว่า ");
        data = data.Replace("Job_", "");
        data = data.Replace("_", " ");
        data = data.Replace("getpartnerid()", "มีคู่สมรส");
        data = data.Replace("{", " รับ ");
        data = data.Replace("}", " ");
        data = data.Replace("ค่าที่", " ค่าที่ ");
        data = data.Replace(" , ", ", ");

        //Use store temporary variables if found in this value
        bool isFoundTempVariable = false;
        List<string> tempVarName = new List<string>();
        List<string> valueFromTempVar = new List<string>();
        for (int j = 0; j < tempVariables.Count; j++)
        {
            if (data.Contains(tempVariables[j].variableName))
            {
                isFoundTempVariable = true;

                tempVarName.Add(tempVariables[j].variableName);
                Log("GetBonusScript >> Found variableName: " + tempVariables[j].variableName);

                valueFromTempVar.Add(tempVariables[j].value);
                Log("GetBonusScript >> Found value: " + tempVariables[j].value);
            }
        }

        //Replace temporary variables
        if (isFoundTempVariable)
        {
            for (int j = 0; j < tempVarName.Count; j++)
                data = data.Replace(tempVarName[j], valueFromTempVar[j]);
        }

        //Replace special variables
        data = ReplaceAllSpecialValue(data);

        return data;
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

    bool IsContainsTemporaryVariables(string txt, List<string> allCut, int mergeIndex, bool isNotAddCheckMatching = false)
    {
        if (txt.Contains(".@"))
        {
            if (txt.Contains(".@") && !txt.Contains(";"))
            {
                MergeItemScripts(allCut, mergeIndex);
                return true;
            }
            else if (!isNotAddCheckMatching)
                AddTemporaryVariable(txt, allCut[mergeIndex - 1]);
            else
                AddTemporaryVariable(txt);
        }
        return false;
    }
    void SearchForTemporaryVariables(string txt)
    {
        if (txt.Contains(".@") && txt.Contains(";"))
            AddTemporaryVariable(txt + "[DONT_DECLARE]", null, true);
    }

    /// <summary>
    /// Add temporary variables
    /// </summary>
    /// <param name="txt"></param>
    void AddTemporaryVariable(string txt, string toCheckMatching = null, bool isFromAutoBonusOneLine = false)
    {
        TempVariables newTempVariables = new TempVariables();

        int declareAt = txt.IndexOf("=");

        if (declareAt <= -1 || txt.Length < declareAt)
            return;

        int startCut = 0;
        int foundTempVarCount = 0;
        if (isFromAutoBonusOneLine)
        {
            for (int i = declareAt; i > 0; i--)
            {
                if (txt[i] == '@' || txt[i] == '.')
                    foundTempVarCount++;
                if (foundTempVarCount == 2)
                {
                    startCut = i;
                    break;
                }
            }
        }
        string txtLeftSide = txt.Substring(startCut, declareAt - startCut);
        string txtRightSide = txt.Substring(declareAt + 1);
        int semiColonAt = txtRightSide.IndexOf(";");
        txtRightSide = txtRightSide.Substring(0, semiColonAt);

        newTempVariables.variableName = MergeWhiteSpace.RemoveWhiteSpace(txtLeftSide);
        newTempVariables.value = MergeWhiteSpace.RemoveWhiteSpace(txtRightSide);
        newTempVariables.toCheckMatching = toCheckMatching;

        if (newTempVariables.variableName.Contains("+")
            || newTempVariables.variableName.Contains(">")
            || newTempVariables.variableName.Contains("<")
            || newTempVariables.variableName.Contains("-")
            || newTempVariables.variableName.Contains("*")
            || newTempVariables.variableName.Contains("/"))
            return;

        bool isFound = false;
        string sameAka = null;
        for (int j = 0; j < tempVariables.Count; j++)
        {
            if (tempVariables[j].variableName == newTempVariables.variableName)
            {
                if (tempVariables[j].txtDefault != txt)
                {
                    isFound = false;
                    sameAka = tempVariables[j].aka;
                }
                else
                    isFound = true;
                break;
            }
        }
        if (!isFound)
        {
            newTempVariables.aka = "ค่าที่ " + (tempVariables.Count + 1);
            if (!string.IsNullOrEmpty(sameAka))
                newTempVariables.aka = sameAka;
            newTempVariables.txtDefault = txt;
            if (newTempVariables.value.Contains("getiteminfo(getequipid("))
                newTempVariables.value = ConvertOneLineIfElse(newTempVariables.value);
            Log("New temporary variables added"
                + " | variableName: " + newTempVariables.variableName
                + " | value: " + newTempVariables.value
                + " | aka: " + newTempVariables.aka
                + " | txtDefault: " + newTempVariables.txtDefault, false, "#FE93C7");
            tempVariables.Add(newTempVariables);
        }
    }

    /// <summary>
    /// Cut function name out of string
    /// </summary>
    /// <param name="toCut"></param>
    /// <param name="functionName"></param>
    /// <returns></returns>
    string CutFunctionName(string toCut, string functionName, int lengthDecrease = 0)
    {
        Log("<color=orange>CutFunctionName >> #1: " + toCut + " | functionName: " + functionName + "</color>");

        int cutStartAt = toCut.IndexOf(functionName);

        string cut = toCut.Substring(cutStartAt + functionName.Length - lengthDecrease);

        Log("CutFunctionName >> #2: " + cut);

        if (cut.Contains(";"))
        {
            int cutEndAt = cut.IndexOf(";");

            cut = cut.Substring(1, cutEndAt - 1);
        }

        Log("CutFunctionName >> #3: " + cut);

        return cut;
    }

    /// <summary>
    /// Return all parameter of input parameter
    /// </summary>
    /// <param name="sumCut"></param>
    /// <returns></returns>
    List<string> GetAllParamerters(string sumCut, bool isBonusScriptCall = false)
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

        if (string.IsNullOrEmpty(sumCut))
        {
            allParam.Add(sumCut);
            return allParam;
        }
        else
            allParam = StringSplit.GetStringSplit(sumCut, ',');

        for (int i = 0; i < allParam.Count; i++)
        {
        L_Redo:
            if (isBonusScriptCall && !allParam[i].Contains(","))
                allParam[i] = "," + allParam[i];

            //Check ()
            int count1 = 0;
            foreach (char c in allParam[i])
            {
                if (c == '(')
                    count1++;
            }

            int count2 = 0;
            foreach (char c in allParam[i])
            {
                if (c == ')')
                    count2++;
            }

            if (count1 != count2 && count1 > 0)
            {
                allParam[i] += "," + allParam[i + 1];
                allParam.RemoveAt(i + 1);
                goto L_Redo;
            }
        }

        for (int i = 0; i < allParam.Count; i++)
            Log("allParam[" + i + "]:\n" + allParam[i], true, "#93FEE4");

        return allParam;
    }

    /// <summary>
    /// Return value of given parameters
    /// </summary>
    /// <param name="data"></param>
    /// <param name="paramCount"></param>
    /// <returns></returns>
    string GetValue(string data, int paramCount = -1, bool isZeroValueOkay = false, bool isForceNoCircle = false)
    {
        bool isForceNegative = false;
        string functionName = "GetValue";
        data = MergeWhiteSpace.RemoveWhiteSpace(data);

        Log(functionName + " >> [" + (paramCount - 1) + "]: " + data);

        //Use store temporary variables if found in this value
        bool isFoundTempVariable = false;
        List<string> tempVarName = new List<string>();
        List<string> valueFromTempVar = new List<string>();
        List<string> akaFromTempVar = new List<string>();
        for (int i = 0; i < tempVariables.Count; i++)
        {
            if (isForceNoCircle)
            {
                string targetVarName = MergeWhiteSpace.RemoveWhiteSpace(tempVariables[i].aka);
                if (data.Contains(targetVarName))
                {
                    tempVariables[i].isOneLineIfElse = IsOneLineIfElse(data);
                    //Log(functionName + " >> Set  tempVariables[" + i + "]: " + tempVariables[i].variableName + " isOneLineIfElse: " + tempVariables[i].isOneLineIfElse);
                }
            }
            else
            {
                if (data.Contains(tempVariables[i].variableName)
                    && !string.IsNullOrEmpty(tempVariables[i].variableName)
                    && !string.IsNullOrWhiteSpace(tempVariables[i].variableName))
                {
                    isFoundTempVariable = true;

                    tempVarName.Add(tempVariables[i].variableName);
                    Log(functionName + " >> Found variableName: " + tempVariables[i].variableName);

                    if (tempVariables[i].isOneLineIfElse)
                        valueFromTempVar.Add(tempVariables[i].aka);
                    else
                        valueFromTempVar.Add(tempVariables[i].value);

                    akaFromTempVar.Add(" ~ [" + tempVariables[i].aka + "]");
                    Log(functionName + " >> Found value: " + valueFromTempVar[valueFromTempVar.Count - 1]);
                }
            }
        }

        //Log("GetValue >> Finish find temporary variables");

        if (data == "INFINITE_TICK")
        {
            SetParamCheck(paramCount, true, false);
            return "ตลอดเวลา";
        }

        bool isHadFunction = false;

        if (data.Contains("("))
            isHadFunction = true;

        if (!data.Contains("(") && data.Contains(")"))
            data = data.Replace(")", "");

        if (isHadFunction)
        {
            bool isMessy = false;
            int count1 = 0;
            foreach (char c in data)
            {
                if (c == '(')
                {
                    count1++;
                    if (count1 >= 4)
                    {
                        isMessy = true;
                        break;
                    }
                }
            }

            //Do messy calculation here
            if (isMessy)
            {
                Log("isMessy >> data #1: " + data);
                data = ReplaceAllSpecialValue(data);
                Log("isMessy >> data #2: " + data);
                data = data.Replace(",", "");
                Log("isMessy >> data #3: " + data);
                data = "(" + data + ")";
                SetParamCheck(paramCount, true, false);
            }
            else
            {
                if (data.Contains("pow"))
                {
                    string subFunctionName = "pow";

                    Log(subFunctionName + ": " + data);

                    string newValue = data;
                    newValue = ReplaceAllSpecialValue(newValue);
                    Log(subFunctionName + " >> newValue: " + newValue);
                    while (newValue.Contains(subFunctionName))
                    {
                        int circleStartAt = 0;
                        int circleEndAt = 0;
                        string findFunc = null;
                        for (int i = 0; i < newValue.Length; i++)
                        {
                            if (newValue[i] == 'p' || newValue[i] == 'o' || newValue[i] == 'w')
                            {
                                findFunc += newValue[i];

                                Log(subFunctionName + " >> findFunc: " + findFunc);

                                if (findFunc == subFunctionName)
                                {
                                    circleStartAt = newValue.IndexOf("(", i - 2);
                                    circleEndAt = newValue.IndexOf(")", i - 2);
                                    int countCircle = 0;
                                    for (int j = 0; j < newValue.Length; j++)
                                    {
                                        if (newValue[j] == '(')
                                            countCircle++;
                                    }
                                    if (countCircle > 1)
                                    {
                                        while (countCircle > 1)
                                        {
                                            circleEndAt = newValue.IndexOf(")", circleEndAt + 2);
                                            countCircle--;
                                        }
                                    }
                                    break;
                                }
                            }
                            else
                                findFunc = null;
                        }

                        string sumToCut = newValue.Substring(circleStartAt + 1);
                        Log(subFunctionName + " >> sumToCut#1: " + sumToCut);
                        sumToCut = CheckSpecialValue(sumToCut);
                        sumToCut = ReplaceAllSpecialValue(sumToCut);
                        Log(subFunctionName + " >> sumToCut#2: " + sumToCut);
                        //int sumCircleStartAt = sumToCut.IndexOf("(");
                        //sumToCut = sumToCut.Substring(sumCircleStartAt + 1);
                        Log(subFunctionName + " >> sumToCut#3: " + sumToCut);
                        int sumCircleEndAt = sumToCut.IndexOf(")");
                        sumToCut = sumToCut.Substring(0, sumCircleEndAt);
                        Log(subFunctionName + " >> sumToCut#4: " + sumToCut);

                        //Calculate here
                        var allValue = StringSplit.GetStringSplit(sumToCut, ',');
                        for (int i = 0; i < allValue.Count; i++)
                        {
                            Log(subFunctionName + " >>  allValue[" + i + "]: " + allValue[i]);
                            if (i > 0)
                                allValue[i] = "," + allValue[i];
                            Log(subFunctionName + " >>  allValue[" + i + "]: " + allValue[i]);
                        }
                        Log(subFunctionName + " >>  start CheckSpecialValue");
                        allValue = CheckSpecialValue(allValue);
                        Log(subFunctionName + " >>  finish CheckSpecialValue");

                        Log(subFunctionName + " >>  start CheckMath");
                        for (int i = 0; i < allValue.Count; i++)
                            allValue[i] = CheckMath(allValue[i]);
                        Log(subFunctionName + " >>  finish CheckMath");

                        for (int i = 0; i < allValue.Count; i++)
                            allValue[i] = allValue[i].Replace(",", "");

                        SetParamCheck(paramCount, true, false);

                        Log(subFunctionName + " >> sumToCut: " + sumToCut);

                        string removeText = newValue.Substring(0, circleStartAt);
                        removeText = removeText.Replace(subFunctionName, "");

                        newValue = removeText + "[" + allValue[0] + " ยกกำลัง " + allValue[1] + "]" + newValue.Substring(circleEndAt + 1);

                        Log(subFunctionName + " >> newValue: " + newValue);
                    }
                    data = newValue;
                }
                else if (data.Contains("rand"))
                {
                    string subFunctionName = "rand";

                    Log(subFunctionName + ": " + data);

                    string newValue = data;
                    newValue = ReplaceAllSpecialValue(newValue);
                    Log(subFunctionName + " >> newValue: " + newValue);
                    while (newValue.Contains(subFunctionName))
                    {
                        int circleStartAt = 0;
                        int circleEndAt = 0;
                        string findFunc = null;
                        for (int i = 0; i < newValue.Length; i++)
                        {
                            if (newValue[i] == 'r' || newValue[i] == 'a' || newValue[i] == 'n' || newValue[i] == 'd')
                            {
                                findFunc += newValue[i];

                                Log(subFunctionName + " >> findFunc: " + findFunc);

                                if (findFunc == subFunctionName)
                                {
                                    circleStartAt = newValue.IndexOf("(", i - 3);
                                    circleEndAt = newValue.IndexOf(")", i - 3);
                                    int countCircle = 0;
                                    for (int j = 0; j < newValue.Length; j++)
                                    {
                                        if (newValue[j] == '(')
                                            countCircle++;
                                    }
                                    if (countCircle > 1)
                                    {
                                        while (countCircle > 1)
                                        {
                                            circleEndAt = newValue.IndexOf(")", circleEndAt + 3);
                                            countCircle--;
                                        }
                                    }
                                    break;
                                }
                            }
                            else
                                findFunc = null;
                        }

                        string sumToCut = newValue.Substring(circleStartAt + 1);
                        Log(subFunctionName + " >> sumToCut#1: " + sumToCut);
                        sumToCut = CheckSpecialValue(sumToCut);
                        sumToCut = ReplaceAllSpecialValue(sumToCut);
                        Log(subFunctionName + " >> sumToCut#2: " + sumToCut);
                        //int sumCircleStartAt = sumToCut.IndexOf("(");
                        //sumToCut = sumToCut.Substring(sumCircleStartAt + 1);
                        Log(subFunctionName + " >> sumToCut#3: " + sumToCut);
                        int sumCircleEndAt = sumToCut.IndexOf(")");
                        sumToCut = sumToCut.Substring(0, sumCircleEndAt);
                        Log(subFunctionName + " >> sumToCut#4: " + sumToCut);

                        //Calculate here
                        var allValue = StringSplit.GetStringSplit(sumToCut, ',');
                        for (int i = 0; i < allValue.Count; i++)
                        {
                            Log(subFunctionName + " >>  allValue[" + i + "]: " + allValue[i]);
                            if (i > 0)
                                allValue[i] = "," + allValue[i];
                            Log(subFunctionName + " >>  allValue[" + i + "]: " + allValue[i]);
                        }
                        Log(subFunctionName + " >>  start CheckSpecialValue");
                        allValue = CheckSpecialValue(allValue);
                        Log(subFunctionName + " >>  finish CheckSpecialValue");

                        Log(subFunctionName + " >>  start CheckMath");
                        for (int i = 0; i < allValue.Count; i++)
                            allValue[i] = CheckMath(allValue[i]);
                        Log(subFunctionName + " >>  finish CheckMath");

                        for (int i = 0; i < allValue.Count; i++)
                            allValue[i] = allValue[i].Replace(",", "");

                        SetParamCheck(paramCount, true, false);

                        Log(subFunctionName + " >> sumToCut: " + sumToCut);

                        string removeText = newValue.Substring(0, circleStartAt);
                        removeText = removeText.Replace(subFunctionName, "");

                        newValue = removeText + "[" + allValue[0] + "~" + allValue[1] + "]" + newValue.Substring(circleEndAt + 1);

                        Log(subFunctionName + " >> newValue: " + newValue);
                    }
                    data = newValue;
                }
                else if (data.Contains("min"))
                {
                    string subFunctionName = "min";

                    Log(subFunctionName + ": " + data);

                    string newValue = data;
                    newValue = ReplaceAllSpecialValue(newValue);
                    Log(subFunctionName + " >> newValue: " + newValue);
                    while (newValue.Contains(subFunctionName))
                    {
                        int circleStartAt = 0;
                        int circleEndAt = 0;
                        string findFunc = null;
                        for (int i = 0; i < newValue.Length; i++)
                        {
                            if (newValue[i] == 'm' || newValue[i] == 'i' || newValue[i] == 'n')
                            {
                                findFunc += newValue[i];

                                Log(subFunctionName + " >> findFunc: " + findFunc);

                                if (findFunc == subFunctionName)
                                {
                                    circleStartAt = newValue.IndexOf("(", i - 2);
                                    circleEndAt = newValue.IndexOf(")", i - 2);
                                    int countCircle = 0;
                                    for (int j = 0; j < newValue.Length; j++)
                                    {
                                        if (newValue[j] == '(')
                                            countCircle++;
                                    }
                                    if (countCircle > 1)
                                    {
                                        while (countCircle > 1)
                                        {
                                            circleEndAt = newValue.IndexOf(")", circleEndAt + 2);
                                            countCircle--;
                                        }
                                    }
                                    break;
                                }
                            }
                            else
                                findFunc = null;
                        }


                        string sumToCut = newValue.Substring(circleStartAt + 1);
                        Log(subFunctionName + " >> sumToCut#1: " + sumToCut);
                        sumToCut = CheckSpecialValue(sumToCut);
                        sumToCut = ReplaceAllSpecialValue(sumToCut);
                        Log(subFunctionName + " >> sumToCut#2: " + sumToCut);
                        //int sumCircleStartAt = sumToCut.IndexOf("(");
                        //sumToCut = sumToCut.Substring(sumCircleStartAt + 1);
                        Log(subFunctionName + " >> sumToCut#3: " + sumToCut);
                        int sumCircleEndAt = sumToCut.IndexOf(")");
                        sumToCut = sumToCut.Substring(0, sumCircleEndAt);
                        Log(subFunctionName + " >> sumToCut#4: " + sumToCut);

                        //Calculate here
                        var allValue = StringSplit.GetStringSplit(sumToCut, ',');
                        for (int i = 0; i < allValue.Count; i++)
                        {
                            Log(subFunctionName + " >>  allValue[" + i + "]: " + allValue[i]);
                            if (i > 0)
                                allValue[i] = "," + allValue[i];
                            Log(subFunctionName + " >>  allValue[" + i + "]: " + allValue[i]);
                        }
                        Log(subFunctionName + " >>  start CheckSpecialValue");
                        allValue = CheckSpecialValue(allValue);
                        Log(subFunctionName + " >>  finish CheckSpecialValue");

                        Log(subFunctionName + " >>  start CheckMath");
                        for (int i = 0; i < allValue.Count; i++)
                            allValue[i] = CheckMath(allValue[i]);
                        Log(subFunctionName + " >>  finish CheckMath");

                        for (int i = 0; i < allValue.Count; i++)
                            allValue[i] = allValue[i].Replace(",", "");

                        bool isHadNonInteger = false;
                        string nonIntegerText = null;

                        int min = 0;
                        for (int i = 0; i < allValue.Count; i++)
                        {
                            int paramInt = 0;
                            bool isInteger = false;

                            isInteger = int.TryParse(allValue[i], out paramInt);

                            if (isInteger)
                            {
                                if (min == 0 || min > paramInt)
                                    min = paramInt;
                            }
                            else
                            {
                                isHadNonInteger = true;

                                //Replace temporary variables
                                if (isFoundTempVariable)
                                {
                                    for (int j = 0; j < tempVarName.Count; j++)
                                    {
                                        if (isUseAkaTempVar)
                                            allValue[i] = allValue[i].Replace(tempVarName[j], valueFromTempVar[j] + akaFromTempVar[j]);
                                        else
                                            allValue[i] = allValue[i].Replace(tempVarName[j], valueFromTempVar[j]);
                                    }

                                    SetParamCheck(paramCount, true, false);
                                }

                                allValue[i] = ReplaceAllSpecialValue(allValue[i]);

                                nonIntegerText = allValue[i];
                            }
                        }

                        SetParamCheck(paramCount, true, false);

                        Log(subFunctionName + " >> sumToCut: " + sumToCut);

                        string removeText = newValue.Substring(0, circleStartAt);
                        removeText = removeText.Replace(subFunctionName, "");
                        if (isHadNonInteger)
                            newValue = removeText + "[" + nonIntegerText + " มากสุด " + min + "]" + newValue.Substring(circleEndAt + 1);
                        else
                            newValue = removeText + "[มากสุด " + min + "]" + newValue.Substring(circleEndAt + 1);

                        Log(subFunctionName + " >> newValue: " + newValue);
                    }
                    data = newValue;
                }
                else if (data.Contains("max"))
                {
                    string subFunctionName = "max";

                    Log(subFunctionName + ": " + data);

                    string newValue = data;
                    newValue = ReplaceAllSpecialValue(newValue);
                    Log(subFunctionName + " >> newValue: " + newValue);
                    while (newValue.Contains(subFunctionName))
                    {
                        int circleStartAt = 0;
                        int circleEndAt = 0;
                        string findFunc = null;
                        for (int i = 0; i < newValue.Length; i++)
                        {
                            if (newValue[i] == 'm' || newValue[i] == 'a' || newValue[i] == 'x')
                            {
                                findFunc += newValue[i];

                                Log(subFunctionName + " >> findFunc: " + findFunc);

                                if (findFunc == subFunctionName)
                                {
                                    circleStartAt = newValue.IndexOf("(", i - 2);
                                    circleEndAt = newValue.IndexOf(")", i - 2);
                                    int countCircle = 0;
                                    for (int j = 0; j < newValue.Length; j++)
                                    {
                                        if (newValue[j] == '(')
                                            countCircle++;
                                    }
                                    if (countCircle > 1)
                                    {
                                        while (countCircle > 1)
                                        {
                                            circleEndAt = newValue.IndexOf(")", circleEndAt + 2);
                                            countCircle--;
                                        }
                                    }
                                    break;
                                }
                            }
                            else
                                findFunc = null;
                        }

                        string sumToCut = newValue.Substring(circleStartAt + 1);
                        Log(subFunctionName + " >> sumToCut#1: " + sumToCut);
                        sumToCut = CheckSpecialValue(sumToCut);
                        sumToCut = ReplaceAllSpecialValue(sumToCut);
                        Log(subFunctionName + " >> sumToCut#2: " + sumToCut);
                        //int sumCircleStartAt = sumToCut.IndexOf("(");
                        //sumToCut = sumToCut.Substring(sumCircleStartAt + 1);
                        Log(subFunctionName + " >> sumToCut#3: " + sumToCut);
                        int sumCircleEndAt = sumToCut.IndexOf(")");
                        sumToCut = sumToCut.Substring(0, sumCircleEndAt);
                        Log(subFunctionName + " >> sumToCut#4: " + sumToCut);

                        //Calculate here
                        var allValue = StringSplit.GetStringSplit(sumToCut, ',');
                        for (int i = 0; i < allValue.Count; i++)
                        {
                            Log(subFunctionName + " >>  allValue[" + i + "]: " + allValue[i]);
                            if (i > 0)
                                allValue[i] = "," + allValue[i];
                            Log(subFunctionName + " >>  allValue[" + i + "]: " + allValue[i]);
                        }
                        Log(subFunctionName + " >>  start CheckSpecialValue");
                        allValue = CheckSpecialValue(allValue);
                        Log(subFunctionName + " >>  finish CheckSpecialValue");

                        Log(subFunctionName + " >>  start CheckMath");
                        for (int i = 0; i < allValue.Count; i++)
                            allValue[i] = CheckMath(allValue[i]);
                        Log(subFunctionName + " >>  finish CheckMath");

                        for (int i = 0; i < allValue.Count; i++)
                            allValue[i] = allValue[i].Replace(",", "");

                        bool isHadNonInteger = false;
                        string nonIntegerText = null;

                        int max = 0;
                        for (int i = 0; i < allValue.Count; i++)
                        {
                            //Log("max >>  allValue[" + i + "]: " + allValue[i]);
                            int paramInt = 0;
                            bool isInteger = false;

                            isInteger = int.TryParse(allValue[i], out paramInt);

                            if (isInteger)
                            {
                                if (max == 0 || max < paramInt)
                                    max = paramInt;
                            }
                            else
                            {
                                Log(subFunctionName + " >>  allValue[" + i + "] is not integer #1");

                                isHadNonInteger = true;

                                //Replace temporary variables
                                if (isFoundTempVariable)
                                {
                                    for (int j = 0; j < tempVarName.Count; j++)
                                    {
                                        if (isUseAkaTempVar)
                                            allValue[i] = allValue[i].Replace(tempVarName[j], valueFromTempVar[j] + akaFromTempVar[j]);
                                        else
                                            allValue[i] = allValue[i].Replace(tempVarName[j], valueFromTempVar[j]);
                                    }

                                    SetParamCheck(paramCount, true, false);
                                }

                                Log(subFunctionName + " >>  allValue[" + i + "] is not integer #2");

                                allValue[i] = ReplaceAllSpecialValue(allValue[i]);

                                Log(subFunctionName + " >>  allValue[" + i + "] is not integer #3");

                                nonIntegerText = allValue[i];
                            }
                        }

                        SetParamCheck(paramCount, true, false);

                        Log(subFunctionName + " >> sumToCut: " + sumToCut);

                        string removeText = newValue.Substring(0, circleStartAt);
                        removeText = removeText.Replace(subFunctionName, "");
                        if (isHadNonInteger)
                            newValue = removeText + "[" + max + " มากสุด " + nonIntegerText + "]" + newValue.Substring(circleEndAt + 1);
                        else
                            newValue = removeText + "[มากสุด " + max + "]" + newValue.Substring(circleEndAt + 1);

                        Log(subFunctionName + " >> newValue: " + newValue);
                    }
                    data = newValue;
                }
                else
                {
                    Log("not had special function >> data #1: " + data);
                    data = ReplaceAllSpecialValue(data);
                    Log("not had special function >> data #2: " + data);
                    data = data.Replace(",", "");
                    Log("not had special function >> data #3: " + data);
                    data = "[" + data + "]";
                    SetParamCheck(paramCount, true, false);
                }
            }
        }
        //Normal value
        else
        {
            //++
            if (data == "+")
            {
                SetParamCheck(paramCount, true, false);
                return "1";
            }
            //--
            else if (data == "-")
            {
                SetParamCheck(paramCount, true, true);
                return "1";
            }

            Log(functionName + " >> data #2: " + data);

            data = CheckMath(data);

            Log(functionName + " >> data #3: " + data);

            //Integer check
            int paramInt = 0;
            bool isInteger = false;

            isInteger = int.TryParse(data, out paramInt);

            Log("isInteger: " + isInteger + ", paramInt: " + paramInt);

            if (isInteger)
            {
                //Zero integer
                if (paramInt == 0 && !isZeroValueOkay)
                {
                    data = "0";
                    SetParamCheck(paramCount, false, false);
                }
                //Negative integer
                else if (paramInt < 0)
                {
                    paramInt = paramInt * -1;
                    data = paramInt.ToString("f0");
                    SetParamCheck(paramCount, true, true);
                }
                //Positive integer
                else
                {
                    data = paramInt.ToString("f0");
                    SetParamCheck(paramCount, true, false);
                }
            }
            else
            {
                SetParamCheck(paramCount, true, false);
                bool isNotRunNegativeCheck = false;
                for (int i = 0; i < tempVariables.Count; i++)
                {
                    if (data.Contains(MergeWhiteSpace.RemoveWhiteSpace(tempVariables[i].aka)))
                    {
                        isNotRunNegativeCheck = true;
                        break;
                    }
                }
                if (!isNotRunNegativeCheck)
                {
                    if (data.Contains("0") || data.Contains("1") || data.Contains("2") || data.Contains("3") || data.Contains("4")
                    || data.Contains("5") || data.Contains("6") || data.Contains("7") || data.Contains("8") || data.Contains("9"))
                    {
                        for (int i = data.Length - 1; i > 0; i--)
                        {
                            if (data[i] == '-' && i - 1 >= 0)
                            {
                                var sumChar = data[i - 1];
                                if (sumChar != '0' || sumChar != '1' || sumChar != '2' || sumChar != '3' || sumChar != '4'
                                    || sumChar != '5' || sumChar != '6' || sumChar != '7' || sumChar != '8' || sumChar != '9' || sumChar == '(')
                                {
                                    Log(functionName + " >> Found negative: " + data);
                                    data = data.Remove(i, 1);
                                    Log(functionName + " >> Found negative: " + data);
                                    SetParamCheck(paramCount, true, true);
                                    isForceNegative = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        //Replace temporary variables
        if (isFoundTempVariable)
        {
            Log(functionName + " >> isFoundTempVariable");

            List<string> tempValues = new List<string>();
            List<int> tempIntegers = new List<int>();
            string highLowValue = null;
            string minValue = null;
            string maxValue = null;

            for (int i = 0; i < tempVarName.Count; i++)
            {
                if (data.Contains(tempVarName[i]))
                {
                    for (int j = 0; j < tempVariables.Count; j++)
                    {
                        //Log(functionName + " >> tempVarName[i]: " + tempVarName[i]);
                        //Log(functionName + " >> tempVariables[j].variableName: " + tempVariables[j].variableName);
                        //Log(functionName + " >> tempVariables[j].value: " + tempVariables[j].value);
                        //Log(functionName + " >> data: " + data);
                        //Log(functionName + " >> tempValues.Contains(tempVariables[j].value): " + tempValues.Contains(tempVariables[j].value));
                        if (!tempValues.Contains(tempVariables[j].value) && tempVariables[j].variableName == tempVarName[i])
                        {
                            if (tempVariables[j].isOneLineIfElse)
                                tempValues.Add(tempVariables[j].aka);
                            else
                                tempValues.Add(tempVariables[j].value);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < tempValues.Count; i++)
            {
                tempValues[i] = tempValues[i].Replace(";", "");
                //Log(functionName + " >>  tempValues[i]: " + tempValues[i]);
            }

            //Log(functionName + " >> tempVarName.Count: " + tempVarName.Count);

            //Normal replace
            if (tempVarName.Count <= 1)
            {
                //Log(functionName + " >> tempVarName.Count: " + tempVarName.Count);
                //Log(functionName + " >> tempValues.Count: " + tempValues.Count);
                //Log(functionName + " >> akaFromTempVar.Count: " + akaFromTempVar.Count);

                for (int i = 0; i < tempVarName.Count; i++)
                {
                    if (tempValues.Count > 0)
                    {
                        Log(akaFromTempVar[i]);
                        if (isUseAkaTempVar)
                            data = data.Replace(tempVarName[i], tempValues[i] + akaFromTempVar[i]);
                        else
                            data = data.Replace(tempVarName[i], tempValues[i]);
                    }
                }
            }
            //Range replace (ตามจำนวนตีบวก ~ Min{Max} Val)
            else
            {
                for (int i = 0; i < tempValues.Count; i++)
                {
                    //Integer check
                    int paramInt = 0;
                    bool isInteger = false;

                    isInteger = int.TryParse(tempValues[i], out paramInt);

                    Log("isInteger: " + isInteger + ", paramInt: " + paramInt);

                    if (isInteger)
                        tempIntegers.Add(paramInt);
                    else if (string.IsNullOrEmpty(minValue))
                        minValue = tempValues[i];
                    else if (string.IsNullOrEmpty(maxValue))
                        maxValue = tempValues[i];
                }

                //Log(">>>>>>>>>>>>>>>>>>>>>>> minValue: " + minValue);
                //Log(">>>>>>>>>>>>>>>>>>>>>>> maxValue: " + maxValue);
                //Log(">>>>>>>>>>>>>>>>>>>>>>> tempIntegers.Count: " + tempIntegers.Count);

                int minVal = -9999;
                int maxVal = -9999;
                if (tempIntegers.Count > 1)
                {
                    for (int j = 0; j < tempIntegers.Count; j++)
                    {
                        if (minVal == -9999)
                            minVal = tempIntegers[j];
                        else if (maxVal == -9999)
                            maxVal = tempIntegers[j];
                        else
                        {
                            if (tempIntegers[j] > maxVal)
                                maxVal = tempIntegers[j];
                        }
                        if (minVal > maxVal)
                        {
                            var tempMin = minVal;
                            var tempMax = maxVal;
                            minVal = tempMax;
                            maxVal = tempMin;
                        }
                    }
                }

                //Log(">>>>>>>>>>>>>>>>>>>>>>> minValue: " + minValue);
                //Log(">>>>>>>>>>>>>>>>>>>>>>> maxValue: " + maxValue);
                //Log(">>>>>>>>>>>>>>>>>>>>>>> tempIntegers.Count: " + tempIntegers.Count);
                //Log(">>>>>>>>>>>>>>>>>>>>>>> minVal: " + minVal);
                //Log(">>>>>>>>>>>>>>>>>>>>>>> maxVal: " + maxVal);

                if (minVal != -9999 && maxVal != -9999)
                    highLowValue = minVal + "~" + maxVal;
                else if (minVal != -9999 && maxVal == -9999)
                    highLowValue = minValue + "~" + minVal.ToString("f0");
                else
                    highLowValue = minValue + "~" + maxValue;

                for (int i = 0; i < tempValues.Count; i++)
                    data = data.Replace(tempVarName[i], highLowValue);
            }

            if (isForceNegative)
                SetParamCheck(paramCount, true, true);
            else
                SetParamCheck(paramCount, true, false);
        }

        Log(functionName + " >> data: " + data + " >> Replace special variables");

        data = ReplaceAllSpecialValue(data);

        Log(functionName + " >> Replace any error text");

        data = data.Replace(";", "");
        data = data.Replace("()", "");
        data = data.Replace("[ ", "");

        Log("<color=yellow>" + functionName + " >> Final: " + data + "</color>");

        if (isForceNoCircle)
        {
            data = data.Replace("[", "");
            data = data.Replace("]", "");
            data = data.Replace("(", "");
            data = data.Replace(")", "");
            return data;
        }
        else if (isFoundTempVariable)
            return "[" + data + "]";
        else
            return data;
    }

    /// <summary>
    /// Continue deep check inside value
    /// </summary>
    /// <param name="allValue"></param>
    /// <returns></returns>
    List<string> CheckSpecialValue(List<string> allValue)
    {
        //Check special function inside value
        for (int i = 0; i < allValue.Count; i++)
        {
            Log("allValue[" + i + "]: " + allValue[i]);
            if (allValue[i].Contains("min") || allValue[i].Contains("max") || allValue[i].Contains("rand") || allValue[i].Contains("pow"))
            {
            L_Redo:
                var sumValue = allValue[i];

                int count1 = 0;
                foreach (char c in sumValue)
                {
                    if (c == '(')
                        count1++;
                }

                int count2 = 0;
                foreach (char c in sumValue)
                {
                    if (c == ')')
                        count2++;
                }

                if (count1 != count2)
                {
                    allValue[i] += allValue[i + 1];
                    allValue.RemoveAt(i + 1);
                    goto L_Redo;
                }

                int endOfFunctionAt = 0;
                for (int j = 0; j < sumValue.Length; j++)
                {
                    if (sumValue[j] == ')')
                    {
                        endOfFunctionAt++;
                        if (endOfFunctionAt == count2)
                        {
                            endOfFunctionAt = j;
                            break;
                        }
                    }
                }

                //Substring and save last string
                var sumCut = sumValue.Substring(0, endOfFunctionAt + 1);
                Log("CheckSpecialValue >> sumCut: " + sumCut);

                sumCut = GetValue(sumCut);
                Log("CheckSpecialValue >> sumCut: " + sumCut);

                allValue[i] = sumCut + allValue[i].Substring(endOfFunctionAt);
                Log("CheckSpecialValue >> allValue[" + i + "]: " + allValue[i]);
            }
            else
                Log("Not found special value in index: " + i);
        }

        return allValue;
    }
    string CheckSpecialValue(string data)
    {
        string functionName = "CheckSpecialValue";
        Log(functionName + " >> data: " + data);

        //Check special function inside value
        if (data.Contains("min") || data.Contains("max") || data.Contains("rand") || data.Contains("pow"))
        {
            Log(functionName + " >> data: " + data);
            data = GetValue(data);
            Log(functionName + " >>  data: " + data);
        }
        else
            Log(functionName + " >> Not found special value");

        return data;
    }

    /// <summary>
    /// Simple Math Calculation
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string CheckMath(string data)
    {
        string functionName = "CheckMath";

        Log(functionName + ": " + data);

        string sumData = data;
        sumData = sumData.Replace(",", "");

        List<MathCalculation> mathCalculations = new List<MathCalculation>();
        MathCalculation mathCalculation = new MathCalculation();

        bool isLastCharIsOperator = false;
        for (int i = 0; i < sumData.Length; i++)
        {
            if (isLastCharIsOperator)
            {
                if (string.IsNullOrEmpty(mathCalculation.a))
                    mathCalculation.a = mathCalculation.param;
                else if (string.IsNullOrEmpty(mathCalculation.b))
                    mathCalculation.b = mathCalculation.param;
                if (!string.IsNullOrEmpty(mathCalculation.a) && !string.IsNullOrEmpty(mathCalculation.b))
                {
                    mathCalculations.Add(mathCalculation);
                    mathCalculation = new MathCalculation();
                }

                mathCalculation.param = null;
            }

            isLastCharIsOperator = false;

            var sum = sumData[i];

            bool isOperator = false;
            if (sum == '+' || sum == '-' || sum == '*' || sum == '/')
                isOperator = true;

            //Log(functionName + " >> first phase >> sum: " + sum);

            if (isOperator)
            {
                if (string.IsNullOrEmpty(mathCalculation.param))
                    mathCalculation.AddParam(sum.ToString());
                else
                {
                    mathCalculation.SetOperator(sum.ToString());
                    isLastCharIsOperator = true;
                }
            }
            else
                mathCalculation.AddParam(sum.ToString());
        }

        if (string.IsNullOrEmpty(mathCalculation.a))
            mathCalculation.a = mathCalculation.param;
        else if (string.IsNullOrEmpty(mathCalculation.b))
            mathCalculation.b = mathCalculation.param;
        if (!string.IsNullOrEmpty(mathCalculation.a) && !string.IsNullOrEmpty(mathCalculation.b))
        {
            mathCalculations.Add(mathCalculation);
            mathCalculation = new MathCalculation();
        }

        mathCalculation.param = null;

        Log(functionName + " >> second phase >> mathCalculations.Count: " + mathCalculations.Count);

        string toReturn = null;
        for (int i = 0; i < mathCalculations.Count; i++)
        {
            Log(functionName + " >> second phase >> mathCalculations[" + i + "].a: " + mathCalculations[i].a);
            Log(functionName + " >> second phase >> mathCalculations[" + i + "].b: " + mathCalculations[i].b);
            Log(functionName + " >> second phase >> mathCalculations[" + i + "].operator: " + mathCalculations[i]._operator);

            mathCalculation = mathCalculations[i];

            if (mathCalculation._operator == "+")
            {
                string sumA = mathCalculation.a;
                int sumIntA = 0;
                bool isInegerA = IsStringInteger.Check(sumA);
                if (isInegerA)
                    sumIntA = int.Parse(sumA);

                string sumB = mathCalculation.b;
                int sumIntB = 0;
                bool isInegerB = IsStringInteger.Check(sumB);
                if (isInegerB)
                    sumIntB = int.Parse(sumB);

                if (isInegerA && isInegerB)
                    toReturn += ", " + (sumIntA + sumIntB).ToString("f0");
                else
                    toReturn += ", " + sumA + mathCalculation._operator + sumB;
            }
            else if (mathCalculation._operator == "-")
            {
                string sumA = mathCalculation.a;
                int sumIntA = 0;
                bool isInegerA = IsStringInteger.Check(sumA);
                if (isInegerA)
                    sumIntA = int.Parse(sumA);

                string sumB = mathCalculation.b;
                int sumIntB = 0;
                bool isInegerB = IsStringInteger.Check(sumB);
                if (isInegerB)
                    sumIntB = int.Parse(sumB);

                if (isInegerA && isInegerB)
                    toReturn += ", " + (sumIntA - sumIntB).ToString("f0");
                else
                    toReturn += ", " + sumA + mathCalculation._operator + sumB;
            }
            else if (mathCalculation._operator == "*")
            {
                string sumA = mathCalculation.a;
                int sumIntA = 0;
                bool isInegerA = IsStringInteger.Check(sumA);
                if (isInegerA)
                    sumIntA = int.Parse(sumA);

                string sumB = mathCalculation.b;
                int sumIntB = 0;
                bool isInegerB = IsStringInteger.Check(sumB);
                if (isInegerB)
                    sumIntB = int.Parse(sumB);

                if (isInegerA && isInegerB)
                    toReturn += ", " + (sumIntA * sumIntB).ToString("f0");
                else
                    toReturn += ", " + sumA + mathCalculation._operator + sumB;
            }
            else if (mathCalculation._operator == "/")
            {
                string sumA = mathCalculation.a;
                int sumIntA = 0;
                bool isInegerA = IsStringInteger.Check(sumA);
                if (isInegerA)
                    sumIntA = int.Parse(sumA);

                string sumB = mathCalculation.b;
                int sumIntB = 0;
                bool isInegerB = IsStringInteger.Check(sumB);
                if (isInegerB)
                    sumIntB = int.Parse(sumB);

                if (isInegerA && isInegerB)
                    toReturn += ", " + (sumIntA / sumIntB).ToString("f0");
                else
                    toReturn += ", " + sumA + mathCalculation._operator + sumB;
            }
        }

        if (mathCalculations.Count > 0)
            toReturn = toReturn.Substring(1);
        else
            toReturn = data;

        return toReturn;
    }

    #region Utilities
    /// <summary>
    /// Get item name by ID
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetItemName(string data)
    {
        for (int i = 0; i < m_output.m_currentItemWithoutScriptDbs.Count; i++)
        {
            var sum = m_output.m_currentItemWithoutScriptDbs[i];
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
            return null;

        WeaponTypeFlag weaponTypeFlag = (WeaponTypeFlag)Enum.Parse(typeof(WeaponTypeFlag), data, true);

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
            return null;
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
            return null;

        return null;
    }

    /// <summary>
    /// Milliseconds to seconds
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="digit"></param>
    /// <returns></returns>
    string TimerToStringTimer(float timer, float divider = 1000)
    {
        string functionName = "TimerToStringTimer";

        string sumDecimal = "f0";

        if (timer % divider != 0)
            sumDecimal = "f3";

        Log(functionName + ">> timer: " + timer);

        var sumTimer = (timer / divider);

        Log(functionName + ">> sumTimer: " + sumTimer);

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
    /// Use function that use int, float with string
    /// </summary>
    /// <param name="data"></param>
    /// <param name="functionType"></param>
    /// <returns></returns>
    string UseFunctionWithString(string data, int functionType, string additionalParam = null)
    {
        string functionName = "UseFunctionWithString";

        Log("<color=#7CFFC1>" + functionName + " >> data: " + data + ", functionType: " + functionType + ", additionalParam: " + additionalParam + "</color>");

        string sum = null;
        List<string> toReplace = new List<string>();
        List<string> toReplaceTo = new List<string>();

        if (functionType == 0)
        {
            if (data.Contains("0") || data.Contains("1") || data.Contains("2") || data.Contains("3") || data.Contains("4")
                    || data.Contains("5") || data.Contains("6") || data.Contains("7") || data.Contains("8") || data.Contains("9"))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    var sumChar = data[i];
                    if (sumChar == '0' || sumChar == '1' || sumChar == '2' || sumChar == '3' || sumChar == '4'
                        || sumChar == '5' || sumChar == '6' || sumChar == '7' || sumChar == '8' || sumChar == '9')
                    {
                        sum += sumChar.ToString();
                        Log("sum: " + sum);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(sum))
                        {
                            float toParse = float.Parse(sum);
                            toReplace.Add(sum);
                            toReplaceTo.Add(TimerToStringTimer(toParse));
                            Log(functionName + " >> Add: " + sum + ", " + TimerToStringTimer(toParse));
                            sum = null;
                        }
                    }
                }
                //Recheck again
                if (!string.IsNullOrEmpty(sum))
                {
                    float toParse = float.Parse(sum);
                    toReplace.Add(sum);
                    toReplaceTo.Add(TimerToStringTimer(toParse));
                    Log(functionName + " >> Add: " + sum + ", " + TimerToStringTimer(toParse));
                    sum = null;
                }
            }
        }

        Log(functionName + " >> data: " + data);

        for (int i = 0; i < toReplace.Count; i++)
            data = data.Replace(toReplace[i], toReplaceTo[i]);

        Log(functionName + " >> Final: " + data);

        return data;
    }

    /// <summary>
    /// Get sc_start flag type
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    string Get_sc_start_Flag(int flag)
    {
        ScStartFlag scStartFlag = (ScStartFlag)Enum.Parse(typeof(ScStartFlag), flag.ToString("f0"), true);
        if (scStartFlag == ScStartFlag.SCSTART_LOADED)
            return " (สถานะจะคงที่)";
        else if (scStartFlag == ScStartFlag.SCSTART_NOAVOID)
            return " (ไม่สามารถยับยั้งการเกิดสถานะนี้ได้)";
        else if (scStartFlag == ScStartFlag.SCSTART_NOICON)
            return " (สถานะจะไม่แสดงเป็น Icon)";
        else if (scStartFlag == ScStartFlag.SCSTART_NORATEDEF)
            return " (โอกาสจะคงที่)";
        else
            return null;
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
        Log("ReplaceAllSpecialValue >> data: " + data);

        string value = data;

        if (IsOneLineIfElse(value))
            value = ConvertOneLineIfElse(value);

        string functionName = "getskilllv";
        if (value.Contains(functionName))
        {
            string newValue = value;
            while (newValue.Contains(functionName))
            {
                int circleStartAt = 0;
                int circleEndAt = 0;
                string findFunc = null;
                for (int i = 0; i < newValue.Length; i++)
                {
                    if (newValue[i] == 'g' || newValue[i] == 'e' || newValue[i] == 't' ||
                        newValue[i] == 's' || newValue[i] == 'k' || newValue[i] == 'i' ||
                        newValue[i] == 'l' || newValue[i] == 'v')
                    {
                        findFunc += newValue[i];

                        Log("findFunc: " + findFunc);

                        if (findFunc == functionName)
                        {
                            circleStartAt = newValue.IndexOf("(", i - 9);
                            circleEndAt = newValue.IndexOf(")", i - 9);
                            break;
                        }
                    }
                    else
                        findFunc = null;
                }


                string sumToCut = newValue.Substring(circleStartAt + 1);
                Log("sumToCut: " + sumToCut);
                int sumCircleStartAt = sumToCut.IndexOf("(");
                sumToCut = sumToCut.Substring(sumCircleStartAt + 1);
                Log("sumToCut: " + sumToCut);
                int sumCircleEndAt = sumToCut.IndexOf(")");
                sumToCut = sumToCut.Substring(0, sumCircleEndAt);
                Log("sumToCut: " + sumToCut);
                sumToCut = GetSkillName(sumToCut);
                Log("sumToCut: " + sumToCut);

                string removeText = newValue.Substring(0, circleStartAt);
                removeText = removeText.Replace(functionName, "");

                newValue = removeText + "(ตามจำนวนที่เรียนรู้ Skill " + sumToCut + ")" + newValue.Substring(circleEndAt + 1);
                Log("newValue: " + newValue);
            }
            value = newValue;
        }

        value = value.Replace("getrefine();", "[ตามจำนวนตีบวก]");

        value = value.Replace("getrefine()", "[ตามจำนวนตีบวก]");

        value = value.Replace("getrefine", "[ตามจำนวนตีบวก]");

        value = value.Replace("readparambStr", "[ตาม STR ที่ฝึกฝน]");
        value = value.Replace("readparam(bStr)", "[ตาม STR ที่ฝึกฝน]");

        value = value.Replace("readparambAgi", "[ตาม AGI ที่ฝึกฝน]");
        value = value.Replace("readparam(bAgi)", "[ตาม AGI ที่ฝึกฝน]");

        value = value.Replace("readparambVit", "[ตาม VIT ที่ฝึกฝน]");
        value = value.Replace("readparam(bVit)", "[ตาม VIT ที่ฝึกฝน]");

        value = value.Replace("readparambInt", "[ตาม INT ที่ฝึกฝน]");
        value = value.Replace("readparam(bInt)", "[ตาม INT ที่ฝึกฝน]");

        value = value.Replace("readparambDex", "[ตาม DEX ที่ฝึกฝน]");
        value = value.Replace("readparam(bDex)", "[ตาม DEX ที่ฝึกฝน]");

        value = value.Replace("readparambLuk", "[ตาม LUK ที่ฝึกฝน]");
        value = value.Replace("readparam(bLuk)", "[ตาม LUK ที่ฝึกฝน]");

        value = value.Replace("BaseLevel", "[Level]");

        value = value.Replace("BaseJob", "ฐานอาชีพ");

        value = value.Replace("JobLevel", "[Job Level]");

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
        Log("ConvertOneLineIfElse >> data: " + data);

        data = data.Replace("getrefine", "หากจำนวนตีบวก");
        data = data.Replace("getrefine()", "หากจำนวนตีบวก");
        data = data.Replace("getiteminfo(getequipid(EQI_HAND_R),11)", "หากมือขวาสวมใส่");
        data = data.Replace("getiteminfo(getequipid(EQI_HAND_L),11)", "หากมือซ้ายสวมใส่");
        data = data.Replace("==W_FIST", " ไม่มีอาวุธ ");
        data = data.Replace("==W_DAGGER", " Dagger ");
        data = data.Replace("==W_1HSWORD", " One-handed swords ");
        data = data.Replace("==W_2HSWORD", " Two-handed swords ");
        data = data.Replace("==W_1HSPEAR", " One-handed spears ");
        data = data.Replace("==W_2HSPEAR", " Two-handed spears ");
        data = data.Replace("==W_1HAXE", " One-handed axes ");
        data = data.Replace("==W_2HAXE", " Two-handed axes ");
        data = data.Replace("==W_MACE", " Maces ");
        data = data.Replace("==W_2HMACE", " Two-handed Maces ");
        data = data.Replace("==W_STAFF", " Staff ");
        data = data.Replace("==W_BOW", " Bows ");
        data = data.Replace("==W_KNUCKLE", " Knuckles ");
        data = data.Replace("==W_MUSICAL", " Musical Instruments ");
        data = data.Replace("==W_WHIP", " Whips ");
        data = data.Replace("==W_BOOK", " Book ");
        data = data.Replace("==W_KATAR", " Katars ");
        data = data.Replace("==W_REVOLVER", " Revolvers ");
        data = data.Replace("==W_RIFLE", " Rifles ");
        data = data.Replace("==W_GATLING", " Gatling guns ");
        data = data.Replace("==W_SHOTGUN", " Shotguns ");
        data = data.Replace("==W_GRENADE", " Grenade launchers ");
        data = data.Replace("==W_HUUMA", " Fuuma Shurikens ");
        data = data.Replace("==W_2HSTAFF", " Two-handed Staff ");
        data = data.Replace("==MAX_WEAPON_TYPE", "");
        data = data.Replace("==W_DOUBLE_DD", " Dual-wield Daggers ");
        data = data.Replace("==W_DOUBLE_SS", " Dual-wield Swords ");
        data = data.Replace("==W_DOUBLE_AA", " Dual-wield Axes ");
        data = data.Replace("==W_DOUBLE_DS", " Dagger + Sword ");
        data = data.Replace("==W_DOUBLE_DA", " Dagger + Axe ");
        data = data.Replace("==W_DOUBLE_SA", " Sword + Axe ");
        data = data.Replace("==MAX_WEAPON_TYPE_ALL", "");
        data = data.Replace(">=", " มากกว่าหรือเท่ากับ ");
        data = data.Replace("<=", " น้อยกว่าหรือเท่ากับ ");
        data = data.Replace(">", " มากกว่า ");
        data = data.Replace("<", " น้อยกว่า ");
        data = data.Replace("?", " จะได้ ");
        data = data.Replace(":", ", หรือหากไม่ตรงเงื่อนไขจะได้ ");
        data = data.Replace("pow", " ยกกำลัง ");

        Log("ConvertOneLineIfElse >> data: " + data);

        return data;
    }

    /// <summary>
    /// Get MID name from value
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetMIDName(string data)
    {
        if (data.ToLower().Contains("class_"))
        {
            Log("<color=red>Wrong class mid!</color>");
            return data;
        }

        if (string.IsNullOrEmpty(data))
            return null;
        int id = int.Parse(data);
        if (!string.IsNullOrEmpty(GetMonsterName(id)))
            return GetMonsterName(id);
        else if (!string.IsNullOrEmpty(GetJobName(id)))
            return GetJobName(id);
        else
            return null;
    }

    /// <summary>
    /// Get job name from id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    string GetJobName(int id)
    {
        if (id == 0)
            return "Novice";
        else if (id == 1)
            return "Swordman";
        else if (id == 2)
            return "Magician";
        else if (id == 3)
            return "Archer";
        else if (id == 4)
            return "Acolyte";
        else if (id == 5)
            return "Merchant";
        else if (id == 6)
            return "Thief";
        else if (id == 7)
            return "Knight";
        else if (id == 8)
            return "Priest";
        else if (id == 9)
            return "Wizard";
        else if (id == 10)
            return "Blacksmith";
        else if (id == 11)
            return "Hunter";
        else if (id == 12)
            return "Assassin";
        else if (id == 13)
            return "Knight (Peco)";
        else if (id == 14)
            return "Crusader";
        else if (id == 15)
            return "Monk";
        else if (id == 16)
            return "Sage";
        else if (id == 17)
            return "Rogue";
        else if (id == 18)
            return "Alchemist";
        else if (id == 19)
            return "Bard";
        else if (id == 20)
            return "Dancer";
        else if (id == 21)
            return "Crusader (Peco)";
        else if (id == 22)
            return "Wedding";
        else if (id == 23)
            return "Super Novice";
        else if (id == 24)
            return "Gunslinger";
        else if (id == 25)
            return "Ninja";
        else if (id == 4001)
            return "Novice High";
        else if (id == 4002)
            return "Swordman High";
        else if (id == 4003)
            return "Magician High";
        else if (id == 4004)
            return "Archer High";
        else if (id == 4005)
            return "Acolyte High";
        else if (id == 4006)
            return "Merchant High";
        else if (id == 4007)
            return "Thief High";
        else if (id == 4008)
            return "Lord Knight";
        else if (id == 4009)
            return "High Priest";
        else if (id == 4010)
            return "High Wizard";
        else if (id == 4011)
            return "Whitesmith";
        else if (id == 4012)
            return "Sniper";
        else if (id == 4013)
            return "Assassin Cross";
        else if (id == 4014)
            return "Lord Knight (Peco)";
        else if (id == 4015)
            return "Paladin";
        else if (id == 4016)
            return "Champion";
        else if (id == 4017)
            return "Professor";
        else if (id == 4018)
            return "Stalker";
        else if (id == 4019)
            return "Creator";
        else if (id == 4020)
            return "Clown";
        else if (id == 4021)
            return "Gypsy";
        else if (id == 4022)
            return "Paladin (Peco)";
        else if (id == 4023)
            return "Baby Novice";
        else if (id == 4024)
            return "Baby Swordman";
        else if (id == 4025)
            return "Baby Magician";
        else if (id == 4026)
            return "Baby Archer";
        else if (id == 4027)
            return "Baby Acolyte";
        else if (id == 4028)
            return "Baby Merchant";
        else if (id == 4029)
            return "Baby Thief";
        else if (id == 4030)
            return "Baby Knight";
        else if (id == 4031)
            return "Baby Priest";
        else if (id == 4032)
            return "Baby Wizard";
        else if (id == 4033)
            return "Baby Blacksmith";
        else if (id == 4034)
            return "Baby Hunter";
        else if (id == 4035)
            return "Baby Assassin";
        else if (id == 4036)
            return "Baby Knight (Peco)";
        else if (id == 4037)
            return "Baby Crusader";
        else if (id == 4038)
            return "Baby Monk";
        else if (id == 4039)
            return "Baby Sage";
        else if (id == 4040)
            return "Baby Rogue";
        else if (id == 4041)
            return "Baby Alchemist";
        else if (id == 4042)
            return "Baby Bard";
        else if (id == 4043)
            return "Baby Dancer";
        else if (id == 4044)
            return "Baby Crusader (Peco)";
        else if (id == 4045)
            return "Baby Super Novice";
        else if (id == 4046)
            return "Taekwon";
        else if (id == 4047)
            return "Star Gladiator";
        else if (id == 4048)
            return "Star Gladiator (Union)";
        else if (id == 4049)
            return "Soul Linker";
        else if (id == 4050)
            return "Gangsi";
        else if (id == 4051)
            return "Death Knight";
        else if (id == 4052)
            return "Dark Collector";
        else if (id == 4054)
            return "Rune Knight (Regular)";
        else if (id == 4055)
            return "Warlock (Regular)";
        else if (id == 4056)
            return "Ranger (Regular)";
        else if (id == 4057)
            return "Arch Bishop (Regular)";
        else if (id == 4058)
            return "Mechanic (Regular)";
        else if (id == 4059)
            return "Guillotine Cross (Regular)";
        else if (id == 4060)
            return "Rune Knight (Trans)";
        else if (id == 4061)
            return "Warlock (Trans)";
        else if (id == 4062)
            return "Ranger (Trans)";
        else if (id == 4063)
            return "Arch Bishop (Trans)";
        else if (id == 4064)
            return "Mechanic (Trans)";
        else if (id == 4065)
            return "Guillotine Cross (Trans)";
        else if (id == 4066)
            return "Royal Guard (Regular)";
        else if (id == 4067)
            return "Sorcerer (Regular)";
        else if (id == 4068)
            return "Minstrel (Regular)";
        else if (id == 4069)
            return "Wanderer (Regular)";
        else if (id == 4070)
            return "Sura (Regular)";
        else if (id == 4071)
            return "Genetic (Regular)";
        else if (id == 4072)
            return "Shadow Chaser (Regular)";
        else if (id == 4073)
            return "Royal Guard (Trans)";
        else if (id == 4074)
            return "Sorcerer (Trans)";
        else if (id == 4075)
            return "Minstrel (Trans)";
        else if (id == 4076)
            return "Wanderer (Trans)";
        else if (id == 4077)
            return "Sura (Trans)";
        else if (id == 4078)
            return "Genetic (Trans)";
        else if (id == 4079)
            return "Shadow Chaser (Trans)";
        else if (id == 4080)
            return "Rune Knight (Dragon) (Regular)";
        else if (id == 4081)
            return "Rune Knight (Dragon) (Trans)";
        else if (id == 4082)
            return "Royal Guard (Gryphon) (Regular)";
        else if (id == 4083)
            return "Royal Guard (Gryphon) (Trans)";
        else if (id == 4084)
            return "Ranger (Warg) (Regular)";
        else if (id == 4085)
            return "Ranger (Warg) (Trans)";
        else if (id == 4086)
            return "Mechanic (Mado) (Regular)";
        else if (id == 4087)
            return "Mechanic (Mado) (Trans)";
        else if (id == 4096)
            return "Baby Rune Knight";
        else if (id == 4097)
            return "Baby Warlock";
        else if (id == 4098)
            return "Baby Ranger";
        else if (id == 4099)
            return "Baby Arch Bishop";
        else if (id == 4100)
            return "Baby Mechanic";
        else if (id == 4101)
            return "Baby Guillotine Cross";
        else if (id == 4102)
            return "Baby Royal Guard";
        else if (id == 4103)
            return "Baby Sorcerer";
        else if (id == 4104)
            return "Baby Minstrel";
        else if (id == 4105)
            return "Baby Wanderer";
        else if (id == 4106)
            return "Baby Sura";
        else if (id == 4107)
            return "Baby Genetic";
        else if (id == 4108)
            return "Baby Shadow Chaser";
        else if (id == 4109)
            return "Baby Rune Knight (Dragon)";
        else if (id == 4110)
            return "Baby Royal Guard (Gryphon)";
        else if (id == 4111)
            return "Baby Ranger (Warg)";
        else if (id == 4112)
            return "Baby Mechanic (Mado)";
        else if (id == 4190)
            return "Super Novice (Expanded)";
        else if (id == 4191)
            return "Super Baby (Expanded)";
        else if (id == 4211)
            return "Kagerou";
        else if (id == 4212)
            return "Oboro";
        else if (id == 4215)
            return "Rebellion";
        else if (id == 4218)
            return "Summoner";
        else if (id == 4220)
            return "Baby Summoner";
        else if (id == 4222)
            return "Baby Ninja";
        else if (id == 4223)
            return "Baby Kagerou";
        else if (id == 4224)
            return "Baby Oboro";
        else if (id == 4225)
            return "Baby Taekwon";
        else if (id == 4226)
            return "Baby Star Gladiator";
        else if (id == 4227)
            return "Baby Soul Linker";
        else if (id == 4228)
            return "Baby Gunslinger";
        else if (id == 4229)
            return "Baby Rebellion";
        else if (id == 4238)
            return "Baby Star Gladiator (Union)";
        else if (id == 4239)
            return "Star Emperor";
        else if (id == 4240)
            return "Soul Reaper";
        else if (id == 4241)
            return "Baby Star Emperor";
        else if (id == 4242)
            return "Baby Soul Reaper";
        else if (id == 4243)
            return "Star Emperor (Union)";
        else if (id == 4244)
            return "Baby Star Emperor (Union)";
        else
            return null;
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
            //Log("GetSkillName >> Integer " + paramInt);
            for (int i = 0; i < m_output.m_currentSkillNames.Count; i++)
            {
                var sumData = m_output.m_currentSkillNames[i];
                if (sumData.id == paramInt)
                    return sumData.desc;
            }
        }
        else
        {
            //Log("GetSkillName >> Not Integer " + data);
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
            return null;

        if (!data.ToLower().Contains("ele_"))
        {
            Log("<color=red>Wrong element!</color>");
            return data;
        }

        Element elementFlag = (Element)Enum.Parse(typeof(Element), data, true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(Element), elementFlag) && !elementFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (elementFlag == Element.Ele_Dark)
            return "Dark";
        else if (elementFlag == Element.Ele_Earth)
            return "Earth";
        else if (elementFlag == Element.Ele_Fire)
            return "Fire";
        else if (elementFlag == Element.Ele_Ghost)
            return "Ghost";
        else if (elementFlag == Element.Ele_Holy)
            return "Holy";
        else if (elementFlag == Element.Ele_Neutral)
            return "Neutral";
        else if (elementFlag == Element.Ele_Poison)
            return "Poison";
        else if (elementFlag == Element.Ele_Undead)
            return "Undead";
        else if (elementFlag == Element.Ele_Water)
            return "Water";
        else if (elementFlag == Element.Ele_Wind)
            return "Wind";
        else if (elementFlag == Element.Ele_All)
            return "ทุกธาตุ";

        return null;
    }

    /// <summary>
    /// Get race name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetRaceName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        if (!data.ToLower().Contains("rc_"))
        {
            Log("<color=red>Wrong race!</color>");
            return data;
        }

        Race raceFlag = (Race)Enum.Parse(typeof(Race), data, true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(Race), raceFlag) && !raceFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (raceFlag == Race.RC_Angel)
            return "Angel";
        else if (raceFlag == Race.RC_Brute)
            return "Brute";
        else if (raceFlag == Race.RC_DemiHuman)
            return "Demi-Human";
        else if (raceFlag == Race.RC_Demon)
            return "Demon";
        else if (raceFlag == Race.RC_Dragon)
            return "Dragon";
        else if (raceFlag == Race.RC_Fish)
            return "Fish";
        else if (raceFlag == Race.RC_Formless)
            return "Formless";
        else if (raceFlag == Race.RC_Insect)
            return "Insect";
        else if (raceFlag == Race.RC_Plant)
            return "Plant";
        else if (raceFlag == Race.RC_Player)
            return "Player";
        else if (raceFlag == Race.RC_Undead)
            return "Undead";
        else if (raceFlag == Race.RC_All)
            return "ทุกเผ่า";

        return null;
    }

    /// <summary>
    /// Get class name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetClassName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        if (!data.ToLower().Contains("class_"))
        {
            Log("<color=red>Wrong class!</color>");
            return data;
        }

        Class classFlag = (Class)Enum.Parse(typeof(Class), data, true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(Class), classFlag) && !classFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (classFlag == Class.Class_Normal)
            return "Normal";
        else if (classFlag == Class.Class_Boss)
            return "Boss";
        else if (classFlag == Class.Class_Guardian)
            return "Guardian";
        else if (classFlag == Class.Class_All)
            return "ทุก Class";

        return null;
    }

    /// <summary>
    /// Get size name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetSizeName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        if (!data.ToLower().Contains("size_"))
        {
            Log("<color=red>Wrong size!</color>");
            return data;
        }

        Size sizeFlag = (Size)Enum.Parse(typeof(Size), data, true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(Size), sizeFlag) && !sizeFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (sizeFlag == Size.Size_Small)
            return "Small";
        else if (sizeFlag == Size.Size_Medium)
            return "Medium";
        else if (sizeFlag == Size.Size_Large)
            return "Large";
        else if (sizeFlag == Size.Size_All)
            return "ทุก Size";

        return null;
    }

    /// <summary>
    /// Get auto spell flag name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetAutoSpellFlagName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        AutoSpellFlag autoSpellFlag = (AutoSpellFlag)Enum.Parse(typeof(AutoSpellFlag), data, true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(AutoSpellFlag), autoSpellFlag) && !autoSpellFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        string sum = null;

        if (autoSpellFlag.HasFlag(AutoSpellFlag.CastOnEnemy))
            sum += ", ร่ายใส่ศัตรู";
        if (autoSpellFlag.HasFlag(AutoSpellFlag.CastOnSelf))
            sum += ", ร่ายใส่ตนเอง";
        if (autoSpellFlag.HasFlag(AutoSpellFlag.RandomSkillLvOnEnemy))
            sum += ", ร่ายสุ่ม Lv. ใส่ศัตรู";
        if (autoSpellFlag.HasFlag(AutoSpellFlag.UseRandomSkillLv))
            sum += ", ร่ายสุ่ม Lv.";

        sum = sum.Substring(1);

        return sum;
    }

    /// <summary>
    /// Get auto spell on skill flag name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetAutoSpellOnSkillFlagName(string data, string lv)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        AutoSpellOnSkillFlag autoSpellOnSkillFlag = (AutoSpellOnSkillFlag)Enum.Parse(typeof(AutoSpellOnSkillFlag), data, true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(AutoSpellOnSkillFlag), autoSpellOnSkillFlag) && !autoSpellOnSkillFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        string sum = null;

        if (autoSpellOnSkillFlag.HasFlag(AutoSpellOnSkillFlag.CastOnSelf))
            sum += ", ร่ายใส่ตนเอง";
        if (autoSpellOnSkillFlag.HasFlag(AutoSpellOnSkillFlag.RandomLvSkillFromHighestGivenBonus))
            sum += ", ร่ายสุ่ม Lv. 1~" + lv;

        sum = sum.Substring(1);

        return sum;
    }

    /// <summary>
    /// Get effect name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetEffectName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        if (!data.ToLower().Contains("eff_"))
        {
            Log("<color=red>Wrong effect!</color>");
            return data;
        }

        StatusEffect statusEffectFlag = (StatusEffect)Enum.Parse(typeof(StatusEffect), data, true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(StatusEffect), statusEffectFlag) && !statusEffectFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (statusEffectFlag == StatusEffect.Eff_Bleeding)
            return "Bleeding";
        else if (statusEffectFlag == StatusEffect.Eff_Blind)
            return "Blind";
        else if (statusEffectFlag == StatusEffect.Eff_Burning)
            return "Burning";
        else if (statusEffectFlag == StatusEffect.Eff_Confusion)
            return "Confusion";
        else if (statusEffectFlag == StatusEffect.Eff_Crystalize)
            return "Crystalize";
        else if (statusEffectFlag == StatusEffect.Eff_Curse)
            return "Curse";
        else if (statusEffectFlag == StatusEffect.Eff_DPoison)
            return "Deadly Poison";
        else if (statusEffectFlag == StatusEffect.Eff_Fear)
            return "Fear";
        else if (statusEffectFlag == StatusEffect.Eff_Freeze)
            return "Freeze";
        else if (statusEffectFlag == StatusEffect.Eff_Poison)
            return "Poison";
        else if (statusEffectFlag == StatusEffect.Eff_Silence)
            return "Silence";
        else if (statusEffectFlag == StatusEffect.Eff_Sleep)
            return "Sleep";
        else if (statusEffectFlag == StatusEffect.Eff_Stone)
            return "Stone";
        else if (statusEffectFlag == StatusEffect.Eff_Stun)
            return "Stun";

        return null;
    }

    /// <summary>
    /// Get monster race name
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetMonsterRaceName(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        if (!data.ToLower().Contains("rc2_"))
        {
            Log("<color=red>Wrong race 2!</color>");
            return data;
        }

        MonsterRace monsterRaceFlag = (MonsterRace)Enum.Parse(typeof(MonsterRace), data, true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(MonsterRace), monsterRaceFlag) && !monsterRaceFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

        if (monsterRaceFlag == MonsterRace.RC2_Goblin)
            return "Goblin";
        else if (monsterRaceFlag == MonsterRace.RC2_Golem)
            return "Golem";
        else if (monsterRaceFlag == MonsterRace.RC2_Guardian)
            return "Guardian";
        else if (monsterRaceFlag == MonsterRace.RC2_Kobold)
            return "Kobold";
        else if (monsterRaceFlag == MonsterRace.RC2_Ninja)
            return "Ninja";
        else if (monsterRaceFlag == MonsterRace.RC2_Orc)
            return "Orc";
        else if (monsterRaceFlag == MonsterRace.RC2_BioLab)
            return "Bio Laboratory";
        else if (monsterRaceFlag == MonsterRace.RC2_SCARABA)
            return "Scaraba";
        else if (monsterRaceFlag == MonsterRace.RC2_FACEWORM)
            return "Faceworm";
        else if (monsterRaceFlag == MonsterRace.RC2_THANATOS)
            return "Thanatos";
        else if (monsterRaceFlag == MonsterRace.RC2_CLOCKTOWER)
            return "Clock Tower";
        else if (monsterRaceFlag == MonsterRace.RC2_ROCKRIDGE)
            return "Rockridge";

        return null;
    }

    /// <summary>
    /// Get all trigger criteria from value
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    string GetTriggerCriteria(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        if (data.ToLower().Contains("bf_")
            || data.ToLower().Contains("atf_"))
        {

        }
        else
        {
            Log("<color=red>Wrong trigger criteria!</color>");
            return null;
        }

        string sum = null;

        List<string> allTriggerCriteria = StringSplit.GetStringSplit(data, '|');

        for (int i = 0; i < allTriggerCriteria.Count; i++)
        {
            TriggerCriteria triggerCriteria = (TriggerCriteria)Enum.Parse(typeof(TriggerCriteria), allTriggerCriteria[i], true);

            // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
            if (!Enum.IsDefined(typeof(TriggerCriteria), triggerCriteria) && !triggerCriteria.ToString().Contains(","))
                throw new InvalidOperationException($"{data} is not an underlying value of the YourEnum enumeration.");

            if (triggerCriteria == TriggerCriteria.BF_SHORT)
                sum += ", โจมตีระยะประชิด";
            if (triggerCriteria == TriggerCriteria.BF_LONG)
                sum += ", โจมตีระยะไกล";
            if (triggerCriteria == TriggerCriteria.BF_WEAPON)
                sum += ", อาวุธ";
            if (triggerCriteria == TriggerCriteria.BF_MAGIC)
                sum += ", เวทย์มนต์";
            if (triggerCriteria == TriggerCriteria.BF_MISC)
                sum += ", การโจมตีอื่น ๆ";
            if (triggerCriteria == TriggerCriteria.BF_NORMAL)
                sum += ", โจมตีกายภาพ";
            if (triggerCriteria == TriggerCriteria.BF_SKILL)
                sum += ", โจมตีเวทย์";
            if (triggerCriteria == TriggerCriteria.ATF_SELF)
                sum += ", เป้าหมาย: ตนเอง";
            if (triggerCriteria == TriggerCriteria.ATF_TARGET)
                sum += ", เป้าหมาย: ศัตรู";
            if (triggerCriteria == TriggerCriteria.ATF_SHORT)
                sum += ", โจมตีระยะประชิด";
            if (triggerCriteria == TriggerCriteria.ATF_LONG)
                sum += ", โจมตีระยะไกล";
            if (triggerCriteria == TriggerCriteria.ATF_WEAPON)
                sum += ", อาวุธ";
            if (triggerCriteria == TriggerCriteria.ATF_MAGIC)
                sum += ", เวทย์มนต์";
            if (triggerCriteria == TriggerCriteria.ATF_MISC)
                sum += ", การโจมตีอื่น ๆ";
            if (triggerCriteria == TriggerCriteria.ATF_SKILL)
                sum += ", เวทย์มนต์";
        }

        Log("GetTriggerCriteria >> sum: " + sum);
        sum = sum.Substring(1);
        Log("GetTriggerCriteria >> sum: " + sum);

        return " เงื่อนไข ( " + sum + " )";
    }

    string GetBonusScriptFlag(string data)
    {
        string sum = null;

        if (string.IsNullOrEmpty(data))
            return null;

        int flag = int.Parse(data);

        BonusScriptFlag bonusScripFlag = (BonusScriptFlag)Enum.Parse(typeof(BonusScriptFlag), flag.ToString("f0"), true);

        // The foo.ToString().Contains(",") check is necessary for enumerations marked with an [Flags] attribute
        if (!Enum.IsDefined(typeof(BonusScriptFlag), bonusScripFlag) && !bonusScripFlag.ToString().Contains(","))
            throw new InvalidOperationException($"{flag.ToString("f0")} is not an underlying value of the YourEnum enumeration.");

        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemoveWhenDead))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อหมดสติ";
            else
                sum += ", หายเมื่อหมดสติ";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemovableByDispell))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อโดน Dispell";
            else
                sum += ", หายเมื่อโดน Dispell";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemovableByClearance))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อโดน Clearance";
            else
                sum += ", หายเมื่อโดน Clearance";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemoveWhenPlayerLogsOut))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อออกจากเกม";
            else
                sum += ", หายเมื่อออกจากเกม";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemoveableByBanishingBuster))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อโดน Banishing Buster";
            else
                sum += ", หายเมื่อโดน Banishing Buster";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemovableByRefresh))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อโดน Refresh";
            else
                sum += ", หายเมื่อโดน Refresh";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemovableByLuxAnima))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อโดน Lux Anima";
            else
                sum += ", หายเมื่อโดน Lux Anima";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemoveWhenMadogearIsActivatedOrDeactivated))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อขึ้นหรือลง Madogear";
            else
                sum += ", หายเมื่อขึ้นหรือลง Madogear";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.RemoveWhenReceiveDamage))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "หายเมื่อโดนโจมตี";
            else
                sum += ", หายเมื่อโดนโจมตี";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.ScriptIsPermanentCannotBeClearedByBonusScriptClear))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "ผลที่ได้รับจะคงอยู่ตลอดเวลา";
            else
                sum += ", ผลที่ได้รับจะคงอยู่ตลอดเวลา";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.ForceToReplaceDuplicatedScriptByExpandingTheDuration))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "ผลที่ได้รับจะ เพิ่มเวลา ให้ผลที่ได้รับเดียวกันเท่านั้น";
            else
                sum += ", ผลที่ได้รับจะ เพิ่มเวลา ให้ผลที่ได้รับเดียวกันเท่านั้น";
        }
        if (bonusScripFlag.HasFlag(BonusScriptFlag.ForceToAddDuplicatedScript))
        {
            if (string.IsNullOrEmpty(sum))
                sum += "ผลที่ได้รับจะไม่ทับกับผลที่ได้รับอื่น ๆ";
            else
                sum += ", ผลที่ได้รับจะไม่ทับกับผลที่ได้รับอื่น ๆ";
        }
        return sum;
    }

    /// <summary>
    /// Correct amount to showing
    /// </summary>
    /// <param name="data"></param>
    /// <param name="divider"></param>
    /// <returns></returns>
    string GetRateByDivider(string data, float divider)
    {
        bool isFloat = false;
        float tryParse = 0;
        isFloat = float.TryParse(data, out tryParse);
        if (!isFloat)
        {
            //500+(200*(ตามจำนวนตีบวก))
            string newData = null;
            string currentNumText = null;
            for (int i = 0; i < data.Length; i++)
            {
                tryParse = 0;
                isFloat = float.TryParse(data[i].ToString(), out tryParse);
                Log(data[i] + " isFloat: " + isFloat + ", currentNumText: " + currentNumText);
                if (isFloat)
                    currentNumText += data[i];
                else if (!string.IsNullOrEmpty(currentNumText))
                {
                    float sumValue = (float.Parse(currentNumText) / divider);
                    newData += sumValue;
                    currentNumText = null;
                    if (!isFloat)
                        newData += data[i];
                }
                else
                    newData += data[i];
            }
            return newData;
        }
        float value = (float.Parse(data) / divider);
        if (value < 1)
            return value.ToString("f1");
        else
            return value.ToString("f0");
    }
    #endregion

    /// <summary>
    /// Set parameter x to true or false for adding description or not add
    /// </summary>
    /// <param name="paramCount"></param>
    /// <param name="isTrue"></param>
    void SetParamCheck(int paramCount, bool isTrue, bool isNegative)
    {
        if (paramCount == -1)
            return;

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
        if (!string.IsNullOrEmpty(toAdd))
        {
            while (toAdd.Contains("  "))
                toAdd = toAdd.Replace("  ", " ");

            toAdd = toAdd.Replace(", ]", " ]");
            toAdd = toAdd.Replace("[ตามจำนวนตีบวก]=[ตามจำนวนตีบวก]", "");
        }
        else
            return null;

        if (string.IsNullOrEmpty(data))
            return "\"" + toAdd + "\",";
        else
            return "\n\"" + toAdd + "\",";
    }

    void Log(object obj, bool isFullLog = false, string color = null)
    {
        bool isFullLogOn = true;

        if (isFullLog && !isFullLogOn)
            return;

        if (!Application.isPlaying)
        {
            if (!string.IsNullOrEmpty(color))
                Debug.Log("<color=" + color + ">" + obj + "</color>");
            else
                Debug.Log(obj);
        }
    }
}
