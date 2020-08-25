using RabbitLabirint;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Highlight : MonoSingleton<Highlight>
{
    public TileBase highlightTile;
    public Tilemap highlightMap;

    [SerializeField]
    private TileBase startHorizontalTile;
    [SerializeField]
    private TileBase horizontalTile;
    [SerializeField]
    private TileBase endHorizontalTile;
    [SerializeField]
    private TileBase startVerticalTile;
    [SerializeField]
    private TileBase verticalTile;
    [SerializeField]
    private TileBase endVerticalTile;

    //private Vector3Int prevTileCoordinate;
    private Vector3Int startCoordinate;
    private Vector3Int endCoordinate;

    public Vector3 Target
    {
        get
        {
            return (Vector3) endCoordinate;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //prevTileCoordinate = Vector3Int.zero;
        startCoordinate = PlayerController.Instance.Coordinate;
        endCoordinate = startCoordinate;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.Instance.IsMoving)
        {
            DrawLine();
        }
    }

    //void OnMouseOver()
    //{
        //If your mouse hovers over the GameObject with the script attached, output this message
        //Debug.Log("Mouse is over GameObject.");

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray))
        //{
        //    Debug.Log(ray.ToString());
        //}
    //}

    void HighlightTile()
    {
        Vector3Int tileCoordinate = GetCoordinateUnderMouse();

        //if (tileCoordinate != prevTileCoordinate)
        //{
            //highlightMap.SetTileFlags(tileCoordinate, TileFlags.None);
            //highlightMap.SetColor(tileCoordinate, new Color(0,0,0, 0.42f));
            //highlightMap.SetTile(prevTileCoordinate, null);
            highlightMap.SetTile(tileCoordinate, highlightTile);
            //Debug.Log(tileCoordinate);
            
            //prevTileCoordinate = tileCoordinate;
        //}
    }

    void DrawLine()
    {
        startCoordinate = PlayerController.Instance.Coordinate;
        endCoordinate = GetEndCoordinateTarget();

        if (startCoordinate.Equals(endCoordinate))
        {
            highlightMap.ClearAllTiles();
            //
            
            HighlightTile();
            return;
        }

        //TileHasCollider(startCoordinate);
        //TileHasCollider(endCoordinate);

        if (startCoordinate.x == endCoordinate.x)
        {
            highlightMap.ClearAllTiles();
            // along the Y axis
            if (startCoordinate.y < endCoordinate.y)
            {
                int minY = startCoordinate.y;
                int maxY = endCoordinate.y;

                for (int i = minY; i <= maxY; i++)
                {
                    if (i == minY)
                    {
                        highlightMap.SetTile(new Vector3Int(startCoordinate.x, i, 0), endVerticalTile);
                    }
                    else if (i == maxY)
                    {
                        highlightMap.SetTile(new Vector3Int(startCoordinate.x, i, 0), startVerticalTile);
                    }
                    else
                    {
                        highlightMap.SetTile(new Vector3Int(startCoordinate.x, i, 0), verticalTile);
                    }
                }
            }
            else
            {
                int minY = endCoordinate.y;
                int maxY = startCoordinate.y;

                for (int i = maxY; i >= minY; i--)
                {
                    if (i == minY)
                    {
                        highlightMap.SetTile(new Vector3Int(startCoordinate.x, i, 0), endVerticalTile);
                    }
                    else if (i == maxY)
                    {
                        highlightMap.SetTile(new Vector3Int(startCoordinate.x, i, 0), startVerticalTile);
                    }
                    else
                    {
                        highlightMap.SetTile(new Vector3Int(startCoordinate.x, i, 0), verticalTile);
                    }
                }
            }            
        }
        else if (startCoordinate.y == endCoordinate.y)
        {
            highlightMap.ClearAllTiles();
            // along the X axis
            if (startCoordinate.x < endCoordinate.x)
            {
                int minX = startCoordinate.x;
                int maxX = endCoordinate.x;

                for (int i = minX; i <= maxX; i++)
                {
                    if (i == minX)
                    {
                        highlightMap.SetTile(new Vector3Int(i, startCoordinate.y, 0), startHorizontalTile);
                    }
                    else if (i == maxX)
                    {
                        highlightMap.SetTile(new Vector3Int(i, startCoordinate.y, 0), endHorizontalTile);
                    }
                    else
                    {
                        highlightMap.SetTile(new Vector3Int(i, startCoordinate.y, 0), horizontalTile);
                    }
                }
            }
            else
            {
                int minX = endCoordinate.x;
                int maxX = startCoordinate.x;

                for (int i = maxX; i >= minX; i--)
                {
                    if (i == minX)
                    {
                        highlightMap.SetTile(new Vector3Int(i, startCoordinate.y, 0), startHorizontalTile);
                    }
                    else if (i == maxX)
                    {
                        highlightMap.SetTile(new Vector3Int(i, startCoordinate.y, 0), endHorizontalTile);
                    }
                    else
                    {
                        highlightMap.SetTile(new Vector3Int(i, startCoordinate.y, 0), horizontalTile);
                    }
                }
            }
        }
    }

    Vector3Int GetCoordinateUnderMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tileCoordinate = highlightMap.WorldToCell(mouseWorldPos);
        return tileCoordinate;
    }

    Vector3Int GetEndCoordinateTarget()
    {
        Vector3Int mouseTileCoordinate = GetCoordinateUnderMouse();
        Vector3Int playerTileCoordinate = PlayerController.Instance.Coordinate;
        Vector3Int targetTileCoordinate = Vector3Int.zero;

        int deltaX = Math.Abs(mouseTileCoordinate.x - playerTileCoordinate.x);
        int deltaY = Math.Abs(mouseTileCoordinate.y - playerTileCoordinate.y);

        if (deltaX <= deltaY)
        {
            targetTileCoordinate.x = playerTileCoordinate.x;
            targetTileCoordinate.y = mouseTileCoordinate.y;
        } else if (deltaX > deltaY)
        {
            targetTileCoordinate.x = mouseTileCoordinate.x;
            targetTileCoordinate.y = playerTileCoordinate.y;
        }

        return targetTileCoordinate;
    }

    void TileHasCollider(Vector3Int position)
    {
        TileBase tile = GameObject.Find("Tilemap").GetComponent<Tilemap>().GetTile(position);
        Debug.Log(tile);

    }


}
