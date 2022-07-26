using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debuger : MonoBehaviour
{
    public Text txt;
    public Text txt2;
    PuzzleGenerator_Runtime puz;
    PuzzleController puzCon;
    

    // Start is called before the first frame update
    void Start()
    {
        puzCon = GetComponent<PuzzleController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(puzCon.placed);
        //txt.text = puzCon.placed.ToString();
        //txt2.text = puzCon.placed.ToString();

        if (puzCon.placed == false)
        {
            txt.text = "0";
            //txt2.text = puzCon._pointerPosition.x.ToString("00");
        }
        if (puzCon.placed == true)
        {
            txt.text = "1";
            //txt2.text = puzCon._pointerPosition.x.ToString("00");
        }
        if (puzCon.currentObject.GetComponent<PlacedBool>().placed == false)
        {
            txt2.text = "2";
            //txt2.text = puzCon._pointerPosition.x.ToString("00");
        }
        if (puzCon.currentObject.GetComponent<PlacedBool>().placed == true)
        {
            txt2.text = "3";
            //txt2.text = puzCon._pointerPosition.x.ToString("00");
        }
        //txt.text = PlayerPrefs.GetInt("touchCount");

    }
}
