using Unity.VisualScripting;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    //마우스로 타일 클릭

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !this.GetComponent<MapManager>().Player.GetComponent<PlayerAnimetion>().isAnimating)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Tile goal = hit.transform.GetComponent<Tile>();
                this.GetComponent<MapManager>().Path_Finding(goal.x, goal.y, goal.z);
                Debug.Log("Click");
            }
        }
    }
}
