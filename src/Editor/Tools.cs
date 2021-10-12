using System.Collections.Generic;
using Appalachia.Audio.Components;
using Appalachia.Core.Constants;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public static class Tools
    {
        private const string FIND_PATCHES_WITHOUT_CLIPS = "Find Patches Without AudioClips";
        private const string FIND_UNUSED_AUDIOCLIPS = "Find Unused AudioClips";
        
        [MenuItem(APPA_MENU.BASE_AppalachiaTools + APPA_MENU.ASM_AppalachiaAudio + FIND_PATCHES_WITHOUT_CLIPS)]
        private static void FindPatchWithoutAudioClips()
        {
            var root = "Assets/Audio";
            var guids = AssetDatabase.FindAssets("t:Object", new[] {root});
            var count = 0;

            Debug.Log($"[{FIND_PATCHES_WITHOUT_CLIPS}]: Searching for patches in " + root);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadMainAssetAtPath(path);
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
                        Debug.LogWarning($"[{FIND_PATCHES_WITHOUT_CLIPS}]: Found " + path, asset);
                    }

                    ++count;
                }
            }

            Debug.Log(
                $"[{FIND_PATCHES_WITHOUT_CLIPS}]: All done, checked " +
                count +
                (count == 1 ? " patch" : " patches")
            );
        }

        
        [MenuItem(APPA_MENU.BASE_AppalachiaTools + APPA_MENU.ASM_AppalachiaAudio + FIND_UNUSED_AUDIOCLIPS)]
        private static void FindUnusedAudioClips()
        {
            var root = "Assets/Audio";
            var guids = AssetDatabase.FindAssets("t:Object", new[] {root});
            var clips = new HashSet<string>();
            var patchCount = 0;
            int clipCount;

            Debug.Log($"[{FIND_PATCHES_WITHOUT_CLIPS}]: Searching for patches in " + root);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadMainAssetAtPath(path);
                var patch = asset as Patch;

                if (patch)
                {
                    if (patch.program.clips != null)
                    {
                        foreach (var clip in patch.program.clips)
                        {
                            if (clip.clip)
                            {
                                clips.Add(AssetDatabase.GetAssetPath(clip.clip));
                            }
                        }
                    }

                    ++patchCount;
                }
            }

            guids = AssetDatabase.FindAssets("t:AudioClip", new[] {root});
            clipCount = guids.Length;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!clips.Contains(path))
                {
                    Debug.LogWarning(
                        $"[{FIND_PATCHES_WITHOUT_CLIPS}]: Found " + path,
                        AssetDatabase.LoadMainAssetAtPath(path)
                    );
                }
            }

            Debug.Log(
                $"[{FIND_PATCHES_WITHOUT_CLIPS}]: All done, checked " +
                patchCount +
                (patchCount == 1 ? " patch " : " patches ") +
                "and " +
                clipCount +
                (clipCount == 1 ? " AudioClip " : " AudioClips")
            );
        }
    }
}
