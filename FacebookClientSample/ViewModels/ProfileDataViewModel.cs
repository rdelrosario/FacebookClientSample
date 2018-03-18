﻿using System; using System.Collections.Generic;
using System.ComponentModel; using System.Threading.Tasks; using FacebookClientSample.Models;
using Newtonsoft.Json.Linq;
using Plugin.FacebookClient; using Plugin.FacebookClient.Abstractions; using Xamarin.Forms;  namespace FacebookClientSample.ViewModels {

    public class ProfileDataViewModel : INotifyPropertyChanged     {         public List<PostData>                        ListPostedMessages        { get; set; }         public FaceBookData                          Profile                   { get; set; }         public Command                               FillPrincipalDataCommand  { get; set; }         public Command<string>                       PostMessageCommand        { get; set; }         public string                                MessagePost               { get; set; } = string.Empty;         FacebookResponse<Dictionary<string, object>> attrs;          string                                       Message = string.Empty;         string                                       Story = string.Empty;          public ProfileDataViewModel()         {             Profile = new FaceBookData();             FillPrincipalDataCommand = new Command(FillPrincipalData);             PostMessageCommand       = new Command<string>(PostMessage);          }          public async void FillPrincipalData()         {             FacebookResponse<bool> resp = await CrossFacebookClient.Current.LoginAsync(new string[] { "email", "public_profile", "user_friends" });              attrs = await CrossFacebookClient.Current.RequestUserDataAsync             (                    new string[] { "id", "name", "picture", "cover", "friends" }, new string[] { }             );
            Profile = new FaceBookData() {                              FullName = attrs.Data["name"].ToString() ,                              Cover    = new UriImageSource { Uri = new System.Uri(Utilities.JsonConvert(attrs.Data["cover"].ToString(), "source")) },                             Picture  = new UriImageSource { Uri = new System.Uri(Utilities.JsonConvert(attrs.Data["picture"].ToString(), "url", "data")) }                        };
             await ShowPosts();             await App.Navigation.PushAsync(new MyProfilePage(Profile, ListPostedMessages));         }          public async void PostMessage(string message)         {             await CrossFacebookClient.Current.PostDataAsync("me/feed",                                                            new string[] { "publish_actions" },                                                               new Dictionary<string, string>()                                                                 {                                                                    {"message" ,message}                                                                }                                                             );             MessagePost = string.Empty;             await ShowPosts();         }          public async Task ShowPosts()         {                  FacebookResponse<string> post = await CrossFacebookClient.Current.QueryDataAsync("me/feed", new string[] { "user_posts" });                 var jo = JObject.Parse(post.Data.Replace("(", "[").Replace(@"\U", "\\\\U").Replace(");", "],").Replace(" = ", ":").Replace(";", ","));              ListPostedMessages = new List<PostData>();                 for (int i = 0; i < ((JArray)jo["data"]).Count; i++)                 {                     try { Message = jo["data"][i]["message"].ToString(); }                     catch { Message = ""; }                      try { Story = jo["data"][i]["story"].ToString(); }                     catch { Story = ""; }                  ListPostedMessages.Add(new PostData() { MessagePosted = Message, Story = Story });                 }          }          public event PropertyChangedEventHandler PropertyChanged;     } } 