using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogController : MonoBehaviour
{
    //GameObjects
    public static DialogController Instance;
    [SerializeField] TextMeshProUGUI DialogTMP;
    [SerializeField] TextMeshProUGUI[] DialogOptionsTMP;
    DialogNode CurrentNode;

    void Start()
    {
        Instance = this;
    }

    public void Talk(Talkative talker)
    {
        DialogTMP.SetText(talker.Dialog.getDialog());
        ShowDialog(talker.Dialog.getDialog());
        ShowOptions(talker.Dialog.getOptions());
        CurrentNode = talker.Dialog;
    }

    public void ShowDialog(string dialog) { DialogTMP.SetText(dialog); }

    public void ShowOptions(string[] options)
    {
        if (options == null)
        {
            DialogOptionsTMP[0].SetText("");
            DialogOptionsTMP[1].SetText("");
            DialogOptionsTMP[2].SetText("");
        }
        else
        {
            DialogOptionsTMP[0].SetText(options[0]);
            DialogOptionsTMP[1].SetText(options[1]);
            DialogOptionsTMP[2].SetText(options[2]);
        }
    }
    public void ContinueDialog(int index)
    {
        if (CurrentNode == null || CurrentNode.getOptionsOutcomes() == null)
        {
            EndDialog();
            return;
        }
        CurrentNode = CurrentNode.getOptionsOutcomes()[index];
        ShowDialog(CurrentNode.getDialog());
        ShowOptions(CurrentNode.getOptions());
    }

    public void EndDialog()
    {
        DialogTMP.SetText("");
        DialogOptionsTMP[0].SetText("");
        DialogOptionsTMP[1].SetText("");
        DialogOptionsTMP[2].SetText("");
        CurrentNode = null;
    }

}
