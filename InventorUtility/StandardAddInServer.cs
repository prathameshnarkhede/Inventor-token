using Inventor;
using LoginManager;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace InventorUtility
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("3d39e523-ec83-437d-872d-6efe1ed5e745")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        private string ClientIdString => AddInGuid(typeof(StandardAddInServer));

        public const string OAuth2TokenScopes = "code:all data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete";
        private ButtonDefinition ButtonDef;

        // Inventor application object.
        private Inventor.Application m_inventorApplication;

        public StandardAddInServer()
        {
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            m_inventorApplication = addInSiteObject.Application;

            // TODO: Add ApplicationAddInServer.Activate implementation.
            // e.g. event initialization, command creation etc.
            InitializeRibbonButton();
        }

        private void InitializeRibbonButton()
        {
            ButtonDef = InitButton();

            ButtonDef.OnExecute += ButtonDef_OnExecute;

            InitCommand(ButtonDef);

            InitRibbon(ButtonDef);

            LoginManager.LoginManager.OnAuthenticationFailed += () => { };
        }

        private void InitCommand(ButtonDefinition buttonDef)
        {
            var cmdCat = m_inventorApplication.CommandManager.CommandCategories.Add("OAuth2Token", "id_Oauth2Token", ClientIdString);
            cmdCat.Add(buttonDef);
        }

        private void ButtonDef_OnExecute(NameValueMap Context)
        {
            if (!LoginManager.LoginManager.PerformUserAuthentication((IntPtr) m_inventorApplication.MainFrameHWND))
                return;
            var token = string.Empty;

            System.Windows.Clipboard.SetText(token = LoginManager.LoginManager.GetOAuth2AccessToken());

            if (string.IsNullOrWhiteSpace(token))
            {
                int button = m_inventorApplication.CommandManager.PromptMessage("Failed to Initialize Entitlement Service. \nWould you like to retry?", 1);
                if (button is 1) ButtonDef?.Execute();
            }
            else
            {
                int button = m_inventorApplication.CommandManager.PromptMessage("OAuth 2 Token Successfully copied to Clipboard", 0);
            }
        }

        private void InitRibbon(ButtonDefinition buttonDef)
        {
            const string getStartedRibbonTabInternalName = "id_GetStarted";

            foreach (Ribbon ribbon in m_inventorApplication.UserInterfaceManager.Ribbons)
            {
                var ribbonTab = ribbon.RibbonTabs[getStartedRibbonTabInternalName];

                var oAuth2RibbonPanel = ribbonTab.RibbonPanels.Add("Get OAuth2 Token", "id_oAuth2Token", ClientIdString);
                oAuth2RibbonPanel.CommandControls.AddButton(buttonDef, true);
            }
        }

        private ButtonDefinition InitButton()
        {
            var oCtrlDefs = m_inventorApplication.CommandManager.ControlDefinitions;

            return oCtrlDefs.AddButtonDefinition("Get OAuth2 Token", "id_button_oAuth2Token", CommandTypesEnum.kQueryOnlyCmdType, ClientIdString);
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
            m_inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation =>
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                null;

        #endregion

        private static string AddInGuid(Type t)
        {
            var guid = string.Empty;

            try
            {
                var customAttributes = t.GetCustomAttributes(typeof(GuidAttribute), false);
                var guidAttribute = customAttributes[0] as GuidAttribute;
                guid = guidAttribute.Value;
            }
            catch
            {
                // ignored
            }

            return guid;
        }

    }
}
