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
        private float initalCameraScroll;
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

        public float InitalCameraScroll
        {
            get { return initalCameraScroll; }
            set { initalCameraScroll = value; }
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
            initalCameraScroll = 3f;
            offSet = new Vector2(initalCameraScroll,0);
        }

        public Camera(int cameraScroll)
        {
            x = 0;
            y = 0;
            this.initalCameraScroll = cameraScroll;
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

        public void Start()
        {
            cameraLock = false;
            offSet.X = initalCameraScroll;
        }

        public void Stop()
        {
            cameraLock = false;
            offSet.X = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                cameraLock = true;
            }
            if (cameraLock == true)
            {
                Camera.Instance.offSet.X = Player.PlayerInstance.Velocity == Vector2.Zero ? Vector2.Zero.X*5 :
                    Vector2.Normalize(Player.PlayerInstance.Velocity).X*5;
            }
        }
    }
}
