using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class PlayerMove : Agent
{

    // 1. 플레이어 받아오기

    // 2. 플레이어 좌우 이동
    private float playerSpeed = 5.0f;


    // 3. 공을 발사함

    public Transform ball;
    Rigidbody rigidBall;

    public float shootSpeed = 7.0f;


    // 4. 블럭의 배열을 가지고 있음
    public List<GameObject> blocks;


    // 결과 및 계산 로직
    public Text resultT;
    public int blockCount;


    //5. 원위치
    Vector3 originPlayer;
    Vector3 originBall;


    private bool isPlay = false;

    //가상함수 구현
    public override void InitializeAgent()
    {
        // 공을 랜덤하게 발사함(60도~120도)
        rigidBall = ball.GetComponent<Rigidbody>();

        //blocks = GameObject.FindGameObjectsWithTag("BLOCK");
        blockCount = blocks.Count;

        originPlayer = transform.localPosition;
        originBall = ball.localPosition;

        resultT.gameObject.SetActive(false);

        Debug.Log("block count : " + blockCount);


        Invoke("ShootBall", 1.5f);
    }


    public override void CollectObservations()
    {
        // 1. 나의 위치
        AddVectorObs(transform.localPosition);
           
        // 2. 공의 위치
        AddVectorObs(ball.localPosition);

        // 3. 공의 속도(크기와 방향)
        AddVectorObs(rigidBall.velocity);
    }



    public override void AgentAction(float[] vectorAction, string textAction)
    {

        CheckBlock();
        AddReward(0.01f);

        // 뒤에 normalize 를 안 쓸 경우에 대비하여 clamp 를 걸어 범위 제한해준다.
        float h = Mathf.Clamp(vectorAction[0], -1f, 1f);

        Vector3 direction = new Vector3(h, 0, 0).normalized;


        transform.position += direction * playerSpeed * Time.deltaTime;



        //좌우 한계 설정 -4.5 ~ 4.5 로 제한

        Vector3 myPosition = transform.position;
        myPosition.x = Mathf.Clamp(myPosition.x, -4.5f, 4.5f);

        transform.position = myPosition;

    }

    void ShootBall()
    {
     
            float degree = Random.Range(60f, 120f);
            Vector3 shootD = new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad),
                                         Mathf.Sin(degree * Mathf.Deg2Rad), 0);

            rigidBall.velocity = shootD * shootSpeed;

        isPlay = true;
    }


   
    //게임 클리어 또는 아웃 체크
    public void CheckBlock()
    {

        if (!isPlay) return;


        // 게임 클리어
        if(blockCount <= 0)
        {
            isPlay = false;
            CheckResult("Game Clear");

            AddReward(15f);
            
        }

        // 게임 오버
        else if(ball.transform.position.y < transform.position.y)
        {
            isPlay = false;
            CheckResult("Game Over");

            AddReward(-3.0f);
        }

    }


    public void CheckResult(string result)
    {

        resultT.gameObject.SetActive(true);

        // UI로 결과출력
        resultT.text = result;

        // 1.5f 뒤에 블럭과 플레이어, 공을 원위치 시킨다.
        //Agent reset 함수를 실행시키면서 외부 tensor flow에게 리셋 되었다는 이벤트와 상벌점 결과를 전달하는 것
        // Done();
        Invoke("Done", 1.5f);

       
        
    }

    public override void AgentReset()
    {
        //1.블록 원위치
        foreach(GameObject block in blocks)
        {
            block.SetActive(true);
        }

        //2. 공 위치, 플레이어 위치 원위치

        transform.localPosition = originPlayer;
        ball.localPosition = originBall;
        rigidBall.velocity = Vector3.zero;


        resultT.gameObject.SetActive(false);

        // 블록 카운트 추가
        blockCount = blocks.Count;

        Invoke("ShootBall", 1.5f);

    }

}
