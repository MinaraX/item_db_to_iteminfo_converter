using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

[CreateAssetMenu(fileName = "Output", menuName = "Start/Output")]
public class Output : ScriptableObject
{
    [TextArea]
    public string currentOutput;

    public string[] lines;

    public TextAsset textAsset_someTextFiles;
}
