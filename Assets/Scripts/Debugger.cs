using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Debugger : Singleton<Debugger>
{
#pragma warning disable 0649
    [SerializeField] Text txtDebugger;

    public void Debug(string txt)
    {
        txtDebugger.text = txt;
        debugTimer = 10;
    }

    float debugTimer;
    void Update()
    {
        if (debugTimer > 0)
        {
            debugTimer -= Time.deltaTime;
            if (debugTimer <= 0)
                txtDebugger.text = null;
        }
    }

    /// <summary>
    /// Credit: https://answers.unity.com/questions/707636/clear-console-window.html
    /// </summary>
    public static void ClearConsole()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
#endif
    }
}
