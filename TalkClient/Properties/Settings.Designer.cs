﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TalkClient.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int ScreenshotCompositingQuality {
            get {
                return ((int)(this["ScreenshotCompositingQuality"]));
            }
            set {
                this["ScreenshotCompositingQuality"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int ScreenshotInterpolationMode {
            get {
                return ((int)(this["ScreenshotInterpolationMode"]));
            }
            set {
                this["ScreenshotInterpolationMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1920")]
        public int ScreenshotDownsampleSizeX {
            get {
                return ((int)(this["ScreenshotDownsampleSizeX"]));
            }
            set {
                this["ScreenshotDownsampleSizeX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1080")]
        public int ScreenshotDownsampleSizeY {
            get {
                return ((int)(this["ScreenshotDownsampleSizeY"]));
            }
            set {
                this["ScreenshotDownsampleSizeY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("A")]
        public global::System.Windows.Forms.Keys PTTKey {
            get {
                return ((global::System.Windows.Forms.Keys)(this["PTTKey"]));
            }
            set {
                this["PTTKey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ScreenshotDownsample {
            get {
                return ((bool)(this["ScreenshotDownsample"]));
            }
            set {
                this["ScreenshotDownsample"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UsePTT {
            get {
                return ((bool)(this["UsePTT"]));
            }
            set {
                this["UsePTT"] = value;
            }
        }
    }
}
