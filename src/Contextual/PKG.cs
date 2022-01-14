// ReSharper disable All
// DO NOT MODIFY: START

using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Appalachia.Utility.Constants;

[assembly: AssemblyTitle("Appalachia.Audio.Contextual")]
[assembly:
    AssemblyDescription(
        "Native audio plugins for Unity that provide analysis of audio system output to create reactive audio components."
    )]
[assembly: AssemblyCompany("Appalachia Interactive")]
[assembly: AssemblyProduct("Keepers Of Creation")]
[assembly: AssemblyCopyright("Copyright © Appalachia Interactive 2021")]
[assembly: AssemblyTrademark("Keepers Of Creation")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: AssemblyVersion("0.2.0.0")]
[assembly: AssemblyFileVersion("0.2.0.0")]

namespace Appalachia.Audio.Contextual
{
    internal static partial class PKG
    {
        #region Constants and Static Readonly

        public const int Priority = -359000;
        public const int VersionInt = 2000;
        public const string AssemblyCompany = "Appalachia Interactive";
        public const string AssemblyCopyright = "Copyright © Appalachia Interactive 2021";
        public const string AssemblyCulture = "";

        public const string AssemblyDescription =
            "Native audio plugins for Unity that provide analysis of audio system output to create reactive audio components.";

        public const string AssemblyFileVersion = "0.2.0.0";
        public const string AssemblyProduct = "Keepers Of Creation";
        public const string AssemblyTitle = "Appalachia.Audio.Contextual";
        public const string AssemblyTrademark = "Keepers Of Creation";
        public const string AssemblyVersion = "0.2.0.0";
        public const string BuildDate = "2021-12-30T17:59:49.7060560Z";
        public const string Name = "Audio/Contextual";
        public const string NeutralResourcesLanguage = "en";
        public const string Prefix = Root + Name + "/";
        public const string Root = "Appalachia/";
        public const string Version = "0.2.0";

        #endregion

// DO NOT MODIFY: START       

        internal static int ConvertFromVersion(string version)
        {
            using (_PRF_ConvertFromVersion.Auto())
            {
                var parts = version.Split('.');

                var majorString = parts[0];
                var minorString = parts[1];
                var patchString = parts[2];

                var major = int.Parse(majorString);
                var minor = int.Parse(minorString);
                var patch = int.Parse(patchString);

                var result = (major * 1_000_000) + (minor * 1_000) + patch;

                return result;
            }
        }

        internal static string ConvertToVersion(int version)
        {
            using (_PRF_ConvertToVersion.Auto())
            {
                var majorInt = version / 1_000_000;
                var minorInt = (version / 1_000) % 1_000;
                var patchInt = version % 1_000;

                var result = string.Format("0.1.2", majorInt, minorInt, patchInt);

                return result;
            }
        }

        #region Nested type: Menu

        public static partial class Menu
        {
            #region Nested type: Appalachia

            public static partial class Appalachia
            {
                #region Constants and Static Readonly

                public const int Priority = PKG.Priority;

                #endregion

                #region Nested type: Add

                public static partial class Add
                {
                    #region Constants and Static Readonly

                    public const int Priority = Components.Priority + 10000;
                    public const string Base = Root + nameof(Add) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Behaviours

                public static partial class Behaviours
                {
                    #region Constants and Static Readonly

                    public const int Priority = Appalachia.Priority;
                    public const string Base = Root + nameof(Behaviours) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Components

                public static partial class Components
                {
                    #region Constants and Static Readonly

                    public const int Priority = Behaviours.Priority + 10000;
                    public const string Base = Root + nameof(Components) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Create

                public static partial class Create
                {
                    #region Constants and Static Readonly

                    public const int Priority = Add.Priority + 10000;
                    public const string Base = Root + nameof(Create) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Data

                public static partial class Data
                {
                    #region Constants and Static Readonly

                    public const int Priority = Manage.Priority + 10000;
                    public const string Base = Root + nameof(Data) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Debug

                public static partial class Debug
                {
                    #region Constants and Static Readonly

                    public const int Priority = External.Priority + 10000;
                    public const string Base = Root + nameof(Debug) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: External

                public static partial class External
                {
                    #region Constants and Static Readonly

                    public const int Priority = Packages.Priority + 10000;
                    public const string Base = Root + nameof(External) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Gizmos

                public static partial class Gizmos
                {
                    #region Constants and Static Readonly

                    public const int Priority = Debug.Priority + 10000;
                    public const string Base = Root + nameof(Gizmos) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Jobs

                public static partial class Jobs
                {
                    #region Constants and Static Readonly

                    public const int Priority = Tools.Priority + 10000;
                    public const string Base = Root + nameof(Jobs) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Logging

                public static partial class Logging
                {
                    #region Constants and Static Readonly

                    public const int Priority = Windows.Priority + 10000;
                    public const string Base = Root + nameof(Logging) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Manage

