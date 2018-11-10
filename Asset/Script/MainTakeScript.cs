using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainTakeScript : MonoBehaviour,IPointerClickHandler
 {
    public RequestTaklAPI requestTaklAPI;
    public Text inputText;

    public void OnPointerClick(PointerEventData eventData)
    {
        requestTaklAPI.MessageStart(inputText.text);
    }
}
