using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLogs : MonoBehaviour
{
    public TextMeshProUGUI[] textsArray;

    private static TextMeshProUGUI[] textsArray_static;

    private static int index = 0;

    private void Awake()
    {
        textsArray_static = textsArray;
    }

    public static void ShowMessage(string message, bool consoleToo = true)
    {        
        textsArray_static[index].text = message;

        if (consoleToo)
        {
            Debug.Log(message);
        }

        index++;

        if (index >= textsArray_static.Length)
        {
            index = 0;
        }
    }
}
