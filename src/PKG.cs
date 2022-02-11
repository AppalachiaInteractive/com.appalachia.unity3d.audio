// ReSharper disable All
// DO NOT MODIFY: START

using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Appalachia.Utility.Constants;

[assembly: AssemblyTitle("Appalachia.Audio")]
[assembly: AssemblyDescription("Native audio plugins for Unity that provide analysis of audio system output to create reactive audio components.")]
[assembly: AssemblyCompany("Appalachia Interactive")]
[assembly: AssemblyProduct("Keepers Of Creation")]
[assembly: AssemblyCopyright("Copyright © Appalachia Interactive 2022")]
[assembly: AssemblyTrademark("Keepers Of Creation")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: AssemblyVersion("0.2.0.0")]
[assembly: AssemblyFileVersion("0.2.0.0")]

namespace Appalachia.Audio
{
    internal static partial class PKG
    {
        public const string AssemblyTitle = "Appalachia.Audio";
        public const string AssemblyDescription = "Native audio plugins for Unity that provide analysis of audio system output to create reactive audio components.";
        public const string AssemblyCompany = "Appalachia Interactive";
        public const string AssemblyProduct = "Keepers Of Creation";
        public const string AssemblyCopyright = "Copyright © Appalachia Interactive 2022";
        public const string AssemblyTrademark = "Keepers Of Creation";
        public const string AssemblyCulture = "";
        public const string NeutralResourcesLanguage = "en";
        public const string AssemblyVersion = "0.2.0.0";
        public const string AssemblyFileVersion = "0.2.0.0";
        public const int Priority = -414000;
        public const string Name = "Audio";
        public const string Prefix = Root + Name + "/";
        public const string Root = "Appalachia/";
        public const string Version = "0.2.0";
        public const int VersionInt = 2000;
        public const string BuildDate = "2022-02-05T14:11:00.1501223Z";
        
        public static partial class Prefs
        {
            public const string Group = Prefix;

            public static partial class General
            {
                public const string Group = Prefs.Group + "General";
                private const string Base = Group + "/";                
            }

            public static partial class Colors
            {
                public const string Group = Prefs.Group + "Colors";
                private const string Base = Group + "/";                
            }

            public static partial class Gizmos
            {
                public const string Group = Prefs.Group + "Gizmos";
                private const string Base = Group + "/";                
            }
        }       

        public static partial class Menu
        {
             public static partial class Assets
             {
                public const int Priority = PKG.Priority;
                public const string Base =  "Assets/" + Prefix;
            }

            public static partial class GameObjects
            {
                public const int Priority = PKG.Priority;
                public const string Base = "GameObject/" + Prefix;
            }

            public static partial class Appalachia
            {
                public const int Priority = PKG.Priority;

                public static partial class Behaviours
                {
                    public const int Priority = Appalachia.Priority;
                    public const string Base =  Root + nameof(Behaviours) + "/" + Name + "/"; 
                }
                
                public static partial class Components
                { 
                    public const int Priority = Behaviours.Priority + 10000;
                    public const string Base = Root + nameof(Components) + "/" + Name + "/";
                }

                public static partial class Add
                { 
                    public const int Priority = Components.Priority + 10000;
                    public const string Base = Root + nameof(Add) +  "/" + Name + "/";
                }
                
                public static partial class Create
                { 
                    public const int Priority = Add.Priority + 10000;
                    public const string Base = Root + nameof(Create) +  "/" + Name + "/";
                }
                
                public static partial class Update
                { 
                    public const int Priority = Create.Priority + 10000;
                    public const string Base = Root + nameof(Update) +  "/" + Name + "/";
                }
                
                public static partial class Manage
                { 
                    public const int Priority = Update.Priority + 10000;
                    public const string Base = Root + nameof(Manage) +  "/" + Name + "/";
                }
                
