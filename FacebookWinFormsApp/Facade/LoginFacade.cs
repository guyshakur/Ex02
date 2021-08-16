using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace FacebookWinFormsApp

{
    public class LoginFacade
    {
        //private User m_LoginUser;
        //private LoginResult m_LoginResult;
        //private AppSettings m_AppSettings;
        public AppSettings AppSettings { get; set; }
        public User LoginUser { get; set; }
        public LoginResult LoginResult { get; set; }

        public LoginFacade()
        {
            AppSettings = AppSettings.AppSettingsInstance;
            FacebookService.s_CollectionLimit = 100;
        }

        public bool Login(bool m_rememberedUser)
        {
            LoginResult = FacebookService.Login(
                    "226428995869586",
                    "email",
                    "user_posts",
                    "user_friends",
                    "user_likes",
                    "user_photos",
                    "user_events",
                    "user_birthday",
                    "user_location",
                    "user_gender",
                    "groups_access_member_info");
            if(m_rememberedUser)
            {
                AppSettings.RememberUser = true;
                setLastAccessToken();
            }
            AppSettings.SaveToFile();
            LoginUser = LoginResult.LoggedInUser;
            return LoginResult.AccessToken!=null;
        }

        private void setLastAccessToken()
        {
            AppSettings.LastAcsessToken = LoginResult.AccessToken;
        }

        public void LogOut()
        {
            FacebookService.Logout();
            AppSettings.LastAcsessToken = string.Empty;
            AppSettings.RememberUser = false;
            AppSettings.SaveToFile();
            LoginResult = null;


        }
    }
}
