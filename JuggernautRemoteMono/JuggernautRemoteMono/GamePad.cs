using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace JuggernautControlAndroid
{
    public class GamePad
    {
        float _vertical, _horizontal;
        public float Vertical { get { return _vertical; } }
        public float Horizontal { get { return _horizontal; } }

        Rectangle verticalBox, horizontalBox;
        Texture2D dummyTexture;

        public GamePad(Rectangle vbox, Rectangle hbox, Texture2D dummy)
        {
            verticalBox = vbox;
            horizontalBox = hbox;
            dummyTexture = dummy;
        }

        public void refresh()
        {
            TouchCollection touchCollection = TouchPanel.GetState();
            _vertical = 0;
            _horizontal = 0;
            foreach (var item in touchCollection)
            {
                if (verticalBox.Contains(item.Position))
                {
                    _vertical = (verticalBox.Y + verticalBox.Height - item.Position.Y) / verticalBox.Height * 2 - 1;
                }
                if (horizontalBox.Contains(item.Position))
                {
                    _horizontal = (horizontalBox.X + horizontalBox.Width - item.Position.X) / horizontalBox.Width * 2 - 1;
                }
            }
        }

        public bool left
        {
            get { return _horizontal < -0.3; } 
        }

        public bool right
        {
            get { return _horizontal > 0.3; }
        }

        public bool up
        {
            get { return _vertical < -0.3; }
        }

        public bool down    
        {
            get { return _vertical > 0.3; }
        }


        Random r = new Random();

        public void draw(SpriteBatch s)
        {
            s.Draw(dummyTexture, verticalBox, new Color(r.Next(128) + 128, r.Next(128) + 128, r.Next(128) + 128));
            s.Draw(dummyTexture, horizontalBox, new Color(r.Next(128) + 128, r.Next(128) + 128, r.Next(128) + 128));
        }

    }
}