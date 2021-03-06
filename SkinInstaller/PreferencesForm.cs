﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SkinInstaller
{
	public partial class PreferencesForm: Form
	{
		public PreferencesForm()
		{
            Init();
		}
        public void Init()
        {
            InitializeComponent();
            this.checkedListBox1.SetItemChecked(0,
                Properties.Settings.Default.ch3d);
            this.checkedListBox1.SetItemChecked(1,
                Properties.Settings.Default.chTx);
            this.checkedListBox1.SetItemChecked(2,
                Properties.Settings.Default.part3d);
            this.checkedListBox1.SetItemChecked(3,
                Properties.Settings.Default.partTx);
            this.checkedListBox1.SetItemChecked(4,
                Properties.Settings.Default.anims);
            this.checkedListBox1.SetItemChecked(5,
                Properties.Settings.Default.air);
            this.checkedListBox1.SetItemChecked(6,
                Properties.Settings.Default.sounds);
            this.checkedListBox1.SetItemChecked(7,
                Properties.Settings.Default.text);
            this.checkedListBox1.SetItemChecked(8,
                Properties.Settings.Default.showEveryTime);
            this.checkedListBox1.SetItemChecked(9,
                Properties.Settings.Default.showUninstallWarning);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ch3d =
                checkedListBox1.GetItemChecked(0);
            Properties.Settings.Default.chTx =
                checkedListBox1.GetItemChecked(1);
            Properties.Settings.Default.part3d =
                checkedListBox1.GetItemChecked(2);
            Properties.Settings.Default.partTx =
                checkedListBox1.GetItemChecked(3);
            Properties.Settings.Default.anims =
                checkedListBox1.GetItemChecked(4);
            Properties.Settings.Default.air =
                checkedListBox1.GetItemChecked(5);
            Properties.Settings.Default.sounds =
                checkedListBox1.GetItemChecked(6);
            Properties.Settings.Default.text =
                checkedListBox1.GetItemChecked(7);
            Properties.Settings.Default.showEveryTime =
                checkedListBox1.GetItemChecked(8);
            Properties.Settings.Default.showUninstallWarning =
                checkedListBox1.GetItemChecked(9);

            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.showUninstallWarning)
            {
                if (Properties.Settings.Default.sounds
                    || Properties.Settings.Default.air
                   || Properties.Settings.Default.text)
                {
                    InfoForm form = new InfoForm(
                        "The Starred Items you have selected for install\r\n" +
                        "have the potential to cause a error the next time\r\n" +
                        "your LoL client updates.\r\n\r\n" +
                        "This program does handle backups, but YOU must \r\n" +
                        "remember to uninstall your skin before the update\r\n" +
                        "download completes", new Size(270, 180), "HEADS UP");
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog();
                }
            }

            this.Close();
        }
	}
}
