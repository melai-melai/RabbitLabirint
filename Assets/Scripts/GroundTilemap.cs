using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundTilemap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TileHasCollider(Vector3Int position)
    {
        TileBase tile = gameObject.GetComponent<Tilemap>().GetTile(position);
        Debug.Log(tile);

    }
}
