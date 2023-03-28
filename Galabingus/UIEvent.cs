using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus
{
    #region Enums

    /// <summary>
    /// the list of all event types which can be triggered
    /// </summary>
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

        //this list of fields encompasses all fields which could be
        //called from an event, but not all need to be (will be) 
        //set, only those which are need for the UIObjects events

        Menu menu;
        GameState returnState;

        #endregion

        #region Constructor

        /// <summary>
        /// default constructor for things with no events
        /// </summary>
        public UIEvent() { }

        /// <summary>
        /// instatiates the UIEvent class for a basic menu showing
        /// </summary>
        /// <param name="menu">the menu to be shown</param>
        public UIEvent(Menu menu)
        {
            this.menu = menu;
        }

        /// <summary>
        /// instantiates the UIEvent class for a change of GameState
        /// </summary>
        /// <param name="returnState">the state to be changed to</param>
        public UIEvent(GameState returnState)
        {
            this.returnState = returnState;
        }

        #endregion

        #region Methods

        /// <summary>
        /// the method which holds all events which can be called
        /// </summary>
        /// <param name="element">the element which is calling the event</param>
        /// <param name="gs">the gameState they are in</param>
        /// <param name="type">the type of event they are calling</param>
        public void Event(UIObject element, GameState gs, EventType type)
        {
            switch (type)
            {

                case EventType.NoEvent:
                    //no event occurs in this case
                    break;
                case EventType.StartGame:
                    //this event changes the gameState
                    UIManager.Instance.ChangeState(returnState);
                    break;
            }
        }

        #endregion
    }
}
