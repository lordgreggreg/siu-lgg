﻿/*
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace SkinInstaller
{
    public struct skinInfo
    {
        public string name, author;

        public skinInfo(string n, string a)
        {
            name = n;
            author = a;
        }
    }
    public class fileLocReturn
    {
        public string folderName { get; set; }
        public string fileName { get; set; }
        public bool valid { get; set; }
        public List<string> moreOptions { get; set; }
        public fileLocReturn(string folder, string file, bool ok, List<string> extraOptions)
        {
            folderName = folder;
            fileName = file;
            valid = ok;
            moreOptions = extraOptions;
        }
        public fileLocReturn(string folder, string file)
        {
            folderName = folder;
            fileName = file;
            valid = true;
            moreOptions = null;
        }
        public fileLocReturn(List<string> extraOptions)
        {
            folderName = fileName = "";
            valid = false;
            moreOptions = extraOptions;
        }
        public static fileLocReturn retError = new fileLocReturn(null,null,false,null);
    }
    public class prettyDate : IComparable
    {
        private DateTime date;
        private bool valid = false;
        public prettyDate(DateTime dt)
        {
            setDate(dt);
        }
        public prettyDate(string exact)
        {
            setDate(exact);
        }
        public void setDate(DateTime dt)
        {
            this.date = dt;
            valid = true;
        }
        public void setDate(string exact)
        {
            valid = DateTime.TryParseExact(exact, "M/d/yyyy HH:mm:ss.f tt", CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out this.date);
            if (!valid)
            {
                //second try?
                valid = DateTime.TryParseExact(exact, "M.d.yyyy HH:mm:ss.f", CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out this.date);

            }
        }
        public override string ToString()
        {
            return valid ? date.ToString("M/d/yy") : "No";
        }
        public DateTime getDate()
        {
            return valid?date:new DateTime(0);
        }
        public string getStringDate()
        {
            return valid ? date.ToString("M/d/yyyy HH:mm:ss.f tt",CultureInfo.InvariantCulture) : "-";
        }
        public int CompareTo(object o)
        {
            prettyDate temp = (prettyDate)o;
            return temp.getDate().CompareTo(this.date);
        }
    }
    public class commonOps
    {
        static public int getDXTVersion(string file)
        {
            int toReturn = 0;
            try
            {
                StreamReader sr = new StreamReader(file);
                BinaryReader br = new BinaryReader(sr.BaseStream);
                System.Text.Encoding enc = System.Text.Encoding.ASCII;

                br.BaseStream.Position = 10;
                byte[] tvByteArray = new byte[100];
                tvByteArray = br.ReadBytes(100);

                string ins = enc.GetString(tvByteArray);//.Replace("\0", "").Trim();
                int st = ins.ToLower().IndexOf("dxt");
                if (st != -1)
                {
                    //s = ins.Substring(st, 4);
                    toReturn = int.Parse(ins.Substring(st + 3, 1));
                }
            }
            catch
            {
            }
            return toReturn;
        }
        public static Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {

            Bitmap b = new Bitmap(size.Width, size.Height);
            try
            {
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
            }
            catch { }

            return b;
        }
        static public Size getFileDimensions(string file)
        {
            int imageID;
            int iW = -1;
            int iH = -1;
            Size s = new Size(-1, -1);
            Tao.DevIl.Il.ilGenImages(1, out imageID);
            Tao.DevIl.Il.ilBindImage(imageID);
            try
            {
                if (false == Tao.DevIl.Il.ilLoadImage(file))
                {
                }
                else
                {
                    iW = Tao.DevIl.Il.ilGetInteger(Tao.DevIl.Il.IL_IMAGE_WIDTH);
                    iH = Tao.DevIl.Il.ilGetInteger(Tao.DevIl.Il.IL_IMAGE_HEIGHT);
                    Tao.DevIl.Il.ilDeleteImages(1, ref imageID);
                }
            }
            catch  {     }
            s.Width = iW;
            s.Height = iH;
            return s;
        }
        static public Dictionary<string,int> readDDSInfoNvidia(string file)
        {
            Dictionary<string, int> result = new Dictionary<string, int>()
                {
                    {"height",0},
                    {"width",0},
                    {"depth",0},
                    {"linear size",0},
                    {"mipmap count",0},
                    {"dxtv",0},
                    {"bit count",0}                    
                };
            Process process = new Process();
            process.StartInfo.FileName = "nvddsinfo.exe";
            process.StartInfo.Arguments = " \"" + file + "\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Application.StartupPath;
            process.StartInfo.RedirectStandardOutput = true;   
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            /*
            Flags: 0x000A1007
	            DDSD_CAPS
	            DDSD_PIXELFORMAT
	            DDSD_WIDTH
	            DDSD_HEIGHT
	            DDSD_LINEARSIZE
	            DDSD_MIPMAPCOUNT
            Height: 512
            Width: 512
            Depth: 0
            Linear size: 131072
            Mipmap count: 10
            Pixel Format:
	            Flags: 0x00000004
		            DDPF_FOURCC
	            FourCC: 'DXT1'
	            Bit count: 0
	            Red mask: 0x00000000
	            Green mask: 0x00000000
	            Blue mask: 0x00000000
	            Alpha mask: 0x00000000
            Caps:
	            Caps 1: 0x00401008
		            DDSCAPS_COMPLEX
		            DDSCAPS_TEXTURE
		            DDSCAPS_MIPMAP
	            Caps 2: 0x00000000
	            Caps 3: 0x00000000
	            Caps 4: 0x00000000
            */
            string[] lines = output.Split(new string[4] { "\r\n", "\r", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string line in lines)
            {
                if (line.Contains(": "))
                {
                    string[] parts = line.Split(new string[1] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        string label = parts[0];
                        string value = parts[1].Trim().ToLower();
                        switch (label.ToLower().Trim())
                        {
                            case "height":
                                result["height"]=int.Parse(value);
                                break;
                            case "width":
                                result["width"]=int.Parse(value);
                                break;
                            case "depth":
                                result["depth"]=int.Parse(value);
                                break;
                            case "linear size":
                                result["linear size"]=int.Parse(value);
                                break;
                            case "fourcc":
                                int dxtv = 0;
                                string dxtstring = value.Replace("'","").Replace("dxt","").Trim();
                                bool parsed = int.TryParse(dxtstring, out dxtv);
                                if (!parsed) dxtv = 0;
                                result["dxtv"]=dxtv;
                                break;
                            case "bit count":
                                result["bit count"]=int.Parse(value);
                                break;
                            case "mipmap count":
                                result["mipmap count"]=int.Parse(value);
                                break;
                            
                            default: break;

                        }
                    }
                }
            }


            return result;

        }
    }

    public class HSLColor
     {
         // Private data members below are on scale 0-1
         // They are scaled for use externally based on scale
         private double hue = 1.0;
         private double saturation = 1.0;
         private double luminosity = 1.0;
  
         private const double scale = 1.0;
  
         public double Hue
         {
             get { return hue * scale; }
             set { hue = CheckRange(value / scale); }
         }
         public double Saturation
         {
             get { return saturation * scale; }
             set { saturation = CheckRange(value / scale); }
         }
         public double Luminosity
         {
             get { return luminosity * scale; }
             set { luminosity = CheckRange(value / scale); }
         }
  
         private double CheckRange(double value)
         {
             if (value < 0.0)
                 value = 0.0;
             else if (value > 1.0)
                 value = 1.0;
             return value;
         }
  
         public override string ToString()
         {
             return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}",   Hue, Saturation, Luminosity);
         }
  
         public string ToRGBString()
         {
             Color color = (Color)this;
             return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
         }
  
         #region Casts to/from System.Drawing.Color
         public static implicit operator Color(HSLColor hslColor)
         {
             double r = 0, g = 0, b = 0;
             if (hslColor.luminosity != 0)
             {
                 if (hslColor.saturation == 0)
                     r = g = b = hslColor.luminosity;
                 else
                 {
                     double temp2 = GetTemp2(hslColor);
                     double temp1 = 2.0 * hslColor.luminosity - temp2;
  
                     r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
                     g = GetColorComponent(temp1, temp2, hslColor.hue);
                     b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
                 }
             }
             return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
         }
  
         private static double GetColorComponent(double temp1, double temp2, double temp3)
         {
             temp3 = MoveIntoRange(temp3);
             if (temp3 < 1.0 / 6.0)
                 return temp1 + (temp2 - temp1) * 6.0 * temp3;
             else if (temp3 < 0.5)
                 return temp2;
             else if (temp3 < 2.0 / 3.0)
                 return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
             else
                 return temp1;
         }
         private static double MoveIntoRange(double temp3)
         {
             if (temp3 < 0.0)
                 temp3 += 1.0;
             else if (temp3 > 1.0)
                 temp3 -= 1.0;
             return temp3;
         }
         private static double GetTemp2(HSLColor hslColor)
         {
             double temp2;
             if (hslColor.luminosity < 0.5)  //<=??
                 temp2 = hslColor.luminosity * (1.0 + hslColor.saturation);
             else
                 temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity * hslColor.saturation);
             return temp2;
         }
  
         public static implicit operator HSLColor(Color color)
         {
             HSLColor hslColor = new HSLColor();
             hslColor.hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
             hslColor.luminosity = color.GetBrightness();
             hslColor.saturation = color.GetSaturation();
             return hslColor;
         }
         #endregion
  
         public void SetRGB(int red, int green, int blue)
         {
             HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
             this.hue = hslColor.hue;
            this.saturation = hslColor.saturation;
            this.luminosity = hslColor.luminosity;
        }
 
        public HSLColor() { }
        public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }
        public HSLColor(int red, int green, int blue)
        {
            SetRGB(red, green, blue);
        }
        public HSLColor(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }
 
 
    }

    public class skinOption
    {
        public string skinName;
        public bool skinSelected;
        public bool origonalSelected;
        public skinOptions parent;
    }
    public class skinOptions
    {
        public List<skinOption> options;
        public string skinName;
        public skinsOptions parent;
        public skinOption getOption(skinOption fakeOption)
        {
            return options.Find(thisItem => thisItem.skinName == fakeOption.skinName);
        }
    }
    public class skinsOptions : List<skinOptions>
    {
        public skinOptions getOptions(skinOptions fakeOptions)
        {
            return this.Find(thisItem => thisItem.skinName == fakeOptions.skinName);
        }
    }
    class riotVersions
    {
        public riotVersion LoLClientNewVersion = new riotVersion();
        public riotVersion LoLClientOldVersion = new riotVersion();
        public riotVersion LoLAirClientNewVersion = new riotVersion();
        public riotVersion LoLAirClientOldVersion = new riotVersion();
        public override string ToString()
        {
            return "LoL Client Newest Version:\t" + LoLClientNewVersion.ToString() +
                "\r\nLol Air Client Newest Version:\t" + LoLAirClientNewVersion.ToString() +
                "\r\nLoL Client Oldest Version:\t" + LoLClientOldVersion.ToString() +
                "\r\nLol Air Client Oldest Version:\t" + LoLAirClientOldVersion.ToString();
        }
        public string toCSVString()
        {
            return LoLClientNewVersion.ToString() +
                "," + LoLAirClientNewVersion.ToString() +
                "," + LoLClientOldVersion.ToString() +
                "," + LoLAirClientOldVersion.ToString();
        }
        public void fromCSVString(string input)
        {
            string[] parts = input.Split(',');
            if (parts.Length < 4) return;
            LoLClientNewVersion.fromString(parts[0]);
            LoLAirClientNewVersion.fromString(parts[1]);
            LoLClientOldVersion.fromString(parts[2]);
            LoLAirClientOldVersion.fromString(parts[3]);
        }
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            riotVersions rvs = obj as riotVersions;
            if ((System.Object)rvs == null)
            {
                return false;
            }

            return rvs.LoLAirClientNewVersion.Equals(LoLAirClientNewVersion) &&
                rvs.LoLClientNewVersion.Equals(LoLClientNewVersion) &&
                rvs.LoLAirClientOldVersion.Equals(LoLAirClientOldVersion) &&
                rvs.LoLClientOldVersion.Equals(LoLClientOldVersion);

        }
        public override int GetHashCode()
        {
            return LoLAirClientNewVersion.GetHashCode();
        }
        public void setFromVersionFile(string versionFilePath)
        {
            BinaryReader r = new BinaryReader(new FileStream(versionFilePath, FileMode.Open));
            if (r.BaseStream.Length != 16)
            {
                /*Cliver.Message.Show("Ah Crap!",
                    SystemIcons.Information,
                    "Something is wrong with your version file! Maybe..\r\n" +
                    "Your length is " + r.BaseStream.Length.ToString() + " when it should be 16..\r\n\r\n" +
                    "PLEASE email lordgreggreg@gmail.com and attach this file\r\n" +
                    versionFilePath, 0, new string[1] { "Yes.. I Obey.. for the sake of humanity." });*/

                r.Close();

                this.LoLAirClientOldVersion.A = new Random().Next();//hack to make it still update;
                return;
            }
            this.LoLClientNewVersion.D = (int)r.ReadByte();
            this.LoLClientNewVersion.C = (int)r.ReadByte();
            this.LoLClientNewVersion.B = (int)r.ReadByte();
            this.LoLClientNewVersion.A = (int)r.ReadByte();

            this.LoLAirClientNewVersion.D = (int)r.ReadByte();
            this.LoLAirClientNewVersion.C = (int)r.ReadByte();
            this.LoLAirClientNewVersion.B = (int)r.ReadByte();
            this.LoLAirClientNewVersion.A = (int)r.ReadByte();

            this.LoLClientOldVersion.D = (int)r.ReadByte();
            this.LoLClientOldVersion.C = (int)r.ReadByte();
            this.LoLClientOldVersion.B = (int)r.ReadByte();
            this.LoLClientOldVersion.A = (int)r.ReadByte();

            this.LoLAirClientOldVersion.D = (int)r.ReadByte();
            this.LoLAirClientOldVersion.C = (int)r.ReadByte();
            this.LoLAirClientOldVersion.B = (int)r.ReadByte();
            this.LoLAirClientOldVersion.A = (int)r.ReadByte();
            r.Close();
        }
        public riotVersions() { }
        public riotVersions(string versionFilePath)
        {
            setFromVersionFile(versionFilePath);
        }
    }
    class riotVersion
    {
        public int A;
        public int B;
        public int C;
        public int D;
        public riotVersion() { A = B = C = D = 0; }
        public override string ToString()
        {
            return A.ToString() + "." + B.ToString() + "." + C.ToString() + "." + D.ToString();
        }
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            riotVersion rv = obj as riotVersion;
            if ((System.Object)rv == null)
            {
                return false;
            }

            return rv.A.Equals(A) && rv.B.Equals(B) && rv.C.Equals(C) && rv.D.Equals(D);
        }
        public override int GetHashCode()
        {
            return D.GetHashCode();
        }
        public void fromString(string input)
        {
            string[] parts = input.Split('.');
            if (parts.Length != 4) return;
            A = int.Parse(parts[0]);
            B = int.Parse(parts[1]);
            C = int.Parse(parts[2]);
            D = int.Parse(parts[3]);
        }
    }
}

