using UnityEngine;
using System;
using System.Collections.Generic;
public class CharacterController2D : MonoBehaviour
{
    public float speed = 5f;
    public AStarPathfinding pathfinding;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private int currentPathIndex = 0;
    private float pathfindingUpdateInterval = 0.5f;
    private float lastPathfindingUpdateTime;

    public List<Vector2Int> path;
    private void Start()
    {
        lastPathfindingUpdateTime = Time.time;
        targetPosition = transform.position;
    }

    private void Update()
    {
        HandleInput();

        if (isMoving)
        {
            Move();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0f);

            // 调用A*寻路
            if (Time.time - lastPathfindingUpdateTime > pathfindingUpdateInterval)
            {
                lastPathfindingUpdateTime = Time.time;
                path = pathfinding.FindPath(transform.position, targetPosition);
                if (path != null && path.Count > 0)
                {
                    currentPathIndex = 0;
                    targetPosition = new Vector3(path[0].x, path[0].y, 0f);
                    isMoving = true;
                }
            }
        }
    }

    private void Move()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            // 到达当前路径点，更新目标路径点
            currentPathIndex++;
            if (currentPathIndex >= path.Count)
            {
                isMoving = false;
            }
            else
            {
                targetPosition = new Vector3(path[currentPathIndex].x, path[currentPathIndex].y, 0f);
            }
        }
    }
}