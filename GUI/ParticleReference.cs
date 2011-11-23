﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RAFLib;
using System.Text.RegularExpressions;
using SkinInstaller;

namespace SkinInstaller
{
    public partial class ParticleReference : Form
    {
        public ParticleReference()
        {
            InitializeComponent();
        }

        public class PStruct : Dictionary
                <String, Dictionary
                    <String, Dictionary
                        <RAFFileListEntry, List<RAFFileListEntry>>>> { }
        public skinInstaller mySkinInstaller = null;
        public int lastProgress = 0;
        string progs = "";

        private void ParticleReference_Load(object sender, EventArgs e)
        {
            
        }
        public void reportProgress(int p)
        {
            if (p != lastProgress)
            {
                lastProgress = p;

                particleReaderWorker.ReportProgress(p);
                progs += "|" + p.ToString() + "|";
                
            }
        }
        public PStruct getParticleStructure(string rafPath)
        {

            List<RAFFileListEntry> fileList = new List<RAFFileListEntry>();
            String baseDir = rafPath;
            string[] files = Directory.GetFiles(baseDir, "*", SearchOption.AllDirectories);
            Directory.GetDirectories(baseDir);
            string[] array = files;
            List<String> rafFiles = new List<string>();
            reportProgress(1);
            int i = 0;
            for (i = 0; i < array.Length; i++)
            {
                FileInfo fileInfo = new FileInfo(array[i]);

                if (fileInfo.Extension == ".raf")
                {
                    if (File.Exists(fileInfo.FullName + ".dat"))
                    {
                        rafFiles.Add(fileInfo.FullName);//add raf files to read afterwards
                    }
                }
                reportProgress((int)(((double)i * 10.0) / (double)array.Length)+1);
            }
            reportProgress(12);
            i = 0;
            List<RAFFileList> listsToSearchLater = new List<RAFFileList>();
            foreach (String file in rafFiles)
            {
                i++;
                FileInfo rafFile = new FileInfo(file);
                //time to process the raf files
                RAFArchive raf = new RAFArchive(rafFile.FullName);
                fileList.AddRange(raf.GetDirectoryFile().GetFileList().GetFileEntries());
                listsToSearchLater.Add(raf.GetDirectoryFile().GetFileList());
                raf.GetDataFileContentStream().Close();
                reportProgress((int)(((double)i * 10.0) / (double)rafFiles.Count) + 12);
            }
            reportProgress(25);

            PStruct particleDef = new PStruct();
            List<String> exceptions = new List<String>();
            i = 0;
            // Reference spellnames to champion names
            foreach ( RAFFileListEntry file in fileList)
            {
                i++;
                reportProgress((int)(((double)i/ (double)fileList.Count) * (double)10.0)+ 25);
                // Only use spell icon files
                if (file.FileName.Contains("DATA/Spells/Icons2D"))
                {
                    // Make sure it is a champion icon
                    String iconFileName = file.FileName.Substring(file.FileName.LastIndexOf('/') + 1, file.FileName.Length - file.FileName.LastIndexOf('/') - 1);
                    if (char.IsLetter(iconFileName.Substring(0, 1).ToCharArray()[0]))
                    {
                        String championName = String.Empty;
                        String spell = String.Empty;
                        // Naming scheme: Word separations is either with underscores or camelcase letters
                        if (iconFileName.Contains("_"))
                        {
                            championName = iconFileName.Split('_')[0].ToLower();
                            // Change the name of Rumble's ult so code isn't lookin for "r" as a spell name
                            if (iconFileName == "Rumble_R.dds")
                            {
                                spell = "rumble_ult";
                            }
                            else
                            {
                                spell = iconFileName.Split('_')[1].Split('.')[0].ToLower().Replace(" ", "");
                            }
                        }
                        else

                        {
                            // Find index of second uppercase letter
                            int splitIndex = 0;
                            char[] charArray = iconFileName.ToCharArray();
                            for (int ii = 1; ii < charArray.Length; ii++)
                            {
                                if (char.IsUpper(charArray[ii]))
                                {
                                    splitIndex = ii;
                                    break;
                                }
                            }

                            if (splitIndex == 0)
                            {
                                // Store exceptions for debugging purposes
                                exceptions.Add(iconFileName);
                                continue;
                            }

                            championName = iconFileName.Substring(0, splitIndex).ToLower();
                            // Ignore "IsPriate, IsNinja, etc. "
                            if (championName == "is")
                                continue;
                            spell = iconFileName.Substring(splitIndex, iconFileName.Length - splitIndex).Split('.')[0].ToLower().Replace(" ", "");
                        }

                        // Don't add passives to spell list because the name isn't unique
                        // All passives are named along with their champion name so no troybins should be skipped
                        if (spell == "passive")
                            continue;

                        // Create dictionary structure if it doesn't already exist
                        if (!particleDef.ContainsKey(championName))
                        {
                            particleDef[championName] = new Dictionary<String, Dictionary<RAFFileListEntry, List<RAFFileListEntry>>>();
                            particleDef[championName]["troybins"] = new Dictionary<RAFFileListEntry, List<RAFFileListEntry>>();
                        }
                        particleDef[championName][spell] = new Dictionary<RAFFileListEntry, List<RAFFileListEntry>>();
                    }
                    else
                    {
                        // Store exceptions for debugging purposes
                        exceptions.Add(iconFileName);
                    }
                }
            }

            List<RAFFileListEntry> missedTroybins = new List<RAFFileListEntry>();

            // 
            // Need to add exceptions for champions who's name spelling changes from icon to troybin
            // List so far: Orianna, Maokai, Xin Xhao, Tryndamere (?Dark champion?), Evelynn, 
            //
            // Figure out why 2 troybins are being missed (3011 vs 3013)
            //
            // Create "other" champion which will contain all the extra troybins, like nexus, summoner spells, general poison, et.
            //
            // Add exception for Trundle's bite vs. Anivia's Frostbite, Swain's torment vs. Morganna's tormented soul
            //
            // Delete champions: "eagleeye", "spiderqueen", "gw", "bantamsting", "toxicshot", "rod", "pet", "spell", "leblancmirrorimage", "storm", 
            //                   "odin", "crystal", 
            //
            // Add exception to champion/spell splitting for MonkeyKing
            //
            //
            //

            int troybinTotal = 0;
            i = 0;
            reportProgress(36);
            // Search for troybins and inibins that correspond to the champion names
            foreach (RAFFileListEntry file in fileList)
            {
                i++;
                reportProgress((int)(((double)i * 10.0) / (double)fileList.Count) + 36);
                if (file.FileName.Contains("DATA/Particles"))
                {
                    String particleFileName = file.FileName.ToLower();

                    // Only look for troybins and inibins
                    if (particleFileName.IndexOf("troybin") != -1 || particleFileName.IndexOf("inibin") != -1)
                    {
                        troybinTotal++; // For debugging purposes
                        Boolean matchFound = false;
                        foreach (KeyValuePair<String, Dictionary<String, Dictionary<RAFFileListEntry, List<RAFFileListEntry>>>> championKVP in particleDef)
                        {
                            // Search for the champion's name in the troybin file name
                            if (particleFileName.Replace("_", "").IndexOf(championKVP.Key) != -1)
                            {
                                particleDef[championKVP.Key]["troybins"][file] = new List<RAFFileListEntry>();
                                matchFound = true;
                                break;
                            }
                        }
                        if(!matchFound)
                            missedTroybins.Add(file);
                    }
                }
            }

            List<RAFFileListEntry> leftoverTroybins = new List<RAFFileListEntry>();
            i = 0;
            reportProgress(47);
            // Search for troybins and inibins that correspond to the champion spells
            foreach (RAFFileListEntry troybin in missedTroybins)
            {
                i++;
                reportProgress((int)(((double)i * 10.0) / (double)missedTroybins.Count) + 47);
                Boolean matchFound = false;
                foreach (KeyValuePair<String, Dictionary<String, Dictionary<RAFFileListEntry, List<RAFFileListEntry>>>> championKVP in particleDef)
                {
                    // Only use spells not troybin list
                    if(championKVP.Key != "troybins")
                    {
                        // Search for spell names in the troybin file name
                        if (troybin.FileName.Replace("_", "").IndexOf(championKVP.Key.Replace("_", "")) != -1)
                        {
                            if (!particleDef[championKVP.Key]["troybins"].ContainsKey(troybin))
                            {
                                particleDef[championKVP.Key]["troybins"][troybin] = new List<RAFFileListEntry>();
                            }
                            matchFound = true;
                            break;
                        }
                    }
                }
                if (!matchFound)
                    leftoverTroybins.Add(troybin);
            }
            reportProgress(58);
            i = 0;
            foreach (KeyValuePair<String, Dictionary<String, Dictionary<RAFFileListEntry, List<RAFFileListEntry>>>> championKVP in particleDef)
            {
                reportProgress((int)(((double)i * 40) / (double)particleDef.Count) + 58);
                i++;
                foreach (KeyValuePair<RAFFileListEntry, List<RAFFileListEntry>> troybinKVP in particleDef[championKVP.Key]["troybins"])
                {
                    // Search troybins for .dds, .sco, .scb, etc.
                    // Create a new archive
                    RAFArchive rafArchive = new RAFArchive(troybinKVP.Key.RAFArchive.RAFFilePath);

                    // Get the data from the archive
                    MemoryStream myInput = new MemoryStream(rafArchive.GetDirectoryFile().GetFileList().GetFileEntry(troybinKVP.Key.FileName).GetContent());
                    StreamReader reader = new StreamReader(myInput);
                    String result = reader.ReadToEnd();
                    reader.Close();
                    myInput.Close();
                    // Release the archive
                    rafArchive.GetDataFileContentStream().Close();

                    String cleanString = Regex.Replace(result, @"[^\u0000-\u007F]", "").Replace('\0','?');
                    Regex captureFileNames = new Regex(@"([a-zA-z0-9\-_ ]+\.(?:tga|sco|scb|dds|png))",RegexOptions.IgnoreCase);
                    MatchCollection matches = captureFileNames.Matches(cleanString);
                    foreach (Match match in matches)
                    {
                        string matchedFile = "/" + match.Groups[1].Value;

                        //foreach (String file in rafFiles)
                        //{
                        foreach(RAFFileList listToSearchNow in listsToSearchLater)
                        {
                            //FileInfo rafFile = new FileInfo(file);
                            //time to process the raf files
                            //RAFArchive searchRaf = new RAFArchive(rafFile.FullName);

                            //searching takes forever, F that
                            //troybinKVP.Value.AddRange(listToSearchNow.SearchFileEntries(matchedFile));
                            
                            //damnit this also takes freking forever!
                            /*
                            bool found = false;
                            RAFFileListEntry entry =
                                    listToSearchNow.GetFileEntry("DATA/Particles" + matchedFile);
                            if (entry != null)
                            {
                                troybinKVP.Value.Add(entry);
                                found = true;
                            }
                            if (matchedFile.ToLower().Contains(".tga"))
                            {
                                entry =
                                    listToSearchNow.GetFileEntry("DATA/Particles" + matchedFile.ToLower().Replace(".tga",".dds"));
                                if (entry != null)
                                {
                                    troybinKVP.Value.Add(entry);
                                    found = true;
                                }
                            }
                            if (!found)
                            {
                                //todo look at not found files and see whats up
                            }*/
                        }

                        //screw it, just fake a entry :|
                        //in fact.. ill do the final search thingy when they actually export!
                        RAFFileListEntry entry = new RAFFileListEntry(null, "DATA/Particles" + 
                            matchedFile.Replace(".tga",".dds").Replace(".TGA",".DDS"), 0,0,0);
                        troybinKVP.Value.Add(entry);
                        cleanString = matchedFile + "\r\n" + cleanString;
                        //searchRaf.GetDataFileContentStream().Close();
                        
                    }
                    if (textBox1.Text == "") textBox1.Text = cleanString;

                    

                }
            }
            reportProgress(100);


            return particleDef;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            particleReaderWorker.RunWorkerAsync("C:\\Riot Games\\League of Legends\\RADS\\projects\\lol_game_client\\filearchives\\");
        }
        public void startGettingParticleStructure(skinInstaller source, string rafDirPath)
        {
            mySkinInstaller = source;
            particleReaderWorker.RunWorkerAsync(rafDirPath);       
        }

