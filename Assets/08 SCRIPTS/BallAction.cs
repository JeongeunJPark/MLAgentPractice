using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAction : MonoBehaviour
{
    public Transform player;

    PlayerMove pM;

    private void Start()
    {
       pM =  player.GetComponent<PlayerMove>();
    }



    //부딪힌 대상의 태그가 block 이면 setActive(false);
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "BLOCK")
        {
            pM.blockCount--;
            collision.gameObject.SetActive(false);
        }
    }


}
