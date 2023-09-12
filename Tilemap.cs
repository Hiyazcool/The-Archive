using UnityEngine;
using Unity.Mathematics;

public static class Tilemap
{
    /*
     * Finish Implementing Mainly 
     * Verify Threadsafe, and Burst Compatible
     * Implement Serializable?
     */
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
        public Pathfinding.PathNode pathNode;
        public Pathfinding.PathNode GetPathNode()
        {
            return pathNode;
        }
        public override string ToString()
        {
            return tileType.ToString() + tileVariant;
        }
    }
    public struct Map
    {
        public TilemapObject[,] TileMap;
        public Pathfinding.PathNode GetTilePathNode(int2 position)
        {
            return TileMap[position.x,position.y].pathNode;
        }
        public Pathfinding.PathNode GetTilePathNode(int x, int y)
        {
            return TileMap[x,y].pathNode;
        }
    }
    
}