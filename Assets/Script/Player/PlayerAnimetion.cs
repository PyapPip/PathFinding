using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimetion : MonoBehaviour
{
    //플레이어 움직임, 애니메이션

    public float[] JumpPosArr = { 0.1f, 0.3f, 0.65f, 0.9f, 1.0f};
    public bool isAnimating;       //애니메이션이 재생중인가?

    private Queue<int> movement_route = new();
    private bool haveRoute;
    private int inAni;          //애니메이션 현재 프레임
    private int rotAni = 3;     //애니메이션의 방향
    private Vector3 startPos;   //애니메이션 시작 지점
    private Vector3 endPos;     //애니메이션 끝 지점

    /// <summary>
    /// 경로를 받아오기위한 함수
    /// </summary>
    public void FollowRoute(List<int> _List)
    {
        movement_route.Clear();

        for (int i = _List.Count - 1; i >= 0; i--)
        {
            haveRoute = true;
            movement_route.Enqueue(_List[i]);
        }
    }

    public void NavigationError()
    {
        movement_route.Clear();
        StartCoroutine(Fail());
    }

    void Start()
    {
        inAni = -1;
    }


    void Update()
    {
        if (haveRoute)
        {
            StartCoroutine(PlayAni());
            haveRoute = false;
        }
    }

    /// <summary>
    /// 애니메이션 재생
    /// </summary>
    IEnumerator PlayAni()
    {
        StartCoroutine(Waiting());
        while (isAnimating) { yield return null; }

        while (true)
        {
            if (movement_route.Count == 0)
            {
                StartCoroutine(Waiting());
                while (!isAnimating) { yield return null; }
                this.GetComponent<Animator>().Play("player_idle_" + rotAni.ToString());
                yield break;
            }

            if (inAni == 32)
                inAni = 0;

            rotAni = movement_route.Dequeue();

            //시작좌표, 끝 좌표 저장
            startPos = this.transform.position;

            if (rotAni == 1)
            {
                endPos = startPos + new Vector3(0, 0, 0.3f);
                GetComponent<Player>().Y--;
            }
            else if (rotAni == 2)
            { 
                endPos = startPos + new Vector3(0.3f, 0, 0);
                GetComponent<Player>().X--;
            }
            else if (rotAni == 3)
            {
                endPos = startPos + new Vector3(-0.3f, 0, 0);
                GetComponent<Player>().X++;

            }
            else if (rotAni == 4)
            {
                endPos = startPos + new Vector3(0, 0, -0.3f);
                GetComponent<Player>().Y++;
            }



            //애니메이션 재생

            if (movement_route.Peek() == 0)
            {
                movement_route.Dequeue();
                this.GetComponent<Animator>().Play("player_walk_" + rotAni.ToString(), -1, inAni * (1.0f / 32.0f));

                isAnimating = true;
                StartCoroutine(Wark());
            }
            else
            {
                GetComponent<Player>().Z += movement_route.Peek();
                endPos = new Vector3(endPos.x, endPos.y + movement_route.Dequeue() * 0.1f, endPos.z);
                this.GetComponent<Animator>().Play("player_jump_" + rotAni.ToString());

                isAnimating = true;
                StartCoroutine(Jump());
            }

            

            while (isAnimating)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// 걷기
    /// </summary>
    IEnumerator Wark()
    {
        while (true)
        {
            //16프레임마다 한 타일 움직임 구현
            transform.position = Vector3.Lerp(startPos, endPos, (inAni % 16) * (1.0f / 16.0f));

            inAni++;
            if (inAni % 16 == 0 && inAni != 0)
            {
                transform.position = endPos;
                isAnimating = false;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Jump()
    {
        int c = 0;
        while (true)
        {
            c++;

            if(c >= 20 && c <= 29)
            {
                //점프
                if(c < 26)
                {
                    transform.position = Vector3.Lerp(startPos, new Vector3(this.transform.position.x, startPos.y + 0.1f, this.transform.position.z), JumpPosArr[c-20]);
                }

                startPos = new Vector3(startPos.x, this.transform.position.y, startPos.z);

                //착지
                if(c >= 26)
                {
                    transform.position = Vector3.Lerp(startPos, new Vector3(this.transform.position.x, endPos.y, this.transform.position.z), JumpPosArr[c-20]);
                }

                transform.position = Vector3.Lerp(startPos, new Vector3(endPos.x, this.transform.position.y, endPos.z), (c - 20) * (1.0f / 9));

            }
            
            else if(c >= 38)
            {
                isAnimating = false;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 대기상태
    /// </summary>
    IEnumerator Waiting()
    {
        isAnimating = true;
        int c = 20;

        while (true)
        {
            c--;
            this.GetComponent<Animator>().Play("player_idle_" + rotAni.ToString(), -1, 0.9f);

            if (c <= 0)
            {
                isAnimating = false;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 길찾기 실패, 갈 수 없는 땅을 클릭했을때.
    /// </summary>
    IEnumerator Fail()
    {
        isAnimating = true;
        int c = 60;
        this.GetComponent<Animator>().Play("player_fail_" + rotAni.ToString());

        while (true)
        {
            c--;

            if (c <= 0)
            {
                isAnimating = false;
                yield break;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator FPS()
    {
        while (true)
        {
            Debug.Log(Time.frameCount);
            yield return new WaitForSeconds(1);
        }
    }
}
