/*
namespace Appalachia.Audio
{
    /*public static class Factory
        {
            public delegate void Initializer<X, Y>(X x, Y y);

            public static void Create<X, Y>(Initializer<X, Y> init)
                where X : ScriptableObject
                where Y : Object
            {
                var p = AssetDatabaseManager.GetAssetPath(Selection.activeObject);
                if (string.IsNullOrEmpty(p))
                {
                    p = ImportSettings.instance.root;
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
                foreach (var i in Selection.objects)
                {
                    if (i is Y && (AssetDatabaseManager.IsMainAsset(i) || AssetDatabaseManager.IsSubAsset(i)))
                    {
                        l.Add((Y) i);
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
                    AppaPath.Combine(p, r != "" ? r : string.Format("New {0}.asset", typeof(X).Name))
                );
                p = EditorUtility.SaveFilePanel(
                    string.Format("Save {0} Asset", typeof(X).Name),
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
                    EditorGUIUtility.PingObject(x);
                }
            }

            [UnityEditor.MenuItem(APPASTR.MENU.BASE_AppalachiaData + APPASTR.MENU.ASM.Audio + "Patch")]
            private static void CreateAudioProgram()
            {
                var newInstance = Patch.CreateNew();

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
                            new AudioProgram.AudioClipParams {clip = c};
                    }
                );
            }
        }#1#
}

*/


