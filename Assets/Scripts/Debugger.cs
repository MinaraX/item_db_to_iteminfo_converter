using UnityEngine;
using UnityEngine.UI;
using Pixelplacement;

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
}
