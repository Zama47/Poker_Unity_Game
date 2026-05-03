using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float dealDuration = 0.5f;
    [SerializeField] private float flipDuration = 0.3f;
    [SerializeField] private float hoverLift = 20f;
    [SerializeField] private float hoverDuration = 0.2f;
    [SerializeField] private Ease dealEase = Ease.OutBack;
    [SerializeField] private Ease flipEase = Ease.InOutQuad;

    private RectTransform rectTransform;
    private Image cardImage;
    private Vector2 originalPosition;
    private Vector2 originalSize;
    private bool isFlipped = false;
    private bool isAnimating = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cardImage = GetComponent<Image>();
        originalPosition = rectTransform.anchoredPosition;
        originalSize = rectTransform.sizeDelta;
    }

    public void DealFrom(Vector2 fromPosition, bool faceDown = false)
    {
        if (isAnimating) return;

        rectTransform.anchoredPosition = fromPosition;
        rectTransform.localScale = Vector3.zero;
        cardImage.color = new Color(1, 1, 1, 0);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScale(Vector3.one, dealDuration).SetEase(dealEase));
        sequence.Join(rectTransform.DOAnchorPos(originalPosition, dealDuration).SetEase(dealEase));
        sequence.Join(cardImage.DOFade(1, dealDuration * 0.5f));

        if (faceDown)
        {
            sequence.AppendCallback(() => isFlipped = false);
        }

        sequence.OnComplete(() => isAnimating = false);
        isAnimating = true;
    }

    public void FlipCard(Sprite newSprite)
    {
        if (isAnimating || isFlipped) return;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScaleX(0, flipDuration * 0.5f).SetEase(flipEase));
        sequence.AppendCallback(() => cardImage.sprite = newSprite);
        sequence.Append(rectTransform.DOScaleX(1, flipDuration * 0.5f).SetEase(flipEase));

        sequence.OnComplete(() =>
        {
            isFlipped = true;
            isAnimating = false;
        });

        isAnimating = true;
    }

    public void FlipOpen(Sprite frontSprite)
    {
        FlipCard(frontSprite);
    }

    public void FlipClose(Sprite backSprite)
    {
        if (isAnimating || !isFlipped) return;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScaleX(0, flipDuration * 0.5f).SetEase(flipEase));
        sequence.AppendCallback(() => cardImage.sprite = backSprite);
        sequence.Append(rectTransform.DOScaleX(1, flipDuration * 0.5f).SetEase(flipEase));

        sequence.OnComplete(() =>
        {
            isFlipped = false;
            isAnimating = false;
        });

        isAnimating = true;
    }

    public void OnPointerEnter()
    {
        if (isAnimating) return;

        rectTransform.DOAnchorPosY(originalPosition.y + hoverLift, hoverDuration)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit()
    {
        if (isAnimating) return;

        rectTransform.DOAnchorPosY(originalPosition.y, hoverDuration)
            .SetEase(Ease.OutQuad);
    }

    public void ResetCard()
    {
        DOTween.Kill(rectTransform);
        DOTween.Kill(cardImage);

        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        cardImage.color = Color.white;
        isFlipped = false;
        isAnimating = false;
    }

    public void SetPosition(Vector2 position)
    {
        originalPosition = position;
        rectTransform.anchoredPosition = position;
    }

    public bool IsAnimating => isAnimating;
}
