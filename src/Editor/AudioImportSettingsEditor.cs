using System;
using System.Collections.Generic;
using System.Diagnostics;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Availability;
using Appalachia.Utility.Strings;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    [CustomEditor(typeof(AudioImportSettings))]
    [CallStaticConstructorInEditor]
    public class AudioImportSettingsEditor : UnityEditor.Editor
    {
        private delegate void EndFunction(SerializedProperty overrideProperty);

        private delegate bool OverrideFunction(
            SerializedProperty overrideProperty,
            int overrideIndex,
            int overrideCount,
            int matchCount);

        private delegate void PathFunction(string path, string name, int overrideIndex);

        #region Constants and Static Readonly

        public static readonly Dictionary<string, int> overridesTable = new();

        private static readonly char[] splitSeparators = { ';' };

        #endregion

        static AudioImportSettingsEditor()
        {
            RegisterInstanceCallbacks.WithoutSorting()
                                     .When.Object<AudioImportSettings>()
                                     .IsAvailableThen(i => _audioImportSettings = i);
        }

        #region Static Fields and Autoproperties

        private static AudioImportSettings _audioImportSettings;

        #endregion

        #region Fields and Autoproperties

        private bool _resetCaches;
        private bool _unfilteredVisible;
        private Dictionary<string, int> _nameCount;

        private GUIStyle _lineStyle;
        private HashSet<string> _filteredPaths;
        private int _deleteCommand;

        private int _moveCommand;
        private List<SortedDictionary<string, string>> _cacheTables;

        private SortedDictionary<string, string> _audioClipPaths;
        private SortedDictionary<string, string> _unfilteredPaths;
        private Stopwatch _stopwatch;

        private string _refreshText;
        private string _unfilteredText;

        #endregion

        #region Event Functions

        protected void OnEnable()
        {
            _audioClipPaths = new SortedDictionary<string, string>();
            _cacheTables = new List<SortedDictionary<string, string>>();
            _nameCount = new Dictionary<string, int>();
            _unfilteredPaths = new SortedDictionary<string, string>();
            _filteredPaths = new HashSet<string>();
            _stopwatch = new Stopwatch();

            InitLine();
            RefreshAudioClips();
        }

        #endregion

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            if (!AudioImportSettings.IsInstanceAvailable)
            {
                return;
            }

            if (GUILayout.Button("Apply"))
            {
                IterateOverrides(
                    (overrideProperty, overrideIndex, overrideCount, matchCount) => true,
                    (path, fileName, overrideIndex) => { overridesTable.Add(path, overrideIndex); },
                    overrideProperty => { }
                );
                AssetDatabaseManager.ImportAsset(
                    _audioImportSettings.AssetPath,
                    ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive
                );
                overridesTable.Clear();
            }

            GUI.enabled = true;

            DrawLine();

            const int w = 40;

            var refreshRect = EditorGUILayout.GetControlRect();
            var refreshRect2 = refreshRect;
            refreshRect2.x += refreshRect2.width - (w * 3);
            refreshRect2.width = w * 3;
            refreshRect.width -= w * 3;

            GUI.Label(refreshRect, _refreshText);

            if (GUI.Button(refreshRect2, "Refresh"))
            {
                RefreshAudioClips();
            }

            EditorGUI.BeginChangeCheck();

            _moveCommand = -1;
            _deleteCommand = -1;
            _resetCaches = false;

            var overridesProperty = IterateOverrides(
                (overrideProperty, overrideIndex, overrideCount, matchCount) =>
                {
                    DrawLine();

                    var buttonRect = EditorGUILayout.GetControlRect();
                    buttonRect.x += buttonRect.width - w;
                    buttonRect.width = w;

                    if (GUI.Button(buttonRect, "\u2716"))
                    {
                        _deleteCommand = overrideIndex;
                    }

                    buttonRect.x -= w;
                    GUI.enabled = overrideIndex < (overrideCount - 1);

                    if (GUI.Button(buttonRect, "\u25bc"))
                    {
                        _moveCommand = overrideIndex | (1 << 30);
                    }

                    buttonRect.x -= w;
                    GUI.enabled = overrideIndex > 0;

                    if (GUI.Button(buttonRect, "\u25b2"))
                    {
                        _moveCommand = overrideIndex | (3 << 30);
                    }

                    GUI.enabled = true;

                    var visibleProperty = overrideProperty.FindPropertyRelative("visible");
                    var filterProperty = overrideProperty.FindPropertyRelative("filter");

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(filterProperty);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _resetCaches = true;
                    }

                    var matchingText = "Matches (" + matchCount + ")";
                    visibleProperty.boolValue = EditorGUILayout.Foldout(
                        visibleProperty.boolValue,
                        matchingText
                    );

                    EditorGUI.indentLevel++;
                    GUI.enabled = false;

                    return visibleProperty.boolValue;
                },
                (path, fileName, overrideIndex) =>
                {
                    if (!_filteredPaths.Contains(fileName))
                    {
                        _filteredPaths.Add(fileName);
                        GUI.Label(
                            EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()),
                            fileName + " (" + _nameCount[fileName] + ")"
                        );
                    }
                },
                overrideProperty =>
                {
                    EditorGUI.indentLevel--;
                    GUI.enabled = true;

                    var settingsProperty = overrideProperty.FindPropertyRelative("settings");
                    if (settingsProperty != null)
                    {
                        EditorGUILayout.PropertyField(settingsProperty, true);
                    }
                }
            );

            if (_moveCommand != -1)
            {
                var direction = (_moveCommand >> 31) | (_moveCommand >> 30);
                _moveCommand &= ~(3 << 30);
                overridesProperty.MoveArrayElement(_moveCommand, _moveCommand + direction);
            }

            if (_deleteCommand != -1)
            {
                overridesProperty.DeleteArrayElementAtIndex(_deleteCommand);
            }

            if (_resetCaches)
            {
                ResetCaches();
            }

            _filteredPaths.Clear();

            DrawLine();

            var addButtonRect = EditorGUILayout.GetControlRect();
            var addButtonRect2 = addButtonRect;
            addButtonRect2.x += addButtonRect2.width - w;
            addButtonRect2.width = w;
            addButtonRect.width -= w * 3;

            if (GUI.Button(addButtonRect2, "\u271a"))
            {
                overridesProperty.InsertArrayElementAtIndex(overridesProperty.arraySize);
            }

            _unfilteredVisible = EditorGUI.Foldout(addButtonRect, _unfilteredVisible, _unfilteredText);

            EditorGUI.indentLevel++;
            GUI.enabled = false;

            if (_unfilteredVisible)
            {
                foreach (var x in _unfilteredPaths)
                {
                    if (!_filteredPaths.Contains(x.Value))
                    {
                        _filteredPaths.Add(x.Value);
                        GUI.Label(
                            EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()),
                            x.Value + " (" + _nameCount[x.Value] + ")"
                        );
                    }
                }
            }

            _filteredPaths.Clear();

            EditorGUI.indentLevel--;
            GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawLine()
        {
            var c = GUI.color;
            var p = GUILayoutUtility.GetRect(GUIContent.none, _lineStyle, GUILayout.Height(1));
            if (Event.current.type == EventType.Repaint)
            {
                _lineStyle.Draw(p, false, false, false, false);
            }
        }

        private void InitLine()
        {
            _lineStyle = new GUIStyle();
            _lineStyle.normal.background = EditorGUIUtility.whiteTexture;
            _lineStyle.stretchWidth = true;
            _lineStyle.margin = new RectOffset(0, 0, 7, 7);
        }

        private SerializedProperty IterateOverrides(
            OverrideFunction overrideFunction,
            PathFunction pathFunction,
            EndFunction endFunction)
        {
            var overridesProperty = serializedObject.FindProperty("overrides");

            for (int i = 0, n = overridesProperty.arraySize; i < n; ++i)
            {
                var overrideProperty = overridesProperty.GetArrayElementAtIndex(i);

                var filterProperty = overrideProperty.FindPropertyRelative("filter");

                //var filter = filterProperty.stringValue;

                if (i < _cacheTables.Count)
                {
                    var table = _cacheTables[i];

                    if (overrideFunction(overrideProperty, i, n, table.Count))
                    {
                        foreach (var x in table)
                        {
                            pathFunction(x.Key, x.Value, i);
                        }
                    }
                }

                endFunction(overrideProperty);
            }

            return overridesProperty;
        }

        private void RefreshAudioClips()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            _audioClipPaths.Clear();
            _nameCount.Clear();

            var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var delimeters = new[] { '_', '-', ' ' };
            var guids = AssetDatabaseManager.FindAssets("t:AudioClip");

            foreach (var guid in guids)
            {
                var path = AssetDatabaseManager.GUIDToAssetPath(guid);
                var fileName = path.FileNameWithoutExtension;
                fileName = fileName.Trim(digits);
                fileName = fileName.Trim(delimeters);

                _audioClipPaths.Add(path.RelativePath, fileName);

                int count;
                _nameCount.TryGetValue(fileName, out count);
                _nameCount[fileName] = count + 1;
            }

            ResetCaches();

            _stopwatch.Stop();

            _refreshText = ZString.Format(
                "Found {0} AudioClips in {1:N2}s",
                guids.Length,
                _stopwatch.Elapsed.TotalSeconds
            );
        }

        private void ResetCaches()
        {
            var overridesProperty = serializedObject.FindProperty("overrides");

            _unfilteredPaths.Clear();

            foreach (var x in _audioClipPaths)
            {
                _unfilteredPaths.Add(x.Key, x.Value);
            }

            for (int i = 0, n = overridesProperty.arraySize; i < n; ++i)
            {
                SortedDictionary<string, string> table;

                if (_cacheTables.Count > i)
                {
                    table = _cacheTables[i];
                    table.Clear();
                }
                else
                {
                    table = new SortedDictionary<string, string>();
                    _cacheTables.Add(table);
                }

                var overrideProperty = overridesProperty.GetArrayElementAtIndex(i);
                var filterProperty = overrideProperty.FindPropertyRelative("filter");
                var filter = filterProperty.stringValue.Split(splitSeparators, StringSplitOptions.None);

                foreach (var x in _unfilteredPaths)
                foreach (var y in filter)
                {
                    if (x.Key.IndexOf(y, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        if (!table.ContainsKey(x.Key))
                        {
                            _filteredPaths.Add(x.Key);
                            table.Add(x.Key, x.Value);
                        }
                    }
                }

                foreach (var x in _filteredPaths)
                {
                    _unfilteredPaths.Remove(x);
                }

                _filteredPaths.Clear();
            }

            _unfilteredText = _unfilteredPaths.Count + " AudioClips left unmodified";
        }
    }
}
