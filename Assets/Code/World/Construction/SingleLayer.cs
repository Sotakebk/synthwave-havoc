#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TopDownShooter.World.Construction
{
    [System.Serializable]
    public struct SingleLayer
    {
        public SingleLayer(int layerIndex)
        {
            _layerIndex = layerIndex;
        }

        [SerializeField]
        private int _layerIndex;

        public int LayerIndex
        {
            get { return _layerIndex; }
        }

        public int Mask
        {
            get => 1 << _layerIndex;
            set
            {
                if (value > 0 && value < 32)
                {
                    _layerIndex = value;
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SingleLayer))]
    public class SingleLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginProperty(_position, GUIContent.none, _property);
            SerializedProperty layerIndex = _property.FindPropertyRelative("_layerIndex");
            _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
            if (layerIndex != null)
            {
                layerIndex.intValue = EditorGUI.LayerField(_position, layerIndex.intValue);
            }
            EditorGUI.EndProperty();
        }
    }
#endif
}