        private void particleReaderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = getParticleStructure((string)e.Argument);
        }

        private void particleReaderWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (mySkinInstaller != null)
            {
                mySkinInstaller.recieveParticleProgress(e.ProgressPercentage);
            }
            if (this.Visible)
            {
                progressBar1.Value = e.ProgressPercentage;
                progressBar1.Refresh();
            }
        }

        private void particleReaderWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PStruct particleDef =(PStruct)e.Result;
            //int troybinCount = 0;
            if (this.Visible)
            {
                textBox.AppendText(progs + "\r\n\r\n");
                this.button1.Enabled = true;
                // Display particleDef for debugging purposes
                foreach (KeyValuePair<String, Dictionary<String, Dictionary<RAFFileListEntry, List<RAFFileListEntry>>>> championKVP in particleDef)
                {
                    textBox.AppendText(championKVP.Key + "->>\n");
                    textBox.AppendText("\tSpell->>\n");
                    foreach (KeyValuePair<String, Dictionary<RAFFileListEntry, List<RAFFileListEntry>>> spellKVP in particleDef[championKVP.Key])
                    {
                        if (spellKVP.Key != "troybins")
                        {
                            textBox.AppendText("\t\t" + spellKVP.Key + "\n");
                        }
                    }
                    textBox.AppendText("\tTroybins->>\n");
                    foreach (KeyValuePair<RAFFileListEntry, List<RAFFileListEntry>> troybinKVP in particleDef[championKVP.Key]["troybins"])
                    {
                        textBox.AppendText("\t\t" + troybinKVP.Key.FileName + "\n");
                        //troybinCount++;
                        foreach (RAFFileListEntry fileEntry in troybinKVP.Value)
                        {
                            textBox.AppendText("\t\t\t" + fileEntry.FileName + "\n");
                        }
                    }
                }
            }
            if (mySkinInstaller != null)
            {
                mySkinInstaller.recieveParticleInformation(particleDef);
            }
            //int temp = troybinCount + leftoverTroybins.Count;
            textBox.AppendText("Troybins identified: ");// + troybinCount);
        }
    }
}