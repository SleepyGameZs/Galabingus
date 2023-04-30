using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Galabingus
{
    internal class Button : UIElement
    {
        #region Events

        public event EventDelegate OnClick;
        public event EventDelegate OnHover;

        #endregion

        #region Fields

        //a second texture for hover
        private Texture2D baseTexture;
        private Texture2D hoverTexture;

        //list of UIElements for new menu
        private List<UIElement> displayMenu;

        #endregion

        #region Properties

        /// <summary>
        /// gets and sets the hover texture of the button
        /// </summary>
        public Texture2D HoverTexture
        {
            get { return hoverTexture; }
            set { hoverTexture = value; }
        }

        /// <summary>
        /// gets and sets the menu to be displayed when the button is clicked
        /// </summary>
        public List<UIElement> DisplayMenu
        {
            get { return displayMenu; }
            set { displayMenu = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// instantiates a basic button
        /// </summary>
        /// <param name="texture">its texure</param>
        /// <param name="position">its position rectangle</param>
        public Button
            (Texture2D texture, Vector2 position, float scale)
            : base(texture, position, scale)
        {
            baseTexture = texture;
        }

        #endregion

        #region Methods

        /// <summary>
        /// updates the button
        /// </summary>
        public override void Update()
        {
            //determine if a button has been clicked
            if (uiPosition.Y == UIManager.Instance.ButtonSelection && (UIManager.Instance.SingleKeyPress(Keys.Enter) || UIManager.Instance.SingleKeyPress(Keys.Space)))
            {
                //plays the sound effect
                AudioManager.Instance.CallSound("Menu Confirm");

                //runs its on click event
                if (OnClick != null)
                    OnClick(this);
            }
            else
            {
                //runs its on hover event
                if(OnHover != null) 
                    OnHover(this);
            }

            //if the button isn't selected return it to its normal texture
            if (uiTexture != baseTexture && UIPosition.Y != UIManager.Instance.ButtonSelection)
                uiTexture = baseTexture;
        }

        /// <summary>
        /// draws the button
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(
                uiTexture,
                uiPosition,
                clearColor
            );
        }

        #endregion
    }
}
