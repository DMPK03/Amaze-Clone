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
        public LevelType Type;
        
        public enum LevelType 
        {
            Level = 0,
            Challenge = 1,
            LimitedTurn = 2,
            TimeTrial = 3
        }
    }

    
    [Serializable]
    public class SavedTile
    {
        public Vector3Int Position;
        public Tile Tile;
    }

    [Serializable]
    public class Colors
    {
        public Color BallColor;
        public Color TileColor;
    }
}