using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    //每一个格子节点
    public class Node
    {
        public int x;
        public int y;
        public int gCost;
        public int hCost;
        public bool Walkable;//用来表示是否可以走
        public Node parent;
        public Node(int _x, int _y, bool _Walkable)
        {
            x = _x;
            y = _y;
            Walkable = _Walkable;
        }
        public int fCost
        { get { return gCost + hCost; } }
    }
    public Vector3 MousePosition;
    public Vector3 Target;
    public Vector2 TurnPoint;
    public Vector2 PlayerDirection;
    public Ray2D DetectRay;
    public RaycastHit2D m_RaycastHit;
    public bool OnMove = false;
    public int CurrentPathIndex = 0;
    public float Speed = 5.0f;
    private Node[,] grid;
    private Vector2Int gridSize;
    List<Vector2Int> path = new List<Vector2Int>();
    private LayerMask ObstacleMask;//障碍层，之后再设置
    private void Start()
    {
        InitGrid(new Vector2Int(20, 20));
    }
    void Update()
    {
        SetTarget();
        Move();
    }
    public void SetTarget()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //把鼠标坐标转换为世界坐标
            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //发射射线
            m_RaycastHit = Physics2D.Raycast(MousePosition, transform.forward, 1.5f);
            if (m_RaycastHit.collider != null)
            {
                Debug.Log(m_RaycastHit.collider.gameObject.name);
            }
            else
            {
                Target = new Vector3(MousePosition.x, MousePosition.y, MousePosition.z);
                path = FindPath(transform.position, Target);
                if (path != null && path.Count > 0)
                {
                    CurrentPathIndex = 0;
                    Target = new Vector3(path[0].x, path[0].y, 0);
                    OnMove = true;
                }
            }
        }
    }
    public void Move()
    {
        if (OnMove)
        {
            Vector2.MoveTowards(transform.position, Target, 5.0f * Time.deltaTime);
            Debug.Log(path.Count);
            if (Vector3.Distance(transform.position, Target) < 0.05f)
            {
                // 到达当前路径点，更新目标路径点
                CurrentPathIndex++;
                if (CurrentPathIndex >= path.Count)
                {
                    OnMove = false;
                }
                else
                {
                    Target = new Vector3(path[CurrentPathIndex].x, path[CurrentPathIndex].y, 0f);
                }
            }
        }
    }
    public void InitGrid(Vector2Int size)
    {
        gridSize = size;
        CreateGrid();
    }
    //新建网格地图
    public void CreateGrid()
    {
        grid = new Node[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPoint = new Vector3(x, y, 0);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, 0.1f, ObstacleMask);
                grid[x, y] = new Node(x, y, walkable);
            }
        }
    }
    public List<Vector2Int> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node targetNode =new Node(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y), true);
        Node startNode = new Node(Mathf.RoundToInt(startPos.x),Mathf.RoundToInt(startPos.y), true);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.Walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // 如果找不到路径，返回空
    }

    private List<Vector2Int> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(new Vector2Int(currentNode.x, currentNode.y));
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.x - nodeB.x);
        int distY = Mathf.Abs(nodeA.y - nodeB.y);
        return distX + distY;
    }
}
