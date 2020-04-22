using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogOptionClickable : MonoBehaviour, IPointerClickHandler
{
    public int index;

    public void OnPointerClick(PointerEventData eventData) 
    {
        DialogController.Instance.ContinueDialog(index);
    }
}