                public static partial class Manage
                {
                    #region Constants and Static Readonly

                    public const int Priority = Update.Priority + 10000;
                    public const string Base = Root + nameof(Manage) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Packages

                public static partial class Packages
                {
                    #region Constants and Static Readonly

                    public const int Priority = Tasks.Priority + 10000;
                    public const string Base = Root + nameof(Packages) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: RootTools

                public static partial class RootTools
                {
                    #region Constants and Static Readonly

                    public const int Priority = 0;
                    public const string Base = Root + "Tools/";

                    #endregion
                }

                #endregion

                #region Nested type: Settings

                public static partial class Settings
                {
                    #region Constants and Static Readonly

                    public const int Priority = Logging.Priority + 10000;
                    public const string Base = Root + nameof(Settings) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: State

                public static partial class State
                {
                    #region Constants and Static Readonly

                    public const int Priority = Data.Priority + 10000;
                    public const string Base = Root + nameof(State) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Tasks

                public static partial class Tasks
                {
                    #region Constants and Static Readonly

                    public const int Priority = Settings.Priority + 10000;
                    public const string Base = Root + nameof(Tasks) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Timing

                public static partial class Timing
                {
                    #region Constants and Static Readonly

                    public const int Priority = Jobs.Priority + 10000;
                    public const string Base = Root + nameof(Timing) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Tools

                public static partial class Tools
                {
                    #region Constants and Static Readonly

                    public const int Priority = State.Priority + 100;
                    public const string Base = Root + nameof(Tools) + "/" + Name + "/";

                    #endregion

                    #region Nested type: Disable

                    public static partial class Disable
                    {
                        #region Constants and Static Readonly

                        public const int Priority = Tools.Priority + 1;
                        public const string Base = Tools.Base + nameof(Disable);

                        #endregion
                    }

                    #endregion

                    #region Nested type: Enable

                    public static partial class Enable
                    {
                        #region Constants and Static Readonly

                        public const int Priority = Tools.Priority + 0;
                        public const string Base = Tools.Base + nameof(Enable);

                        #endregion
                    }

                    #endregion
                }

                #endregion

                #region Nested type: Update

                public static partial class Update
                {
                    #region Constants and Static Readonly

                    public const int Priority = Create.Priority + 10000;
                    public const string Base = Root + nameof(Update) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Utility

                public static partial class Utility
                {
                    #region Constants and Static Readonly

                    public const int Priority = Timing.Priority + 10000;
                    public const string Base = Root + nameof(Utility) + "/" + Name + "/";

                    #endregion
                }

                #endregion

                #region Nested type: Windows

                public static partial class Windows
                {
                    #region Constants and Static Readonly

                    public const int Priority = Utility.Priority + 10000;
                    public const string Base = Root + nameof(Windows) + "/" + Name + "/";

                    #endregion
                }

                #endregion
            }

            #endregion

            #region Nested type: Assets

            public static partial class Assets
            {
                #region Constants and Static Readonly

                public const int Priority = PKG.Priority;
                public const string Base = "Assets/" + Prefix;

                #endregion
            }

            #endregion

            #region Nested type: CONTEXT

            public static partial class CONTEXT
            {
                #region Constants and Static Readonly

                public const int Priority = PKG.Priority;
                public const string Mid = "/" + Prefix;
                public const string Mid_short = "/" + Root;
                public const string Start = "CONTEXT/";

                #endregion
            }

            #endregion

            #region Nested type: GameObjects

            public static partial class GameObjects
            {
                #region Constants and Static Readonly

                public const int Priority = PKG.Priority;
                public const string Base = "GameObject/" + Prefix;

                #endregion

                #region Nested type: Create

                public static partial class Create
                {
                    #region Constants and Static Readonly

                    public const int Priority = GameObjects.Priority + 0;
                    public const string Base = "GameObject/Create/" + Prefix;

                    #endregion
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #region Nested type: Prefs

        public static partial class Prefs
        {
            #region Constants and Static Readonly

            public const string Group = Prefix;

            #endregion

            #region Nested type: Gizmos

            public static partial class Gizmos
            {
                #region Constants and Static Readonly

                public const string Base = Group + "Gizmos/";

                #endregion
            }

            #endregion
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(PKG) + ".";

        private static readonly Unity.Profiling.ProfilerMarker _PRF_ConvertToVersion =
            new Unity.Profiling.ProfilerMarker(_PRF_PFX + nameof(ConvertToVersion));

        private static readonly Unity.Profiling.ProfilerMarker _PRF_ConvertFromVersion =
            new Unity.Profiling.ProfilerMarker(_PRF_PFX + nameof(ConvertFromVersion));

        #endregion

// DO NOT MODIFY: END

        #region User Modifiable

        #endregion // User Modifiable
    }
}

// DO NOT MODIFY: END