                public static partial class Data
                { 
                    public const int Priority = Manage.Priority + 10000;
                    public const string Base = Root + nameof(Data) +  "/" + Name + "/";
                }
                
                public static partial class RootTools
                { 
                    public const int Priority = 0;
                    public const string Base = Root + "Tools/";
                }
                
                public static partial class State
                { 
                    public const int Priority = Data.Priority + 10000;
                    public const string Base = Root + nameof(State) +  "/" + Name + "/";
                }
                
                public static partial class Tools
                { 
                    public const int Priority = State.Priority + 100;
                    public const string Base = Root + nameof(Tools) +  "/" + Name + "/";
                                        
                    public static partial class Enable
                    { 
                        public const int Priority = Tools.Priority + 0;
                        public const string Base = Tools.Base + nameof(Enable);
                    }                                                 
                    
                    public static partial class Disable
                    { 
                        public const int Priority = Tools.Priority + 1;
                        public const string Base = Tools.Base + nameof(Disable);
                    }   
                }
                
                public static partial class Jobs
                { 
                    public const int Priority = Tools.Priority + 10000;
                    public const string Base = Root + nameof(Jobs) +  "/" + Name + "/";
                }
                
                public static partial class Timing
                { 
                    public const int Priority = Jobs.Priority + 10000;
                    public const string Base = Root + nameof(Timing) +  "/" + Name + "/";
                }
                                
                public static partial class Utility
                { 
                    public const int Priority = Timing.Priority + 10000;
                    public const string Base = Root + nameof(Utility) +  "/" + Name + "/";
                }
                
                public static partial class Windows
                { 
                    public const int Priority = Utility.Priority + 10000;
                    public const string Base = Root + nameof(Windows) +  "/" + Name + "/";
                }
                
                public static partial class Logging
                { 
                    public const int Priority = Windows.Priority + 10000;
                    public const string Base = Root + nameof(Logging) +  "/" + Name + "/";
                }          
                
                public static partial class Settings
                { 
                    public const int Priority = Logging.Priority + 10000;
                    public const string Base = Root + nameof(Settings) + "/" + Name + "/";
                }               
                
                public static partial class Tasks
                { 
                    public const int Priority = Settings.Priority + 10000;
                    public const string Base = Root + nameof(Tasks) + "/" + Name + "/";
                }               
                
                public static partial class Packages
                { 
                    public const int Priority = Tasks.Priority + 10000;
                    public const string Base = Root + nameof(Packages) + "/" + Name + "/";
                }                          
                
                public static partial class External
                { 
                    public const int Priority = Packages.Priority + 10000;
                    public const string Base = Root + nameof(External) + "/" + Name + "/";
                }           
                
                public static partial class Debug
                { 
                    public const int Priority = External.Priority + 10000;
                    public const string Base = Root + nameof(Debug) +  "/" + Name + "/";
                }                                               
                
                public static partial class Gizmos
                { 
                    public const int Priority = Debug.Priority + 10000;
                    public const string Base = Root + nameof(Gizmos) +  "/" + Name + "/";
                }                 
            }

            public static partial class CONTEXT
            {
                public const int Priority = PKG.Priority;
                public const string Start = "CONTEXT/";
                public const string Mid = "/" + Prefix;
                public const string Mid_short = "/" + Root;
            }
        }

// DO NOT MODIFY: END
#region User Modifiable
        
#endregion // User Modifiable

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

        #region Profiling

        private const string _PRF_PFX = nameof(PKG) + ".";

        private static readonly Unity.Profiling.ProfilerMarker _PRF_ConvertToVersion =
            new Unity.Profiling.ProfilerMarker(_PRF_PFX + nameof(ConvertToVersion));

        private static readonly Unity.Profiling.ProfilerMarker _PRF_ConvertFromVersion =
            new Unity.Profiling.ProfilerMarker(_PRF_PFX + nameof(ConvertFromVersion));

        #endregion 
    }
}
// DO NOT MODIFY: END