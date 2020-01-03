using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditorInternal;

namespace RuleTileExtras.Editor
{
    [CustomEditor(typeof(DoorRuleTile))]
    public class DoorRuleTileEditor : BaseRuleTileEditor
    {

        public struct DataKey
        {
            public Vector3Int position;
            public Tilemap tilemap;
        }

        public DoorRuleTile doorTile => target as DoorRuleTile;

        public List<KeyValuePair<DataKey, bool>> m_DataList = new List<KeyValuePair<DataKey, bool>>();
        public ReorderableList m_InstanceList;

        public RuleTile onCloseTile => GetSubTile(0, 0, doorTile.rulesSplitIndex);
        public RuleTile onOpenTile => GetSubTile(1, doorTile.rulesSplitIndex, doorTile.m_TilingRules.Count);
        public RuleTileEditor onCloseTileEditor => GetSubTileEditor(0, onCloseTile);
        public RuleTileEditor onOpenTileEditor => GetSubTileEditor(1, onOpenTile);

        RuleTileEditor[] m_SubTileEditors = new RuleTileEditor[2];
        RuleTile[] m_SubTiles = new RuleTile[2];

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

        public RuleTile GetSubTile(int index, int rulesStart, int rulesEnd)
        {
            if (m_SubTiles[index] == null)
            {
                m_SubTiles[index] = DoorRuleTile.CreateInstance("RuleTile") as RuleTile;

                for (int i = rulesStart; i < rulesEnd; i++)
                    m_SubTiles[index].m_TilingRules.Add(doorTile.m_TilingRules[i]);
            }

            return m_SubTiles[index];
        }

        public RuleTileEditor GetSubTileEditor(int index, RuleTile tile)
        {
            if (m_SubTileEditors[index] == null)
            {
                m_SubTileEditors[index] = UnityEditor.Editor.CreateEditor(tile) as RuleTileEditor;
                m_SubTileEditors[index].m_ReorderableList.onChangedCallback += list => OnSubListUpdate(index);
            }

            return m_SubTileEditors[index];
        }

        public void OnSubListUpdate(int index)
        {
            List<RuleTile.TilingRule> listToFix = m_SubTiles[index].m_TilingRules;
            List<RuleTile.TilingRule> otherList = m_SubTiles[(index + 1) % 2].m_TilingRules;
            HashSet<int> usedIdSet = new HashSet<int>();

            foreach (var rule in otherList)
                usedIdSet.Add(rule.m_Id);

            foreach (var rule in listToFix)
            {
                while (usedIdSet.Contains(rule.m_Id))
                    rule.m_Id++;
                usedIdSet.Add(rule.m_Id);
            }

            UpdateRulesFromSubLists();
        }

        public void UpdateRulesFromSubLists()
        {
            doorTile.m_TilingRules.Clear();

            foreach (var rule in onCloseTile.m_TilingRules)
                doorTile.m_TilingRules.Add(rule);
            foreach (var rule in onOpenTile.m_TilingRules)
                doorTile.m_TilingRules.Add(rule);

            doorTile.rulesSplitIndex = onCloseTile.m_TilingRules.Count;
            SaveTile();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            DestroyImmediate(m_SubTileEditors[0]);
            DestroyImmediate(m_SubTileEditors[1]);
            DestroyImmediate(m_SubTiles[0]);
            DestroyImmediate(m_SubTiles[1]);
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            tile.m_DefaultSprite = EditorGUILayout.ObjectField("Default Sprite", tile.m_DefaultSprite, typeof(Sprite), false) as Sprite;
            tile.m_DefaultGameObject = EditorGUILayout.ObjectField("Default Game Object", tile.m_DefaultGameObject, typeof(GameObject), false) as GameObject;
            tile.m_DefaultColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default Collider", tile.m_DefaultColliderType);
            DrawCustomFields(false);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("On Close");
            onCloseTileEditor.m_ReorderableList.DoLayoutList();
            EditorGUILayout.LabelField("On Open");
            onOpenTileEditor.m_ReorderableList.DoLayoutList();
            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
                SaveTile();

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
                        var other = tilemap.GetTile(position);
                        if (other == doorTile)
                        {
                            bool isOpen = doorTile.IsOpen(position, tilemap.GetComponent<GridInformation>());

                            var dataKey = new DataKey();
                            dataKey.tilemap = tilemap;
                            dataKey.position = position;

                            m_DataList.Add(new KeyValuePair<DataKey, bool>(dataKey, isOpen));
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

            var gridInfo = data.Key.tilemap.GetComponent<GridInformation>();
            if (!gridInfo)
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
                        doorTile.SetOpen(data.Key.position, gridInfo, open);
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

            doorTile.SetOpen(new Vector3Int(-1, 0, 0), m_PreviewTilemaps[0].gameObject.AddComponent<GridInformation>(), false);
            doorTile.SetOpen(new Vector3Int(0, 0, 0), m_PreviewTilemaps[1].gameObject.AddComponent<GridInformation>(), true);
        }
    }
}