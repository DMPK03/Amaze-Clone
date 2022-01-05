using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DM
{
    public class Level : ScriptableObject
    {
        public int LevelIndex;
        public List<SavedTile> GroundTiles;
        public List<SavedTile> WallTiles;
    }

    
    [Serializable]
    public class SavedTile
    {
        public Vector3Int Position;
        public Tile Tile;
    }
}