using System;
using System.IO;
using System.Text.Json;

namespace YetAnotherSnake
{
    public class SaveFile
    {
        private float _volume = 100; 
        private int _score = 0;
        private bool _isFullScreen=false, _isVignette=true, _isBloom=true;

        public float Volume
        {
            get => _volume;
            set => _volume = value;
        }

        public int Score
        {
            get => _score;
            set => _score = value;
        }

        public bool IsFullScreen
        {
            get => _isFullScreen;
            set => _isFullScreen = value;
        }

        public bool IsVignette
        {
            get => _isVignette;
            set => _isVignette = value;
        }

        public bool IsBloom
        {
            get => _isBloom;
            set => _isBloom = value;
        }
    }

    public class SaveSystem
    {
        public SaveFile SaveFile;

        public SaveSystem()
        {
            if (!IsHasSaveFile())
                File.Create(Content.Save).Dispose();
            
            SaveFile = new SaveFile();
            var sr = new StreamReader(Content.Save);
            var json = sr.ReadToEnd();
            sr.Dispose();
            
            if (!string.IsNullOrEmpty(json))
                SaveFile = JsonSerializer.Deserialize<SaveFile>(json);
        }

        public void SaveChanges()
        {
            var json = JsonSerializer.Serialize(SaveFile);
            var sr = new StreamWriter(Content.Save);
            sr.Write(json);
            sr.Flush();
            sr.Dispose();
        }

        private bool IsHasSaveFile() => File.Exists(Content.Save);
    }
    
}