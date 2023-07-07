using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using FinSimLauncher.View;
using System.Net;   
using GalaSoft.MvvmLight.Command;
using CefSharp;
using System.Windows;

namespace FinSimLauncher.ViewModel
{
    
    public class MainVM : ViewModelBase
    {
        private string _account = "";
        private Page _startPG = new FinSimGamePage();
        private Page _currPG = new FinSimGamePage();
        private Page _finSimGamePG = new FinSimGamePage();
        private Page _educationPG = new EducationPage();
        private Page _accountPG = new AccountPage();
        private Page _forumPG = new ForumPage();
        private Page _newsPG = new NewsPage();
        private Page _settingsPG = new SettingsPage();

        public Page CurrentPG
        {
            get 
            {
                return _currPG;
            }
            set 
            {
                if (_currPG != value)
                {
                    Set(ref _currPG, value);
                }
            }
        }
        public ICommand OpenStartPG
        {
            get
            {
                return new RelayCommand(() => CurrentPG = _startPG);
            }
        }
        public ICommand OpenFSGamePG
        {
            get 
            {
                return new RelayCommand(() => CurrentPG = _finSimGamePG); 
            }
        }
        public ICommand OpenEdPG
        {
            get
            {
                return new RelayCommand(() => CurrentPG = _educationPG);
            }
        }
        
        public ICommand OpenAccountPG
        {
            get
            {
                return new RelayCommand(() => CurrentPG = _accountPG);
            }
        }
        public ICommand OpenForumPG
        {
            get
            {
                return new RelayCommand(() => CurrentPG = _forumPG);
            }
        }
        public ICommand OpenNewsPG
        {
            get
            {
                return new RelayCommand(() => CurrentPG = _newsPG);
            }
        }
        public ICommand OpenSettingsPG
        {
            get
            {
                return new RelayCommand(() => CurrentPG = _settingsPG);
            }
        }

        public string Account { get => _account; set => _account = value; }


        public void ChangeBrowserSize(bool SidePanelClosed, Page page)
        {
            if (page is EducationPage || page is ForumPage || page is NewsPage)
            {
                if (page is EducationPage)
                {
                    if (SidePanelClosed)
                    {
                        (page as EducationPage).browser.Margin = new Thickness(-170, 0, 0, 0);
                        (page as EducationPage).browser.SetZoomLevel(-3.00);
                    }
                    else
                    {
                        (page as EducationPage).browser.Margin = new Thickness(0);
                        (page as EducationPage).browser.SetZoomLevel(-4.08);
                    }
                }
                
            }
            if (page is ForumPage)
            {
                if (SidePanelClosed)
                {
                    (page as ForumPage).browser.Margin = new Thickness(-170, 0, 0, 0);
                }
                else
                {
                    (page as ForumPage).browser.Margin = new Thickness(0);
                }
            }
            if (page is NewsPage)
            {
                if (SidePanelClosed)
                {
                    (page as NewsPage).browser.Margin = new Thickness(-170, 0, 0, 0);
                }
                else
                {
                    (page as NewsPage).browser.Margin = new Thickness(0);
                }
            }
        }


    }
}
