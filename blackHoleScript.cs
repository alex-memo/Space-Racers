using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @Denzil 2022
 * 
 */
public class blackHoleScript : MonoBehaviour
{
    public bool activeOnStart = true;
    bool active;
    public int position;
    public Color activeColor;
    public Color inActiveColor = Color.gray;

    private void Start()
    {
        setActive(activeOnStart);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && active)
        {
            Controller playerController = collision.gameObject.GetComponent<Controller>();
            if (!playerController.getHasToMove() && !playerController.getForcedMovement()&&!playerController.getHasToMoveBack() && !playerController.trapped && playerController.getPlayerPosition() != position)
            {
                playerController.trapped = true;
                playerController.canMove = false;
                playerController.trapPosition = position;
                playerController.setPlayerPosition(position);
            }
        }
    }

    public void setActive(bool active)
    {       
        if (this.active == active)
        {
            return;
        }
        this.active = active;
        if (active)
        {
            GetComponent<ParticleSystem>().Play();
            GetComponent<SpriteRenderer>().color = new Color(activeColor.r, activeColor.g, activeColor.b, 1);
        }
        else
        {
            GetComponent<ParticleSystem>().Stop();
            GetComponent<SpriteRenderer>().color = new Color(inActiveColor.r, inActiveColor.g, inActiveColor.b, 0.5f);
        }
    }
}
