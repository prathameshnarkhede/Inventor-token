using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.WebServicesManaged;

namespace LoginManager
{
    public class WebServicesObserver : AdWSObserverCLR
    {
        public bool AboutToLogout()
        {
            return true;
        }

        public void AccountCreated()
        {
            //UpdateLoginState();
            //MessageBox.Show("AccountCreated","Work",MessageBoxButton.OK);
        }

        public void LoggedIn()
        {
            //UpdateLoginState();
            //MessageBox.Show("AccountCreated", "Work", MessageBoxButton.OK);
        }

        public void LoggedOut()
        {
            //UpdateLoginState();
        }

        public void LoggingOut()
        {
            AdWebServicesManaged.GetInstance().RemoveObserver();
           
        }

        public void LoginDialogTimeoutInAdSSOServer()
        {
            //MessageBox.Show("LoginDialogTimeoutInAdSSOServer");
        }

        public void OAuth2TokenAvailable()
        {
            //MessageBox.Show("OAuth2TokenAvailable");
        }

        public void OAuth2TokenDiscarded()
        {
            // MessageBox.Show("OAuth2TokenDiscarded");
        }

        public void OAuth2TokenQueryFailed()
        {
            //MessageBox.Show("OAuth2TokenQueryFailed");
        }

        public void OAuth2TokenRefreshed()
        {
            //MessageBox.Show("OAuth2TokenRefreshed");
        }

        public void ServerChanged()
        {
            //UpdateServer();
        }

        public void TokenAvailable()
        {
            //MessageBox.Show("LoginDialogTimeoutInAdSSOServer");
        }

        public void TokenQueryFailed()
        {
            //MessageBox.Show("LoginDialogTimeoutInAdSSOServer");
        }

        public void TokenRefreshed()
        {
            //MessageBox.Show("LoginDialogTimeoutInAdSSOServer");
        }
    }
}
