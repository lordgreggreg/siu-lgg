﻿using RelManLib;
/*
 * Skin Installer Ultimate + LGG (SIU+LGG)
 * Copyright 2011 Greg Hendrickson
 * This file is part of SIU+LGG.
 * 
 * SIU+LGG is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * any later version.
 * 
 * SIU+LGG is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with LOLViewer.  If not, see <http://www.gnu.org/licenses/>.
*/

/*
 * This Program is designed to help safely install
 * a variety of skins for the League of Legends game.
 * http://www.leagueoflegends.com 
 * 
 * A good website to browse such skins is
 * http://leaguecraft.com/skins 
 * 
 * This program is a modification of the original 
 * Skin installer generously provided and created by
 * sgun and found here http://forum.leaguecraft.com/index.php?/topic/5542-tool-lol-skin-installer-skin-installation-and-management-tool 
 * 
 * This program incorporates sound installation code
 * found from http://forum.leaguecraft.com/index.php?/topic/21301-release-lolmod 
 * 
 * And uses Raf Manager to read raf files by
 * ItzWarty and found here http://code.google.com/p/raf-manager/source/browse/ 
 * 
 * And uses fsbext to read fmod sound files
 * found here http://aluigi.altervista.org/papers.htm 
 * 
 * And also uses code from LoLViewer by 
 * Authentication found here http://code.google.com/p/lolmodelviewer/
 * 
 * It also uses the devil image library, 7zip, nvidia dds tools, and openTK
 * 
 * All external code is licensed and copyright 
 * by their respective owners
 */

// To make navigation of this code easier, inside msvc
// press ctrl+M, then ctrl+O to collapse regions.

namespace SkinInstaller
{
    using LOLViewer;
    using System.Runtime.InteropServices;
    using System;
    using System.Linq;
    using Tao.DevIl;
    using System.Net;
    using System.Web;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SQLite;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using RAFLib;
    using RelManLib;
    using System.Text;
    using System.Text.RegularExpressions;
    

    using System.Drawing.Drawing2D;

    //using zlib = ComponentAce.Compression.Libs.zlib;
    using Ionic.Zip;
    using System.Drawing;
    using System.Threading;

    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Platform;
    using System.Reflection;
    using System.Collections;
    using System.Globalization;
    //using mshtml;
    
    public class skinInstaller : Form
    {
        #region consts
        const String c_TEMP_DIR_NAME_FIXED_SKIN_FILES = "fx";
        const String c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL = "sti";
        const String c_DIRECTORY_NAME_FOR_FIXING_ZIPS = "ztmp";
        const String c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_OPTIONS = "hlp";
        
        #endregion
        #region vars
        RelManDirectoryFile rmdf = null;
        string dlURL = "https://sites.google.com/site/siuupdates/version.txt";
        TextEditor.TextEditorMain ted;            
        TreeNode newRafNode=new TreeNode("RAF");
        PreviewWindow previewWindow;
        Dictionary<String, String> charFixs = new Dictionary<String, String>();
        Dictionary<String, Dictionary<String, int>> dxtVersions = new Dictionary<String, Dictionary<String, int>>();
        List<Image> imageIcons = new List<Image>();
        int lggsiu1tex;
        string argsToProc = "";
        public const int WM_USER = 0x400;
        public const int WM_COPYDATA = 0x4A;
        //Used for WM_COPYDATA for string messages
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        string myCurrentVersion = "1.0";
        string dateFormat = "M/d/yyyy HH:mm:ss.f tt";
        bool loaded = false;
        DateTime started;
        DateTime frameTime;
        //other
        
        private Point openedAt;
        private static List<string> fileExtensions = new List<string>();
        private static List<string> fileList = new List<string>();
        private Dictionary<string, List<string>> excludedFiles = new Dictionary<string, List<string>>();
        private static long fileSize = 0L;
        private static string baseDir;
        //other
        public string CorrectFileLocation
        {
            get
            {
                return this.correctFileLocation;
            }
            set
            {
                this.correctFileLocation = value;
            }
        }
        public string FileLoc
        {
            get
            {
                return this.oFileLock;
            }
            set
            {
                this.oFileLock = value;
            }
        }
        public string GlobalFileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }
        public string[] FilePossibles
        {
            get
            {
                return this.filePossibiles;
            }
            set
            {
                this.filePossibiles = value;
            }
        }

        private Bitmap m_bit;
        private StringBuilder debug =new StringBuilder("DEBUG LOG\r\n\r\n");
        private List<KeyValuePair<String, String>> allFilesList = new List<KeyValuePair<string, string>>();
        private static string[] allFilesExtensions = new string[] { "" };
        private String directoryPath = " ";
        #region GUI_Vars
        private Button b_AddDirectory;
        private Button b_AddFiles;
        private Button b_ClearAll;
        private Button b_CreatePack;
        private Button b_IAddDirectory;
        private Button b_IAddFiles;
        private Button b_IClearAll;
        private Button b_IInstallFiles;
        private Button b_IRemoveFiles;
        private Button b_RemoveFiles;
        private Button browse;
        private Button browseLoadScreen;
        private Button button1;
        private Button changeSkinButton;
        private Button clearButton;
        private Button exit;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private IContainer components;
        private string correctFileLocation;
        private Button createZip;
        private Button dbDelete;
        private Button dbInstall;
        private Button dbUninstall;
        private TextBox fileNameTextBox;
        private StatusStrip helpBar;
        private ToolStripStatusLabel helpText;
        private ListBox installFiles_ListBox;
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label7;
        private ListViewItemHover listView1;
        private OpenFileDialog loadScreenFile;
        private TextBox loadScreenImageTextBox;
        private Label loadScreenLbl;
        private Button locateGameClient;
        private ListBox packFileList;
        private TextBox packFileNameTextBox;
        private Button reset;
        private Button resetLoadScreenBox;
        private Button resetSkinBox;
        private CheckBox saveToDb;
        private ComboBox selectChampion;
        private Label selectchampiontxt;
        private TabControl tabControl1;
        private TabPage tabPage2;
        private TabPage tabPage4;
        private Button UpdateFL;
        private OpenFileDialog skinFile;
        private TextBox skinNameTextbox;
        private ComboBox skinVariant;
        private ProgressBar progressBar1;
        private Button button2;
        private TextBox textBoxauthor;
        private Label label8;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem editInstallPreferencesToolStripMenuItem;
        private ToolStripMenuItem changeGameClientLocationToolStripMenuItem;
        private ToolStripMenuItem updateFileListToolStripMenuItem;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripMenuItem soundFileLocationToolStripMenuItem;
        private Button button3repath;
        private ToolStripMenuItem clientLocationToolStripMenuItem;
        private ToolStripMenuItem repathAllFilesToolStripMenuItem;
        private ContextMenuStrip dataBaseListMenuStrip1;
        private ToolStripMenuItem toolStripSelectUninstalled;
        private ToolStripMenuItem toolStripSelectAllInstalled;
        private ToolStripMenuItem showDebugToolStripMenuItem;
        private ToolStripMenuItem selectAllSkinsToolStripMenuItem;
        private ToolStripMenuItem deselectAllSkinsToolStripMenuItem;
        private Panel panel1;
        private Panel panel2;
        private Panel panel4;
        private Panel panel3;
        private Panel panel5;
        private BackgroundWorker fileListWorker1;
        private ToolStripMenuItem imagesToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private OpenFileDialog openFileDialog1;
        private TabPage tabPage1;
        private PictureBox pictureBox1;
        private SaveFileDialog saveFileDialog1;
        private SplitContainer splitContainer1;
        private TextBox textBox1;
        private Panel panel6;
        private ToolStripMenuItem editAllPreferencesToolStripMenuItem;
        private ToolStripMenuItem iAmLGGToolStripMenuItem;
        private ToolStripMenuItem iCantStandLGGToolStripMenuItem; private BackgroundWorker backgroundWorkerCountUpdate;
        private ToolStripMenuItem editThisSkinToolStripMenuItem;
        private ToolStripMenuItem wtfRainbowsToolStripMenuItem;
        private Panel panelGL;
        private Panel panel7;
        private BackgroundWorker installWorker2;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer3;
        private TextBox textBox2;
        private Label labelSkinName;
        private PictureBox pictureBox2;
        private BackgroundWorker updateWorker2;
        private ToolStripMenuItem checkForUpdateToolStripMenuItem;
        private BackgroundWorker webPinger;
        private ToolStripMenuItem pingToolStripMenuItem;
        private ToolStripMenuItem viewStatsToolStripMenuItem;
        private PictureBox pictureBoxCount;
        private Label label5;
        private ToolStripMenuItem setSoundFileLocationToolStripMenuItem;
        private ColumnHeader columnHeader6;
        private Panel panel8;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private CheckBox checkBox1dispDateInstalledFull;
        private CheckBox checkBox1dispDateAddedFull;
        private CheckBox checkBox1dispDateAdded;
        private CheckBox checkBox1dispInstalled;
        private CheckBox checkBox1dispFileCount;
        private CheckBox checkBox1dispAuthor;
        private CheckBox checkBox1dispTitle;
        private Label label9;
        private ColorDialog colorDialog1;
        private Button button3lcintegrate;
        private ToolStripMenuItem registerAppForWebUrlsToolStripMenuItem;
        private ToolStripStatusLabel statusText;
        private ToolStripMenuItem getProgramLocationToolStripMenuItem;
        private ColumnHeader columnHeader7;
        private ImageList imageList1;
        private CheckBox checkBox1dispCharacter;
        private Panel panel9;
        private MB.Controls.ColorSlider colorSlider1;
        private ToolStripMenuItem resetCharacterIconsCacheToolStripMenuItem;
        private Button button3startLoL;
        private ToolStripMenuItem useDDSVersionReaderToolStripMenuItem;
        private ToolStripMenuItem testReadResFilesToolStripMenuItem;
        private ToolStripMenuItem unpackSoundsToolStripMenuItem;
        #endregion
        private string[] filePossibiles = new string[10000];
        private riotVersions currentVersions = new riotVersions();
        private static string euGameDirectory = @"C:\Program Files\League of Legends\";
        private static string euGameDirectory64 = @"C:\Program Files (x86)\League of Legends\";
        private static string usGameDirectory = @"C:\Riot Games\League of Legends\";
        private string fileName;
        private static string gameDirectory = string.Empty;        
        private static string lastDirectoryBrowsed = string.Empty;
        
        private string oFileLock;
        private static string otherGameDirectory = "";
        
        private FileHandler SIFileOp = new FileHandler();
        private SQLiteConnection sqLiteCon = new SQLiteConnection();
        
        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.Timer timer1;
        public OpenTK.Graphics.GraphicsMode mode = null;        
        private System.Windows.Forms.Timer timeupdatecount;
        private ImagePreviewForm imagePreviewForm = new ImagePreviewForm();
        private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        private ToolStripMenuItem previewThisSkinToolStripMenuItem;
        private ToolStripMenuItem loLViewerOpenNotPreviewToolStripMenuItem;
        private Panel addFilesPanel;
        private Panel AddToDatabasePanel;
        private TabPage tabPage3;
        private SplitContainer splitContainer4;
        private SplitContainer splitContainer5;
        private TreeView treeView1;
        private Button buttonRebuildTree;
        private BackgroundWorker rafTreeBuilderWorker2;
        private ContextMenuStrip treeViewMenuStrip1;
        private ToolStripMenuItem exportSelectedFilesToolStripMenuItem;
        private ToolStripMenuItem deselectAllFilesToolStripMenuItem;
        private FolderBrowserDialog exportFolderBrowserDialog1;
        private ToolStripMenuItem helpToolStripMenuItem1;
        private Button button3exporttree;
        private Label label10;

        PaintEventHandler crazyP;
        private TextBox textBox3;
        private ToolStripMenuItem openParticleReferenceToolStripMenuItem;
        private BackgroundWorker ParticleTreeWorker2;
        private Panel progrespanel;
        private Button buttoncancel;
        private ContextMenuStrip treeMenuStripSkin1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem makeSimpleSkinFromThisRiotSkinToolStripMenuItem;
        private ToolStripMenuItem showMenuFileLocationToolStripMenuItem;
        private ToolStripMenuItem openTextTreeEditorToolStripMenuItem;
        private BackgroundWorker exportTreeViewWorker1;
        private Button button3reinstallText;
        private ToolStripMenuItem openPublisherToolStripMenuItem;
        private ToolStripMenuItem moreDebugToolStripMenuItem;
        private ToolStripMenuItem getVersionFilePathToolStripMenuItem;
        private ToolStripMenuItem readVersionsToolStripMenuItem;
        private Label label2Percent;
        private BackgroundWorker ParticleTreeWorkerNew;
        private ToolStripMenuItem getLastModDateToolStripMenuItem;
        PaintEventHandler importantP;
        private SplitContainer splitContainer6;
        private ExtendedWebBrowser webBrowser1;
        private Panel panel10;
        private Button button3openAd;
        private Button button3openAds3;
        private Button button3openAds2;
        private Button button3CloseAd;
        private SkinInstaller.ExtendedWebBrowser webBrowser2Test;
        private ToolStripMenuItem deleteBackupsToolStripMenuItem;
        private ToolStripMenuItem viewDXTVersionsToolStripMenuItem;
        private BackgroundWorker addFilesWorker;
        private Button button3FixCrashAfterPatch;
        private BackgroundWorker patchFixerWorker;
        private ToolStripMenuItem createDesktopShortcutToolStripMenuItem;
        private ToolStripMenuItem associateFilesToolStripMenuItem;
        private ToolStripMenuItem skinHelpersToolStripMenuItem;
        private ToolStripMenuItem copyLikeFilesToolStripMenuItem;
        private BackgroundWorker uninstallWorker1;
        private ToolStripMenuItem lookUpReleaseManifestInfoToolStripMenuItem;
        private ToolStripMenuItem fixRiotWarningsToolStripMenuItem;

        TreeNode database = new TreeNode("dbRoot");
        #endregion
        #region webIntegrate
        private void button3lcintegrate_Click(object sender, EventArgs e)
        {
            LCIntHelp helpf = new LCIntHelp();
            helpf.StartPosition = FormStartPosition.CenterParent;
            helpf.ShowDialog();
        }
        private void registerAppForWebUrlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3lcintegrate_Click(sender, e);
        }
        public void processArgs(string args)
        {
            if (args == "") return;
            args = args.Replace("%22", "").Replace("%20", "[[P20]]").Replace("[[P20]]", "%20");
            args = Uri.UnescapeDataString(args.Replace("#",""));
            //Cliver.Message.Inform(args);
            string dir = Application.StartupPath + "\\t1\\";
            if (Directory.Exists(dir)) SIFileOp.DirectoryDelete(dir, true);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            bool addItFlag = false;
            bool addItFileFlag = false;
            List<String> files = new List<string>();
            //Cliver.Message.Inform("got args to proc " + args);
            //got args to proc --webArgs|--url|skininstallerultimatelgg://test/||
            string[] parts = args.Split('|');
            string urlToDownloadAsync = "";
            //string[] parts = Regex.Split(args,"\\[param\\]");
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i].Trim();
                if (part == "--webArgs")
                {
                    if (i++ < parts.Length - 1)
                    {
                        part = parts[i].Trim();
                    }
                }
                if (part == "--url")
                {
                    if (i++ < parts.Length - 1)
                    {
                        part = parts[i].Trim();
                    }
                }
                if (part.Contains("skininstallerultimatelgg"))
                {
                     //were in busines
                     //string[] urlparts = part.Split('/');
                     string[] urlparts = Regex.Split(part, "\\[param\\]");
                     foreach (string up in urlparts)
                     {
                        //string [] para = up.Split('=');
                         string[] para = Regex.Split(up, "\\[value\\]");
                        if (para.Length == 2)
                        {
                            if (para[0].ToLower() == "name")
                            {
                                this.skinNameTextbox.Text=Uri.UnescapeDataString(para[1]).Trim();
                                this.tabControl1.SelectedIndex = 0;
                            }else
                            if (para[0].ToLower() == "author")
                            {
                                this.textBoxauthor.Text = Uri.UnescapeDataString(para[1]).Trim();
                                this.tabControl1.SelectedIndex = 0;
                            }else
                            if (para[0].ToLower() == "url")
                            {
                                String theURL = para[1];
                                this.tabControl1.SelectedIndex = 0;
                                System.Net.WebClient client = new WebClient();
                                try
                                {
                                    urlToDownloadAsync=theURL;

                                    addItFlag = false;
                                    //string response = client.DownloadString(dlURL).Trim();
                                }
                                catch (Exception ex1)
                                {
                                    debugadd(ex1.ToString());
                                }
                
                            }else
                            if (para[0].ToLower() == "preview")
                            {
                                String theURL = para[1];
                                this.tabControl1.SelectedIndex = 0;
                                System.Net.WebClient client = new WebClient();
                                try
                                {
                                            
                                    //http://leaguecraft.com/uploads/SixxDeathxx/skins/5583.jpg
                                    client.DownloadFile(theURL, dir + "SIUPreview.jpg");
                                    files.Add(dir + "SIUPreview.jpg");
                                            
                                }
                                catch (Exception ex1)
                                {
                                    debugadd(ex1.ToString());
                                }

                            }else
                            if (para[0].ToLower() == "info")
                            {
                                this.tabControl1.SelectedIndex = 0;

                                String theInfo = "skin info: " + Uri.UnescapeDataString(para[1]).Trim().Replace("\r\n", "[New Line]").Replace("\r", "[New Line]").Replace("\n", "[New Line]").Replace(":", "[Colon]");
                                //Cliver.Message.Inform(theInfo);
                                TextWriter tw = new StreamWriter(dir+"SIUInfo.txt");
                                tw.WriteLine(theInfo);
                                tw.Close();
                                files.Add(dir + "SIUInfo.txt");
            
                            }
                            else
                            {
                                Cliver.Message.Inform(para[0] + " is " + para[1]);
                            }
                        }
                    }

                }
                else if (part.Contains("lolsh"))
                {
                    //Cliver.Message.Inform("Good"+args+" and \r\n"+parts[1]);
                    this.tabControl1.SelectedIndex = 0;
                    string downloadURL = "http"+parts[1].Substring(5);
                    try
                    {
                        urlToDownloadAsync = downloadURL;
                        //files.Add(dir + "downloaded.7z");
                        addItFlag =false;
                    }
                    catch (Exception ex1)
                    {
                        debugadd(ex1.ToString());
                    }
                }
                else if (part.Contains("--file"))
                {
                    if (i++ < parts.Length - 1)
                    {
                        part = parts[i].Trim();
                    }
                    files.Add(part);
                    addItFileFlag = true;

                }
                else if (File.Exists(part))
                {
                    files.Add(part);
                    addItFileFlag = true;
                }
                else if (part == "")
                {

                }
                else
                {
                    Cliver.Message.Inform(args);
                }

            }
            if(files.Count>0)
            {
                processNewDirectory(files.ToArray(),addItFlag||addItFileFlag,addItFlag,urlToDownloadAsync);

            }
            else
            {
                downloadWaitingFile(urlToDownloadAsync);
            }
        }
        void downloadWaitingFile(string urlToDownloadAsync)
        {
            if (urlToDownloadAsync == "") return;
            string dir = Application.StartupPath + "\\t1\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            System.Net.WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri(urlToDownloadAsync), dir + "downloaded.7z", urlToDownloadAsync);
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            UpdateProgressSafe((int)percentage);
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpdateProgressSafe(100);

            string dir = Application.StartupPath + "\\t1\\";
            string fileLocation = dir + "downloaded.7z";
            try
            {
                System.Net.WebClient senderC = (System.Net.WebClient)sender;
                WebHeaderCollection heds = senderC.ResponseHeaders;
                string type = heds["Content-Type"].ToString();
                if (type.ToLower().Contains("/zip"))
                {
                    this.SIFileOp.FileMove(fileLocation, fileLocation+".zip");
                    fileLocation = fileLocation + ".zip";
                }
            }
            catch (System.Exception ex)
            {
            	
            }
            processNewDirectory(new string[] { fileLocation }, true);

        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_USER:
                    MessageBox.Show("Message recieved: " + m.WParam + " - " + m.LParam);
                    break;
                case WM_COPYDATA:
                    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
                    this.processArgs(mystr.lpData);
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion
        public skinInstaller(string incname, string version, string args)
        {
            //lgg open db
            this.sqLiteCon.ConnectionString = "data source=\"" + Application.StartupPath + "\\skins.db\"";
            this.sqLiteCon.Open();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            myCurrentVersion = version;
            //one time or die
            Tao.DevIl.Il.ilInit();
            Tao.DevIl.Il.ilSetInteger(Tao.DevIl.Il.IL_DXTC_FORMAT, string2DXInt(Properties.Settings.Default.dxFormat));
            
            try
            {
                for (int aa = 0; aa <= 8; aa += 2)
                {
                    var testMode = new OpenTK.Graphics.GraphicsMode(OpenTK.DisplayDevice.Default.BitsPerPixel, 16, 0, aa);
                    if (testMode.Samples == aa)
                    {
                        mode = testMode;
                    }
                }
            }
            catch { }
            
            this.InitializeComponent();
            this.Text = incname + myCurrentVersion;
            if (!File.Exists(Application.StartupPath + "\\allfiles.ini"))
            {
                
            }else
            if (Properties.Settings.Default.size2.Height != 0)
            {
                this.Size = Properties.Settings.Default.size2;
            }
            this.listView1.ListViewItemSorter=lvwColumnSorter;
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += new DrawItemEventHandler(this.tabControl1_DrawItem);
            //only show database tab if we have installed a skin before.
            this.tabControl1.SelectTab(Properties.Settings.Default.lastSelectedTab);
            this.glControl1 = new OpenTK.GLControl(mode);
            this.glControl1.Dock = DockStyle.Fill;

            this.glControl1.Location = new System.Drawing.Point(0, 0);
            this.glControl1.Name = "gicontrol";
            this.glControl1.Size = new System.Drawing.Size(100, 100
                );
            this.glControl1.Load += glControl1_Load;
            this.glControl1.Paint += glControl1_Paint;
            this.glControl1.Resize += new EventHandler(glControl1_Resize);
            
            panelGL.Controls.Add(this.glControl1);

            //makeRed(this.Controls);
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Reload();
            started = DateTime.Now;
            Application.Idle += new EventHandler(Application_Idle);

            //this.listView1.ContextMenu =;
            if (Properties.Settings.Default.omfgred) changeColors(Color.Red, Color.FromArgb(10, 0, 0));

            crazyP = new System.Windows.Forms.PaintEventHandler(this.crazyPaint);
            makePaint(this.Controls, crazyP);
            
            importantP = new System.Windows.Forms.PaintEventHandler(this.makeLookImportant);
            this.addFilesPanel.Paint += importantP;
            this.AddToDatabasePanel.Paint += importantP;


            this.argsToProc = args;  
            
        }
        private void registerFiles()
        {
            Microsoft.Win32.RegistryKey yurixy = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".yurixyworks");
            Microsoft.Win32.RegistryKey lolmodkey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(".lolmod");
            Microsoft.Win32.RegistryKey softkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\lolsh");
            bool runReg = false;
            if (yurixy == null||lolmodkey==null||softkey==null)
            {
                runReg = true;
            }else
            {
                string val = yurixy.GetValue("").ToString();
                if (val!= "siulggyurixyworks")
                {
                    runReg = true;
                }
                else
                {
                    val = lolmodkey.GetValue("").ToString();
                    if (val != "siulgglolmod")
                    {
                        runReg = true;
                    }

                }

                string lolshVal = softkey.GetValue("").ToString();
                if (lolshVal != "URL:LoL Skin Installer LC LGG")
                {
                    runReg = true;

                }
                else
                {
                    Microsoft.Win32.RegistryKey commandKey = softkey.OpenSubKey("shell\\open\\command");
                    if (commandKey == null)
                    {
                        runReg = true;
                    }
                    else
                    {

                 
                        string commandVal = commandKey.GetValue("").ToString();
                        string matchVal = "\"" + Application.StartupPath + "\\Skin Installer Ultimate.exe" + "\" --url \"%1\"";
                        if (commandVal.CompareTo(matchVal) != 0)
                        {
                            runReg = true;
                        }
                    }
                }
            }
            if(runReg)
            {
                Process process = new Process();
                process.StartInfo.FileName = "sai.exe";
                process.StartInfo.Arguments = "";
                //process.StartInfo.UseShellExecute = false;
                //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WorkingDirectory = Application.StartupPath;
                process.Start();
                process.WaitForExit();
            }
        }
        private void skinInstaller_Load(object sender, EventArgs e)
        {
#if(USkin)
            string skinUPath = Application.StartupPath +
                    "\\guiskinning\\"+
                   // "ConcaveD.msstyles"
                   //"ClearLooks.msstyles" 
                   "DiyGreen.msstyles";
            USkinSDK.USkinInit("", "", skinUPath);
                USkinSDK.USkinLoadSkin(skinUPath);
#endif
            
            loadNameReplacements();
            loadExcludedFiles();
            UpdateProgressSafe(0);
            registerFiles();

            if (Application.StartupPath.Length > 63)
            {
                bool hideTempWarningMessage = false;
                if (Cliver.Message.Show("No too long!", SystemIcons.Warning,
                    out hideTempWarningMessage,
                    "The path that this program is running from\n\"" +
                    Application.StartupPath + "\" \nIs too long and could potentially cause errors.\n\n" +
                    "It is STRONGLY advised to move this folder to a shorter path location (like C:\\siu)\n" +
                    "Please do this now before continuing"
                    , 0, new string[]
                {
                    "Ok",
                    "Let me run it and see if it crashes"
                }) == 0)
                {
                    base.Close();
                }

                Properties.Settings.Default.hideTempWarningMessage = hideTempWarningMessage;
                Properties.Settings.Default.Save();
            }
            if (Application.StartupPath.ToLower().Contains("temp") && !Properties.Settings.Default.hideTempWarningMessage)
            {

                bool hideTempWarningMessage = false;
                if (Cliver.Message.Show("No no no no noooo!!!!", SystemIcons.Warning,
                    out hideTempWarningMessage,
                    "The path that this program is running from\n\"" + Application.StartupPath +
                    "\" \nAppears to be a temp directory (like from winrar)\r\nThis program will NOT work unless you fully extract it first!\r\n\r\nPlease close this program, extract it instead of running it, and then run the extracted program.", 0, new string[]
				{
					"Ok", 
					"I PROMISE I have fully extracted this, and know what that means"
				}) == 0)
                {
                    base.Close();
                }
                Properties.Settings.Default.hideTempWarningMessage = hideTempWarningMessage;
                Properties.Settings.Default.Save();
                
            }

            if (Directory.Exists(Application.StartupPath + @"\st\"))
            {
                this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\st\", true);
            }
            if (Directory.Exists(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_OPTIONS+"\\"))
            {
                this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_OPTIONS+"\\", true);
            }
            if (Directory.Exists(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL+"\\"))
            {
                this.SIFileOp.DirectoryDelete(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL+"\\", true);
            }
            if (Directory.Exists(Application.StartupPath + "\\" + c_DIRECTORY_NAME_FOR_FIXING_ZIPS + "\\"))
            {
                this.SIFileOp.DirectoryDelete(Application.StartupPath + "\\" + c_DIRECTORY_NAME_FOR_FIXING_ZIPS + "\\", true);
            }
            if (Directory.Exists(Application.StartupPath + @"\spctemp\"))
            {
                this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\spctemp\", true);
            }
            if (isValidLoLDir(Properties.Settings.Default.gameDir ))
            {
                gameDirectory = Properties.Settings.Default.gameDir;
            }
            else if (isValidLoLDir(usGameDirectory))
            {
                gameDirectory = usGameDirectory;
            }
            else if (isValidLoLDir(euGameDirectory ))
            {
                gameDirectory = euGameDirectory;
            }
            else if (isValidLoLDir(euGameDirectory64))
            {
                gameDirectory = euGameDirectory64;
            }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Application.StartupPath;
                dialog.Title = @"Please locate your League of Legends\lol.launcher.exe file..";
                dialog.CheckFileExists = true;
                dialog.FileName = "lol.launcher.exe";
                dialog.Filter = "Executable (*.exe)|*.exe";

                bool flag = true;
                while ((gameDirectory == string.Empty) && flag)
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string[] dialogFileNamesSplitPath = dialog.FileName.ToString().ToLower().Split(new char[] { '\\' });
                        for (int i = 0; i < (dialogFileNamesSplitPath.Length - 1); i++)
                        {
                            gameDirectory = gameDirectory + dialogFileNamesSplitPath[i] + @"\";
                        }
                        if (isValidLoLName(dialogFileNamesSplitPath[dialogFileNamesSplitPath.Length - 1]))
                        {
                            Properties.Settings.Default.gameDir = gameDirectory;
                            Properties.Settings.Default.Save();
                            flag = false;
                        }
                        else
                        {
                            gameDirectory = string.Empty;
                            if (Cliver.Message.Show("Error", SystemIcons.Error, "Failed to locate League of Legends.exe, do you want to try again?", 0, new string[2] { "Yes", "No" }) == 0)
                            {
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                    }
                    else if (Cliver.Message.Show("Error", SystemIcons.Error, "Failed to locate League of Legends.exe, do you want to try again?", 0, new string[2] { "Yes", "No" }) == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }

            this.rmdf = RelManDirectoryFile.RelManDirectoryFileFromRiotRoot(gameDirectory);
            removeSoftRepairFile();
            //List<RelFileEntry> entries = rmdf.fileList.SearchFileEntries("lol_sfx_hud.fsb");
           // RAFArchive raf = new RAFArchive("C:\\Riot Games\\League of Legends\\RADS\\projects\\lol_game_client\\filearchives\\0.0.0.155\\Archive_65414672.raf");
            string compFile = "zacwgoomovemoving.luaobj";

            //if (gameDirectory.ToLower().Contains("garena"))
            //{
            //    Cliver.Message.Show("Unsupported :<", SystemIcons.Error,
            //        "The garena client is currently not supported by SIU,\r\nPlans are to add this soon,\r\n\r\n" +
            //        "Most features will not work right now, but you can still use it to check for updates", 0,
            //     new string[1] { "OK" });
            //}            else
            if (!File.Exists(Application.StartupPath + "\\allfiles.ini"))// && File.Exists(gameDirectory + "HeroPak_client.zip"))
            {
                Cliver.Message.Inform("Looks like this is your First Time Running this program.\r\n" +
                    "Please be patient while we inspect your LoL installation, and format any pre-packed skins\r\n\r\n" +
                    "Go ahead and approve any \"ok\" dialogs,\r\n" +
                    "eventually you will see the main window.  for this first install, you should wait for the progress bar at the bottom to finish");
                this.UpdateFileList();
                
                b_IAddDirectory.Enabled = false;
                b_IAddFiles.Enabled = false;
            }
            else if (File.Exists(Application.StartupPath + "\\allfiles.ini"))
            {
                this.ReadFilelistINI();
                //this.CheckForUpdate(false);//remove this later
            }

            setImageValue(Properties.Settings.Default.iconSize,true);
            this.UpdateListView();
            updateWorker2.RunWorkerAsync(new object[] { this, false });
            backgroundWorkerCountUpdate.RunWorkerAsync();
            this.timeupdatecount.Start();
            lvwColumnSorter.SortColumn = Properties.Settings.Default.columnToSortBy;
            lvwColumnSorter.Order = Properties.Settings.Default.sortAscending ? SortOrder.Ascending : SortOrder.Descending;
            this.listView1.Sort();
            this.loadListViewSettings();
            doBrowser(Properties.Settings.Default.showAds);

        }
        public void removeSoftRepairFile()
        {
            string dirToSearch = gameDirectory + "/RADS/projects/lol_game_client/";
            if (Directory.Exists(dirToSearch))
            {
                try
                {
                    string[] filesInDir = Directory.GetFiles(dirToSearch, "*.*", SearchOption.AllDirectories);
                    foreach (string file in filesInDir)
                    {
                        FileInfo info = new FileInfo(file);
                        if (info.Name == "SOFT_REPAIR")
                        {
                            SIFileOp.FileDelete(info.FullName);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                	
                }
            }            

        }
        public void loadNameReplacements()
        {
            charFixs = new Dictionary<string, string>();

            //hud fix
            charFixs.Add("hudatlas2.tga", "hudatlas.tga");

            //charFixs.Add("wallofgrass.dds", "brush_sr.dds");

            charFixs.Add("chemicalman", "singed");
            //f rammus man
            charFixs.Add("\\armordillo_", "\\rammus");
            charFixs.Add("armordillo_pb.s", "rammuspb.s");
            charFixs.Add("armordillo_dbc.s", "rammusdbc.s");
            charFixs.Add("armordillo_dbc_attack.", "rammus_dbc_attack1.");
            charFixs.Add("armordillo_dbc_idle.", "rammus_dbc_idle1.");
            charFixs.Add("armordillo_dbc_spell.", "rammus_dbc_spell1.");
            charFixs.Add("armordillo_pb_idle.", "rammus_pb_idle1.");
            charFixs.Add("armordillo_pb_run.", "rammus_pb_run_60fps.");
            charFixs.Add("armordillo_shellbashcast.", "rammus_spell1_windup.");
            charFixs.Add("armordillo_spellcast.", "rammus_spell3.");
            charFixs.Add("armordillo_spellcast1.", "rammus_spell4.");            
            charFixs.Add("armordillo", "rammus");

            //shen anim renames
            charFixs.Add("shen_vorpalblade.anm", "shen_spell1.anm");
            charFixs.Add("shen_shurikenspray_1080.anm", "shen_spell2.anm");
            charFixs.Add("shen_shadowdash.anm", "shen_spell3.anm");
            charFixs.Add("shen_standunited.anm", "shen_spell4.anm");
            charFixs.Add("shen_standunited_windup.anm", "shen_spell4_windup.anm");


            charFixs.Add("armsmaster", "jax");
            charFixs.Add("bantamtrap", "teemo Mushroom");
            charFixs.Add("bear", "annietibbers");
            charFixs.Add("blindmonk", "leesin");
            charFixs.Add("bowmaster", "ashe");
            charFixs.Add("cardmaster", "twistedfate");
            charFixs.Add("chronokeeper", "zilean");
            charFixs.Add("cryophoenix", "anivia");
            charFixs.Add("rebirthegg", "aniviaegg");
            charFixs.Add("iceblock", "aniviaiceblock");

            //charFixs.Add("darkchampion.skl", "tryndamere_2011.skl");
            charFixs.Add("darkchampion.dds", "tryndamere_base2011_tx_cm.dds");
            charFixs.Add("darkchampion", "tryndamere");

            charFixs.Add("fallenangel", "morgana");
            charFixs.Add("gemknight", "taric");
            charFixs.Add("greenterror", "chogath");
            charFixs.Add("jester", "shaco");
            charFixs.Add("judicator", "kayle");
            charFixs.Add("lich", "karthus");
            charFixs.Add("minotaur", "alistar");
            charFixs.Add("pirate", "gangplank");
            charFixs.Add("sadmummy", "amumu");
            charFixs.Add("steamgolem", "blitzcrank");
            charFixs.Add("voidwalker", "kassadin");
            charFixs.Add("yeti", "nunu");
            charFixs.Add("wolfman", "warwick");
            charFixs.Add("caitlyn_trap", "caitlyntrap");
            //corki
            charFixs.Add("bobsled","icetoboggan");
            charFixs.Add("flyingsauce","ufo");
            charFixs.Add("urf.", "urfrider.");
            charFixs.Add("gragas_hillb.s", "gragas_hillbilly.s");
            //heimer turrets
            charFixs.Add("h28gevolutionturret", "heimertyellow");
            charFixs.Add("h28green", "heimertgreen");
            charFixs.Add("h28red", "heimertred");
            charFixs.Add("h28redfrost", "heimertblue");

            charFixs.Add("\\kogmaw_dead", "\\kogmawdead");
            charFixs.Add("maokai_sproutling", "maokaisproutling");
            charFixs.Add("\\oriana_noball", "\\oriannanoball");
            charFixs.Add("\\oriana_ball", "\\oriannaball");
            charFixs.Add("oriana", "orianna");


            charFixs.Add("\\jackinthebox", "\\shacobox");
            charFixs.Add("jackinthebox.i", "shacobox.i");
            
            charFixs.Add("swain_nobird", "swainnobird");
            charFixs.Add("swain_raven", "swainraven");

            charFixs.Add("\\bantamtrap", "\\teemomushroom");
            charFixs.Add("\\plagueblock", "\\trundlewall");

            charFixs.Add("xenzhao", "xinzhao");

            charFixs.Add("airavatar.dds", "janna_base_tx_cm.dds");
            charFixs.Add("teemo_plant_motion_sensor.anm","teemo_spell4.anm");
            charFixs.Add("teemo_spellcast.anm","teemo_spell3.anm");

            charFixs.Add("masteryi.dds", "masteryi_2013_tx_cm.dds");


        }
        public void addInFiles(String who, string dir)
        {
            Assembly _assembly = Assembly.GetExecutingAssembly();
            
            string[] files = _assembly.GetManifestResourceNames();
            //textBox1.Text = "";
            foreach (string file in files)
            {
                string toFind = "fixs." + who.ToLower() + ".";
                if (file.ToLower().Contains(toFind))
                {
                    string fileName = file.Substring(file.IndexOf(toFind) + toFind.Length);
                    debugadd("Going to copy over reasorce file: " + file+" at file name "+fileName);
                    String folder = Application.StartupPath + "\\" + dir;
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    BinaryReader br = new BinaryReader(_assembly.GetManifestResourceStream
                        (file));

                    BinaryWriter bw = new BinaryWriter(File.Create(folder+"\\"+fileName));

                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, read);
                    }


                    br.Close();
                    bw.Close();
                
                    //textBox1.Text += file + "\r\n";
                }
            }
            
            
            
            
        }
        public int string2DXInt(string dx)
        {
            dx = dx.Trim().ToLower();
            int toReturn =Tao.DevIl.Il.IL_DXT3;
            switch (dx)
            {
                case "dxt1":
                    toReturn = Tao.DevIl.Il.IL_DXT1;
                    break;
                case "dxt2":
                    toReturn = Tao.DevIl.Il.IL_DXT2;
                    break;
                case "dxt3":
                    toReturn = Tao.DevIl.Il.IL_DXT3;
                    break;
                case "dxt4":
                    toReturn = Tao.DevIl.Il.IL_DXT4;
                    break;
                case "dxt5":
                    toReturn = Tao.DevIl.Il.IL_DXT5;
                    break;
            }
            return toReturn;
        }
        public void debugadd(string tooAdd)
        {
            if(true)
            debug.Append(tooAdd + "\r\n");
        }
        void UpdateProgressSafe(int value, string extra="")
        {
            if (value > this.progressBar1.Maximum) value = this.progressBar1.Maximum;
            MethodInvoker action = delegate
            {
                this.progressBar1.Value = value;
                string extras = "";
                if (extra != "") extras = " (" + extra + ")";
                this.label2Percent.Text = ((value != 0) ? value.ToString() + "%"+extras: "");
                this.progressBar1.Refresh();
            };
            this.progressBar1.BeginInvoke(action);
        }
        private void Log(string s)
        {
            this.debugadd(s);
            //log.Text += "\r\n" + s;
            //log.SelectionStart = log.Text.Length;
            //log.ScrollToCaret();
        }
        public void rafBackup(string destination, string altDestination="", bool addLocalDirsToPath=false)
        {
            String rafLocation = destination.Substring(0, destination.IndexOf(".raf") + 4);
            //do a test for raf.dat, warn if not exist
            if (!File.Exists(rafLocation + ".dat"))
            {
                Cliver.Message.Show("Big Warning!!!", SystemIcons.Warning,"ERROR!\n\nIt looks like this skin is totally messed up right now\n\nPlease use the \"Fix Skin\" button on it asap!", 0, new string[] { "OK" });
            }
            String rafInnerLocation = destination.Substring(destination.IndexOf(".raf") + 5).Replace("\\", "/");
            String localLocation = destination.Replace(gameDirectory, "");
            String backupDir = Application.StartupPath + @"\backup\";
            if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
            string backupDest = backupDir + localLocation;
            if (altDestination != "")
            {
                backupDest = altDestination;
                if (addLocalDirsToPath) backupDest +=
                    rafInnerLocation.Replace("/", "\\");
                    //rafInnerLocation.Substring(0,rafInnerLocation.LastIndexOf("/")).Replace("/","\\");
            }
            

            string infos = "attempting to backup A RAF FILE\r\n\r\ndesitnation is\r\n" + destination + "\r\n raf file is\r\n" + rafLocation
             + "\r\ninner location is\r\n" + rafInnerLocation+"\r\n backup destination is \r\n"+backupDest;

            string[] strArray = backupDest.Split(new char[] { '\\' });
            string path = backupDest.Remove(backupDest.Length - strArray[strArray.Length - 1].Length);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            debugadd(infos);
            FileInfo rafFile = new FileInfo(rafLocation);
            RAFArchive raf = new RAFArchive(rafFile.FullName);
            //raf.
            List<RAFFileListEntry> files = raf.GetDirectoryFile().GetFileList().GetFileEntries();
            foreach (RAFFileListEntry entry in files)
            {
                string itslocation = entry.FileName;
                if (itslocation.ToLower() == rafInnerLocation.ToLower())
                {
                    //FileStream fStream = raf.GetDataFileContentStream();
            
                    debugadd("found matching raf file to backup at " + entry.FileName);
                    //byte[] buffer = new byte[entry.FileSize];
                    //fStream.Seek(entry.FileOffset, SeekOrigin.Begin);
                    //fStream.Read(buffer, 0, (int)entry.FileSize);
                    
                    
                    if (File.Exists(backupDest))
                    {
                        entry.RAFArchive.GetDataFileContentStream().Close();
              
                        return;
                    }

                    File.WriteAllBytes(backupDest, entry.GetContent());

                    entry.RAFArchive.GetDataFileContentStream().Close();
                    return;
                        
                    /*try
                    {
                        MemoryStream mStream = new MemoryStream(buffer);
                        zlib.ZInputStream zinput = new zlib.ZInputStream(mStream);

                        List<byte> dBuffer = new List<byte>(); //decompressed buffer, arraylist to my knowledge...
                        int data = 0;
                        while ((data = zinput.Read()) != -1)
                            dBuffer.Add((byte)data);
                        //ComponentAce.Compression.Libs.zlib.ZStream a = new ComponentAce.Compression.Libs.zlib.ZStream();
                        File.WriteAllBytes(backupDest, dBuffer.ToArray());
                    }
                    catch
                    {
                        debugadd("UNABLE TO DECOMPRESS FILE, WRITING DEFLATED FILE:" + entry.FileName);
                        File.WriteAllBytes(backupDest, buffer);
                    }*/

                }
                else
                {
                    //debugadd("error, unable to find match for, comparing " + itslocation + " to " + rafInnerLocation);
                }
            }

            raf.GetDataFileContentStream().Close();
            
        }
        public void rafInject(string origonal, string destination)
        {
            
            //\\LGG_MINISERVE\Super Nova\Documents\Projects\LOL-Skinning_StartUp_Kit\Installer creation\Skin Installer Ultimate\bin\debug\skins\annietesting\rads\projects\lol_game_client\filearchives\0.0.0.25\archive_84986336.raf\data\particles\anniebasicattack03.dds
//desitnation is
//C:\Riot Games\League of Legends - Copy\rads\projects\lol_game_client\filearchives\0.0.0.25\archive_84986336.raf\data\particles\anniebasicattack03.dds
            String rafLocation = destination.Substring(0, destination.IndexOf(".raf")+4);
            String rafInnerLocation = destination.Substring(destination.IndexOf(".raf") + 5).Replace("\\","/");
            string infos = "THIS IS A RAF FILE\r\n\r\nOrigonal is\r\n" + origonal +
              "\r\ndesitnation is\r\n" + destination + "\r\n raf file is\r\n" + rafLocation
             + "\r\ninner location is\r\n" + rafInnerLocation;
            debugadd(infos);
            //Cliver.Message.Inform(infos);

                FileInfo rafFile = new FileInfo(rafLocation);
                RAFArchive raf = new RAFArchive(rafFile.FullName);
            byte[] origonalBytes =File.ReadAllBytes(origonal);
            try
            {
                raf.InsertFile(
                                rafInnerLocation,
                                origonalBytes,
                                new LogTextWriter(
                                    (Func<string, object>)delegate(string s)
                                    {
                                        Log(s);
                                        return null;
                                    }
                                ));
            }
            catch
            {
                debugadd("O no big error trying to insert !!");
            }
                // Cliver.Message.Inform("Error on " + rafInnerLocation + "\r\n and " + origonal);
            // }
            raf.SaveDirectoryFile();
            raf.GetDataFileContentStream().Close();

            RelManDirectoryFile rmdf = RelManDirectoryFile.RelManDirectoryFileFromRiotRoot(gameDirectory);
            if (rmdf.valid)
            {
                rmdf.adjustSizeByBytes(origonalBytes, rafInnerLocation);
                rmdf.saveFile();
            }
            else
            {
                    Cliver.Message.Show("Error Modifying Release Manifest", SystemIcons.Error, "Warning, there was an error finding your release manifest file!\n\n" +
                         "This means that something is weird inside your LoL directory\n\nIt is probably best to do a full repair.\n" +
                         "Go To " + gameDirectory + "\\RADS\\projects\\ and delete the folder \"lol_game_client\"\nThen open up your launcher for LoL, click the gear on the top right, and choose repair.\n\n" +
                         "It may also help to right click -> run as administrator, this program as well", 0, new string[] { "OK!" });
                    return;
                
            }
        }
        #region notImportant
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
        private void b_AddFiles_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Browse for files to add to the zip pack";
        }
        private void euClient_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = euGameDirectory;
        }
        private void exit_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Exit the program";
        }
        private void b_IInstallFiles_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Installs all files from the file list to your game directory";
        }
        private void clearButton_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Clears all currently loaded settings and files";
        }
        private void fileNameTextBox_MouseEnter(object sender, EventArgs e)
        {
            if (this.fileNameTextBox.Text == "")
            {
                this.helpText.Text = "Use the browse button to the right to locate a file..";
            }
            else
            {
                this.helpText.Text = "The name and location of the loaded file";
            }
        }
        private void selectchampionbox_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Select the champion you want to work with";
        }
        private void filePackTextBox_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "The location of the selected zip file";
        }
        private void createZip_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Creates a zip file from the selected skin(s)";
        }
        private void colorSlider1_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Change the character icon size";
        }
        private void dbDelete_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Deletes the selected skin(s) from the database";
        }
        private void dbInstall_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Installs the selected skin(s) to your game directory";
        }
        private void locateGameClient_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = gameDirectory;
        }

        private void NoToolTip_MouseLeave(object sender, EventArgs e)
        {
            this.helpText.Text = "";
        }

        private void otherClient_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = otherGameDirectory;
        }

        private void packFileList_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "The list of files to be included in your zip file pack";
        }

        private void packFileNameTextBox_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Specify a name for your zip file pack";
        }

        private void installFiles_ListBox_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "The list of files to be installed";
        }

        private void installParticles_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Select and install particle files";
        }

        private void loadScreenImageTextBox_MouseEnter(object sender, EventArgs e)
        {
            if (this.loadScreenImageTextBox.Text == "")
            {
                this.helpText.Text = "Use the browse button to the right to locate a file..";
            }
            else
            {
                this.helpText.Text = "The name and location of the loaded file";
            }
        }


        private void dbUninstall_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Uninstalls the selected skin(s) from your game directory";
        }

        private void resetLoadScreenBox_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Clears the Load Screen Image field";
        }

        private void resetSkinBox_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Clears the Skin Location field";
        }
        private void reset_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Resets the selected champions skin to the game default";
        }
        private void skinNameTextbox_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Specify a name for this skin or file compilation";
        }

        private void skinVarient_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Select the skin variant you want to work with";
        }
        private void Update_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Extracts your HeroPak file and creates an updated allfiles.ini";
        }
        private void usClient_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = usGameDirectory;
        }
        private void restore_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Restores the seletected champions skin to an earlier version";
        }
        private void saveToDb_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Check this box to add this skin to the database when installing";
        }

        private void b_Browse_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Browse for the LoL Skin zip file you wish to install";
        }

        private void b_ClearAll_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Clears the zip file name and all files you have added to the pack";
        }

        private void b_CreatePack_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Creates the zip pack with the given name and files";
        }

        private void b_IAddDirectory_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Browse for a folder to add to the file list";
        }
        private void b_IAddFiles_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Browse for file(s) to add to the file list";
        }
        private void b_IClearAll_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Clears all the files in the file list";
        }

        private void b_InstallPack_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Install the selected LoL Skin zip file";
        }

        private void b_IRemoveFiles_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Removes the selected file from the file list";
        }

        private void b_RemoveFiles_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Removes a selected file from the zip pack";
        }

        private void browse_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Locate the Champion Skin you want to install";
        }

        private void browseLoadScreen_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Locate the Load Screen Image you want to install";
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Add this file compilation to the database (without installing)";
        }

        private void changeSkinButton_MouseEnter(object sender, EventArgs e)
        {
            this.helpText.Text = "Install the selected skin or loading screen image";
        }

        #endregion notImportant
        #region ADDtoDatabase
        private void b_IAddDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (lastDirectoryBrowsed != string.Empty)
            {
                dialog.SelectedPath = lastDirectoryBrowsed;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(Application.StartupPath + @"\st"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\st");
                }
                directoryPath = dialog.SelectedPath;
                string[] strArray3 = Directory.GetFiles(dialog.SelectedPath, "*.*", SearchOption.AllDirectories);
                if (strArray3.Length > 10000)
                {
                    Cliver.Message.Inform("You have selected " + (strArray3.Length + 1) + " files.\nDue to the risk of error, the maximum file limit has been set to 10000\nPlease try again with a smaller selection.");
                }
                else
                {
                    //checked, ok
                    processNewDirectory(strArray3);
                }
            }
            lastDirectoryBrowsed = dialog.SelectedPath;
        }
        private String fixRiotv40Name(string nameAndPath)
        {
            FileInfo fi = new FileInfo(nameAndPath.ToLower());
            string dirName = fi.DirectoryName;
            string fileName = fi.Name;
            foreach (KeyValuePair<String,String> keyValue in charFixs)
            {
                dirName = dirName.Replace(keyValue.Key.ToLower(),keyValue.Value.ToLower());
                fileName = fileName.Replace(keyValue.Key.ToLower(), keyValue.Value.ToLower());
            
            }
            
            String fullName = dirName +"\\"+ fileName;

            return fullName;
        }
        private List<String> addInOldFiles(List<String> currentFiles)
        {
            {//sound fix
                bool hasSounds=false;
                foreach (string fileName in currentFiles)
                {
                    FileInfo fi = new FileInfo(fileName);
                    if (fi.Extension.ToLower().Contains("wav") || fi.Extension.ToLower().Contains("mp3")) hasSounds = true;
                }
                if (hasSounds)
                {
                    //not needed with new sound method
                    //addInFiles("soundfix", c_TEMP_DIR_NAME_FIXED_SKIN_FILES);
                }
            }
            {//trynd fix check
                bool noAnimations = true;
                bool oldSKNFound = false;
                bool oldDDSFound = false;
                foreach (string fileName in currentFiles)
                {
                    FileInfo fi = new FileInfo(fileName);
                    if (fi.Extension.ToLower().Contains("anm")) noAnimations = false;
                    if (fi.Name.ToLower().Contains("darkchampion.skn")) oldSKNFound = true;
                    if (fi.Name.ToLower().Contains("darkchampion.dds")) oldDDSFound = true;
                }
                if ((oldDDSFound == true) && (oldSKNFound == false))
                {
                    oldSKNFound = true;
                    addInFiles("tryndamereskn", c_TEMP_DIR_NAME_FIXED_SKIN_FILES);
                }
                if ((oldSKNFound == true) && (noAnimations == true))
                {
                    //we need the backup files!
                    addInFiles("tryndamereanm", c_TEMP_DIR_NAME_FIXED_SKIN_FILES);
                }
            }


            {//janna fix check
                bool noAnimations = true;
                bool oldDDSFound = false;
                bool anySKNFound = false;
                foreach (string fileName in currentFiles)
                {
                    FileInfo fi = new FileInfo(fileName);
                    if (fi.Extension.ToLower().Contains("anm")) noAnimations = false;
                    if (fi.Name.ToLower().Contains("airavatar.dds")) oldDDSFound = true;
                    if (fi.Name.ToLower().Contains("janna.skn")) anySKNFound = true;
                }
                if ((oldDDSFound == true) && (anySKNFound == false))//assume they need the old skin too
                {
                    addInFiles("jannaskn", c_TEMP_DIR_NAME_FIXED_SKIN_FILES);
                }
                if ((oldDDSFound == true) && (noAnimations == true))
                {
                    //we need the backup files!
                    addInFiles("jannaanm", c_TEMP_DIR_NAME_FIXED_SKIN_FILES);
                }
            }
            {//shen fix
                //bool oldShen = false;
                if (Properties.Settings.Default.removeUnsafeFiles)
                {

                    string list = "";
                    string inibin = "";
                    foreach (string fileName in currentFiles)
                    {
                        FileInfo fi = new FileInfo(fileName);
                        //if (fi.Extension.ToLower().Contains("shen_idle1_surrendering.anm")) oldShen = true;
                        if (fi.Name.ToLower().Contains(".list")) list = fileName;
                        if (fi.Name.ToLower().Contains(".inibin")) inibin = fileName;
                    }
                    if (list != "")
                        currentFiles.Remove(list);
                    if (inibin != "")
                        currentFiles.Remove(inibin);
                }

            }
            if (Directory.Exists(Application.StartupPath + @"\" + c_TEMP_DIR_NAME_FIXED_SKIN_FILES))
            {
                string[] filesInFXdir = Directory.GetFiles(Application.StartupPath + @"\" + c_TEMP_DIR_NAME_FIXED_SKIN_FILES, "*.*", SearchOption.AllDirectories);
                currentFiles.AddRange(filesInFXdir);
            }
            return currentFiles;
        }
        
        private void addFilesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcessParams1 params1 = (ProcessParams1)e.Argument;
            string[] origonalInputFiles = params1.files;//e.Argument as string[];
            processNewDirectoryWork(origonalInputFiles,params1.allowDialog);
            e.Result = params1;
        }

        private void addFilesWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage);
            if (e.UserState != null)
            {
                string info = e.UserState as string;
                //debugadd(info);
                //this.statusText.Text = info;
                //this.helpBar.Update();

                this.installFiles_ListBox.Items.Add(info);
            }
        }

        private void addFilesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Error == null))
            {
                Cliver.Message.Inform("Error!\r\nSomething has gone wrong adding the skin,\r\n" +
                    "You can normally stop this error from happening by downloading and installing dot net 3.5 from\r\n" +
                    " here http://www.microsoft.com/downloads/en/details.aspx?familyid=ab99342f-5d1a-413d-8319-81da479ab0d7 " +
                    "\r\n\r\nIf you have done that, and you still get this error, please contact LGG, tell him what you did, and give him this info" +
                    "\r\n" + e.Error.ToString());
                debugadd("Error: " + e.Error.Message);
            }
            else
            {
                ProcessParams1 params1 = (ProcessParams1)e.Result;
                if (params1.runAutoCode)
                {

                    int doneAdding = Properties.Settings.Default.optionDoneAdding;
                    if (doneAdding == -1)
                    {
                        Cliver.Message.NextTime_ButtonColors = new Color[] { Color.LightGreen, Color.LightGreen };
                        bool saveThis = false;
                        doneAdding
                            = Cliver.Message.Show("Done Adding files?",
                                SystemIcons.Information, out saveThis,
                                "We have just added the files you dropped to the list of files\r\n"
                            + "that will be part of your skin, do you have more files to add?\r\n"
                            + "or are you ready to finalize this skin and add it to the database?",
                                0, new string[2] { "I am done adding files, finalize this skin.", "I am not done yet." });
                        if (saveThis) Properties.Settings.Default.optionDoneAdding = doneAdding;
                        Properties.Settings.Default.Save();
                    }
                    if (doneAdding == 0)
                    {
                        //they are done, install skin
                        addToDatabase_click(null, null);
                        //button1.PerformClick();
                    }
                }
                downloadWaitingFile(params1.urlToDownloadAsyncAfter);
            }

        }

        struct ProcessParams1
        {
            public string[] files;
            public bool runAutoCode;
            public bool allowDialog;
            public string urlToDownloadAsyncAfter;
        }
        private void processNewDirectory(string[] origonalInputFiles, bool runAutoAddCode = false, bool allowDialog = true, string urlToDownloadAsynAfter = "")
        {
            if (addFilesWorker.IsBusy)
            {
               Cliver.Message.Inform("Please wait for the current files to complete being added to the list before you add more");
               return;
            }
            addFilesPanel.BackColor = System.Drawing.SystemColors.Control;
            AddToDatabasePanel.BackColor = Color.Lime;
            ProcessParams1 params1;
            params1.urlToDownloadAsyncAfter = urlToDownloadAsynAfter;
            params1.runAutoCode = runAutoAddCode;
            params1.files = origonalInputFiles;
            params1.allowDialog = allowDialog;
            
            addFilesWorker.RunWorkerAsync(params1);
        }
        private string add_0_toName(string inName)
        {
            int extensionLocation = inName.LastIndexOf(".");
            if (extensionLocation < 0) extensionLocation = inName.Length;
            inName = inName.Insert(extensionLocation, "_0");
            return inName;
        }
        private void processNewDirectoryWork(string[] origonalInputFiles, bool allowDialog=true)
        {
            //addFilesPanel.BackColor = System.Drawing.SystemColors.Control;
            //AddToDatabasePanel.BackColor = Color.Lime;

            int num = 0;
            int num2 = 0;
            ListBox box = new ListBox();


            if (!Directory.Exists(Application.StartupPath + @"\st"))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\st");
            }
            if (Directory.Exists(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
            {
                this.SIFileOp.DirectoryDelete(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, true);
            }
            if (Directory.Exists(Application.StartupPath + @"\" + c_TEMP_DIR_NAME_FIXED_SKIN_FILES))
            {
                this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\" + c_TEMP_DIR_NAME_FIXED_SKIN_FILES, true);
            }
            int fileNumber = 1;
            foreach (string origonalFile in origonalInputFiles)
            {

                

                if ((origonalFile.Trim().ToLower().EndsWith(".testzip")) ||
                    (origonalFile.Trim().ToLower().EndsWith(".testu9lolpatch")))
                {
                    ZipUtil.UnZipFiles(origonalFile, Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "", false);
                }
                else if (origonalFile.Trim().ToLower().EndsWith(".rar") ||
                        origonalFile.Trim().ToLower().EndsWith(".7z") ||
                        origonalFile.Trim().ToLower().EndsWith(".zip") ||
                        origonalFile.Trim().ToLower().EndsWith(".u9lolpatch") ||
                        origonalFile.Trim().ToLower().EndsWith(".yurixyworks") ||
                        origonalFile.Trim().ToLower().EndsWith(".lolmod") ||
                        origonalFile.Trim().ToLower().EndsWith(".bzip2") ||
                        origonalFile.Trim().ToLower().EndsWith(".gzip") ||
                        origonalFile.Trim().ToLower().EndsWith(".tar"))
                {

                    ExtractWith7z(origonalFile, Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL);
                }
                else if (origonalFile.Trim().ToLower().EndsWith("animations.ini"))
                {
                    //hack for ini to list mistakes
                    if (!Directory.Exists(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
                        Directory.CreateDirectory(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL);
                    FileInfo fi = new FileInfo(origonalFile);
                    SIFileOp.FileCopy(origonalFile, Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL+"\\" + fi.Name.Replace(".ini", ".list"));

                }
                else if (origonalFile.Trim().ToLower().EndsWith("animations.list"))
                {
                    if (!Directory.Exists(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
                        Directory.CreateDirectory(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL);
                    FileInfo fi = new FileInfo(origonalFile);
                    SIFileOp.FileCopy(origonalFile, Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL + "\\" + fi.Name.Replace(".list", ".ini"));
                }
                else if (origonalFile.Trim().ToLower().EndsWith("wallofgrass.dds") && origonalFile.Trim().ToLower().Contains("map1"))
                {
                    if (!Directory.Exists(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
                        Directory.CreateDirectory(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL);
                    FileInfo fi = new FileInfo(origonalFile);
                    SIFileOp.FileCopy(origonalFile, Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL + "\\" + fi.Name.Replace("wallofgrass.dds", "brush_sr.dds"));
                }

            }
            bool flag = false;
            while (!flag)
            {
                if (!Directory.Exists(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
                {
                    break;
                }
                string[] AniminiFiles = Directory.GetFiles(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "*.*",
                        SearchOption.AllDirectories).Where(s => s.EndsWith("animations.ini", StringComparison.OrdinalIgnoreCase)).ToArray<string>();
                foreach (string ai in AniminiFiles)
                {
                    SIFileOp.FileCopy(ai, ai.Replace(".ini", ".list"));
                }
                AniminiFiles = Directory.GetFiles(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "*.*",
                            SearchOption.AllDirectories).Where(s => s.EndsWith("animations.list", StringComparison.OrdinalIgnoreCase)).ToArray<string>();
                foreach (string ai in AniminiFiles)
                {
                    SIFileOp.FileCopy(ai, ai.Replace(".list", ".ini"));
                }

                string[] archiveFiles = Directory.GetFiles(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "*.*",
                        SearchOption.AllDirectories).Where(s =>
                           s.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".u9lolpatch", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".yurixyworks", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".lolmod", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".rar", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".bzip2", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".gzip", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".tar", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".7z", StringComparison.OrdinalIgnoreCase)
                            ).ToArray();

                if (archiveFiles.Length != 0)
                {
                    foreach (string archiveFileName in archiveFiles)
                    {
                        //Cliver.Message.Inform("going to extract " + str2+" cuz \r\n"+strArray4.Length.ToString());
                        String targ = Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL+"\\" +
                            archiveFileName.Substring(archiveFileName.LastIndexOf("\\")).Replace(".", "-") + "\\";
                        if (!Directory.Exists(targ)) Directory.CreateDirectory(targ);
                        ExtractWith7z(archiveFileName, targ);
                        //ZipUtil.UnZipFiles(str2, Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "", true);

                        SIFileOp.FileDelete(archiveFileName);
                    }
                }
                else
                {
                    flag = true;
                }
            }
            //at this point all arhives have been extracted into the sti dir
            List<string> allFilesInTheSkin = new List<string>();
            allFilesInTheSkin.AddRange(origonalInputFiles);
            if (Directory.Exists(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
            {
                string[] wallOfGrassFiles = Directory.GetFiles(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "*.*",
                        SearchOption.AllDirectories).Where(s => s.EndsWith("wallofgrass.dds", StringComparison.OrdinalIgnoreCase)).ToArray<string>();
                foreach (string wog in wallOfGrassFiles)
                {
                    if (wog.Trim().ToLower().Contains("map1"))
                    {
                        FileInfo fi = new FileInfo(wog);
                        SIFileOp.FileCopy(fi.FullName, Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL + "\\" + fi.Name.Replace("wallofgrass.dds", "brush_sr.dds"));
                    }
                }

                string[] filesInSTIdir = Directory.GetFiles(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "*.*", SearchOption.AllDirectories);
                allFilesInTheSkin.AddRange(filesInSTIdir);
            }

            //this is important
            allFilesInTheSkin = addInOldFiles(allFilesInTheSkin);

            StringBuilder InfoLog = new StringBuilder("");
            int maxFiles = allFilesInTheSkin.ToArray().Length;
            foreach (string fullNameAndPath in allFilesInTheSkin)
            {
                int percent = (int)Math.Floor(((double)fileNumber++ / (double)maxFiles) * 100.0);
                addFilesWorker.ReportProgress(percent);
                //debugadd("starting on " + fullNameAndPath);
                string foldername = null;
                string fileName = null;
                //bool flag2 = false;
                string[] brokenUpPathWithFileName = fullNameAndPath.Split(new char[] { '\\' });
                if (true)//flag2)
                {
                    String oldFullNameAndPath = fullNameAndPath;
                    String newFullNameAndPath = this.fixRiotv40Name(fullNameAndPath);
                    //cuz we cnat check sounds yet,error on the side of making new sounds work
                    if (oldFullNameAndPath.ToLower().Contains(".wav")) newFullNameAndPath = oldFullNameAndPath;
                    if (oldFullNameAndPath.ToLower().Contains(".mp3")) newFullNameAndPath = oldFullNameAndPath;
                  
                    FileInfo fiold = new FileInfo(oldFullNameAndPath);
                    FileInfo finew = new FileInfo(newFullNameAndPath);
                    String mixedNewPathOldName = finew.DirectoryName + "\\" + fiold.Name;
                    String newPathWith0 = add_0_toName(newFullNameAndPath);
                    String mixedPathWith0 = add_0_toName(mixedNewPathOldName);

                    //String mixedOldPathNewName = fiold.DirectoryName + "\\" + finew.Name;

                    fileLocReturn namePath = this.FileNameToLocation(newFullNameAndPath, true,null);//try new names first
                    if (!namePath.valid)
                    {
                        namePath = this.FileNameToLocation(mixedNewPathOldName, true, namePath.moreOptions);
                    } if (!namePath.valid)
                    {
                        namePath = this.FileNameToLocation(newPathWith0, true, namePath.moreOptions);
                    } if (!namePath.valid)
                    {
                        namePath = this.FileNameToLocation(mixedPathWith0, true, namePath.moreOptions);
                    }
                    //fileName = this.FileNameToLocation(brokenUpPathWithFileName[brokenUpPathWithFileName.Length - 1], false,true);
                    if (!namePath.valid)//foldername == null || fileName == null)
                    {
                        //fix names here armodillo_ to rammus folder
                        //String newfullNameAndPath = this.fixRiotv40Name(fullNameAndPath);
                        namePath = this.FileNameToLocation(oldFullNameAndPath, false, namePath.moreOptions);

                    }
                    if (!namePath.valid  && (namePath.moreOptions!=null))
                    {
                        //ask user
                        if (namePath.moreOptions.Count > 1) this.FilePossibles = namePath.moreOptions.ToArray();

                        this.FileLoc = fiold.DirectoryName;
                        FileLocForm form = new FileLocForm(this);
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.ShowDialog();
                        this.CorrectFileLocation = form.fileLoc;
                        form.Close();
                        if (this.CorrectFileLocation != "")
                        {
                            FileInfo fi = new FileInfo(this.CorrectFileLocation);
                            namePath = new fileLocReturn(fi.DirectoryName,fi.Name);
                        }
                    }
                    if (namePath.valid)//foldername != null && fileName!=null)
                    {
                        foldername = namePath.folderName.Replace(Application.StartupPath,"");
                        fileName = namePath.fileName;

                        Directory.CreateDirectory(Application.StartupPath + @"\st\" + foldername);
                        box.Items.Clear();
                        box.Items.Add(foldername +"\\"+ fileName);
                        if (this.installFiles_ListBox.Items.Contains(box.Items[0].ToString()))
                        {
                            if (Properties.Settings.Default.autoOverwrite
                                && (!Properties.Settings.Default.showAllWarnings))
                            {
                                copyAndFix(fullNameAndPath, Application.StartupPath + @"\st\" + foldername + @"\" + fileName);

                            }
                            else
                            {
                                if (Cliver.Message.Show("Error", SystemIcons.Error, "Error: " + fileName + "\nA file with this name has already been added, do you wish to overwrite it?" +
                                    "\n\n" + box.Items[0].ToString() + "\n\n" + fullNameAndPath + "\n\n" + foldername + "\n\n" + fileName
                                    , 0, new string[2] { "Yes", "No" }) == 0)
                                {
                                    //num++;
                                    copyAndFix(fullNameAndPath, Application.StartupPath + @"\st\" + foldername + @"\" + fileName);
                                }
                                else
                                {
                                    num2++;
                                }
                            }
                        }
                        else
                        {
                            num++;
                            //Cliver.Message.Inform("str4 is " + str4 + " and str5 is " + str5);
                            copyAndFix(fullNameAndPath, Application.StartupPath + @"\st\" + foldername + @"\" + fileName);
                            addFilesWorker.ReportProgress(percent, foldername + fileName);
                            //this.installFiles_ListBox.Items.Add(foldername + fileName);
                        }
                    }
                    else
                    {
                        
                        string toTest = brokenUpPathWithFileName[brokenUpPathWithFileName.Length - 1].ToLower();
                        if ((!toTest.Contains("animations.")) &&
                            (!toTest.Contains("thumbs.db")) &&
                            (!toTest.Contains(".zip")) &&
                            (!toTest.Contains(".u9lolpatch")) &&
                            (!toTest.Contains(".yurixyworks")) &&
                            (!toTest.Contains(".lolmod")) &&
                            (!toTest.Contains(".rar")) &&
                            (!toTest.Contains(".7z")) &&
                            (!toTest.Contains(".bzip2")) &&
                            (!toTest.Contains(".gzip")) &&
                            (!toTest.Contains(".tar")) &&
                            (!toTest.Contains(".inibin")))
                        {
                            num2++;
                            if (toTest.Contains(".wgt"))
                            {
                                InfoLog.Append("Error: " + brokenUpPathWithFileName[brokenUpPathWithFileName.Length - 1] +
                                    "\nThis file could not be identified and was skipped.\n" +
                                    "This is because wgt files are not yet supported :/\n" +
                                    "There is still a good chance the skin may work! Try it in a custom game first." +
                                    "\r\n");
                            }
                            else
                                if (toTest.Contains(".txt"))
                                {
                                    if (Cliver.Message.Show("Hey!", SystemIcons.Information,
                                        "The skin author included a text file named \"" + brokenUpPathWithFileName[brokenUpPathWithFileName.Length - 1] + "\" that does not look like it belongs in LoL.\r\n" +
                                        "\r\nThis probably means that they want you to read it, and it contains important information,\r\n" +
                                        "\r\nWould you like to read it now?"
                                       , 0, new string[2] { "Yes", "No" }) == 0)
                                    {
                                        //string filename = Application.StartupPath + @"\st\" + str4 + @"\" + str5;
                                        System.Diagnostics.Process.Start(fullNameAndPath);
                                    }
                                }
                                else
                                {
                                    InfoLog.Append("Error: " + brokenUpPathWithFileName[brokenUpPathWithFileName.Length - 1] + "\nThis file could not be identified and was skipped.\nThis normally means the skin creator accidentally included an extra file that was not needed for the skin to work." +
                                        "\r\n");
                                }

                            //Cliver.Message.Inform("Error: " + strArray5[strArray5.Length - 1] + "\nThis file could not be identified and was skipped.\n\nThis normally means the skin creator accidentally included an extra file that was not needed for the skin to work.");
                        }
                    }
                }
                else
                {
                    //num2++;
                }
                //debugadd("Finished " + fullNameAndPath);
            }
            float percentAdded = ((float)num/((float)(num+num2)))*100.0f;
            if (percentAdded < 20)
            {
                Cliver.Message.Show("Error: Not Many Files Found!", SystemIcons.Error, "Warning, there was a problem finding where these\nfiles belong in your LoL installation!\n\n" +
                     "This means one of two issues.\n\n1. You added a skin that was not really a skin for LoL\n(Skins for LoL have files named the same way that riot named them)\n"+
                     "To fix this, just add in a skin that other people have tested as working\n\n"+
                     "2. That SIU can't find the names of the files in your LoL!\n"+
                     "Right now SIU things that your LoL is installed at " + gameDirectory + "\n"+
                     "Please make sure this is correct, if it is not, please press the \"Game Client\" Button in SIU to change it, and let it scan, then try again.\n\n"+
                     "If that does not work, then that might mean that something is weird inside your LoL directory\nIt is probably best to do a full repair.\n" +
                     "Go To " + gameDirectory + "\\RADS\\projects\\ and delete the folder \"lol_game_client\"\nThen open up your launcher for LoL, click the gear on the top right, and choose repair.\n\n" +
                     "It may also help to right click -> run as administrator, this program as well", 0, new string[] { "OK!" });
            }
            string extra = "";
            if (InfoLog.ToString().Length > 1)
                extra = "\r\n\r\nINFO\r\n" + InfoLog.ToString();
            if (!Properties.Settings.Default.hideAddedFilesMessage && allowDialog)
            {
                if (Cliver.Message.Show("Successfully Added " + num.ToString() + " files",
                    SystemIcons.Information, string.Concat(new object[]
				{
					"Added ", 
					num, 
					" of ", 
					num + num2, 
					" files (skipped ", 
					num2, 
					")." + extra
				}), 0, new string[]
				{
					"OK", 
					"Do not tell me again (You can turn this back on in Options)"
				}) == 1)
                {
                    Properties.Settings.Default.hideAddedFilesMessage = true;
                    Properties.Settings.Default.Save();
                }
            }
            //cleanup
            debugadd("cleaning up folders");
            if (Directory.Exists(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
                SIFileOp.DirectoryDelete(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, true);
            if (Directory.Exists(Application.StartupPath + "\\" + c_TEMP_DIR_NAME_FIXED_SKIN_FILES))
            SIFileOp.DirectoryDelete(Application.StartupPath + "\\" + c_TEMP_DIR_NAME_FIXED_SKIN_FILES,true);
        }
        private void ExtractWith7z(string file, string toDirectory)
        {
            Process process = new Process();
            process.StartInfo.FileName = "7z.exe";
            process.StartInfo.Arguments = string.Concat(new string[]
						{
							" x \"",
 
							file.Replace("\\\\", "\\"), 
							"\" -aoa -o\"", 

							toDirectory
                            
                            +"\""
						});
            debugadd(process.StartInfo.Arguments);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Application.StartupPath;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;

            process.Start();
            int nc = -1;
            string redirectedOutput = string.Empty;
            do 
            {
                nc = process.StandardOutput.Read();
                if ((redirectedOutput += (char)nc).Contains("(will not be echoed)"))
                {
                    process.StandardInput.WriteLine("lorixy");
                    redirectedOutput = String.Empty;
                }
            } while (nc != -1);
            process.WaitForExit();
        }
        private void b_IAddFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog ();
            dialog.Filter = string.Empty;
            foreach (string str in allFilesExtensions)
            {
                if (dialog.Filter == string.Empty)
                {
                    dialog.Filter = "All Supported Files (*.*) | *.zip";
                }
                else
                {
                    dialog.Filter = dialog.Filter + "; *" + str;
                }
            }
            dialog.FileName = "";
            dialog.CheckFileExists = true;
            dialog.Multiselect = true;
            dialog.Title = "Please select skin files..";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] arrayFileNames = dialog.FileNames;
                #region oldCode
                /*
                Directory.CreateDirectory(Application.StartupPath + @"\st");
                foreach (string str2 in arrayFileNames)
                {
                    if (str2.Trim().ToLower().EndsWith(".zip"))
                    {
                        ZipUtil.UnZipFiles(str2, Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "", false);
                    }else if (
                        str2.Trim().ToLower().EndsWith(".rar")||
                        str2.Trim().ToLower().EndsWith(".7z")||
                        str2.Trim().ToLower().EndsWith(".bzip2") ||
                        str2.Trim().ToLower().EndsWith(".gzip")||
                        str2.Trim().ToLower().EndsWith(".tar")
                        )
                    {
                        //BZIP2, GZIP, TAR
                        ExtractWith7z(str2, Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL);                        
                    }
                    //check for animations.ini and convert to list, or vice versa
                    else if (str2.Trim().ToLower().EndsWith("animations.ini"))
                    {
                        SIFileOp.FileCopy(str2, str2.Replace(".ini", ".list"));
                        string[] array1 = new string[arrayFileNames.Length + 1];
                        arrayFileNames.CopyTo(array1, 0);
                        array1[array1.Length - 1] = str2.Replace(".ini", ".list");
                        arrayFileNames = array1;
                    }
                    else if (str2.Trim().ToLower().EndsWith("animations.list"))
                    {
                        SIFileOp.FileCopy(str2, str2.Replace(".list", ".ini"));
                        string[] array2 = new string[arrayFileNames.Length + 1];
                        arrayFileNames.CopyTo(array2, 0);
                        array2[array2.Length - 1] = str2.Replace(".ini", ".list");
                        arrayFileNames = array2;
                    }
            
                }
                bool flag = false;
                while (!flag)
                {
                    flag = true;
                    if (!Directory.Exists(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
                    {
                        break;
                    }
                    string [] strArray3 = Directory.GetFiles(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "*.*",
                        SearchOption.AllDirectories).Where(s=>
                           s.EndsWith(".rar", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".bzip2", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".gzip", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".tar", StringComparison.OrdinalIgnoreCase) ||
                           s.EndsWith(".7z", StringComparison.OrdinalIgnoreCase)                            
                            ).ToArray();

                    
                    if (strArray3.Length != 0)
                    {
                        foreach (string str3 in strArray3)
                        {
                            ExtractWith7z(str3, Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL);
                            //ZipUtil.UnZipFiles(str3, Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "", true);
                            flag = false;
                        }
                    }
                }
                if (!Directory.Exists(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL);
                }
                string[] strArray2 = Directory.GetFiles(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, "*.*", SearchOption.AllDirectories);
                string[] array = new string[arrayFileNames.Length + strArray2.Length];
                arrayFileNames.CopyTo(array, 0);
                strArray2.CopyTo(array, arrayFileNames.Length);
                foreach (string str4 in array)
                {
                    string[] strArray4 = str4.Split(new char[] { '\\' });
                    ListBox box = new ListBox();
                    bool flag2 = false;
                    foreach (string str5 in allFilesExtensions)
                    {
                        if (str4.Trim().ToLower().EndsWith(str5.ToLower()))
                        {
                            flag2 = true;
                        }
                    }
                    if (flag2)
                    {
                        string str6 = this.FileNameToLocation(str4, true);
                        string str7 = this.FileNameToLocation(strArray4[strArray4.Length - 1], false);
                        if (str6 != null)
                        {
                            Directory.CreateDirectory(Application.StartupPath + @"\st\" + str6);
                            box.Items.Clear();
                            box.Items.Add(str6 + str7);
                            if (this.installFiles_ListBox.Items.Contains(box.Items[0].ToString()))
                            {
                                if (Cliver.Message.Show("Error",SystemIcons.Error,"Error: " + str7 + "\nA file with this name has already been added, do you wish to overwrite it?",0,new string[2]{"Yes","No"}) == 0)
                                {
                                    num++;
                                    copyAndFix(str4, Application.StartupPath + @"\st\" + str6 + @"\" + str7);
                                }
                                else
                                {
                                    num2++;
                                }
                            }
                            else
                            {
                                num++;
                                copyAndFix(str4, Application.StartupPath + @"\st\" + str6 + @"\" + str7);
                                this.installFiles_ListBox.Items.Add(str6 + str7);
                            }
                        }
                        else if (str4.EndsWith(".txt"))
                        {
                            //if (Cliver.Message.Show("Open Text File?",SystemIcons.Question,"Found a text file, " + strArray4[strArray4.Length - 1] + ", would you like to open this file?", 0,new string[2]{"Yes","No"}) == 0)
                            //{
                            //    Process.Start(str4);
                            //}
                        }
                        else
                        {
                            num2++;
                            if ((!strArray4[strArray4.Length - 1].ToLower().Contains("animations.")) &&
                            (!strArray4[strArray4.Length - 1].ToLower().Contains("thumbs.db")) &&
                            (!strArray4[strArray4.Length - 1].ToLower().Contains(".zip")) &&
                            (!strArray4[strArray4.Length - 1].ToLower().Contains(".rar")) &&
                            (!strArray4[strArray4.Length - 1].ToLower().Contains(".7z")) &&
                            (!strArray4[strArray4.Length - 1].ToLower().Contains(".bzip2")) &&
                            (!strArray4[strArray4.Length - 1].ToLower().Contains(".gzip")) &&
                            (!strArray4[strArray4.Length - 1].ToLower().Contains(".tar")) &&
                            (!strArray4[strArray4.Length - 1].ToLower().Contains(".inibin")))
                            {
                                ////BZIP2, GZIP, TAR
                        
                                Cliver.Message.Inform("Error: " + strArray4[strArray4.Length - 1] + "\nCould not be identified and was skipped.");
                        
                                //Cliver.Message.Inform("Error: " + strArray5[strArray5.Length - 1] + "\nThis file could not be identified and was skipped.\n\nThis normally means the skin creator accidentally included an extra file that was not needed for the skin to work.");
                            }
                            //Cliver.Message.Inform("Error: " + strArray4[strArray4.Length - 1] + "\nCould not be identified and was skipped.");
                        }
                    }
                    else if (!str4.Trim().ToLower().EndsWith(".zip"))
                    {
                        num2++;
                    }
                }
                Cliver.Message.Inform(string.Concat(new object[] { "Added ", num, " of ", num + num2, " files (skipped ", num2, ")." }));
            }
            if (Directory.Exists(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
            {
                this.SIFileOp.DirectoryDelete(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, true);
            }*/
                #endregion
                processNewDirectory(arrayFileNames);
            }

        }
        private void b_IClearAll_Click(object sender, EventArgs e)
        {
            clearFiles(true);
        }
        private void clearFiles(bool confirm)
        {
            bool doit = !confirm;
            if (!doit)
            {
                if (Cliver.Message.Show("Confirm", SystemIcons.Question, "Are you sure you wish to remove all loaded files?", 0, new string[2] { "Yes", "No" }) == 0)
                {
                    doit = true;
                }
            }
            if (doit)
            {
                addFilesPanel.BackColor = Color.Lime;
                AddToDatabasePanel.BackColor = System.Drawing.SystemColors.Control; 
            
                this.skinNameTextbox.Text = "";
                this.textBoxauthor.Text = "Unknown";
                this.installFiles_ListBox.Items.Clear();
                if (Directory.Exists(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL))
                {
                    this.SIFileOp.DirectoryDelete(Application.StartupPath + "\\"+c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL, true);
                }
                if (Directory.Exists(Application.StartupPath + @"\st"))
                {
                    this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\st", true);
                }
            }
        }
        private void b_IInstallFiles_Click(object sender, EventArgs e)
        {
            //depreciated and not in use!!!
            
            /*
            int num = 0;
            if (!Directory.Exists(Application.StartupPath + @"\st"))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\st");
            }
            string[] strArray = Directory.GetFiles(Application.StartupPath + @"\st", "*.*", SearchOption.AllDirectories);
            if (this.saveToDb.Checked && (this.skinNameTextbox.Text == ""))
            {
                Cliver.Message.Inform("Please enter a skin name or uncheck the 'Save To Database' checkbox.");
            }
            else if (this.saveToDb.Checked && Directory.Exists(Application.StartupPath + @"\skins\" + this.skinNameTextbox.Text))
            {
                Cliver.Message.Inform("A skin by that name appears to already exists. Please choose a new name or delete the existing skin first.");
            }
            else if (strArray.Length == 0)
            {
                Cliver.Message.Inform("You don't have any files selected!\nClick 'Add Files' or 'Add Directory' and select the files you wish to install.");
            }
            else
            {
                foreach (string str in strArray)
                {
                    string[] strArray2 = str.Split(new char[] { '\\' });
                    string str2 = string.Empty;
                    string[] strArray3 = str.Split(new char[] { '\\' });
                    bool flag = false;
                    for (int i = 1; i < (strArray3.Length - 1); i++)
                    {
                        if (flag)
                        {
                            str2 = str2 + strArray3[i] + @"\";
                        }
                        else if (strArray3[i - 1] == "st")
                        {
                            str2 = strArray3[i] + @"\";
                            flag = true;
                        }
                    }
                    string str3 = this.FileNameToLocation(strArray2[strArray2.Length - 1], false);
                    if (str2 != null)
                    {
                        num++;
                        if (this.saveToDb.Checked)
                        {
                            this.SIFileOp.FileMove(str, Application.StartupPath + @"\skins\" + this.skinNameTextbox.Text + @"\" + str2 + str3);
                        }
                        this.SIFileOp.FileMove(str, gameDirectory + str2 + str3);
                    }
                    else
                    {
                        Cliver.Message.Inform("File: " + strArray2[strArray2.Length - 1] + "\n This file could not be identified and was not installed.");
                    }
                }
                Cliver.Message.Inform(string.Concat(new object[] { "Installation Complete.\nInstalled ", num, " of ", strArray.Length, " files." }));
                if (this.saveToDb.Checked)
                {
                    this.ExecuteQuery("INSERT INTO skins (sName, sInstalled, sFileCount, author) VALUES ('" + this.skinNameTextbox.Text + "', 1, '" + num.ToString() + "', '"+this.textBoxauthor.Text+"')");
                }
                this.UpdateListView();
            }*/
        }
        private void b_IRemoveFiles_Click(object sender, EventArgs e)
        {
            if (this.installFiles_ListBox.SelectedItem != null)
            {
                string[] strArray = this.installFiles_ListBox.SelectedItem.ToString().Split(new char[] { '\\' });
                string path = string.Empty;
                if (strArray.Length > 1)
                {
                    path = Application.StartupPath + @"\st\" + this.installFiles_ListBox.SelectedItem.ToString().Remove(this.installFiles_ListBox.SelectedItem.ToString().Length - (strArray[strArray.Length - 1].Length + 1));
                }
                else
                {
                    path = Application.StartupPath + @"\st\" + this.installFiles_ListBox.SelectedItem.ToString();
                }
                if (File.Exists(Application.StartupPath + @"\st\" + this.installFiles_ListBox.SelectedItem.ToString()))
                {
                    this.SIFileOp.FileDelete(Application.StartupPath + @"\st\" + this.installFiles_ListBox.SelectedItem.ToString());
                }
                if (Directory.Exists(path) && (Directory.GetFiles(path).Length == 0))
                {
                    this.SIFileOp.DirectoryDelete(path, true);
                }
                this.installFiles_ListBox.Items.Remove(this.installFiles_ListBox.SelectedItem);
            }
        }
        private void inputBox_Validating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length == 0)
            {
                e.Cancel = true;
                e.Message = "Required";
            }
        }
        private void addToDatabase_click(object sender, EventArgs e)
        {
            if (addFilesWorker.IsBusy)
            {
                Cliver.Message.Inform("Please wait for the current files to complete\nbeing added to the list before add it to the database");
                return;
            }
            int num = 0;
            bool updateing = false;
            if (!Directory.Exists(Application.StartupPath + @"\st"))
            {
                Directory.CreateDirectory(Application.StartupPath + @"\st");
            }
            string[] filesInsideSTDir = Directory.GetFiles(Application.StartupPath + @"\st", "*.*", SearchOption.AllDirectories);
            
            //this.skinNameTextbox.Text = this.skinNameTextbox.Text.Replace("'", "-");
            //sanitize!
            // remeber to snitize search query on skin selection on instal and uninstall button
            if (filesInsideSTDir.Length == 0)
            {
                Cliver.Message.Inform("You don't have any files selected!\nClick 'Add Files' or 'Add Directory' and select the files you wish to install.");
                return;
            }
            else
            if (this.skinNameTextbox.Text == "")
            {
                //Cliver.Message.Inform("Please enter a skin name.");
                //Cliver.Message.
                //string input = Microsoft.VisualBasic.Interaction.InputBox("Prompt", "Title", "Default", 0, 0);
                FileInfo fi = new FileInfo(filesInsideSTDir[0]);
                
                InputBoxResult result = InputBox.Show("Please enter in a name for this skin", "Please Choose a Name", fi.Name, new InputBoxValidatingHandler(inputBox_Validating));
                if (result.OK)
                {
                    this.skinNameTextbox.Text = result.Text;
                }
                else
                {
                    return;
                }

            }
            this.skinNameTextbox.Text =
                this.skinNameTextbox.Text.Replace("\\", "-").Replace("/", "-").Replace(":", "-").Replace("*", "-").Replace("\"", "-").Replace("|", "-").Replace(">", "-").Replace("<", "-").Replace("?", "-")
               ;
            this.textBoxauthor.Text =
                this.textBoxauthor.Text.Replace("\\", "-").Replace("/", "-").Replace(":", "-").Replace("*", "-").Replace("\"", "-").Replace("|", "-").Replace(">", "-").Replace("<", "-").Replace("?", "-")
               ;

               
            if (Directory.Exists(Application.StartupPath + @"\skins\" + this.skinNameTextbox.Text))
            {
                
                
                string qurr = "SELECT * FROM skins WHERE sName=\"" +
                        this.skinNameTextbox.Text +
                        "\"";
                debugadd(qurr);
                SQLiteDataReader reader = new SQLiteCommand(qurr, this.sqLiteCon).ExecuteReader();
                if (reader.HasRows)
                {
                    //Cliver.Message.Inform("A skin by that name appears to already exists. Please choose a new name or delete the existing skin first.");
                    //this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\skins\" + this.skinNameTextbox.Text, true);
                    int replace = Properties.Settings.Default.optionReplaceSkinWarning;
                    if (replace == -1)
                    {
                        bool saveThis = false;
                        replace = Cliver.Message.Show("Replace Skin?",
                            SystemIcons.Information,out saveThis,
                            "A skin with this name is already installed!\r\n" +
                            "You should type in a new name, then press the \"Add to Database\" Button.\r\n" +
                            "\r\nIf you named it this way intentionally, and wish to update a skin\r\n" +
                            "You may do so, but it is important to uninstall the old skin first\r\n" +
                            "if your new skin has any less files than the one it is replaceing",
                            0, new string[2] { "Stop and Let Me Choose a New Name", "I am updating a skin, and am ready to replace it." });
                        if (saveThis) Properties.Settings.Default.optionReplaceSkinWarning=replace;
                        Properties.Settings.Default.Save();
                    }
                    if (replace == 1)
                    {
                        this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\skins\" + this.skinNameTextbox.Text, true);
                        updateing = true;
                    }
                    else
                    {
                        return;
                    }
                    //Cliver.Message.Inform("skin already existrs in databse cuz " + reader.HasRows.ToString());
                }
                else
                {
                    //Cliver.Message.Inform("SKIN not in databse but folder exists");
                    int repFold = Properties.Settings.Default.optionReplaceFolderWarning;
                    if (repFold == -1)
                    {
                        bool saveThis = false;
                        repFold = Cliver.Message.Show("Replace Folder", 
                            SystemIcons.Information,out saveThis,
                            "A folder with this name already exists\r\n"+
                            "But the skin inside is not yet in the database...\r\n"+
                            "\r\nWould you like to update this folder and replace its contents?",
                            0, new string[2] { "Yes", "No" });
                        if (saveThis) Properties.Settings.Default.optionReplaceFolderWarning = repFold;
                        Properties.Settings.Default.Save();
                    }
                    if(repFold== 1)
                    {
                        return;
                    }
                    else
                    {
                        //clear that folder and contiunue
                        this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\skins\" + this.skinNameTextbox.Text, true);

                    }
                }


            }

            if(true)
            {
                foreach (string fileInsideSTDir in filesInsideSTDir)
                {
                    string[] borkenPathInSTDir = fileInsideSTDir.Split(new char[] { '\\' });
                    string folderString = string.Empty;
                    //string[] strArray3 = fileInsideSTDir.Split(new char[] { '\\' });
                    bool flag = false;
                    for (int i = 1; i < (borkenPathInSTDir.Length - 1); i++)
                    {
                        if (flag)
                        {
                            folderString = folderString + borkenPathInSTDir[i] + @"\";
                        }
                        else if (borkenPathInSTDir[i - 1] == "st")
                        {
                            folderString = borkenPathInSTDir[i] + @"\";
                            flag = true;
                        }
                    }
                    string filename = borkenPathInSTDir[borkenPathInSTDir.Length - 1];
                    if (folderString != null)
                    {
                        num++;
                        this.SIFileOp.FileMove(fileInsideSTDir,
                            Application.StartupPath + @"\skins\" + this.skinNameTextbox.Text + @"\" 
                            + folderString + filename);
                    }
                    else
                    {
                        Cliver.Message.Inform("File: " + borkenPathInSTDir[borkenPathInSTDir.Length - 1] + "\n This file could not be identified and was not installed.");
                    }
                }

                String skinDateAdded = DateTime.Now.ToString(dateFormat);
                String skinDateInstalled = "-";
                string qurry = "INSERT INTO skins (sName, sInstalled, sFileCount, author, dateadded, dateinstalled)"+
                    " VALUES (\"" + this.skinNameTextbox.Text + "\", 0, '" + num.ToString() +
                    "', \"" + this.textBoxauthor.Text + "\", \""+skinDateAdded+"\", \""+skinDateInstalled+
                    "\")";
                if (updateing)
                {
                    qurry = "UPDATE skins SET sFileCount=\""+num.ToString()+"\", dateadded=\""+skinDateAdded+"\" WHERE sName=\"" +
                        this.skinNameTextbox.Text +
                        "\"";
                }
                debugadd(qurry);
                this.ExecuteQuery(qurry);
                //Cliver.Message.Inform("Added " + this.skinNameTextbox.Text + " to the Skin Database!\nGo to the other tab to install it!");
                this.UpdateListView();
            

                //this.tabControl1.SelectTab(1);
                int install = Properties.Settings.Default.optionInstallAsWellOption;
                if (install == -1)
                {
                    bool saveThis = false;
                    Cliver.Message.NextTime_ButtonColors = new Color[] {Color.LightGreen,Color.LightGreen };
                    install = Cliver.Message.Show("Done Adding " + this.skinNameTextbox.Text,
                            SystemIcons.Information, out saveThis,
                         "Added " + this.skinNameTextbox.Text + " to the Skin Database!\nYou can go to the other tab to install it!",
                            0, new string[2] { "OK", "Please automatically install it at this time." });
                    if (saveThis) Properties.Settings.Default.optionInstallAsWellOption = install;
                    Properties.Settings.Default.Save();
                }
                if(install ==1)
                {
                    //Cliver.Message.Inform("not implemented");
                    if (fileListWorker1.IsBusy)
                    {
                        Cliver.Message.Inform("Oh.. actually..Please wait a moment for this program to finish updating\r\nThe Progress Bar below will show you the status of this");
                        this.tabControl1.SelectedIndex = 1;
                        return;
                    }
                    if (Properties.Settings.Default.showEveryTime)
                    {
                        PreferencesForm form = new PreferencesForm();
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.ShowDialog();
                    }
                    debugadd("running worker");
                    List<ListViewItem> args = new List<ListViewItem>();
                    ListViewItem item = new ListViewItem(new string []{"",this.skinNameTextbox.Text,"","","No"});
                    
                    args.Add(item);
                    installWorker2.RunWorkerAsync(args);

                    this.dbInstall.Enabled = this.dbUninstall.Enabled = this.dbDelete.Enabled = this.UpdateFL.Enabled = false;
                }
            }
            clearFiles(false);
                
            this.tabControl1.SelectedIndex = 1;
        }
        #region fileNameVerification
        private fileLocReturn FileNameToLocation(
            string foldernamewFileName, bool firstPass,
            List<string> moreOptions)
        {
            FileInfo fi = new FileInfo(foldernamewFileName);
            string fileName = fi.Name;
            if (fileName.ToLower().Contains("thumbs.db"))
            {
                if (Properties.Settings.Default.showAllWarnings && !firstPass)
                    Cliver.Message.Inform("thumbs.db files are silly, not going to use\r\n" + fileName);

                return new fileLocReturn(moreOptions);
            
            }
            if (fileName.ToLower().Contains(".inibin") && !firstPass)
            {
                if (Properties.Settings.Default.showAllWarnings)
                    Cliver.Message.Inform("Inibin files are known to cause issues\r\nNot going to use\r\n" + fileName);
                return new fileLocReturn(moreOptions);
            }
            if (fileName.ToLower().EndsWith(".wav") || fileName.ToLower().EndsWith(".mp3"))
            {
                return new fileLocReturn( "formatedsounds\\\\", fileName );
            }
            if (fileName.ToLower().Contains("siuinfo.txt"))
            {

                debugadd("trying to read " + foldernamewFileName);
                string[] info = readInfoFile(foldernamewFileName);
                try
                {
                    if (info[0] != "")
                    {
                        MethodInvoker action = delegate
                        { this.skinNameTextbox.Text = info[0]; };
                        this.skinNameTextbox.BeginInvoke(action);
                    }
                    if (info[1] != "")
                    {
                        MethodInvoker action = delegate
                        { this.textBoxauthor.Text = info[1]; };
                        this.textBoxauthor.BeginInvoke(action);
                    }
                }
                catch (System.Exception ex)
                {
                	
                }
                
                return new fileLocReturn("\\\\skinInfo\\\\",fileName);
            }
            if (fileName.ToLower().Contains("customtext.txt"))
            {
                return new fileLocReturn("\\\\textmods\\\\", fileName);
            }
            if (fileName.ToLower().Contains("fontconfig_en_us"))
            {
                // this is guna break stuff, dont do it!
                return new fileLocReturn(moreOptions);
            }
            if (fileName.ToLower().Contains("siupreview"))
            {
                return new fileLocReturn( "\\\\skinInfo\\\\", fileName);
            }
            if (fileName.ToLower().Contains("animations.list") || fileName.ToLower().Contains("animations.ini"))
            {
                // this is guna break stuff, dont do it!
                return new fileLocReturn(moreOptions);
            }
            
            //Cliver.Message.Inform("filename is " + fileName + " and bool is " + trueForDirFalseForFile.ToString());
            List<string> options = new List<string>();
            string directoryWODirPath = foldernamewFileName;
            directoryWODirPath = directoryWODirPath.Replace(directoryPath, "").Replace(Application.StartupPath,"");

            //if (foldernamewFileName.Split(new char[] { '\\' }).Length > 1)
            //{
            //    fileName = fileName.Split(new char[] { '\\' })[fileName.Split(new char[] { '\\' }).Length - 1];
            //}
            //for (int i = 0; i < allFilesCt; i++)
            foreach (KeyValuePair<String, String> pairFileName_Path in allFilesList)
            {
                //if (fileName.ToLower() == allFiles[i, 0].ToLower())
                if (fileName.ToLower() == pairFileName_Path.Key.ToLower())
                {
                    //if (!trueForDirFalseForFile)
                    //{
                        //return allFiles[i, 0];
                    //    return pairFileName_Path.Key;
                    //}
                    //string[] strArray = allFiles[i, 1].Split(new char[] { '\\' });
                    string[] strArray = pairFileName_Path.Value.Split(new char[] { '\\' });
                    string optionalPath = string.Empty;
                    for (int k = 0; k < (strArray.Length - 1); k++)
                    {
                        optionalPath = optionalPath + strArray[k] + @"\";
                    }
                    if (optionalPath != "")
                    {
                        options.Add(optionalPath);
                    }
                }
            }
            if (options.Count == 1)
            {
                //foreach (string path in options)
                    //only 1 option! wooo
                    return new fileLocReturn(options[0], fileName );
            }
            if (options.Count <= 1)
            {
                return new fileLocReturn(moreOptions);
            }
            //filePossibiles = new string[10000];
            /*for (int j = 0; (j < this.filePossibiles.Length) && (this.filePossibiles[j] != null); j++)
            {
                this.filePossibiles[j] = null;
            }*/
            //int index = 0;
            //foreach (string str3 in strArray2)
            //{
            //    this.filePossibiles[index] = str3;
            //    index++;
            //}
            this.GlobalFileName = fileName;
            //str.Replace(
            /*string[] strArray3 = directoryWODirPath.Split(new char[] { '\\' });
            string str4 = string.Empty;
            if (strArray3.Length >= 4)
            {
                for (int m = 0; m < strArray3.Length; m++)
                {
                    if (((m + 4) >= strArray3.Length) && (m != (strArray3.Length - 1)))
                    {
                        str4 = str4 + @"\\" + strArray3[m];
                    }
                }
            }
            else
            {
                str4 = directoryWODirPath;
            }*/

            
            //Cliver.Message.Inform("FileNmae is " + FileName + " filename is " + fileName);


            List<String> betterOptions = new List<string>();
            List<String> badOptions = new List<string>();
            foreach (string aoption in options)
            {
                //siparept.FileName;
                //Cliver.Message.Inform("looking for " + str4.ToLower() + " in " + str3.ToLower());
                string option = "\\\\" + aoption.ToLower();
                option = option.Replace(@"\\\\", @"\\");
                String test2 = directoryWODirPath.ToLower().Replace(@"\\\\", @"\\").Replace("\\\\game", "") + "\\\\";
                if (option.Contains(test2))
                {
                    //Cliver.Message.Inform("Found at " + testS+" because it had \n"+str4);
                    //return str3;
                    betterOptions.Add(aoption);
                }
                else
                {
                    badOptions.Add(aoption);
                    //really last try.. 
                }

            }
            if (betterOptions.Count == 0) betterOptions = badOptions;//no good ones :<

            if (betterOptions.Count != 1)
            {
                List<String> newOptions = pickFileNameHarder(new List<String>(options), directoryWODirPath);
                if (newOptions.Count != 0)
                    betterOptions = newOptions;
            }
            if (betterOptions.Count != 1)
            {
                List<String> newOptions = pickFileNameFixRafVersion(new List<String>(options), directoryWODirPath);
                if (newOptions.Count != 0)
                    betterOptions = newOptions;
            }
            if (betterOptions.Count != 1)
            {
                List<String> newOptions = pickFileNameHarder2(new List<String>(options), directoryWODirPath);
                if (newOptions.Count != 0)
                    betterOptions = newOptions;
            }

            if (betterOptions.Count != 1)
            {
                List<String> newOptions = pickFileNameFixRafVersion(new List<String>(options), directoryWODirPath);
                if (newOptions.Count != 0)
                    betterOptions = newOptions;
            }

            if (betterOptions.Count != 1)
            {
                List<String> newOptions = pickFileNameFixRafVersion(betterOptions, directoryWODirPath);
                if (newOptions.Count != 0)
                    betterOptions = newOptions;
            }

            if (betterOptions.Count == 1) return new fileLocReturn( betterOptions[0],fileName);

            //didnt get it , add file name for later use
            List<string> temp = new List<string>();
            foreach (string opt in betterOptions)
            {
                temp.Add(opt  + fileName);
            }
            betterOptions = temp;
            //didnt get it, combine options to ask with

            if (moreOptions != null)
                foreach (string option in moreOptions)
                {
                    if (!betterOptions.Contains(option))
                        betterOptions.Add(option);
                }

            //if (firstPass)
                return new fileLocReturn(null,null,false,betterOptions);//dontask user on first pass save options tho
            //int howMany = betterOptions.Count;
            //debugadd("have " + howMany + " options");
            
            //Cliver.Message.Inform("cant find " + str4.ToLower());

            //dont show a location if its a animations. file
            //this is because omse installs have animations.list
            //others have animations.ini
            //if (GlobalFileName.ToLower().Contains("animations."))
            //{
                //ignore it and dont show option dialog
            //    return new fileLocReturn(moreOptions);
            //}
            
            
        }
        public List<String> pickFileNameHarder(List<String> betterOptions, string test)
        {
            //debugadd("picking better file nname again..");
            List<String> newOptions = new List<string>();
            foreach (string str3 in betterOptions)
            {
                //lgg last ditch try and guess if one of straray2 is correct
                //siparept.FileName;
                //Cliver.Message.Inform("looking for " + str4.ToLower() + " in " + str3.ToLower());
                string option = "\\\\" + str3.ToLower();
                option = option.Replace(@"\\\\", @"\\");
                String test2 = test.ToLower().Replace(@"\\\\", @"\\").Replace("\\\\game", "") + "\\\\";
                option = removeRafName(option, false);
                test2 = removeRafName(test2, false);

                if (option.Contains(test2))
                {
                    //Cliver.Message.Inform("Found at " + testS+" because it had \n"+str4);
                    //return str3;
                    newOptions.Add(str3);
                }
                else
                {
                    //really last try.. 
                }
            }


            return newOptions;
            //if (betterOptions.Count > 1) pickFileNameHarder(betterOptions, str4);
        }
        public List<String> pickFileNameHarder2(List<String> betterOptions, string test)
        {
            //debugadd("picking better file nname again2..");
            List<String> newOptions = new List<string>();
            double highestMatch = 0.0f;

            foreach (string str3 in betterOptions)
            {
                //siparept.FileName;
                //Cliver.Message.Inform("looking for " + str4.ToLower() + " in " + str3.ToLower());
                string option = "\\\\" + str3.ToLower();
                option = option.ToLower().Replace(@"\\\\", @"\").Replace(@"\\", @"\");
                String test2 = test.ToLower().Replace(@"\\\\", @"\").Replace(@"\\", @"\").Replace("\\game", "") + "\\";
                option = removeRafName(option, false);
                test2 = removeRafName(test2, false);
                List<string> pathOptionFolderNames =
                    new List<string>(option.Split('\\'));

                List<string> testFolderNames =
                    new List<string>(test2.Split('\\'));
                double matches = 0.0f;
                foreach (string pathF in pathOptionFolderNames)
                {
                    if (pathF == "") continue;
                    int worthBasedOnEndOfPathNameBeingWorthMore = 1;
                    foreach (string testF in testFolderNames)
                    {
                        worthBasedOnEndOfPathNameBeingWorthMore++;
                        if (testF == "") continue;                    
                        if (testF == pathF)
                        {
                            if (testF != "")
                            {
                                matches+= 1 + ( .001 *  worthBasedOnEndOfPathNameBeingWorthMore);
                            }
                        }
                        else
                        {
                            matches -= (.001 * worthBasedOnEndOfPathNameBeingWorthMore);
                        }
                    }
                }
                if (matches == highestMatch)
                {
                    //tie
                    newOptions.Add(str3);

                }
                else if (matches > highestMatch)
                {

                    highestMatch = matches;
                    newOptions.Clear();
                    newOptions.Add(str3);
                }
            }


            return newOptions;
            //if (betterOptions.Count > 1) pickFileNameHarder(betterOptions, str4);
        }
        public List<String> pickFileNameFixRafVersion(List<String> betterOptions, string test)
        {
            //debugadd("picking better file nname again2..");
            List<String> newOptions = new List<string>();
            String nameWithOutRaf = "";

            foreach (string str3 in betterOptions)
            {
                String tempWORaf = str3;
                if (str3.Contains(".raf\\"))
                    tempWORaf = removeRafName(tempWORaf, false);
                tempWORaf = removeVersionFolder(tempWORaf);

                if (nameWithOutRaf == "")
                {
                    nameWithOutRaf = tempWORaf;
                }
                else
                {
                    if (nameWithOutRaf != tempWORaf)
                        return newOptions;//we got nothing, they MUST match
                }
            }
            int bestScore = 0;
            foreach (string str3 in betterOptions)
            {

                /*int rafSpot = str3.IndexOf(".raf");
                string firstPart = str3.Substring(0, rafSpot);
                //chop off raf name
                firstPart = firstPart.Substring(0, firstPart.LastIndexOf("\\\\"));
                //grab the name only
                string versionString = firstPart.Substring(firstPart.LastIndexOf("\\\\") + 2);*/

                Regex r1 = new Regex(@"\\(\d+\.\d+\.\d+\.\d+)\\");
                Match match = r1.Match(str3);
                string versionString = "";
                if (match.Success)
                {
                    versionString = match.Groups[1].Value;

                }
                else
                {
                    
                }
                debugadd(versionString);
                string[] versions = versionString.Split('.');
                int total = 0;
                int multiplier = 1;
                for (int i = versions.Length - 1; i >= 0; i--)
                {
                    int vA = int.Parse(versions[i].Trim());
                    total += (vA * multiplier);
                    multiplier += 500;
                }
                if (total == bestScore)
                {
                    //tie
                    newOptions.Add(str3);

                }
                else if (total > bestScore)
                {
                    newOptions.Clear();
                    newOptions.Add(str3);
                    bestScore = total;
                }

            }


            return newOptions;
            //if (betterOptions.Count > 1) pickFileNameHarder(betterOptions, str4);
        }
        public string removeRafName(string path, bool andVersion)
        {
            // debugadd("remove raf from " + path);
            if (path.Contains(".raf\\"))
            {
                int end = path.IndexOf(".raf\\") + 5;
                string temp = path.Substring(0, end - 5);
                int start = temp.LastIndexOf("\\") + 1;
                if ((start != -1) && andVersion)
                {
                    temp = temp.Substring(0, start);
                    start = temp.LastIndexOf("\\") + 1;
                }
                if ((end != -1) && (start != -1))
                {
                    path = path.Remove(start, end - start);
                }
            }
            //debugadd("result is " + path);
            return path;
        }
        public string getAfterRafName(string path)
        {
            // debugadd("remove raf from " + path);
            if (path.Contains(".raf\\"))
            {
                int start = path.IndexOf(".raf\\") + 5;
                path = path.Substring(start);
            }
            //debugadd("result is " + path);
            return path;
        }
        public string removeVersionFolder(string path)
        {
            string toReturn="";
            toReturn = Regex.Replace(path, @"\\\d+\.\d+\.\d+\.\d+\\", @"\");
            return toReturn;
        }
        #endregion
        #endregion       
        private void CreateEasyInstallFile(string zipFileName)
        {
            TextWriter writer = new StreamWriter((Application.StartupPath + @"\skins\" + zipFileName + @"\") + @"\Read Me.txt");
            writer.WriteLine("This file pack was generated using LoL Skin Installer");
            writer.WriteLine("Origonally Available at: http://forum.leaguecraft.com/index.php?/topic/31502-release-skin-installer-for-new-launcher-v30/ \n\n");
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine("To manually install this file pack, extract the files to the following locations:\n\n");
            foreach (string str2 in Directory.GetFiles(Application.StartupPath + @"\skins\" + zipFileName, "*.*", SearchOption.AllDirectories))
            {
                string[] strArray2 = str2.Split(new char[] { '\\' });
                string str3 = string.Empty;
                bool flag = false;
                for (int i = 1; i < (strArray2.Length - 1); i++)
                {
                    if ((!flag && (strArray2[i - 1] == "skins")) && (strArray2[i] == zipFileName))
                    {
                        flag = true;
                    }
                    else if (flag)
                    {
                        str3 = str3 + strArray2[i] + @"\";
                    }
                }
                writer.WriteLine(str3 + strArray2[strArray2.Length - 1]);
            }
            writer.Flush();
            writer.Close();
            writer.Dispose();
        }
        private void backupSounds()
        {
            //we need to make sure we get a backup of this thing asap
            string full = findSoundsFSBLocation().ToLower();
            int lastIndex = full.LastIndexOf("\\");
            if (lastIndex != -1)
            {
                string name = full.Substring(lastIndex);
                string path = full.Replace(name, "");
                //backupFile(findSoundsFSBLocation().ToLower().Replace("vobank_en_us.fsb", ""), "VOBank_en_US.fsb");
                backupFile(path, name);
            }
        }
        private void restoreSounds()
        {
            this.statusText.Text="restoring sounds";
            //this.Refresh();
            String restoreFile = Application.StartupPath + @"\backup"
                + findSoundsFSBLocation();
            if (File.Exists(restoreFile))
            {
                this.SIFileOp.FileCopy(restoreFile, gameDirectory + findSoundsFSBLocation());
            }
        }
        private void runthis(string fn,string args,string wd,bool vis)
        {
           // vis = true;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fn;
            startInfo.WorkingDirectory = wd;
            startInfo.Arguments = args;
            startInfo.CreateNoWindow=!vis;
            startInfo.WindowStyle = vis ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
                try
                {
                    using (Process process = Process.Start(startInfo))
                    {
                        process.WaitForExit();
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
        }
        private void replaceSounds(String path)
        {
            //rads\projects\lol_game_client_en_us\managedfiles\0.0.0.13\data\sounds\fmod\vobank_en_us.fsb
            string soundlocname = findSoundsFSBLocation();
            //Cliver.Message.Inform(path);
            path += @"\formatedsounds";
            backupSounds();
            installWorker2.ReportProgress(10, "Installing...Building rebuild file..extracting current sounds...");        

            String backupDir = Application.StartupPath+@"\backup\";
            String fsbDir = Application.StartupPath+@"\fsb\";
                    
	        String rebuildFile = fsbDir+"rebuild.dat";
            String soundsFolder = fsbDir+"sounds";
            String tempFSB = fsbDir + "temp.fsb";

            installWorker2.ReportProgress(30, "Installing...Clearing old sound folder...");
            if(File.Exists(rebuildFile))
            {
                File.Delete(rebuildFile);
            }
            if (File.Exists(tempFSB)) File.Delete(tempFSB);
            this.SIFileOp.FileCopy(gameDirectory + soundlocname, tempFSB);
            if (Directory.Exists(soundsFolder)) SIFileOp.DirectoryDelete(soundsFolder, true);
            Directory.CreateDirectory(soundsFolder);

            installWorker2.ReportProgress(35, "Installing ... Unpacking soundbank...(This takes a while, be patient");
            
            runthis(fsbDir + "ext.bat", "",fsbDir, false);

            File.Delete(tempFSB);
            //runthis(fsbDir + "fsbext.exe", "-s -d sounds " + "\"" + gameDirectory +       soundlocname + "\"", fsbDir,false);
            //RunWait(@ScriptDir & '\fsbext\fsbext -R -d sounds "' & $SLOLDIR & '\Game\DATA\Sounds\FMOD\VOBank_en_US.fsb" ', @ScriptDir & "\fsbext", @SW_HIDE)

            installWorker2.ReportProgress(65, "Installing .... Replacing modded soundfiles...");
            
            foreach (string str in Directory.GetFiles(path))
            {
                FileInfo fi = new FileInfo(str);
                string fileNameWithExtension = fi.Name;
                int extLoc = fileNameWithExtension.LastIndexOf(".");
                string fileNameNoExt = fileNameWithExtension.Substring(0, extLoc);
                int underscore = fileNameNoExt.LastIndexOf("_");
                //mkae sure its a number after..
                int num = 0;
                int.TryParse(fileNameNoExt.Substring(underscore+1),out num);
                if (num < 1) underscore = -1;
                string beforeUnder = fileNameNoExt;
                string afterUnder = "";
                if (underscore > -1)
                {
                    beforeUnder = fileNameNoExt.Substring(0, underscore);
                    afterUnder = fileNameNoExt.Substring(underscore);
                    //Cliver.Message.Inform("before under is " + beforeUnder.ToString() + "\nafter is " + afterUnder.ToString());
                }
                int length = beforeUnder.Length;
                if (length > 29) beforeUnder = beforeUnder.Substring(0, 29);

                string name = beforeUnder + afterUnder+".mp3";
                //string name = str.Substring(str.LastIndexOf("\\"));
                String target = fsbDir + "sounds\\" + name;
                //Cliver.Message.Inform(target);
               this.SIFileOp.FileCopy(str, target);
            }
            //For $A = 1 To $ASEL[0]
		    //    FileCopy(@ScriptDir & "\Mods\" & $ASEL[$A] & "\*", @ScriptDir & "\fsbext\sounds\", 1)
	        //Next

            //this.statusText.Text = "Installing ... Repacking soundbank...";
            installWorker2.ReportProgress(80, "Installing ... Repacking soundbank...");

            runthis(fsbDir + "reb.bat", "", tempFSB, false);            
            //runthis("fsbext.exe", "-s rebuild.dat -d sounds -r rebuiltSounds.fsb", fsbDir,false);
            //RunWait(@ScriptDir & "\fsbext\fsbext -s rebuild.dat -d sounds -r VOBank_en_US.fsb", @ScriptDir & "\fsbext", @SW_HIDE)

            installWorker2.ReportProgress(80, "Installing ... Copying soundbank...");


            this.SIFileOp.FileCopy(fsbDir + "rebuiltSounds.fsb", gameDirectory + soundlocname);
            //FileCopy(@ScriptDir & "\fsbext\VOBank_en_US.fsb", $SLOLDIR & "\Game\DATA\Sounds\FMOD\VOBank_en_US.fsb", 1)

            File.Delete(fsbDir + "rebuiltSounds.fsb");
            if(Directory.Exists(soundsFolder))SIFileOp.DirectoryDelete(soundsFolder, true);
            File.Delete(rebuildFile);
            

        }
        private void backupFile(String path, String name)
        {
            
            if (File.Exists(gameDirectory + path + name))
            {
            
                String tn = path+name;
                
                //we might need to back this up!
                //check if its in char or particles or one of the zip..
                //edit this is only if this type of install has a zip!
                //edit removed for now, we need another way to determine if this is a .zip legacy lol
                /*if(int.Parse(allFilesExtensions[0])!=0)
                {
                    if(
                       path.ToLower().Contains("data\\characters")
                       || path.ToLower().Contains("data\\buffs")
                       || path.ToLower().Contains("data\\buildingblocks")
                       || path.ToLower().Contains("data\\editor")
                       || path.ToLower().Contains("data\\globals")
                       || path.ToLower().Contains("data\\images")
                       || path.ToLower().Contains("data\\items")
                       || path.ToLower().Contains("data\\levels")
                       || path.ToLower().Contains("data\\loadingscreen")
                       || path.ToLower().Contains("data\\particles")
                       || path.ToLower().Contains("data\\scripts")
                       || path.ToLower().Contains("data\\spells")
                       || path.ToLower().Contains("data\\talents")
                       )
                    {
                        //ignore
                        return;
                    }
                }*/
                String backupDir = Application.StartupPath+@"\backup\";
                if(!Directory.Exists(backupDir))Directory.CreateDirectory(backupDir);
                // We need to do some crazy path changing for air files
                // currently air files are automatically moved to a higher version number
                // C:\Riot Games\League of Legends\RADS\projects\lol_air_client\releases\0.0.0.95
                // This makes old backups not work.
                // We need to remove the version dependency
                // We also want this to work with older SIU backups...
                // For now, just make sure the file doesn't exist in ANY version air backup
                // To do: also make sure we can restore this correctly...
                int airIndex = path.ToLower().IndexOf("\\league_client\\releases\\");
                if (airIndex>0)
                {
                    //oh boy it is a air file, 
                    //search backups dir for other air backups so we don't replace one
                    DirectoryInfo airbackupDir =  new DirectoryInfo(backupDir + path.Substring(0, airIndex + 25));
                    string remainingPath = path.Substring(path.Substring(airIndex + 25).IndexOf("\\")+airIndex+25);
                    debugadd("air file backup dir :"+airbackupDir.FullName);
                    if(airbackupDir.Exists)
                    foreach (DirectoryInfo dir in airbackupDir.GetDirectories())
                    {
                        string toCheck = airbackupDir.FullName + dir + remainingPath + name;
                        if (File.Exists(toCheck)) return;//we already have this in a version non-specific folder
                    }

                }

                //make sure we don't already have a backup there.. don't want to overwrite                    
                if (!File.Exists(backupDir + tn))
                {
                    this.SIFileOp.FileMove(gameDirectory + tn, backupDir + tn);
                }
            }
        }
        #region installing
        private void setInstallButtons(bool state)
        {
            this.dbInstall.Enabled = this.dbUninstall.Enabled =
                this.dbDelete.Enabled = this.UpdateFL.Enabled =
                this.button3repath.Enabled=this.button3reinstallText.Enabled=state;
            this.buttoncancel.Visible = !state;
        }
        private void dbInstall_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count < 1)
            {
                Cliver.Message.Inform("Please click the check mark of the skin you want to install above\n"+
                    "\nIf you do not see any skins to install, please add them in the other tab\n"+
                "\"Add New Skin\"");
                return;
            }
            if (fileListWorker1.IsBusy)
            {
                Cliver.Message.Inform("Please wait a moment for this program to finish updating\r\nThe Progress Bar below will show you the status of this");
                return;
            }
            if (Properties.Settings.Default.showEveryTime)
            {
                PreferencesForm form = new PreferencesForm();
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog();
            }
            debugadd("running worker");
            List<ListViewItem> args = new List<ListViewItem>();
            foreach (ListViewItem item in this.listView1.CheckedItems)
            {
                args.Add(item);
            }
            installWorker2.RunWorkerAsync(args);
            setInstallButtons(false);
        }

        private void installWork(object sender, DoWorkEventArgs e)
        {
            debugadd("install process started");
            removeSoftRepairFile();
            

            List<ListViewItem> a = (List<ListViewItem>)e.Argument;
            debugadd("installing " + a.Count.ToString() + " skins");

            installWorker2.ReportProgress(2);
            int num = 0;
            int skipped = 0;
            bool airFlag = false;
            foreach (ListViewItem item in a)
            {
                debugadd("installing " + item.ToString());
                if ((item.SubItems[4].Text == "Yes") && (Cliver.Message.Show("Skin Already Installed", SystemIcons.Information, item.SubItems[1].Text + " is list as already installed. Do you want to reinstall it?", 0, new string[2] { "Yes", "No" }) == 1))
                {
                    return;
                }
                bool soundflag = false;
                String skinPath = Application.StartupPath + @"\skins\" + item.SubItems[1].Text;
                int fn = 0;
                string[] files = Directory.GetFiles(skinPath, "*.*", SearchOption.AllDirectories);
                List<installFileInfo> filteredFileInfo = new List<installFileInfo>(); 
                foreach (string skinFileName in files)
                {
                    int percent = (int)Math.Floor(((double)fn++ / (double)files.Length) * 10.0);
                    installWorker2.ReportProgress(percent, "Installing file " + fn.ToString() + " of " + files.Length.ToString() + " :: " + skinFileName);

                    string[] folderNames = skinFileName.Split(new char[] { '\\' });
                    string fixedFilePath = string.Empty;
                    bool flag = false;
                    for (int i = 1; i < (folderNames.Length - 1); i++)
                    {
                        if ((!flag && (folderNames[i - 1] == "skins")) && (folderNames[i] == item.SubItems[1].Text))
                        {
                            flag = true;
                        }
                        else if (flag)
                        {
                            fixedFilePath = fixedFilePath + folderNames[i] + @"\";
                        }
                    }
                    string fixedFileName = folderNames[folderNames.Length - 1];
                    if (fixedFilePath != null)
                    {
                        if (fixedFilePath.Contains("formatedsounds"))
                        {
                            if (Properties.Settings.Default.sounds)
                            {
                                soundflag = true;
                                num++;
                            }
                            else
                            {
                                skipped++;
                            }
                        }
                        else if (fixedFilePath.ToLower().Contains("skininfo"))
                        {
                            //ignore these
                        }
                        else if (fixedFilePath.ToLower().Contains("textmods"))
                        {
                            //
                            if (Properties.Settings.Default.text)
                            {
                                num++;
                                installTextMod(skinFileName);
                            }

                        }
                        else
                        {
                            //bool yesFlag = false;
                            bool noFlag = false;
                            if (!Properties.Settings.Default.chTx &
                                (
                                fixedFilePath.ToLower().Contains("data\\characters")

                                )
                                &
                                (
                                fixedFileName.ToLower().Contains(".dds")
                                ||
                                fixedFileName.ToLower().Contains(".tga")
                                )
                                &!fixedFileName.ToLower().Contains("load"))
                            {
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.spelliconsinst &
                                (
                                fixedFilePath.ToLower().Contains("spells\\icons2d")

                                )
                                &
                                (
                                fixedFileName.ToLower().Contains(".dds")
                                ||
                                fixedFileName.ToLower().Contains(".tga")

                                ))
                            {
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.spelliconsinst &
                                (
                                fixedFilePath.ToLower().Contains("data\\characters")
                                )
                                &
                                (
                                fixedFilePath.ToLower().Contains("\\info")
                                )
                                &
                                (
                                fixedFileName.ToLower().Contains(".dds")
                                ||
                                fixedFileName.ToLower().Contains(".tga")
                                )
                                & !fixedFileName.ToLower().Contains("circle")
                                & !fixedFileName.ToLower().Contains("square")
                                )
                            {
                                //data/characters/name/icons/stuff
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.iconcharinst &
                                (
                                fixedFilePath.ToLower().Contains("data\\characters")
                                )
                                &
                                (
                                fixedFilePath.ToLower().Contains("\\info")
                                )
                                &
                                (
                                fixedFileName.ToLower().Contains(".dds")
                                ||
                                fixedFileName.ToLower().Contains(".tga")
                                )
                                &
                                (fixedFileName.ToLower().Contains("circle") ||
                                fixedFileName.ToLower().Contains("square"))
                                )
                            {
                                //data/characters/name/icons/stuff with square
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.loadscreensinst &
                                (
                                fixedFilePath.ToLower().Contains("data\\characters")
                                )
                                &
                                (
                                fixedFileName.ToLower().Contains(".dds")
                                ||
                                fixedFileName.ToLower().Contains(".tga")
                                )
                                &
                                (fixedFileName.ToLower().Contains("load"))
                                )
                            {
                                //data/characters/name/icons/stuff with square
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.ch3d &
                                fixedFilePath.ToLower().Contains("data\\characters")
                                & fixedFileName.ToLower().Contains(".skn"))
                            {
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.anims &
                                fixedFilePath.ToLower().Contains("data\\characters")
                                &
                                (
                                fixedFileName.ToLower().Contains(".anm")
                                ||
                                fixedFileName.ToLower().Contains(".ini")
                                ||
                                fixedFileName.ToLower().Contains(".txt")
                                ||
                                fixedFileName.ToLower().Contains(".skl")

                                ))
                            {
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.part3d &
                                fixedFilePath.ToLower().Contains("data\\particles")
                                &
                                (
                                fixedFileName.ToLower().Contains(".sco")
                                ||
                                fixedFileName.ToLower().Contains(".scb")

                                ))
                            {
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.partTx &
                                fixedFilePath.ToLower().Contains("data\\particles")
                                &
                                (
                                fixedFileName.ToLower().Contains(".dds")
                                ||
                                fixedFileName.ToLower().Contains(".tga")

                                ))
                            {
                                noFlag = true;
                            }
                            if (!Properties.Settings.Default.air &
                               ( fixedFilePath.ToLower().Contains("air\\")||
                               fixedFilePath.ToLower().Contains("\\lol_air_client\\") || fixedFilePath.ToLower().Contains("air\\")))
                            {
                                noFlag = true;
                            }
                            if (fixedFilePath.ToLower().Contains("air\\") ||
                              fixedFilePath.ToLower().Contains("\\lol_air_client\\") || fixedFilePath.ToLower().Contains("air\\"))
                            {
                                airFlag = true;
                            }

                            if (!Properties.Settings.Default.text & fixedFilePath.ToLower().Contains("menu"))
                            {
                                noFlag = true;
                            }

                            if (noFlag == false)
                            {
                                filteredFileInfo.Add(new 
                                    installFileInfo(skinFileName,fixedFileName,fixedFilePath));
                                //filteredFiles.Add(fixedFilePath + fixedFileName);                            
                                //num++;
                            }
                            else
                            {
                                //Cliver.Message.Show(this.Icon, "SKIPPED moving " + str + " to " + gameDirectory + str2 + str3, 0, "OK");
                                debugadd("SKIPED " + skinFileName + " to " + gameDirectory + fixedFilePath + fixedFileName);
                                skipped++;
                            }
                        }
                    }
                    else
                    {
                        Cliver.Message.Inform("Cannot identify the file: " + fixedFileName);
                    }
                    
                }
                if (soundflag)
                {
                    backupSounds();

                    this.installWorker2.ReportProgress(5);
                    replaceSounds(skinPath);
                }
                chooseCharacterFiles(ref filteredFileInfo);
                fileCopyFiles(ref filteredFileInfo);
                num +=filteredFileInfo.Count;
                prettyDate pd = new prettyDate(DateTime.Now);
                //item.SubItems[4].Text = pd.ToString();
                this.ExecuteQuery("UPDATE skins SET sInstalled=1, dateinstalled=\"" + pd.getStringDate() + "\"" +
                    " WHERE sName=\"" +
                    item.SubItems[1].Text +
                    "\"");
            }
            if (false)//num == 0)
            {
                //Cliver.Message.Inform("No files were installed.");
            }
            else
            {
                this.installWorker2.ReportProgress(100, "Done!");
                string info = "Installed " + num.ToString() + " files.\r\n" +
               "Skipped " + skipped.ToString() + " files based on your settings.";
                if (airFlag)
                {
                    info += "\r\n\r\nYou have also installed an air client mod,\r\nThis will only take effect after a restart of LoL";
                }
                InfoForm form = new InfoForm(
               info,
               new Size(270, 150),
               "Install Complete");
                form.StartPosition = FormStartPosition.CenterParent;
                form.CustShowDialog(this);
                //form.ShowDialog();
                // Cliver.Message.Inform("Installation Complete.\nInstalled " + num + " files.");
                String extraRafFolder = Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_OPTIONS + "\\";
                if (Directory.Exists(extraRafFolder)) SIFileOp.DirectoryDelete(extraRafFolder, true);
            
            }

        }
        private void chooseCharacterFiles(ref List<installFileInfo> filteredFileInfo)
        {
            //remove stuff from last time, just in case
            String extraRafFolder = Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_OPTIONS + "\\";
            if (Directory.Exists(extraRafFolder)) SIFileOp.DirectoryDelete(extraRafFolder,true);
            Directory.CreateDirectory(extraRafFolder);

            if (Properties.Settings.Default.showCharSelection)
            {
                skinsOptions mySkinsOptions = new skinsOptions();
                List<string> fileNames = new List<String>();
                foreach (installFileInfo installInfo in filteredFileInfo)
                {
                    fileNames.Add(installInfo.getFileNamePath());
                }
                List<String> models = previewWindow.getModelsFromFileNames(fileNames);
                if (models.Count < 1) return;
                foreach (String modelName in models)
                {
                    skinOptions mySkinOptions = new skinOptions();
                    mySkinOptions.options = new List<skinOption>();
                    mySkinOptions.skinName = modelName;
                    mySkinOptions.parent = mySkinsOptions;
                    List<string> otherSkins = previewWindow.getOtherSkinNames(modelName);
                    foreach (String otherSkin in otherSkins)
                    {
                        skinOption myOption = new skinOption();
                        myOption.skinName = otherSkin;
                        myOption.skinSelected = myOption.origonalSelected = (otherSkin == modelName);
                        myOption.parent = mySkinOptions;
                        mySkinOptions.options.Add(myOption);
                    }
                    mySkinsOptions.Add(mySkinOptions);
                }
                CharacterSelectionForm form = new CharacterSelectionForm(mySkinsOptions);
                form.StartPosition = FormStartPosition.CenterParent;
                form.CustShowDialog(this);
                mySkinsOptions = form.mySkinsOptions;
                //now the fun part, seeing what files to add and remove and rename..


                List<installFileInfo> toRemove = new List<installFileInfo>();
                List<installFileInfo> newFileInfos = new List<installFileInfo>();
                        
                foreach (skinOptions mySkinOptions in mySkinsOptions)
                {
                    //first lets get the origonal skin that prompted this mess
                    skinOption origonalOption = null;
                    foreach (skinOption option in mySkinOptions.options)
                    {
                        if (option.origonalSelected) origonalOption = option;
                    }
                    if (origonalOption != null)
                    {
                        LOLViewer.IO.LOLModel origonalModel = previewWindow.reader.GetModel(origonalOption.skinName);

                        if (true)//!origonalOption.skinSelected)
                        {
                            //todo we need to remove the origonal files (we will add them again below if needed)
                            foreach (installFileInfo installInfo in filteredFileInfo)
                            {
                                if (
                                (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.skl.FileName.ToLower())) ||
                                (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.skn.FileName.ToLower())) ||
                                (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.texture.FileName.ToLower()))||
                                (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.champSplash.ToLower()))||
                                (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.champPic.ToLower()))
                                )
                                {
                                    debugadd("We might be removing " + installInfo.origonal + " from the install because it was not selected before");
                                    toRemove.Add(installInfo);
                                }else
                                if(origonalModel.loadScreen!=null)
                                    if (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.loadScreen.FileName.ToLower()))
                                    {
                                        debugadd("We might be removing " + installInfo.origonal + " from the install because it was not selected before");
                                        toRemove.Add(installInfo);
                                    }

                                
                            }
                        }

                        foreach (skinOption option in mySkinOptions.options)
                        {
                            if (option.skinSelected)
                            {
                                bool gotSKL; bool gotSKN; bool gotTXT; bool gotLOD; bool gotAirSplash; bool gotAirPic;
                                gotTXT = gotSKN = gotSKL = gotLOD= gotAirSplash = gotAirPic = false;
                                if (option.origonalSelected) gotTXT = gotSKN = gotSKL = gotLOD = gotAirSplash = gotAirPic = true;//srsly
                                //we gota skin we need to .. change
                                LOLViewer.IO.LOLModel targetModel = previewWindow.reader.GetModel(option.skinName);
                                foreach (installFileInfo installInfo in filteredFileInfo)
                                {
                                    FileInfo newLoc = null;
                                    if (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.champPic.ToLower()))
                                    {
                                        if (Properties.Settings.Default.air)
                                        {
                                            KeyValuePair<string, string> isfound = allFilesList.FirstOrDefault(m => m.Key.ToLower().Contains(targetModel.champPic.ToLower()));
                                            if (isfound.Key != null)
                                            {
                                                string whereFrom = gameDirectory + isfound.Value;
                                                newLoc = new FileInfo(whereFrom);
                                                gotAirPic = true;
                                            }
                                        }
                                    }
                                    if (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.champSplash.ToLower()))
                                    {
                                        if (Properties.Settings.Default.air)
                                        {
                                            KeyValuePair<string, string> isfound = allFilesList.FirstOrDefault(m => m.Key.ToLower().Contains(targetModel.champSplash.ToLower()));
                                            if (isfound.Key != null)
                                            {
                                                string whereFrom = gameDirectory + isfound.Value;
                                                newLoc = new FileInfo(whereFrom);
                                                gotAirSplash = true;
                                            }
                                        }
                                    }
                                    if (installInfo.origonal.ToLower().Replace("\\","/").Contains(origonalModel.skl.FileName.ToLower()))
                                    {
                                        if (Properties.Settings.Default.ch3d)
                                        {
                                            newLoc = new FileInfo(targetModel.skl.RAFArchive.RAFFilePath + "\\" +
                                                targetModel.skl.FileName);
                                            gotSKL = true;
                                        }
                                    }
                                    if (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.skn.FileName.ToLower()))
                                    {
                                        if (Properties.Settings.Default.ch3d)
                                        {
                                            newLoc = new FileInfo(targetModel.skn.RAFArchive.RAFFilePath + "\\" +
                                                targetModel.skn.FileName);
                                            gotSKN = true;
                                        }
                                    }
                                    if (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.texture.FileName.ToLower()))
                                    {
                                        if (Properties.Settings.Default.chTx)
                                        {
                                            newLoc = new FileInfo(targetModel.texture.RAFArchive.RAFFilePath + "\\" +
                                                targetModel.texture.FileName);
                                            gotTXT = true;
                                        }
                                    }
                                    if (origonalModel.loadScreen != null)
                                    {
                                        if (Properties.Settings.Default.loadscreensinst)
                                        {
                                            if (installInfo.origonal.ToLower().Replace("\\", "/").Contains(origonalModel.loadScreen.FileName.ToLower()))
                                            {
                                                if (targetModel.loadScreen != null)
                                                    newLoc = new FileInfo(targetModel.loadScreen.RAFArchive.RAFFilePath + "\\" +
                                                        targetModel.loadScreen.FileName);
                                                else gotLOD = true;//don't try and get it later

                                                gotLOD = true;
                                            }
                                            else
                                            {
                                            }
                                        }
                                    }
                                    else
                                    {
                                        gotLOD = true;//don't try and get it later
                                    }
                                    if(newLoc!=null)
                                    {
                                        newFileInfos.Add(new installFileInfo(installInfo.origonal,
                                        newLoc.Name,
                                        newLoc.Directory.FullName.ToLower()
                                        .Replace(gameDirectory.ToLower(), "")+"\\"));
                                        
                                        debugadd("We are moving a skin to a new skin, old is "+installInfo.origonal+" new is "
                                            +newLoc.Name);
                                    }
                                    
                                    
                                }
                                //check for skn and skl files that may not have been included
                                if (!gotSKL && Properties.Settings.Default.ch3d)
                                {
                                    FileInfo targetLoc = new FileInfo(targetModel.skl.RAFArchive.RAFFilePath + "\\" +
                                                    targetModel.skl.FileName);

                                    debugadd("we need to get \r\n" +
                                         origonalModel.skl.RAFArchive.RAFFilePath + "\\" +
                                                    origonalModel.skl.FileName
                                        + "\r\nTo replace \r\n" +
                                        targetModel.skl.RAFArchive.RAFFilePath + "\\" +
                                                    targetModel.skl.FileName);

                                    if (!Directory.Exists(extraRafFolder))
                                        Directory.CreateDirectory(extraRafFolder);

                                    rafBackup(origonalModel.skl.RAFArchive.RAFFilePath + "\\" + origonalModel.skl.FileName,
                                    extraRafFolder + "\\" + targetLoc.Name, false);

                                    newFileInfos.Add(new installFileInfo(
                                        extraRafFolder + "\\" + targetLoc.Name,
                                       targetLoc.Name,
                                        targetLoc.Directory.FullName.ToLower()
                                        .Replace(gameDirectory.ToLower(), "") + "\\"));

                                }
                                if (!gotSKN && Properties.Settings.Default.ch3d)
                                {
                                    FileInfo targetLoc = new FileInfo(targetModel.skn.RAFArchive.RAFFilePath + "\\" + targetModel.skn.FileName);
                                    debugadd("we need to get \r\n" + origonalModel.skn.RAFArchive.RAFFilePath + "\\" + origonalModel.skn.FileName + "\r\nTo replace \r\n" + targetModel.skn.RAFArchive.RAFFilePath + "\\" + targetModel.skn.FileName);

                                    if (!Directory.Exists(extraRafFolder)) Directory.CreateDirectory(extraRafFolder);

                                    rafBackup(origonalModel.skn.RAFArchive.RAFFilePath + "\\" + origonalModel.skn.FileName, extraRafFolder + "\\" + targetLoc.Name, false);

                                    newFileInfos.Add(new installFileInfo(extraRafFolder + "\\" + targetLoc.Name,
                                       targetLoc.Name, targetLoc.Directory.FullName.ToLower()
                                        .Replace(gameDirectory.ToLower(), "") + "\\"));
                                }
                                if (!gotTXT && Properties.Settings.Default.chTx)
                                {
                                    FileInfo targetLoc = new FileInfo(targetModel.texture.RAFArchive.RAFFilePath + "\\" + targetModel.texture.FileName);
                                    debugadd("we need to get \r\n" + origonalModel.texture.RAFArchive.RAFFilePath + "\\" + origonalModel.texture.FileName + "\r\nTo replace \r\n" + targetModel.texture.RAFArchive.RAFFilePath + "\\" + targetModel.texture.FileName);

                                    if (!Directory.Exists(extraRafFolder)) Directory.CreateDirectory(extraRafFolder);

                                    rafBackup(origonalModel.texture.RAFArchive.RAFFilePath + "\\" + origonalModel.texture.FileName, extraRafFolder + "\\" + targetLoc.Name, false);

                                    newFileInfos.Add(new installFileInfo(extraRafFolder + "\\" + targetLoc.Name,
                                       targetLoc.Name, targetLoc.Directory.FullName.ToLower()
                                        .Replace(gameDirectory.ToLower(), "") + "\\"));
                                }
                                if (!gotLOD && Properties.Settings.Default.loadscreensinst)
                                {
                                    FileInfo targetLoc = new FileInfo(targetModel.loadScreen.RAFArchive.RAFFilePath + "\\" + targetModel.loadScreen.FileName);
                                    debugadd("we need to get \r\n" + origonalModel.loadScreen.RAFArchive.RAFFilePath + "\\" + origonalModel.loadScreen.FileName + "\r\nTo replace \r\n" + targetModel.loadScreen.RAFArchive.RAFFilePath + "\\" + targetModel.loadScreen.FileName);

                                    if (!Directory.Exists(extraRafFolder)) Directory.CreateDirectory(extraRafFolder);

                                    rafBackup(origonalModel.loadScreen.RAFArchive.RAFFilePath + "\\" + origonalModel.loadScreen.FileName, extraRafFolder + "\\" + targetLoc.Name, false);

                                    newFileInfos.Add(new installFileInfo(extraRafFolder + "\\" + targetLoc.Name,
                                       targetLoc.Name, targetLoc.Directory.FullName.ToLower()
                                        .Replace(gameDirectory.ToLower(), "") + "\\"));
                                }
                                if (!gotAirPic && Properties.Settings.Default.air)
                                {
                                    KeyValuePair<string, string> targetfound =
                                        allFilesList.FirstOrDefault(m => m.Key.ToLower().
                                            Contains(targetModel.champPic.ToLower()));
                                    KeyValuePair<string, string> origonalfound =
                                        allFilesList.FirstOrDefault(m => m.Key.ToLower().
                                            Contains(origonalModel.champPic.ToLower()));
                                    if (targetfound.Key != null && origonalfound.Key != null)
                                    {
                                        FileInfo targetLoc = new FileInfo(gameDirectory + targetfound.Value);
                                        debugadd("we need to get \r\n" + gameDirectory + origonalfound.Value + "\r\nTo replace \r\n" + gameDirectory + targetfound.Value);

                                        if (!Directory.Exists(extraRafFolder)) Directory.CreateDirectory(extraRafFolder);

                                        SIFileOp.FileCopy(gameDirectory + origonalfound.Value, extraRafFolder + "\\" + targetLoc.Name);

                                        newFileInfos.Add(new installFileInfo(extraRafFolder + "\\" + targetLoc.Name,
                                           targetLoc.Name, targetLoc.Directory.FullName.ToLower()
                                            .Replace(gameDirectory.ToLower(), "") + "\\"));
                                    }
                                }
                                if (!gotAirSplash && Properties.Settings.Default.air)
                                {
                                    KeyValuePair<string, string> targetfound =
                                        allFilesList.FirstOrDefault(m => m.Key.ToLower().
                                            Contains(targetModel.champSplash.ToLower()));
                                    KeyValuePair<string, string> origonalfound =
                                        allFilesList.FirstOrDefault(m => m.Key.ToLower().
                                            Contains(origonalModel.champSplash.ToLower()));
                                    if (targetfound.Key != null && origonalfound.Key != null)
                                    {
                                        FileInfo targetLoc = new FileInfo(gameDirectory + targetfound.Value);
                                        debugadd("we need to get \r\n" + gameDirectory + origonalfound.Value + "\r\nTo replace \r\n" + gameDirectory + targetfound.Value);

                                        if (!Directory.Exists(extraRafFolder)) Directory.CreateDirectory(extraRafFolder);

                                        SIFileOp.FileCopy(gameDirectory + origonalfound.Value, extraRafFolder + "\\" + targetLoc.Name);

                                        newFileInfos.Add(new installFileInfo(extraRafFolder + "\\" + targetLoc.Name,
                                           targetLoc.Name, targetLoc.Directory.FullName.ToLower()
                                            .Replace(gameDirectory.ToLower(), "") + "\\"));
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (installFileInfo toAddInfo in newFileInfos)
                {
                    filteredFileInfo.Add(toAddInfo);
                }
                foreach (installFileInfo toRemoveInfo in toRemove)
                {
                    filteredFileInfo.Remove(toRemoveInfo);
                }
            }
            debugadd("After char selection we have");
            foreach (installFileInfo fileInfo in filteredFileInfo)
            {
                debugadd(fileInfo.getFileNamePath());
            }
            debugadd("fin");
        }
        private void zipBackup(string destination, string altDestination="", bool addLocalDirsToPath=false)
        {
            String zipLocation = destination.Substring(0, destination.IndexOf(".zip") + 4);
            
            String zipInnerLocation = destination.Substring(destination.IndexOf(".zip") + 5).Replace("\\", "/");
            String localLocation = destination.Replace(gameDirectory, "");
            String backupDir = Application.StartupPath + @"\backup\";
            if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
            string backupDest = backupDir + localLocation;
            if (altDestination != "")
            {
                backupDest = altDestination;
                if (addLocalDirsToPath) backupDest +=
                    zipInnerLocation.Replace("/", "\\");
                    //rafInnerLocation.Substring(0,rafInnerLocation.LastIndexOf("/")).Replace("/","\\");
            }
            

            string infos = "attempting to backup A ZIP FILE\r\n\r\ndesitnation is\r\n" + destination + "\r\n raf file is\r\n" + zipLocation
             + "\r\ninner location is\r\n" + zipInnerLocation+"\r\n backup destination is \r\n"+backupDest;

            string[] strArray = backupDest.Split(new char[] { '\\' });
            string path = backupDest.Remove(backupDest.Length - strArray[strArray.Length - 1].Length);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            debugadd(infos);
            try
            {
                using (FileStream fs = new FileStream(backupDest, FileMode.OpenOrCreate))
                {
                    using (ZipFile zip = ZipFile.Read(zipLocation))
                    {
                        //find the right entry
                        foreach (ZipEntry potentry in zip.Entries)
                        {
                            string entryPath = potentry.FileName.ToLower().Replace("\\", "/");
                            if (entryPath == zipInnerLocation)
                            {                            
                                potentry.Extract(fs);

                                zip.Dispose();
                                break;
                            }
                        }
                    
                    }

                    fs.Close();
                }
            }
            catch (System.Exception ex)
            {

            }
        }
        public void zipInject(string origonal, string destination)
        {
            FileInfo origonalFI = new FileInfo(origonal);
            String zipLocation = destination.Substring(0, destination.IndexOf(".zip") + 4);
            String zipInnerLocation = destination.Substring(destination.IndexOf(".zip") + 5).Replace("\\", "/");
            string infos = "THIS IS A ZIP FILE\r\n\r\nOrigonal is\r\n" + origonal +
              "\r\ndesitnation is\r\n" + destination + "\r\n zip file is\r\n" + zipLocation
             + "\r\ninner location is\r\n" + zipInnerLocation;
            debugadd(infos);
            //Cliver.Message.Inform(infos);
            try
            {
            using (ZipFile zip = ZipFile.Read(zipLocation))
            {
                //find the right entry
                for (int x = zip.Count - 1; x >= 0; x--)
                {
                    ZipEntry potentry = zip[x];
                    string entryPath = potentry.FileName.ToLower().Replace("\\", "/");
                    if (entryPath == zipInnerLocation)
                    {
                        string zipRealName = potentry.FileName;
                        //zipRealName = zipRealName.Replace("\\", "/");
                        string zipRealPath = zipRealName.Substring(0, zipRealName.Length - origonalFI.Name.Length -1);
                        if (zip.ContainsEntry(zipRealName))
                        {
                            //expected
                            zip.RemoveEntry(zipRealName);
                            zip.RemoveEntry(potentry);
                            if (zip.ContainsEntry(zipRealName))
                            {
                                //unable to delete!!
                            }
                            //todo make this caps
                            zip.AddEntry(zipRealName, File.ReadAllBytes(origonal));
                            //zip.AddFile(origonal, zipRealPath);
                            zip.Save();
                        }
                        else
                        {
                            /*zip.Dispose();
                            //This zip is fubar!!
                            if (!Directory.Exists(Application.StartupPath + "\\"+c_DIRECTORY_NAME_FOR_FIXING_ZIPS))
                                Directory.CreateDirectory(Application.StartupPath + "\\"+c_DIRECTORY_NAME_FOR_FIXING_ZIPS);

                            zip.ExtractAll(Application.StartupPath + "\\" + c_DIRECTORY_NAME_FOR_FIXING_ZIPS);
                            string moveTo = Application.StartupPath + "\\" + c_DIRECTORY_NAME_FOR_FIXING_ZIPS + "\\" + zipRealName.Replace("/","\\");

                            this.SIFileOp.FileMove(origonal,moveTo);
                            string newZipLocation=Application.StartupPath + "\\" + c_DIRECTORY_NAME_FOR_FIXING_ZIPS + "\\" +"Archive.zip";
                            using (ZipFile zipFixed = new ZipFile())
                            {
                                zipFixed.AddDirectory(Application.StartupPath + "\\" + c_DIRECTORY_NAME_FOR_FIXING_ZIPS);
                                zipFixed.Save(newZipLocation);
                            }
                            this.SIFileOp.FileMove(newZipLocation, zipLocation);

                            this.SIFileOp.DirectoryDelete(Application.StartupPath + "\\"+c_DIRECTORY_NAME_FOR_FIXING_ZIPS+"\\", true);
                            */

                        }
                        
                        break;
                    }
                }
            }
            }
            catch (System.Exception ex)
            {

            }
        }
        private void fileCopyFiles(ref List<installFileInfo> filteredFileInfo)
        {
            int fn = 0;
            foreach (installFileInfo installInfo in filteredFileInfo)
            {
                int percent = (int)Math.Floor(((double)fn++ / (double)filteredFileInfo.Count) * 90.0)+10;
                installWorker2.ReportProgress(percent, "Installing file " + fn.ToString() + " of " + filteredFileInfo.Count.ToString() + " :: " + installInfo.fileName);

                if (installInfo.getFileNamePath().Contains(".raf"))
                {
                    //handle this one different
                    debugadd("raf backup " + installInfo.getFileNamePath());
                    rafBackup(gameDirectory + installInfo.getFileNamePath(), "");
                    debugadd("raf Installing " + installInfo.getFileNamePath());
                    rafInject(installInfo.origonal, gameDirectory + installInfo.getFileNamePath());
                }
                else if (installInfo.getFileNamePath().Contains(".zip"))
                {
                    
                    // lgg todo
                    debugadd("zip backup " + installInfo.getFileNamePath());
                    zipBackup(gameDirectory + installInfo.getFileNamePath(), "");
                    debugadd("zip Installing " + installInfo.getFileNamePath());
                    zipInject(installInfo.origonal, gameDirectory + installInfo.getFileNamePath());
                }
                else
                {
                    debugadd("Backup " + installInfo.getFileNamePath());

                    backupFile( installInfo.filePath,installInfo.fileName);
                    debugadd("Installing " + installInfo.origonal);

                    this.SIFileOp.FileMove(installInfo.origonal,
                        gameDirectory + installInfo.getFileNamePath());
                }
                //Cliver.Message.Show(this.Icon, "Installing " + str + " to " + gameDirectory + str2 + str3, 0, "OK");
                debugadd("Done Installing " + installInfo.origonal + " to " 
                    + gameDirectory + installInfo.getFileNamePath());
            }                 
        }
        private void installProgress(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage);
            if (e.UserState != null)
            {
                string info = e.UserState as string;
                debugadd(info);
                this.statusText.Text = info;
                this.helpBar.Update();
            }
        }
        private void installComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Error == null))
            {
                Cliver.Message.Inform("Error!\r\nSomething has gone wrong installing the skin,\r\n" +
                    "You can normally stop this error from happening by downloading and installing dot net 3.5 from\r\n" +
                    " here http://www.microsoft.com/downloads/en/details.aspx?familyid=ab99342f-5d1a-413d-8319-81da479ab0d7 " +
                    "\r\n\r\nIf you have done that, and you still get this error, please contact LGG, tell him what you did, and give him this info" +
                    "\r\n" + e.Error.ToString());
                debugadd("Error: " + e.Error.Message);
            }
            //this.Enabled = true;
            //           prettyDate pd = new prettyDate(DateTime.Now);
            //item.SubItems[4].Text = pd.ToString();
            setInstallButtons(true);
            UpdateListView();
            if(Properties.Settings.Default.sendStats)
                webPinger.RunWorkerAsync("http://c.statcounter.com/6898201/0/e70b18ab/0/");
        }
        private void installTextMod(string textModLocation)
        {
            String backupDir = Application.StartupPath + @"\backup\";
            //Cliver.Message.Inform("Going to install text from "+ textModLocation);
            string textPath = getMenuFilePath();
            if(textPath!="")
                ted.installText(textPath, textModLocation, backupDir);
        }
        private void button3reinstallText_Click(object sender, EventArgs e)
        {
            setInstallButtons(false);
            int reinst = 0;
            string extra = "";
            string riottextPath = getMenuFilePath();
            if (riottextPath == "") return;
                            
            foreach (ListViewItem item in this.listView1.Items)
            {
                if (item.SubItems[4].Text != "No")
                {
                    string skinName = item.SubItems[1].Text;
                    String skinPath = Application.StartupPath + @"\skins\" + skinName;
                    string textPath = skinPath += @"\textmods\customtext.txt";
                    if (File.Exists(textPath))
                    {
                        //Cliver.Message.Inform("Found a skin to reinstall at \r\n" + textPath);
                        extra += textPath;
                        String backupDir = Application.StartupPath + @"\backup\";
                        reinst++;
                        ted.installText(riottextPath, textPath, backupDir);
                    }
                }
            }
            if (reinst > 0)
            {
                Cliver.Message.Inform("Reinstalled " + reinst.ToString() + " text mods\r\n\r\n"
                    + extra
                    + "\r\n\r\nYou text mods should now be applied for this update");
            }
            else
            {
                Cliver.Message.Inform("No text mods found to be installed,\r\nSo nothing to re-install");
            }
            setInstallButtons(true);
        }
        #endregion
        #region Uninstalling
        private void uninstallWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bool soundsflag = false;
            List<ListViewItem> a = (List<ListViewItem>)e.Argument;
            string msgs = "";
            int totalCheckedItems = a.Count;
            int checksCompleted = 0;
            float checkIsWorth = 100.0f / (float)totalCheckedItems;
            foreach (ListViewItem item in a)
            {
                

                List<string> charsToUninstall = new List<string>();
                if (true)//item.SubItems[4].Text == "Yes")
                {
                    string [] skinFiles = Directory.GetFiles(Application.StartupPath + @"\skins\" + item.SubItems[1].Text, "*.*", SearchOption.AllDirectories);
                    int filesDone = 0;
                    foreach (string str in skinFiles)
                    {
                        int percent = (int)((Math.Floor(((double)filesDone++ / (double)skinFiles.Length) * checkIsWorth))+(checkIsWorth*checksCompleted));
                        uninstallWorker1.ReportProgress(percent, "Installing file " + str.ToString() + " of " + skinFiles.Length.ToString() + "");

                        string[] strArray2 = str.Split(new char[] { '\\' });
                        string path = string.Empty;
                        bool flag = false;
                        for (int i = 1; i < (strArray2.Length - 1); i++)
                        {
                            if ((!flag && (strArray2[i - 1] == "skins")) && (strArray2[i] == item.SubItems[1].Text))
                            {
                                flag = true;
                            }
                            else if (flag)
                            {
                                path = path + strArray2[i] + @"\";
                            }
                        }
                        string fileName = strArray2[strArray2.Length - 1];
                        //Cliver.Message.Inform("str2 is " + str2 + " str3 is " + str3);

                        int airIndex = path.ToLower().IndexOf("\\lol_air_client\\releases\\");
                        //path must exist, unless its a air silly thing, in which case we need to check new version    
                        if ((path != null) && (File.Exists(gameDirectory + path + fileName) || (airIndex > 0)))
                        {
                            //check and see if we have a backup to restore
                            String backupDir = Application.StartupPath + @"\backup\";

                            debugadd("Uninstalling " + gameDirectory + path + fileName + " looking for backup in " + backupDir + path + fileName);
                            String bf = backupDir + path + fileName;
                            //we need to add a fancy check about air files here
                            //make sure we use the older version backup we have so legacy backups still work
                            //we need to also make sure we re-install the backup to the new air location
                            if (airIndex > 0)
                            {
                                //oh boy it is a air file
                                DirectoryInfo airRestoreDir = new DirectoryInfo(backupDir + path.Substring(0, airIndex + 25));
                                string remainingPath = path.Substring(path.Substring(airIndex + 25).IndexOf("\\") + airIndex + 25);
                                //debugadd("air file restore dir :"+airbackupDir.FullName);

                                //we have the correct backup, now we have to find 
                                string highestVersion = "";
                                DirectoryInfo airWorkingDir = new DirectoryInfo(gameDirectory + path.Substring(0, airIndex + 25));
                                foreach (DirectoryInfo dir in airWorkingDir.GetDirectories())
                                {
                                    //make sure we pick the highest version
                                    if (highestVersion == "") highestVersion = dir.Name;
                                    else
                                    {
                                        string[] versions = dir.Name.Split('.');
                                        string[] highestversions = highestVersion.Split('.');
                                        for (int i = versions.Length - 1; i >= 0; i--)
                                        {
                                            int vA = int.Parse(versions[i].Trim());
                                            int lvA = int.Parse(highestversions[i].Trim());
                                            //tacky but im re-using this var name to mean "highest dir"
                                            if (vA > lvA) highestVersion = dir.Name;

                                        }
                                    }
                                }



                                string lowestVersion = "";
                                //todo get for all air files
                                for (int airV = 0; airV <= 10; airV++)
                                {
                                    string fileNameOption = Regex.Replace(fileName, @"\d+", airV.ToString(), RegexOptions.IgnoreCase);

                                    if (airRestoreDir.Exists)
                                    {
                                        foreach (DirectoryInfo dir in airRestoreDir.GetDirectories())
                                        {
                                            string lookfor = airRestoreDir + dir.Name + remainingPath + fileNameOption;
                                            if (File.Exists(lookfor))
                                            {
                                                if (lowestVersion == "") lowestVersion = dir.Name;
                                                else
                                                {

                                                    string[] versions = dir.Name.Split('.');
                                                    string[] lowestversions = lowestVersion.Split('.');
                                                    for (int i = versions.Length - 1; i >= 0; i--)
                                                    {
                                                        int vA = int.Parse(versions[i].Trim());
                                                        int lvA = int.Parse(lowestversions[i].Trim());
                                                        if (vA < lvA) lowestVersion = dir.Name;
                                                    }
                                                }
                                            }
                                        }
                                        if (lowestVersion != "")
                                        {

                                            bf = airRestoreDir + lowestVersion + remainingPath + fileNameOption;

                                            path = path.Substring(0, airIndex + 25) + highestVersion + remainingPath;
                                            if (File.Exists(bf))
                                            {
                                                this.SIFileOp.FileDelete(gameDirectory + path + fileNameOption);

                                                this.SIFileOp.FileCopy(bf, gameDirectory + path + fileNameOption);
                                            }
                                        }
                                    }
                                }

                            }
                            if (File.Exists(bf))
                            {
                                this.SIFileOp.FileDelete(gameDirectory + path + fileName);

                                this.SIFileOp.FileCopy(bf, gameDirectory + path + fileName);
                            }
                            else
                            {
                                debugadd("Missing backup for " + path + " and " + fileName + ", this probably means it was skipped during install");
                            }
                        }
                        else if (path.Contains(".zip"))
                        {
                            //lgg todo
                            string pathTOInstallTo = path.Replace("zipfile", "\\GameData\\Apps\\LoL\\Game");
                            pathTOInstallTo=gameDirectory + pathTOInstallTo + fileName;
                            this.SIFileOp.FileDelete(pathTOInstallTo);
                        }
                        else if (path.Contains(".raf"))
                        {

                            //handle this one different

                            //check and see if we have a backup to restore
                            String backupDir = Application.StartupPath + @"\backup\";
                            debugadd("Uninstalling " + gameDirectory + path + fileName + " looking for backup in " + backupDir + path + fileName);
                            String bf = backupDir + path + fileName;
                            if (File.Exists(bf))
                            {
                                //this.SIFileOp.FileCopy(bf, gameDirectory + str2 + str3);
                                rafInject(bf, gameDirectory + path + fileName);
                            }


                        }

                        int pathIndex = path.ToLower().IndexOf("characters");
                        if (pathIndex > -1)
                        {
                            string character = path.Substring(pathIndex + 11).Replace("\\", "");
                            if (!charsToUninstall.Contains(character))
                                charsToUninstall.Add(character);
                        }
                        if (path.Contains("formatedsounds"))
                        {
                            soundsflag = true;
                        }
                        if (path.Contains("textmods"))
                        {
                            uninstallTextMod(str);
                        }
                    }
                }
                if (soundsflag)
                {
                    restoreSounds();
                }
                if (charsToUninstall.Count > 0)
                {
                    foreach (String character in charsToUninstall)
                    {
                        // un install all files of a character to account for modifications during char selection
                        string backupFolder = Application.StartupPath + @"\backup\";
                        string lookFor = ".raf\\data\\characters\\" + character.ToLower() + "\\";

                        string[] allBackupFiles = Directory.GetFiles(backupFolder, "*.*", SearchOption.AllDirectories);
                        foreach (string abackupFile in allBackupFiles)
                        {
                            if (abackupFile.ToLower().Contains(lookFor))
                            {
                                string rafDestination = gameDirectory + abackupFile.ToLower().Replace(backupFolder.ToLower(), "");
                                string TheRaf = rafDestination.Substring(0, rafDestination.IndexOf(".raf") + 4);
                                if (File.Exists(TheRaf))//make sure we don't restore a ancient backup that cant be
                                    rafInject(abackupFile, rafDestination);
                            }

                        }

                    }
                }
                this.ExecuteQuery("UPDATE skins SET sInstalled=0, dateinstalled=\"" + "-" + "\"" +
                    " WHERE sName=\"" +
                   item.SubItems[1].Text + "\"");
                this.SIFileOp.FileDelete(Application.StartupPath + @"\skins\" + item.SubItems[1].Text + @"\installed");
                msgs += "Successfully un-installed " + item.SubItems[1].Text +
                    "\r\n";
                e.Result += msgs;
                //Cliver.Message.Inform("Successfuly uninstalled " + item.SubItems[1].Text);

                checksCompleted++;
            }
        }

        private void uninstallWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage);
            if (e.UserState != null)
            {
                string info = e.UserState as string;
                debugadd(info);
                this.statusText.Text = info;
                this.helpBar.Update();
            }
        }

        private void uninstallWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateProgressSafe(100);
            string msgs = (string)e.Result;
            Cliver.Message.Inform(msgs);
            UpdateProgressSafe(0);
            this.UpdateListView();

            this.dbInstall.Enabled = this.dbUninstall.Enabled = this.dbDelete.Enabled = this.UpdateFL.Enabled = this.button3reinstallText.Enabled = this.button3FixCrashAfterPatch.Enabled= true;
        }

        private void dbUninstall_Click(object sender, EventArgs e)
        {
            //Cliver.Message.Inform("UnInstall currently does not work, please repair your client to undo skins.\n\nOpen up your launcher, click the gear on the top right, then click repair.");
            //return;
            if (fileListWorker1.IsBusy)
            {
                Cliver.Message.Inform("Please wait a moment for this program to finish updating\r\nThe Progress Bar below will show you the status of this");
                return;
            }
            this.dbInstall.Enabled = this.dbUninstall.Enabled = this.dbDelete.Enabled = this.UpdateFL.Enabled = this.button3reinstallText.Enabled = this.button3FixCrashAfterPatch.Enabled = false;
            List<ListViewItem> args = new List<ListViewItem>();
            foreach (ListViewItem item in this.listView1.CheckedItems)
            {
                args.Add(item);
            }
            UpdateProgressSafe(1);
            uninstallWorker1.RunWorkerAsync(args);

        }
        private void uninstallTextMod(string textModPath)
        {
            String backupDir = Application.StartupPath + @"\backup\";

            //Cliver.Message.Inform("Going to uninstall text from " + textModPath);
            List<string> textPaths = new List<string>();
            foreach (KeyValuePair<string, string> pair in getMenuFilePaths())
            {
                textPaths.Add(gameDirectory + pair.Value.Replace("\\\\", "\\").Substring(1));
            }
            ted.uninstallTexts(textPaths, textModPath, backupDir);
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void ExecuteQuery(string txtQuery)
        {
            try
            {
                //this.sqLiteCon.ConnectionString = "data source=\"" + Application.StartupPath + "\\skins.db\"";
                //if (this.sqLiteCon.State.ToString() != "Open")
                //{
                //    this.sqLiteCon.Open();
                //}
                SQLiteCommand command = this.sqLiteCon.CreateCommand();
                command.CommandText = txtQuery;
                command.ExecuteNonQuery();
            }
            catch
            {
                if (Cliver.Message.Show("View Exception", SystemIcons.Information,"There was an error with the database! Do you want to see the exception?", 0, new string[2] { "Yes", "No" }) == 0)
                {
                    throw;
                }
            }
            finally
            {
                //this.sqLiteCon.Close();
            }
        }
        private String findSoundsFSBLocation()
        {
            if (Properties.Settings.Default.manualSoundFileLocation != "")
            {
                return Properties.Settings.Default.manualSoundFileLocation;
            }
            //\Game\DATA\Sounds\FMOD\VOBank_en_US.fsb
            string soundfile = "VOBank_en_US.fsb";
            foreach (KeyValuePair<String, String> pair in allFilesList)
            {
                if (soundfile.ToLower()== pair.Key.ToLower())
                {
                    return pair.Value.Replace("\\\\", "\\");
                }
            }
            //if(allFilesDic.ContainsKey(soundfile.ToLower()))
            //{
            //    return allFilesDic[soundfile.ToLower()].Replace("\\\\", "\\");
            //}
           /* for (int i = 0; i < allFilesCt; i++)
            {
                if (soundfile.ToLower() == allFiles[i, 0].ToLower())
                {
                    return allFiles[i, 1].Replace("\\\\", "\\");
                }
            }*/
            return "not found";
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(skinInstaller));
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Please wait for the progress bar to finish loading bellow...");
            this.exit = new System.Windows.Forms.Button();
            this.skinFile = new System.Windows.Forms.OpenFileDialog();
            this.helpBar = new System.Windows.Forms.StatusStrip();
            this.helpText = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.loadScreenFile = new System.Windows.Forms.OpenFileDialog();
            this.b_AddDirectory = new System.Windows.Forms.Button();
            this.b_ClearAll = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.packFileNameTextBox = new System.Windows.Forms.TextBox();
            this.packFileList = new System.Windows.Forms.ListBox();
            this.b_RemoveFiles = new System.Windows.Forms.Button();
            this.b_CreatePack = new System.Windows.Forms.Button();
            this.b_AddFiles = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.installFiles_ListBox = new System.Windows.Forms.ListBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.skinNameTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxauthor = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button3openAd = new System.Windows.Forms.Button();
            this.AddToDatabasePanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.addFilesPanel = new System.Windows.Forms.Panel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.b_IAddFiles = new System.Windows.Forms.Button();
            this.b_IAddDirectory = new System.Windows.Forms.Button();
            this.saveToDb = new System.Windows.Forms.CheckBox();
            this.b_IClearAll = new System.Windows.Forms.Button();
            this.b_IRemoveFiles = new System.Windows.Forms.Button();
            this.b_IInstallFiles = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.resetLoadScreenBox = new System.Windows.Forms.Button();
            this.resetSkinBox = new System.Windows.Forms.Button();
            this.loadScreenLbl = new System.Windows.Forms.Label();
            this.browseLoadScreen = new System.Windows.Forms.Button();
            this.loadScreenImageTextBox = new System.Windows.Forms.TextBox();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.reset = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.browse = new System.Windows.Forms.Button();
            this.skinVariant = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.changeSkinButton = new System.Windows.Forms.Button();
            this.selectchampiontxt = new System.Windows.Forms.Label();
            this.selectChampion = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new SkinInstaller.ListViewItemHover();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dataBaseListMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSelectUninstalled = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSelectAllInstalled = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllSkinsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllSkinsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editThisSkinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewThisSkinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel9 = new System.Windows.Forms.Panel();
            this.colorSlider1 = new MB.Controls.ColorSlider();
            this.panel8 = new System.Windows.Forms.Panel();
            this.checkBox1dispCharacter = new System.Windows.Forms.CheckBox();
            this.checkBox1dispDateInstalledFull = new System.Windows.Forms.CheckBox();
            this.checkBox1dispDateAddedFull = new System.Windows.Forms.CheckBox();
            this.checkBox1dispDateAdded = new System.Windows.Forms.CheckBox();
            this.checkBox1dispInstalled = new System.Windows.Forms.CheckBox();
            this.checkBox1dispFileCount = new System.Windows.Forms.CheckBox();
            this.checkBox1dispAuthor = new System.Windows.Forms.CheckBox();
            this.checkBox1dispTitle = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.labelSkinName = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3openAds3 = new System.Windows.Forms.Button();
            this.button3reinstallText = new System.Windows.Forms.Button();
            this.dbDelete = new System.Windows.Forms.Button();
            this.dbInstall = new System.Windows.Forms.Button();
            this.dbUninstall = new System.Windows.Forms.Button();
            this.createZip = new System.Windows.Forms.Button();
            this.button3repath = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button3openAds2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.button3exporttree = new System.Windows.Forms.Button();
            this.buttonRebuildTree = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.treeViewMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportSelectedFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lookUpReleaseManifestInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateFL = new System.Windows.Forms.Button();
            this.locateGameClient = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editInstallPreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeGameClientLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateFileListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editAllPreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerAppForWebUrlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createDesktopShortcutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixRiotWarningsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moreDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getVersionFilePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readVersionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getLastModDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDXTVersionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.associateFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteBackupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repathAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iAmLGGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iCantStandLGGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wtfRainbowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setSoundFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getProgramLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCharacterIconsCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useDDSVersionReaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testReadResFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unpackSoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loLViewerOpenNotPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openParticleReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMenuFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTextTreeEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPublisherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skinHelpersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyLikeFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelGL = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBoxCount = new System.Windows.Forms.PictureBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.button3FixCrashAfterPatch = new System.Windows.Forms.Button();
            this.progrespanel = new System.Windows.Forms.Panel();
            this.label2Percent = new System.Windows.Forms.Label();
            this.buttoncancel = new System.Windows.Forms.Button();
            this.button3startLoL = new System.Windows.Forms.Button();
            this.button3lcintegrate = new System.Windows.Forms.Button();
            this.fileListWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.installWorker2 = new System.ComponentModel.BackgroundWorker();
            this.updateWorker2 = new System.ComponentModel.BackgroundWorker();
            this.webPinger = new System.ComponentModel.BackgroundWorker();
            this.timeupdatecount = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorkerCountUpdate = new System.ComponentModel.BackgroundWorker();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.rafTreeBuilderWorker2 = new System.ComponentModel.BackgroundWorker();
            this.exportFolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ParticleTreeWorker2 = new System.ComponentModel.BackgroundWorker();
            this.treeMenuStripSkin1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.makeSimpleSkinFromThisRiotSkinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTreeViewWorker1 = new System.ComponentModel.BackgroundWorker();
            this.ParticleTreeWorkerNew = new System.ComponentModel.BackgroundWorker();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.panel10 = new System.Windows.Forms.Panel();
            this.webBrowser2Test = new SkinInstaller.ExtendedWebBrowser();
            this.button3CloseAd = new System.Windows.Forms.Button();
            this.addFilesWorker = new System.ComponentModel.BackgroundWorker();
            this.patchFixerWorker = new System.ComponentModel.BackgroundWorker();
            this.uninstallWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tabPage2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.AddToDatabasePanel.SuspendLayout();
            this.addFilesPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.dataBaseListMenuStrip1.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel8.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.treeViewMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelGL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCount)).BeginInit();
            this.panel7.SuspendLayout();
            this.progrespanel.SuspendLayout();
            this.treeMenuStripSkin1.SuspendLayout();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // exit
            // 
            this.exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exit.Location = new System.Drawing.Point(825, 3);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(57, 23);
            this.exit.TabIndex = 20;
            this.exit.Text = "Exit";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            this.exit.MouseEnter += new System.EventHandler(this.exit_MouseEnter);
            this.exit.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // skinFile
            // 
            this.skinFile.FileName = "skinFile";
            // 
            // helpBar
            // 
            this.helpBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.helpBar.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.helpBar.Location = new System.Drawing.Point(0, 463);
            this.helpBar.Name = "helpBar";
            this.helpBar.Size = new System.Drawing.Size(886, 22);
            this.helpBar.TabIndex = 0;
            this.helpBar.Text = "Ready..";
            // 
            // helpText
            // 
            this.helpText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpText.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.helpText.Name = "helpText";
            this.helpText.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // statusText
            // 
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(18, 17);
            this.statusText.Text = ":D";
            // 
            // b_AddDirectory
            // 
            this.b_AddDirectory.Location = new System.Drawing.Point(0, 0);
            this.b_AddDirectory.Name = "b_AddDirectory";
            this.b_AddDirectory.Size = new System.Drawing.Size(75, 23);
            this.b_AddDirectory.TabIndex = 0;
            // 
            // b_ClearAll
            // 
            this.b_ClearAll.Location = new System.Drawing.Point(0, 0);
            this.b_ClearAll.Name = "b_ClearAll";
            this.b_ClearAll.Size = new System.Drawing.Size(75, 23);
            this.b_ClearAll.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Name Your File Pack:";
            // 
            // packFileNameTextBox
            // 
            this.packFileNameTextBox.Location = new System.Drawing.Point(0, 0);
            this.packFileNameTextBox.Name = "packFileNameTextBox";
            this.packFileNameTextBox.Size = new System.Drawing.Size(100, 26);
            this.packFileNameTextBox.TabIndex = 0;
            // 
            // packFileList
            // 
            this.packFileList.FormattingEnabled = true;
            this.packFileList.Location = new System.Drawing.Point(7, 43);
            this.packFileList.Name = "packFileList";
            this.packFileList.Size = new System.Drawing.Size(285, 134);
            this.packFileList.Sorted = true;
            this.packFileList.TabIndex = 29;
            this.packFileList.MouseEnter += new System.EventHandler(this.packFileList_MouseEnter);
            this.packFileList.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // b_RemoveFiles
            // 
            this.b_RemoveFiles.Location = new System.Drawing.Point(0, 0);
            this.b_RemoveFiles.Name = "b_RemoveFiles";
            this.b_RemoveFiles.Size = new System.Drawing.Size(75, 23);
            this.b_RemoveFiles.TabIndex = 0;
            // 
            // b_CreatePack
            // 
            this.b_CreatePack.Location = new System.Drawing.Point(0, 0);
            this.b_CreatePack.Name = "b_CreatePack";
            this.b_CreatePack.Size = new System.Drawing.Size(75, 23);
            this.b_CreatePack.TabIndex = 0;
            // 
            // b_AddFiles
            // 
            this.b_AddFiles.Location = new System.Drawing.Point(0, 0);
            this.b_AddFiles.Name = "b_AddFiles";
            this.b_AddFiles.Size = new System.Drawing.Size(75, 23);
            this.b_AddFiles.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tabPage2.Controls.Add(this.panel4);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(720, 302);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "==Add New Skin==";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.installFiles_ListBox);
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(547, 296);
            this.panel4.TabIndex = 39;
            // 
            // installFiles_ListBox
            // 
            this.installFiles_ListBox.AllowDrop = true;
            this.installFiles_ListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.installFiles_ListBox.FormattingEnabled = true;
            this.installFiles_ListBox.ItemHeight = 20;
            this.installFiles_ListBox.Location = new System.Drawing.Point(0, 77);
            this.installFiles_ListBox.Name = "installFiles_ListBox";
            this.installFiles_ListBox.Size = new System.Drawing.Size(547, 219);
            this.installFiles_ListBox.TabIndex = 29;
            this.installFiles_ListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragDrop);
            this.installFiles_ListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragEnter);
            this.installFiles_ListBox.MouseEnter += new System.EventHandler(this.installFiles_ListBox_MouseEnter);
            this.installFiles_ListBox.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // panel5
            // 
            this.panel5.AllowDrop = true;
            this.panel5.Controls.Add(this.skinNameTextbox);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.textBoxauthor);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(547, 77);
            this.panel5.TabIndex = 38;
            this.panel5.DragDrop += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragDrop);
            this.panel5.DragEnter += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragEnter);
            // 
            // skinNameTextbox
            // 
            this.skinNameTextbox.Location = new System.Drawing.Point(64, 25);
            this.skinNameTextbox.MaxLength = 50;
            this.skinNameTextbox.Name = "skinNameTextbox";
            this.skinNameTextbox.Size = new System.Drawing.Size(228, 26);
            this.skinNameTextbox.TabIndex = 32;
            this.skinNameTextbox.WordWrap = false;
            this.skinNameTextbox.MouseEnter += new System.EventHandler(this.skinNameTextbox_MouseEnter);
            this.skinNameTextbox.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 20);
            this.label6.TabIndex = 34;
            this.label6.Text = "Skin Name:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(394, 20);
            this.label7.TabIndex = 30;
            this.label7.Text = "Please locate the skin or particle files you wish to install";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 20);
            this.label8.TabIndex = 36;
            this.label8.Text = "Author:";
            // 
            // textBoxauthor
            // 
            this.textBoxauthor.Location = new System.Drawing.Point(64, 50);
            this.textBoxauthor.MaxLength = 50;
            this.textBoxauthor.Name = "textBoxauthor";
            this.textBoxauthor.Size = new System.Drawing.Size(228, 26);
            this.textBoxauthor.TabIndex = 37;
            this.textBoxauthor.Text = "Unknown";
            this.textBoxauthor.WordWrap = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.button3openAd);
            this.panel3.Controls.Add(this.AddToDatabasePanel);
            this.panel3.Controls.Add(this.addFilesPanel);
            this.panel3.Controls.Add(this.saveToDb);
            this.panel3.Controls.Add(this.b_IClearAll);
            this.panel3.Controls.Add(this.b_IRemoveFiles);
            this.panel3.Controls.Add(this.b_IInstallFiles);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(550, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(167, 296);
            this.panel3.TabIndex = 38;
            // 
            // button3openAd
            // 
            this.button3openAd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3openAd.Location = new System.Drawing.Point(133, 270);
            this.button3openAd.Name = "button3openAd";
            this.button3openAd.Size = new System.Drawing.Size(31, 23);
            this.button3openAd.TabIndex = 39;
            this.button3openAd.Text = "<<";
            this.button3openAd.UseVisualStyleBackColor = true;
            this.button3openAd.Click += new System.EventHandler(this.button3openAd_Click);
            // 
            // AddToDatabasePanel
            // 
            this.AddToDatabasePanel.Controls.Add(this.button1);
            this.AddToDatabasePanel.Location = new System.Drawing.Point(4, 157);
            this.AddToDatabasePanel.Name = "AddToDatabasePanel";
            this.AddToDatabasePanel.Size = new System.Drawing.Size(161, 87);
            this.AddToDatabasePanel.TabIndex = 38;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 65);
            this.button1.TabIndex = 35;
            this.button1.Text = "Add to DataBase";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.addToDatabase_click);
            this.button1.MouseEnter += new System.EventHandler(this.button1_MouseEnter);
            this.button1.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // addFilesPanel
            // 
            this.addFilesPanel.BackColor = System.Drawing.Color.Lime;
            this.addFilesPanel.Controls.Add(this.textBox3);
            this.addFilesPanel.Controls.Add(this.b_IAddFiles);
            this.addFilesPanel.Controls.Add(this.b_IAddDirectory);
            this.addFilesPanel.Location = new System.Drawing.Point(3, 7);
            this.addFilesPanel.Name = "addFilesPanel";
            this.addFilesPanel.Size = new System.Drawing.Size(164, 115);
            this.addFilesPanel.TabIndex = 37;
            // 
            // textBox3
            // 
            this.textBox3.AllowDrop = true;
            this.textBox3.Location = new System.Drawing.Point(16, 34);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(124, 68);
            this.textBox3.TabIndex = 37;
            this.textBox3.Text = "You may also\r\nDrag and Drop\r\nFiles or Folders\r\nHere";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // b_IAddFiles
            // 
            this.b_IAddFiles.Location = new System.Drawing.Point(6, 5);
            this.b_IAddFiles.Name = "b_IAddFiles";
            this.b_IAddFiles.Size = new System.Drawing.Size(64, 23);
            this.b_IAddFiles.TabIndex = 25;
            this.b_IAddFiles.Text = "Add Files";
            this.b_IAddFiles.UseVisualStyleBackColor = true;
            this.b_IAddFiles.Click += new System.EventHandler(this.b_IAddFiles_Click);
            this.b_IAddFiles.MouseEnter += new System.EventHandler(this.b_IAddFiles_MouseEnter);
            this.b_IAddFiles.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // b_IAddDirectory
            // 
            this.b_IAddDirectory.Location = new System.Drawing.Point(81, 4);
            this.b_IAddDirectory.Name = "b_IAddDirectory";
            this.b_IAddDirectory.Size = new System.Drawing.Size(80, 23);
            this.b_IAddDirectory.TabIndex = 31;
            this.b_IAddDirectory.Text = "Add Directory";
            this.b_IAddDirectory.UseVisualStyleBackColor = true;
            this.b_IAddDirectory.Click += new System.EventHandler(this.b_IAddDirectory_Click);
            this.b_IAddDirectory.MouseEnter += new System.EventHandler(this.b_IAddDirectory_MouseEnter);
            this.b_IAddDirectory.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // saveToDb
            // 
            this.saveToDb.AutoSize = true;
            this.saveToDb.Checked = true;
            this.saveToDb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveToDb.Location = new System.Drawing.Point(11, 332);
            this.saveToDb.Name = "saveToDb";
            this.saveToDb.Size = new System.Drawing.Size(163, 24);
            this.saveToDb.TabIndex = 33;
            this.saveToDb.Text = "Save to Database";
            this.saveToDb.UseVisualStyleBackColor = false;
            this.saveToDb.Visible = false;
            this.saveToDb.MouseEnter += new System.EventHandler(this.saveToDb_MouseEnter);
            this.saveToDb.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // b_IClearAll
            // 
            this.b_IClearAll.Location = new System.Drawing.Point(85, 128);
            this.b_IClearAll.Name = "b_IClearAll";
            this.b_IClearAll.Size = new System.Drawing.Size(80, 23);
            this.b_IClearAll.TabIndex = 27;
            this.b_IClearAll.Text = "Clear All";
            this.b_IClearAll.UseVisualStyleBackColor = true;
            this.b_IClearAll.Click += new System.EventHandler(this.b_IClearAll_Click);
            this.b_IClearAll.MouseEnter += new System.EventHandler(this.b_IClearAll_MouseEnter);
            this.b_IClearAll.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // b_IRemoveFiles
            // 
            this.b_IRemoveFiles.Location = new System.Drawing.Point(3, 128);
            this.b_IRemoveFiles.Name = "b_IRemoveFiles";
            this.b_IRemoveFiles.Size = new System.Drawing.Size(80, 23);
            this.b_IRemoveFiles.TabIndex = 26;
            this.b_IRemoveFiles.Text = "Remove File";
            this.b_IRemoveFiles.UseVisualStyleBackColor = true;
            this.b_IRemoveFiles.Click += new System.EventHandler(this.b_IRemoveFiles_Click);
            this.b_IRemoveFiles.MouseEnter += new System.EventHandler(this.b_IRemoveFiles_MouseEnter);
            this.b_IRemoveFiles.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // b_IInstallFiles
            // 
            this.b_IInstallFiles.Location = new System.Drawing.Point(55, 234);
            this.b_IInstallFiles.Name = "b_IInstallFiles";
            this.b_IInstallFiles.Size = new System.Drawing.Size(80, 23);
            this.b_IInstallFiles.TabIndex = 28;
            this.b_IInstallFiles.Text = "Install Files";
            this.b_IInstallFiles.UseVisualStyleBackColor = true;
            this.b_IInstallFiles.Visible = false;
            this.b_IInstallFiles.Click += new System.EventHandler(this.b_IInstallFiles_Click);
            this.b_IInstallFiles.MouseEnter += new System.EventHandler(this.b_IInstallFiles_MouseEnter);
            this.b_IInstallFiles.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(0, 0);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 0;
            // 
            // resetLoadScreenBox
            // 
            this.resetLoadScreenBox.Location = new System.Drawing.Point(0, 0);
            this.resetLoadScreenBox.Name = "resetLoadScreenBox";
            this.resetLoadScreenBox.Size = new System.Drawing.Size(75, 23);
            this.resetLoadScreenBox.TabIndex = 0;
            // 
            // resetSkinBox
            // 
            this.resetSkinBox.Location = new System.Drawing.Point(0, 0);
            this.resetSkinBox.Name = "resetSkinBox";
            this.resetSkinBox.Size = new System.Drawing.Size(75, 23);
            this.resetSkinBox.TabIndex = 0;
            // 
            // loadScreenLbl
            // 
            this.loadScreenLbl.Location = new System.Drawing.Point(0, 0);
            this.loadScreenLbl.Name = "loadScreenLbl";
            this.loadScreenLbl.Size = new System.Drawing.Size(100, 23);
            this.loadScreenLbl.TabIndex = 0;
            // 
            // browseLoadScreen
            // 
            this.browseLoadScreen.Location = new System.Drawing.Point(0, 0);
            this.browseLoadScreen.Name = "browseLoadScreen";
            this.browseLoadScreen.Size = new System.Drawing.Size(75, 23);
            this.browseLoadScreen.TabIndex = 0;
            // 
            // loadScreenImageTextBox
            // 
            this.loadScreenImageTextBox.Location = new System.Drawing.Point(0, 0);
            this.loadScreenImageTextBox.Name = "loadScreenImageTextBox";
            this.loadScreenImageTextBox.Size = new System.Drawing.Size(100, 26);
            this.loadScreenImageTextBox.TabIndex = 0;
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Location = new System.Drawing.Point(119, 80);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.ReadOnly = true;
            this.fileNameTextBox.Size = new System.Drawing.Size(225, 26);
            this.fileNameTextBox.TabIndex = 39;
            this.fileNameTextBox.WordWrap = false;
            this.fileNameTextBox.MouseEnter += new System.EventHandler(this.fileNameTextBox_MouseEnter);
            this.fileNameTextBox.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // reset
            // 
            this.reset.Location = new System.Drawing.Point(0, 0);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(75, 23);
            this.reset.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 0;
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(0, 0);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(75, 23);
            this.browse.TabIndex = 0;
            // 
            // skinVariant
            // 
            this.skinVariant.Location = new System.Drawing.Point(0, 0);
            this.skinVariant.Name = "skinVariant";
            this.skinVariant.Size = new System.Drawing.Size(121, 28);
            this.skinVariant.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            // 
            // changeSkinButton
            // 
            this.changeSkinButton.Location = new System.Drawing.Point(0, 0);
            this.changeSkinButton.Name = "changeSkinButton";
            this.changeSkinButton.Size = new System.Drawing.Size(75, 23);
            this.changeSkinButton.TabIndex = 0;
            // 
            // selectchampiontxt
            // 
            this.selectchampiontxt.Location = new System.Drawing.Point(0, 0);
            this.selectchampiontxt.Name = "selectchampiontxt";
            this.selectchampiontxt.Size = new System.Drawing.Size(100, 23);
            this.selectchampiontxt.TabIndex = 0;
            // 
            // selectChampion
            // 
            this.selectChampion.Location = new System.Drawing.Point(0, 0);
            this.selectChampion.Name = "selectChampion";
            this.selectChampion.Size = new System.Drawing.Size(121, 28);
            this.selectChampion.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(728, 335);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragDrop);
            this.tabControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragEnter);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer2);
            this.tabPage4.Controls.Add(this.panel1);
            this.tabPage4.Location = new System.Drawing.Point(4, 29);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(720, 302);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "==Install Existing Skin==";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listView1);
            this.splitContainer2.Panel1.Controls.Add(this.panel9);
            this.splitContainer2.Panel1.Controls.Add(this.panel8);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(714, 265);
            this.splitContainer2.SplitterDistance = 522;
            this.splitContainer2.TabIndex = 7;
            // 
            // listView1
            // 
            this.listView1.AutoArrange = false;
            this.listView1.BackColor = System.Drawing.SystemColors.Window;
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader5,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader6,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader7});
            this.listView1.ContextMenuStrip = this.dataBaseListMenuStrip1;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.GridLines = true;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(18, 15);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(504, 250);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 0;
            this.listView1.TileSize = new System.Drawing.Size(2, 2);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemHover += new SkinInstaller.ListViewItemHover.ItemHoverEventHandler(this.listView1_ItemMouseHover);
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = " ";
            this.columnHeader1.Width = 43;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Skin Title";
            this.columnHeader2.Width = 190;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Author";
            this.columnHeader5.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "File Count";
            this.columnHeader3.Width = 69;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Installed";
            this.columnHeader4.Width = 53;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Added";
            this.columnHeader6.Width = 67;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Date and Time Added";
            this.columnHeader8.Width = 0;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Date and Time Installed";
            this.columnHeader9.Width = 0;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Character";
            this.columnHeader7.Width = 90;
            // 
            // dataBaseListMenuStrip1
            // 
            this.dataBaseListMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.dataBaseListMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSelectUninstalled,
            this.toolStripSelectAllInstalled,
            this.selectAllSkinsToolStripMenuItem,
            this.deselectAllSkinsToolStripMenuItem,
            this.editThisSkinToolStripMenuItem,
            this.previewThisSkinToolStripMenuItem});
            this.dataBaseListMenuStrip1.Name = "contextMenuStrip1";
            this.dataBaseListMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.dataBaseListMenuStrip1.Size = new System.Drawing.Size(294, 184);
            this.dataBaseListMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            this.dataBaseListMenuStrip1.Opened += new System.EventHandler(this.contextMenuStrip1_Opened);
            // 
            // toolStripSelectUninstalled
            // 
            this.toolStripSelectUninstalled.AutoToolTip = true;
            this.toolStripSelectUninstalled.Name = "toolStripSelectUninstalled";
            this.toolStripSelectUninstalled.Size = new System.Drawing.Size(293, 30);
            this.toolStripSelectUninstalled.Text = "Select All Uninstalled Skins";
            this.toolStripSelectUninstalled.Click += new System.EventHandler(this.toolStripSelectUninstalled_Click);
            // 
            // toolStripSelectAllInstalled
            // 
            this.toolStripSelectAllInstalled.Name = "toolStripSelectAllInstalled";
            this.toolStripSelectAllInstalled.Size = new System.Drawing.Size(293, 30);
            this.toolStripSelectAllInstalled.Text = "Select All Installed Skins";
            this.toolStripSelectAllInstalled.Click += new System.EventHandler(this.toolStripSelectAllInstalled_Click);
            // 
            // selectAllSkinsToolStripMenuItem
            // 
            this.selectAllSkinsToolStripMenuItem.Name = "selectAllSkinsToolStripMenuItem";
            this.selectAllSkinsToolStripMenuItem.Size = new System.Drawing.Size(293, 30);
            this.selectAllSkinsToolStripMenuItem.Text = "Select All Skins";
            this.selectAllSkinsToolStripMenuItem.Click += new System.EventHandler(this.selectAllSkinsToolStripMenuItem_Click);
            // 
            // deselectAllSkinsToolStripMenuItem
            // 
            this.deselectAllSkinsToolStripMenuItem.Name = "deselectAllSkinsToolStripMenuItem";
            this.deselectAllSkinsToolStripMenuItem.Size = new System.Drawing.Size(293, 30);
            this.deselectAllSkinsToolStripMenuItem.Text = "Deselect All Skins";
            this.deselectAllSkinsToolStripMenuItem.Click += new System.EventHandler(this.deselectAllSkinsToolStripMenuItem_Click);
            // 
            // editThisSkinToolStripMenuItem
            // 
            this.editThisSkinToolStripMenuItem.Name = "editThisSkinToolStripMenuItem";
            this.editThisSkinToolStripMenuItem.Size = new System.Drawing.Size(293, 30);
            this.editThisSkinToolStripMenuItem.Text = "* Edit This Skin *";
            this.editThisSkinToolStripMenuItem.Click += new System.EventHandler(this.editThisSkinToolStripMenuItem_Click);
            // 
            // previewThisSkinToolStripMenuItem
            // 
            this.previewThisSkinToolStripMenuItem.Name = "previewThisSkinToolStripMenuItem";
            this.previewThisSkinToolStripMenuItem.Size = new System.Drawing.Size(293, 30);
            this.previewThisSkinToolStripMenuItem.Text = "* Preview This Skin *";
            this.previewThisSkinToolStripMenuItem.Click += new System.EventHandler(this.previewThisSkinToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.colorSlider1);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel9.Location = new System.Drawing.Point(0, 15);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(18, 250);
            this.panel9.TabIndex = 2;
            // 
            // colorSlider1
            // 
            this.colorSlider1.BackColor = System.Drawing.Color.Transparent;
            this.colorSlider1.BarInnerColor = System.Drawing.Color.Red;
            this.colorSlider1.BarOuterColor = System.Drawing.Color.Black;
            this.colorSlider1.BarPenColor = System.Drawing.Color.Black;
            this.colorSlider1.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.colorSlider1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorSlider1.ElapsedInnerColor = System.Drawing.Color.Black;
            this.colorSlider1.ElapsedOuterColor = System.Drawing.Color.DarkRed;
            this.colorSlider1.LargeChange = ((uint)(5u));
            this.colorSlider1.Location = new System.Drawing.Point(0, 0);
            this.colorSlider1.MouseEffects = false;
            this.colorSlider1.Name = "colorSlider1";
            this.colorSlider1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.colorSlider1.Size = new System.Drawing.Size(18, 250);
            this.colorSlider1.SmallChange = ((uint)(1u));
            this.colorSlider1.TabIndex = 0;
            this.colorSlider1.Text = "colorSlider1";
            this.colorSlider1.ThumbInnerColor = System.Drawing.Color.OrangeRed;
            this.colorSlider1.ThumbOuterColor = System.Drawing.Color.Gold;
            this.colorSlider1.ThumbPenColor = System.Drawing.Color.Black;
            this.colorSlider1.ThumbRoundRectSize = new System.Drawing.Size(4, 14);
            this.colorSlider1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.colorSlider1_Scroll);
            this.colorSlider1.MouseEnter += new System.EventHandler(this.colorSlider1_MouseEnter);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.checkBox1dispCharacter);
            this.panel8.Controls.Add(this.checkBox1dispDateInstalledFull);
            this.panel8.Controls.Add(this.checkBox1dispDateAddedFull);
            this.panel8.Controls.Add(this.checkBox1dispDateAdded);
            this.panel8.Controls.Add(this.checkBox1dispInstalled);
            this.panel8.Controls.Add(this.checkBox1dispFileCount);
            this.panel8.Controls.Add(this.checkBox1dispAuthor);
            this.panel8.Controls.Add(this.checkBox1dispTitle);
            this.panel8.Controls.Add(this.label9);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(522, 15);
            this.panel8.TabIndex = 1;
            // 
            // checkBox1dispCharacter
            // 
            this.checkBox1dispCharacter.AutoSize = true;
            this.checkBox1dispCharacter.Checked = true;
            this.checkBox1dispCharacter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1dispCharacter.Location = new System.Drawing.Point(507, 1);
            this.checkBox1dispCharacter.Name = "checkBox1dispCharacter";
            this.checkBox1dispCharacter.Size = new System.Drawing.Size(105, 24);
            this.checkBox1dispCharacter.TabIndex = 8;
            this.checkBox1dispCharacter.Text = "Character";
            this.checkBox1dispCharacter.UseVisualStyleBackColor = true;
            this.checkBox1dispCharacter.CheckedChanged += new System.EventHandler(this.dispCheckChanged);
            // 
            // checkBox1dispDateInstalledFull
            // 
            this.checkBox1dispDateInstalledFull.AutoSize = true;
            this.checkBox1dispDateInstalledFull.Checked = true;
            this.checkBox1dispDateInstalledFull.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1dispDateInstalledFull.Location = new System.Drawing.Point(420, 1);
            this.checkBox1dispDateInstalledFull.Name = "checkBox1dispDateInstalledFull";
            this.checkBox1dispDateInstalledFull.Size = new System.Drawing.Size(133, 24);
            this.checkBox1dispDateInstalledFull.TabIndex = 7;
            this.checkBox1dispDateInstalledFull.Text = "Time Installed";
            this.checkBox1dispDateInstalledFull.UseVisualStyleBackColor = true;
            this.checkBox1dispDateInstalledFull.CheckedChanged += new System.EventHandler(this.dispCheckChanged);
            // 
            // checkBox1dispDateAddedFull
            // 
            this.checkBox1dispDateAddedFull.AutoSize = true;
            this.checkBox1dispDateAddedFull.Checked = true;
            this.checkBox1dispDateAddedFull.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1dispDateAddedFull.Location = new System.Drawing.Point(342, 1);
            this.checkBox1dispDateAddedFull.Name = "checkBox1dispDateAddedFull";
            this.checkBox1dispDateAddedFull.Size = new System.Drawing.Size(120, 24);
            this.checkBox1dispDateAddedFull.TabIndex = 6;
            this.checkBox1dispDateAddedFull.Text = "Time Added";
            this.checkBox1dispDateAddedFull.UseVisualStyleBackColor = true;
            this.checkBox1dispDateAddedFull.CheckedChanged += new System.EventHandler(this.dispCheckChanged);
            // 
            // checkBox1dispDateAdded
            // 
            this.checkBox1dispDateAdded.AutoSize = true;
            this.checkBox1dispDateAdded.Checked = true;
            this.checkBox1dispDateAdded.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1dispDateAdded.Location = new System.Drawing.Point(265, 1);
            this.checkBox1dispDateAdded.Name = "checkBox1dispDateAdded";
            this.checkBox1dispDateAdded.Size = new System.Drawing.Size(121, 24);
            this.checkBox1dispDateAdded.TabIndex = 5;
            this.checkBox1dispDateAdded.Text = "Date Added";
            this.checkBox1dispDateAdded.UseVisualStyleBackColor = true;
            this.checkBox1dispDateAdded.CheckedChanged += new System.EventHandler(this.dispCheckChanged);
            // 
            // checkBox1dispInstalled
            // 
            this.checkBox1dispInstalled.AutoSize = true;
            this.checkBox1dispInstalled.Checked = true;
            this.checkBox1dispInstalled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1dispInstalled.Location = new System.Drawing.Point(204, 1);
            this.checkBox1dispInstalled.Name = "checkBox1dispInstalled";
            this.checkBox1dispInstalled.Size = new System.Drawing.Size(95, 24);
            this.checkBox1dispInstalled.TabIndex = 4;
            this.checkBox1dispInstalled.Text = "Installed";
            this.checkBox1dispInstalled.UseVisualStyleBackColor = true;
            this.checkBox1dispInstalled.CheckedChanged += new System.EventHandler(this.dispCheckChanged);
            // 
            // checkBox1dispFileCount
            // 
            this.checkBox1dispFileCount.AutoSize = true;
            this.checkBox1dispFileCount.Checked = true;
            this.checkBox1dispFileCount.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1dispFileCount.Location = new System.Drawing.Point(134, 1);
            this.checkBox1dispFileCount.Name = "checkBox1dispFileCount";
            this.checkBox1dispFileCount.Size = new System.Drawing.Size(107, 24);
            this.checkBox1dispFileCount.TabIndex = 3;
            this.checkBox1dispFileCount.Text = "File Count";
            this.checkBox1dispFileCount.UseVisualStyleBackColor = true;
            this.checkBox1dispFileCount.CheckedChanged += new System.EventHandler(this.dispCheckChanged);
            // 
            // checkBox1dispAuthor
            // 
            this.checkBox1dispAuthor.AutoSize = true;
            this.checkBox1dispAuthor.Checked = true;
            this.checkBox1dispAuthor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1dispAuthor.Location = new System.Drawing.Point(78, 1);
            this.checkBox1dispAuthor.Name = "checkBox1dispAuthor";
            this.checkBox1dispAuthor.Size = new System.Drawing.Size(83, 24);
            this.checkBox1dispAuthor.TabIndex = 2;
            this.checkBox1dispAuthor.Text = "Author";
            this.checkBox1dispAuthor.UseVisualStyleBackColor = true;
            this.checkBox1dispAuthor.CheckedChanged += new System.EventHandler(this.dispCheckChanged);
            // 
            // checkBox1dispTitle
            // 
            this.checkBox1dispTitle.AutoSize = true;
            this.checkBox1dispTitle.Checked = true;
            this.checkBox1dispTitle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1dispTitle.Location = new System.Drawing.Point(36, 1);
            this.checkBox1dispTitle.Name = "checkBox1dispTitle";
            this.checkBox1dispTitle.Size = new System.Drawing.Size(64, 24);
            this.checkBox1dispTitle.TabIndex = 1;
            this.checkBox1dispTitle.Text = "Title";
            this.checkBox1dispTitle.UseVisualStyleBackColor = true;
            this.checkBox1dispTitle.CheckedChanged += new System.EventHandler(this.dispCheckChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 1);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "Show:";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.textBox2);
            this.splitContainer3.Panel1.Controls.Add(this.labelSkinName);
            this.splitContainer3.Panel1.Cursor = System.Windows.Forms.Cursors.Cross;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pictureBox2);
            this.splitContainer3.Size = new System.Drawing.Size(188, 265);
            this.splitContainer3.SplitterDistance = 83;
            this.splitContainer3.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Cursor = System.Windows.Forms.Cursors.Cross;
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(0, 31);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(188, 52);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // labelSkinName
            // 
            this.labelSkinName.AutoSize = true;
            this.labelSkinName.Cursor = System.Windows.Forms.Cursors.Cross;
            this.labelSkinName.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelSkinName.Font = new System.Drawing.Font("Palatino Linotype", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSkinName.Location = new System.Drawing.Point(0, 0);
            this.labelSkinName.Name = "labelSkinName";
            this.labelSkinName.Size = new System.Drawing.Size(148, 31);
            this.labelSkinName.TabIndex = 0;
            this.labelSkinName.Text = "Click a Skin!";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.ErrorImage = null;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(188, 178);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3openAds3);
            this.panel1.Controls.Add(this.button3reinstallText);
            this.panel1.Controls.Add(this.dbDelete);
            this.panel1.Controls.Add(this.dbInstall);
            this.panel1.Controls.Add(this.dbUninstall);
            this.panel1.Controls.Add(this.createZip);
            this.panel1.Controls.Add(this.button3repath);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 268);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(714, 31);
            this.panel1.TabIndex = 6;
            // 
            // button3openAds3
            // 
            this.button3openAds3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3openAds3.Location = new System.Drawing.Point(680, 5);
            this.button3openAds3.Name = "button3openAds3";
            this.button3openAds3.Size = new System.Drawing.Size(31, 23);
            this.button3openAds3.TabIndex = 40;
            this.button3openAds3.Text = "<<";
            this.button3openAds3.UseVisualStyleBackColor = true;
            this.button3openAds3.Click += new System.EventHandler(this.button3openAd_Click);
            // 
            // button3reinstallText
            // 
            this.button3reinstallText.Location = new System.Drawing.Point(166, 3);
            this.button3reinstallText.Name = "button3reinstallText";
            this.button3reinstallText.Size = new System.Drawing.Size(117, 23);
            this.button3reinstallText.TabIndex = 6;
            this.button3reinstallText.Text = "Re-install Text Mods";
            this.button3reinstallText.UseVisualStyleBackColor = true;
            this.button3reinstallText.Click += new System.EventHandler(this.button3reinstallText_Click);
            // 
            // dbDelete
            // 
            this.dbDelete.Location = new System.Drawing.Point(498, 3);
            this.dbDelete.Name = "dbDelete";
            this.dbDelete.Size = new System.Drawing.Size(75, 23);
            this.dbDelete.TabIndex = 3;
            this.dbDelete.Text = "Delete";
            this.dbDelete.UseVisualStyleBackColor = true;
            this.dbDelete.Click += new System.EventHandler(this.dbDelete_Click);
            this.dbDelete.MouseEnter += new System.EventHandler(this.dbDelete_MouseEnter);
            this.dbDelete.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // dbInstall
            // 
            this.dbInstall.Location = new System.Drawing.Point(6, 3);
            this.dbInstall.Name = "dbInstall";
            this.dbInstall.Size = new System.Drawing.Size(75, 23);
            this.dbInstall.TabIndex = 1;
            this.dbInstall.Text = "Install";
            this.dbInstall.UseVisualStyleBackColor = true;
            this.dbInstall.Click += new System.EventHandler(this.dbInstall_Click);
            this.dbInstall.MouseEnter += new System.EventHandler(this.dbInstall_MouseEnter);
            this.dbInstall.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // dbUninstall
            // 
            this.dbUninstall.Location = new System.Drawing.Point(85, 3);
            this.dbUninstall.Name = "dbUninstall";
            this.dbUninstall.Size = new System.Drawing.Size(75, 23);
            this.dbUninstall.TabIndex = 2;
            this.dbUninstall.Text = "Uninstall";
            this.dbUninstall.UseVisualStyleBackColor = true;
            this.dbUninstall.Click += new System.EventHandler(this.dbUninstall_Click);
            this.dbUninstall.MouseEnter += new System.EventHandler(this.dbUninstall_MouseEnter);
            this.dbUninstall.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // createZip
            // 
            this.createZip.Location = new System.Drawing.Point(289, 3);
            this.createZip.Name = "createZip";
            this.createZip.Size = new System.Drawing.Size(75, 23);
            this.createZip.TabIndex = 4;
            this.createZip.Text = "Create Zip";
            this.createZip.UseVisualStyleBackColor = true;
            this.createZip.Click += new System.EventHandler(this.createZip_Click);
            this.createZip.MouseEnter += new System.EventHandler(this.createZip_MouseEnter);
            this.createZip.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // button3repath
            // 
            this.button3repath.Location = new System.Drawing.Point(378, 3);
            this.button3repath.Name = "button3repath";
            this.button3repath.Size = new System.Drawing.Size(114, 23);
            this.button3repath.TabIndex = 5;
            this.button3repath.Text = "Fix Skin (Re-path)";
            this.button3repath.UseVisualStyleBackColor = true;
            this.button3repath.Click += new System.EventHandler(this.button3repath_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer4);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(720, 302);
            this.tabPage3.TabIndex = 5;
            this.tabPage3.Text = "==Skin Creation==";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(3, 3);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.splitContainer5);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.button3openAds2);
            this.splitContainer4.Panel2.Controls.Add(this.label10);
            this.splitContainer4.Panel2.Controls.Add(this.button3exporttree);
            this.splitContainer4.Panel2.Controls.Add(this.buttonRebuildTree);
            this.splitContainer4.Size = new System.Drawing.Size(714, 296);
            this.splitContainer4.SplitterDistance = 267;
            this.splitContainer4.TabIndex = 0;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.treeView1);
            this.splitContainer5.Size = new System.Drawing.Size(714, 267);
            this.splitContainer5.SplitterDistance = 25;
            this.splitContainer5.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.Black;
            this.treeView1.CheckBoxes = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ForeColor = System.Drawing.Color.Red;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HotTracking = true;
            this.treeView1.ItemHeight = 16;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode2.Name = "Please Wait";
            treeNode2.Text = "Please wait for the progress bar to finish loading bellow...";
            treeNode2.ToolTipText = "Please wait...";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(685, 267);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCheck);
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
            // 
            // button3openAds2
            // 
            this.button3openAds2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3openAds2.Location = new System.Drawing.Point(680, -1);
            this.button3openAds2.Name = "button3openAds2";
            this.button3openAds2.Size = new System.Drawing.Size(31, 23);
            this.button3openAds2.TabIndex = 40;
            this.button3openAds2.Text = "<<";
            this.button3openAds2.UseVisualStyleBackColor = true;
            this.button3openAds2.Click += new System.EventHandler(this.button3openAd_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(272, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(374, 20);
            this.label10.TabIndex = 2;
            this.label10.Text = "Items are colored by version, green is new, red is old";
            // 
            // button3exporttree
            // 
            this.button3exporttree.Location = new System.Drawing.Point(27, 2);
            this.button3exporttree.Name = "button3exporttree";
            this.button3exporttree.Size = new System.Drawing.Size(239, 23);
            this.button3exporttree.TabIndex = 1;
            this.button3exporttree.Text = "Export Checked Files to Computer Directory";
            this.button3exporttree.UseVisualStyleBackColor = true;
            this.button3exporttree.Click += new System.EventHandler(this.button3exporttree_Click);
            // 
            // buttonRebuildTree
            // 
            this.buttonRebuildTree.Location = new System.Drawing.Point(288, 2);
            this.buttonRebuildTree.Name = "buttonRebuildTree";
            this.buttonRebuildTree.Size = new System.Drawing.Size(119, 23);
            this.buttonRebuildTree.TabIndex = 0;
            this.buttonRebuildTree.Text = "Rebuild Tree View";
            this.buttonRebuildTree.UseVisualStyleBackColor = true;
            this.buttonRebuildTree.Visible = false;
            this.buttonRebuildTree.Click += new System.EventHandler(this.buttonRebuildTree_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(720, 302);
            this.tabPage1.TabIndex = 4;
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Panel2.Controls.Add(this.panel6);
            this.splitContainer1.Size = new System.Drawing.Size(714, 296);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(232, 296);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(478, 196);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Nothing To See Here";
            // 
            // panel6
            // 
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 196);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(478, 100);
            this.panel6.TabIndex = 0;
            // 
            // treeViewMenuStrip1
            // 
            this.treeViewMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.treeViewMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportSelectedFilesToolStripMenuItem,
            this.deselectAllFilesToolStripMenuItem,
            this.helpToolStripMenuItem1,
            this.lookUpReleaseManifestInfoToolStripMenuItem});
            this.treeViewMenuStrip1.Name = "treeViewMenuStrip1";
            this.treeViewMenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.treeViewMenuStrip1.Size = new System.Drawing.Size(320, 124);
            // 
            // exportSelectedFilesToolStripMenuItem
            // 
            this.exportSelectedFilesToolStripMenuItem.Name = "exportSelectedFilesToolStripMenuItem";
            this.exportSelectedFilesToolStripMenuItem.Size = new System.Drawing.Size(319, 30);
            this.exportSelectedFilesToolStripMenuItem.Text = "Export Selected Files";
            this.exportSelectedFilesToolStripMenuItem.Click += new System.EventHandler(this.exportSelectedFilesToolStripMenuItem_Click);
            // 
            // deselectAllFilesToolStripMenuItem
            // 
            this.deselectAllFilesToolStripMenuItem.Name = "deselectAllFilesToolStripMenuItem";
            this.deselectAllFilesToolStripMenuItem.Size = new System.Drawing.Size(319, 30);
            this.deselectAllFilesToolStripMenuItem.Text = "Deselect All Files";
            this.deselectAllFilesToolStripMenuItem.Click += new System.EventHandler(this.deselectAllFilesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(319, 30);
            this.helpToolStripMenuItem1.Text = "Help!";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // lookUpReleaseManifestInfoToolStripMenuItem
            // 
            this.lookUpReleaseManifestInfoToolStripMenuItem.Name = "lookUpReleaseManifestInfoToolStripMenuItem";
            this.lookUpReleaseManifestInfoToolStripMenuItem.Size = new System.Drawing.Size(319, 30);
            this.lookUpReleaseManifestInfoToolStripMenuItem.Text = "Look Up ReleaseManifest Info";
            this.lookUpReleaseManifestInfoToolStripMenuItem.Click += new System.EventHandler(this.lookUpReleaseManifestInfoToolStripMenuItem_Click);
            // 
            // UpdateFL
            // 
            this.UpdateFL.Location = new System.Drawing.Point(90, 3);
            this.UpdateFL.Name = "UpdateFL";
            this.UpdateFL.Size = new System.Drawing.Size(120, 23);
            this.UpdateFL.TabIndex = 35;
            this.UpdateFL.Text = "Re-Read LoL Files";
            this.UpdateFL.UseVisualStyleBackColor = true;
            this.UpdateFL.Click += new System.EventHandler(this.UpdateFL_Click);
            this.UpdateFL.MouseEnter += new System.EventHandler(this.Update_MouseEnter);
            this.UpdateFL.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // locateGameClient
            // 
            this.locateGameClient.Location = new System.Drawing.Point(9, 3);
            this.locateGameClient.Name = "locateGameClient";
            this.locateGameClient.Size = new System.Drawing.Size(75, 23);
            this.locateGameClient.TabIndex = 36;
            this.locateGameClient.Text = "Game Client";
            this.locateGameClient.UseVisualStyleBackColor = true;
            this.locateGameClient.Click += new System.EventHandler(this.locateGameClient_Click);
            this.locateGameClient.MouseEnter += new System.EventHandler(this.locateGameClient_MouseEnter);
            this.locateGameClient.MouseLeave += new System.EventHandler(this.NoToolTip_MouseLeave);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(836, 22);
            this.progressBar1.TabIndex = 40;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(767, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(44, 23);
            this.button2.TabIndex = 41;
            this.button2.Text = "Help";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(886, 33);
            this.menuStrip1.TabIndex = 42;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(74, 29);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editInstallPreferencesToolStripMenuItem,
            this.changeGameClientLocationToolStripMenuItem,
            this.updateFileListToolStripMenuItem,
            this.editAllPreferencesToolStripMenuItem,
            this.checkForUpdateToolStripMenuItem,
            this.registerAppForWebUrlsToolStripMenuItem,
            this.createDesktopShortcutToolStripMenuItem,
            this.fixRiotWarningsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // editInstallPreferencesToolStripMenuItem
            // 
            this.editInstallPreferencesToolStripMenuItem.Name = "editInstallPreferencesToolStripMenuItem";
            this.editInstallPreferencesToolStripMenuItem.Size = new System.Drawing.Size(328, 30);
            this.editInstallPreferencesToolStripMenuItem.Text = "Edit Install Preferences";
            this.editInstallPreferencesToolStripMenuItem.Click += new System.EventHandler(this.editInstallPreferencesToolStripMenuItem_Click);
            // 
            // changeGameClientLocationToolStripMenuItem
            // 
            this.changeGameClientLocationToolStripMenuItem.Name = "changeGameClientLocationToolStripMenuItem";
            this.changeGameClientLocationToolStripMenuItem.Size = new System.Drawing.Size(328, 30);
            this.changeGameClientLocationToolStripMenuItem.Text = "Change Game Client Location";
            this.changeGameClientLocationToolStripMenuItem.Click += new System.EventHandler(this.changeGameClientLocationToolStripMenuItem_Click);
            // 
            // updateFileListToolStripMenuItem
            // 
            this.updateFileListToolStripMenuItem.Name = "updateFileListToolStripMenuItem";
            this.updateFileListToolStripMenuItem.Size = new System.Drawing.Size(328, 30);
            this.updateFileListToolStripMenuItem.Text = "Update File List";
            this.updateFileListToolStripMenuItem.Click += new System.EventHandler(this.updateFileListToolStripMenuItem_Click);
            // 
            // editAllPreferencesToolStripMenuItem
            // 
            this.editAllPreferencesToolStripMenuItem.Name = "editAllPreferencesToolStripMenuItem";
            this.editAllPreferencesToolStripMenuItem.Size = new System.Drawing.Size(328, 30);
            this.editAllPreferencesToolStripMenuItem.Text = "Edit All Preferences";
            this.editAllPreferencesToolStripMenuItem.Click += new System.EventHandler(this.editAllPreferencesToolStripMenuItem_Click);
            // 
            // checkForUpdateToolStripMenuItem
            // 
            this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            this.checkForUpdateToolStripMenuItem.Size = new System.Drawing.Size(328, 30);
            this.checkForUpdateToolStripMenuItem.Text = "Check For Update";
            this.checkForUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdateToolStripMenuItem_Click);
            // 
            // registerAppForWebUrlsToolStripMenuItem
            // 
            this.registerAppForWebUrlsToolStripMenuItem.Name = "registerAppForWebUrlsToolStripMenuItem";
            this.registerAppForWebUrlsToolStripMenuItem.Size = new System.Drawing.Size(328, 30);
            this.registerAppForWebUrlsToolStripMenuItem.Text = "Register App For WebUrls";
            this.registerAppForWebUrlsToolStripMenuItem.Click += new System.EventHandler(this.registerAppForWebUrlsToolStripMenuItem_Click);
            // 
            // createDesktopShortcutToolStripMenuItem
            // 
            this.createDesktopShortcutToolStripMenuItem.Name = "createDesktopShortcutToolStripMenuItem";
            this.createDesktopShortcutToolStripMenuItem.Size = new System.Drawing.Size(328, 30);
            this.createDesktopShortcutToolStripMenuItem.Text = "Create Desktop Shortcut";
            this.createDesktopShortcutToolStripMenuItem.Click += new System.EventHandler(this.createDesktopShortcutToolStripMenuItem_Click);
            // 
            // fixRiotWarningsToolStripMenuItem
            // 
            this.fixRiotWarningsToolStripMenuItem.Name = "fixRiotWarningsToolStripMenuItem";
            this.fixRiotWarningsToolStripMenuItem.Size = new System.Drawing.Size(328, 30);
            this.fixRiotWarningsToolStripMenuItem.Text = "Fix Riot Corruption Warnings";
            this.fixRiotWarningsToolStripMenuItem.Click += new System.EventHandler(this.fixRiotWarningsToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moreDebugToolStripMenuItem,
            this.soundFileLocationToolStripMenuItem,
            this.deleteBackupsToolStripMenuItem,
            this.repathAllFilesToolStripMenuItem,
            this.showDebugToolStripMenuItem,
            this.imagesToolStripMenuItem,
            this.iAmLGGToolStripMenuItem,
            this.iCantStandLGGToolStripMenuItem,
            this.pingToolStripMenuItem,
            this.viewStatsToolStripMenuItem,
            this.wtfRainbowsToolStripMenuItem,
            this.setSoundFileLocationToolStripMenuItem,
            this.getProgramLocationToolStripMenuItem,
            this.resetCharacterIconsCacheToolStripMenuItem,
            this.useDDSVersionReaderToolStripMenuItem,
            this.testReadResFilesToolStripMenuItem,
            this.unpackSoundsToolStripMenuItem,
            this.loLViewerOpenNotPreviewToolStripMenuItem,
            this.openParticleReferenceToolStripMenuItem,
            this.showMenuFileLocationToolStripMenuItem,
            this.openTextTreeEditorToolStripMenuItem,
            this.openPublisherToolStripMenuItem,
            this.skinHelpersToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(78, 29);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // moreDebugToolStripMenuItem
            // 
            this.moreDebugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getVersionFilePathToolStripMenuItem,
            this.readVersionsToolStripMenuItem,
            this.clientLocationToolStripMenuItem,
            this.getLastModDateToolStripMenuItem,
            this.viewDXTVersionsToolStripMenuItem,
            this.associateFilesToolStripMenuItem});
            this.moreDebugToolStripMenuItem.Name = "moreDebugToolStripMenuItem";
            this.moreDebugToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.moreDebugToolStripMenuItem.Text = "More Debug";
            // 
            // getVersionFilePathToolStripMenuItem
            // 
            this.getVersionFilePathToolStripMenuItem.Name = "getVersionFilePathToolStripMenuItem";
            this.getVersionFilePathToolStripMenuItem.Size = new System.Drawing.Size(256, 30);
            this.getVersionFilePathToolStripMenuItem.Text = "Get Version File Path";
            this.getVersionFilePathToolStripMenuItem.Click += new System.EventHandler(this.getVersionFilePathToolStripMenuItem_Click);
            // 
            // readVersionsToolStripMenuItem
            // 
            this.readVersionsToolStripMenuItem.Name = "readVersionsToolStripMenuItem";
            this.readVersionsToolStripMenuItem.Size = new System.Drawing.Size(256, 30);
            this.readVersionsToolStripMenuItem.Text = "Read Versions";
            this.readVersionsToolStripMenuItem.Click += new System.EventHandler(this.readVersionsToolStripMenuItem_Click);
            // 
            // clientLocationToolStripMenuItem
            // 
            this.clientLocationToolStripMenuItem.Name = "clientLocationToolStripMenuItem";
            this.clientLocationToolStripMenuItem.Size = new System.Drawing.Size(256, 30);
            this.clientLocationToolStripMenuItem.Text = "Client Location";
            this.clientLocationToolStripMenuItem.Click += new System.EventHandler(this.clientLocationToolStripMenuItem_Click);
            // 
            // getLastModDateToolStripMenuItem
            // 
            this.getLastModDateToolStripMenuItem.Name = "getLastModDateToolStripMenuItem";
            this.getLastModDateToolStripMenuItem.Size = new System.Drawing.Size(256, 30);
            this.getLastModDateToolStripMenuItem.Text = "Get Last Mod Date";
            this.getLastModDateToolStripMenuItem.Click += new System.EventHandler(this.getLastModDateToolStripMenuItem_Click);
            // 
            // viewDXTVersionsToolStripMenuItem
            // 
            this.viewDXTVersionsToolStripMenuItem.Name = "viewDXTVersionsToolStripMenuItem";
            this.viewDXTVersionsToolStripMenuItem.Size = new System.Drawing.Size(256, 30);
            this.viewDXTVersionsToolStripMenuItem.Text = "View DXT Versions";
            this.viewDXTVersionsToolStripMenuItem.Click += new System.EventHandler(this.viewDXTVersionsToolStripMenuItem_Click);
            // 
            // associateFilesToolStripMenuItem
            // 
            this.associateFilesToolStripMenuItem.Name = "associateFilesToolStripMenuItem";
            this.associateFilesToolStripMenuItem.Size = new System.Drawing.Size(256, 30);
            this.associateFilesToolStripMenuItem.Text = "Associate Files";
            this.associateFilesToolStripMenuItem.Click += new System.EventHandler(this.associateFilesToolStripMenuItem_Click);
            // 
            // soundFileLocationToolStripMenuItem
            // 
            this.soundFileLocationToolStripMenuItem.Name = "soundFileLocationToolStripMenuItem";
            this.soundFileLocationToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.soundFileLocationToolStripMenuItem.Text = "Sound File Location";
            this.soundFileLocationToolStripMenuItem.Click += new System.EventHandler(this.soundFileLocationToolStripMenuItem_Click);
            // 
            // deleteBackupsToolStripMenuItem
            // 
            this.deleteBackupsToolStripMenuItem.Name = "deleteBackupsToolStripMenuItem";
            this.deleteBackupsToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.deleteBackupsToolStripMenuItem.Text = "Delete Backups";
            this.deleteBackupsToolStripMenuItem.Click += new System.EventHandler(this.deleteBackupsToolStripMenuItem_Click);
            // 
            // repathAllFilesToolStripMenuItem
            // 
            this.repathAllFilesToolStripMenuItem.Name = "repathAllFilesToolStripMenuItem";
            this.repathAllFilesToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.repathAllFilesToolStripMenuItem.Text = "Repath All Files";
            this.repathAllFilesToolStripMenuItem.Click += new System.EventHandler(this.repathAllFilesToolStripMenuItem_Click);
            // 
            // showDebugToolStripMenuItem
            // 
            this.showDebugToolStripMenuItem.Name = "showDebugToolStripMenuItem";
            this.showDebugToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.showDebugToolStripMenuItem.Text = "Show Debug";
            this.showDebugToolStripMenuItem.Click += new System.EventHandler(this.showDebugToolStripMenuItem_Click);
            // 
            // imagesToolStripMenuItem
            // 
            this.imagesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.imagesToolStripMenuItem.Name = "imagesToolStripMenuItem";
            this.imagesToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.imagesToolStripMenuItem.Text = "Images";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(135, 30);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(135, 30);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // iAmLGGToolStripMenuItem
            // 
            this.iAmLGGToolStripMenuItem.Name = "iAmLGGToolStripMenuItem";
            this.iAmLGGToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.iAmLGGToolStripMenuItem.Text = "I am LGG";
            this.iAmLGGToolStripMenuItem.Click += new System.EventHandler(this.iAmLGGToolStripMenuItem_Click);
            // 
            // iCantStandLGGToolStripMenuItem
            // 
            this.iCantStandLGGToolStripMenuItem.Name = "iCantStandLGGToolStripMenuItem";
            this.iCantStandLGGToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.iCantStandLGGToolStripMenuItem.Text = "I can\'t stand LGG";
            this.iCantStandLGGToolStripMenuItem.Click += new System.EventHandler(this.iCantStandLGGToolStripMenuItem_Click);
            // 
            // pingToolStripMenuItem
            // 
            this.pingToolStripMenuItem.Name = "pingToolStripMenuItem";
            this.pingToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.pingToolStripMenuItem.Text = "Ping";
            this.pingToolStripMenuItem.Click += new System.EventHandler(this.pingToolStripMenuItem_Click);
            // 
            // viewStatsToolStripMenuItem
            // 
            this.viewStatsToolStripMenuItem.Name = "viewStatsToolStripMenuItem";
            this.viewStatsToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.viewStatsToolStripMenuItem.Text = "View Stats";
            this.viewStatsToolStripMenuItem.Click += new System.EventHandler(this.viewStatsToolStripMenuItem_Click);
            // 
            // wtfRainbowsToolStripMenuItem
            // 
            this.wtfRainbowsToolStripMenuItem.Name = "wtfRainbowsToolStripMenuItem";
            this.wtfRainbowsToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.wtfRainbowsToolStripMenuItem.Text = "wtf rainbows";
            this.wtfRainbowsToolStripMenuItem.Click += new System.EventHandler(this.wtfRainbowsToolStripMenuItem_Click);
            // 
            // setSoundFileLocationToolStripMenuItem
            // 
            this.setSoundFileLocationToolStripMenuItem.Name = "setSoundFileLocationToolStripMenuItem";
            this.setSoundFileLocationToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.setSoundFileLocationToolStripMenuItem.Text = "Set Sound File Location";
            this.setSoundFileLocationToolStripMenuItem.Click += new System.EventHandler(this.setSoundFileLocationToolStripMenuItem_Click);
            // 
            // getProgramLocationToolStripMenuItem
            // 
            this.getProgramLocationToolStripMenuItem.Name = "getProgramLocationToolStripMenuItem";
            this.getProgramLocationToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.getProgramLocationToolStripMenuItem.Text = "Get Program Location";
            this.getProgramLocationToolStripMenuItem.Click += new System.EventHandler(this.getProgramLocationToolStripMenuItem_Click);
            // 
            // resetCharacterIconsCacheToolStripMenuItem
            // 
            this.resetCharacterIconsCacheToolStripMenuItem.Name = "resetCharacterIconsCacheToolStripMenuItem";
            this.resetCharacterIconsCacheToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.resetCharacterIconsCacheToolStripMenuItem.Text = "Reset Character Icons Cache";
            this.resetCharacterIconsCacheToolStripMenuItem.Click += new System.EventHandler(this.resetCharacterIconsCacheToolStripMenuItem_Click);
            // 
            // useDDSVersionReaderToolStripMenuItem
            // 
            this.useDDSVersionReaderToolStripMenuItem.Name = "useDDSVersionReaderToolStripMenuItem";
            this.useDDSVersionReaderToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.useDDSVersionReaderToolStripMenuItem.Text = "Use DDS Version Reader";
            this.useDDSVersionReaderToolStripMenuItem.Click += new System.EventHandler(this.useDDSVersionReaderToolStripMenuItem_Click);
            // 
            // testReadResFilesToolStripMenuItem
            // 
            this.testReadResFilesToolStripMenuItem.Name = "testReadResFilesToolStripMenuItem";
            this.testReadResFilesToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.testReadResFilesToolStripMenuItem.Text = "Test Read Res Files";
            this.testReadResFilesToolStripMenuItem.Click += new System.EventHandler(this.testReadResFilesToolStripMenuItem_Click);
            // 
            // unpackSoundsToolStripMenuItem
            // 
            this.unpackSoundsToolStripMenuItem.Name = "unpackSoundsToolStripMenuItem";
            this.unpackSoundsToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.unpackSoundsToolStripMenuItem.Text = "unpack sounds";
            this.unpackSoundsToolStripMenuItem.Click += new System.EventHandler(this.unpackSoundsToolStripMenuItem_Click);
            // 
            // loLViewerOpenNotPreviewToolStripMenuItem
            // 
            this.loLViewerOpenNotPreviewToolStripMenuItem.Name = "loLViewerOpenNotPreviewToolStripMenuItem";
            this.loLViewerOpenNotPreviewToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.loLViewerOpenNotPreviewToolStripMenuItem.Text = "LoLViewer Open (Not Preview)";
            this.loLViewerOpenNotPreviewToolStripMenuItem.Click += new System.EventHandler(this.loLViewerOpenNotPreviewToolStripMenuItem_Click);
            // 
            // openParticleReferenceToolStripMenuItem
            // 
            this.openParticleReferenceToolStripMenuItem.Name = "openParticleReferenceToolStripMenuItem";
            this.openParticleReferenceToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.openParticleReferenceToolStripMenuItem.Text = "Open Particle Reference";
            this.openParticleReferenceToolStripMenuItem.Click += new System.EventHandler(this.openParticleReferenceToolStripMenuItem_Click);
            // 
            // showMenuFileLocationToolStripMenuItem
            // 
            this.showMenuFileLocationToolStripMenuItem.Name = "showMenuFileLocationToolStripMenuItem";
            this.showMenuFileLocationToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.showMenuFileLocationToolStripMenuItem.Text = "Show Menu File Location";
            this.showMenuFileLocationToolStripMenuItem.Click += new System.EventHandler(this.showMenuFileLocationToolStripMenuItem_Click);
            // 
            // openTextTreeEditorToolStripMenuItem
            // 
            this.openTextTreeEditorToolStripMenuItem.Name = "openTextTreeEditorToolStripMenuItem";
            this.openTextTreeEditorToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.openTextTreeEditorToolStripMenuItem.Text = "Open Text Tree Editor";
            this.openTextTreeEditorToolStripMenuItem.Click += new System.EventHandler(this.openTextTreeEditorToolStripMenuItem_Click);
            // 
            // openPublisherToolStripMenuItem
            // 
            this.openPublisherToolStripMenuItem.Name = "openPublisherToolStripMenuItem";
            this.openPublisherToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.openPublisherToolStripMenuItem.Text = "Open Publisher";
            this.openPublisherToolStripMenuItem.Click += new System.EventHandler(this.openPublisherToolStripMenuItem_Click);
            // 
            // skinHelpersToolStripMenuItem
            // 
            this.skinHelpersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyLikeFilesToolStripMenuItem});
            this.skinHelpersToolStripMenuItem.Name = "skinHelpersToolStripMenuItem";
            this.skinHelpersToolStripMenuItem.Size = new System.Drawing.Size(333, 30);
            this.skinHelpersToolStripMenuItem.Text = "Skin Helpers";
            // 
            // copyLikeFilesToolStripMenuItem
            // 
            this.copyLikeFilesToolStripMenuItem.Name = "copyLikeFilesToolStripMenuItem";
            this.copyLikeFilesToolStripMenuItem.Size = new System.Drawing.Size(212, 30);
            this.copyLikeFilesToolStripMenuItem.Text = "Copy Like Files";
            this.copyLikeFilesToolStripMenuItem.Click += new System.EventHandler(this.copyLikeFilesToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panelGL);
            this.panel2.Controls.Add(this.panel7);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 368);
            this.panel2.MaximumSize = new System.Drawing.Size(2000, 2000);
            this.panel2.MinimumSize = new System.Drawing.Size(478, 95);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(886, 95);
            this.panel2.TabIndex = 43;
            // 
            // panelGL
            // 
            this.panelGL.BackColor = System.Drawing.Color.Black;
            this.panelGL.Controls.Add(this.label5);
            this.panelGL.Controls.Add(this.pictureBoxCount);
            this.panelGL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGL.Location = new System.Drawing.Point(0, 54);
            this.panelGL.Name = "panelGL";
            this.panelGL.Size = new System.Drawing.Size(886, 41);
            this.panelGL.TabIndex = 43;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(642, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 15);
            this.label5.TabIndex = 45;
            this.label5.Text = "Total Skins Installed :";
            // 
            // pictureBoxCount
            // 
            this.pictureBoxCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCount.BackColor = System.Drawing.Color.Black;
            this.pictureBoxCount.Location = new System.Drawing.Point(803, 28);
            this.pictureBoxCount.Name = "pictureBoxCount";
            this.pictureBoxCount.Size = new System.Drawing.Size(83, 13);
            this.pictureBoxCount.TabIndex = 44;
            this.pictureBoxCount.TabStop = false;
            this.pictureBoxCount.Click += new System.EventHandler(this.pictureBoxCount_Click);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.button3FixCrashAfterPatch);
            this.panel7.Controls.Add(this.progrespanel);
            this.panel7.Controls.Add(this.button3startLoL);
            this.panel7.Controls.Add(this.button3lcintegrate);
            this.panel7.Controls.Add(this.UpdateFL);
            this.panel7.Controls.Add(this.locateGameClient);
            this.panel7.Controls.Add(this.exit);
            this.panel7.Controls.Add(this.button2);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(886, 54);
            this.panel7.TabIndex = 42;
            // 
            // button3FixCrashAfterPatch
            // 
            this.button3FixCrashAfterPatch.Location = new System.Drawing.Point(392, 3);
            this.button3FixCrashAfterPatch.Name = "button3FixCrashAfterPatch";
            this.button3FixCrashAfterPatch.Size = new System.Drawing.Size(275, 23);
            this.button3FixCrashAfterPatch.TabIndex = 45;
            this.button3FixCrashAfterPatch.Text = "Fix Crashes/Invisible Chars/Blue Textures after Patch";
            this.button3FixCrashAfterPatch.UseVisualStyleBackColor = true;
            this.button3FixCrashAfterPatch.Click += new System.EventHandler(this.button3FixCrashAfterPatch_Click);
            // 
            // progrespanel
            // 
            this.progrespanel.Controls.Add(this.label2Percent);
            this.progrespanel.Controls.Add(this.progressBar1);
            this.progrespanel.Controls.Add(this.buttoncancel);
            this.progrespanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progrespanel.Location = new System.Drawing.Point(0, 32);
            this.progrespanel.Name = "progrespanel";
            this.progrespanel.Size = new System.Drawing.Size(886, 22);
            this.progrespanel.TabIndex = 44;
            // 
            // label2Percent
            // 
            this.label2Percent.AutoSize = true;
            this.label2Percent.Location = new System.Drawing.Point(29, 5);
            this.label2Percent.Name = "label2Percent";
            this.label2Percent.Size = new System.Drawing.Size(64, 20);
            this.label2Percent.TabIndex = 42;
            this.label2Percent.Text = "Percent";
            // 
            // buttoncancel
            // 
            this.buttoncancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttoncancel.Location = new System.Drawing.Point(836, 0);
            this.buttoncancel.Name = "buttoncancel";
            this.buttoncancel.Size = new System.Drawing.Size(50, 22);
            this.buttoncancel.TabIndex = 41;
            this.buttoncancel.Text = "Cancel";
            this.buttoncancel.UseVisualStyleBackColor = true;
            this.buttoncancel.Click += new System.EventHandler(this.buttoncancel_Click);
            // 
            // button3startLoL
            // 
            this.button3startLoL.Location = new System.Drawing.Point(673, 3);
            this.button3startLoL.Name = "button3startLoL";
            this.button3startLoL.Size = new System.Drawing.Size(75, 23);
            this.button3startLoL.TabIndex = 43;
            this.button3startLoL.Text = "Start LoL";
            this.button3startLoL.UseVisualStyleBackColor = true;
            this.button3startLoL.Click += new System.EventHandler(this.button3startLoL_Click);
            // 
            // button3lcintegrate
            // 
            this.button3lcintegrate.Location = new System.Drawing.Point(233, 3);
            this.button3lcintegrate.Name = "button3lcintegrate";
            this.button3lcintegrate.Size = new System.Drawing.Size(153, 23);
            this.button3lcintegrate.TabIndex = 42;
            this.button3lcintegrate.Text = "Use with WebSites";
            this.button3lcintegrate.UseVisualStyleBackColor = true;
            this.button3lcintegrate.Click += new System.EventHandler(this.button3lcintegrate_Click);
            // 
            // fileListWorker1
            // 
            this.fileListWorker1.WorkerReportsProgress = true;
            this.fileListWorker1.WorkerSupportsCancellation = true;
            this.fileListWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.fileListWorker1_DoWork);
            this.fileListWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroudProgress_Changed);
            this.fileListWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_Done);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // installWorker2
            // 
            this.installWorker2.WorkerReportsProgress = true;
            this.installWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.installWork);
            this.installWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.installProgress);
            this.installWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.installComplete);
            // 
            // updateWorker2
            // 
            this.updateWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updateWorker2_DoWork);
            this.updateWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.updateWorker2_RunWorkerCompleted);
            // 
            // webPinger
            // 
            this.webPinger.DoWork += new System.ComponentModel.DoWorkEventHandler(this.webPinger_DoWork);
            this.webPinger.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.webPinger_RunWorkerCompleted);
            // 
            // timeupdatecount
            // 
            this.timeupdatecount.Interval = 10000;
            this.timeupdatecount.Tick += new System.EventHandler(this.timeupdatecount_Tick);
            // 
            // backgroundWorkerCountUpdate
            // 
            this.backgroundWorkerCountUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerCountUpdate_DoWork);
            // 
            // rafTreeBuilderWorker2
            // 
            this.rafTreeBuilderWorker2.WorkerReportsProgress = true;
            this.rafTreeBuilderWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.rafTreeBuilderWorker2_DoWork);
            this.rafTreeBuilderWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.rafTreeBuilderWorker2_ProgressChanged);
            this.rafTreeBuilderWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.rafTreeBuilderWorker2_RunWorkerCompleted);
            // 
            // exportFolderBrowserDialog1
            // 
            this.exportFolderBrowserDialog1.Description = "Please Choose the Export Directory";
            // 
            // ParticleTreeWorker2
            // 
            this.ParticleTreeWorker2.WorkerReportsProgress = true;
            this.ParticleTreeWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ParticleTreeWorker2_DoWork);
            this.ParticleTreeWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.ParticleTreeWorker2_ProgressChanged);
            this.ParticleTreeWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ParticleTreeWorker2_RunWorkerCompleted);
            // 
            // treeMenuStripSkin1
            // 
            this.treeMenuStripSkin1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.treeMenuStripSkin1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.makeSimpleSkinFromThisRiotSkinToolStripMenuItem});
            this.treeMenuStripSkin1.Name = "treeViewMenuStrip1";
            this.treeMenuStripSkin1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.treeMenuStripSkin1.Size = new System.Drawing.Size(377, 124);
            this.treeMenuStripSkin1.Opening += new System.ComponentModel.CancelEventHandler(this.treeMenuStripSkin1_Opening);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(376, 30);
            this.toolStripMenuItem1.Text = "Export Selected Files";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.exportSelectedFilesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(376, 30);
            this.toolStripMenuItem2.Text = "Deselect All Files";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.deselectAllFilesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(376, 30);
            this.toolStripMenuItem3.Text = "Help!";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // makeSimpleSkinFromThisRiotSkinToolStripMenuItem
            // 
            this.makeSimpleSkinFromThisRiotSkinToolStripMenuItem.Name = "makeSimpleSkinFromThisRiotSkinToolStripMenuItem";
            this.makeSimpleSkinFromThisRiotSkinToolStripMenuItem.Size = new System.Drawing.Size(376, 30);
            this.makeSimpleSkinFromThisRiotSkinToolStripMenuItem.Text = "Make Simple Skin from this Riot Skin";
            this.makeSimpleSkinFromThisRiotSkinToolStripMenuItem.Click += new System.EventHandler(this.makeSimpleSkinFromThisRiotSkinToolStripMenuItem_Click);
            // 
            // exportTreeViewWorker1
            // 
            this.exportTreeViewWorker1.WorkerReportsProgress = true;
            this.exportTreeViewWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.exportTreeViewWorker1_DoWork);
            this.exportTreeViewWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.exportTreeViewWorker1_ProgressChanged);
            this.exportTreeViewWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.exportTreeViewWorker1_RunWorkerCompleted);
            // 
            // ParticleTreeWorkerNew
            // 
            this.ParticleTreeWorkerNew.WorkerReportsProgress = true;
            this.ParticleTreeWorkerNew.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ParticleTreeWorkerNew_DoWork);
            this.ParticleTreeWorkerNew.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.ParticleTreeWorkerNew_ProgressChanged);
            this.ParticleTreeWorkerNew.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ParticleTreeWorkerNew_RunWorkerCompleted);
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.Location = new System.Drawing.Point(0, 33);
            this.splitContainer6.Name = "splitContainer6";
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.panel10);
            this.splitContainer6.Panel2MinSize = 1;
            this.splitContainer6.Size = new System.Drawing.Size(886, 335);
            this.splitContainer6.SplitterDistance = 728;
            this.splitContainer6.TabIndex = 44;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.webBrowser2Test);
            this.panel10.Controls.Add(this.button3CloseAd);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel10.Location = new System.Drawing.Point(0, 297);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(154, 38);
            this.panel10.TabIndex = 0;
            // 
            // webBrowser2Test
            // 
            this.webBrowser2Test.AdditionalHeaders = null;
            this.webBrowser2Test.Location = new System.Drawing.Point(50, 8);
            this.webBrowser2Test.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser2Test.Name = "webBrowser2Test";
            this.webBrowser2Test.Size = new System.Drawing.Size(39, 21);
            this.webBrowser2Test.TabIndex = 1;
            this.webBrowser2Test.Tag = true;
            this.webBrowser2Test.Visible = false;
            this.webBrowser2Test.BeforeNavigate2 += new System.EventHandler<SkinInstaller.BeforeNavigate2EventArgs>(this.webBrowser2_BeforeNavigate2);
            this.webBrowser2Test.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser2Test_DocumentCompleted);
            // 
            // button3CloseAd
            // 
            this.button3CloseAd.Location = new System.Drawing.Point(3, 6);
            this.button3CloseAd.Name = "button3CloseAd";
            this.button3CloseAd.Size = new System.Drawing.Size(30, 23);
            this.button3CloseAd.TabIndex = 0;
            this.button3CloseAd.Text = ">>";
            this.button3CloseAd.UseVisualStyleBackColor = true;
            this.button3CloseAd.Click += new System.EventHandler(this.button3CloseAd_Click);
            // 
            // addFilesWorker
            // 
            this.addFilesWorker.WorkerReportsProgress = true;
            this.addFilesWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.addFilesWorker_DoWork);
            this.addFilesWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.addFilesWorker_ProgressChanged);
            this.addFilesWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.addFilesWorker_RunWorkerCompleted);
            // 
            // patchFixerWorker
            // 
            this.patchFixerWorker.WorkerReportsProgress = true;
            this.patchFixerWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.patchFixerWorker_DoWork);
            this.patchFixerWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.patchFixerWorker_ProgressChanged);
            this.patchFixerWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.patchFixerWorker_RunWorkerCompleted);
            // 
            // uninstallWorker1
            // 
            this.uninstallWorker1.WorkerReportsProgress = true;
            this.uninstallWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.uninstallWorker1_DoWork);
            this.uninstallWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.uninstallWorker1_ProgressChanged);
            this.uninstallWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.uninstallWorker1_RunWorkerCompleted);
            // 
            // skinInstaller
            // 
            this.AllowDrop = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(886, 485);
            this.Controls.Add(this.splitContainer6);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.helpBar);
            this.Controls.Add(this.menuStrip1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(2000, 2000);
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "skinInstaller";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "(Set by main program file)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.skinInstaller_FormClosing);
            this.Load += new System.EventHandler(this.skinInstaller_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.skinInstaller_DragOver);
            this.tabPage2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.AddToDatabasePanel.ResumeLayout(false);
            this.addFilesPanel.ResumeLayout(false);
            this.addFilesPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.dataBaseListMenuStrip1.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.treeViewMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panelGL.ResumeLayout(false);
            this.panelGL.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCount)).EndInit();
            this.panel7.ResumeLayout(false);
            this.progrespanel.ResumeLayout(false);
            this.progrespanel.PerformLayout();
            this.treeMenuStripSkin1.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            this.splitContainer6.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private void ReadVersionListINI()
        {
            if (File.Exists(Application.StartupPath + "\\dxtVersion.ini"))
            {
                TextReader reader = new StreamReader(Application.StartupPath + "\\dxtVersion.ini");
                dxtVersions.Clear();
                while (reader.Peek() != -1)
                {
                    string[] line = reader.ReadLine().ToLower().Split(new char[] { '|' });
                    if (line.Length > 1)
                    {
                        if (!dxtVersions.ContainsKey(line[0].Trim()))
                        {

                            Dictionary<String, int> infos = new Dictionary<String, int>();
                            //File Size|Height|Width|Depth|Linear Size|Mip Maps|dxtVersion|Bit Count (uncompressed)
                            infos.Add("filesize", int.Parse(line[1]));
                            infos.Add("height", int.Parse(line[2]));
                            infos.Add("width", int.Parse(line[3]));
                            infos.Add("depth", int.Parse(line[4]));
                            infos.Add("linearsize", int.Parse(line[5]));
                            infos.Add("mipmaps", int.Parse(line[6]));
                            infos.Add("dxtv", int.Parse(line[7]));
                            infos.Add("bitcount", int.Parse(line[8]));
                            dxtVersions.Add(line[0].Trim(), infos);
                        }
                    }
                }
                reader.Close();
            }
            else
            {
                Cliver.Message.Inform("This program is missing the file dxdVersion.ini\r\nPlease make sure you extracted it correctly, or have the rights to read it");
            }
                        
        }
        private void ReadFilelistINI()
        {
            ReadVersionListINI();
            string[] strArray = new string[2];
            if (File.Exists(Application.StartupPath + "\\allfiles.ini"))
            {
                allFilesList = new List<KeyValuePair<String, String>>();
                TextReader reader = new StreamReader(Application.StartupPath + "\\allfiles.ini");
                //allFilesCt = 0;
                bool firstLine = true;
                while (reader.Peek() != -1)
                {
                    //if (allFilesCt == 0)
                    if(firstLine)
                    {
                        allFilesExtensions = reader.ReadLine().Split(new char[] { '|' });
                        firstLine = false;
                        //allFilesCt++;
                    }
                    else
                    {
                        strArray = reader.ReadLine().Split(new char[] { '|' });
                        if (strArray[0] != "")
                        {
                            //allFiles[allFilesCt - 1, 0] = strArray[0];
                            //allFiles[allFilesCt - 1, 1] = strArray[1];
                            //allFilesCt++;
                            //allFilesDic.Add(strArray[0], strArray[1]);
                            if(isNotAnExcludedFile(strArray[0],strArray[1]))
                                allFilesList.Add(new KeyValuePair<string, string>(strArray[0], strArray[1]));
                        }
                    }
                }
                reader.Close();
                /*if (allFilesCt >= 2)
                {
                    allFilesCt--;
                }*/
                this.CheckForUpdate(false);
            }
            else
            {
                Cliver.Message.Inform("The file list, allfiles.ini, could not be found or created.\nPlease restart Skin Installer Ultimate again to attempt to correct the issue.");
            }
            setInstallButtons(true);
            ted = new TextEditor.TextEditorMain(getMenuFilePath(false));      

        }
        private void loadExcludedFiles()
        {
            excludedFiles.Clear();
            List<string> armsmaster_square = new List<string>();
            armsmaster_square.Add("\\data\\characters\\malphite\\info\\armsmaster_square.dds");
            armsmaster_square.Add("\\data\\characters\\shen\\info\\armsmaster_square.dds");
            excludedFiles.Add("armsmaster_square.dds", armsmaster_square);


            List<string> armsmaster_circle = new List<string>();
            armsmaster_circle.Add("\\data\\characters\\malphite\\info\\armsmaster_circle.dds");
            armsmaster_circle.Add("\\data\\characters\\shen\\info\\armsmaster_circle.dds");
            excludedFiles.Add("armsmaster_circle.dds", armsmaster_circle);

            List<string> annieLoadScreen = new List<string>();
            annieLoadScreen.Add("\\data\\characters\\lux\\annieloadscreen.dds");
            excludedFiles.Add("annieloadscreen.dds", annieLoadScreen);
        }
        private bool isNotAnExcludedFile(string fileName, string filePath)
        {
            if(excludedFiles.ContainsKey(fileName.ToLower()))
            {
                
                foreach (string badPaths in excludedFiles[fileName.ToLower()])
                {                    
                    if (filePath.ToLower().Replace("\\\\","\\").Contains(badPaths.ToLower()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Brush brush;
            Graphics graphics = e.Graphics;
            TabPage page = this.tabControl1.TabPages[e.Index];
            StringFormat format = new StringFormat();
            RectangleF layoutRectangle = new RectangleF((float) e.Bounds.X, (float) (e.Bounds.Y + 2), (float) e.Bounds.Width, (float) (e.Bounds.Height - 2));
            format.Alignment = StringAlignment.Center;
            string text = page.Text;
            if (this.tabControl1.SelectedIndex == e.Index)
            {
                brush = new SolidBrush(page.BackColor);
                graphics.FillRectangle(brush, e.Bounds);
                brush = new SolidBrush(page.ForeColor);
                graphics.DrawString(text, this.tabControl1.Font, brush, layoutRectangle, format);
            }
            else
            {
                brush = new SolidBrush(Color.WhiteSmoke);
                graphics.FillRectangle(brush, e.Bounds);
                brush = new SolidBrush(Color.Black);
                graphics.DrawString(text, this.tabControl1.Font, brush, layoutRectangle, format);
            }
        }
        private void UpdateFileList()
        {
            //if (Cliver.Message.Show("Create File List", SystemIcons.Error,"Are you sure you want to update your allfiles.ini file?\n\nThe process takes several minutes and temporarily\nrequires approx. 500 MBs of hard drive space.", 0, new string[2] { "Yes", "No" }) == 0)
            if(true)
            {
               //if (Application.StartupPath.ToLower().Contains(gameDirectory.ToLower()))
               // {

                    //fffff
                    //Cliver.Message.Show("VERY BAD PROBLEM!", SystemIcons.Error, "You have your skin installer installed INSIDE of your lol directory!  You cant do that!", 0, "OK ILL FIX IT");
                //}
                if (!fileListWorker1.IsBusy)
                {
                    UpdateFL.Enabled = false;
                    UpdateFL.Text = "Currently Updating :)";

                    b_IAddFiles.Enabled = false;
                    b_IAddDirectory.Enabled = false;
                    statusText.Text = "Initializing Program and Scanning LoL Files";
                    setInstallButtons(false);
            
                    fileListWorker1.RunWorkerAsync(gameDirectory);
                    
                }
                /*
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "siupdate.exe";
                startInfo.Arguments = gameDirectory;
                try
                {
                    using (Process process = Process.Start(startInfo))
                    {
                        process.WaitForExit();
                    }
                }
                catch (Exception exception)
                {
                    Cliver.Message.Inform("An error occured during the update procedure:\n" + exception.Message);
                }

                */
                //probably should delete the sound backup.. its probably out of date now.
                //String restoreFile = Application.StartupPath + @"\backup\Game\DATA\Sounds\FMOD\VOBank_en_US.fsb";
                //if (File.Exists(restoreFile)) File.Delete(restoreFile);


            }
        }
        
        #region DisplayList
        private void UpdateListView()
        {
            
            try
            {
                //if (this.sqLiteCon.State.ToString() != "Open")
                //{
                //    this.sqLiteCon.Open();
                //}
                new SQLiteCommand("CREATE TABLE IF NOT EXISTS skins(sName TEXT, sInstalled BOOLEAN, sFileCount TEXT)", this.sqLiteCon).ExecuteNonQuery();
                
                
                
                //add author field
                //new SQLiteCommand("ALTER TABLE `skins` ADD `author` VARCHAR( 100 ) NOT NULL DEFAULT 'Unknown' ",this.sqLiteCon).ExecuteNonQuery();
                SQLiteDataReader reader = new SQLiteCommand("SELECT skins.* FROM skins", this.sqLiteCon).ExecuteReader();
                bool alreadyUpdated = false;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals("author",
                        StringComparison.InvariantCultureIgnoreCase))
                        alreadyUpdated = true;
                }
                if (!alreadyUpdated)
                {
                    new SQLiteCommand("ALTER TABLE `skins` ADD `author` VARCHAR( 100 ) NOT NULL DEFAULT 'Unknown' ", this.sqLiteCon).ExecuteNonQuery();
                    reader = new SQLiteCommand("SELECT skins.* FROM skins", this.sqLiteCon).ExecuteReader();

                }
                DateTime dt = DateTime.Now;
                String dtstring = dt.ToString(dateFormat);
                //look for second database update oo.oo
                alreadyUpdated = false;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals("dateadded",
                        StringComparison.InvariantCultureIgnoreCase))
                        alreadyUpdated = true;
                }
                if (!alreadyUpdated)
                {
                    new SQLiteCommand("ALTER TABLE `skins` ADD `dateadded` VARCHAR( 100 ) NOT NULL DEFAULT '" + dtstring + "' ", this.sqLiteCon).ExecuteNonQuery();
                    reader = new SQLiteCommand("SELECT skins.* FROM skins", this.sqLiteCon).ExecuteReader();
                }
                //look for third database update oo.oo
                alreadyUpdated = false;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals("dateinstalled",
                        StringComparison.InvariantCultureIgnoreCase))
                        alreadyUpdated = true;
                }
                if (!alreadyUpdated)
                {
                    new SQLiteCommand("ALTER TABLE `skins` ADD `dateinstalled` VARCHAR( 100 ) NOT NULL DEFAULT '" + "-" + "' ", this.sqLiteCon).ExecuteNonQuery();
                    reader = new SQLiteCommand("SELECT skins.* FROM skins", this.sqLiteCon).ExecuteReader();
                }
                imageIcons.Clear();

                //listView1.BackColor = Color.Black;
                int number = 0;
                if (reader.HasRows)
                {
                    this.listView1.Items.Clear();
                    
                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem();
                        string da = reader["dateadded"].ToString().Trim();
                        prettyDate addedD = new prettyDate(da);
                        prettyDate installedD = new prettyDate(reader["dateinstalled"].ToString().Trim());
                        
                        item.SubItems.Add(reader["sName"].ToString().Trim());
                        item.SubItems.Add(reader["author"].ToString().Trim());
                        item.SubItems.Add(reader["sFileCount"].ToString().Trim());
                        if (installedD.ToString() == "No" &&
                            reader["sInstalled"].ToString().Trim() == "True")
                        {
                            item.SubItems.Add(addedD.ToString());
                        }
                        else
                        {
                            item.SubItems.Add(installedD.ToString());
                            
                        }
                        /*if (reader["sInstalled"].ToString().Trim() == "True")
                        {
                            item.SubItems.Add("Yes");
                        }
                        else
                        {
                            item.SubItems.Add("No");
                        }*/
                        item.SubItems.Add(addedD.ToString());
                        //item.SubItems.Add(installedD.ToString());
                        item.SubItems.Add(addedD.getStringDate());
                        if (installedD.ToString() == "No" &&
                            reader["sInstalled"].ToString().Trim() == "True")
                        {
                            item.SubItems.Add(addedD.getStringDate());
                        }else
                        item.SubItems.Add(installedD.getStringDate());

                        if ((reader["sInstalled"].ToString().Trim() == "True") || (installedD.ToString() != "No"))
                        {
                            item.Font = new Font(item.Font, FontStyle.Bold);
                            item.BackColor = Color.LightYellow;
                        }
                        else
                        {
                            item.Font = new Font(item.Font, FontStyle.Regular);
                            
                            item.BackColor = Color.White;
                            
                        }

                        item.SubItems.Add(getCharacterName(reader["sName"].ToString().Trim(),true));

                        imageIcons.Add(getCharacterImage(reader["sName"].ToString().Trim()));
            
                        item.ImageIndex = number++;
                        
                        this.listView1.Items.AddRange(new ListViewItem[] { item });
                        
                    }
                }
            }
            catch(Exception e)
            {
                if (Cliver.Message.Show("View Exception", SystemIcons.Error,"There was an  1 error with the database! Do you want to see the exception?", 0, new string[2] { "Yes", "No" }) == 0)
                {
                    throw e;
                }
            }
            finally
            {
                //this.sqLiteCon.Close();
            }
            imageList1.Images.Clear();
            imageList1.Images.AddRange(imageIcons.ToArray());
            //his.listView1.Columns[0].

                
        }
        private Image getCharacterImage(string skinFolder)
        {
            string cName = getCharacterName(skinFolder, false);
            string imageName = cName + "_square.dds";
            string FileName = Application.StartupPath + "\\icons\\" + imageName;
            if (File.Exists(FileName))
            {
                Image im = LGGDevilLoadImage(FileName);
                if (im != null) return im;
            }

            imageName = cName + "_square_0.png";
            FileName = Application.StartupPath + "\\icons\\" + imageName;
            if (File.Exists(FileName))
            {
                Image im = LGGDevilLoadImage(FileName);
                if (im != null) return im;
            }

            if(true)
            {
                //if (backgroundWorker1.IsBusy) return new Bitmap(2, 2);
            
                if (!Directory.Exists(Application.StartupPath + "\\icons"))
                    Directory.CreateDirectory(Application.StartupPath + "\\icons");
                //look through our files, see if its here

                foreach (KeyValuePair<String, String> pair in allFilesList)
                {
                    if (
                        (pair.Key.ToLower().Contains(cName.ToLower()+"_squ"))
                        &&(true)
                        )
                    {
                        FileName = Application.StartupPath + "\\icons\\" + pair.Key.ToLower();
                        if (File.Exists(FileName))
                            return LGGDevilLoadImage(FileName);

                        
                        try
                        {
                            //found!
                            string foundAt = pair.Value.Replace("\\\\", "\\");
                            int slash = foundAt.IndexOf("\\") ;
                            if (slash>= 0 && slash < 2)
                            {
                                foundAt = foundAt.Substring(slash+1);
                            }
                            if (pair.Value.ToLower().Contains(".raf"))
                            {
                                if (fileListWorker1.IsBusy) return new Bitmap(2, 2);
            
                                rafBackup(gameDirectory +
                                    foundAt, FileName);
                            }
                            else if (pair.Value.ToLower().Contains(".zip"))
                            {
                                if (fileListWorker1.IsBusy) return new Bitmap(2, 2);

                                zipBackup(gameDirectory +
                                    foundAt, FileName);
                            }
                            else
                            {
                                
                                copyAndFix(gameDirectory +
                                    foundAt, FileName);
                            }
                            if (File.Exists(FileName))
                                return LGGDevilLoadImage(FileName);
                        }
                        catch (Exception ex)
                        {
                            debugadd(ex.ToString());
                        }
                    }
                }

                
            }
            return new Bitmap(2, 2);
            //return Image.FromFile(Application.StartupPath + "\\LGGSIU1.bmp");
        }
        private String getCharacterName(string skinFolder, bool identifiable)
        {
            string toReturn = "";
            if(!skinFolder.Contains(@"\skins\"))
                skinFolder = Application.StartupPath + @"\skins\" + skinFolder;
            if (!Directory.Exists(skinFolder)) return toReturn;
            int winningFileCount = 0;
            string[] dirs = Directory.GetDirectories(skinFolder,"*",SearchOption.AllDirectories);
            //string[] files = Directory.GetFiles(skinFolder, "*.*", SearchOption.AllDirectories);
            foreach (string dir in dirs)
            {
                int loc = dir.IndexOf(@"\data\characters\");
                if (loc!=-1)
                {
                    int numFiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Length;
                    if (numFiles > winningFileCount)
                    {
                        winningFileCount = numFiles;
                        //get the skin name
                        string lastpart = dir.Substring(loc + 17);
                        int stop = lastpart.IndexOf("\\");
                        if (stop != -1)
                            toReturn = lastpart.Substring(0, lastpart.IndexOf("\\"));
                        else
                            toReturn = lastpart;


                    }
                    

                }
            }
            if(identifiable)
                toReturn = makeNameIdentifiable(toReturn);
            char[] splittedPhraseChars = toReturn.ToCharArray();
            if (splittedPhraseChars.Length > 0)
            {
                splittedPhraseChars[0] = ((new String(splittedPhraseChars[0], 1)).ToUpper().ToCharArray())[0];
            }
            toReturn = new String(splittedPhraseChars);
            return toReturn;
        }
        public string makeNameIdentifiable(string inName)
        {
            inName = inName.ToLower();
            
            if (charFixs.ContainsKey(inName))
                inName = charFixs[inName];
            return inName;
        }
        #endregion
        #region GUIClicks
        private void unpackSoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String fsbDir = Application.StartupPath + @"\fsb\";
            String soundsFolder = fsbDir + "sounds";
            /*String tempFSB = fsbDir + "temp.fsb";

            this.SIFileOp.FileCopy(gameDirectory + findSoundsFSBLocation(), tempFSB);
            

            runthis(fsbDir + "map.bat", "", tempFSB, false);
            File.Delete(tempFSB);*/
            if (Directory.Exists(soundsFolder)) SIFileOp.DirectoryDelete(soundsFolder, true);
            Directory.CreateDirectory(soundsFolder);

            runthis(fsbDir + "fsbext.exe", "-s -d sounds " + "\"" + gameDirectory +
               findSoundsFSBLocation() + "\"", fsbDir, false);
        }
        private void openPublisherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string updater = Application.StartupPath + "\\SIU-Publish.exe";
            if (File.Exists(updater))
            {
                Process.Start(updater);
            }
        }
        private void createZip_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.CheckedItems)
            {
                string text = item.SubItems[1].Text;
                string path = Application.StartupPath + @"\skins\" + text + @"\";
                this.CreateEasyInstallFile(text);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                ZipUtil.ZipFiles(path, text + ".zip", "");
                this.SIFileOp.FileCopy(path + text + ".zip", Application.StartupPath + @"\" + text + ".zip");
                if (File.Exists(path + "Read Me.txt"))
                {
                    this.SIFileOp.FileDelete(path + "Read Me.txt");
                }
                if (File.Exists(path + text + ".zip"))
                {
                    this.SIFileOp.FileDelete(path + text + ".zip");
                }
                Cliver.Message.Inform("The file " + text + ".zip\nwas successfully created and placed in the Skin Installer folder.");
            }
        }
        private void dbDelete_Click(object sender, EventArgs e)
        {
            if (Cliver.Message.Show("Are you sure?",
                SystemIcons.Question, "Are you sure want delete these skins?\nThis will prevent you from being able to UNINSTALL or install", 0, new string[2] { "Yes", "No" }) == 0)
            {
                foreach (ListViewItem item in this.listView1.CheckedItems)
                {
                    if (Directory.Exists(Application.StartupPath + @"\skins\" + item.SubItems[1].Text))
                    {
                        this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\skins\" + item.SubItems[1].Text, true);
                    }

                    this.ExecuteQuery("DELETE FROM skins WHERE sName=\"" + item.SubItems[1].Text + "\"");
                    this.listView1.Items.Remove(item);
                }
            }
            this.UpdateListView();
        }
        private void exit_Click(object sender, EventArgs e)
        {
            if (Cliver.Message.Show("Exit", SystemIcons.Question, "Are you sure want to exit?", 0, new string[2] { "Yes", "No" }) == 0)
            {
                if (Directory.Exists(Application.StartupPath + @"\st\"))
                {
                    this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\st\", true);
                }
                if (Directory.Exists(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL + "\\"))
                {
                    this.SIFileOp.DirectoryDelete(Application.StartupPath + "\\" + c_EXTRACTED_AND_EXTRA_TEMP_FILES_FOR_SKIN_INSTALL + "\\", true);
                }
                if (Directory.Exists(Application.StartupPath + "\\" + c_DIRECTORY_NAME_FOR_FIXING_ZIPS + "\\"))
                {
                    this.SIFileOp.DirectoryDelete(Application.StartupPath + "\\" + c_DIRECTORY_NAME_FOR_FIXING_ZIPS + "\\", true);
                }
                if (Directory.Exists(Application.StartupPath + @"\spctemp\"))
                {
                    this.SIFileOp.DirectoryDelete(Application.StartupPath + @"\spctemp\", true);
                }
                Application.Exit();
            }
        }
        private void UpdateFL_Click(object sender, EventArgs e)
        {
            UpdateFileList();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            /*InfoForm form =new InfoForm(
                "For help with this program\r\nPlease watch this youtube video http://www.youtube.com/watch?v=FaqzqhWoCHs\r\n\r\nFor basic information.\r\nThis program is designed to assist your installation of all LoL skins you may find.\r\nIt does this by auto-detecting the correct location for each file, as well as keeping track of what you have installed, and creating backups making it easy to uninstall each skin as well.\r\n\r\nIf you have just downloaded a skin, you must first add it to the list of skins in this program.  This step allows the program to make sure it knows what locations to use, so it can properly install and uninstall, read bellow on how to do this.\r\n\r\nTo install a skin, make sure you are inside the second tab named \"Skin Database\".  Then click the check box next to the name of the skin you wish to install, and click the \"Install\" button at the bottom.\r\n\r\nTo uninstall a skin, follow the same instructions, just use the \"Uninstall\" button.\r\n\r\nTo add a new skin that you have downloaded from the internet, switch to the first tab named \"Skin Installer\".  Type in the name you wish to identify the skin by, and then either click \"add files\" or \"add directory\" until you have told the program where all the files for this skin are (that you downloaded).\r\n\r\nOnce you have the list of files you want, press the \"Add to DB\" button, the program will make a copy of the skin (allowing you to delete the files you just downloaded) and its name will now be viewable on the other tab to install.\r\n\r\nAdditional Helpful information.\r\n\r\nYou can press the \"Update File List\" button to make the program re-scan your LoL game, so it has a better idea where each file belongs.\r\n\r\nYou can press the \"Game Client\" button to tell the program what LoL install location to add skins to.\r\n\r\nMore information about the preferences options.\r\n\r\nCharacter 3d models refers to the .skn files that contain new data on the 3d shape of the character.\r\n\r\nCharacter textures refers to the .dds or tga files that contain the data of the texture and colors that are applied to the model.\r\n\r\nParticle 3d models refers to the .sco or sbc files that contain new data on the 3d shape of effects from spells or events in game.\r\n\r\nParticle textures refers to the .dds or tga files that contain the data of the color of these effects.\r\n\r\nAnimations refers to the .anim files, which contain the data for the time and movement of characters arms and legs, the .txt or .ini files which contain data on what .anim file to use for what event, and the .skl files which contain the data on what bones each model has.\r\n\r\nAir Mods refers to all files withing the \"air\" folder of your game client.  These files are what effects how the game behaves before you see the character loading screen, and contains the images of character in the character select or purchase screen, etc. Depending on which of these files is changed, it may be necessary to uninstall before a client update, and then reinstall after.\r\n\r\nSounds refers to all sound effects that you hear within game, such as the characters response to you telling them to move or attack.  These files must be fmod mp2 formatted files in the same bitrate as the ones they are replacing.  Installing these files will prevent your LoL from updating correctly if you do not uninstall them first.  PLEASE REMEMBER TO UNINSTALL then update, then reinstall, if you wish to use sound modifications.\r\n\r\nMenu and Text mods refers to the text in the game, such as character names and spell descriptions.  There is only one file for all text in the game, so installing a skin with a menu mod will mean that further updates to the text (such as new spell names) will not be included.  Keep this in mind when choosing to install them.\r\n\r\nFor additional help, Please email\r\nLordGregGreg@Gmail.com\r\n",

                new Size(470, 750),
                "Help",true)
            {
                StartPosition = FormStartPosition.CenterParent
            };
            form.ShowDialog();*/

            string help = "For help with this program\r\nPlease watch this youtube video http://www.youtube.com/watch?v=FaqzqhWoCHs\r\nThere is also a basic tutorial here http://forum.leaguecraft.com/index.php?/topic/28468-how-to-install-skins-in-new-launcher-the-easy-way/ \r\n\r\nTrouble Shooting!\r\n* Make sure that your LoL path is set correctly, click the \"Game Client\" Button to change it\r\n* Click the \"Update File List\" button, then try re-addig and re-installing your skin (of if its already added, use the fix skin button)\r\n* If LoL crashes after you installed a skin, uninstall it and report the issue to me\r\n(currently some skins just wont work right)\r\n* If absolutely all hell breaks loose, you may need to go to \"Riot Games\\League of Legends\\RADS\\projects\\lol_game_client\\filearchives\" and delete everything in there, then\r\nopen up your launcher, click the gear on the top right, then click repair.\r\n===========================================\r\nFor basic information.\r\nThis program is designed to assist your installation of all LoL skins you may find.\r\nIt does this by auto-detecting the correct location for each file, as well as keeping track of what you have installed, and creating backups making it easy to uninstall each skin as well.\r\n\r\nIf you have just downloaded a skin, you must first add it to the list of skins in this program.  This step allows the program to make sure it knows what locations to use, so it can properly install and uninstall, read bellow on how to do this.\r\n\r\nTo install a skin, make sure you are inside the second tab named \"Skin Database\".  Then click the check box next to the name of the skin you wish to install, and click the \"Install\" button at the bottom.\r\n\r\nTo uninstall a skin, follow the same instructions, just use the \"Uninstall\" button.\r\n\r\nTo add a new skin that you have downloaded from the internet, switch to the first tab named \"Skin Installer\".  Type in the name you wish to identify the skin by, and then either click \"add files\" or \"add directory\" until you have told the program where all the files for this skin are (that you downloaded).\r\n\r\nOnce you have the list of files you want, press the \"Add to DB\" button, the program will make a copy of the skin (allowing you to delete the files you just downloaded) and its name will now be viewable on the other tab to install.\r\n\r\nIf you have changed the path of your LoL to a new type of install (the beta installer, new language, etc) you can press the repath button on the skin to make the program rescan your LoL, and pick out the new correct locations for your skin's files.\r\n\r\n=============================================\r\nAdditional Helpful information.\r\n\r\nYou can press the \"Update File List\" button to make the program re-scan your LoL game, so it has a better idea where each file belongs.\r\n\r\nYou can press the \"Game Client\" button to tell the program what LoL install location to add skins to.\r\n\r\nMore information about the preferences options.\r\n\r\nCharacter 3d models refers to the .skn files that contain new data on the 3d shape of the character.\r\n\r\nCharacter textures refers to the .dds or tga files that contain the data of the texture and colors that are applied to the model.\r\n\r\nParticle 3d models refers to the .sco or sbc files that contain new data on the 3d shape of effects from spells or events in game.\r\n\r\nParticle textures refers to the .dds or tga files that contain the data of the color of these effects.\r\n\r\nAnimations refers to the .anim files, which contain the data for the time and movement of characters arms and legs, the .txt or .ini files which contain data on what .anim file to use for what event, and the .skl files which contain the data on what bones each model has.\r\n\r\nAir Mods refers to all files withing the \"air\" folder of your game client.  These files are what effects how the game behaves before you see the character loading screen, and contains the images of character in the character select or purchase screen, etc. Depending on which of these files is changed, it may be necessary to uninstall before a client update, and then reinstall after.\r\n\r\nSounds refers to all sound effects that you hear within game, such as the characters response to you telling them to move or attack.  These files must be fmod mp2 formatted files in the same bitrate as the ones they are replacing.  Installing these files will prevent your LoL from updating correctly if you do not uninstall them first.  PLEASE REMEMBER TO UNINSTALL then update, then reinstall, if you wish to use sound modifications.\r\n\r\nMenu and Text mods refers to the text in the game, such as character names and spell descriptions.  There is only one file for all text in the game, so installing a skin with a menu mod will mean that further updates to the text (such as new spell names) will not be included.  Keep this in mind when choosing to install them.\r\n\r\n===============================================\r\nInformation for skin installers\r\n\r\n\r\nIf you want to distribute this program and your skins at the same time, preinstalled, you may do so like this.\r\n\r\n1.) install the skins you want in your pack, make sure they all work.\r\n2.) make a copy of your installation directory\r\n3.) delete your backups folder\r\n4.) delete the \"allfiles.ini\" file\r\n5.) add a empty file (create one) named FreshInstall.LGG in the install directory.\r\n6.) zip it up and ship it out\r\n\r\nIf you want your author and skin name to be auto filled out when a user adds your folder, create a file named \"SIUinfo.txt\" and format it like this\r\n\r\nskin name: Sauron-Mord V-1.1\r\nskin author: LordGregGreg\r\nskin info: This skin turns Mordekaiser into Lord Sauron\r\n\r\n\r\nIf you want a preset preview image, name it \"SIUPreview\".extension\r\n\r\n\r\n================================================\r\nFor additional help, Please email\r\nLordGregGreg@Gmail.com\r\n";
            Cliver.Message.Show( this.Icon, help, 0, "OK");
            //new InfoForm("For help with this program\nPlease email\nLordGregGreg@Gmail.com", new Size(100, 40));
            //Cliver.Message.Inform("For help with this program\nPlease email\nLordGregGreg@Gmail.com");
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }
        private void editInstallPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreferencesForm form = new PreferencesForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }
        private void changeGameClientLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            locateGameClient_Click(sender, e);
        }
        private void updateFileListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateFileList();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*InfoForm form = new InfoForm(
                "This Program is designed to help safely install a variety of skins for the Leauge of Legengs game.\r\n" +
                "\r\nIt is a modification of the origonal program by sgun found here \r\n http://forum.leaguecraft.com/index.php?/topic/5542-tool-lol-skin-installer-skin-installation-and-management-tool \r\n" +
                "with sound installation code from here http://forum.leaguecraft.com/index.php?/topic/21301-release-lolmod \r\n" +
                "\r\n Magor Changes include" +
                "\r\n\t* Automatic Backup System" +
                "\r\n\t* Ability to install into all folders (sounds, menus, etc)" +
                "\r\n\t* Ability to only install certain parts of a skin" +
                "\r\n\t* Sound installation through fsbext found here http://aluigi.altervista.org/papers.htm " +
                "\r\n\t* It's red o.o" +
                "\r\n\r\nThis Program was modified by LGG (lordgreggreg@gmail.com) feel free to email for more info"

                ,new Size(560, 300),
                "About this Skin Installer")
            {
                StartPosition = FormStartPosition.CenterParent
            };
            form.ShowDialog();*/
            string about = "This Program is designed to help safely install a variety of skins for the League of Legends game.\r\n" +
                " http://www.leagueoflegends.com \r\n" +
                "\r\nA good website to browse such skins is http://leaguecraft.com/skins "+
                "\r\n\r\nIt is a modification of the original program by sgun found here \r\n http://forum.leaguecraft.com/index.php?/topic/5542-tool-lol-skin-installer-skin-installation-and-management-tool \r\n" +
                "With particle magic organizing code by RichieSams!\r\n"+
                "with sound installation code from here http://forum.leaguecraft.com/index.php?/topic/21301-release-lolmod \r\n" +
                "and RAF reading code from ItzWarty! here http://code.google.com/p/raf-manager/source/browse/ \r\n" +
                "and 3dModel viewing from LoLViewer here http://code.google.com/p/lolmodelviewer/ \r\n"+
                "\r\nThis program works with the new LoL installer and can auto detect where old files are supposed to go\r\n"+            
                "\r\n Major Changes include" +
                "\r\n\t* Works with new RAF format" +
                "\r\n\t* Automatic Backup System" +
                "\r\n\t* Ability to install into all folders (sounds, menus, air, etc)" +
                "\r\n\t* Ability to only install certain parts of a skin" +
                "\r\n\t* Sound installation through fsbext found here http://aluigi.altervista.org/papers.htm " +
                "\r\n\t* It's red o.o (or at least it was till a evil black cat came around)" +
                "\r\n\r\nPlease see the README file for more information and credits"+
                "\r\n\r\nThis Program was modified by LGG (lordgreggreg@gmail.com) feel free to email for more info";
            Cliver.Message.Show(this.Icon, about, 0, "OK");
            
        }
        private void soundFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cliver.Message.Inform("Sound file location is \n" +
                findSoundsFSBLocation());
        }
        public bool isValidLoLName(string inputName)
        {
            bool ret = false;
            inputName = inputName.ToLower();
            if (inputName.Contains("lol") && inputName.Contains("launcher"))
            {
                ret = true;
            }
            if (inputName.Contains("league of legends"))
            {
                ret = true;
            }
            if (inputName.Contains("lien minh huyen thoai"))
            {
                ret = true;
            }
            return ret;
        }
        public bool isValidLoLDir(string inputDir)
        {
            bool ret = false;
            if (!Directory.Exists(inputDir)) return false;
            string[] files = Directory.GetFiles(inputDir, "*", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                if (isValidLoLName(file)) ret = true;
            }
            return ret;
        }
        private void locateGameClient_Click(object sender, EventArgs e)
        {
            bool flag = true;
            string str = string.Empty;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Application.StartupPath;
            dialog.Title = @"Please locate your League of Legends\lol.launcher.exe file..";
            dialog.CheckFileExists = true;
            dialog.FileName = "lol.launcher.exe";
            dialog.Filter = "Executable (*.exe)|*.exe|All files (*.*)|*.*";

            if (
                (!isValidLoLDir(gameDirectory))
                || (Cliver.Message.Show("Confirm", SystemIcons.Question, "Current game directory is set to:\n" + gameDirectory + "\nWould you like to change it?", 0, new string[2] { "Yes", "No" }) != 1))
            {
                while ((str == string.Empty) && flag)
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string[] strArray = dialog.FileName.ToString().Split(new char[] { '\\' });
                        for (int i = 0; i < (strArray.Length - 1); i++)
                        {
                            str = str + strArray[i] + @"\";
                        }
                        if (isValidLoLName(strArray[strArray.Length - 1]))
                        {
                            gameDirectory = str;
                            //gameDirectory = gameDirectory;

                            //if (strArray[strArray.Length - 1].ToLower() == "League Of Legends.exe")
                            //{ gameDirectory = gameDirectory.Replace("game\\", "").Replace("Game\\", ""); }
                            //gameDirectory = gameDirectory.Substring(0, gameDirectory.Length - 5);
                            Properties.Settings.Default.gameDir = gameDirectory;
                            Properties.Settings.Default.Save();
                            this.CheckForUpdate(true);
                        }
                        else
                        {
                            str = string.Empty;
                            if (Cliver.Message.Show("Error", SystemIcons.Error, "Failed to locate League of Legends, do you want to try again?", 0, new string[2] { "Yes", "No" }) == 0)
                            {
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                    }
                    else if (Cliver.Message.Show("Error", SystemIcons.Error, "Failed to locate League of Legends, do you want to try again?", 0, new string[2] { "Yes", "No" }) == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }
        }
        private void clientLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cliver.Message.Inform("Client is at " + gameDirectory);
        }
        private void showDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cliver.Message.Show(this.Icon, debug.ToString(), 0, "OK");
        }
        private void label5_Click(object sender, EventArgs e)
        {
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_bit = LGGDevilLoadImage(openFileDialog1.FileName);
                if (m_bit != null)
                {
                    pictureBox1.Image = m_bit;
                }
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                LGGImageSave(m_bit, saveFileDialog1.FileName);
                //DevIL.SaveBitmap(saveFileDialog1.FileName, m_bit);
            }
        }
        private void editAllPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullPreferencesForm form = new FullPreferencesForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();

            Tao.DevIl.Il.ilSetInteger(Tao.DevIl.Il.IL_DXTC_FORMAT, string2DXInt(Properties.Settings.Default.dxFormat));
        }
        private void setSoundFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int answer = Cliver.Message.Show("Manual Sound Location Set",
                       SystemIcons.Information,
                       "Please choose how you would like to set your sound location\r\n\r\n" +
                       "1. Clear manual setting, revert to autoDetection.\r\n" +
                       "2. Browse for a file path, choose the fsb file\r\n" +
                       "3. Manually Type in a file Location by hand.",
                       0, new string[] { "1. Clear", "2. Browse", "3. Type" }
           );
            FileInfo fi = new FileInfo(gameDirectory + "\\" + findSoundsFSBLocation().Replace("\\\\", "\\"));

            if (answer == 0)
            {
                Properties.Settings.Default.manualSoundFileLocation = "";
            }
            else if (answer == 2)
            {
                InputBoxResult result = InputBox.Show("Please enter in the file location", "Sound File:",
                    fi.FullName, new InputBoxValidatingHandler(inputBox_Validating));
                if (result.OK)
                {
                    Properties.Settings.Default.manualSoundFileLocation = "\\" + result.Text.ToLower().Replace(gameDirectory.ToLower(), "").Replace("\\", "\\");
                }
            }
            else if (answer == 1)
            {
                //browse
                openFileDialog1.Filter = "All files (*.*)|*.*";
                openFileDialog1.InitialDirectory = fi.Directory.FullName;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.manualSoundFileLocation = "\\" + openFileDialog1.FileName.ToLower().Replace(gameDirectory.ToLower(), "").Replace("\\", "\\");
                }
            }
            Properties.Settings.Default.Save();
        }
        private void getProgramLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cliver.Message.Inform("I am at \r\n\r\n" + Application.ExecutablePath);
        }
        private void resetCharacterIconsCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (updateWorker2.IsBusy)
            {
                Cliver.Message.Inform("Please wait until the update is finished (progress bar bellow)");
            }
            else
                if (Directory.Exists(Application.StartupPath + "\\icons"))
                {
                    SIFileOp.DirectoryDelete(Application.StartupPath + "\\icons", true);
                }
            UpdateListView();
        }
        private void button3startLoL_Click(object sender, EventArgs e)
        {
            runthis(gameDirectory + "\\lol.launcher.exe", "", "", true);
        }
        private void useDDSVersionReaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dxtVersionReader vr = new dxtVersionReader();
            //vr.StartPosition=
            vr.Show();
        }
        private void viewDXTVersionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageVersionView ivv = new ImageVersionView(this.dxtVersions);
            ivv.Show();
        }
        private void testReadResFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.addInFiles("Tryndamere", "fx");
        }
        private void openParticleReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PartRef.ParticleReference p = new PartRef.ParticleReference();
            p.Show();
        }
        private void createDesktopShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            using (StreamWriter writer = new StreamWriter(deskDir + "\\" + "Skin Installer Ultimate" + ".url"))
            {
                string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + app);
                writer.WriteLine("IconIndex=0");
                string icon = app.Replace('\\', '/');
                writer.WriteLine("IconFile=" + icon);
                writer.Flush();
            }
            Cliver.Message.Show("Shortcut created on desktop", SystemIcons.Information,
                "The Shortcut \n\"Skin Installer Ultimate\" \nhas been created on your desktop at\n" + deskDir
                    , 0, new string[1] { "Ok", });
        }
        private void associateFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey yurixy = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(".yurixyworks");
            Microsoft.Win32.RegistryKey lolmodkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(".lolmod");
            if (yurixy == null||lolmodkey==null)
            {
                Process process = new Process();
                process.StartInfo.FileName = "sai.exe";
                process.StartInfo.Arguments = "";
                //process.StartInfo.UseShellExecute = false;
                //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WorkingDirectory = Application.StartupPath;
                process.Start();
                process.WaitForExit();
            }

        }
        #endregion
        #region repathing
        private void button3repath_Click(object sender, EventArgs e)
        {
            List<skinInfo> toRepath = new List<skinInfo>();
            foreach (ListViewItem item in this.listView1.CheckedItems)
            {
                toRepath.Add(new skinInfo(item.SubItems[1].Text,item.SubItems[2].Text));
                this.listView1.Items.Remove(item);
            }
            this.UpdateListView();
            repathThese(toRepath);
        }
        private void repathAll()
        {
            List<skinInfo> toRepath = new List<skinInfo>();
            foreach (ListViewItem item in this.listView1.Items)
            {
                toRepath.Add(new skinInfo(item.SubItems[1].Text, item.SubItems[2].Text));
                this.listView1.Items.Remove(item);
            }
            this.UpdateListView();
            repathThese(toRepath);
        }
        private void repathThese(List<skinInfo> toRepath)
        {
            String tempAllPath = Application.StartupPath + @"\t\";
            if (!Directory.Exists(tempAllPath))
            {
                Directory.CreateDirectory(tempAllPath);
            }
            foreach (skinInfo skinI in toRepath)
            {
                String skinName = skinI.name;
                String skinAuthor = skinI.author;
                String skinPath = Application.StartupPath + @"\skins\" + skinName;
                directoryPath = skinPath;
                String tempPath = Application.StartupPath + @"\t\" + skinName;
                if (Directory.Exists(skinPath))
                {
                    
                    //move skins to new path
                    this.SIFileOp.DirectoryMove(skinPath, tempPath);
                    // Directory.Delete(skinPath, true);
                }
                //remove database entry
                this.ExecuteQuery("DELETE FROM skins WHERE sName=\"" + skinName + "\"");
                //clear existing ones

                clearFiles(false);
                //set name and author in box
                this.skinNameTextbox.Text = skinName;
                this.textBoxauthor.Text = skinAuthor;
                //add it to the list
                string[] strArray3 = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                processNewDirectoryWork(strArray3);
                //add it to the database
                //button1.PerformClick();
                addToDatabase_click(this, null);
                //delete temp folder
                SIFileOp.DirectoryDelete(tempPath, true);
                //Directory.Delete(tempPath, true);
                //clear list again
                clearFiles(false);

            }
            //did them all, get rid of folder now
            SIFileOp.DirectoryDelete(Application.StartupPath + @"\t\", true);
        }
        private void repathAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            repathAll();
        }
        #endregion
        #region createAllFilesini

        private void CheckForUpdate(bool force)
        {
            DateTime lastModDate = getLastModDate();
            bool needsUpdate = true;

            if (allFilesExtensions[0] != string.Empty)
            {
                long ticks = 0;
                long.TryParse(allFilesExtensions[0], out ticks);
                if(ticks==lastModDate.Ticks) needsUpdate = false;
            }
            
            if (
                ((force) || (!File.Exists(Application.StartupPath + "\\allfiles.ini") || (needsUpdate)))
                && 
                (Cliver.Message.Show("Update Required", SystemIcons.Information, "Program data appears to be out of date, would you like to update now?\nThis process will take several minutes.\nBut will allow the program to have a better knowledge for where each file name belongs.", 0,
                new string[2] { "Yes", "No" }) == 0)
                
                )
            {
                this.UpdateFileList();
                
            }
            else
            {
                if (previewWindow == null)
                {
                    previewWindow = new PreviewWindow(gameDirectory);

                    rebuildTree();

                    if (argsToProc != "")
                    {
                        processArgs(argsToProc);
                    }

                    if (File.Exists("FreshInstall.LGG"))
                    {
                        File.Delete("FreshInstall.LGG");
                        repathAll();
                    }

                    statusText.Text = "Ready!";
                    b_IAddFiles.Enabled = true;
                    b_IAddDirectory.Enabled = true;
                }
            }
        }
        private void fileListWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string  argument = e.Argument as string;

            string gameDirectory = argument;
            baseDir = gameDirectory;//.Replace("game\\", "").Replace("Game\\", "");
            gameDirectory = baseDir;
            if (File.Exists(gameDirectory + "game\\HeroPak_client.zip"))
            {
                FileInfo info = new FileInfo(gameDirectory + "game\\HeroPak_client.zip");
                fileSize = info.Length;
            }
            else
            {
                fileSize = 0;
            }

            //gameDirectory.Substring(0,gameDirectory.Length-5);
            bool success = ExtractHeroPak(gameDirectory);
            //Thread.Sleep(2000);
            //backgroundWorker1.ReportProgress(50);
            //Thread.Sleep(2000);
            if (success)
                e.Result = "done from " + argument;
            else
            {
                e.Result = "errors";
                
            }
        }

        private void backgroundWorker1_Done(object sender, RunWorkerCompletedEventArgs e)
        {

            previewWindow = new PreviewWindow(gameDirectory);

            if (!(e.Error == null))
            {
                Cliver.Message.Inform("Something bad and unexpected has happened.. you should tell LGG so he can fix it!" +
                    "\r\nTell him what you did, and give him this information" +
                    "\r\n" + e.Error.ToString());
            }

            UpdateFL.Enabled = true;
            UpdateFL.Text = "Update File List";
            UpdateProgressSafe(0);
            string result = e.Result as string;
            debugadd(result);

            if (e.Result.ToString() != "errors")
            {
                SIFileOp.FileCopy(Application.StartupPath + "\\allfiles_temp.ini", Application.StartupPath + "\\allfiles.ini");
                SIFileOp.FileDelete(Application.StartupPath + "\\allfiles_temp.ini");

                this.ReadFilelistINI();

                if (File.Exists("FreshInstall.LGG"))
                {
                    File.Delete("FreshInstall.LGG");
                    repathAll();
                }
                if (argsToProc != "")
                {
                    processArgs(argsToProc);
                }

                statusText.Text = "Ready!";
                b_IAddFiles.Enabled = true;
                b_IAddDirectory.Enabled = true;
                //UpdateListView();

                

            }
            else
            {
                if (File.Exists(Application.StartupPath + "\\allfiles.ini"))
                {
                    this.ReadFilelistINI();
                }
                else
                {
                    Cliver.Message.Inform("This program will not be usable until it can read the LoL directory at least once\n" +
                        "\nPlease close w/e programs are running, then press the \"Update File List\" button, or restart your computer");
                    //Application.Exit();
                }
            }

            rebuildTree();
           //Cliver.Message.Inform("work is "+result);
            
            //test and maybe repath all
            
        }

        private void backgroudProgress_Changed(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                int extra = (int)e.UserState;
                UpdateProgressSafe(e.ProgressPercentage, extra.ToString() + "%");
            }
            else
            {

                UpdateProgressSafe(e.ProgressPercentage);
            }
            //Cliver.Message.Inform("progress is now" + e.ProgressPercentage);
        }
        private bool CreateAllFileListCS(string base2Dir, string fileName, string gameDirectory)
        {
            fileList = new List<string>();
            string str = string.Empty;
            //Cliver.Message.Inform("looking in " + gameDirectory + "HeroPak_client.zip");
            //string[] files = Directory.GetFiles(baseDir, "*",SearchOption.AllDirectories);
            //string[] directories = Directory.GetDirectories(baseDir);
            if (File.Exists(gameDirectory + "game\\HeroPak_client.zip"))
            {
                using (ZipFile zipFile = ZipFile.Read(gameDirectory + "game\\HeroPak_client.zip"))
                {
                    int num = 0;
                    int lastP = 0;

                    foreach (ZipEntry current in zipFile)
                    {
                        num++;
                        if (!current.FileName.Contains(".")) continue;

                        string empty = string.Empty;
                        string FileName = "game\\" + current.FileName.Replace("/", "\\");
                        string ext = FileName.Substring(FileName.LastIndexOf('.'));
                        //Cliver.Message.Inform(FileName);
                        //FileInfo st = new FileInfo(current.FileName);


                        //FileInfo st = new FileInfo(strrr);
                        if (ext != (".rar") && ext != (".7z") && ext != ".db" && ext != ".zip" && ext != "")
                        {
                            //Console.WriteLine(st.FullName);
                            //string mypath = "";

                            if (!fileExtensions.Contains(ext))
                            {
                                fileExtensions.Add(ext);
                            }
                            String niceName = FileName.ToLower();
                            String tfileName = niceName.Substring(niceName.LastIndexOf("\\") + 1);
                            niceName = tfileName + "|" + niceName.Replace("\\", "\\\\");

                            if (!fileList.Contains(niceName))
                            {
                                fileList.Add(niceName);
                                int percent = (int)Math.Floor((double)num / (double)zipFile.Count * (double)100.0);

                                //if(new Random().Next(100)>96)
                                if (percent - lastP >= 10)
                                {
                                    lastP = percent;
                                    Console.WriteLine("Please wait..." + (percent / 2).ToString() + "%");

                                    fileListWorker1.ReportProgress(percent / 2);
                                }
                            }
                        }
                    }
                }
            }
            List<String> rafFiles = new List<string>();
            try
            {
                //just read the zip file.. now we need the other stuff
                string[] files = Directory.GetFiles(baseDir, "*", SearchOption.AllDirectories);
                Directory.GetDirectories(baseDir);
                string[] array = files;
                
                int lastpP = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    if (fileListWorker1.CancellationPending)
                        return false;
                    string text = array[i];
                    FileInfo fileInfo = new FileInfo(text);

                    if (fileInfo.Extension != ".rar" && fileInfo.Extension != ".7z" && fileInfo.Extension != ".db" && fileInfo.Extension != ".zip" && fileInfo.Extension != "")
                    {
                        if (fileInfo.FullName.Contains("HeroPak_client\\"))
                        {
                            fileInfo = new FileInfo(fileInfo.FullName.Replace("HeroPak_client\\", ""));
                        }
                        String startupPath = System.IO.Path.GetDirectoryName(
                            System.Reflection.Assembly.GetExecutingAssembly().Location);

                        bool flag = false;
                        if (
                            //fileInfo.FullName.ToLower().Contains("\\skins") ||
                            fileInfo.FullName.ToLower().Contains("\\backup") ||
                            fileInfo.FullName.ToLower().Contains("\\dump") ||
                            fileInfo.FullName.ToLower().Contains("\\pack")
                            //||fileInfo.FullName.ToLower().Contains("\\managedfiles")
                            )
                        {
                            //Console.WriteLine("Creepy... not adding " + fileInfo.FullName);
                            flag = true;
                        }
                        if (fileInfo.Extension != ".raf" &&
                            fileInfo.FullName.ToLower().Contains(".raf"))
                        {
                            //dont use folders with .raf in the name 
                            flag = true;
                        }
                        if (
                            fileInfo.FullName.ToLower().Contains("filearchives")
                            &&

                               (
                                  fileInfo.FullName.ToLower().Contains("data")
                                 || fileInfo.FullName.ToLower().Contains("levels")
                               )
                            )
                        {
                            flag = true;
                        }

                        if (!fileExtensions.Contains(fileInfo.Extension))
                        {
                            fileExtensions.Add(fileInfo.Extension);
                        }
                        string lowerFirstName = fileInfo.FullName.ToLower();
                        string relativePath = lowerFirstName.Substring(baseDir.Length - 1);
                        string lowerName = fileInfo.Name.ToLower();
                        string text4 = lowerName + "|" + relativePath.Replace("\\", "\\\\");
                        int depth = (relativePath.Split('\\')).Length;
                        //if (depth <= 2) flag = true;

                        if (!fileList.Contains(text4))
                        {
                            if (!flag)
                            {
                                if (fileInfo.Extension == ".raf")
                                {
                                    if (fileInfo.FullName.Contains("\\filearchives\\"))
                                    {
                                        if (File.Exists(fileInfo.FullName + ".dat"))
                                        {
                                            rafFiles.Add(fileInfo.FullName);//add raf files to read afterwards
                                        }

                                    }
                                }

                                fileList.Add(text4);
                            }
                            int percent = (int)Math.Floor((double)i / (double)array.Length * (double)100.0);
                            //if(new Random().Next(100)>96)
                            if (percent - lastpP >= 10)
                            {
                                lastpP = percent;
                                //Console.WriteLine("Please wait..." + ((percent / 2) + 0).ToString() + "%");
                                fileListWorker1.ReportProgress(percent);
                            }
                        }
                    }
                    else if (fileInfo.Extension == ".zip")
                    {
                        //we need to extract these..and like.. keep track of them
                        string lowerFirstName = fileInfo.FullName.ToLower();
                        string relativePath = lowerFirstName.Substring(baseDir.Length - 1);
                        using (ZipFile zipFile = ZipFile.Read(fileInfo.FullName))
                        {
                            int num = 0;
                            int lastzP = 0;
                            
                            foreach (ZipEntry current in zipFile)
                            {
                                num++;
                                if (!current.FileName.Contains(".")) continue;

                                string empty = string.Empty;
                                string FileName = relativePath+"\\" + current.FileName.Replace("/", "\\");
                                string ext = FileName.Substring(FileName.LastIndexOf('.'));
                                //Cliver.Message.Inform(FileName);
                                //FileInfo st = new FileInfo(current.FileName);


                                //FileInfo st = new FileInfo(strrr);
                                if (ext != (".rar") && ext != (".7z") && ext != ".db" && ext != ".zip" && ext != "")
                                {
                                    //Console.WriteLine(st.FullName);
                                    //string mypath = "";

                                    if (!fileExtensions.Contains(ext))
                                    {
                                        fileExtensions.Add(ext);
                                    }
                                    String niceName = FileName.ToLower();
                                    String tfileName = niceName.Substring(niceName.LastIndexOf("\\") + 1);
                                    niceName = tfileName + "|" + niceName.Replace("\\", "\\\\");

                                    if (!fileList.Contains(niceName))
                                    {
                                        fileList.Add(niceName);
                                        int zpercent = (int)Math.Floor((double)num / (double)zipFile.Count * (double)100.0);
                                        float zpartWorth = 100.0f / (float)array.Length;
                                        int percent =1+ (int)((Math.Floor((double)i / (double)array.Length * (double)100.0))+((float)zpercent*zpartWorth));
                                        
                                        //if(new Random().Next(100)>96)
                                       if (percent - lastpP >= 1||zpercent-lastzP>=1)
                                        {
                                            lastzP = zpercent;
                                           lastpP = percent;

                                            fileListWorker1.ReportProgress(percent ,zpercent);
                                            Console.WriteLine("Please wait..." + (percent ).ToString() + "%");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            int inc = 0;
            bool openflag = false;
                              
            foreach (String file in rafFiles)
            {
                int percent = (int)Math.Floor((double)inc++ / (double)(rafFiles.Count+1) * (double)100.0);
                
                if (fileListWorker1.CancellationPending)
                    return false;
                
                FileInfo rafFile = new FileInfo(file);
                //time to process the raf files
                try
                {
                    RAFArchive raf = new RAFArchive(rafFile.FullName);

                    //RAF raf = new RAF(rafFile.FullName);
                    FileStream fStream = raf.GetDataFileContentStream();
                    List<RAFFileListEntry> filez = raf.GetDirectoryFile().GetFileList().GetFileEntries();
                    int innerInc = 0;
                    int lastP = 0;
                    foreach (RAFFileListEntry entry in filez)
                    {
                        if (fileListWorker1.CancellationPending)
                        {
                            fStream.Close();
                            return false;
                        }

                        int innerpercent = (int)Math.Floor((double)innerInc++ / (double)filez.Count * (double)(50.0/(double)rafFiles.Count)/*100.0*/);
                        if (lastP != innerpercent)
                        {
                            lastP = innerpercent;
                            fileListWorker1.ReportProgress(50 + (percent / 2) + (innerpercent / 1));
                        }
                        FileInfo innerRafFile = new FileInfo(rafFile.FullName + "\\\\" + entry.FileName);
                        if (!fileExtensions.Contains(innerRafFile.Extension))
                        {
                            fileExtensions.Add(innerRafFile.Extension);
                        }
                        string text2 = innerRafFile.FullName.ToLower();
                        string text3 = innerRafFile.Name.ToLower();
                        string text4 = text3 + "|" + text2.Substring(baseDir.Length - 1).Replace("\\", "\\\\");
                        if (!fileList.Contains(text4))
                        {
                            fileList.Add(text4);
                            //temp.Append("\r\n" + entry.GetFileName);
                        }
                    }
                    fStream.Close();
                }
                catch 
                {
                    debugadd("Error, unable to open file " + file);
                    //warn here!!
                    openflag = true;
                
                }
                
            }
            if (openflag)
            {
                if (Cliver.Message.Show("Error Reading LoL Directory Raf Files", SystemIcons.Error, "Warning, there was a error opening the raf files!\n\n" +
                     "This means that some other program is using them RIGHT NOW\n\nPlease, make sure that LoL is not in a game, and that other programs are closed (like rafmanager)\n"+
                     "It may also help to right click -> run as administrator, this program as well\n\n" +
                     "If this does not work, please restart your computer", 0, new string[] { "OK!"})==0)// "I think this error is wrong and I want to Cause Hell" }) == 0)
                {
                    //Application.Exit();
                }
            }

            debugadd("Please wait...writing to disk");
            TextWriter tw = new StreamWriter(fileName);
            
            tw.WriteLine(getLastModDate().Ticks.ToString()+ "|" + string.Join("|", fileExtensions.ToArray()));
            foreach (string str3 in fileList.ToArray())
            {
                if (fileListWorker1.CancellationPending)
                {
                    tw.Dispose();
                    tw.Close();
                    return false;
                }
                if (str3 != string.Empty)
                {
                    tw.WriteLine(str3);
                }
            }
            tw.Dispose();
            tw.Close();
            if (!File.Exists(Application.StartupPath + "\\allfiles_temp.ini"))
            {
                Cliver.Message.Inform("Error, not able to write to a file o.o!" +
                    "\r\nThis probably means that this program needs administrator rights.." +
                    "\r\nTry closing this program, then start it up again, but this time, do" +
                    "\r\nRight Click-> Run as Administrator" +
                    "\r\nIf this does not fix the issue, try moving this program to a different directory,\r\nlike inside your documents folder");
            }

            //SIFileOp.FileCopy(Application.StartupPath+"\\allfiles_temp.ini",Application.StartupPath+ "\\allfiles.ini");
            //SIFileOp.FileDelete(Application.StartupPath + "\\allfiles_temp.ini");
            return !openflag;
        }

        private bool ExtractHeroPak(string gameDirectory)
        {
            if (Directory.Exists(gameDirectory + "HeroPak_client"))
            {
                try
                {
                    debugadd("Deleting exiting directory...   ");
                    Directory.Delete(gameDirectory + "HeroPak_client", true);
                    debugadd("COMPLETE!\n");
                }
                catch (Exception exception)
                {
                    Console.WriteLine("\n" + exception.Message);
                }
            }
            debugadd("Extracting files...   ");
            //ZipUtil.UnZipFiles(gameDirectory + "HeroPak_client.zip", gameDirectory + "HeroPak_client", "", false);

            debugadd("COMPLETE!\n");
            debugadd("Creating .ini files....   ");

            String searchDir = gameDirectory;//.Replace("game\\", "").Replace("Game\\", "");//.Substring(0, gameDirectory.Length - 5);

            debugadd("Game dir is " + gameDirectory + " and basedir is " + baseDir + " and search dir is " + searchDir);
            debugadd("Creating .ini files from " + searchDir + "....   ");

            bool success = CreateAllFileListCS(searchDir, Application.StartupPath + "\\allfiles_temp.ini", gameDirectory);
            //Thread.Sleep(0x1388);
            debugadd("COMPLETE!\n");
            if (Directory.Exists(gameDirectory + "game\\HeroPak_client"))
            {
                try
                {
                    debugadd("Deleting extraction directory...   ");
                    Directory.Delete(gameDirectory + "game\\HeroPak_client", true);
                    debugadd("COMPLETE!\n");
                }
                catch (Exception exception2)
                {
                    debugadd("\n" + exception2.Message);
                }
            }
            return success;
        }
#endregion
        #region DDSSavingLoading
        public Bitmap LGGDevilLoadImage(string fileName)
        {
            int imageID;
            Tao.DevIl.Il.ilGenImages(1, out imageID);
            Tao.DevIl.Il.ilBindImage(imageID);

            //FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Write, FileShare.None);
            //bool loaded= Tao.DevIl.Il.ilLoadF(Tao.DevIl.Il.IL_PNG,fileName);
            //bool loaded= Tao.DevIl.Il.ilLoad(Tao.DevIl.Il.IL_PNG,fileName);
            bool loaded = Tao.DevIl.Il.ilLoadImage(fileName);
            if (!loaded)
            {
                debugadd("error loading image");
                int aerror = Tao.DevIl.Il.ilGetError();
                int op1 = Tao.DevIl.Il.IL_INVALID_ENUM;
                int op2 = Tao.DevIl.Il.IL_COULD_NOT_OPEN_FILE;
                int op3 = Tao.DevIl.Il.IL_INTERNAL_ERROR;
                int op4 = Tao.DevIl.Il.IL_OUT_OF_MEMORY;
                int op5 = Tao.DevIl.Il.IL_INVALID_EXTENSION;
                return new Bitmap(2, 2);// null;
            }
            int aerror2 = Tao.DevIl.Il.ilGetError();
            int iW = Tao.DevIl.Il.ilGetInteger(Tao.DevIl.Il.IL_IMAGE_WIDTH);
	        int iH = Tao.DevIl.Il.ilGetInteger(Tao.DevIl.Il.IL_IMAGE_HEIGHT);
           // Tao.DevIl.Il.il

            Bitmap bit = new Bitmap(
                iW,iH,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            
            Rectangle rect = new Rectangle(0, 0,iW,iH);

            System.Drawing.Imaging.BitmapData pBd = bit.LockBits(rect,System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            IntPtr pScan0 = pBd.Scan0;

            Tao.DevIl.Il.ilConvertImage(Tao.DevIl.Il.IL_BGRA,Tao.DevIl.Il.IL_UNSIGNED_BYTE); // support for non 32bit images..
            Tao.DevIl.Il.ilCopyPixels(0, 0, 0, iW, iH, 1, Tao.DevIl.Il.IL_BGRA, Tao.DevIl.Il.IL_UNSIGNED_BYTE, pScan0);

            Tao.DevIl.Il.ilDeleteImages(1,ref imageID);

            bit.UnlockBits(pBd);

            return bit;

        }
        public bool LGGNvidiaSave(Bitmap bit, string fileName, string dtxType = "u888", int numMipMaps =-1)
        {
            //First save a targa somewhere and then delete it once we are done...
            string tgaFileName = fileName.Substring(0,fileName.LastIndexOf(".")) + ".tga";
            int ImageId;
            Tao.DevIl.Il.ilGenImages(1, out ImageId);
            Tao.DevIl.Il.ilBindImage(ImageId);

            int iW = bit.Width;
            int iH = bit.Height;
            Rectangle rect = new Rectangle(0, 0, iW, iH);
            bit.RotateFlip(RotateFlipType.RotateNoneFlipY);

            System.Drawing.Imaging.BitmapData pBd = bit.LockBits(rect,
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            IntPtr pScan0 = (pBd.Scan0);

            bool bSuccess = Tao.DevIl.Il.ilTexImage(iW, iH, 1, 4, Tao.DevIl.Il.IL_BGRA,
                Tao.DevIl.Il.IL_UNSIGNED_BYTE, pScan0);
            if (!bSuccess)
            {
                return false;
            }
            bool bRes = Tao.DevIl.Il.ilSaveImage(tgaFileName);            
            Tao.DevIl.Il.ilDeleteImages(1, ref ImageId);
            string mipMapPart = "";
            if (numMipMaps != -1)
            {
                if(numMipMaps==0)
                {
                    mipMapPart=" -nomipmap";
                }else
                {
                    mipMapPart = " -nmips " + numMipMaps.ToString();
                }
            }

            bit.UnlockBits(pBd);
            bit.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //now convert that tga to a dds
            Process process = new Process();
            process.StartInfo.FileName = "nvdxt.exe";
            process.StartInfo.Arguments = " -file \"" + tgaFileName + "\" -" + dtxType + ""+mipMapPart+" -outfile \""+fileName+"\"";
            debugadd(process.StartInfo.Arguments);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Application.StartupPath;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            //get rid of the tga
            File.Delete(tgaFileName);
            return true;
        }
        public bool LGGImageSave(Bitmap bit, string fileName, int dXTFileType = -1, int numMipMaps = -1, int bits = 32)
        {
            string fileSaveType = "dxt5";
            switch (dXTFileType)
            {
                case 0:
                    switch (bits)
                    {
                        case 24: fileSaveType = "u888"; break;
                        case 32: fileSaveType = "u8888"; break;
                        default: fileSaveType = "u888"; break;
                    }
                    break;
                case 1: fileSaveType  = "dxt1c"; break;
                case 3: fileSaveType  = "dxt3";  break;
                case 5: fileSaveType  = "dxt5";  break;
                default: fileSaveType = "dxt5";  break;
            }
            return LGGNvidiaSave(bit, fileName, fileSaveType,numMipMaps);
            
        }
        public bool LGGDevilSave(Bitmap bit, string fileName, int dXTFileType = -1, int numMipMaps = -1, int bits = 32)
        {
            int fileSaveType = Tao.DevIl.Il.IL_DXT5;
            switch (dXTFileType)
            {
                case 0: fileSaveType = Tao.DevIl.Il.IL_DXT_NO_COMP; ; break;
                case 1: fileSaveType = Tao.DevIl.Il.IL_DXT1; break;
                case 2: fileSaveType = Tao.DevIl.Il.IL_DXT2; break;
                case 3: fileSaveType = Tao.DevIl.Il.IL_DXT3; break;
                case 4: fileSaveType = Tao.DevIl.Il.IL_DXT4; break;
                case 5: fileSaveType = Tao.DevIl.Il.IL_DXT5; break;
                default: fileSaveType = Tao.DevIl.Il.IL_DXT5; break;
            }
            if (fileSaveType == Tao.DevIl.Il.IL_DXT_NO_COMP)
            {
                return LGGNvidiaSave(bit, fileName, "u888");
            }

            Tao.DevIl.Il.ilSetInteger(Tao.DevIl.Il.IL_DXTC_FORMAT, fileSaveType);
            int ImageId;
            Tao.DevIl.Il.ilGenImages(1, out ImageId);
            Tao.DevIl.Il.ilBindImage(ImageId);

            int iW = bit.Width;
            int iH = bit.Height;
            Rectangle rect = new Rectangle(0, 0, iW, iH);
            bit.RotateFlip(RotateFlipType.RotateNoneFlipY);

            System.Drawing.Imaging.BitmapData pBd = bit.LockBits(rect,
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            IntPtr pScan0 = (pBd.Scan0);

            bool bSuccess = Tao.DevIl.Il.ilTexImage(iW, iH, 1, 4, Tao.DevIl.Il.IL_BGRA,
                Tao.DevIl.Il.IL_UNSIGNED_BYTE, pScan0);
            if (!bSuccess)
            {
                return false;
            }

            bool bRes = Tao.DevIl.Il.ilSaveImage(fileName);
            //bool bRes = Tao.DevIl.Il.ilSave(fileSaveType,fileName);
            if (!bRes)
            {

                int aerror = Tao.DevIl.Il.ilGetError();
                int op1 = Tao.DevIl.Il.IL_INVALID_ENUM;
                int op2 = Tao.DevIl.Il.IL_COULD_NOT_OPEN_FILE;
                int op3 = Tao.DevIl.Il.IL_INTERNAL_ERROR;
                int op4 = Tao.DevIl.Il.IL_OUT_OF_MEMORY;
            }
            Tao.DevIl.Il.ilDeleteImages(1, ref ImageId);

            bit.UnlockBits(pBd);
            bit.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bRes;
        }
        private void copyAndFix(string origination, string destination)
        {//Application.StartupPath + @"\st\" + str4 + @"\" + str5
            FileInfo fi_orig = new FileInfo(origination);
            FileInfo fi_dest = new FileInfo(destination);
            if (fi_orig.Extension == ".dds" && (Properties.Settings.Default.ddsResizeToSD || Properties.Settings.Default.ddsForceRiotFormat))
            {
                Dictionary<string, int> origonalInfo = commonOps.readDDSInfoNvidia(fi_orig.FullName);
                int origFile_dxtv = origonalInfo["dxtv"];
                int origFile_width =origonalInfo["width"];
                int origFile_height = origonalInfo["height"];
                int origFile_mipMaps = origonalInfo["mipmap count"];
                int origFile_bitCount = origonalInfo["bit count"];
                long origFile_fileSize = fi_orig.Length;
                int newFile_dxtv = -1;
                int newFile_width = -1;
                int newFile_height = -1;
                int newFile_mipMaps = -1;
                int newFile_bitCount = -1;
                long newFile_fileSize = -1;
                string fiDestPath = "\\"+getAfterRafName(fi_dest.FullName).ToLower();
                //+		fi_dest	{C:\Users\LGG\Desktop\g\LoL\Skin Installer Ultimate\bin\debug\st\\\rads\projects\lol_game_client\filearchives\0.0.0.142\archive_65415616.raf\data\characters\annie\skins\base\annieloadscreen.dds}	System.IO.FileInfo
                if (dxtVersions.ContainsKey(fiDestPath))
                {
                    Dictionary<String, int> infos = dxtVersions[fiDestPath];
                    newFile_dxtv = infos["dxtv"];
                    newFile_width = infos["width"];
                    newFile_height = infos["height"];
                    newFile_mipMaps = infos["mipmaps"];
                    newFile_bitCount = infos["bitcount"];
                    newFile_fileSize = infos["filesize"];
                }

                //hack to prevent trasnp rewritting, 3s will all be 5
                int origComparator = (origFile_dxtv == 3) ? 5 : origFile_dxtv;
                int newComparator = (newFile_dxtv == 3) ? 5 : newFile_dxtv;

                if (((origComparator == newComparator) &&
                    (origFile_width == newFile_width) &&
                    (origFile_mipMaps == newFile_mipMaps) &&
                    (origFile_bitCount == newFile_bitCount) &&
                    (origFile_fileSize == newFile_fileSize) &&  
                    (origFile_height == newFile_height))
                    || (newComparator == -1)
                    || (newFile_width == -1)
                    || (newFile_height == -1)
                    || (newFile_bitCount == -1)
                    || (newFile_mipMaps == -1)
                    || (newFile_fileSize == origFile_fileSize))
                {
                    this.SIFileOp.FileCopy(origination, destination);
                    debugadd("No Need to re-write " + origination+ " , riot file is "+newFile_dxtv.ToString()+" and input was "+origFile_dxtv.ToString());
                }
                else
                {
                    debugadd("attempting to verify the integredy of " + origination);
                    Bitmap bb = null;
                    try
                    {
                        bb = LGGDevilLoadImage(origination);
                    }
                    catch
                    {
                        debugadd("Something went wrong in file conversion opening" + origination);
                    }

                    if (bb != null)
                    {
                        try
                        {
                            if ((origFile_width != newFile_width) || (origFile_height != newFile_height))
                            {
                                if(Properties.Settings.Default.ddsResizeToSD)
                                {

                                
                                    bb = commonOps.ResizeImage(bb, new System.Drawing.Size(newFile_width, newFile_height));

                                    debugadd(destination + " was saved correctly from dim " + origFile_width.ToString() + " x "+origFile_height.ToString() +
                                    " to what the origonal riot file is at dim " + newFile_width.ToString()+" x "+newFile_height.ToString());
                                }
                            }
                            int dest_dxtv = origFile_dxtv;
                            int dest_mipMaps = origFile_mipMaps;
                            int dest_bitCount = origFile_bitCount;
                            if(Properties.Settings.Default.ddsForceRiotFormat)
                            {
                                dest_dxtv=newFile_dxtv;
                                dest_mipMaps = newFile_mipMaps;
                                dest_bitCount = newFile_bitCount;
                            }

                            if (LGGImageSave(bb, destination, dest_dxtv, dest_mipMaps,dest_bitCount))
                            {
                                debugadd(destination + " was saved correctly from dxt "+origFile_dxtv.ToString()+" to what the origonal riot file is at dxt "+newFile_dxtv.ToString());
                            }
                        }
                        catch
                        {
                            debugadd("Something went wrong in file conversion saving to " + destination);
                            this.SIFileOp.FileCopy(origination, destination);

                        }
                    }
                    else
                    {
                        debugadd("looks like something else went wrong opening " + origination);
                        this.SIFileOp.FileCopy(origination, destination);
                    }
                }
                
            
            }
            else
            {
                this.SIFileOp.FileCopy(origination, destination);
            }
            File.SetAttributes(destination, FileAttributes.Normal);                                    
        }
        #endregion DDSSavingLoading
        #region GLPaint
        void glControl1_Resize(object sender, EventArgs e)
        {
            SetupViewport();
        }
        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }
        void Application_Idle(object sender, EventArgs e)
        {
            // no guard needed -- we hooked into the event in Load handler
            // while (glControl1.IsIdle)
            //{
            //rotation += 1;
            // if ((DateTime.Now - frameTime).TotalMilliseconds < (1000 / 60))
            //   { }
            // else
            // { Render(); }
            //}
        }
        
        private void glControl1_Load(object sender, EventArgs e)
        {
           try
            {
	
	            GL.ClearColor(Color.Black);
	            SetupViewport();
	            frameTime = DateTime.Now;
	            timer1.Start();
	            // Setup GL state for ordinary texturing.
	            TexUtil.InitTexturing();
	
	            // Load a bitmap from disc, and put it in a GL texture.
	             lggsiu1tex = TexUtil.CreateTextureFromFile(Application.StartupPath + "//LGGSIU1.bmp");
	             loaded = true;
	            
	            // Create a TextureFont object from the loaded texture.
	            //lggsiu1Font = new TextureFont(tex);
            }
           catch 
           {
               loaded = false;
           }

        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            
            Render();
               
        }
        double shiftedByTime(double r)
        {
            int intev =(int)Math.Round((( DateTime.Now - started).TotalMilliseconds)%1000.0);
            r += ((float)intev / 1000.0);//plus 0to2, making range  0 to 3 with 2 effective
            return (Math.Sin(r *2* Math.PI)*.5)+.5;
        }

        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
        private void centerline(double hue,int x, double h, int max, Random r)
        {
            double v = shiftedByTime(r.NextDouble());
            //GL.Color3(ColorFromHSV(hue, 1,v));
            Color c = ColorFromHSV(hue, 1,1);
            Color c2 = Color.FromArgb((int)(v*255), c);
            GL.Color4(c2);
                
            int len =(int)Math.Floor((double)max * h);
            int space =( max - len )/ 2;
            GL.Vertex2(x, space);
            GL.Vertex2(x, len+space);
            if (Properties.Settings.Default.graficsGlow)
            {

                GL.Color4(Color.FromArgb((int)(50 * v), Color.White));
                len = (int)Math.Floor((double)len / 2.1);
                space = (max - len) / 2;
                //GL.Color3(ColorFromHSV(hue, .95, v));
                GL.Vertex2(x, space);
                GL.Vertex2(x, len + space);

                len = (int)Math.Floor((double)len / 1.3);
                space = (max - len) / 2;
                //GL.Color3(ColorFromHSV(hue, .8, v));
                GL.Vertex2(x, space);
                GL.Vertex2(x, len + space);

                len = (int)Math.Floor((double)len / 1.7);
                space = (max - len) / 2;
                //GL.Color3(ColorFromHSV(hue, .5, v));
                GL.Vertex2(x, space);
                GL.Vertex2(x, len + space);
                len = (int)Math.Floor((double)len / 2.0);
                space = (max - len) / 2;
                //GL.Color3(ColorFromHSV(hue, .2, v));
                GL.Vertex2(x, space);
                GL.Vertex2(x, len + space);
            }
            
        }
        private void Render()
        {
            if (!loaded) // Play nice
            return;
            if (previewWindow!=null)
                if(previewWindow.Visible) return;

            if (( DateTime.Now-frameTime).TotalMilliseconds < (1000/60)) return;
            
            try
            {
	            frameTime = DateTime.Now;
	            //image
	            GL.Clear(ClearBufferMask.ColorBufferBit| ClearBufferMask.DepthBufferBit);
	            GL.MatrixMode(MatrixMode.Modelview);
	            GL.LoadIdentity();
	            int w = glControl1.Width;
	            int h = glControl1.Height;
	
	
	            GL.PushMatrix();
	
	            TexUtil.InitTexturing();
	            double tth = ((double)((DateTime.Now - started).TotalMilliseconds) / 5000.0) % 1.0;
	            double vv = Math.Sin(tth * 2 * Math.PI) * .07;
	            double ttth = ((double)((DateTime.Now - started).TotalMilliseconds) / 24000.0) % 1.0;
	            int vvv = (int)Math.Floor((Math.Sin(ttth * 2 * Math.PI) *.5+.5)*255
	                *((double)Properties.Settings.Default.lgglogostrangth/100.0));
	
	            GL.Color3(Color.FromArgb(vvv,vvv,vvv));         
	            GL.BindTexture(TextureTarget.Texture2D, lggsiu1tex);
	            GL.Begin(BeginMode.Quads);
	            double x1 =( w / 4) + (w * vv / 4);
	            double x2 = (w / 4 * 3) - (w * vv / 4);
	            GL.TexCoord2(0, 1); GL.Vertex3(x1, 0,1);
	            GL.TexCoord2(1, 1); GL.Vertex3(x2, 0,1);
	            GL.TexCoord2(1, 0); GL.Vertex3(x2, h,1);
	            GL.TexCoord2(0, 0); GL.Vertex3(x1, h,1);
	            GL.End();
	            GL.Disable(EnableCap.Texture2D);
	            GL.PopMatrix();
	
	            if (Properties.Settings.Default.drawGraficsLines)
	            {
	                GL.Color3(Color.White);
	                double longt2h = ((double)((DateTime.Now - started).TotalMilliseconds) / 5000.0) % 1.0;
	
	                GL.Rotate(
	                   Math.Sin(longt2h * 2 * Math.PI) * 5.0
	                , 0, 0, 1);
	
	                GL.PushMatrix();
	                GL.Begin(BeginMode.Lines);
	                Random r = new Random(1);
	                for (int x = 0; x < w; x++)
	                {
	                    double hue = (float)x / 3.0;
	                    double longth = ((double)((DateTime.Now - started).TotalMilliseconds) / 20000.0) % 1.0;
	                    double hueh = ((double)((DateTime.Now - started).TotalMilliseconds) / 11000.0) % 1.0;
	
	                    double ll = (Math.Sin(longth * Math.PI * 2) * .5) + .5;
	                    double huel = (Math.Sin(hueh * Math.PI * 2) * .7) + .5;
	
	                    hue -= huel * 150;
	                    if (hue < 0) hue = 0;
	                    if (hue > 360) hue = 360;
	
	                    //this.helpText.Text = longth.ToString();
	                    centerline(hue, x, (Math.Sin((x / 20.0 * ll) + ll * 40) * 0.40) + 0.6 + (r.NextDouble() * ll), h, r);
	                }
	                GL.End();
	                GL.PopMatrix();
	            }
            }
            catch 
            {
                loaded = false;
            }
            try { glControl1.SwapBuffers(); }
            catch //(System.Exception ex)
            { }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (glControl1.IsIdle) Render();
        }
        #endregion
        #region autoUpdate
        private delegate void ShowModalDialogHandler(Form form);
        private void updateWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                debugadd(e.Error.ToString());
            }
            else
                debugadd("Update Check Completed");
        }
        private void updateWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] oa = e.Argument as object[];
            skinInstaller parent = oa[0] as skinInstaller;
            bool force = (bool)oa[1];
            Double myVersion = Double.Parse(myCurrentVersion, System.Globalization.NumberFormatInfo.InvariantInfo);//take care of locale shit
            System.Net.WebClient client = new WebClient();
            try
            {
                string response = client.DownloadString(dlURL).Trim();
                string[] responceParts = response.Split('|');
                Double version = Double.Parse(responceParts[0], System.Globalization.NumberFormatInfo.InvariantInfo);
                string link = responceParts[1];
                string info = "No Extra Info Provided This Time.";
                if(responceParts.Length>=3)
                    info = responceParts[2];
                //Double version = Double.Parse(response.Substring(0, response.IndexOf("|")));
                //string link = response.Substring(response.IndexOf("|") + 1);
                if (((version > myVersion) && (version>Properties.Settings.Default.ignoreUpdatesVersion))||force)
                {
                    //new update
                   // Cliver.Message.Inform("New Update Availiable\r\nDownload Version " + version.ToString() + " from \r\n" +
                   //     link);
                    UpdateAvailiableForm1 form = new UpdateAvailiableForm1();
                    form.setData(link, info, version, myVersion);
                    //form.textBox2updateurl.Text = link;
                  //form.textBox1updateinfo.Text = info;
                    //form.textBox1newVersio.Text = version.ToString();
                    //form.textBox1currentVersion.Text = myVersion.ToString();
                    form.StartPosition = FormStartPosition.CenterParent;
                    
                    //(sender as skinInstaller).BeginInvoke(new ShowModalDialogHandler(ShowModalDialog), new object[] { form });
                    form.CustShowDialog(parent);
                    //dialog.ShowDialog();

                }
            }
            catch (Exception ee)
            {
                //Console.WriteLine("Update Check failed");
                Console.WriteLine(ee.ToString());
                
            }

        }
        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!updateWorker2.IsBusy)
            {
                Properties.Settings.Default.ignoreUpdatesVersion = 0.0;
                updateWorker2.RunWorkerAsync(new object[]{this,true});
            }
        }
        #endregion
        #region dragandDropstuff
        private void skinInstaller_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void skinInstaller_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> usable = new List<string>();
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    //SIFileOp.FileCopy(fi.FullName, Application.StartupPath + "\\dragtemp\\" +
                      //  fi.Name);      
                    FileAttributes attr = File.GetAttributes(fi.FullName);

                    //detect whether its a directory or file
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        directoryPath = fi.FullName;
                        string[] strArray3 = Directory.GetFiles(fi.FullName, "*.*", SearchOption.AllDirectories);
                        foreach (string str in strArray3)
                        {
                            usable.Add(str);
                        }
                    }
                    else
                    {
                        directoryPath = fi.DirectoryName;
                        usable.Add(fi.FullName);
                    }

                    
                }
                //string[] strArray3 = Directory.GetFiles(tempPath, "*.*", SearchOption.AllDirectories);
                tabControl1.SelectedIndex=0;
                //checked, not ok
                processNewDirectory(usable.ToArray()    ,true          );
                
                //add it to the database
                //
                //button1_Click(this, null);
                //delete temp folder
                //Directory.Delete(tempPath, true);
            }
        }
        private void skinInstaller_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        #endregion
        #region statsStuff
        private void webPinger_DoWork(object sender, DoWorkEventArgs e)
        {
            string url = e.Argument as string;
            System.Net.WebClient client = new WebClient();
            try
            {
                client.DownloadData(url);
                

            }
            catch { }
                
        }

        private void pingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.sendStats)
            {
                                       //"http://c.statcounter.com/6898201/0/e70b18ab/0/"
                webPinger.RunWorkerAsync("http://c.statcounter.com/6898201/0/e70b18ab/0/");
            }
        }

        private void viewStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string statsurl = "http://statcounter.com/project/standard/stats.php?project_id=6898201&guest=1";
            System.Diagnostics.Process.Start(statsurl);
        }

        private void timeupdatecount_Tick(object sender, EventArgs e)
        {
            if(!backgroundWorkerCountUpdate.IsBusy)
                backgroundWorkerCountUpdate.RunWorkerAsync();
        }
        private void webPinger_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                debugadd(e.Error.ToString());
            }
            else
                debugadd("Ping Completed");
        }

        private void backgroundWorkerCountUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            //client.DownloadData(
            try
            {
                string url = "http://c.statcounter.com//counter.php" +
                    "?displayonly=1&" +
                    "delcache=1&" +
                    "rand=1306291246509&" +
                    "fg_red=255&" +
                    "fg_green=0&" +
                    "fg_blue=0&" +
                    "bg_red=0&" +
                    "bg_green=0&" +
                    "bg_blue=0&" +
                    "min_num_digits=3&" +
                    "transparent=0&" +
                    "counter_option=3&" +
                    "sc_project=6898201";
                pictureBoxCount.Load(url);
            }
            catch (Exception xe)
            {
                debugadd(xe.ToString());
            }
            //Image i = new Image(
        }

        private void pictureBoxCount_Click(object sender, EventArgs e)
        {
            if (!backgroundWorkerCountUpdate.IsBusy)
                backgroundWorkerCountUpdate.RunWorkerAsync();
        }
        #endregion  
        #region listviewmods
        #region mouseHoverStuff
        public string[] readInfoFile(string fileName)
        {
            string skinName = "";
            string skinAuthor = "";
            string skinInfo = "";
            string[] strArray = new string[2];

            if (File.Exists(fileName))
            {
                TextReader reader = new StreamReader(fileName);
                while (reader.Peek() != -1)
                {

                    strArray = reader.ReadLine().Split(new char[] { ':' });
                    if (strArray[0].ToLower().Trim() == "skinname" || strArray[0].ToLower().Trim() == "skin name")
                    {
                        skinName = strArray[1].Trim();
                    }
                    if (strArray[0].ToLower().Trim() == "skinauthor" || strArray[0].ToLower().Trim() == "skin author")
                    {
                        skinAuthor = strArray[1].Trim();
                    }
                    if (strArray[0].ToLower().Trim() == "skininfo" || strArray[0].ToLower().Trim() == "skin info")
                    {
                        skinInfo = strArray[1].Trim().Replace("[New Line]", "\r\n").Replace("[Colon]", ":");
                    }

                }
                reader.Close();

            }
            return new string[] { skinName, skinAuthor, skinInfo };
        }
        //returns a the number 0-9 if name has it, default otherwise
        int getNumberWorth(String name, int def)
        {
            int r = def;
            foreach (Char c in name)
            {

                if (Char.IsNumber(c))
                {
                    r = Convert.ToUInt16(c.ToString());
                }
            }
            return r;
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //fire up bigger preview window
            imagePreviewForm.Text = labelSkinName.Text + " Preview";
            imagePreviewForm.pictureBox1.Image = pictureBox2.Image;
            imagePreviewForm.Show();
        }
        private bool tbBusy = false;
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (tbBusy) return;
            tbBusy = true;
            TextBox tb = sender as TextBox;
            //if (tb.Text.Length > 20) tb.ScrollBars = ScrollBars.Vertical;
            //else tb.ScrollBars = ScrollBars.None;
            Size tS = TextRenderer.MeasureText(tb.Text, tb.Font);
            bool Hsb = tb.ClientSize.Height < ((tS.Width + Convert.ToInt32(tb.Font.Size))
                / tb.ClientSize.Width) * (tS.Height + Convert.ToInt32(tb.Font.Size));
            bool Vsb = tb.ClientSize.Width < tS.Width;

            if (Hsb && Vsb)
                tb.ScrollBars = ScrollBars.Both;
            else if (!Hsb && !Vsb)
                tb.ScrollBars = ScrollBars.None;
            else if (Hsb && !Vsb)
                tb.ScrollBars = ScrollBars.Vertical;
            else if (!Hsb && Vsb)
                tb.ScrollBars = ScrollBars.Horizontal;

            sender = tb as object;
            tbBusy = false;
        }
        #endregion
        
        private string getImagePreviewName(string skinName)
        {
            string bestOption = "";
            String skinPath = Application.StartupPath + @"\skins\" + skinName;
            int goodness = 1000;//lower is better option
            string[] files = Directory.GetFiles(skinPath, "*.*", SearchOption.AllDirectories);
            foreach (string str in files)
            {
                String lowers = str.ToLower();
                FileInfo fi = new FileInfo(lowers);
                if (fi.Name.Contains("siupreview"))
                {
                    bestOption = fi.FullName;
                    break;
                }
                if (fi.Extension == ".dds")
                {
                    if (fi.Name.Contains("loadscreen"))
                    {
                        //pick the one with the shortest name
                        int mygoodness = 500 +( getNumberWorth(fi.Name, 0) * 10 )+ fi.Name.Length;
                        if (goodness > mygoodness) { bestOption = fi.FullName; goodness = mygoodness; }

                    }
                    else if (lowers.Contains("info"))
                    {
                        int mygoodness = 600 + getNumberWorth(fi.Name, 0) * 10 + fi.Name.Length;

                        if (fi.Name.Contains("square"))
                        {
                            mygoodness -= 50;
                        }
                        if (goodness > mygoodness) { bestOption = fi.FullName; goodness = mygoodness; }


                    }
                    else if (lowers.Contains("characters"))
                    {
                        int mygoodness = 750 + getNumberWorth(fi.Name, 0) * 10 + fi.Name.Length;
                        if (goodness > mygoodness) { bestOption = fi.FullName; goodness = mygoodness; }
                    }
                    else
                    {
                        int mygoodness = 800 + getNumberWorth(fi.Name, 0) * 10 + fi.Name.Length;
                        if (goodness > mygoodness) { bestOption = fi.FullName; goodness = mygoodness; }
                    }
                }
                else if (fi.Extension == ".jpg")
                {
                    if (lowers.Contains("champions\\"))
                    {
                        if (fi.Name.Contains("splash"))
                        {
                            int mygoodness = 300 + getNumberWorth(fi.Name, 10) * 10 + fi.Name.Length;
                            if (goodness > mygoodness) { bestOption = fi.FullName; goodness = mygoodness; }
                        }
                        else
                        {
                            int mygoodness = 700 + getNumberWorth(fi.Name, 10) * 10 + fi.Name.Length;
                            if (goodness > mygoodness) { bestOption = fi.FullName; goodness = mygoodness; }
                        }
                    }

                }

            }
            return bestOption;
        }
        private void listView1_ItemMouseHover(object sender, ListViewItemHover.ItemHoverEventArgs e)
        {
            string s = listView1.Items[e.Item].SubItems[1].Text;
            this.labelSkinName.Text = s;
            #region getImageName
            string bestOption = getImagePreviewName(s);
            #endregion

            //done with loop
            if (bestOption != "")
            {
                Bitmap m_bit = LGGDevilLoadImage(bestOption);
                if (m_bit != null)
                {
                    pictureBox2.Image = m_bit;
                }
            }
            else
            {
                pictureBox2.Image = null;
                pictureBox2.Invalidate();
            }
            //read info from file
            //debugadd("looking for " + skinPath + "\\skininfo\\SIUInfo.txt");
            String infoPath = Application.StartupPath + @"\skins\" + s + "\\skininfo\\SIUInfo.txt";
            if (File.Exists(infoPath))
            {
                string[] info = readInfoFile(infoPath);
                textBox2.Text = info[2];
            }
            else textBox2.Text = "";


        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs ee)
        {
            int Column = ee.Column;
            //hack to use more acurate data
            if (Column == 4) Column = 7;
            if (Column == 5) Column = 6;
            if (Column == 0) Column = 8;
            // Determine if clicked column is already the column that is being sorted.
            if (Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }
            Properties.Settings.Default.columnToSortBy = lvwColumnSorter.SortColumn;
            Properties.Settings.Default.sortAscending = (lvwColumnSorter.Order == SortOrder.Ascending);
            Properties.Settings.Default.Save();
            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }
        private void toolStripSelectAllInstalled_Click(object sender, EventArgs e)
        {
            debugadd("Toolstrip install clicked");
            foreach (ListViewItem item in this.listView1.Items)
            {
                if (item.SubItems[4].Text != "No")
                {
                    item.Checked = true;
                }
                else
                {

                    item.Checked = false;
                }
            }
        }
        private void toolStripSelectUninstalled_Click(object sender, EventArgs e)
        {
            debugadd("Toolstrip uninstall clicked");
            foreach (ListViewItem item in this.listView1.Items)
            {
                if (item.SubItems[4].Text == "No")
                {
                    item.Checked = true;
                }
                else
                {

                    item.Checked = false;
                }
            }
        }
        private void selectAllSkinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugadd("Toolstrip select all clicked");
            foreach (ListViewItem item in this.listView1.Items)
                item.Checked = true;
        }
        private void deselectAllSkinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugadd("Toolstrip deselect all clicked");
            foreach (ListViewItem item in this.listView1.Items)
                item.Checked = false;
        }
        private void editThisSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openedAt != null)
            {
                ListViewHitTestInfo li = listView1.HitTest(openedAt);

                ListViewItem item = li.Item;// listView1.GetItemAt(openedAt.X, openedAt.Y);
            
                if (item != null)
                {
                    //Cliver.Message.Inform("Goint to edit " + item.SubItems[1]);
                    string skinName = item.SubItems[1].Text;
                    string sinfo = "";
                    String infoPath = Application.StartupPath + @"\skins\" + skinName + "\\skininfo\\SIUInfo.txt";
                    if (File.Exists(infoPath))
                    {
                        string[] info = readInfoFile(infoPath);
                        sinfo = info[2];
                    }
                    prettyDate dta = new prettyDate(item.SubItems[6].Text);
                    prettyDate dti = new prettyDate(item.SubItems[7].Text);


                    EditSkinItemForm1 form = new EditSkinItemForm1(
                        this, item.SubItems[1].Text, item.SubItems[2].Text, item.SubItems[3].Text
                        ,(dti.getDate().Ticks!=0),
                        sinfo, getImagePreviewName(skinName),
                        dta.getDate(),dti.getDate());
                    form.ShowDialog();
                }
                else
                {
                    Cliver.Message.Inform("item is null");
                }
            }
        }
        private void contextMenuStrip1_Opened(object sender, EventArgs e)
        {
            //openedAt = new Point(contextMenuStrip1.ClientRectangle.X, contextMenuStrip1.ClientRectangle.Y);
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            //listView1.hi
        }
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            openedAt = e.Location;
        }
        public void saveNewInfo(string skinName, string skinAuthor,
            bool skinInstalled, string skinInfo, string skinImagePath,
            DateTime dateAdded, DateTime dateInstalled)
        {
            String infoPath = Application.StartupPath + @"\skins\" + skinName + "\\skininfo";
                   
            string oldImagePath = getImagePreviewName(skinName);
            if (skinImagePath != oldImagePath)
            {
                String newImagePath = Application.StartupPath + @"\skins\" + skinName + "\\skininfo\\SIUPreview.png";
            
                if (oldImagePath.ToLower().Contains("siu"))
                {
                    //need to delete this one
                    SIFileOp.FileDelete(oldImagePath);
                }
                Bitmap m_bit = LGGDevilLoadImage(skinImagePath);
                if (m_bit != null)
                {
                    LGGImageSave(m_bit, newImagePath);

                }
            }
            prettyDate dt = new prettyDate(dateAdded);            
            prettyDate dti = new prettyDate(dateInstalled);

            string qurry = "UPDATE skins SET" +
                " author=\"" + skinAuthor + "\"," +
                " dateadded=\"" + dt.getStringDate() + "\"," +
                " dateinstalled=\"" + (skinInstalled ? dti.getStringDate() : "-") + "\"," +
                " sInstalled=\""+ (skinInstalled?"Yes":"No")+
                
                "\" WHERE sName=\"" +
                        skinName+
                        "\"";

            debugadd(qurry);
            this.ExecuteQuery(qurry);
            //now write info file, in infoPath
            if (!Directory.Exists(infoPath)) Directory.CreateDirectory(infoPath);
            TextWriter writer = new StreamWriter(infoPath+"\\SIUInfo.txt");
            writer.WriteLine("skin name: "+skinName);
            writer.WriteLine("skin author: "+skinAuthor);
            writer.WriteLine("skin info: " + skinInfo.Replace("\r\n", "[New Line]").Replace(":","[Colon]"));
            writer.Close();
            UpdateListView();
        }
        private void dispCheckChanged(object sender, EventArgs e)
        {
            if (listView1.Columns.Count > 8)
            {
                if (checkBox1dispTitle.Checked)
                    this.listView1.Columns[1].Width = 190;
                else this.listView1.Columns[1].Width = 0;
                if (checkBox1dispAuthor.Checked)
                    this.listView1.Columns[2].Width = 100;
                else this.listView1.Columns[2].Width = 0;
                if (checkBox1dispFileCount.Checked)
                    this.listView1.Columns[3].Width = 59;
                else this.listView1.Columns[3].Width = 0;
                if (checkBox1dispInstalled.Checked)
                    this.listView1.Columns[4].Width = 59;
                else this.listView1.Columns[4].Width = 0;
                if (checkBox1dispDateAdded.Checked)
                    this.listView1.Columns[5].Width = 59;
                else this.listView1.Columns[5].Width = 0;
                if (checkBox1dispDateAddedFull.Checked)
                    this.listView1.Columns[6].Width = 140;
                else this.listView1.Columns[6].Width = 0;
                if (checkBox1dispDateInstalledFull.Checked)
                    this.listView1.Columns[7].Width = 140;
                else this.listView1.Columns[7].Width = 0;
                if (checkBox1dispCharacter.Checked)
                    this.listView1.Columns[8].Width = 90;
                else this.listView1.Columns[8].Width = 0;
            }

            Properties.Settings.Default.dispTitle = checkBox1dispTitle.Checked;
            Properties.Settings.Default.dispAuthor = checkBox1dispAuthor.Checked;
            Properties.Settings.Default.dispFileCount = checkBox1dispFileCount.Checked;
            Properties.Settings.Default.dispInstalled = checkBox1dispInstalled.Checked;
            Properties.Settings.Default.dispDateAdded = checkBox1dispDateAdded.Checked;
            Properties.Settings.Default.dispFullDateAdded = checkBox1dispDateAddedFull.Checked;
            Properties.Settings.Default.dispFullDateInstalled = checkBox1dispDateInstalledFull.Checked;
            Properties.Settings.Default.Save();
        }
        private void loadListViewSettings()
        {
            bool dispTitle = Properties.Settings.Default.dispTitle;
            bool dispAuthor = Properties.Settings.Default.dispAuthor;
            bool dispFileCount = Properties.Settings.Default.dispFileCount;
            bool dispInstalled = Properties.Settings.Default.dispInstalled;
            bool dispDateAdded = Properties.Settings.Default.dispDateAdded;
            bool dispDateAddedFull = Properties.Settings.Default.dispFullDateAdded;
            bool dispDateInstalledFull = Properties.Settings.Default.dispFullDateInstalled;
            //temp seeting so not overwritten in mass set event
            checkBox1dispTitle.Checked = dispTitle;
            checkBox1dispAuthor.Checked = dispAuthor;
            checkBox1dispFileCount.Checked = dispFileCount;
            checkBox1dispInstalled.Checked = dispInstalled;
            checkBox1dispDateAdded.Checked = dispDateAdded;
            checkBox1dispDateAddedFull.Checked = dispDateAddedFull;
            checkBox1dispDateInstalledFull.Checked = dispDateInstalledFull;
            dispCheckChanged(this, null);
        }
        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            setImageValue(colorSlider1.Value, false);
        }
        private void setImageValue(int val, bool setToo)
        {
            int v = 16 + (int)Math.Round((((double)(100 - val)) / 100.0) * (128.0 - 16.0));
            this.imageList1.ImageSize = new Size(v, v);
            Properties.Settings.Default.iconSize = val;
            Properties.Settings.Default.Save();
            if (setToo) colorSlider1.Value = val;
            if (listView1.Columns.Count > 0)
            {
                listView1.Columns[0].Width = 20 + (v / 1);
                imageList1.Images.Clear();
                imageList1.Images.AddRange(imageIcons.ToArray());
            }
        }       
        #endregion
        #region wtfRainbows
        public void changeColors(Color fc, Color bc)
        {
            makeColor(this.Controls, fc, bc);

            this.ForeColor = fc;
            this.BackColor = bc;
        }
        public void makeColor(System.Windows.Forms.Control.ControlCollection cc, Color fc, Color bc)
        {
            foreach (Control c in cc)
            {
                c.ForeColor = fc;
                c.BackColor = bc;
                //c.BackColor = Color.Red;
                if (c.Controls.Count > 0)
                    makeColor(c.Controls, fc, bc);
            }
        }
        public void makePaint(System.Windows.Forms.Control.ControlCollection cc,
            System.Windows.Forms.PaintEventHandler ph)
        {
            foreach (Control c in cc)
            {
                //if(c
                if (c.GetType() == panel3.GetType())
                {
                    // Cliver.Message.Inform("going to paint " + c.Name);
                    c.Paint += ph;
                }

                if (c.Controls.Count > 0)
                    makePaint(c.Controls, ph);

            }

        }
        public void stopPaint(System.Windows.Forms.Control.ControlCollection cc,
            System.Windows.Forms.PaintEventHandler ph)
        {
            foreach (Control c in cc)
            {
                //if(c
                if (c.GetType() == panel3.GetType())
                {
                    // Cliver.Message.Inform("going to paint " + c.Name);
                    c.Paint -= ph;
                }

                if (c.Controls.Count > 0)
                    makePaint(c.Controls, ph);

            }

        }
        private void makeLookImportant(object sender, PaintEventArgs e)
        {
            Panel senderPanel = (Panel)sender;
            base.OnPaint(e);
            if (senderPanel.BackColor != Color.Lime) return;
            Graphics g = e.Graphics;
            int w = e.ClipRectangle.Width;
            int h = e.ClipRectangle.Height;
            if (w == 0) return;

            Brush myBrush;
            PointF[] p = new PointF[] { new PointF(0, 0), new PointF(w, 0),
                new PointF(w, h),new PointF(0,h) };
            GraphicsPath pth = new GraphicsPath();
            pth.AddLines(p);
            PathGradientBrush pgb = new PathGradientBrush(pth);
            ColorBlend cb = new ColorBlend(6);
            cb.Positions = new float[]{
                0.0f,
                0.3f,
                0.4f,
                0.6f,
                0.8f,
                1.0f
            };
            cb.Colors = new Color[]{
                Properties.Settings.Default.omfgred?Color.FromArgb(10, 0, 0):
            Color.Lime,
                 Color.Yellow,
                 Color.Green,
                 Color.Yellow,
                 Color.Green,
                 Color.Yellow};
            pgb.InterpolationColors = cb;
            myBrush = pgb;
            e.Graphics.FillRectangle(myBrush, 0, 0, w, h);
        }
        private void crazyPaint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Properties.Settings.Default.wtfRainbows == false) return;
            Graphics g = e.Graphics;
            int w = e.ClipRectangle.Width;
            int h = e.ClipRectangle.Height;
            if (w == 0) return;

            Brush myBrush;
            // myBrush = new LinearGradientBrush(new Rectangle(0, 0, panel3.Width, panel3.Height),
            //    Color.Violet, Color.White, LinearGradientMode.BackwardDiagonal);
            PointF[] p = new PointF[] { new PointF(0, 0), new PointF(w, 0),
                new PointF(w, h),new PointF(0,h) };     
            GraphicsPath pth = new GraphicsPath();
            pth.AddLines(p); 
            PathGradientBrush pgb = new PathGradientBrush(pth);
            //changeColors(System.Drawing.SystemColors.ControlText, System.Drawing.SystemColors.Control);
        
            //pgb.CenterColor = System.Drawing.SystemColors.Control;
            //pgb.SurroundColors = new Color[] { Color.Black};
            ColorBlend cb = new ColorBlend(6);
            cb.Positions = new float[]{
                0.0f,
                0.3f,
                0.4f,
                0.6f,
                0.8f,
                1.0f
            };
            cb.Colors = new Color[]{
                Properties.Settings.Default.omfgred?Color.FromArgb(10, 0, 0):
            System.Drawing.SystemColors.Control,
                 Color.Red,
                 Color.Orange,
                 Color.Yellow,
                 Color.Green,
                 Color.Blue};
            pgb.InterpolationColors = cb; 
            //Blend b = new Blend();
            //b.Factors = new float[] {   1,   1,    0,   .5f,  1 };
            //b.Positions = new float[] { 0,.25f, 0.5f, .75f,  1f };
            //pgb.FocusScales = new PointF(panel3.Width/2,panel3.Height/2);
            //pgb.Blend = b;
            myBrush = pgb;
            e.Graphics.FillRectangle(myBrush, 0, 0,w, h);
            //g.DrawString("www.java2s.com", Font, Brushes.Black, 50, 75);
            //g.DrawRectangle(new Pen(new Brush(
  
        }
        private void iAmLGGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.omfgred = true;
            Properties.Settings.Default.Save();
            changeColors(Color.Red, Color.FromArgb(10, 0, 0));
        }
        private void iCantStandLGGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.omfgred = false;
            Properties.Settings.Default.Save();
            changeColors(System.Drawing.SystemColors.ControlText, System.Drawing.SystemColors.Control);
        }
        private void wtfRainbowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.wtfRainbows)
            {
                Properties.Settings.Default.wtfRainbows = false;
            }
            else
            {
                Properties.Settings.Default.wtfRainbows = true;
            }
            Properties.Settings.Default.Save();
            this.Refresh();
        }
        #endregion
        private void skinInstaller_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.size2 = this.Size;
            Properties.Settings.Default.lastSelectedTab = tabControl1.SelectedIndex;
            Properties.Settings.Default.Save();
        }
        private void previewThisSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListWorker1.IsBusy)
            {
                Cliver.Message.Inform("Please wait till SIU is fully loaded\r\nBefore Previewing a skin\r\n\r\nCheck the progress bar at the bottom for status.");
                return;
            }
            if (openedAt != null)
            {
                ListViewHitTestInfo li = listView1.HitTest(openedAt);
                ListViewItem item = li.Item;
                if (item != null)
                {
                    string skinName = item.SubItems[1].Text;
                    String skinPath = Application.StartupPath + @"\skins\" + skinName;
                    if (Directory.Exists(skinPath))
                    {
                        //timer1.Stop();//cant opengl them both :/
                        List<String> fileNames = new List<String>(Directory.GetFiles(skinPath, "*.*", SearchOption.AllDirectories));
                        
                        if(previewWindow==null)
                            previewWindow = new PreviewWindow(gameDirectory);
                        //previewWindow.previewModel(fileNames);
                        previewWindow.Show();//.Show();
                        previewWindow.previewModel(fileNames);
                        previewWindow.Focus();
                    }
                }
            }

        }
        private void loLViewerOpenNotPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileListWorker1.IsBusy)
            {
                Cliver.Message.Inform("Please wait till SIU is fully loaded\r\nBefore Previewing a skin\r\n\r\nCheck the progress bar at the bottom for status.");
                return;
            }
            timer1.Stop();
            MainWindow main = new MainWindow();
            main.reader.SetRoot(gameDirectory);
            main.Show();
        }
        #region TreeViewStuff
        private Color colorFromRafVersion(string fileString)
        {
            return colorFromRafPower(getRafPowerFromVersion(fileString));
        }
        private Color colorFromRafPower(int rafPower)
        {
            int numberOfRafVersions = previewWindow.reader.rafArchives.Count;

            return new HSLColor(
                ((double)(numberOfRafVersions - rafPower) /
                (double)numberOfRafVersions) /
                (double)3.0,
                1.0, .5);
        }
        private int getRafPowerFromVersion(string fileString)
        {
            int rafPower = 0;
            int numberOfRafVersions = previewWindow.reader.rafArchives.Count;
            Regex rafRegex = new Regex(@"\\\d+\.\d+\.\d+\.\d+\\");
            Match rafMatch = rafRegex.Match(fileString);
            if (rafMatch.Success)
            {
                string caught = rafMatch.Captures[0].Value.Substring(1, rafMatch.Captures[0].Value.Length - 2);
                for (int i = 0; i < numberOfRafVersions; i++)
                {
                    if (previewWindow.reader.rafArchives.Keys.ToArray()[i].Contains(caught))
                    {
                        rafPower = i;
                    }
                }
            }
            return rafPower;
        }
        private string getRafVersionFromRafPower(int rafPower)
        {
            if (rafPower > previewWindow.reader.rafArchives.Count) return "Unknown";
            return new Regex(@"\\(\d+\.\d+\.\d+\.\d+)\\").
                Match(previewWindow.reader.rafArchives.Keys.ToArray()[rafPower]).
                Captures[0].Value;
            
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name == "Text" && e.Node.Nodes.Count < 2)
            {
                ted.ShowDialog();
            }
        }
        private void buttonRebuildTree_Click(object sender, EventArgs e)
        {
            rebuildTree();
        }
        private void rebuildTree()
        {
            treeView1.Nodes.Clear();
            treeView1.ImageList = new ImageList();
            treeView1.ImageList.ImageSize = new Size(treeView1.ItemHeight, treeView1.ItemHeight);
            treeView1.ImageList.Images.Add(Properties.Resource.folder);
            treeView1.ImageList.Images.Add(Properties.Resource.skul16b);
            treeView1.ImageList.Images.Add(Properties.Resource.skin);
            treeView1.ImageList.Images.Add(Properties.Resource.Dee_dee);
            treeView1.ImageList.Images.Add(Properties.Resource.tex_2);
            treeView1.ImageList.Images.Add(Properties.Resource.lemon);
            treeView1.ImageList.Images.Add(Properties.Resource.small_cube_1);
            treeView1.ImageList.Images.Add(Properties.Resource.small_cube_2);
            treeView1.ImageList.Images.Add(Properties.Resource.loadscreenicon);
            treeView1.TreeViewNodeSorter = new NodeSorter();
            #region modelsTreeBuild
            TreeNode modelsRootNode = new TreeNode("Models");
            treeView1.Nodes.Add(modelsRootNode);
            if (previewWindow != null)
            {
                foreach (KeyValuePair<string, LOLViewer.IO.LOLModel> kv in previewWindow.reader.models)
                {
                    int firstSlash = kv.Key.IndexOf("/");
                    String charFolder = kv.Key.Substring(0, firstSlash);
                    if (!modelsRootNode.Nodes.ContainsKey(charFolder))
                    {
                        TreeNode temp=modelsRootNode.Nodes.Add(charFolder,charFolder);
                        //TreeNode temp = modelsRootNode.Nodes.Find(charFolder, false)[0];
                        temp.ToolTipText = charFolder;
                    }
                    String charName = kv.Key.Substring(firstSlash+1); 
                    TreeNode folderNode = modelsRootNode.Nodes.Find(charFolder, false)[0];
                    TreeNode charNode = folderNode.Nodes.Add(charName, charName);
                    //TreeNode charNode = folderNode.Nodes.Find(charName, false)[0];
                    charNode.ToolTipText = kv.Key;
                    rafTreeDataObject ctag = new rafTreeDataObject();
                    ctag.fileLocation = "Skin";
                    charNode.Tag = ctag;
                    //charNode.Tag = new rafTreeDataObject(0, "");// kv.Value;
                    //add models and stuff
                    if (kv.Value.animations.Count > 0)
                    {
                        TreeNode animationsNode = new TreeNode("Animations");
                        animationsNode.ToolTipText = "List of animations used by this model";
                        foreach (KeyValuePair<string, RAFFileListEntry> animkv in kv.Value.animations)
                        {
                            TreeNode tempAnimNode = new TreeNode(animkv.Key);
                            rafTreeDataObject tag = new rafTreeDataObject();
                            tag.fileLocation = animkv.Value.RAFArchive.RAFFilePath + "\\" +
                            animkv.Value.FileName.Replace("/", "\\");
                            tag.rafPower =getRafPowerFromVersion(tag.fileLocation);
                            tempAnimNode.Tag = tag;
                            tempAnimNode.ForeColor = colorFromRafPower(tag.rafPower);
                            tempAnimNode.ToolTipText = animkv.Value.FileName;
                            tempAnimNode.ImageIndex = tempAnimNode.SelectedImageIndex = 3;
                            animationsNode.Nodes.Add(tempAnimNode);
                        }
                        charNode.Nodes.Add(animationsNode);
                    }
                                       
                    
                    //skn
                    FileInfo sknInfo = new FileInfo(kv.Value.skn.FileName);
                    TreeNode sknNode = new TreeNode(sknInfo.Name.Substring(0, sknInfo.Name.IndexOf(".")));
                    sknNode.ToolTipText = kv.Value.skn.FileName;
                    sknNode.ImageIndex = sknNode.SelectedImageIndex = 2;
                    sknNode.Tag = kv.Value.skn;
                    rafTreeDataObject skntag = new rafTreeDataObject();
                    skntag.fileLocation = kv.Value.skn.RAFArchive.RAFFilePath + "\\" +
                    kv.Value.skn.FileName.Replace("/", "\\");
                    skntag.rafPower = getRafPowerFromVersion(skntag.fileLocation);
                    sknNode.Tag = skntag;
                    sknNode.ForeColor = colorFromRafPower(skntag.rafPower);

                    charNode.Nodes.Add(sknNode);

                    //air files
                    if (kv.Value.champPic != null && kv.Value.champPic != null)
                    {
                        TreeNode airNode = new TreeNode("Champion Selection");
                        airNode.ToolTipText = "List of images used by this model when selecting their skin before a game.";

                        TreeNode airSNode = new TreeNode(kv.Value.champPic);
                        rafTreeDataObject airStag = new rafTreeDataObject();
                        airStag.fileLocation = kv.Value.champPic;
                        airStag.rafPower = skntag.rafPower;//getRafPowerFromVersion(airStag.fileLocation);
                        airSNode.Tag = airStag;
                        airSNode.ForeColor = colorFromRafPower(airStag.rafPower);
                        airSNode.ToolTipText = airStag.fileLocation;
                        airSNode.ImageIndex = airSNode.SelectedImageIndex = 8;
                        airNode.Nodes.Add(airSNode);

                        TreeNode airCNode = new TreeNode(kv.Value.champSplash);
                        rafTreeDataObject airCtag = new rafTreeDataObject();
                        airCtag.fileLocation = kv.Value.champSplash;
                        airCtag.rafPower = skntag.rafPower;//getRafPowerFromVersion(airStag.fileLocation);
                        airCNode.Tag = airCtag;
                        airCNode.ForeColor = colorFromRafPower(airCtag.rafPower);
                        airCNode.ToolTipText = airCtag.fileLocation;
                        airCNode.ImageIndex = airCNode.SelectedImageIndex = 8;
                        airNode.Nodes.Add(airCNode);




                        charNode.Nodes.Add(airNode);
                    }

                    //skl
                    FileInfo sklInfo = new FileInfo(kv.Value.skl.FileName);
                    TreeNode sklNode = new TreeNode(sklInfo.Name.Substring(0, sklInfo.Name.IndexOf(".")));
                    sklNode.ToolTipText = kv.Value.skl.FileName;
                    rafTreeDataObject skltag = new rafTreeDataObject();
                    skltag.fileLocation = kv.Value.skl.RAFArchive.RAFFilePath + "\\" +
                    kv.Value.skl.FileName.Replace("/", "\\");
                    skltag.rafPower = getRafPowerFromVersion(skltag.fileLocation);
                    sklNode.Tag = skltag;
                    sklNode.ForeColor = colorFromRafPower(skltag.rafPower);
                            
                    sklNode.ImageIndex = sklNode.SelectedImageIndex = 1;
                    charNode.Nodes.Add(sklNode);
                    //load screen
                    if (kv.Value.loadScreen != null)
                    {

                        FileInfo lodInfo = new FileInfo(kv.Value.loadScreen.FileName);
                        TreeNode lodNode = new TreeNode(lodInfo.Name.Substring(0, lodInfo.Name.IndexOf(".")));
                        lodNode.ToolTipText = kv.Value.loadScreen.FileName;
                        rafTreeDataObject lodtag = new rafTreeDataObject();
                        lodtag.fileLocation = kv.Value.loadScreen.RAFArchive.RAFFilePath + "\\" +
                        kv.Value.loadScreen.FileName.Replace("/", "\\");
                        lodtag.rafPower = getRafPowerFromVersion(lodtag.fileLocation);
                        lodNode.Tag = lodtag;
                        lodNode.ForeColor = colorFromRafPower(lodtag.rafPower);

                        lodNode.ImageIndex = lodNode.SelectedImageIndex = 8;
                        charNode.Nodes.Add(lodNode);
                    }
                    //texture
                    FileInfo txtInfo = new FileInfo(kv.Value.texture.FileName);
                    TreeNode txtNode = new TreeNode(txtInfo.Name.Substring(0, txtInfo.Name.IndexOf(".")));
                    txtNode.ToolTipText = kv.Value.texture.FileName;
                    txtNode.Tag = kv.Value.texture;
                    rafTreeDataObject txttag = new rafTreeDataObject();
                    txttag.fileLocation = kv.Value.texture.RAFArchive.RAFFilePath + "\\" +
                    kv.Value.texture.FileName.Replace("/", "\\");
                    txttag.rafPower = getRafPowerFromVersion(txttag.fileLocation);
                    txtNode.Tag = txttag;
                    txtNode.ForeColor = colorFromRafPower(txttag.rafPower);
                            
                    txtNode.ImageIndex = txtNode.SelectedImageIndex = 4;
                    charNode.Nodes.Add(txtNode);
                }
                //we added all the models, remove unnecessary folders now
                TreeNode othersNode = new TreeNode("Others");
                othersNode.ToolTipText="Other non-character models";
                modelsRootNode.Nodes.Add(othersNode);
                List<string> toRemoves = new List<string>();
                List<TreeNode> toAdds = new List<TreeNode>();
                foreach (TreeNode folderNode in modelsRootNode.Nodes)
                {
                    if (folderNode.Nodes.Count == 1)
                    {
                        // no need for a folder here
                        TreeNode tempNode = folderNode.Nodes[0];
                        toRemoves.Add(folderNode.Name);
                        toAdds.Add(tempNode);
                    }
                }
                foreach (string toRemove in toRemoves)
                {
                    modelsRootNode.Nodes.RemoveByKey(toRemove);                
                }
                foreach (TreeNode toAdd in toAdds)
                {
                    othersNode.Nodes.Add(toAdd);
                }

            }
            //modelsRootNode.Expand();
            colorizeFolders();
            #endregion
            treeView1.Nodes.Add("RAF", "RAF");
            TreeNode rafNode = treeView1.Nodes.Find("RAF", false)[0];
            rafNode.Name = "RAF";
            rafNode.Nodes.Add("Loading...(Watch Progress bar at bottom)");
            treeView1.Nodes.Add("Text", "Text");
            TreeNode textNode = treeView1.Nodes.Find("Text", false)[0];
            textNode.Name = "Text";
            textNode.Nodes.Add("Loading...");
            treeView1.Nodes.Add("Particles", "Particles");
            TreeNode particleNode = treeView1.Nodes.Find("Particles", false)[0];
            particleNode.Name = "Particles";
            TreeNode particleNewNode = particleNode.Nodes.Add("Particles New", "Particles New");
            particleNewNode.Name = "Particles New";
            particleNewNode.Nodes.Add("Loading...(Watch Progress bar at bottom)");
            TreeNode particleOldNode = particleNode.Nodes.Add("Particles Old", "Particles Old");
            particleOldNode.Name = "Particles Old";
            particleOldNode.Nodes.Add("Loading...(Watch Progress bar at bottom)");
        }
        private int colorizeFolder(TreeNode folder, bool usePreviousResults = true)
        {
            int toReturn = 10000;
            if (folder.Nodes.Count > 0)
            {
                //we know this is a folder so we color it based on what is inside of it!               
                int lowestRafPower = 100000;//not very low yet :P
                foreach (TreeNode innerNode in folder.Nodes)
                {
                    int nodesPower = -1;
                    /*if one of the items inside of this folder 
                     * (either another folder, or a item)
                     * has already figured out what version (and thus what color)
                     * it is from, we need to find that and 
                     * choose the newest version to color this one from
                     * 
                     * basically the idea is that we need to get version information
                     * from EVERYTHING inside this folder, before we set it
                     * thus the recursion to set the other folders first before we 
                     * continue
                    */
                    if ((usePreviousResults != true) && (innerNode.Nodes.Count > 0))
                    {
                        //we are not using previous results, so don't bother checking tag
                        //get new one, and make sure all child folders do the same
                        nodesPower = colorizeFolder(innerNode, usePreviousResults);
                    }
                    else
                        if (innerNode.Tag != null)
                        {
                            if (((rafTreeDataObject)innerNode.Tag).fileLocation == "Skin")
                            {
                                /*this folder item is not complete yet, 
                                 * recursion to color it first before
                                 * we continue */
                                nodesPower = colorizeFolder(innerNode, usePreviousResults);
                            }
                            else //this inner node already has its raf version set, take it
                                nodesPower = ((rafTreeDataObject)innerNode.Tag).rafPower;

                        }
                        else
                        {
                            if ((innerNode.Name.ToLower() == "dummy") && usePreviousResults == false)
                            {
                                nodesPower = 100000;
                            }
                            else
                                //this means the folder(node) inside doesn't even have a tag yet
                                //recursion call to set it first too
                                nodesPower = colorizeFolder(innerNode, usePreviousResults);
                        }
                    //ok, now w/e this node was, should be set, 
                    //so we can use it to find the newest raf
                    if (nodesPower < lowestRafPower)
                    {
                        lowestRafPower = nodesPower;
                    }
                }
                if (lowestRafPower != 100000)
                {
                    //we have the newest raf version for this node(folder)
                    //we should create a tag for us in case we are in a folder and
                    //either by recursion, or another call, our function wants to see what version
                    //we are
                    rafTreeDataObject tag = new rafTreeDataObject();
                    tag.fileLocation = "";
                    if (folder.Tag != null) tag.fileLocation = ((rafTreeDataObject)folder.Tag).fileLocation;
                    tag.rafPower = lowestRafPower;
                    folder.Tag = tag;
                    Color test = colorFromRafPower(tag.rafPower);
                    //if (test.Equals(Color.Black)) test = Color.Lime;
                    folder.ForeColor = test;
                    folder.ToolTipText = "Highest Raf Version Detected In This Folder So Far:\r\n" +
                    getRafVersionFromRafPower(tag.rafPower);
                    //also return the result NOW for the recursion events above
                    toReturn = tag.rafPower;
                }
            }
            return toReturn;
        }
        private void colorizeFolders()
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                colorizeFolder(node);
            }
        }
        private void loadRafIntoTree()
        {
            rafTreeBuilderWorker2.ReportProgress(1);
            #region rafTreeBuild
            TreeNode rafRootNode = new TreeNode("RAF");
            rafRootNode.Nodes.Clear();
            int prog = 0;
            int lastReport = 1;
            // Iterate through all the files
            foreach (KeyValuePair<String, String> fileskv in allFilesList)
            {
                prog++;
                int newProg = (int)Math.Floor(((double)prog / (double)allFilesList.Count) * 99);
                if (newProg > lastReport)
                {
                    lastReport = newProg;
                    rafTreeBuilderWorker2.ReportProgress(newProg);
                }
                // Only care about raf files
                if (fileskv.Value.ToLower().Contains(".raf\\"))
                {
                    // Strip off the raf archive text
                    string rafPath = fileskv.Value.Substring(fileskv.Value.IndexOf(".raf\\") + 6);
                    // Create a fileinfo to easily get extensions and short file names
                    FileInfo rafFileInfo = new FileInfo(rafPath);
                    // Strip off the file name (ie. Only want the directory path)
                    if (rafPath.IndexOf("\\\\") == -1)
                        rafPath = "";
                    else
                        rafPath = rafPath.Remove(rafPath.LastIndexOf("\\\\"));

                    // Create a refence to the database
                    TreeNode lookIn = database;
                    // Split the directory into its parts
                    String[] folderArray = rafPath.Split(new string[] { "\\\\" }, StringSplitOptions.RemoveEmptyEntries);

                    // Create a new node for each directory part
                    foreach (String folder in folderArray)
                    {
                        if (!lookIn.Nodes.ContainsKey(folder))
                            lookIn.Nodes.Add(folder, folder);
                        lookIn = lookIn.Nodes.Find(folder, false)[0];
                    }

                    // Create a node for the actual file
                    TreeNode addedNode = lookIn.Nodes.Add(fileskv.Value, rafFileInfo.Name);
                    // Set tooltip text
                    addedNode.ToolTipText = fileskv.Value;

                    // Get text color based on what RAF the file is from
                    rafTreeDataObject tag = new rafTreeDataObject();
                    tag.fileLocation = gameDirectory.Substring(0, gameDirectory.Length - 1) + fileskv.Value.Replace("\\\\", "\\");
                    tag.rafPower = getRafPowerFromVersion(tag.fileLocation);
                    addedNode.Tag = tag;

                    // Get image based on extension
                    int imageIndex = 5;
                    switch (rafFileInfo.Extension.ToLower())
                    {
                        case ".skl": imageIndex = 1; break;
                        case ".skn": imageIndex = 2; break;
                        case ".anm": imageIndex = 3; break;
                        case ".dds": imageIndex = 4; break;
                        case ".tga": imageIndex = 4; break;
                        case ".sco": imageIndex = 6; break;
                        case ".scb": imageIndex = 7; break;
                        default: imageIndex = 5; break;
                    }
                    if (imageIndex == 4)
                    {
                        if(rafFileInfo.Name.ToLower().Contains("loadscreen"))
                            imageIndex = 8;
                    }

                    // Set image and color
                    addedNode.ImageIndex = addedNode.SelectedImageIndex = imageIndex;
                    addedNode.ForeColor = colorFromRafPower(tag.rafPower);
                }
            }

            // Initial fill
            foreach (TreeNode node in database.Nodes)
            {
                TreeNode childNode = new TreeNode(node.Text);
                // If the child has children, create a dummy child for it
                if (node.Nodes.Count > 0)
                    childNode.Nodes.Add("dummy", "dummy");
                childNode.Tag = node.Tag;
                childNode.ToolTipText = node.ToolTipText;
                childNode.ImageIndex = childNode.SelectedImageIndex = node.ImageIndex;
                childNode.ForeColor = node.ForeColor;
                rafRootNode.Nodes.Add(childNode);
            }

            // Add rootnode to the treeview
            newRafNode = rafRootNode;
            // Color the folders 
            colorizeFolder(treeView1.Nodes.Find("RAF", false)[0], false);
            #endregion
            rafTreeBuilderWorker2.ReportProgress(100);
        }
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // If the expanded node contains a dummy node, replace it with the real data
            if (e.Node.Nodes.ContainsKey("dummy"))
            {
                createChildren(e.Node);
            }
        }
        private void createChildren(TreeNode node)
        {
            // Disable redrawing of the treeview to prevent hangs
            node.TreeView.BeginUpdate();

            // Remove the dummy child
            node.Nodes["dummy"].Remove();

            TreeNode lookUp = node;
            // Create a list to hold the heirarchy required to get to the top
            List<String> parentList = new List<String>();
            // Add the actual node itself
            parentList.Add(node.Text);
            // Keep adding until it's at the top
            while (lookUp.Parent.Text != "RAF")
            {
                parentList.Add(lookUp.Parent.Text);
                lookUp = lookUp.Parent;
            }
            // Reverse the list so I can go down the heirarchy
            parentList.Reverse();

            // Reference the first node in the heirarchy to the database
            lookUp = database.Nodes[parentList[0]];
            // Work down to the node in question
            for (int i = 1; i < parentList.Count; i++)
            {
                lookUp = lookUp.Nodes[parentList[i]];
            }

            // Iterate through any children the node has
            foreach (TreeNode luNode in lookUp.Nodes)
            {
                TreeNode childNode = new TreeNode(luNode.Text);
                // If the child has children, create a dummy child for it
                if (luNode.Nodes.Count > 0)
                    childNode.Nodes.Add("dummy", "dummy");
                // Update the image and color according to the database values
                childNode.ImageIndex = childNode.SelectedImageIndex = luNode.ImageIndex;
                childNode.ForeColor = luNode.ForeColor;
                childNode.Tag = luNode.Tag;
                childNode.ToolTipText = luNode.ToolTipText;
                node.Nodes.Add(childNode);
            }

            // Check new children if e.Node is checked
            if (node.Checked)
                setNodeAndChildrenCheck(true, node);

            // Color the folders
            colorizeFolder(node.TreeView.Nodes.Find("RAF", false)[0], false);

            // Re-enable drawing of the treeview
            node.TreeView.EndUpdate();
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name == "RAF" && e.Node.Nodes.Count < 2)
                rafTreeBuilderWorker2.RunWorkerAsync();
            if (e.Node.Name == "Text" && e.Node.Nodes.Count < 2)
            {
                ted.ShowDialog();
                e.Node.Collapse();
            }
            if ((e.Node.Name == "Particles Old") && e.Node.Nodes.Count < 2)
            {
                e.Node.Text = "Particles Old (Powered By RichieSams)";
                PartRef.ParticleReference p = new PartRef.ParticleReference();
                p.startGettingParticleStructure(this,gameDirectory + "RADS\\projects\\lol_game_client\\filearchives\\");
            }
            if ((e.Node.Name == "Particles New") && e.Node.Nodes.Count < 2)
            {
                e.Node.Text = "Particles New (Powered By RichieSams)";
                ParticleFinder.ParticleFinderNew p = new ParticleFinder.ParticleFinderNew();
                p.startGettingParticleStructure(this, gameDirectory + "RADS\\projects\\lol_game_client\\filearchives\\");
            }
        }
        private void rafTreeBuilderWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            loadRafIntoTree();
        }
        private void rafTreeBuilderWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage);
        }
        private void rafTreeBuilderWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TreeNode rafRootNode = treeView1.Nodes.Find("RAF", false)[0];
            
            rafRootNode.Nodes.Clear();
            foreach (TreeNode node in newRafNode.Nodes)
            {
                rafRootNode.Nodes.Add(node);
            }
            //treeView1.Nodes.Remove(rafRootNode);
            //treeView1.Nodes.Add(newRafNode);
            treeView1.Sort();
        }
        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            setNodeAndChildrenCheck(!e.Node.Checked,e.Node);
        }
        private void setNodeAndChildrenCheck(bool check,TreeNode node)
        {
            foreach (TreeNode innerNode in node.Nodes)
            {
                innerNode.Checked = check;
                setNodeAndChildrenCheck(check, innerNode);
            }
        }
        private void exportSelectedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //to do: export them!
            List<TreeNode> checkedNodes = new List<TreeNode>();

            foreach (TreeNode node in treeView1.Nodes)
            {
                getCheckedNodes(node, ref checkedNodes);
            }
            exportNodes(checkedNodes);
        }
        private void exportTreeViewWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            exportTreeViewWorker1.ReportProgress(1);
            exportNodeDataStruct ends = (exportNodeDataStruct)e.Argument;
            List<TreeNode> checkedNodes=ends.checkedNodes;
            string extraFolder = ends.extraFolder;
            string path = ends.path;
            string output = "";
            List<String> toBackup = new List<String>();
            if (checkedNodes.Count == 0)
            {
                Cliver.Message.Show("Nothing Checked", SystemIcons.Error,
                    "Please check the mark on the files you want to export",
                    0, new String[] { "Ok" });
                return;
            }
            int i = 0;
            int last = 0;
            foreach (TreeNode checkedNode in checkedNodes)
            {
                int prog =(int)( ((float)i / (float)checkedNodes.Count) * 50.0f);
                if (prog != last)
                {
                    last = prog;
                    exportTreeViewWorker1.ReportProgress(prog);
                }
                i++;
                if (checkedNode.Nodes.Count == 0)//no folders
                {
                    if (checkedNode.FullPath.ToLower().IndexOf("particles") == 0)
                    {

                        //todo back it up
                        //find tga and dds
                        //watch out for empty folders! (check for extension)
                        foreach (KeyValuePair<String, String> pairFileName_Path in allFilesList)
                        {
                            //NOstring rafTestPath = ((rafTreeDataObject)checkedNode.Tag).fileLocation.ToLower();
                            string rafTestPath = checkedNode.Name.ToLower();
                            //rafTestPath=rafTestPath.Substring(rafTestPath.LastIndexOf(".raf"));
                            string testPath = pairFileName_Path.Key.ToLower();
                            //remove extensions so we can find .tga files as well and .dds files
                            //int dotLoc1 = rafTestPath.LastIndexOf("\\");
                            int dotLoc1 = rafTestPath.LastIndexOf(".");
                            int dotLoc2 = testPath.LastIndexOf(".");
                            //if (dotLoc1 >= 0) rafTestPath = rafTestPath.Substring(dotLoc1+1);
                            if (dotLoc2 >= 0) testPath = testPath.Substring(0, dotLoc2);
                            //dotLoc1 = rafTestPath.LastIndexOf(".");
                            if (dotLoc1 >= 0) rafTestPath = rafTestPath.Substring(0, dotLoc1);

                            if (testPath == rafTestPath)
                            {
                                string fileLoc = gameDirectory + pairFileName_Path.Value.Substring(1).Replace("\\\\", "\\");
                                toBackup.Add(fileLoc);
                                output += fileLoc + "\r\n";
                                //dont break; cuz we want it all!
                            }
                        }
                    }
                    else
                    {
                        if (checkedNode.Text == "Loading...(Watch Progress bar at bottom)")
                        {
                            //skip, warn
                            Cliver.Message.Inform("Please Expand the RAF folder before exporting");
                            //return;
                        }
                        else
                        {

                            string fileLocation = ((rafTreeDataObject)checkedNode.Tag).fileLocation;

                            toBackup.Add(fileLocation);
                            output += fileLocation + "\r\n";
                        }
                    }
                }
            }
            
            if (extraFolder != "") path += extraFolder + "\\";
            i = 0;
            foreach (String rafFile in toBackup)
            {
                int prog = ((int)(((float)i / (float)toBackup.Count) * 50.0f))+50;
                if (prog != last)
                {
                    last = prog;
                    exportTreeViewWorker1.ReportProgress(prog);
                }
                i++;

                if (rafFile.ToLower().Contains(".raf"))
                    rafBackup(rafFile, path, true);
                else
                {
                    // find it, save it, go!
                    KeyValuePair<string, string> isfound = allFilesList.FirstOrDefault(m => m.Key.ToLower().Contains(rafFile.ToLower()));
                    if (isfound.Key != null)
                    {
                        string whereFrom = gameDirectory + isfound.Value;
                        string whereTo = path + "air\\" + isfound.Key;
                        this.SIFileOp.FileCopy(whereFrom, whereTo);
                    }
                }
            }
            
            treeExportResut result;
            result.output = output;
            result.path = path;
            result.extraFolder = extraFolder;
            e.Result = result;
            exportTreeViewWorker1.ReportProgress(0);
            
        }
        private void exportTreeViewWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage);
        }
        struct treeExportResut
        {
            public string extraFolder;
            public string path;
            public string output;
        }
        private void exportTreeViewWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            treeExportResut myResult = (treeExportResut)e.Result;
            string output = myResult.output;
            string path = myResult.path;
            string extraFolder = myResult.extraFolder;
            if (extraFolder == "")
            {
                if(output!="")
                    Cliver.Message.Show("Success", SystemIcons.Application, "Successfully processed:\r\n" + output, 0, new string[] { "Yay!" });
                else
                {
                    Cliver.Message.Inform("No files exported, please check your checked entries or expand the folders you selected.");
                }
            }
            else
            {
                if (Cliver.Message.Show("Success", SystemIcons.Application, "Successfully saved " + extraFolder + "\r\n\r\nTo: " + path
                     , 0, new string[] { "Yay!", "Please start adding this skin now" }) == 1)
                {
                    string[] strArray3 = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                    //checked, ok
                    processNewDirectory(strArray3);
                    this.tabControl1.SelectedIndex = 0;
                }
            }
            deselectAllFilesToolStripMenuItem_Click(sender, e);

        }
        private void exportNodes(List<TreeNode> checkedNodes, string extraFolder = "")
        {
            exportNodeDataStruct ends;
            ends.checkedNodes = checkedNodes;
            ends.extraFolder = extraFolder;
            if (exportFolderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                ends.path = exportFolderBrowserDialog1.SelectedPath+"\\";
                exportTreeViewWorker1.RunWorkerAsync(ends);
            }
        }
        struct exportNodeDataStruct
        {
            //so lazy >.>
            public List<TreeNode> checkedNodes;
            public string extraFolder;
            public string path;
        }
        private void deselectAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TreeNode> checkedNodes = new List<TreeNode>();

            foreach (TreeNode node in treeView1.Nodes)
            {
                getCheckedNodes(node, ref checkedNodes);
            }
            foreach (TreeNode checkedNode in checkedNodes)
            {
                checkedNode.Checked = false;
            }
        }
        private void treeMenuStripSkin1_Opening(object sender, CancelEventArgs e)
        {

        }
        private void makeSimpleSkinFromThisRiotSkinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //setNodeAndChildrenCheck(true,
            //  treeView1.SelectedNode);
            List<TreeNode> nodes = new List<TreeNode>();

            foreach (TreeNode node in treeView1.SelectedNode.Nodes)
            {
                getChildrenNodes(node, ref nodes);
            }
            exportNodes(nodes,treeView1.SelectedNode.Text);
        }

        private void lookUpReleaseManifestInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selected  = treeView1.SelectedNode;
            String path = selected.FullPath.Replace("RAF", "").Replace("\\","/");
            List<RelFileEntry> founds = this.rmdf.fileList.SearchFileEntries(path,true);
            if (founds.Count < 1)
            {
                founds = this.rmdf.fileList.SearchFileEntries(selected.ToolTipText);
            }
            if (founds.Count > 0)
            {
                RelManLib.ReleaseManifestEntryForm form = new RelManLib.ReleaseManifestEntryForm(founds[0]);
                form.Show();
            }
        }

        
        private void getCheckedNodes(TreeNode node, ref List<TreeNode> checkedNodes)
        {
            if (node.Checked)
            {
                checkedNodes.Add(node);
                if (node.Nodes.ContainsKey("dummy"))
                {
                    createChildren(node);
                }
            }
            foreach (TreeNode innerNode in node.Nodes)
            {
                getCheckedNodes(innerNode, ref checkedNodes);
            }
        }
        private void getChildrenNodes(TreeNode node, ref List<TreeNode> childNodes)
        {
            childNodes.Add(node);
            foreach (TreeNode innerNode in node.Nodes)
            {
                getChildrenNodes(innerNode, ref childNodes);
            }
        }
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cliver.Message.Show("How to create skins",SystemIcons.Information,
                "To create a skin, you need to modify current files that are already inside the game\r\n\r\n"+
            "Most Files reside inside of archive files with a .raf extension\r\n"+
            "You can see the files inside in the tree in the skin install tab\r\n\r\n"+
            "You can check mark the files you want to change, then RIGHT CLICK and choose to export them\r\n\r\n"+
            "Once you have them on your computer you can modify them with various programs, and then put them back into the game using the \r\n"+
            "\"Add New Skin\" tab.\r\n\r\n"+
            "If you want more help on how to modify these files, see http://forum.leaguecraft.com/index.php?/topic/29647-model-replacement-tutorial/ \r\n"+
            "",
                0,new string[]{"OK"});
        }
        private void button3exporttree_Click(object sender, EventArgs e)
        {
            exportSelectedFilesToolStripMenuItem_Click(sender, e);
        }
        public void recieveParticleInformation(Dictionary<String, Dictionary<String, Dictionary<RAFFileListEntry, List<String>>>> particleDef)
        {
            ParticleTreeWorker2.RunWorkerAsync(particleDef);
        }
        public void recieveNewParticleInformation(Dictionary<String, Dictionary<String, Dictionary<RAFFileListEntry, List<String>>>> particleDef)
        {
            ParticleTreeWorkerNew.RunWorkerAsync(particleDef);
        }
        public void recieveParticleProgress(int p)
        {
            UpdateProgressSafe(p/2);
        }
        private void ParticleTreeWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            TreeNode particleNode = new TreeNode("Particles");//treeView1.Nodes.Find("Particles", false)[0];
            particleNode.Nodes.Clear();

            int i = 0;
            int lastProg = 0;
            
            foreach (KeyValuePair<String, Dictionary<String, Dictionary<RAFFileListEntry, List<String>>>> championKVP in
                (Dictionary
                <String, Dictionary
                    <String, Dictionary
                        <RAFFileListEntry, List<String>>>>)e.Argument)
            {
                i++;
                int prog = (int)(((double)i / (double)((Dictionary
                <String, Dictionary
                    <String, Dictionary
                        <RAFFileListEntry, List<String>>>>)e.Argument).Count) * 45) + 50;
                if (prog != lastProg) { lastProg = prog; ParticleTreeWorker2.ReportProgress(prog); }

                TreeNode champNode= particleNode.Nodes.Add(championKVP.Key,Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(championKVP.Key));
                //TreeNode champNode =particleNode.Nodes.Find(championKVP.Key, false)[0];
                int lowestPower = int.MaxValue;
                foreach (KeyValuePair<RAFFileListEntry, List<String>> troybinKVP in championKVP.Value["troybins"])
                {
                    FileInfo troyFileInfo = new FileInfo(troybinKVP.Key.FileName);
                    TreeNode troybinNode =champNode.Nodes.Add(troybinKVP.Key.FileName, Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(troyFileInfo.Name.Substring(0, troyFileInfo.Name.IndexOf("."))));

                    //TreeNode troybinNode = champNode.Nodes.Find(troybinKVP.Key.FileName, false)[0];
                    rafTreeDataObject tag = new rafTreeDataObject();
                    
                    //get troy version number
                    string troyfullpath = "";
                    Color troyColor = Color.Red;
                    foreach (KeyValuePair<String, String> pairFileName_Path in allFilesList)
                    {
                        if (pairFileName_Path.Key.ToLower() == troyFileInfo.Name.ToLower())
                        {
                            troyfullpath = pairFileName_Path.Value;
                            break;
                        }
                    }
                    if (troyfullpath != "")
                    {
                        tag.rafPower = getRafPowerFromVersion(troyfullpath);
                        troyColor = colorFromRafPower(tag.rafPower);
                        if (tag.rafPower < lowestPower) lowestPower = tag.rafPower;
                    }

                    tag.fileLocation = troyfullpath;
                    troybinNode.ToolTipText = tag.fileLocation;
                    troybinNode.ForeColor = troyColor;
                    troybinNode.Tag = tag;

                    foreach (String fileEntry in troybinKVP.Value)
                    {
                        TreeNode[] matchingNodes = troybinNode.Nodes.Find(fileEntry, false);
                        if (matchingNodes.Length==0)//avoid duplicate entries
                        {
                            troybinNode.Nodes.Add(fileEntry, fileEntry);
                            matchingNodes = troybinNode.Nodes.Find(fileEntry, false);                   
                        }
                        TreeNode fileNode = matchingNodes[0];
                        
                        Color fileColor = troyColor;
                        FileInfo fileNodeInfo = new FileInfo(fileEntry);
                        int imageIndex = 5;
                        rafTreeDataObject filetag = new rafTreeDataObject();
                        switch (fileNodeInfo.Extension.ToLower())                        
                        {
                            case ".skl": imageIndex = 1; break;
                            case ".skn": imageIndex = 2; break;
                            case ".anm": imageIndex = 3; break;
                            case ".dds": imageIndex = 4; break;
                            case ".tga": imageIndex = 4; break;
                            case ".sco": imageIndex = 6; break;
                            case ".scb": imageIndex = 7; break;
                            default: imageIndex = 5; break;
                        }
                        string filefullpath = "";
                        fileNode.ToolTipText = "DATA\\Particles\\" + fileEntry;
                        filetag.fileLocation = fileEntry;
                        //still slow as hell
                        /*foreach (KeyValuePair<String, String> pairFileName_Path in allFilesList)
                        {
                            if (pairFileName_Path.Key.ToLower() == fileNodeInfo.Name.ToLower())
                            {
                                filefullpath = pairFileName_Path.Value;
                                break;
                            }
                        }*/
                        if (filefullpath != "")
                        {
                            filetag.rafPower = getRafPowerFromVersion(filefullpath);
                            fileColor = colorFromRafPower(filetag.rafPower);
                            fileNode.ToolTipText = filefullpath;
                            filetag.fileLocation = filefullpath;
                            if (filetag.rafPower < lowestPower) lowestPower = filetag.rafPower;
                        }
                        fileNode.Tag = tag;
                        fileNode.ImageIndex = fileNode.SelectedImageIndex = imageIndex;
                    }
                }
                champNode.ForeColor = colorFromRafPower(lowestPower);
            }

            e.Result = particleNode;
        }
        private void ParticleTreeWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage);
        }
        private void ParticleTreeWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TreeNode particleNode = treeView1.Nodes.Find("Particles Old", true)[0];
            particleNode.Nodes.Clear();
            foreach (TreeNode node in ((TreeNode)e.Result).Nodes)
            {
                particleNode.Nodes.Add(node);
            }
            treeView1.Sort();
            UpdateProgressSafe(100);
        }

        private void ParticleTreeWorkerNew_DoWork(object sender, DoWorkEventArgs e)
        {
            TreeNode particleNode = new TreeNode("Particles");//treeView1.Nodes.Find("Particles", false)[0];
            particleNode.Nodes.Clear();

            int i = 0;
            int lastProg = 0;

            foreach (KeyValuePair<String, Dictionary<String, Dictionary<RAFFileListEntry, List<String>>>> championKVP in
                (Dictionary
                <String, Dictionary
                    <String, Dictionary
                        <RAFFileListEntry, List<String>>>>)e.Argument)
            {
                i++;
                int prog = (int)(((double)i / (double)((Dictionary
                <String, Dictionary
                    <String, Dictionary
                        <RAFFileListEntry, List<String>>>>)e.Argument).Count) * 45) + 50;
                if (prog != lastProg) { lastProg = prog; ParticleTreeWorkerNew.ReportProgress(prog); }

                TreeNode champNode = particleNode.Nodes.Add(championKVP.Key, Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(championKVP.Key));
                //TreeNode champNode =particleNode.Nodes.Find(championKVP.Key, false)[0];
                int lowestPower = int.MaxValue;
                foreach (KeyValuePair<String, Dictionary<RAFFileListEntry, List<String>>> abilityKVP in championKVP.Value)
                {
                    TreeNode abilityNode = champNode.Nodes.Add(abilityKVP.Key, Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(abilityKVP.Key));

                    foreach (KeyValuePair<RAFFileListEntry, List<String>> troybinKVP in abilityKVP.Value)
                    {
                        FileInfo troyFileInfo = new FileInfo(troybinKVP.Key.FileName);
                        TreeNode troybinNode = abilityNode.Nodes.Add(troybinKVP.Key.FileName, Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(troyFileInfo.Name.Substring(0, troyFileInfo.Name.IndexOf("."))));

                        //TreeNode troybinNode = champNode.Nodes.Find(troybinKVP.Key.FileName, false)[0];
                        rafTreeDataObject tag = new rafTreeDataObject();

                        //get troy version number
                        string troyfullpath = "";
                        Color troyColor = Color.Red;
                        foreach (KeyValuePair<String, String> pairFileName_Path in allFilesList)
                        {
                            if (pairFileName_Path.Key.ToLower() == troyFileInfo.Name.ToLower())
                            {
                                troyfullpath = pairFileName_Path.Value;
                                break;
                            }
                        }
                        if (troyfullpath != "")
                        {
                            tag.rafPower = getRafPowerFromVersion(troyfullpath);
                            troyColor = colorFromRafPower(tag.rafPower);
                            if (tag.rafPower < lowestPower) lowestPower = tag.rafPower;
                        }

                        tag.fileLocation = troyfullpath;
                        troybinNode.ToolTipText = tag.fileLocation;
                        troybinNode.ForeColor = troyColor;
                        troybinNode.Tag = tag;

                        foreach (String fileEntry in troybinKVP.Value)
                        {
                            TreeNode[] matchingNodes = troybinNode.Nodes.Find(fileEntry, false);
                            if (matchingNodes.Length == 0)//avoid duplicate entries
                            {
                                troybinNode.Nodes.Add(fileEntry, fileEntry);
                                matchingNodes = troybinNode.Nodes.Find(fileEntry, false);
                            }
                            TreeNode fileNode = matchingNodes[0];

                            Color fileColor = troyColor;
                            FileInfo fileNodeInfo = new FileInfo(fileEntry);
                            int imageIndex = 5;
                            rafTreeDataObject filetag = new rafTreeDataObject();
                            switch (fileNodeInfo.Extension.ToLower())
                            {
                                case ".skl": imageIndex = 1; break;
                                case ".skn": imageIndex = 2; break;
                                case ".anm": imageIndex = 3; break;
                                case ".dds": imageIndex = 4; break;
                                case ".tga": imageIndex = 4; break;
                                case ".sco": imageIndex = 6; break;
                                case ".scb": imageIndex = 7; break;
                                default: imageIndex = 5; break;
                            }
                            string filefullpath = "";
                            fileNode.ToolTipText = "DATA\\Particles\\" + fileEntry;
                            filetag.fileLocation = fileEntry;
                            //still slow as hell
                            /*foreach (KeyValuePair<String, String> pairFileName_Path in allFilesList)
                            {
                                if (pairFileName_Path.Key.ToLower() == fileNodeInfo.Name.ToLower())
                                {
                                    filefullpath = pairFileName_Path.Value;
                                    break;
                                }
                            }*/
                            if (filefullpath != "")
                            {
                                filetag.rafPower = getRafPowerFromVersion(filefullpath);
                                fileColor = colorFromRafPower(filetag.rafPower);
                                fileNode.ToolTipText = filefullpath;
                                filetag.fileLocation = filefullpath;
                                if (filetag.rafPower < lowestPower) lowestPower = filetag.rafPower;
                            }
                            fileNode.Tag = tag;
                            fileNode.ImageIndex = fileNode.SelectedImageIndex = imageIndex;
                        }
                    }
                }
                champNode.ForeColor = colorFromRafPower(lowestPower);
            }

            e.Result = particleNode;
        }
        private void ParticleTreeWorkerNew_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage);
        }
        private void ParticleTreeWorkerNew_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TreeNode particleNode = treeView1.Nodes.Find("Particles New", true)[0];
            particleNode.Nodes.Clear();
            foreach (TreeNode node in ((TreeNode)e.Result).Nodes)
            {
                particleNode.Nodes.Add(node);
            }
            treeView1.Sort();
            UpdateProgressSafe(100);
        }
        private void treeView1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = treeView1.GetNodeAt(p);
                if (node != null)
                {

                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    //m_OldSelectNode = treeView1.SelectedNode;
                    treeView1.SelectedNode = node;

                    //Find the appropriate ContextMenu depending on the selected node.
                    if (node.Tag != null)
                    {
                        rafTreeDataObject tagObj = ((rafTreeDataObject)node.Tag);
                        if (tagObj.fileLocation == "Skin")
                            treeMenuStripSkin1.Show(treeView1, p);
                        else
                            treeViewMenuStrip1.Show(treeView1, p);
                    }else
                        treeViewMenuStrip1.Show(treeView1, p);

                    // Highlight the selected node.
                    //treeView1.SelectedNode = m_OldSelectNode;
                    //m_OldSelectNode = null;
                }
                else treeViewMenuStrip1.Show(treeView1, p);
            }
        }
        #endregion
        private void buttoncancel_Click(object sender, EventArgs e)
        {
            if (fileListWorker1.IsBusy)
                fileListWorker1.CancelAsync();
        }
        #region MenuFileStuff
        public List<KeyValuePair<string, string>> getMenuFilePaths()
        {
            Regex regex = new Regex(
        @"\\\\managedfiles\\\\\d+\.\d+\.\d+\.\d+\\\\data\\\\menu\\\\fontconfig_"+getLocale()+@"\.txt",
                RegexOptions.IgnoreCase
                );
            return new List<KeyValuePair<string, string>>(
                allFilesList.Where(m => regex.Match(m.Value).Success));
            //return gameDirectory + matches[0].Value.Replace("\\\\", "\\").Substring(1);
           
        }
        public string getLocale()
        {
            string to_return = "en_us";
            string path = gameDirectory+"rads\\system\\locale.cfg";
            if(File.Exists(path))
            {
                TextReader reader = new StreamReader(path);
                string[] lines = reader.ReadLine().Split(new string[1] { "=" }, StringSplitOptions.None);
                reader.Close();
                //locale = en_gb
                if(lines.Length>1)
                    to_return = lines[1].Trim();
            }
            return to_return;
                
        }
        public string getMenuFilePath(bool showError=true)
        {
            List<KeyValuePair<string, string>> matched = getMenuFilePaths();
            string textPath = "";
            if (matched.Count == 1)
            {
                textPath = gameDirectory + matched[0].Value.Replace("\\\\", "\\").Substring(1);
            }
            else if (matched.Count == 0)
            {
                if(showError)Cliver.Message.Show("Can't find menu!", SystemIcons.Error,
                    "Unable to find the menu file to change text in\r\nWe will skip this part."
                    , 0, new string[1] { "Ok", });
                return "";
            }
            else
            {
                int bestScore = 0;
                List<KeyValuePair<string, string>> betterOptions = new List<KeyValuePair<string, string>>();
                foreach (KeyValuePair<string, string> pair in matched)
                {
                    Regex r1 = new Regex(@"\\(\d+\.\d+\.\d+\.\d+)\\");
                    Match match = r1.Match(pair.Value);

                    if (match.Success)
                    {
                        string versionString = match.Groups[1].Value;
                        string[] versions = versionString.Split('.');
                        int total = 0;
                        int multiplier = 1;
                        for (int i = versions.Length - 1; i >= 0; i--)
                        {
                            int vA = int.Parse(versions[i].Trim());
                            total += (vA * multiplier);
                            multiplier += 500;
                        }
                        if (total == bestScore)
                        {
                            betterOptions.Add(pair);
                        }
                        else if (total > bestScore)
                        {
                            betterOptions.Clear();
                            betterOptions.Add(pair);
                            bestScore = total;
                        }
                    }
                }
                if (betterOptions.Count == 1)
                {
                    textPath = gameDirectory+betterOptions[0].Value.Replace("\\\\", "\\").Substring(1);
                }
                else
                {
                    //if we still cant tell, ask
                    List<string> options = new List<string>();
                    foreach (KeyValuePair<string, string> pair in matched)
                    {
                        options.Add(pair.Key);
                    }
                    int option = Cliver.Message.Show("Select Menu Language",
                        SystemIcons.Information, "Which menu are you using?", 0, options.ToArray());
                    textPath = gameDirectory+matched[option].Value.Replace("\\\\", "\\").Substring(1);
                }

            }
            return textPath;
        }
        private void showMenuFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string paths = "";
            foreach (KeyValuePair<string, string> pair in getMenuFilePaths())
            {
                paths += gameDirectory +pair.Value.Replace("\\\\", "\\").Substring(1)+"\r\n";
            }
            paths += "\r\n Final Choice is \r\n" + getMenuFilePath();
            Cliver.Message.Inform("Menu File is at\r\n" + paths);
        }
        private void openTextTreeEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //hi ted
            ted.ShowDialog();
        }
        #endregion
        #region versionDetection
        private string getVersionFilePath()
        {
            return gameDirectory+"RADS\\last_versions";
        }
        private void getVersionFilePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cliver.Message.Inform("Version file is at \r\n\r\n" + getVersionFilePath());
        }
        private void readVersionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            riotVersions rv = new riotVersions(getVersionFilePath());

            string theCSV = rv.toCSVString();
            riotVersions testRV = new riotVersions();
            testRV.fromCSVString(theCSV);
            Cliver.Message.Inform("Versions are\r\n\r\n"
                + rv.ToString()+"\r\n\r\nTest Result is\r\n"+
                testRV.ToString()+"\r\n\r\nAnd our equals shows that they are "+
                (rv.Equals(testRV)?"Equal!":"Not Equal :<")+
                "\r\n\r\nThe CSV Used was \r\n"+theCSV
                );
        }
        private DateTime getLastModDate()
        {
            //check version file mod data
            //check raf file creation (not mod!) date
            //check empty data folder leftovers date
            //check air folder
            FileInfo versionFile = new FileInfo(getVersionFilePath());
            DateTime newestDate = versionFile.LastWriteTime;
            //Cliver.Message.Inform("version date i s" + newestDate.ToLongDateString() +" - "+ newestDate.ToLongTimeString());
            //C:\Riot Games\League of Legends\RADS\projects\lol_game_client\filearchives\0.0.0.25
            string rafPath = gameDirectory + @"RADS\projects\lol_game_client\filearchives\";
            if (Directory.Exists(rafPath))
            {
                string[] files = Directory.GetFiles(rafPath, "*.raf.dat*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    FileInfo rafFile = new FileInfo(file);
                    if (rafFile.CreationTime > newestDate) newestDate = rafFile.CreationTime;
                    // Cliver.Message.Inform("raf date i s"+rafFile+" is " + newestDate.ToLongDateString() + " - " + newestDate.ToLongTimeString());

                    string dataFolerPath = rafFile.DirectoryName + "\\DATA\\";
                    //Cliver.Message.Inform("data folder is " + dataFolerPath);
                    FileInfo datafolder = new FileInfo(dataFolerPath);
                    if (datafolder.CreationTime > newestDate) newestDate = datafolder.CreationTime;
                    //if (datafolder.LastAccessTime > newestDate) newestDate = datafolder.LastAccessTime;
                    if (datafolder.LastWriteTime > newestDate) newestDate = datafolder.LastWriteTime;
                    //Cliver.Message.Inform("data date i s" + rafFile + " is " + newestDate.ToLongDateString() + " - " + newestDate.ToLongTimeString());

                    //Cliver.Message.Inform("file name is " + file);
                }

                string airDir = gameDirectory + @"RADS\projects\league_client\releases";
                files = Directory.GetDirectories(airDir, "*", SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    FileInfo airFolderInfo = new FileInfo(file);
                    if (airFolderInfo.CreationTime > newestDate) newestDate = airFolderInfo.CreationTime;
                    // Cliver.Message.Inform("air date i s" + file + " is " + newestDate.ToLongDateString() + " - " + newestDate.ToLongTimeString());

                }
            }
            else
            {
                newestDate = DateTime.Now;
                newestDate = new DateTime(newestDate.Year, newestDate.Month, newestDate.Day);
            }
            
            return newestDate;
        }

        private void getLastModDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cliver.Message.Inform("Last mod date was...\r\n\r\n"+getLastModDate().ToString());
        }

        #endregion
        #region webbrowserBs
        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            //Cliver.Message.Inform("new windows\r\n" + e.ToString());
            try
            {
            
                HtmlElement link = webBrowser1.Document.ActiveElement;
                String url = link.GetAttribute("href");
                if (url.Equals(""))
                {
                    e.Cancel = false;
                    return;
                }
                if (url.StartsWith("//"))
                {

                }
                else if (url.StartsWith("/"))
                    url = webBrowser1.Url.Host + url;

                //Cliver.Message.Inform("link is \r\n" + url);
                System.Diagnostics.Process.Start(url);
                e.Cancel = true;
            }
            catch (System.Exception ex)
            {

            }
        }
        private void link_MouseUp(object sender, HtmlElementEventArgs e)
        {
           /* mshtml.HTMLAnchorElementClass a = (mshtml.HTMLAnchorElementClass)((HtmlElement)sender).DomElement;
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    {
                        string url = a.href;
                        // open new tab
                        if (url.StartsWith("//"))
                        {

                        }
                        else if (url.StartsWith("/"))
                            url = webBrowser1.Url.Host + url;

                        //Cliver.Message.Inform("link is \r\n" + url);
                        System.Diagnostics.Process.Start(url);
                        break;
                    }
                case MouseButtons.Right:
                    {
                        // open context menu
                        break;
                    }
            }*/
        }
        private void webBrowser1_NewWindow2(object sender, NewWindow2EventArgs e)
        {
            
            try
            {
	            clicked = true;
	                      //  Cliver.Message.Inform("new windows2\r\n" + e.ToString()+"\r\n\r\n");
	                        e.PPDisp = webBrowser2Test.Application;
            }
            catch (System.Exception ex)
            {
	
            }
        }
        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
	            if (mainWebReady)
	                        {
	                            //Cliver.Message.Inform("Nav " + e.Url + "\r\n\r\nAt " + e.TargetFrameName.ToString());
	                            //_MyUrl = e.Url;
	                            string url = e.Url.ToString();
	                            if (url.StartsWith("/"))
	                            url = webBrowser1.Url.Host + url;
	                
	                            System.Diagnostics.Process.Start(url);
	                           e.Cancel=true;
	                        }
            }
            catch (System.Exception ex)
            {
	
            }
           
        }
        private void button3openAd_Click(object sender, EventArgs e)
        {
            doBrowser(true);
        }
        void webBrowser2Test_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            waitForWeb = false;
        }
       
        public bool waitForWeb = false;
        public bool clicked = false;
        public bool mainWebReady = false;
        void webBrowser2_BeforeNavigate2(object sender, BeforeNavigate2EventArgs e)
        {
           try
           {
               if (waitForWeb) return;
               if (!clicked) return;

               if ((bool)this.webBrowser2Test.Tag == true)
               {
                   if (
                     ((int)e.flags == 64 || true)

                       && e.url.ToString() != "about:blank")
                   {
                       e.Cancel = true;
                       if (e.url.ToString().ToLower().Contains("http"))
                           System.Diagnostics.Process.Start(e.url.ToString());

                       e.Cancel = true;
                       waitForWeb = true;

                       clicked = false;

                   }
               }
           }
           catch (System.Exception ex)
           {

           }
            //  waitForWeb = true;
          //  while (webBrowser2Test.ReadyState != WebBrowserReadyState.Complete)
           // {
          //      Application.DoEvents();
          //  }
          //  waitForWeb = false;
          //      
            
        }
        void webBrowser1_BeforeNavigate2(object sender, BeforeNavigate2EventArgs e)
        {
            try
            {
	            if (mainWebReady)
	                        {
	                            //Cliver.Message.Inform("Srs click! @@\r\n\r\n " + e.url.ToString());
	                            //e.url = "about:blank";
	                            string url = e.url.ToString();
	                            if (url.StartsWith("/"))
	                            url = webBrowser1.Url.Host + url;
	                
	                            System.Diagnostics.Process.Start(url);
	                            e.Cancel = true;
	                        }//else
	                            //Cliver.Message.Inform("Normal click to follow click! @@\r\n\r\n " + e.url.ToString());
            }
            catch (System.Exception ex)
            {
	
            }
        }
        private static bool TrySetSuppressScriptErrors(WebBrowser webBrowser, bool value)
        {
            FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                object axIWebBrowser2 = field.GetValue(webBrowser);
                if (axIWebBrowser2 != null)
                {
                    axIWebBrowser2.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, axIWebBrowser2, new object[] { value });
                    return true;
                }
            }

            return false;
        }
        private void doBrowser(bool on)
        {
            try
            {

                //on = false;//screw it :|
                if (on)
                {
                    if (this.webBrowser1 == null)
                    {
                        this.webBrowser1 = new SkinInstaller.ExtendedWebBrowser();
                        //this.webBrowser1 = new Axwebbroswer;
                        this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
                        this.webBrowser1.Location = new System.Drawing.Point(0, 0);
                        this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
                        this.webBrowser1.Name = "webBrowser1";
                        this.webBrowser1.ScrollBarsEnabled = false;
                        this.webBrowser1.Size = new System.Drawing.Size(154, 306);
                        this.webBrowser1.TabIndex = 1;
                        this.webBrowser1.ScriptErrorsSuppressed = true;

                        this.webBrowser1.Url = new System.Uri("http://Lɢɢ.com/league_of_legends_video_game_mod.html", System.UriKind.Absolute);
                        this.webBrowser1.NewWindow2 += new System.EventHandler<SkinInstaller.NewWindow2EventArgs>(this.webBrowser1_NewWindow2);
                        this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser1_Navigating);
                        this.webBrowser1.NewWindow += new System.ComponentModel.CancelEventHandler(this.webBrowser1_NewWindow);
                        this.webBrowser1.BeforeNavigate2 += new EventHandler<BeforeNavigate2EventArgs>(webBrowser1_BeforeNavigate2);
                        this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);

                        TrySetSuppressScriptErrors(this.webBrowser1, true);
                        this.splitContainer6.Panel2.Controls.Add(this.webBrowser1);
                        //this.webBrowser1.Navigate("http://lgg1.comeze.com/Home/");
                    }
                    else
                    {

                        //this.webBrowser1.Navigate("http://lgg1.comeze.com/league_of_legends_video_game_mod.html");
                    }
                }
                /*if (webBrowser1 != null)
                {
                    webBrowser1.Visible = on;
                    //webBrowser1.Refresh();
                    while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(20);
                    }
                    IHTMLElement2 body = webBrowser1.Document.Body.DomElement as IHTMLElement2;
                    IHTMLElement2 doc = (webBrowser1.Document.DomDocument as IHTMLDocument3).documentElement as IHTMLElement2;
                
                    doc.scrollLeft = 20;
                    doc.scrollTop = 1;

                }*/
                button3CloseAd.Enabled = button3CloseAd.Visible = on;
                button3openAd.Visible = button3openAds2.Visible = button3openAds3.Visible =
                    button3openAd.Enabled = button3openAds2.Enabled = button3openAds3.Enabled =
                    !on;
                int roomForWeb = (splitContainer6.Size.Width - (/*886 - 728*/190));
                if (roomForWeb < 200) roomForWeb = 200;
                splitContainer6.SplitterDistance = on ? roomForWeb : splitContainer6.Size.Width - 0;

                Properties.Settings.Default.showAds = on;
                Properties.Settings.Default.Save();
            }
            catch (System.Exception ex)
            {

            }

        }
        private void Window_Error(object sender,
            HtmlElementErrorEventArgs e)
        {
            // Ignore the error and suppress the error dialog box. 
            e.Handled = true;
        }
        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (true && e.Url.AbsoluteUri == "http://lɢɢ.com/league_of_legends_video_game_mod.html")
            {
               // System.Threading.Thread.Sleep(5000);
                try
                {
                     ((WebBrowser)sender).Document.Window.Error += 
                    new HtmlElementErrorEventHandler(Window_Error);

/*
                    foreach (HtmlElement tag in webBrowser1.Document.All)
                    {
                        tag.Id = String.Empty;
                        switch (tag.TagName.ToUpper())
                        {
                            case "A":
                                {
                                    tag.MouseUp += new HtmlElementEventHandler(link_MouseUp);
                                    break;
                                }
                        }
                    }
                    mainWebReady = true;
                    webBrowser1.Document.MouseUp += new HtmlElementEventHandler(Document_MouseUp);
 * */
                }
                catch (System.Exception ex)
                {

                }
                
            }
        }
        void Document_MouseUp(object sender, HtmlElementEventArgs e)
        {
            //Cliver.Message.Inform("Click!");
        }
        private void button3CloseAd_Click(object sender, EventArgs e)
        {
            doBrowser(false);
        }
        #endregion

        private void deleteBackupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Cliver.Message.Show("Are you sure?",
                                SystemIcons.Information,
                                "You should NOT delete backups unless you have just repaired or reinstalled LoL\r\n"+
                "\r\nGo to the help menu to learn how to repair\r\n\r\nAre you sure you want to delete backups now?\r\nThis process can not be undone.",
                                0, new string[2] { "No! Never mind!", "Yes" }) == 1)
            {

                String backupDir = Application.StartupPath + @"\backup\";
                SIFileOp.DirectoryDelete(backupDir,true);
            }
        }
        #region PatchFixer
        private void button3FixCrashAfterPatch_Click(object sender, EventArgs e)
        {
            removeSoftRepairFile();
            if (Cliver.Message.Show("Are you sure?",
                                SystemIcons.Information,
                                "This process will attempt to repair LoL to a working state after a patch\r\n" +
                                "You do not need to run this if a LoL Patch has not happened.\r\n"+
                                "Sometimes even without removing your skins (You can always press install again if you need to\r\n"+
                "\r\nDo not install any skins untill the progress bar completes\r\n\r\nAre you sure you want to do this now?",
                                0, new string[2] { "Yes", "Not Now" }) == 0)
            {

                if (patchFixerWorker.IsBusy)
                {
                    Cliver.Message.Show("Already fixing!", SystemIcons.Error,
                    "We are already fixing the lol patch, please watch the progress bar bellow."
                    , 0, new string[1] { "Ok", });
                    return;
                }
                patchFixerWorker.RunWorkerAsync();
            }

        }

        private void patchFixerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] files = Directory.GetFiles(gameDirectory, "*.raf.dat", SearchOption.AllDirectories);
            //		[0]	"C:\\Riot Games\\League of Legends\\RADS\\projects\\lol_game_client\\filearchives\\0.0.0.99\\Archive_155085344.raf.dat"	string
            Dictionary<int, RAFArchive> rafs = new Dictionary<int, RAFArchive>();

            foreach (string file in files)
            {
                Regex r1 = new Regex(@"\\(\d+\.\d+\.\d+\.\d+)\\");
                Match match = r1.Match(file);
                string versionString = "";
                if (match.Success)
                {
                    int version = 0;
                    versionString = match.Groups[1].Value;
                    string[] parts = versionString.Split(new string[1] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string part in parts)
                    {
                        int innerV = 0;
                        int.TryParse(part, out innerV);
                        version += innerV;
                    }
                    string rafFile = file.Substring(0, file.Length - 4);
                    if (File.Exists(rafFile))
                    {
                        RAFArchive raf = new RAFArchive(rafFile);
                        rafs.Add(version, raf);
                    }

                }
                
            }
            List<string> notSames = new List<string>();
            RelManDirectoryFile rmdf = RelManDirectoryFile.RelManDirectoryFileFromRiotRoot(gameDirectory);
            int inc = 0;
            int count = rmdf.fileList.fileEntries.Count;
            int lastP = -1;
            int entriesFixed = 0;
            foreach (RelFileEntry entry in rmdf.fileList.fileEntries)
            {
                int percent = (int)Math.Floor((double)inc++ / (double)(count + 1) * (double)100.0);
                if (lastP != percent)
                {
                    lastP = percent;
                    patchFixerWorker.ReportProgress(percent);
                }
                if (entry.folder == null) continue;
                if (entry.folder.name == "") continue;
                int version = (int)entry.version;
                string rmdfPath = entry.getPathAndName().Substring(1);
                if (rafs.ContainsKey(version))
                {
                    RAFArchive raf = rafs[version];
                    RAFFileListEntry rafEntry = raf.GetDirectoryFile().GetFileList().GetFileEntry(rmdfPath);
                    if (rafEntry != null)
                    {
                        int rafFileSize = (int)rafEntry.FileSize;
                        int rmdfFileSizeCompressed = (int)entry.compressedFileSize;
                        int rmdfFileSizeUncompressed = (int)entry.uncompressedFileSize;
                        if (rafFileSize != rmdfFileSizeCompressed && rafFileSize != rmdfFileSizeUncompressed)
                        {
                            string debugInfo = entry.getPathAndName().ToString() + " does not match.";
                            notSames.Add(debugInfo);
                            debugadd(debugInfo);
                            int uncompressedRafSize = rafEntry.GetContent().Length;
                            entry.uncompressedFileSize = (UInt32)uncompressedRafSize;
                            entry.compressedFileSize = (UInt32)rafFileSize;
                            entriesFixed++;
                        }
                    }

                }
            }
            foreach (KeyValuePair<int,RAFArchive> rafEntry in rafs)
            {
                rafEntry.Value.GetDataFileContentStream().Close();
            }
            rmdf.saveFile();
            e.Result = entriesFixed;

        }

        private void patchFixerWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage);
        }

        private void patchFixerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int numFixed = (int)e.Result;
            Cliver.Message.Show("Fix Complete", SystemIcons.Information,
                "We fixed "+numFixed.ToString()+ " files!"+
                    "Please test lol, it should not crash because of a patch any more.\r\n\r\n"+
                    "Please note this process only fixes crashes caused by a patch\r\nInvalid skins can still crash LoL, and will need to be uninstalled."
                    , 0, new string[1] { "Ok", });
            UpdateProgressSafe(100);
        }
        #endregion PatchFixer

        private void copyLikeFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicateFileToMatchSkinForm dftmsf = new DuplicateFileToMatchSkinForm(this);
            dftmsf.Show();
        }

        private void fixRiotWarningsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeSoftRepairFile();
            Cliver.Message.Show("Fix Complete", SystemIcons.Information,
                "Fix Complete, you should no longer see this warning." +"\r\n\r\n" +
                    "If you still have issue, you may need to do a full repair."
                    , 0, new string[1] { "Ok", });
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    #region strucks
    public class LogTextWriter : TextWriter
    {
        public Func<string, object> writeLineHandler;
        public LogTextWriter(Func<string, object> writeLineHandler)
        {
            this.writeLineHandler = writeLineHandler;
        }
        public override void WriteLine()
        {
            WriteLine("");
        }
        public override void WriteLine(string value)
        {
            writeLineHandler(value);
        }
        public override Encoding Encoding
        {
            get
            {
                return Encoding.ASCII;
            }
        }
    }
    public struct rafTreeDataObject
    {
        public rafTreeDataObject(int rafPowerIn, string fileLocationIn)
        {
            rafPower=rafPowerIn;
            fileLocation = fileLocationIn;
        }
        public int rafPower;
        public string fileLocation;
    }
    public struct installFileInfo
    {
        public string origonal;
        public string fileName;
        public string filePath;
        public string getFileNamePath()
        {
            return filePath+fileName;
        }
        public installFileInfo(string _orig, string _fn, string _fp)
        {
            origonal = _orig;
            fileName = _fn;
            filePath = _fp;
        }
        
    }
    public class NodeSorter : IComparer
    {
        // compare between two tree nodes
        public int Compare(object thisObj, object otherObj)
        {
            TreeNode thisNode = thisObj as TreeNode;
            TreeNode otherNode = otherObj as TreeNode;
            if ((thisNode.Nodes.Count != otherNode.Nodes.Count)&&(thisNode.Nodes.Count==0 || otherNode.Nodes.Count==0))
            {
                return -1*thisNode.Nodes.Count.CompareTo(otherNode.Nodes.Count);
            }
            //try extension sort too.
            int dotIndex1 =thisNode.Text.LastIndexOf(".");            
            int dotIndex2 =otherNode.Text.LastIndexOf(".");
            if(dotIndex1>=0 && dotIndex2>=0)
            {
                string ext1 = thisNode.Text.Substring(dotIndex1).ToLower();
                string ext2 = otherNode.Text.Substring(dotIndex2).ToLower();
                if (ext1 != ext2)
                {
                    return ext1.CompareTo(ext2);
                }
            }

            return thisNode.Text.CompareTo(otherNode.Text);
        }
    } 
    #endregion
}

