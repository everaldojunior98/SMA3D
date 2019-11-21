using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderDrag : MonoBehaviour, IPointerUpHandler
{
    public CommunicationManager Manager;

    public void OnPointerUp(PointerEventData eventData)
    {
        Manager.SendPWM();
    }
}