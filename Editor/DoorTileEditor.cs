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

        public DoorTile doorTile => target as DoorTile;

        public List<KeyValuePair<KeyValuePair<Vector3Int, Tilemap>, bool>> m_DataList = new List<KeyValuePair<KeyValuePair<Vector3Int, Tilemap>, bool>>();
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
            doorTile.m_OnOpenSprite = EditorGUILayout.ObjectField("On Open Sprite", doorTile.m_OnOpenSprite, typeof(Sprite), false) as Sprite;
            doorTile.m_OnOpenGameObject = EditorGUILayout.ObjectField("On Open Game Object", doorTile.m_OnOpenGameObject, typeof(GameObject), false) as GameObject;
            doorTile.m_OnOpenColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("On Open Collider", doorTile.m_OnOpenColliderType);
            EditorGUILayout.Space();

            doorTile.m_OnCloseSprite = EditorGUILayout.ObjectField("On Close Sprite", doorTile.m_OnCloseSprite, typeof(Sprite), false) as Sprite;
            doorTile.m_OnCloseGameObject = EditorGUILayout.ObjectField("On Close Game Object", doorTile.m_OnCloseGameObject, typeof(GameObject), false) as GameObject;
            doorTile.m_OnCloseColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("On Close Collider", doorTile.m_OnCloseColliderType);
            EditorGUILayout.Space();

            m_DataList.Clear();
            foreach (var data in DoorTile.m_Data)
                m_DataList.Add(data);

            m_ReorderableList.DoLayoutList();
        }

        public void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, "Door Tiles");
        }

        public void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            var data = m_DataList[index];

            Rect toggleRect = new Rect(rect.xMax - 16, rect.y, 16, rect.height);
            Rect tilemapRect = new Rect(rect.xMin, rect.y, rect.width * 0.3f - toggleRect.width, rect.height);
            Rect positionRect = new Rect(tilemapRect.xMax, rect.y, rect.width - tilemapRect.width - toggleRect.width, rect.height);

            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUI.ObjectField(tilemapRect, GUIContent.none, data.Key.Value, typeof(Tilemap), true);
                EditorGUI.Vector3IntField(positionRect, GUIContent.none, data.Key.Key);
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                bool open = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, data.Value);

                if (check.changed)
                    DoorTile.SetOpen(data.Key, open);
            }
        }
    }
}