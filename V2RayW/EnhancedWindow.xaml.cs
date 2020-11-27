using Newtonsoft.Json;
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

            encryptionCheckBox.IsChecked = selectedUserInfo["encryption"].ToString() != "none";
            encryptionContentBox.Text = selectedUserInfo["encryption"].ToString();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> selectedProfile = configWindow.profiles[configWindow.vmessListBox.SelectedIndex];
            Dictionary<string, object> selectedVnext = ((selectedProfile["settings"] as Dictionary<string, object>)["vnext"] as IList<object>)[0] as Dictionary<string, object>;
            Dictionary<string, object> selectedUserInfo = (selectedVnext["users"] as IList<object>)[0] as Dictionary<string, object>;

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
            }
        }

    }
}
