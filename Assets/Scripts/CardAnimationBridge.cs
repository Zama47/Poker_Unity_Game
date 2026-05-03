using UnityEngine;
using UnityEngine.UI;

public class CardAnimationBridge : MonoBehaviour
{
    [Header("Managers")]
    public DeckManager deckManager;
    public DeckAnimator deckAnimator;

    [Header("Card Images")]
    public Image playerACard1;
    public Image playerACard2;
    public Image playerBCard1;
    public Image playerBCard2;
    public Image[] communitySprites;

    private void Start()
    {
        if (deckManager == null)
            deckManager = FindObjectOfType<DeckManager>();

        if (deckAnimator == null)
            deckAnimator = FindObjectOfType<DeckAnimator>();
    }

    public void AnimateShowPlayerCards(Sprite card1, Sprite card2)
    {
        if (deckAnimator == null) return;

        if (playerACard1 != null)
        {
            playerACard1.sprite = deckManager.backSprite;
            deckAnimator.playerCard1?.FlipOpen(card1);
        }

        if (playerACard2 != null)
        {
            playerACard2.sprite = deckManager.backSprite;
            deckAnimator.playerCard2?.FlipOpen(card2);
        }
    }

    public void AnimateShowOpponentCards(Sprite card1, Sprite card2)
    {
        if (deckAnimator == null) return;

        deckAnimator.FlipOpponentCards(card1, card2);
    }

    public void AnimateCommunityCard(int index, Sprite sprite)
    {
        if (deckAnimator == null || index >= communitySprites.Length) return;

        if (communitySprites[index] != null)
        {
            communitySprites[index].sprite = sprite;
        }
    }

    public void StartDealAnimation()
    {
        if (deckAnimator == null) return;

        deckAnimator.ResetAllCards();
        deckAnimator.StartDealAnimation();
    }

    public void AnimateFlop(Sprite[] sprites)
    {
        if (deckAnimator == null) return;

        for (int i = 0; i < 3 && i < sprites.Length; i++)
        {
            if (communitySprites[i] != null)
            {
                communitySprites[i].sprite = sprites[i];
            }
        }

        deckAnimator.DealCommunityCards(3);
    }

    public void AnimateTurn(Sprite sprite)
    {
        if (deckAnimator == null) return;

        if (communitySprites.Length > 3 && communitySprites[3] != null)
        {
            communitySprites[3].sprite = sprite;
        }

        deckAnimator.DealCommunityCards(4);
    }

    public void AnimateRiver(Sprite sprite)
    {
        if (deckAnimator == null) return;

        if (communitySprites.Length > 4 && communitySprites[4] != null)
        {
            communitySprites[4].sprite = sprite;
        }

        deckAnimator.DealCommunityCards(5);
    }
}
