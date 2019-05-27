using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockText : MonoBehaviour
{
    public InputField TextToVerify;
    public InputField TextToBlock;

    void Update()
    {
        TextToBlock.interactable = string.IsNullOrEmpty(TextToVerify.text);
    }
}
