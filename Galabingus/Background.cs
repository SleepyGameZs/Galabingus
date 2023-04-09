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
    internal class Background : UIElement
    {
        #region Event

        public event EventDelegate OnMenuBack;

        #endregion

        #region Fields

        #endregion

        #region Constructor
        public Background(Texture2D texture, Vector2 position, GameState gs)
            : base(texture, position, 5) { }
        #endregion

        #region Methods

        public override void Update()
        {
            if(UIManager.Instance.SingleKeyPress(Keys.Back))
            {
                OnMenuBack(this);
            }
        }

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
