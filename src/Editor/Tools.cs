using System;
using System.Collections.Generic;
using Appalachia.Audio.Scriptables;
using Appalachia.CI.Constants;
using Appalachia.CI.Integration.Assets;
using Appalachia.Utility.Strings;
using UnityEditor;

namespace Appalachia.Audio
{
    public static class Tools
    {
        #region Constants and Static Readonly

        private const string FIND_PATCHES_WITHOUT_CLIPS = "Find Patches Without AudioClips";
        private const string FIND_UNUSED_AUDIOCLIPS = "Find Unused AudioClips";

        #endregion

        #region Static Fields and Autoproperties

        [NonSerialized] private static AppaContext _context;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(Tools));
                }

                return _context;
            }
        }

        #region Menu Items

        [MenuItem(PKG.Menu.Appalachia.Tools.Base + FIND_PATCHES_WITHOUT_CLIPS)]
        private static void FindPatchWithoutAudioClips()
        {
            var root = "Assets/Audio";
            var guids = AssetDatabaseManager.FindAssets("t:Object", new[] { root });
            var count = 0;

            Context.Log.Info(
                ZString.Format("[{0}]: Searching for patches in ", FIND_PATCHES_WITHOUT_CLIPS) + root
            );

            foreach (var guid in guids)
            {
                var path = AssetDatabaseManager.GUIDToAssetPath(guid);
                var asset = AssetDatabaseManager.LoadMainAssetAtPath(path);
                var patch = asset as Patch;

                if (patch)
                {
                    var noClips = false;
                    if ((patch.program.clips == null) || (patch.program.clips.Length == 0))
                    {
                        noClips = true;
                    }
                    else
                    {
                        foreach (var clip in patch.program.clips)
                        {
                            if (!clip.clip)
                            {
                                noClips = true;
                            }
                        }
                    }

                    if (noClips)
                    {
                        Context.Log.Warn(
                            ZString.Format("[{0}]: Found ", FIND_PATCHES_WITHOUT_CLIPS) + path,
                            asset
                        );
                    }

                    ++count;
                }
            }

            Context.Log.Info(
                ZString.Format("[{0}]: All done, checked ", FIND_PATCHES_WITHOUT_CLIPS) +
                count +
                (count == 1 ? " patch" : " patches")
            );
        }

        [MenuItem(PKG.Menu.Appalachia.Tools.Base + FIND_UNUSED_AUDIOCLIPS)]
        private static void FindUnusedAudioClips()
        {
            var root = "Assets/Audio";
            var guids = AssetDatabaseManager.FindAssets("t:Object", new[] { root });
            var clips = new HashSet<string>();
            var patchCount = 0;
            int clipCount;

            Context.Log.Info(
                ZString.Format("[{0}]: Searching for patches in ", FIND_PATCHES_WITHOUT_CLIPS) + root
            );

            foreach (var guid in guids)
            {
                var path = AssetDatabaseManager.GUIDToAssetPath(guid);
                var asset = AssetDatabaseManager.LoadMainAssetAtPath(path);
                var patch = asset as Patch;

                if (patch)
                {
                    if (patch.program.clips != null)
                    {
                        foreach (var clip in patch.program.clips)
                        {
                            if (clip.clip)
                            {
                                clips.Add(AssetDatabaseManager.GetAssetPath(clip.clip));
                            }
                        }
                    }

                    ++patchCount;
                }
            }

            guids = AssetDatabaseManager.FindAssets("t:AudioClip", new[] { root });
            clipCount = guids.Length;

            foreach (var guid in guids)
            {
                var path = AssetDatabaseManager.GUIDToAssetPath(guid);
                if (!clips.Contains(path.relativePath))
                {
                    Context.Log.Warn(
                        ZString.Format("[{0}]: Found ", FIND_PATCHES_WITHOUT_CLIPS) + path,
                        AssetDatabaseManager.LoadMainAssetAtPath(path)
                    );
                }
            }

            Context.Log.Info(
                ZString.Format("[{0}]: All done, checked ", FIND_PATCHES_WITHOUT_CLIPS) +
                patchCount +
                (patchCount == 1 ? " patch " : " patches ") +
                "and " +
                clipCount +
                (clipCount == 1 ? " AudioClip " : " AudioClips")
            );
        }

        #endregion
    }
}
