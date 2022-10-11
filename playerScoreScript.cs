using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;
/**
 * @memo 2022
 * Simple player score ui script
 */
public class playerScoreScript : MonoBehaviour
{
    Controller player;
    public RawImage pfp=null;
    public TMP_Text nameText;
    public TMP_Text scoreText;
    /**
 * @memo 2022
 * sets the player on the ui
 */
    public void setPlayer(Controller playerAssigned)
    {
        player = playerAssigned;
        nameText.text = player.steamName;
        print(player.steamName);
        int imageID = SteamFriends.GetLargeFriendAvatar((CSteamID)player.playerSteamID);
        if (imageID == -1) { return; }
        pfp.texture = GetSteamImageAsTexture(imageID);
        updatePlayer();
        //somehow get this player steam icon
    }
    /**
* @memo 2021
* gets the steam pfp as texture
*/
    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        //AvatarReceived = true;
        return texture;
    }
    /**
 * @memo 2022
 * updates teh player score ui
 */
    public void updatePlayer()
    {
        scoreText.text = "Score: " + player.score;
        nameText.text = player.steamName;
        //print(player.steamName);
        int imageID = SteamFriends.GetLargeFriendAvatar((CSteamID)player.playerSteamID);
        print(imageID);
        if (imageID == -1) { return; }
        pfp.texture = GetSteamImageAsTexture(imageID);
    }
}
