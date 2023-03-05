using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoppyToClipboardExtension
{
    // This needs to be added to a public static class to be used like an extension
    public static void CopyToClipboard(this string s)
    {
        TextEditor te = new TextEditor {text = s};
        te.SelectAll();
        te.Copy();
    }

    /*
    Example Use Case 1 :: 
    string s = "https://answers.unity.com";
    s.CopyToClipboard();
    
    Example Use Case 2 ::
    Text textField;
    string s = textField.text;
    s.CopyToClipboard();
    */
}