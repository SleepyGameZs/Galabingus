using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus
{
    #region Enums

    public enum EventType
    {
        NoEvent,
        UpMenu,
        DownMenu,
        StartGame
    }

    #endregion
    internal class UIEvent
    {
        #region Fields

        Menu menu;
        GameState returnState;

        #endregion

        #region Constructor

        public UIEvent(Menu menu)
        {
            this.menu = menu;
        }

        public UIEvent(GameState returnState)
        {
            this.returnState = returnState;
        }

        #endregion

        #region Methods

        public void Event(UIObject element, GameState gs, EventType type)
        {
            switch(type)
            {
                case EventType.NoEvent: 
                    break;
                case EventType.StartGame:
                    UIManager.Instance.ChangeState(returnState);
                    break;
            }
        }

        #endregion
    }
}
