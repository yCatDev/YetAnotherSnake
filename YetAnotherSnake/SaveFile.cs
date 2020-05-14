using System;
using System.IO;
using System.Text.Json;

namespace YetAnotherSnake
{
    
    /// <summary>
    /// Save file class
    /// </summary>
    public class SaveFile
    {
        /// <summary>
        /// Game music volume
        /// </summary>
        public float Volume { get; set; } = 100;

        /// <summary>
        /// High score
        /// </summary>
        public int Score { get; set; } = 0;

        /// <summary>
        /// Fullscreen mode
        /// </summary>
        public bool IsFullScreen { get; set; } = false;

        /// <summary>
        /// Enable vignette post-processing 
        /// </summary>
        public bool IsVignette { get; set; } = true;

        /// <summary>
        /// Enable bloom post-processing 
        /// </summary>
        public bool IsBloom { get; set; } = true;
    }

    /// <summary>
    /// Save system class that works with save file
    /// </summary>
    public class SaveSystem
    {
        /// <summary>
        /// Working file
        /// </summary>
        public readonly SaveFile SaveFile;

        public SaveSystem()
        {
            //Checking for the first game start and creating save file 
            if (!IsHasSaveFile())
                File.Create(Content.Save).Dispose();
            
            //Reading json file with settings
            SaveFile = new SaveFile();
            var sr = new StreamReader(Content.Save);
            var json = sr.ReadToEnd();
            sr.Dispose();
            
            //If first start create default save file
            if (!string.IsNullOrEmpty(json))
                SaveFile = JsonSerializer.Deserialize<SaveFile>(json);
        }

        /// <summary>
        /// Saving changes to json file
        /// </summary>
        public void SaveChanges()
        {
            var json = JsonSerializer.Serialize(SaveFile);
            var sr = new StreamWriter(Content.Save);
            sr.Write(json);
            sr.Flush();
            sr.Dispose();
        }

        /// <summary>
        /// Check if save file exists
        /// </summary>
        /// <returns>true if is exists</returns>
        private static bool IsHasSaveFile() => File.Exists(Content.Save);
    }
    
}