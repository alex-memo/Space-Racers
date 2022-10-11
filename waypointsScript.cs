using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
 * @memo 2022
 * simple waypoint holder script
 */
public class waypointsScript : MonoBehaviour
{
    /**
 * @memo 2022
 * Creates instance of this
 */
    public static waypointsScript instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public GameObject planet;
    public Color[] planetColours;
    public List<Transform>waypoints;
    public List<Transform> blackholes;
    public List<Transform> shops;
    public GameObject blackHolePrefab;
    /**
 * @memo 2022
 * Adds every waypoint to the list and sets the planets
 */
    private void Start()
    {
        int i = 0;
        foreach(Transform child in this.transform)
        {
            waypoints.Add(child);
            singleWaypoint tempWaypoint = child.GetComponent<singleWaypoint>();
            tempWaypoint.setPlanet(i);
            if (tempWaypoint.isBlackHole)
            {
                blackholes.Add(child);
            }
            if (tempWaypoint.isShop)
            {
                shops.Add(child);               
            }
            i++;
        }
    }
    /**
* @memo 2022
* Disables/enables black holes locally
*/
    public void disableEnableBlackHoles(bool var)
    {
        foreach(Transform bh in blackholes)
        {
            bh.GetComponent<singleWaypoint>().disableEnableBlackHole(var);
        }
    }
    /**
* @memo 2022
* Disables/enables shops locally
*/
    public void disableEnableShops(bool var)
    {
        foreach (Transform bh in shops)
        {
            bh.GetComponent<singleWaypoint>().disableEnableShop(var);
        }
    }

}
