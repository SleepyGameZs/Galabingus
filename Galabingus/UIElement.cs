using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus
{
    #region Delegates

    public delegate void EventDelegate(object sender);

    #endregion

    abstract class UIElement
    {
        #region Fields

        //most objects
        protected Texture2D uiTexture; //its texture
        protected Color clearColor; //its tint
        protected Rectangle uiPosition; //its position rect

        //text
        protected SpriteFont uiFont; //the font type
        protected string uiText; //the text
        protected Vector2 uiTextPosition; //the position
         
        #endregion

        #region Properties

        public Color ClearColor { 

            get { return clearColor; } 
            set { clearColor = value; } 

        }

        public Texture2D UITexture { 
            
            get { return uiTexture; } 
            set { uiTexture = value; } 
        
        }

        public Vector2 UIPosition
        {
            get { return new Vector2(uiPosition.X, uiPosition.Y); }
        }

        public string UIText { 

            get { return uiText; } 
            set { uiText = value; } 
        
        }

        public Vector2 UITextPosition { 

            get { return uiTextPosition; } 
            set { uiTextPosition = value; } 

        }

        #endregion

        #region Constructor

        /// <summary>
        /// creates a basic UIObject
        /// </summary>
        /// <param name="uiTexture">the objects texture</param>
        /// <param name="position">the objects position</param>
        /// <param name="scale">the size it needs to be scaled to</param>
        public UIElement(Texture2D uiTexture, Vector2 position, float scale)
        {
            //sets the texture to this classes texture
            this.uiTexture = uiTexture;

            //sets the length and width of the object
            int length = (int)(uiTexture.Width / scale);
            int width = (int)(uiTexture.Height / scale);

            //creates its position rectangle
            uiPosition =
                new Rectangle(
                    ((int)position.X) - (length / 2),
                    ((int)position.Y) - (width / 2),
                    length,
                    width
                );

            clearColor = Color.White;
        }

        public UIElement(SpriteFont uiFont, string uiText, Vector2 position)
        {
            this.uiFont = uiFont;
            this.uiText = uiText;
            this.uiTextPosition = position;
            clearColor = Color.Black;
        }

        #endregion

        #region Methods

        public abstract void Update();

        public abstract void Draw(SpriteBatch sb);
        #endregion
    }
}
