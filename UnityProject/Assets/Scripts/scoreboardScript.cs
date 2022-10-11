using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/**
 * @memo 2022
 * Script to update and create scoreboards
 */
public class scoreboardScript : MonoBehaviour
{
    public TMP_Text roundText;
    public GameObject playerObjPrefab;
    /**
 * @memo 2022
 * Updates the scoreboard
 */
    public void UpdateScores()
    {
        gameManagerScript manager = gameManagerScript.instance;
        roundText.text = "Round: " + manager.round+ "\t"+gameManagerScript.instance.getRoundName()+"'s turn";

        playerScoreScript[] scores= this.GetComponentsInChildren<playerScoreScript>();
        foreach (playerScoreScript score in scores)
        {
            score.updatePlayer();
        }
    }
    /**
 * @memo 2022
 * Creates the scoreboard
 */
    public void createPlayerScoreboard()
    {
        foreach(Transform child in this.transform)
         {
            Destroy(child.gameObject);
         }
        foreach(GameObject player in gameManagerScript.instance.gamePlayers)
        {
            GameObject tempPlayer= Instantiate(playerObjPrefab, this.transform);
            tempPlayer.GetComponent<playerScoreScript>().setPlayer(player.GetComponent<Controller>());
        }
    }
    
}
