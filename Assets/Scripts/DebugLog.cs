using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class DebugLog : MonoBehaviour
{
    public TextMeshProUGUI[] textsArray;
    private static TextMeshProUGUI[] textsArray_static;

    private static int index = 0;

    private void Awake()
    {
        textsArray_static = textsArray;
    }

    public static void Log(string message, bool consoleToo = true)
    {
        if (index > 0)
        {
            textsArray_static[index - 1].color = UnityEngine.Color.white;
        }
        else
        {
            textsArray_static[textsArray_static.Length - 1].color = UnityEngine.Color.white;
        }

        textsArray_static[index].color = UnityEngine.Color.red;
        textsArray_static[index].text = message;

        if (consoleToo)
        {
            Debug.Log(message);
        }

        if (index >= textsArray_static.Length - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }
    }
}
