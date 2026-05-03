// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PokerHandsTest : MonoBehaviour
// {
//     void Start()
//     {
//         var handA = "TC JC KC QC AC";
//         var handB = "2D 5H 8C TD KC";

//         int result = PokerHands.CompareHands(handA, handB);

//         Debug.Log($"{handA} / {handB} = {result}");

//         handA = "5S 8D 7H 9D 6D";
//         handB = "2D 5D 8D TD KD";

//         result = PokerHands.CompareHands(handA, handB);
//         Debug.Log($"{handA} / {handB} = {result}");

//         handA = "8S 7D 5D 6H 4D";
//         handB = "5D 4D 3H 2C AD";

//         result = PokerHands.CompareHands(handA, handB);
//         Debug.Log($"{handA} / {handB} = {result}");

//         List<string> handA = new List<string>() { "KD", "QD" };
//         List<string> handB = new List<string>() { "QS", "8C" };
//         List<string> community = new List<string> { "AD", "JD", "TD", "TH", "JS" };

//         PokerHands.Results test = PokerHands.GetWinner(handA, handB, community);

//         Debug.Log($"{test.winner} // {test.playerBestHand} / {test.OpponentBestHand}");

//     }
// }
