using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class KKProgressBar : MonoBehaviourPun
{
    public int max;
    public int current;
    //Gets the image that fills up the basic attack bar
    public Image progressbar;
    public KarateKid basicattack;

    // Start is called before the first frame update
    void Start()
    {
        basicattack = GameObject.FindGameObjectWithTag("Player").GetComponent<KarateKid>();
    }

    // Update is called once per frame
    void Update()
    {
        basicattack = GameObject.FindGameObjectWithTag("Player").GetComponent<KarateKid>();
        //If the current fill is less than the maximum value (The bar is not full) and the countdown is more than 0 (The move is recharging)
        if (basicattack.timer > 0)
        {
            //Percentage of the timer for the progress bar
            float percent = basicattack.timer / basicattack.cooldown;
            //Fills the progress bar starting from the bottom (Because Lerp is (1,0) if it was (0,1) then progress bar will start from the top and go down)
            progressbar.fillAmount = Mathf.Lerp(1, 0, percent);

        }
        else if (basicattack.timer <= 0)
        {
            return;
        }
    }

}

