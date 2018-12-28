using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyncfusionHandy
{
    public class Utils
    {
        /// <summary>
        /// Gets the url and keywords stored in file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>A dictionary of url and keyword pair</returns>
        public static Dictionary<string, string> GetUrlByKeyword(string fileName)
        {   
            string[] lines = File.ReadAllLines(fileName);
            Dictionary<string, string> urlKeywords = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                string[] text = line.Split(',');
                urlKeywords.Add(text[0], text[1].Replace(";", ""));
            }
            return urlKeywords;
        }

        /// <summary>
        /// Gets the list of applications to start
        /// </summary>
        /// <param name="fileName">Name of the file in which list is stored</param>
        /// <returns>A list of application names to start</returns>
        public static List<string> GetStartUpApplications(string fileName)
        {
            string content = File.ReadAllText(fileName);
            content = content.Replace(";", "");
            return content.Split(',').ToList();
        }

        /// <summary>
        /// Save url and keyword pair
        /// </summary>
        /// <param name="fileName">Name of the file to store the data</param>
        /// <param name="keyword">Keyword</param>
        /// <param name="url">Url</param>
        public static void SaveUrlByKeyword(string fileName, string keyword, string url)
        {
            File.AppendAllText(fileName, "\n" + keyword + "," + url + ";");
        }

        /// <summary>
        /// Save data in specified file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="contents">Content to be saved</param>
        public static void SaveData(string fileName, string[] contents)
        {
            if(contents.Length > 0){
                string data = "";
                int length = contents.Length;
                for(var i=0; i<length; i++)
                {
                    data += contents[i] + (i == length - 1 ? ";" : ",");
                }
                File.AppendAllText(fileName, data);
            }
        }

        /// <summary>
        /// Save data in specified file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="contents">Content to be saved</param>
        public static void SaveData(string fileName, List<string> contents)
        {
            SaveData(fileName, contents.ToArray());
        }
        public static void takeScreen()
        {

            Rectangle captureRectangle = Screen.AllScreens[0].Bounds;

            Bitmap captureBitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height, PixelFormat.Format32bppArgb);
            
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);
            
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            
            captureBitmap.Save(@"D:\Synchandysnap-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".jpg", ImageFormat.Jpeg);
        }
        public static void deleteTemp()
        {
            string tempLocation = System.IO.Path.GetTempPath();
            foreach (string filePath in Directory.GetFiles(tempLocation, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    FileInfo currentFile = new FileInfo(filePath);
                    currentFile.Delete();
                }
                catch
                {
                }
            }
            MessageBox.Show("Temp files deleted succesfully!");
        }
    }
}
