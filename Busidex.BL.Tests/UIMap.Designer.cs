﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by coded UI test builder.
//      Version: 11.0.0.0
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

namespace Busidex.BL.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public partial class UIMap
    {
        
        /// <summary>
        /// ViewMyBusidex - Use 'ViewMyBusidexParams' to pass parameters into this method.
        /// </summary>
        public void ViewMyBusidex()
        {
            #region Variable Declarations
            HtmlHyperlink uILoginHyperlink = this.UIBusidexWindowsInternWindow.UIBusidexDocument1.UILoginHyperlink;
            HtmlEdit uIUsernameEdit = this.UIBusidexWindowsInternWindow.UIBusidexDocument1.UIUsernameEdit;
            HtmlEdit uIPasswordEdit = this.UIBusidexWindowsInternWindow.UIBusidexDocument1.UIPasswordEdit;
            HtmlInputButton uILoginButton = this.UIBusidexWindowsInternWindow.UIBusidexDocument1.UILoginFormCustom.UILoginButton;
            HtmlTextArea uINotesEdit = this.UIBusidexWindowsInternWindow.UIMineDocument.UINotesEdit;
            HtmlHyperlink uILogoffHyperlink = this.UIBusidexWindowsInternWindow.UIMineDocument.UILoginCustom.UILogoffHyperlink;
            #endregion

            // Click 'Log in' link
            Mouse.Click(uILoginHyperlink, new Point(20, 8));

            // Type 'lizzabethbrown' in 'User name' text box
            uIUsernameEdit.Text = this.ViewMyBusidexParams.UIUsernameEditText;

            // Type '{Tab}' in 'User name' text box
            Keyboard.SendKeys(uIUsernameEdit, this.ViewMyBusidexParams.UIUsernameEditSendKeys, ModifierKeys.None);

            // Type '********' in 'Password' text box
            uIPasswordEdit.Password = this.ViewMyBusidexParams.UIPasswordEditPassword;

            // Click 'Log in' button
            Mouse.Click(uILoginButton, new Point(38, 13));

            // Click 'Notes' text box
            Mouse.Click(uINotesEdit, new Point(162, 27));

            // Click 'Log off' link
            Mouse.Click(uILogoffHyperlink, new Point(28, 9));
        }
        
        #region Properties
        public virtual ViewMyBusidexParams ViewMyBusidexParams
        {
            get
            {
                if ((this.mViewMyBusidexParams == null))
                {
                    this.mViewMyBusidexParams = new ViewMyBusidexParams();
                }
                return this.mViewMyBusidexParams;
            }
        }
        
        public UIBusidexGoogleChromeWindow UIBusidexGoogleChromeWindow
        {
            get
            {
                if ((this.mUIBusidexGoogleChromeWindow == null))
                {
                    this.mUIBusidexGoogleChromeWindow = new UIBusidexGoogleChromeWindow();
                }
                return this.mUIBusidexGoogleChromeWindow;
            }
        }
        
        public UIBusidexWindowsInternWindow UIBusidexWindowsInternWindow
        {
            get
            {
                if ((this.mUIBusidexWindowsInternWindow == null))
                {
                    this.mUIBusidexWindowsInternWindow = new UIBusidexWindowsInternWindow();
                }
                return this.mUIBusidexWindowsInternWindow;
            }
        }
        #endregion
        
        #region Fields
        private ViewMyBusidexParams mViewMyBusidexParams;
        
        private UIBusidexGoogleChromeWindow mUIBusidexGoogleChromeWindow;
        
        private UIBusidexWindowsInternWindow mUIBusidexWindowsInternWindow;
        #endregion
    }
    
    /// <summary>
    /// Parameters to be passed into 'ViewMyBusidex'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class ViewMyBusidexParams
    {
        
        #region Fields
        /// <summary>
        /// Type 'lizzabethbrown' in 'User name' text box
        /// </summary>
        public string UIUsernameEditText = "lizzabethbrown";
        
        /// <summary>
        /// Type '{Tab}' in 'User name' text box
        /// </summary>
        public string UIUsernameEditSendKeys = "{Tab}";
        
        /// <summary>
        /// Type '********' in 'Password' text box
        /// </summary>
        public string UIPasswordEditPassword = "r3KXCwrkaqV66FjR1qybVCIOWoq3golG";
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIBusidexGoogleChromeWindow : WinWindow
    {
        
        public UIBusidexGoogleChromeWindow()
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.Name] = "Busidex - Google Chrome";
            this.SearchProperties[WinWindow.PropertyNames.ClassName] = "Chrome_WidgetWin_1";
            this.WindowTitles.Add("Busidex - Google Chrome");
            this.WindowTitles.Add("Mine - Google Chrome");
            #endregion
        }
        
        #region Properties
        public UIItemWindow UIItemWindow
        {
            get
            {
                if ((this.mUIItemWindow == null))
                {
                    this.mUIItemWindow = new UIItemWindow(this);
                }
                return this.mUIItemWindow;
            }
        }
        #endregion
        
        #region Fields
        private UIItemWindow mUIItemWindow;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIItemWindow : WinWindow
    {
        
        public UIItemWindow(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.ControlId] = "135294464";
            this.WindowTitles.Add("Busidex - Google Chrome");
            this.WindowTitles.Add("Mine - Google Chrome");
            #endregion
        }
        
        #region Properties
        public WinControl UIItemDocument
        {
            get
            {
                if ((this.mUIItemDocument == null))
                {
                    this.mUIItemDocument = new WinControl(this);
                    #region Search Criteria
                    this.mUIItemDocument.SearchProperties[UITestControl.PropertyNames.ControlType] = "Document";
                    this.mUIItemDocument.WindowTitles.Add("Busidex - Google Chrome");
                    this.mUIItemDocument.WindowTitles.Add("Mine - Google Chrome");
                    #endregion
                }
                return this.mUIItemDocument;
            }
        }
        
        public WinCustom UIItemCustom
        {
            get
            {
                if ((this.mUIItemCustom == null))
                {
                    this.mUIItemCustom = new WinCustom(this);
                    #region Search Criteria
                    this.mUIItemCustom.WindowTitles.Add("Busidex - Google Chrome");
                    #endregion
                }
                return this.mUIItemCustom;
            }
        }
        #endregion
        
        #region Fields
        private WinControl mUIItemDocument;
        
        private WinCustom mUIItemCustom;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIBusidexWindowsInternWindow : BrowserWindow
    {
        
        public UIBusidexWindowsInternWindow()
        {
            #region Search Criteria
            this.SearchProperties[UITestControl.PropertyNames.Name] = "Busidex";
            this.SearchProperties[UITestControl.PropertyNames.ClassName] = "IEFrame";
            this.WindowTitles.Add("Busidex");
            this.WindowTitles.Add("Mine");
            #endregion
        }
        
        public void LaunchUrl(System.Uri url)
        {
            this.CopyFrom(BrowserWindow.Launch(url));
        }
        
        #region Properties
        public UIBusidexDocument UIBusidexDocument
        {
            get
            {
                if ((this.mUIBusidexDocument == null))
                {
                    this.mUIBusidexDocument = new UIBusidexDocument(this);
                }
                return this.mUIBusidexDocument;
            }
        }
        
        public UINotificationToolBar UINotificationToolBar
        {
            get
            {
                if ((this.mUINotificationToolBar == null))
                {
                    this.mUINotificationToolBar = new UINotificationToolBar(this);
                }
                return this.mUINotificationToolBar;
            }
        }
        
        public UIMineDocument UIMineDocument
        {
            get
            {
                if ((this.mUIMineDocument == null))
                {
                    this.mUIMineDocument = new UIMineDocument(this);
                }
                return this.mUIMineDocument;
            }
        }
        
        public UIBusidexDocument1 UIBusidexDocument1
        {
            get
            {
                if ((this.mUIBusidexDocument1 == null))
                {
                    this.mUIBusidexDocument1 = new UIBusidexDocument1(this);
                }
                return this.mUIBusidexDocument1;
            }
        }
        #endregion
        
        #region Fields
        private UIBusidexDocument mUIBusidexDocument;
        
        private UINotificationToolBar mUINotificationToolBar;
        
        private UIMineDocument mUIMineDocument;
        
        private UIBusidexDocument1 mUIBusidexDocument1;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIBusidexDocument : HtmlDocument
    {
        
        public UIBusidexDocument(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[HtmlDocument.PropertyNames.Id] = null;
            this.SearchProperties[HtmlDocument.PropertyNames.RedirectingPage] = "False";
            this.SearchProperties[HtmlDocument.PropertyNames.FrameDocument] = "False";
            this.FilterProperties[HtmlDocument.PropertyNames.Title] = "Busidex";
            this.FilterProperties[HtmlDocument.PropertyNames.AbsolutePath] = "/Home";
            this.FilterProperties[HtmlDocument.PropertyNames.PageUrl] = "http://local.busidex.com/Home";
            this.WindowTitles.Add("Busidex");
            #endregion
        }
        
        #region Properties
        public HtmlHyperlink UILoginHyperlink
        {
            get
            {
                if ((this.mUILoginHyperlink == null))
                {
                    this.mUILoginHyperlink = new HtmlHyperlink(this);
                    #region Search Criteria
                    this.mUILoginHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Id] = "loginLink";
                    this.mUILoginHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Name] = null;
                    this.mUILoginHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Target] = null;
                    this.mUILoginHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.InnerText] = "Log in";
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.AbsolutePath] = "/Account/Login";
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Title] = null;
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Href] = "http://local.busidex.com/Account/Login";
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Class] = null;
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.ControlDefinition] = "id=\"loginLink\" href=\"/Account/Login\" dat";
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.TagInstance] = "3";
                    this.mUILoginHyperlink.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUILoginHyperlink;
            }
        }
        
        public UIBodyPane UIBodyPane
        {
            get
            {
                if ((this.mUIBodyPane == null))
                {
                    this.mUIBodyPane = new UIBodyPane(this);
                }
                return this.mUIBodyPane;
            }
        }
        
        public HtmlEdit UIUsernameEdit
        {
            get
            {
                if ((this.mUIUsernameEdit == null))
                {
                    this.mUIUsernameEdit = new HtmlEdit(this);
                    #region Search Criteria
                    this.mUIUsernameEdit.SearchProperties[HtmlEdit.PropertyNames.Id] = "UserName";
                    this.mUIUsernameEdit.SearchProperties[HtmlEdit.PropertyNames.Name] = "UserName";
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.LabeledBy] = "User name";
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.Type] = "SINGLELINE";
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.Title] = null;
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.Class] = null;
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.ControlDefinition] = "name=\"UserName\" id=\"UserName\" type=\"text";
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = "2";
                    this.mUIUsernameEdit.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUIUsernameEdit;
            }
        }
        
        public HtmlEdit UIPasswordEdit
        {
            get
            {
                if ((this.mUIPasswordEdit == null))
                {
                    this.mUIPasswordEdit = new HtmlEdit(this);
                    #region Search Criteria
                    this.mUIPasswordEdit.SearchProperties[HtmlEdit.PropertyNames.Id] = "Password";
                    this.mUIPasswordEdit.SearchProperties[HtmlEdit.PropertyNames.Name] = "Password";
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.LabeledBy] = "Password";
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.Type] = "PASSWORD";
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.Title] = null;
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.Class] = null;
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.ControlDefinition] = "name=\"Password\" id=\"Password\" type=\"pass";
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = "3";
                    this.mUIPasswordEdit.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUIPasswordEdit;
            }
        }
        
        public UILoginFormCustom UILoginFormCustom
        {
            get
            {
                if ((this.mUILoginFormCustom == null))
                {
                    this.mUILoginFormCustom = new UILoginFormCustom(this);
                }
                return this.mUILoginFormCustom;
            }
        }
        #endregion
        
        #region Fields
        private HtmlHyperlink mUILoginHyperlink;
        
        private UIBodyPane mUIBodyPane;
        
        private HtmlEdit mUIUsernameEdit;
        
        private HtmlEdit mUIPasswordEdit;
        
        private UILoginFormCustom mUILoginFormCustom;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIBodyPane : HtmlDiv
    {
        
        public UIBodyPane(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[HtmlDiv.PropertyNames.Id] = "body";
            this.SearchProperties[HtmlDiv.PropertyNames.Name] = null;
            this.FilterProperties[HtmlDiv.PropertyNames.InnerText] = "\r\n\r\nBusi-What?\r\n  \r\nBusi-dex is the busi";
            this.FilterProperties[HtmlDiv.PropertyNames.Title] = null;
            this.FilterProperties[HtmlDiv.PropertyNames.Class] = null;
            this.FilterProperties[HtmlDiv.PropertyNames.ControlDefinition] = "id=\"body\"";
            this.FilterProperties[HtmlDiv.PropertyNames.TagInstance] = "4";
            this.WindowTitles.Add("Busidex");
            #endregion
        }
        
        #region Properties
        public HtmlCustom UIItemCustom
        {
            get
            {
                if ((this.mUIItemCustom == null))
                {
                    this.mUIItemCustom = new HtmlCustom(this);
                    #region Search Criteria
                    this.mUIItemCustom.SearchProperties["Id"] = null;
                    this.mUIItemCustom.SearchProperties[UITestControl.PropertyNames.Name] = null;
                    this.mUIItemCustom.SearchProperties["TagName"] = "SECTION";
                    this.mUIItemCustom.FilterProperties["Class"] = "feature";
                    this.mUIItemCustom.FilterProperties["ControlDefinition"] = "class=\"feature\"";
                    this.mUIItemCustom.FilterProperties["TagInstance"] = "6";
                    this.mUIItemCustom.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUIItemCustom;
            }
        }
        #endregion
        
        #region Fields
        private HtmlCustom mUIItemCustom;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UILoginFormCustom : HtmlCustom
    {
        
        public UILoginFormCustom(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties["Id"] = "LoginForm";
            this.SearchProperties[UITestControl.PropertyNames.Name] = null;
            this.SearchProperties["TagName"] = "FORM";
            this.FilterProperties["Class"] = null;
            this.FilterProperties["ControlDefinition"] = "id=\"LoginForm\" action=\"/Account/JsonLogi";
            this.FilterProperties["TagInstance"] = "1";
            this.WindowTitles.Add("Busidex");
            #endregion
        }
        
        #region Properties
        public HtmlInputButton UILoginButton
        {
            get
            {
                if ((this.mUILoginButton == null))
                {
                    this.mUILoginButton = new HtmlInputButton(this);
                    #region Search Criteria
                    this.mUILoginButton.SearchProperties[HtmlButton.PropertyNames.Id] = null;
                    this.mUILoginButton.SearchProperties[HtmlButton.PropertyNames.Name] = null;
                    this.mUILoginButton.SearchProperties[HtmlButton.PropertyNames.DisplayText] = "Log in";
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.Type] = "submit";
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.Title] = null;
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.Class] = null;
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.ControlDefinition] = "type=\"submit\" value=\"Log in\"";
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.TagInstance] = "6";
                    this.mUILoginButton.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUILoginButton;
            }
        }
        #endregion
        
        #region Fields
        private HtmlInputButton mUILoginButton;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UINotificationToolBar : WinToolBar
    {
        
        public UINotificationToolBar(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinToolBar.PropertyNames.Name] = "Notification";
            this.WindowTitles.Add("Busidex");
            #endregion
        }
        
        #region Properties
        public WinButton UIYesButton
        {
            get
            {
                if ((this.mUIYesButton == null))
                {
                    this.mUIYesButton = new WinButton(this);
                    #region Search Criteria
                    this.mUIYesButton.SearchProperties[WinButton.PropertyNames.Name] = "Yes";
                    this.mUIYesButton.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUIYesButton;
            }
        }
        #endregion
        
        #region Fields
        private WinButton mUIYesButton;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIMineDocument : HtmlDocument
    {
        
        public UIMineDocument(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[HtmlDocument.PropertyNames.Id] = null;
            this.SearchProperties[HtmlDocument.PropertyNames.RedirectingPage] = "False";
            this.SearchProperties[HtmlDocument.PropertyNames.FrameDocument] = "False";
            this.FilterProperties[HtmlDocument.PropertyNames.Title] = "Mine";
            this.FilterProperties[HtmlDocument.PropertyNames.AbsolutePath] = "/Busidex/Mine";
            this.FilterProperties[HtmlDocument.PropertyNames.PageUrl] = "http://local.busidex.com/Busidex/Mine";
            this.WindowTitles.Add("Mine");
            #endregion
        }
        
        #region Properties
        public HtmlTextArea UINotesEdit
        {
            get
            {
                if ((this.mUINotesEdit == null))
                {
                    this.mUINotesEdit = new HtmlTextArea(this);
                    #region Search Criteria
                    this.mUINotesEdit.SearchProperties[HtmlEdit.PropertyNames.Id] = null;
                    this.mUINotesEdit.SearchProperties[HtmlEdit.PropertyNames.Name] = "Notes";
                    this.mUINotesEdit.SearchProperties[HtmlEdit.PropertyNames.LabeledBy] = null;
                    this.mUINotesEdit.FilterProperties[HtmlEdit.PropertyNames.Title] = null;
                    this.mUINotesEdit.FilterProperties[HtmlEdit.PropertyNames.Class] = "Notes";
                    this.mUINotesEdit.FilterProperties[HtmlEdit.PropertyNames.ControlDefinition] = "name=\"Notes\" class=\"Notes\" style=\"font-s";
                    this.mUINotesEdit.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = "1";
                    this.mUINotesEdit.WindowTitles.Add("Mine");
                    #endregion
                }
                return this.mUINotesEdit;
            }
        }
        
        public UILoginCustom UILoginCustom
        {
            get
            {
                if ((this.mUILoginCustom == null))
                {
                    this.mUILoginCustom = new UILoginCustom(this);
                }
                return this.mUILoginCustom;
            }
        }
        #endregion
        
        #region Fields
        private HtmlTextArea mUINotesEdit;
        
        private UILoginCustom mUILoginCustom;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UILoginCustom : HtmlCustom
    {
        
        public UILoginCustom(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties["Id"] = "login";
            this.SearchProperties[UITestControl.PropertyNames.Name] = null;
            this.SearchProperties["TagName"] = "SECTION";
            this.FilterProperties["Class"] = null;
            this.FilterProperties["ControlDefinition"] = "id=\"login\"";
            this.FilterProperties["TagInstance"] = "1";
            this.WindowTitles.Add("Mine");
            #endregion
        }
        
        #region Properties
        public HtmlHyperlink UILogoffHyperlink
        {
            get
            {
                if ((this.mUILogoffHyperlink == null))
                {
                    this.mUILogoffHyperlink = new HtmlHyperlink(this);
                    #region Search Criteria
                    this.mUILogoffHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Id] = null;
                    this.mUILogoffHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Name] = null;
                    this.mUILogoffHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Target] = null;
                    this.mUILogoffHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.InnerText] = "Log off";
                    this.mUILogoffHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.AbsolutePath] = "/Account/LogOff";
                    this.mUILogoffHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Title] = null;
                    this.mUILogoffHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Href] = "http://local.busidex.com/Account/LogOff";
                    this.mUILogoffHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Class] = null;
                    this.mUILogoffHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.ControlDefinition] = "href=\"/Account/LogOff\"";
                    this.mUILogoffHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.TagInstance] = "2";
                    this.mUILogoffHyperlink.WindowTitles.Add("Mine");
                    #endregion
                }
                return this.mUILogoffHyperlink;
            }
        }
        #endregion
        
        #region Fields
        private HtmlHyperlink mUILogoffHyperlink;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UIBusidexDocument1 : HtmlDocument
    {
        
        public UIBusidexDocument1(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[HtmlDocument.PropertyNames.Id] = null;
            this.SearchProperties[HtmlDocument.PropertyNames.RedirectingPage] = "False";
            this.SearchProperties[HtmlDocument.PropertyNames.FrameDocument] = "False";
            this.FilterProperties[HtmlDocument.PropertyNames.Title] = "Busidex";
            this.FilterProperties[HtmlDocument.PropertyNames.AbsolutePath] = "/";
            this.FilterProperties[HtmlDocument.PropertyNames.PageUrl] = "http://local.busidex.com/";
            this.WindowTitles.Add("Busidex");
            #endregion
        }
        
        #region Properties
        public HtmlHyperlink UILoginHyperlink
        {
            get
            {
                if ((this.mUILoginHyperlink == null))
                {
                    this.mUILoginHyperlink = new HtmlHyperlink(this);
                    #region Search Criteria
                    this.mUILoginHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Id] = "loginLink";
                    this.mUILoginHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Name] = null;
                    this.mUILoginHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.Target] = null;
                    this.mUILoginHyperlink.SearchProperties[HtmlHyperlink.PropertyNames.InnerText] = "Log in";
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.AbsolutePath] = "/Account/Login";
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Title] = null;
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Href] = "http://local.busidex.com/Account/Login";
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.Class] = null;
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.ControlDefinition] = "id=\"loginLink\" href=\"/Account/Login\" dat";
                    this.mUILoginHyperlink.FilterProperties[HtmlHyperlink.PropertyNames.TagInstance] = "3";
                    this.mUILoginHyperlink.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUILoginHyperlink;
            }
        }
        
        public HtmlEdit UIUsernameEdit
        {
            get
            {
                if ((this.mUIUsernameEdit == null))
                {
                    this.mUIUsernameEdit = new HtmlEdit(this);
                    #region Search Criteria
                    this.mUIUsernameEdit.SearchProperties[HtmlEdit.PropertyNames.Id] = "UserName";
                    this.mUIUsernameEdit.SearchProperties[HtmlEdit.PropertyNames.Name] = "UserName";
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.LabeledBy] = "User name";
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.Type] = "SINGLELINE";
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.Title] = null;
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.Class] = null;
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.ControlDefinition] = "name=\"UserName\" id=\"UserName\" type=\"text";
                    this.mUIUsernameEdit.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = "2";
                    this.mUIUsernameEdit.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUIUsernameEdit;
            }
        }
        
        public HtmlEdit UIPasswordEdit
        {
            get
            {
                if ((this.mUIPasswordEdit == null))
                {
                    this.mUIPasswordEdit = new HtmlEdit(this);
                    #region Search Criteria
                    this.mUIPasswordEdit.SearchProperties[HtmlEdit.PropertyNames.Id] = "Password";
                    this.mUIPasswordEdit.SearchProperties[HtmlEdit.PropertyNames.Name] = "Password";
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.LabeledBy] = "Password";
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.Type] = "PASSWORD";
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.Title] = null;
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.Class] = null;
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.ControlDefinition] = "name=\"Password\" id=\"Password\" type=\"pass";
                    this.mUIPasswordEdit.FilterProperties[HtmlEdit.PropertyNames.TagInstance] = "3";
                    this.mUIPasswordEdit.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUIPasswordEdit;
            }
        }
        
        public UILoginFormCustom1 UILoginFormCustom
        {
            get
            {
                if ((this.mUILoginFormCustom == null))
                {
                    this.mUILoginFormCustom = new UILoginFormCustom1(this);
                }
                return this.mUILoginFormCustom;
            }
        }
        #endregion
        
        #region Fields
        private HtmlHyperlink mUILoginHyperlink;
        
        private HtmlEdit mUIUsernameEdit;
        
        private HtmlEdit mUIPasswordEdit;
        
        private UILoginFormCustom1 mUILoginFormCustom;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "11.0.50727.1")]
    public class UILoginFormCustom1 : HtmlCustom
    {
        
        public UILoginFormCustom1(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties["Id"] = "LoginForm";
            this.SearchProperties[UITestControl.PropertyNames.Name] = null;
            this.SearchProperties["TagName"] = "FORM";
            this.FilterProperties["Class"] = null;
            this.FilterProperties["ControlDefinition"] = "id=\"LoginForm\" action=\"/Account/JsonLogi";
            this.FilterProperties["TagInstance"] = "1";
            this.WindowTitles.Add("Busidex");
            #endregion
        }
        
        #region Properties
        public HtmlInputButton UILoginButton
        {
            get
            {
                if ((this.mUILoginButton == null))
                {
                    this.mUILoginButton = new HtmlInputButton(this);
                    #region Search Criteria
                    this.mUILoginButton.SearchProperties[HtmlButton.PropertyNames.Id] = null;
                    this.mUILoginButton.SearchProperties[HtmlButton.PropertyNames.Name] = null;
                    this.mUILoginButton.SearchProperties[HtmlButton.PropertyNames.DisplayText] = "Log in";
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.Type] = "submit";
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.Title] = null;
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.Class] = null;
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.ControlDefinition] = "type=\"submit\" value=\"Log in\"";
                    this.mUILoginButton.FilterProperties[HtmlButton.PropertyNames.TagInstance] = "6";
                    this.mUILoginButton.WindowTitles.Add("Busidex");
                    #endregion
                }
                return this.mUILoginButton;
            }
        }
        #endregion
        
        #region Fields
        private HtmlInputButton mUILoginButton;
        #endregion
    }
}
