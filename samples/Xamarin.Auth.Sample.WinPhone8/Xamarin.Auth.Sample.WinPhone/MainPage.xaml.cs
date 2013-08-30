﻿//
//  Copyright 2013, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Controls;

namespace Xamarin.Auth.Sample.WinPhone
{
	public partial class MainPage : PhoneApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void OnClickFacebook (object sender, RoutedEventArgs e)
		{
			var auth = new OAuth2Authenticator (
				clientId: "App ID from https://developers.facebook.com/apps",
				scope: "",
				authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += (s, ee) => {
				if (!ee.IsAuthenticated) {
					this.facebookStatus.Text = "Not Authenticated";
					return;
				}

				// Now that we're logged in, make a OAuth2 request to get the user's info.
				var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, ee.Account);
				request.GetResponseAsync().ContinueWith (t => {
					if (t.IsFaulted)
						this.facebookStatus.Text = "Error: " + t.Exception.InnerException.Message;
					else if (t.IsCanceled)
						this.facebookStatus.Text = "Canceled";
					else
					{
						this.facebookStatus.Text = t.Result.GetResponseText();
					}
				}, uiScheduler);
			};

			Uri uri = auth.GetUI();
			NavigationService.Navigate (uri);
		}

		private readonly TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}