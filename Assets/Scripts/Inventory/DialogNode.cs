using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogNode : ScriptableObject
{
    string dialog;
    string[] options;
    DialogNode[] optionsOutcomes;

    public DialogNode(string dialog)
    {
        this.dialog = dialog;
    }

    public DialogNode(string dialog, string[] options, DialogNode[] optionsOutcomes)
    {
        this.dialog = dialog;
        this.options = options;
        this.optionsOutcomes = optionsOutcomes;
    }

    public string getDialog() { return dialog; }
    public string[] getOptions() { return options; }
    public DialogNode[] getOptionsOutcomes() { return optionsOutcomes; }

}