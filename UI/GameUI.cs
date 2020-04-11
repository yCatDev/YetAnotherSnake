using System.Collections.Generic;
using Nez;
using Nez.UI;

namespace YetAnotherSnake
{
    /// <summary>
    /// Class that helps to create table-based UI
    /// </summary>
    public class GameUI: SceneComponent
    {

        private readonly UICanvas _sceneUI;
        private Dictionary<string, Table> _tables;
        
        public GameUI()
        {
            _sceneUI = Scene.CreateEntity("UI").AddComponent<UICanvas>();
            CreateTable("root");
        }

        public Table CreateTable(string name, string parentName = "")
        {
            if (string.IsNullOrEmpty(parentName))
                _tables.Add(name, _sceneUI.Stage.AddElement( new Table() ));
        }
        
        
    }
}