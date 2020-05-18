using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Systems;
using Nez.UI;
using YetAnotherSnake.Scenes;

namespace YetAnotherSnake
{
    /// <summary>
    /// Class that helps to create table-based UI elements
    /// </summary>
    public class GameUIHelper
    {
        
        /// <summary>
        /// Skin settings for ui elements
        /// </summary>
        private GameSkin _skin;
        
        /// <summary>
        /// Init skin for ui
        /// </summary>
        /// <param name="content">Content manager that able to load resources (Scene.Content)</param>
        public GameUIHelper(NezContentManager content)
        {
            _skin = new GameSkin(content);
        }

        /// <summary>
        /// Create button with text on table
        /// </summary>
        /// <param name="t">TargetTable</param>
        /// <param name="label">Text</param>
        /// <param name="onClick">On button click event handler</param>
        /// <returns>Element</returns>
        public TextButton CreateBtn(Table t, string label, Action<TextButton> onClick)
        {
            var button = new TextButton(label, _skin.Skin.Get<TextButtonStyle>("regular-button"));
            button.GetLabel().SetStyle(_skin.Skin.Get<LabelStyle>("label"));
            button.OnClicked += btn =>
            {
                btn.ResetMouseHover();
                onClick(button);
               
                
            };
            t.Add( button ).SetMinWidth( 450 ).SetMinHeight( 100 );
            
            return button;
        }
        
        /// <summary>
        /// Create input field on table
        /// </summary>
        /// <param name="t">TargetTable</param>
        /// <param name="placeholder">Placeholder text</param>
        /// <param name="onInput">On input event handler</param>
        /// <returns>Element</returns>
        public TextField CreateInputField(Table t, string placeholder, Action<TextField,string> onInput = null)
        {
            var input = new TextField(placeholder, _skin.Skin.Get<TextFieldStyle>("inputfield"));
            //input.SetStyle(_skin.Skin.Get<TextFieldStyle>("inputfield"));
            input.OnTextChanged += (field, s) =>
            {
                onInput?.Invoke(field, s);
            };
            t.Add( input ).SetMinWidth( 450 ).SetMinHeight( 100 );
            input.SetAlignment(Align.Center);
            return input;
        }

        /// <summary>
        /// Create checkbox on table
        /// </summary>
        /// <param name="t">Target table</param>
        /// <param name="label">Text</param>
        /// <param name="defaultState">Default state of check</param>
        /// <param name="onChanged">On state changed handler</param>
        /// <returns>Element</returns>
        public CheckBox CreateCheckBox(Table t, string label, bool defaultState, Action<bool> onChanged = null)
        {
            var checkBox = new CheckBox(label, Skin.CreateDefaultSkin());
            checkBox.GetLabel().GetStyle().Font = _skin.Skin.Get<LabelStyle>("label").Font;
            checkBox.GetLabel().SetFontScale(0.75f);
            checkBox.IsChecked = defaultState;
            checkBox.OnChanged += onChanged;
            onChanged?.Invoke(defaultState);
            t.Add(checkBox);

            return checkBox;
        }

        /// <summary>
        /// Create text label with title style
        /// </summary>
        /// <param name="t">Target table</param>
        /// <param name="text">Text</param>
        /// <returns>Element</returns>
        public Label CreateTitleLabel(Table t, string text)
        {
            var label = new Label(text, _skin.Skin.Get<LabelStyle>("title-label"));
            t.Add(label);
            
            return label;
        }

        /// <summary>
        /// Create label text with regular style
        /// </summary>
        /// <param name="t">Target table</param>
        /// <param name="text">Text</param>
        /// <returns>Element</returns>
        public Label CreateRegularLabel(Table t, string text, float fontScale = 1)
        {
            var label = new Label(text,
                _skin.Skin.Get<LabelStyle>("label"));
            label.SetFontScale(fontScale);
            t.Add(label);
            
            return label;
        }

        /// <summary>
        /// Create indent in table
        /// </summary>
        /// <param name="t">Target table</param>
        /// <param name="height">Height</param>
        /// <returns>Element</returns>
        public Element CreateVerticalIndent(Table t, float height)
        {
            var el = new Container();
            el.SetHeight(height);
            t.Add(el);

            return el;
        }

        /// <summary>
        /// Create slider
        /// </summary>
        /// <param name="t">Target table</param>
        /// <param name="onChanged">On slider value changes handler</param>
        /// <returns></returns>
        public Slider CreateSlider(Table t, Action<float> onChanged)
        {
            var slider = new Slider(0, 1, 0.05f, false,_skin.Skin.Get<SliderStyle>());
            slider.SetValue(MyGame.GameInstance.SaveSystem.SaveFile.Volume);
            slider.OnChanged += onChanged;
            t.Add(slider);

            return slider;
        }
        
        
    }

  
    
}