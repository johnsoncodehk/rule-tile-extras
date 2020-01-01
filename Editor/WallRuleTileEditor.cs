using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace RuleTileExtras.Editor
{
    [CustomEditor(typeof(WallRuleTile))]
    public class WallRuleTileEditor : BaseRuleTileEditor
    {

        TileBase m_PreviewOtherTile;

        TileBase previewOtherTile
        {
            get
            {
                if (m_PreviewOtherTile == null)
                {
                    m_PreviewOtherTile = Tile.CreateInstance("Tile") as Tile;
                }
                return m_PreviewOtherTile;
            }
        }

        public override void OnPreviewSettings()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_PreviewOtherTile = EditorGUILayout.ObjectField("Other Tile", m_PreviewOtherTile, typeof(TileBase), false) as TileBase;
                if (check.changed)
                    UpdatePreviews();
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (m_PreviewUtility == null)
                CreatePreview();

            if (Event.current.type != EventType.Repaint)
                return;

            m_PreviewUtility.BeginPreview(r, background);
            m_PreviewUtility.camera.orthographicSize = 2;
            if (r.height * 2 > r.width)
                m_PreviewUtility.camera.orthographicSize *= (float)r.height * 2 / r.width;
            m_PreviewUtility.camera.Render();
            m_PreviewUtility.EndAndDrawPreview(r);
        }

        public override void CreatePreview()
        {
            m_PreviewUtility = new PreviewRenderUtility(true);
            m_PreviewUtility.camera.orthographic = true;
            m_PreviewUtility.camera.orthographicSize = 2;
            m_PreviewUtility.camera.transform.position = new Vector3(0, 0, -10);

            var previewInstance = new GameObject();
            m_PreviewGrid = previewInstance.AddComponent<Grid>();
            m_PreviewUtility.AddSingleGO(previewInstance);

            m_PreviewTilemaps = new List<Tilemap>();
            m_PreviewTilemapRenderers = new List<TilemapRenderer>();

            for (int i = 0; i < 2; i++)
            {
                var previewTilemapGo = new GameObject();
                m_PreviewTilemaps.Add(previewTilemapGo.AddComponent<Tilemap>());
                m_PreviewTilemapRenderers.Add(previewTilemapGo.AddComponent<TilemapRenderer>());

                previewTilemapGo.transform.SetParent(previewInstance.transform, false);
            }

            UpdatePreviews();
        }

        void UpdatePreviews()
        {
            for (int x = -2; x <= 1; x++)
                for (int y = -2; y <= 1; y++)
                    m_PreviewTilemaps[0].SetTile(new Vector3Int(x - 2, y, 0), tile);

            for (int x = -2; x <= 1; x++)
                for (int y = -2; y <= 1; y++)
                    m_PreviewTilemaps[1].SetTile(new Vector3Int(x + 2, y, 0), previewOtherTile);

            for (int x = -1; x <= 0; x++)
                for (int y = -1; y <= 0; y++)
                    m_PreviewTilemaps[0].SetTile(new Vector3Int(x - 2, y, 0), previewOtherTile);

            for (int x = -1; x <= 0; x++)
                for (int y = -1; y <= 0; y++)
                    m_PreviewTilemaps[1].SetTile(new Vector3Int(x + 2, y, 0), tile);
        }
    }
}