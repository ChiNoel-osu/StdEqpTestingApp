﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StdEqpTesting.Localization {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Loc {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Loc() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("StdEqpTesting.Localization.Loc", typeof(Loc).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
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
        ///   查找类似 Login 的本地化字符串。
        /// </summary>
        public static string LoginButton {
            get {
                return ResourceManager.GetString("LoginButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Please enter your credentials below. 的本地化字符串。
        /// </summary>
        public static string LoginEnterCred {
            get {
                return ResourceManager.GetString("LoginEnterCred", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Password: 的本地化字符串。
        /// </summary>
        public static string LoginPassword {
            get {
                return ResourceManager.GetString("LoginPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Username: 的本地化字符串。
        /// </summary>
        public static string LoginUsername {
            get {
                return ResourceManager.GetString("LoginUsername", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Welcome! 的本地化字符串。
        /// </summary>
        public static string LoginWelcome {
            get {
                return ResourceManager.GetString("LoginWelcome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Login 的本地化字符串。
        /// </summary>
        public static string LoginWndTitle {
            get {
                return ResourceManager.GetString("LoginWndTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Loading 的本地化字符串。
        /// </summary>
        public static string MainWndTitle {
            get {
                return ResourceManager.GetString("MainWndTitle", resourceCulture);
            }
        }
    }
}
