using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AStarPathfinding pathfinding;
    public CharacterController2D character;

    private void Start()
    {
        pathfinding.InitializeGrid(new Vector2Int(100, 100)); // ���õ�ͼ��С

        // ������Ը������������ϰ����

    }
}