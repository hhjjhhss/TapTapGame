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
