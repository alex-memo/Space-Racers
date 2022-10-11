using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/**
 * @memo 2022
 * Simple card Controller
 */
public class cardController : MonoBehaviour
{
    public TMP_Text cardNumber;
    public Image cardImage;
    public TMP_Text description;
    private cardObject currentCard;
    /**
 * @memo 2022
 * sets card on ui
 */
    public void setCard(cardObject card)
    {
        currentCard = card;
        description.text = card.cardDescription;
        cardNumber.text = card.cardNumber.ToString();
        cardImage.sprite = card.cardImage;

    }
    /**
 * @memo 2022
 * botton clicked (card)
 */
    public void OnClickCard()
    {
        Controller player = transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.GetComponent<Controller>();
        if (currentCard.cardType == 0&&player.getHasToMove()==false)//if it is a number (movement card)
        {
            
            
            if (player.getTurn()==gameManagerScript.instance.turn)//only if players turn
            {
                print("player played a " + currentCard.cardNumber);
                player.move(currentCard.cardNumber, true);
                player.removeCard(currentCard);
                player.getCard();
            }
            else
            {
                print("not player turn");
            }
        }
        else if (currentCard.cardType == cardObject.CardType.Special && player.getHasToMove() == false)//if special card
        {
            if (player.getTurn() == gameManagerScript.instance.turn)
            {
                print("player played a special card!");
                SpecialCardPlayed();
            }
        }
        
        
    }
    /**
 * @memo 2022
 * If user buys a card, clicks on buy button
 */
    public void OnBuyCard()
    {
        int price = 10;//could set this on scriptable object for csutom prices but i just dont have the time anymore as I have a life
        //add this card to player controller
        print("buy card " + currentCard.name);
        Controller player = transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.GetComponent<Controller>();
        if (player.score - price < 0) { return; }//if user has less than 10 coins then cant buy
        player.subtractScore(price);
        player.deck.Add(currentCard);
        player.setDeckPlayer();
        player.quitShop();
    }
    /**
 * @memo 2022
 * Method in charge for all the special card interactions in a card
 */
    private void SpecialCardPlayed()
    {
        Controller player = transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.GetComponent<Controller>();
        if (currentCard.cardNumber == 11)//Swap decks with a specific player
        {
            //player.selectedCard = currentCard;
            //wont implement
        }
        else if (currentCard.cardNumber == 12)//Swap decks with a random player
        {
            //player.swapDecks();//wont have time to finish it, it was close to being done
            //gameManagerScript.instance.swapDecks();//no auth
        }
        else if (currentCard.cardNumber == 13)//Only you get a new set of cards
        {
            player.getDeckLocal(player.deck.Count-1);//works
        }
        else if (currentCard.cardNumber == 14)//Inverts state of traps
        {
            player.enableDisableBlackholes(!gameManagerScript.instance.areTrapsEnabled);           
        }
        else if (currentCard.cardNumber == 15)//Make a specific player move back
        {
            //wont implement card removed
        }
        else if (currentCard.cardNumber == 16)//Make all players move back (but you)
        {
            player.everyoneBackButYou(5, player.netId);//works
        }
        else if (currentCard.cardNumber == 17)//Make all players move front (including you?)
        {
            player.callAllPlayersMove(5);//works but wont go thorugh black holes
        }
        else if (currentCard.cardNumber == 18)//Disable all shops
        {
            player.disableShops(!gameManagerScript.instance.areShopsEnabled);//not fully impemented, disables but no way to tell to players yet
        }
        else if (currentCard.cardNumber == 19)//Double points gained on your next turn
        {
            player.doublePoints = true;//works
        }
        else if (currentCard.cardNumber == 20)//Skip next player's turn
        {
            player.skipNextPlayerTurn();//works
        }
        else if (currentCard.cardNumber == 21)//Reverse turn order
        {

        }
        player.callAddTurn();
        //player.isTurn = false;
        //player.timer = 0;
        //player.UpdateScoreboard();
        player.removeCard(currentCard);
        player.getCard();

    }

}
