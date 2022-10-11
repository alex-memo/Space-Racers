using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{
    public int camSpeed = 20;
    // Update is called once per frame
    /**
 * @memo 2022
 * simple camera movement script for league lke camera
 */
    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.mousePosition.y >= Screen.height - 40 &&  pos.y <= 55)
        {
            pos.y += camSpeed * Time.deltaTime;
        }
        if(Input.mousePosition.y <= 40&&pos.y>=0)
        {
            pos.y -= camSpeed * Time.deltaTime;
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            print(Input.mousePosition.y+" "+Screen.height);
        }
        transform.position = pos;
    }
}
