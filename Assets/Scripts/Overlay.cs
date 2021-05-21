using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    //Image wKey; 
    Image aKey;// these are the keys they are hard coded. Make sure that if you change the canvas order that you change the order of the keys.
    //Image sKey;
    Image dKey;
    Image mouse;
    Image space;
    Options optionsMenu;


    // Start is called before the first frame update
    void Start()
    {
        optionsMenu = FindObjectOfType<Options>();
        aKey = transform.GetChild(0).GetComponent<Image>();
        dKey = transform.GetChild(1).GetComponent<Image>();
        mouse = transform.GetChild(3).GetComponent<Image>();
        space = transform.GetChild(2).GetComponent<Image>();
    }


    public void SetAColor(Color color)
    {
        aKey.color = color;
    }

    public void SetDColor(Color color)
    {
        dKey.color = color;
    }

    public void SetMouseColor(Color color)
    {
        mouse.color = color;
    }

    public void SetSpaceColor(Color color)
    {
        space.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!optionsMenu.isPaused())
        {
            if (Input.GetKeyDown("a"))
            {
                SetAColor(Color.grey);
            }
            else if (Input.GetKeyUp("a"))
            {
                SetAColor(Color.white);
            }

            if (Input.GetKeyDown("d"))
            {
                SetDColor(Color.grey);
            }
            else if (Input.GetKeyUp("d"))
            {
                SetDColor(Color.white);
            }

            if (Input.GetMouseButtonDown(0))
            {
                SetMouseColor(Color.grey);
            }

            else if (Input.GetMouseButtonUp(0))
            {
                SetMouseColor(Color.white);
            }

            if (Input.GetKeyDown("space"))
            {
                SetSpaceColor(Color.grey);
            }
            else if (Input.GetKeyUp("space"))
            {
                SetSpaceColor(Color.white);
            }
        }
          

    }
}
