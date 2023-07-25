using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimetion : MonoBehaviour
{
    //�÷��̾� ������, �ִϸ��̼�

    public float[] JumpPosArr = { 0.1f, 0.3f, 0.65f, 0.9f, 1.0f};
    public bool isAnimating;       //�ִϸ��̼��� ������ΰ�?

    private Queue<int> movement_route = new();
    private bool haveRoute;
    private int inAni;          //�ִϸ��̼� ���� ������
    private int rotAni = 3;     //�ִϸ��̼��� ����
    private Vector3 startPos;   //�ִϸ��̼� ���� ����
    private Vector3 endPos;     //�ִϸ��̼� �� ����

    /// <summary>
    /// ��θ� �޾ƿ������� �Լ�
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
    /// �ִϸ��̼� ���
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

            //������ǥ, �� ��ǥ ����
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



            //�ִϸ��̼� ���

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
    /// �ȱ�
    /// </summary>
    IEnumerator Wark()
    {
        while (true)
        {
            //16�����Ӹ��� �� Ÿ�� ������ ����
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
                //����
                if(c < 26)
                {
                    transform.position = Vector3.Lerp(startPos, new Vector3(this.transform.position.x, startPos.y + 0.1f, this.transform.position.z), JumpPosArr[c-20]);
                }

                startPos = new Vector3(startPos.x, this.transform.position.y, startPos.z);

                //����
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
    /// ������
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
    /// ��ã�� ����, �� �� ���� ���� Ŭ��������.
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
