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

        public List<KeyValuePair<DoorTile.DataKey, bool>> m_DataList = new List<KeyValuePair<DoorTile.DataKey, bool>>();
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
            doorTile.outputs[0].m_Sprite = EditorGUILayout.ObjectField("On Close Sprite", doorTile.outputs[0].m_Sprite, typeof(Sprite), false) as Sprite;
            doorTile.outputs[0].m_GameObject = EditorGUILayout.ObjectField("On Close Game Object", doorTile.outputs[0].m_GameObject, typeof(GameObject), false) as GameObject;
            doorTile.outputs[0].m_ColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("On Close Collider", doorTile.outputs[0].m_ColliderType);
            EditorGUILayout.Space();

            doorTile.outputs[1].m_Sprite = EditorGUILayout.ObjectField("On Open Sprite", doorTile.outputs[1].m_Sprite, typeof(Sprite), false) as Sprite;
            doorTile.outputs[1].m_GameObject = EditorGUILayout.ObjectField("On Open Game Object", doorTile.outputs[1].m_GameObject, typeof(GameObject), false) as GameObject;
            doorTile.outputs[1].m_ColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("On Open Collider", doorTile.outputs[1].m_ColliderType);
            EditorGUILayout.Space();

            m_DataList.Clear();
            if (Application.isPlaying)
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
                EditorGUI.ObjectField(tilemapRect, GUIContent.none, data.Key.tilemap, typeof(Tilemap), true);
                EditorGUI.Vector3IntField(positionRect, GUIContent.none, data.Key.position);
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