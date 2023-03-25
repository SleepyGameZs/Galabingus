using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus
{
    internal class Button : UIObject
    {
        #region Fields

        private Texture2D buttonTexture;
        private Rectangle buttonPosition;
        private Color clearColor;

        #endregion

        #region Properties
        #endregion

        #region Constructor

        public Button
            (string filename, ContentManager cm, 
            int screenWidth, int screenHeight, int scale)
        {
            buttonTexture = cm.Load<Texture2D>(filename);
            buttonPosition =
                new Rectangle(
                    (screenWidth - (buttonTexture.Width / scale)) / 2,
                    (screenHeight - (buttonTexture.Height / scale)) / 2,
                    (buttonTexture.Width / scale),
                    (buttonTexture.Height / scale)
                );
        }

        #endregion

        #region Methods

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        //hover

        //onClick

        #endregion
    }
}
