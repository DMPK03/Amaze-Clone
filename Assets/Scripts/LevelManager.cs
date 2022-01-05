using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace DM
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Tilemap _groundTilemap, _wallTilemap;
        [SerializeField] private int _levelIndex;

        public delegate void V3E(Vector3 value);
        public static event V3E OnMapLoadedEvent;

        public void LoadNewLevel()
        {
            _levelIndex +=1;
            LoadTilemap();
        }


        public void SaveTilemap()
        {
            var newLevel = ScriptableObject.CreateInstance<Level>();
            newLevel.LevelIndex = _levelIndex;
            newLevel.name = $"Level {_levelIndex}";
            newLevel.GroundTiles = GetTilesFromMap(_groundTilemap);
            newLevel.WallTiles = GetTilesFromMap(_wallTilemap);

            EditorStuff.SaveLevelFile(newLevel);
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

        public void ClearTilemap()
        {
            _groundTilemap.ClearAllTiles();
            _wallTilemap.ClearAllTiles();

        }
        public void LoadTilemap()
        {
            Level level = Resources.Load<Level>($"Levels/Level {_levelIndex}");
            if(level == null){
                Debug.Log($"Level {_levelIndex} does not exist, loading default first level");
                _levelIndex = 0;
                level = Resources.Load<Level>($"Levels/Level {_levelIndex}");
                if(level == null) Debug.LogError("no level 0 found");
            }
            ClearTilemap();


            foreach(SavedTile tile in level.GroundTiles)
            {
                _groundTilemap.SetTile(tile.Position, tile.Tile);
            }
            foreach(SavedTile tile in level.WallTiles)
            {
                _wallTilemap.SetTile(tile.Position, tile.Tile);
            }
            OnMapLoadedEvent?.Invoke(level.GroundTiles[0].Position);
        }
    }

    #if UNITY_EDITOR
    public static class EditorStuff
    {
        public static void SaveLevelFile(Level level)
        {
            AssetDatabase.CreateAsset(level, $"Assets/Resources/Levels/{level.name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    #endif

}