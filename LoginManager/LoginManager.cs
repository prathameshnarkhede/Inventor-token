using Autodesk.WebServicesManaged;
using Autodesk.WebServicesUIManaged;
using System;

namespace LoginManager
{
    public class LoginManager
    {
        public delegate void Notify();
        public static event Notify OnAuthenticationFailed;
        private static bool _checkObserver;
        public static WebServicesObserver AppObserver = new WebServicesObserver();

        public const string OAuth2TokenScopes = "code:all data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete";
        private const OAuth2TokenType tokenType = OAuth2TokenType.OAuth2TokenTypeJWT;

        public static bool IsUserLoggedIn()
        {
            var loginManager = AdWebServicesManaged.GetInstance();
            return loginManager.IsLoggedIn();
        }

        public static void InitializeWebService(IntPtr mainFrameHwnd)
        {
            if (mainFrameHwnd == IntPtr.Zero)
                return;

            //AdSSO UI services Initialization
            if (!AdWebServicesUIManaged.GetInstance().IsInitialized())
            {
                AdWebServicesUIManaged.Initialize((int) tokenType, OAuth2TokenScopes);

                unsafe
                {
                    AdWebServicesUIManaged.GetInstance().SetHostApplicationWindow(&mainFrameHwnd);
                }
            }
            //AdSSO services Initialization
            if (AdWebServicesManaged.IsInitialized())
            {
                AdWebServicesManaged.Initialize((int) tokenType, OAuth2TokenScopes);
            }
        }

        public static void LoginUser(IntPtr mainFrameHwnd)
        {
            InitializeWebService(mainFrameHwnd);
            var loginManager = AdWebServicesManaged.GetInstance();
            loginManager?.AddObserver("AdSSOObserver", AppObserver);
            _checkObserver = true;
        }

        public static string GetOAuth2AccessToken(string scope = OAuth2TokenScopes)
        {
            var refToken = string.Empty;

            var loginManager = AdWebServicesManaged.GetInstance();

            if (loginManager.IsTokenExpired())
                loginManager.RefreshOAuth2Token((int) tokenType, scope, true);

            if (loginManager.IsOAuth2TokenExpired(tokenType, scope))
                loginManager.RefreshOAuth2Token((int) tokenType, scope, true);


            loginManager.EnableOAuth2((int) tokenType, scope);
            loginManager.GetOAuth2AccessToken((int) tokenType, scope, ref refToken, true);
            if (refToken.Equals(string.Empty))
            {
                loginManager.GetOAuth2AccessToken((int) tokenType, scope, ref refToken, true);
            }

            return refToken;
        }

        public static string GetUserId()
        {
            var refUserId = string.Empty;
            var loginManager = AdWebServicesManaged.GetInstance();
            loginManager.GetUserId(ref refUserId);
            return refUserId;
        }
        public static bool PerformUserAuthentication(IntPtr mainFrameHwnd)
        {
            var isUserAuthenticated = false;

            if (!_checkObserver)
                LoginUser(mainFrameHwnd);

            // Launch UI
            if (IsUserLoggedIn())
                isUserAuthenticated = true;
            else
            {
                LoginUser(mainFrameHwnd);
                if (!IsUserLoggedIn())
                {
                    OnAuthenticationFailed?.Invoke();
                }
            }
            return isUserAuthenticated;
        }

        public static int GetServer()
        {
            var loginManager = AdWebServicesManaged.GetInstance();
            return (int)loginManager.GetServer();
        }

    }
}
