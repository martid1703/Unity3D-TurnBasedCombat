using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleQueueSlider : MonoBehaviour, IEndDragHandler
{
    private ScrollRect _rect;
    private void Awake()
    {
        _rect = this.GetComponent<ScrollRect>();
    }
    public void OnEndDrag(PointerEventData data)
    {
        _rect.horizontalNormalizedPosition = 0f;
    }
}
