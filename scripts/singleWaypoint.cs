using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
 * @memo 2022
 * Simple stat holder for waypoints
 */
public class singleWaypoint : MonoBehaviour
{
    public int pointsAwarded = 0;//make sure there is enough colours on the waypoint script
    public bool isShop=false;
    public bool isBlackHole=false;

    private int pos;

    /**
* @memo 2022
* creats planet  or black hole on top of waypoint and sets planet colour if planet
*/
    public void setPlanet(int tempPos)
    {
        pos = tempPos;
        if (!isBlackHole)
        {
            GameObject planetSprite = Instantiate(waypointsScript.instance.planet, this.transform);
            float size = .35f;
            if (pointsAwarded == 0)//means start of game
            {
                size = .7f;
                planetSprite.GetComponent<SpriteRenderer>().color = waypointsScript.instance.planetColours[waypointsScript.instance.planetColours.Length - 1];
            }
            else if (pointsAwarded == -1)//means game end
            {
                size = .85f;
                planetSprite.GetComponent<SpriteRenderer>().color = waypointsScript.instance.planetColours[waypointsScript.instance.planetColours.Length - 1];
            }
            else
            {
                planetSprite.GetComponent<SpriteRenderer>().color = waypointsScript.instance.planetColours[pointsAwarded - 1];
            }
            planetSprite.transform.localScale = new Vector3(size, size, size);
            if (isShop)
            {
                pointsAwarded = 1;//makes sure that shops always only grant 1 point
                planetSprite.GetComponent<SpriteRenderer>().color = waypointsScript.instance.planetColours[waypointsScript.instance.planetColours.Length - 2];
            }
        }
        else//if a black hole
        {
            GameObject blackHole = Instantiate(waypointsScript.instance.blackHolePrefab, this.transform);
            float size = .04f;
            blackHole.transform.localScale = new Vector3(size, size, size);
            blackHole.GetComponent<blackHoleScript>().position = pos;
        }
    }
    /**
* @memo 2022
* disables/enables this black hole
*/
    public void disableEnableBlackHole(bool val)
    {
        if (!isBlackHole) { return; }
        GetComponentInChildren<blackHoleScript>().setActive(val);
    }
    /**
* @memo 2022
* disables / enables this shop
*/
    public void disableEnableShop(bool val)
    {
        if (!isShop) { return; }
        if (val == true)
        {
            transform.GetComponentInChildren<SpriteRenderer>().color= waypointsScript.instance.planetColours[waypointsScript.instance.planetColours.Length - 2];
        }
        else
        {
            transform.GetComponentInChildren<SpriteRenderer>().color = waypointsScript.instance.planetColours[waypointsScript.instance.planetColours.Length - 1];
        }
    }

}
