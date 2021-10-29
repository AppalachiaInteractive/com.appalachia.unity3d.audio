// ReSharper disable All
// DO NOT MODIFY: START

using Appalachia.Utility.Constants;


namespace Appalachia.Audio
{
    internal static partial class PKG
    {
        public const string Root = "Appalachia/";
        public const string Prefix = Root + "Audio/";
        public const int Priority = 1000*6;
        
        public static partial class Prefs
        {
            public const string Group = Prefix;

            public static partial class Gizmos
            {
                public const string Base = Group + "Gizmos/";
            }
        }       

        public static partial class Menu
        {
            public static partial class Assets
            {
                public const int Priority = PKG.Priority;
                public const string Base =  "Assets/" + Prefix;
            }

            public static partial class GameObject
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
                    public const string Base =  Prefix + nameof(Behaviours) + "/"; 
                }
                
                public static partial class Components
                { 
                    public const int Priority = Behaviours.Priority + 100;
                    public const string Base = Prefix + nameof(Components) + "/";
                }

                public static partial class Add
                { 
                    public const int Priority = Components.Priority + 100;
                    public const string Base = Prefix + nameof(Add) +  "/";
                }
                
                public static partial class Create
                { 
                    public const int Priority = Add.Priority + 100;
                    public const string Base = Prefix + nameof(Create) +  "/";
                }
                
                public static partial class Update
                { 
                    public const int Priority = Create.Priority + 100;
                    public const string Base = Prefix + nameof(Update) +  "/";
                }
                
                public static partial class Manage
                { 
                    public const int Priority = Update.Priority + 100;
                    public const string Base = Prefix + nameof(Manage) +  "/";
                }
                
                public static partial class Data
                { 
                    public const int Priority = Manage.Priority + 100;
                    public const string Base = Prefix + nameof(Data) +  "/";
                }
                
                public static partial class RootTools
                { 
                    public const int Priority = Data.Priority + 100;
                    public const string Base = Root + "Tools/";
                }
                
                public static partial class State
                { 
                    public const int Priority = RootTools.Priority + 100;
                    public const string Base = Prefix + nameof(State) +  "/";
                }
                
                public static partial class Tools
                { 
                    public const int Priority = State.Priority + 100;
                    public const string Base = Prefix + nameof(Tools) +  "/";
                                        
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
                    public const int Priority = Tools.Priority + 100;
                    public const string Base = Prefix + nameof(Jobs) +  "/";
                }
                
                public static partial class Timing
                { 
                    public const int Priority = Jobs.Priority + 100;
                    public const string Base = Prefix + nameof(Timing) +  "/";
                }
                                
                public static partial class Utility
                { 
                    public const int Priority = Timing.Priority + 100;
                    public const string Base = Prefix + nameof(Utility) +  "/";
                }
                
                public static partial class Windows
                { 
                    public const int Priority = Utility.Priority + 100;
                    public const string Base = Prefix + nameof(Windows) +  "/";
                }
                
                public static partial class Logging
                { 
                    public const int Priority = Windows.Priority + 100;
                    public const string Base = Prefix + nameof(Logging) +  "/";
                }          
                
                public static partial class Settings
                { 
                    public const int Priority = Logging.Priority + 100;
                    public const string Base = Prefix + nameof(Settings) + "/";
                }               
                
                public static partial class Packages
                { 
                    public const int Priority = Settings.Priority + 100;
                    public const string Base = Prefix + nameof(Packages) + "/";
                }                          
                
                public static partial class External
                { 
                    public const int Priority = Packages.Priority + 100;
                    public const string Base = Prefix + nameof(External) + "/";
                }           
                
                public static partial class Debug
                { 
                    public const int Priority = External.Priority + 100;
                    public const string Base = Prefix + nameof(Debug) +  "/";
                }                                               
                
                public static partial class Gizmos
                { 
                    public const int Priority = Debug.Priority + 100;
                    public const string Base = Prefix + nameof(Gizmos) +  "/";
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
// MODIFICATIONS ALLOWED: START

// MODIFICATIONS ALLOWED: END
// DO NOT MODIFY: START        
    }
}
// DO NOT MODIFY: END