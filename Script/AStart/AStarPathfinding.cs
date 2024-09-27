<<<<<<< HEAD:Script/Player.cs
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 MousePosition;
    public Vector2 PlayerDirection;
    public Ray2D DetectRay;
    public RaycastHit2D m_RaycastHit;
    public float Speed = 5.0f;
    private int CurIndex = 0;
    private Seeker m_seeker;
    private List<Vector3> m_pathPoint;
    private LayerMask ObstacleMask;//障碍层，之后再设置
    private void Awake()
    {
        m_seeker = GetComponent<Seeker>();

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
            MousePosition.z = 0;
            //发射射线
            m_RaycastHit = Physics2D.Raycast(MousePosition, transform.forward, 1.5f);
            if (m_RaycastHit.collider != null)
            {
                Debug.Log(m_RaycastHit.collider.gameObject.name);
            }
            else
            {
                CreatePath(MousePosition);
            }
        }
    }
    public void Move()
    {
        if (m_pathPoint != null)
        {
            if (Vector3.Distance(transform.position, m_pathPoint[CurIndex]) < 0.4f)
            {
                CurIndex++;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, m_pathPoint[CurIndex], 5.0f * Time.deltaTime);
            }
        }
    }
    public void CreatePath(Vector3 target)
    {
        CurIndex = 0;
        m_seeker.StartPath(transform.position, target, path =>
        {
            m_pathPoint = new List<Vector3>(path.vectorPath);
        });
    }
}
=======
using UnityEngine;
using System.Collections.Generic;

public class AStarPathfinding : MonoBehaviour
{
    // 定义一个节点类来表示地图中的每个格子
    private class Node
    {
        public int x;
        public int y;
        public bool walkable;
        public int gCost;
        public int hCost;
        public Node parent;

        public Node(int _x, int _y, bool _walkable)
        {
            x = _x;
            y = _y;
            walkable = _walkable;
        }

        public int fCost
        {
            get { return gCost + hCost; }
        }
    }

    public LayerMask obstacleMask; // 障碍物层

    private Node[,] grid;
    private Vector2Int gridSize;

    public void InitializeGrid(Vector2Int size)
    {
        gridSize = size;
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPoint = new Vector3(x, y, 0);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, 0.1f, obstacleMask); // 检测当前格子是否可行走
                grid[x, y] = new Node(x, y, walkable);
            }
        }
    }

    public List<Vector2Int> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = GetNodeFromWorldPoint(startPos);
        Node targetNode = GetNodeFromWorldPoint(targetPos);

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
                if (!neighbor.walkable || closedSet.Contains(neighbor))
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

    private Node GetNodeFromWorldPoint(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.y);
        return grid[x, y];
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
>>>>>>> c2deb570a6cfbd3e244bceac55ca3e7d8fd5a23f:Script/AStart/AStarPathfinding.cs
