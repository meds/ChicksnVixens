using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Jabber.Media;
using System.IO;
using System.IO.IsolatedStorage;
using System.Globalization;

namespace Jabber.Util
{
    static public class FileSaverLoader
    {
        static public void SaveToFile(List<string> lines, string filename)
        {
#if IPHONE
			string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string filePath = Path.Combine(path, filename);
			
            File.WriteAllLines(filePath, lines.ToArray());
#endif
#if WINDOWS_PHONE
            using (IsolatedStorageFile isolatedStorageFile
                = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // If user choose to save, create a new file
                using (IsolatedStorageFileStream fileStream
                    = isolatedStorageFile.CreateFile(filename))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        for (int i = 0; i < lines.Count; i++)
                        {
                            streamWriter.WriteLine(lines[i]);
                        }

                        streamWriter.Close();
                    }

                    fileStream.Close();
                }
            }
#endif
#if WINDOWS
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, filename);

            File.WriteAllLines(filePath, lines.ToArray());
#endif
        }

        static public List<string> LoadFromFile(string filename)
        {
            List<string> data = new List<string>();
#if IPHONE
			string path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string filePath = Path.Combine(path, filename);
			
			if(!File.Exists(filePath))
			{
				return null;
			}			
			
			IEnumerable<string> hmm = File.ReadAllLines(filePath);
			
			foreach(string s in hmm)
				data.Add(s);
#endif
#if WINDOWS_PHONE
            // Load from Isolated Storage file
            using (IsolatedStorageFile isolatedStorageFile
                = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isolatedStorageFile.FileExists(filename))
                {
                    //If user choose to save, create a new file
                    using (IsolatedStorageFileStream fileStream
                        = isolatedStorageFile.OpenFile(filename, FileMode.Open))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            while (!streamReader.EndOfStream)
                            {

                                data.Add(streamReader.ReadLine());
                            }
                            streamReader.Close();
                        }
                    }
                }
            }
#endif
#if WINDOWS
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, filename);

            if (!File.Exists(filePath))
            {
                return null;
            }

            IEnumerable<string> hmm = File.ReadAllLines(filePath);

            foreach (string s in hmm)
                data.Add(s);
#endif

            return data;
        }

    }
}