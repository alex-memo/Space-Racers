using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New Card", menuName = "Cards/New Card")]
/**
 * @memo 2022
 * Card Scriptable Object
 */
public class cardObject : ScriptableObject
{
    public CardType cardType;
    public int cardNumber;
    public string cardDescription;
    public Sprite cardImage;
    public string constellationName;
    public enum CardType { Number, Special };

}
