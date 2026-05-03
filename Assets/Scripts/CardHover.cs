using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardAnimator))]
public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CardAnimator cardAnimator;

    private void Awake()
    {
        cardAnimator = GetComponent<CardAnimator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardAnimator?.OnPointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardAnimator?.OnPointerExit();
    }
}
