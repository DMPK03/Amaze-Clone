using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace DM
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private int _levelIndex;

        [SerializeField] Colors[] ColorArray;
        [SerializeField] SpriteRenderer _ballSprite;
        [SerializeField] Tile _coloredTile;

        public delegate void V3E(Vector3 value);
        public static event V3E OnLevelLoadedEvent;

        public void LoadNewLevel()
        {
            //check for adds here, if no adds ready load next level

            _levelIndex +=1;
            GetRandomColors();
            LoadTilemap();
        }


        public void SaveTilemap()   //editor only
        {
            var newLevel = ScriptableObject.CreateInstance<Level>();
            newLevel.LevelIndex = _levelIndex;
            newLevel.name = $"Level {_levelIndex}";
            newLevel.GroundTiles = GetTilesFromMap(_groundTilemap);

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
        }
        
        public void LoadTilemap()
        {
            Level level = Resources.Load<Level>($"Levels/Level {_levelIndex}");
            if(level == null){
                Debug.Log($"Level {_levelIndex} does not exist, loading default first level");
                _levelIndex = 0;
                level = Resources.Load<Level>($"Levels/Level {_levelIndex}");
                if(level == null) Debug.LogError("no levels found");
            }

            ClearTilemap();

            foreach(SavedTile tile in level.GroundTiles)
            {
                _groundTilemap.SetTile(tile.Position, tile.Tile);
            }
            OnLevelLoadedEvent?.Invoke(level.GroundTiles[0].Position);
        }

        private void GetRandomColors()
        {
            int x = UnityEngine.Random.Range(0, ColorArray.Length);
            if(ColorArray[x] != null){
                _ballSprite.color = ColorArray[x].BallColor;
                _coloredTile.color = ColorArray[x].TileColor;
            }
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