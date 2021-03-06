﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using V2RayW.Resources;

namespace V2RayW
{
    /// <summary>
    /// EnhancedWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EnhancedWindow : Window
    {
        private ConfigWindow configWindow;
        public EnhancedWindow()
        {
            InitializeComponent();
        }

        public void InitializeData()
        {
            configWindow = this.Owner as ConfigWindow;
            FillinData();
        }

        private void FillinData()
        {
            Dictionary<string, object> selectedProfile = configWindow.profiles[configWindow.vmessListBox.SelectedIndex];
            Dictionary<string, object> selectedVnext = ((selectedProfile["settings"] as Dictionary<string, object>)["vnext"] as IList<object>)[0] as Dictionary<string, object>;
            Dictionary<string, object> selectedUserInfo = (selectedVnext["users"] as IList<object>)[0] as Dictionary<string, object>;

            #region encryption
            encryptionCheckBox.IsChecked = selectedUserInfo["encryption"].ToString() != "none";
            encryptionContentBox.Text = selectedUserInfo["encryption"].ToString();
            #endregion encryption

            #region flow
            enableFlow.IsChecked = selectedUserInfo.ContainsKey("flow");           
            if (enableFlow.IsChecked ?? false)
            {
                flowCheckBox.IsChecked = selectedUserInfo["flow"].ToString() != "";
                flowContentBox.Text = selectedUserInfo["flow"].ToString();
            }
            
            #endregion flow
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> selectedProfile = configWindow.profiles[configWindow.vmessListBox.SelectedIndex];
            Dictionary<string, object> selectedVnext = ((selectedProfile["settings"] as Dictionary<string, object>)["vnext"] as IList<object>)[0] as Dictionary<string, object>;
            Dictionary<string, object> selectedUserInfo = (selectedVnext["users"] as IList<object>)[0] as Dictionary<string, object>;
            Dictionary<string, object> streamSettings = selectedProfile["streamSettings"] as Dictionary<string, object>;

            if (encryptionCheckBox.IsChecked ?? false)
            {
                try
                {
                    selectedUserInfo["encryption"] = encryptionContentBox.Text.ToString();

                }
                catch
                {
                    MessageBox.Show("encryption " + "\n" + encryptionContentBox.Text, Strings.messagenotvalidjson);
                    return;
                }
            }else 
            { 
                selectedUserInfo["encryption"] = "none";
            }
            if (enableFlow.IsChecked ?? false)
            {
                selectedUserInfo["flow"] = selectedUserInfo.ContainsKey("flow") ? selectedUserInfo["flow"].ToString() : "";
                if ((flowCheckBox.IsChecked ?? false) && flowContentBox.Text.Trim().Length != 0)
                {
                    try
                    {
                        selectedUserInfo["flow"] = flowContentBox.Text.ToString();
                        streamSettings["security"] = streamSettings["security"].ToString() == "tls" ? "xtls" : streamSettings["security"]; ;
                        selectedProfile["streamSettings"] = streamSettings.ToDictionary(k => k.Key == "tlsSettings" ? "xtlsSettings" : k.Key, k => k.Value);
                    }
                    catch
                    {
                        MessageBox.Show("flow " + "\n" + flowContentBox.Text, Strings.messagenotvalidjson);
                        return;
                    }
                } else
                {
                    selectedUserInfo["flow"] = "";
                    streamSettings["security"] = streamSettings["security"].ToString() == "xtls" ? "tls" : streamSettings["security"];
                    
                    if (streamSettings.ContainsKey("xtlsSettings"))
                    {
                        streamSettings.Remove("tlsSettings");
                        selectedProfile["streamSettings"] = streamSettings.ToDictionary(k => k.Key == "xtlsSettings" ? "tlsSettings" : k.Key, k => k.Value);
                    }
                }
            } else
            {
                selectedUserInfo.Remove("flow");
                streamSettings["security"] = streamSettings["security"].ToString() == "xtls" ? "tls" : streamSettings["security"];              
                if (streamSettings.ContainsKey("xtlsSettings"))
                {
                    streamSettings.Remove("tlsSettings");
                    selectedProfile["streamSettings"] = streamSettings.ToDictionary(k => k.Key == "xtlsSettings" ? "tlsSettings" : k.Key, k => k.Value);
                }     
            }
            
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainTabControl.SelectedIndex == mainTabControl.Items.Count - 1)
            {
                Process.Start(Strings.encryptionHelpPage);
            }else
            {
                Process.Start(Strings.flowHelpPage);
            }
        }

    }
}
