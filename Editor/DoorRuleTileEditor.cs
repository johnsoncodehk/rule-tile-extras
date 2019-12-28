using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditorInternal;

namespace RuleTileExtras.Editor
{
    [CustomEditor(typeof(DoorRuleTile))]
    public class DoorRuleTileEditor : RuleTileEditor
    {

        public struct DataKey
        {
            public Vector3Int position;
            public Tilemap tilemap;
            public GridInformation gridInfo;
        }

        public DoorRuleTile doorTile => target as DoorRuleTile;

        public List<KeyValuePair<DataKey, bool>> m_DataList = new List<KeyValuePair<DataKey, bool>>();
        public ReorderableList m_InstanceList;

        public ReorderableList instanceList
        {
            get
            {
                if (m_InstanceList == null)
                {
                    m_InstanceList = new ReorderableList(m_DataList, typeof(RuleTile.TilingRule), false, false, false, false);
                    m_InstanceList.drawHeaderCallback = OnDrawInstanceListHeader;
                    m_InstanceList.drawElementCallback = OnDrawInstanceElement;
                }
                return m_InstanceList;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (doorTile.m_TilingRules.Count > 0)
            {
                float index = doorTile.rulesSplitIndex;
                float max = doorTile.m_TilingRules.Count;
                EditorGUILayout.MinMaxSlider(ref index, ref max, 0, max);
                int newIndex = Mathf.RoundToInt(index);
                if (doorTile.rulesSplitIndex != newIndex)
                {
                    doorTile.rulesSplitIndex = newIndex;
                    SaveTile();
                }

                List<int> closeRules = new List<int>();
                List<int> openRules = new List<int>();
                for (int i = 0; i < doorTile.m_TilingRules.Count; i++)
                    (i < doorTile.rulesSplitIndex ? closeRules : openRules).Add(i);
                string info = "Close Tiling Rules: " + string.Join(", ", closeRules);
                info += "\nOpen Tiling Rules: " + string.Join(", ", openRules);
                EditorGUILayout.HelpBox(info, MessageType.Info);
            }
            EditorGUILayout.Space();

            m_DataList.Clear();
            var tilemaps = FindObjectsOfType<Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                var bounds = tilemap.cellBounds;
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        var position = new Vector3Int(x, y, 0);
                        var doorTile = tilemap.GetTile(position);
                        if (doorTile == this.doorTile)
                        {
                            bool dataValue = false;
                            var gridInfo = tilemap.GetComponent<GridInformation>();
                            if (gridInfo)
                                dataValue = this.doorTile.IsOpen(position, gridInfo);

                            var dataKey = new DataKey();
                            dataKey.tilemap = tilemap;
                            dataKey.gridInfo = gridInfo;
                            dataKey.position = position;

                            m_DataList.Add(new KeyValuePair<DataKey, bool>(dataKey, dataValue));
                        }
                    }
                }
            }

            instanceList.DoLayoutList();
        }

        public void OnDrawInstanceListHeader(Rect rect)
        {
            Rect toggleRect = new Rect(rect.xMax - 136, rect.y, 136, rect.height);
            Rect tilemapRect = new Rect(rect.xMin, rect.y, (rect.width - toggleRect.width) * 0.5f, rect.height);
            Rect positionRect = new Rect(tilemapRect.xMax, rect.y, rect.width - tilemapRect.width - toggleRect.width, rect.height);

            GUI.Label(tilemapRect, "Tilemap");
            GUI.Label(positionRect, "Position");
            GUI.Label(toggleRect, doorTile.gridInformationKey);
        }

        public void OnDrawInstanceElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            var data = m_DataList[index];

            Rect toggleRect = new Rect(rect.xMax - 136, rect.y, 136, rect.height);
            Rect tilemapRect = new Rect(rect.xMin, rect.y, (rect.width - toggleRect.width) * 0.5f, rect.height);
            Rect positionRect = new Rect(tilemapRect.xMax, rect.y, rect.width - tilemapRect.width - toggleRect.width, rect.height);

            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUI.ObjectField(tilemapRect, GUIContent.none, data.Key.tilemap, typeof(Tilemap), true);
                EditorGUI.Vector3IntField(positionRect, GUIContent.none, data.Key.position);
            }

            if (!data.Key.gridInfo)
            {
                if (GUI.Button(toggleRect, "Add GridInformation"))
                    data.Key.tilemap.gameObject.AddComponent<GridInformation>();
            }
            else
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    bool open = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, data.Value);

                    if (check.changed)
                        doorTile.SetOpen(data.Key.position, data.Key.tilemap, data.Key.gridInfo, open);
                }
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (m_PreviewUtility == null)
                CreatePreview();

            if (Event.current.type != EventType.Repaint)
                return;

            m_PreviewUtility.BeginPreview(r, background);
            m_PreviewUtility.camera.orthographicSize = 0.5f;
            if (r.height * 2 > r.width)
                m_PreviewUtility.camera.orthographicSize *= (float)r.height * 2 / r.width;
            m_PreviewUtility.camera.Render();
            m_PreviewUtility.EndAndDrawPreview(r);
        }

        public override void CreatePreview()
        {
            m_PreviewUtility = new PreviewRenderUtility(true);
            m_PreviewUtility.camera.orthographic = true;
            m_PreviewUtility.camera.orthographicSize = 0.5f;
            m_PreviewUtility.camera.transform.position = new Vector3(0, 0.5f, -10);

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

            m_PreviewTilemaps[0].SetTile(new Vector3Int(-1, 0, 0), tile);
            m_PreviewTilemaps[1].SetTile(new Vector3Int(0, 0, 0), tile);

            doorTile.SetOpen(new Vector3Int(-1, 0, 0), m_PreviewTilemaps[0], m_PreviewTilemaps[0].gameObject.AddComponent<GridInformation>(), false);
            doorTile.SetOpen(new Vector3Int(0, 0, 0), m_PreviewTilemaps[1], m_PreviewTilemaps[1].gameObject.AddComponent<GridInformation>(), true);
        }
    }
}