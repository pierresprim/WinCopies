﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WinCopies.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WinCopies.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have multiple windows open. Are you sure you want to close the application?.
        /// </summary>
        public static string ApplicationClosingMessage {
            get {
                return ResourceManager.GetString("ApplicationClosingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Close tabs to the left or right.
        /// </summary>
        public static string CloseTabsToTheLeftOrRight {
            get {
                return ResourceManager.GetString("CloseTabsToTheLeftOrRight", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {\rtf1\ansi\deff0{\fonttbl{\f0 \fswiss Helvetica;}{\f1 Courier;}}
        ///{\colortbl;\red255\green0\blue0;\red0\green0\blue255;}
        ///\widowctrl\hyphauto
        ///
        ///{\pard \ql \f0 \sa180 \li0 \fi0 \b \fs28 GNU GENERAL PUBLIC LICENSE\par}
        ///{\pard \ql \f0 \sa180 \li0 \fi0 Version 3, 29 June 2007\par}
        ///{\pard \ql \f0 \sa180 \li0 \fi0 Copyright \u169? 2007 Free Software Foundation, Inc. &lt;{\field{\*\fldinst{HYPERLINK &quot;https://fsf.org/&quot;}}{\fldrslt{\ul
        ///https://fsf.org/
        ///}}}
        ///&gt;\par}
        ///{\pard \ql \f0 \sa180 \li0 \fi0 Everyone is permi [rest of string was truncated]&quot;;.
        /// </summary>
        public static string gpl_3_0 {
            get {
                return ResourceManager.GetString("gpl_3_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The clipboard operation did not succeeded..
        /// </summary>
        public static string SetClipboardErrorMessage {
            get {
                return ResourceManager.GetString("SetClipboardErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        public static System.Drawing.Icon WinCopies {
            get {
                object obj = ResourceManager.GetObject("WinCopies", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
    }
}
