using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace DM
{
    public class LevelManager : MonoBehaviour
    {
        public static event Action<Level> OnLevelLoadedEvent;
        
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] Tile _coloredTile;
        [SerializeField] Colors[] ColorArray;

        [SerializeField] private int _levelIndex;   //used in editor for saving/loading maps
        [SerializeField] LevelType _levelType;    //used in editor for saving/loading maps
        

        public void SaveTilemap()   //editor only
        {
            var newLevel = ScriptableObject.CreateInstance<Level>();
            newLevel.LevelIndex = _levelIndex;
            newLevel.Type = _levelType;
            newLevel.name = $"Level {_levelIndex}";
            newLevel.GroundTiles = GetTilesFromMap(_groundTilemap);

            //EditorStuff.SaveLevelFile(newLevel);
        }

        public void ClearTilemap()
        {
            _groundTilemap.ClearAllTiles();
        }
        
        public void LoadTilemap()   //editor only
        {
            Level level = Resources.Load<Level>($"{_levelType}s/Level {_levelIndex}");
            if(level == null){
                Debug.Log($"Level {_levelIndex} does not exist, loading default first level");
                level = Resources.Load<Level>($"{_levelType}s/Level {0}");
                if(level == null) Debug.LogError("no levels found");
            }

            ClearTilemap();

            foreach(SavedTile tile in level.GroundTiles)
            {
                _groundTilemap.SetTile(tile.Position, tile.Tile);
            }
        }

        public void LoadTilemap(LevelType type, int index)
        {
            Level level = Resources.Load<Level>($"{type}s/Level {index}");
            if(level == null) level = Resources.Load<Level>($"{type}s/Level {1}");

            ClearTilemap();
            GetRandomColors();

            foreach(SavedTile tile in level.GroundTiles)
            {
                _groundTilemap.SetTile(tile.Position, tile.Tile);
            }
            OnLevelLoadedEvent?.Invoke(level);
        }

        private List<SavedTile> GetTilesFromMap(Tilemap map)
        {
            List<SavedTile> tileList = new List<SavedTile>();
            foreach (var pos in map.cellBounds.allPositionsWithin)
            {
                if(map.HasTile(pos)){
                    tileList.Add(new SavedTile(){
                    Position = pos,
                    Tile = map.GetTile<Tile>(pos)
                    });
                }
            }
            return tileList;
        }

        public void GetRandomColors()
        {
            int x = UnityEngine.Random.Range(0, ColorArray.Length);
            if(ColorArray[x] != null) _coloredTile.color = ColorArray[x].TileColor;
        }
    }

    #if UNITY_EDITOR
    public static class EditorStuff
    {
        public static void SaveLevelFile(Level level)
        {
            AssetDatabase.CreateAsset(level, $"Assets/Resources/{level.Type}s/{level.name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    #endif

}