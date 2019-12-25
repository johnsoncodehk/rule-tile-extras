using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditorInternal;

namespace RuleTileExtras.Editor
{
    [CustomEditor(typeof(DoorTile))]
    public class DoorTileEditor : UnityEditor.Editor
    {

        public struct DataKey
        {
            public Vector3Int position;
            public Tilemap tilemap;
            public GridInformation gridInfo;
        }

        public DoorTile doorTile => target as DoorTile;

        public List<KeyValuePair<DataKey, bool>> m_DataList = new List<KeyValuePair<DataKey, bool>>();
        public ReorderableList m_ReorderableList;

        public void OnEnable()
        {
            if (m_ReorderableList == null)
            {
                m_ReorderableList = new ReorderableList(m_DataList, typeof(RuleTile.TilingRule), false, false, false, false);
                m_ReorderableList.drawHeaderCallback = OnDrawHeader;
                m_ReorderableList.drawElementCallback = OnDrawElement;
            }
        }

        public override void OnInspectorGUI()
        {
            doorTile.outputs[0].sprite = EditorGUILayout.ObjectField("On Close Sprite", doorTile.outputs[0].sprite, typeof(Sprite), false) as Sprite;
            doorTile.outputs[0].gameObject = EditorGUILayout.ObjectField("On Close Game Object", doorTile.outputs[0].gameObject, typeof(GameObject), false) as GameObject;
            doorTile.outputs[0].colliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("On Close Collider", doorTile.outputs[0].colliderType);
            EditorGUILayout.Space();

            doorTile.outputs[1].sprite = EditorGUILayout.ObjectField("On Open Sprite", doorTile.outputs[1].sprite, typeof(Sprite), false) as Sprite;
            doorTile.outputs[1].gameObject = EditorGUILayout.ObjectField("On Open Game Object", doorTile.outputs[1].gameObject, typeof(GameObject), false) as GameObject;
            doorTile.outputs[1].colliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("On Open Collider", doorTile.outputs[1].colliderType);
            EditorGUILayout.Space();

            doorTile.gridInformationKey = EditorGUILayout.TextField("Grid Information Key", doorTile.gridInformationKey);
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

            m_ReorderableList.DoLayoutList();
        }

        public void OnDrawHeader(Rect rect)
        {
            Rect toggleRect = new Rect(rect.xMax - 136, rect.y, 136, rect.height);
            Rect tilemapRect = new Rect(rect.xMin, rect.y, (rect.width - toggleRect.width) * 0.5f, rect.height);
            Rect positionRect = new Rect(tilemapRect.xMax, rect.y, rect.width - tilemapRect.width - toggleRect.width, rect.height);

            GUI.Label(tilemapRect, "Tilemap");
            GUI.Label(positionRect, "Position");
            GUI.Label(toggleRect, doorTile.gridInformationKey);
        }

        public void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
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
    }
}