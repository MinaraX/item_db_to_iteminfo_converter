using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Credit: https://thatfrenchgamedev.com/785/unity-2018-how-to-copy-string-to-clipboard/
/// </summary>
public static class ClipboardExtension
{
    /// <summary>
    /// Puts the string into the Clipboard.
    /// </summary>
    /// <param name="str"></param>
    public static void CopyToClipboard(this string str)
    {
        var textEditor = new TextEditor();
        textEditor.text = str;
        textEditor.SelectAll();
        textEditor.Copy();
    }
}