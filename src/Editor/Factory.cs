using System;
using System.Collections.Generic;
using Appalachia.Audio.Core;
using Appalachia.Audio.Scriptables;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Strings;
using UnityEngine;

namespace Appalachia.Audio
{
    [CallStaticConstructorInEditor]
    public static class Factory
    {
        public delegate void Initializer<X, Y>(X x, Y y);

        static Factory()
        {
            AudioImportSettings.InstanceAvailable += i => _audioImportSettings = i;
        }

        #region Static Fields and Autoproperties

        private static AudioImportSettings _audioImportSettings;

        #endregion

        public static void Create<X, Y>(Initializer<X, Y> init)
            where X : AppalachiaObject
            where Y : UnityEngine.Object
        {
            if (!AudioImportSettings.IsInstanceAvailable)
            {
                return;
            }

            var p = AssetDatabaseManager.GetAssetPath(UnityEditor.Selection.activeObject);
            if (string.IsNullOrEmpty(p))
            {
                p = _audioImportSettings.AssetPath;
            }

            if (!AppaDirectory.Exists(p))
            {
                p = AppaPath.GetDirectoryName(p);
                if (!AppaDirectory.Exists(p))
                {
                    p = "Assets";
                }
            }

            var l = new List<Y>();
            foreach (var i in UnityEditor.Selection.objects)
            {
                if (i is Y && (AssetDatabaseManager.IsMainAsset(i) || AssetDatabaseManager.IsSubAsset(i)))
                {
                    l.Add((Y)i);
                }
            }

            l.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            var r = "";
            foreach (var i in l)
            {
                if (r == "")
                {
                    r = i.name;
                }
                else if (i.name.IndexOf(r, StringComparison.Ordinal) != 0)
                {
                    for (var n = r.Length - 1; n > 0; --n)
                    {
                        var s = r.Substring(0, n);
                        if (i.name.IndexOf(s, StringComparison.Ordinal) == 0)
                        {
                            r = s;
                            break;
                        }
                    }
                }
            }

            if ((r != "") &&
                (((r[r.Length - 1] >= '0') && (r[r.Length - 1] <= '9')) ||
                 (r[r.Length - 1] == '_') ||
                 (r[r.Length - 1] == '-')))
            {
                if (r.Contains("_"))
                {
                    r = r.Substring(0, r.LastIndexOf("_", StringComparison.Ordinal));
                }
                else if (r.Contains("-"))
                {
                    r = r.Substring(0, r.LastIndexOf("-"));
                }
            }

            p = AssetDatabaseManager.GenerateUniqueAssetPath(
                AppaPath.Combine(p, r != "" ? r : ZString.Format("New {0}.asset", typeof(X).Name))
            );
            p = UnityEditor.EditorUtility.SaveFilePanel(
                ZString.Format("Save {0} Asset", typeof(X).Name),
                AppaPath.GetDirectoryName(p),
                AppaPath.GetFileName(p),
                "asset"
            );
            if (!string.IsNullOrEmpty(p))
            {
                if ((p.IndexOf("Assets",            StringComparison.Ordinal) >= 0) &&
                    (p.Length > p.IndexOf("Assets", StringComparison.Ordinal)))
                {
                    p = p.Substring(p.IndexOf("Assets", StringComparison.Ordinal));
                }

                var x = ScriptableObject.CreateInstance<X>();
                foreach (var i in l)
                {
                    init(x, i);
                }

                AssetDatabaseManager.CreateAsset(x, p);
                UnityEditor.EditorGUIUtility.PingObject(x);
            }
        }

        #region Menu Items

        [UnityEditor.MenuItem(PKG.Menu.Assets.Base + "Patch")]
        private static void CreateAudioProgram()
        {
            /*var newInstance = Patch.CreateNew();*/

            Create<Patch, AudioClip>(
                (a, c) =>
                {
                    if (a.program == null)
                    {
                        a.program = new AudioProgram();
                    }

                    Array.Resize(
                        ref a.program.clips,
                        a.program.clips != null ? a.program.clips.Length + 1 : 1
                    );
                    a.program.clips[a.program.clips.Length - 1] =
                        new AudioProgram.AudioClipParams { clip = c };
                }
            );
        }

        #endregion
    }
}
