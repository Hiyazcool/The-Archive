using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hiyazcool;
using Hiyazcool.Unity;

public class MapHandler : MonoBehaviourSingleton<MapHandler>
{
    /*
     * Todo 
     * Map Generation Low Priority
     * Premade-map loading
     * Ingame Map editor would be cool but VERY low Priority
     * Save/Load Map
     * Map to PathNode Conversion
     * Map class, with Mesh and Tile Information
     */
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public enum TileType
    {
        None,
        Stone,
        Grass,
        Dirt,
        Water,
    }
    public struct TilemapObject
    {
        private short tileVariant;
        private TileType tileType;
        private PathfindingNode pathNode;
        public PathfindingNode GetPathNode()
        {
            return pathNode;
        }
        public override string ToString()
        {  
            return tileType.ToString() + tileVariant;
        }
    }
    public struct PathfindingNode
    {
        private bool isTraversable;
        private float fCost;
        private float hCost;
        private float gCost;
    }
}
