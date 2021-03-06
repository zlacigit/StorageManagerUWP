﻿using HF.Models;
using HF.Services;
using HF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace HF.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IContentProviderApiService _contentProviderApiService;

        private List<User> _userGroup;
        public List<User> UserGroups
        {
            get { return _userGroup; }
            set { Set(ref _userGroup, value); }
        }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }

        public LoginPageViewModel(IContentProviderApiService contentProviderApiService)
        {
            _contentProviderApiService = contentProviderApiService;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            UserGroups = _contentProviderApiService.GetUsers();
            await base.OnNavigatedToAsync(parameter, mode, suspensionState);
            Services.SettingsServices.SettingsService _settings;
            _settings = Services.SettingsServices.SettingsService.Instance;
            _settings.IsFullScreen = true;
        }
  
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void Login(object sender)
        {
            User loggedIn =_contentProviderApiService.getAccess(Name, Password);
            if (loggedIn == null)
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            else
            {
                _contentProviderApiService.setAsLoggedIn(loggedIn);
                dropNotification();
                NavigationService.Navigate(typeof(Views.MainPage), 2);

            }
            
        }

        public void LoginAsGuest()
        {
            _contentProviderApiService.setAsLoggedIn(new User("Guest", ""));
            NavigationService.Navigate(typeof(Views.LogoutPage), 2);
        }
        public void Help(object sender)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        public void Reg()
        {
            NavigationService.Navigate(typeof(Views.RegistrationPage), 5);
        }
        private void dropNotification()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("Login"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(_contentProviderApiService.getLoggedInUser().Name+" logged in"));
            XmlNodeList toastImageElements = toastXml.GetElementsByTagName("image");
            IXmlNode toasNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toasNode).SetAttribute("duration", "long");

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
