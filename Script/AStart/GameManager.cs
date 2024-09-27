using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AStarPathfinding pathfinding;
    public CharacterController2D character;

    private void Start()
    {
        pathfinding.InitializeGrid(new Vector2Int(100, 100)); // 设置地图大小

        // 这里可以根据需求设置障碍物等

    }
}