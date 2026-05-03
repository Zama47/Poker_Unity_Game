using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerHands : MonoBehaviour
{
    public struct Rankedhand
    {
        public HandRanking HandRanking;
        public List<int> SortedRanks;
    }

    public enum Winner
    {
        Player = 0,
        Tie = 1,
        Opponent = 2,
    }
    public enum HandRanking
    {
        Invalid = 0,
        HighCard = 1,
        OnePair = 2,
        TwoPairs = 3,
        ThreeOfAKind = 4,
        Straight = 5,
        Flush = 6,
        FullHouse = 7,
        FourOfAKind = 8,
        StraightFlush = 9,
        RoyalFlush = 10,
    }

    public struct Results
    {
        public Winner winner;

        public string playerBestHand;
        public HandRanking playerHandRanking;

        public string OpponentBestHand;
        public HandRanking opponentHandRanking;

    }

    public static readonly Dictionary<char, int> CardRanks = new()
    {
        { 'A', 14 },
        { 'K', 13 },
        { 'Q', 12 },
        { 'J', 11 },
        { 'T', 10 },
        { '9', 9 },
        { '8', 8 },
        { '7', 7 },
        { '6', 6 },
        { '5', 5 },
        { '4', 4 },
        { '3', 3 },
        { '2', 2 },
    };

    public static Rankedhand GetHandRanking(string hand)
    {
        var cardsInHand = hand.Split(" ").ToList();

        if (cardsInHand.Count != 5)
        {
            return new Rankedhand
            {
                HandRanking = HandRanking.Invalid,
                SortedRanks = new List<int>(),
            };
        }

        cardsInHand.Sort((card1, card2) =>
        {
            var numericValueOfCard1 = CardRanks[card1[0]];
            var numericValueOfCard2 = CardRanks[card2[0]];
            return numericValueOfCard1.CompareTo(numericValueOfCard2);
        });
        cardsInHand.Reverse();

        var rankGroups =
            cardsInHand
            .GroupBy(card => card[0])
            .OrderBy(group => group.Count())
            .Reverse();

        var cardsNumInGroup = new List<int>();

        foreach (var rankGroup in rankGroups)
        {
            cardsNumInGroup.Add(rankGroup.Count());
        }

        var suitGroups = cardsInHand.GroupBy(card => card[1]);

        var isFlush = suitGroups.Count() == 1;

        var firstCardRank = CardRanks[cardsInHand[0][0]];
        var isStraight =
            firstCardRank == CardRanks[cardsInHand[1][0]] + 1 &&
            firstCardRank == CardRanks[cardsInHand[2][0]] + 2 &&
            firstCardRank == CardRanks[cardsInHand[3][0]] + 3 &&
            firstCardRank == CardRanks[cardsInHand[4][0]] + 4;

        var isStraightWithAceLow =
            cardsInHand[0][0] == 'A' &&
            cardsInHand[1][0] == '5' &&
            cardsInHand[2][0] == '4' &&
            cardsInHand[3][0] == '3' &&
            cardsInHand[4][0] == '2';

        if (isStraightWithAceLow)
        {
            isStraight = true;

            cardsInHand.Add(cardsInHand[0]);
            cardsInHand.RemoveAt(0);
        }

        var sortedRanks = new List<int>();

        foreach (var card in cardsInHand)
        {
            sortedRanks.Add(CardRanks[card[0]]);
        }

        if (isStraightWithAceLow)
        {
            sortedRanks[4] = 1;
        }

        if (isFlush && isStraight)
        {
            if (cardsInHand[0][0] == 'A')
            {
                return new Rankedhand
                {
                    HandRanking = HandRanking.RoyalFlush,
                    SortedRanks = sortedRanks,
                };
            }

            return new Rankedhand
            {
                HandRanking = HandRanking.StraightFlush,
                SortedRanks = sortedRanks,
            };
        }

        if (cardsNumInGroup[0] == 4)
        {
            return new Rankedhand
            {
                HandRanking = HandRanking.FourOfAKind,
                SortedRanks = sortedRanks,
            };
        }

        if (cardsNumInGroup.Count == 2)
        {
            if (cardsNumInGroup[0] == 3 && cardsNumInGroup[1] == 2)
            {
                return new Rankedhand
                {
                    HandRanking = HandRanking.FullHouse,
                    SortedRanks = sortedRanks,
                };
            }
        }

        if (suitGroups.Count() == 1)
        {
            return new Rankedhand
            {
                HandRanking = HandRanking.Flush,
                SortedRanks = sortedRanks,
            };
        }

        if (isStraight)
        {
            return new Rankedhand
            {
                HandRanking = HandRanking.Straight,
                SortedRanks = sortedRanks,
            };
        }

        if (cardsNumInGroup[0] == 3)
        {
            return new Rankedhand
            {
                HandRanking = HandRanking.ThreeOfAKind,
                SortedRanks = sortedRanks,
            };
        }

        if (cardsNumInGroup.Count > 2)
        {
            if (cardsNumInGroup[0] == 2 && cardsNumInGroup[1] == 2)
            {
                return new Rankedhand
                {
                    HandRanking = HandRanking.TwoPairs,
                    SortedRanks = sortedRanks,
                };
            }
        }

        if (cardsNumInGroup[0] == 2)
        {
            return new Rankedhand
            {
                HandRanking = HandRanking.OnePair,
                SortedRanks = sortedRanks,
            };
        }

        return new Rankedhand
        {
            HandRanking = HandRanking.HighCard,
            SortedRanks = sortedRanks,
        };
    }

    public static int CompareLists(List<int> A, List<int> B)
    {
        if (A.Count > B.Count)
        {
            return 1;
        }
        else if (A.Count < B.Count)
        {
            return -1;
        }

        for (int idx = 0; idx < A.Count; idx++)
        {
            if (A[idx] > B[idx])
            {
                return 1;
            }
            else if (A[idx] < B[idx])
            {
                return -1;
            }
        }

        return 0;
    }

    public static int CompareHands(string handA, string handB)
    {

        Rankedhand rankedhandA = GetHandRanking(handA);
        Rankedhand rankedhandB = GetHandRanking(handB);
        var handARank = (int)rankedhandA.HandRanking;
        var handBRank = (int)rankedhandB.HandRanking;

        if (handARank.CompareTo(handBRank) == 0)
        {
            return CompareLists(rankedhandA.SortedRanks, rankedhandB.SortedRanks);
        }

        return handARank.CompareTo(handBRank);
    }

    public static Results GetWinner(List<string> playerCards, List<string> opponentCards, List<string> communityCards)
    {
        var cardsAvailableToPlayerA = playerCards.Concat(communityCards).ToList();
        var cardsAvailableToPlayerB = opponentCards.Concat(communityCards).ToList();

        cardsAvailableToPlayerA.Sort((n1, n2) =>
        {
            return CardRanks[n1[0]].CompareTo(CardRanks[n2[0]]);
        });
        cardsAvailableToPlayerA.Reverse();

        cardsAvailableToPlayerB.Sort((n1, n2) =>
        {
            return CardRanks[n1[0]].CompareTo(CardRanks[n2[0]]);
        });
        cardsAvailableToPlayerB.Reverse();

        int nNumberOfElements = cardsAvailableToPlayerA.Count;
        int kSampleSize = 5;

        var handCombinations = Combinations.CreateCombinations(nNumberOfElements, kSampleSize);

        var possibleHandsPlayerA = new List<string>();
        var possibleHandsPlayerB = new List<string>();

        foreach (List<int> hand in handCombinations)
        {
            string handStringA = "";
            string handStringB = "";

            foreach (int cardIdx in hand)
            {
                handStringA += $"{cardsAvailableToPlayerA[cardIdx - 1]} ";
                handStringB += $"{cardsAvailableToPlayerB[cardIdx - 1]} ";
            }

            handStringA = handStringA.Trim();
            handStringB = handStringB.Trim();

            possibleHandsPlayerA.Add(handStringA);
            possibleHandsPlayerB.Add(handStringB);

        }
  
        possibleHandsPlayerA.Sort(
            (handA, handB) => CompareHands(handA, handB)
        );
        possibleHandsPlayerA.Reverse();

        possibleHandsPlayerB.Sort(
            (handA, handB) => CompareHands(handA, handB)
        );
        possibleHandsPlayerB.Reverse();

        // Вот тут первый элемент А - максимальная рука из имеющихся. Потом надо будет реализовать вывод на экран макс руки для удобства

        int compareValue = CompareHands(possibleHandsPlayerA[0], possibleHandsPlayerB[0]);

        Results results = new()
        {
            winner = compareValue == -1 ? Winner.Opponent : compareValue == 1 ? Winner.Player : Winner.Tie,
            playerBestHand = possibleHandsPlayerA[0],
            playerHandRanking = GetHandRanking(possibleHandsPlayerA[0]).HandRanking,
            OpponentBestHand = possibleHandsPlayerB[0],
            opponentHandRanking = GetHandRanking(possibleHandsPlayerB[0]).HandRanking,
        };

        return results;
    }
}
