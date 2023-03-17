using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus
{
    internal class Camera
    {
        private static Camera instance = null;

        public static Camera Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Camera();
                }
                return instance;
            }

        }

        // -------------------------------------------------
        // Fields
        // -------------------------------------------------

        private int x;
        private int y;
        private int cameraScroll;
        private Vector2 offSet;
        private bool cameraLock;


        // -------------------------------------------------
        // Properties
        // -------------------------------------------------

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int CameraScroll
        {
            get { return y; }
            set { y = value; }
        }

        public Vector2 OffSet
        {
            get { return offSet; }
        }

        // -------------------------------------------------
        // Contructors
        // -------------------------------------------------

        public Camera()
        {
            x = 0;
            y = 0;
            cameraScroll = 1;
            offSet = new Vector2(3,0);
        }

        public Camera(int cameraScroll)
        {
            x = 0;
            y = 0;
            this.cameraScroll = cameraScroll;
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                cameraLock = true;
            }
        }

        public void DebugMode(Player player)
        {
            if (cameraLock == true)
            {
                
            }
        }
    }
}
