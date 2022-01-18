using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DM
{
    public class Level : ScriptableObject
    {
        public List<SavedTile> GroundTiles;        
        public LevelType Type;
        public int LevelIndex, AllowedMoves;
        public Vector3 CameraPosition;
        public float CameraSize;
    }
    
    public enum LevelType 
    {
        Level = 0,
        Challenge = 1,
        LimitedTurn = 2,
        TimeTrial = 3
    }

    [Serializable]
    public class SavedTile
    {
        public Vector3Int Position;
        public Tile Tile;
    }

}