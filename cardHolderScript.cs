using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardHolderScript : MonoBehaviour
{
    public GameObject cardUIPrefab;
    /**
 * @memo 2022
 * sets deck on ui
 */
    public void setDeck(List<cardObject> deck)
    {
        
        foreach(Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (cardObject card in deck)
        {
            GameObject temp = Instantiate(cardUIPrefab, this.transform);
            temp.GetComponent<cardController>().setCard(card);
        }
    }
    /**
 * @memo 2022
 * closes shop
 */
    public void closeShop()
    {
        transform.parent.transform.parent.gameObject.SetActive(false);
    }
    /**
 * @memo 2022
 * opens shop
 */
    public void openShop()
    {
        transform.parent.transform.parent.gameObject.SetActive(true);
    }

}
