using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckAnimator : MonoBehaviour
{
    [Header("Card References")]
    public CardAnimator playerCard1;
    public CardAnimator playerCard2;
    public CardAnimator opponentCard1;
    public CardAnimator opponentCard2;
    public CardAnimator[] communityCards;

    [Header("Deck Settings")]
    public RectTransform deckPosition;
    public float dealDelay = 0.15f;
    public float communityDealDelay = 0.2f;

    [Header("Sprites")]
    public Sprite backSprite;

    private Vector2 deckPos;

    private void Start()
    {
        if (deckPosition != null)
            deckPos = deckPosition.anchoredPosition;
    }

    public void StartDealAnimation()
    {
        StartCoroutine(DealPlayerCardsRoutine());
    }

    private IEnumerator DealPlayerCardsRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (playerCard1 != null)
            playerCard1.DealFrom(deckPos, true);

        yield return new WaitForSeconds(dealDelay);

        if (playerCard2 != null)
            playerCard2.DealFrom(deckPos, true);

        yield return new WaitForSeconds(dealDelay);

        if (opponentCard1 != null)
            opponentCard1.DealFrom(deckPos, true);

        yield return new WaitForSeconds(dealDelay);

        if (opponentCard2 != null)
            opponentCard2.DealFrom(deckPos, true);
    }

    public void DealCommunityCards(int count)
    {
        StartCoroutine(DealCommunityRoutine(count));
    }

    private IEnumerator DealCommunityRoutine(int count)
    {
        for (int i = 0; i < count && i < communityCards.Length; i++)
        {
            if (communityCards[i] != null)
                communityCards[i].DealFrom(deckPos, false);

            yield return new WaitForSeconds(communityDealDelay);
        }
    }

    public void FlipPlayerCard(int cardIndex, Sprite frontSprite)
    {
        switch (cardIndex)
        {
            case 0:
                playerCard1?.FlipOpen(frontSprite);
                break;
            case 1:
                playerCard2?.FlipOpen(frontSprite);
                break;
        }
    }

    public void FlipOpponentCards(Sprite frontSprite1, Sprite frontSprite2)
    {
        StartCoroutine(FlipOpponentCardsRoutine(frontSprite1, frontSprite2));
    }

    private IEnumerator FlipOpponentCardsRoutine(Sprite sprite1, Sprite sprite2)
    {
        if (opponentCard1 != null)
        {
            opponentCard1.FlipOpen(sprite1);
            yield return new WaitForSeconds(0.2f);
        }

        if (opponentCard2 != null)
        {
            opponentCard2.FlipOpen(sprite2);
        }
    }

    public void ResetAllCards()
    {
        playerCard1?.ResetCard();
        playerCard2?.ResetCard();
        opponentCard1?.ResetCard();
        opponentCard2?.ResetCard();

        foreach (var card in communityCards)
        {
            card?.ResetCard();
        }
    }

    public bool IsAnyCardAnimating()
    {
        if (playerCard1?.IsAnimating == true) return true;
        if (playerCard2?.IsAnimating == true) return true;
        if (opponentCard1?.IsAnimating == true) return true;
        if (opponentCard2?.IsAnimating == true) return true;

        foreach (var card in communityCards)
        {
            if (card?.IsAnimating == true) return true;
        }

        return false;
    }
}
