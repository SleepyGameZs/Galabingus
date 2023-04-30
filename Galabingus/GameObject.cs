using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// Matthew Rodriguez
// 2023, 4, 30
// GameObject
// Provides essentails for all game objects
//
// Setup:
// Create a dynamic for the initalization process
// Set the dynamic to the result of the Initlizer method Initialize()
// Accessing GameObjects:
// Set the instance like so: GameObject.Instance.Content = GameObject.Instance.Content.YOURCONTENTNAME
// Note: YOURCONTENTNAME must follow file name specifications ie _strip(i) where (i) is the number of sprites
//
// Inharatence:
// There is no default constructor so use one of the parameter constructors,
// you MUST re-implement every used property using the content setter like so:
// GameObject.Instance.Content = GameObject.Instance.Content.YOURCONTENTNAME
// then use the property as normal

namespace Galabingus
{
    public delegate void OnDebug(SpriteBatch spriteBatch);

    /// <summary>
    ///  GameObject contains
    ///  the common funtionalites 
    ///  all game objects have
    /// </summary>
    internal class GameObject : DynamicObject, IConvertible
    {
        public event OnDebug Debug;
        private const byte animationConst = 0;
        private const byte colliderConst = 1;
        private const byte transformConst = 2;
        private const byte positionConst = 3;
        private const byte spritesConst = 4;
        private const byte scalesConst = 5;
        private const byte objectEnumsConst = 6;
        private const byte effectConst = 7;
        private static GameObject allGameObjects = null;
        private static List<Animation> animations = null;
        private static List<Collider> colliders = null;
        private static List<Rectangle> transforms = null;
        private static List<Vector2> positions = null;
        private static List<Texture2D> sprites = null;
        private static List<float> scales = null;
        private static List<string> objectEnums = null;
        private static List<Effect> effects = null;
        unsafe private static GameObjectTrie<Animation> animationsI;
        unsafe private static GameObjectTrie<Collider> collidersI;
        unsafe private static GameObjectTrie<Rectangle> transformsI;
        unsafe private static GameObjectTrie<Vector2> positionsI;
        unsafe private static GameObjectTrie<Texture2D> spritesI;
        unsafe private static GameObjectTrie<float> scalesI;
        unsafe private static GameObjectTrie<string> objectEnumsI;
        unsafe private static GameObjectTrie<Effect> effectI;
        private ushort index;
        private ushort instance;
        private ContentManager contentManager;           // Used to load in the content
        private GraphicsDevice graphicsDevice;           // Graphics Device
        private SpriteBatch spriteBatch;                 // Sprite Batch
        private static List<List<List<ushort>>> trie;
        private List<List<CollisionGroup>> collisionGroups;
        private System.Type typeOfObject;
        private Effect universalShader;
        private static float fade;
        public dynamic thisGameObject;
        private bool holdCollider;
        private ushort contentName;
        private CollisionGroup collisionGroup;
        private static List<Vector2> cameraStopPositions;
        private static float universalScaleX;
        private static float redShade;
        private static float timeShadeEffect;
        private static bool flipSine;
        private static bool bossEffectIsActive;
        private static float universalScaleY;
        private static float clockTime;
        private bool enableCollisionDebug;

        /// <summary>
        ///  Matiral Node has the instructions to preform on a matiral
        /// </summary>
        public struct GameObjectMaterialNode
        {
            private Effect effect;
            private Action<int> prePass;
            private Action<int> info;
            private Action<int> reset;

            /// <summary>
            ///  The shader to run on the activation pass
            /// </summary>
            public Effect Effect
            {
                get
                {
                    return effect;
                }
            }

            /// <summary>
            ///  Defines a Matiral Node with three stages:
            ///   - Setup, pre activatoin of the node
            ///   - Activate activates the node,
            ///   - Reset resets things the way they were before activate ran
            /// </summary>
            /// <param name="shader">Shader to run</param>
            /// <param name="setup">Setup for the shader</param>
            /// <param name="properties">Any aditional actions to run on activation</param>
            /// <param name="reset">Reset everything</param>
            public GameObjectMaterialNode(Effect shader, Action<int> setup, Action<int> properties, Action<int> reset)
            {
                prePass = setup;
                effect = shader;
                info = properties;
                this.reset = reset;
            }

            /// <summary>
            ///  Setup for this Node
            /// </summary>
            public void Setup()
            {
                prePass(0);
            }
            
            /// <summary>
            ///  Activates this Node
            /// </summary>
            public void Activate()
            {
                info(0);
            }

            /// <summary>
            ///  Reset pass, 
            ///  make everyhing the way it was before you ran activeate
            /// </summary>
            public void Reset()
            {
                reset(0);
            }
        }

        /// <summary>
        ///  Material here allows for mutli view of shaders
        /// </summary>
        public struct GameObjectMaterial
        {
            private List<GameObjectMaterialNode> shaderBuffer;
            private bool skipUniversalPass;

            /// <summary>
            ///  Creates a GameObjectMatiral
            /// </summary>
            public GameObjectMaterial()
            {
                shaderBuffer = new List<GameObjectMaterialNode>();
                skipUniversalPass = false;
            }

            /// <summary>
            ///  Skips the universal shader
            /// </summary>
            public void SkipUniversalPass()
            {
                skipUniversalPass = true;
            }

            /// <summary>
            ///  Enables the universal shader
            /// </summary>
            public void EnableUniversalPass()
            {
                skipUniversalPass = false;
            }

            /// <summary>
            ///  Adds a material node to the shaderBuffer to be drawn
            /// </summary>
            /// <param name="effect"></param>
            public void AddMaterialNode(GameObjectMaterialNode effect)
            { 
                shaderBuffer.Add(effect);
            }

            /// <summary>
            ///  Draws the material at differnt stages per shader
            /// </summary>
            /// <param name="draw">All of the draw code</param>
            public void Draw(Action<byte> draw)
            {
                foreach (GameObjectMaterialNode shader in shaderBuffer)
                {
                    shader.Setup();
                    if (!skipUniversalPass)
                    {
                        draw(0);
                    }
                    GameObject.Instance.SpriteBatch.End();
                    shader.Activate();
                    GameObject.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, effect: shader.Effect);
                    draw(0);
                    GameObject.Instance.SpriteBatch.End();
                    shader.Reset();
                    GameObject.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, effect: GameObject.Instance.UniversalShader);
                    skipUniversalPass = false;
                }
            }
        }

        /// <summary>
        ///  Game Object Instance System,
        ///  Uses the Trie datastruture for splitting content per instance
        /// </summary>
        /// <typeparam name="T">Type of content</typeparam>
        public struct GameObjectTrie<T>
        {
            /// <summary>
            ///  Retrives the data for the instance pass and tpye of content
            /// </summary>
            /// <param name="layer1Find">tpye of content</param>
            /// <param name="layer3Pass">instance pass</param>
            /// <returns>Data</returns>
            public object GetPass(ushort layer1Find, ushort layer3Pass)
            {
                switch (layer1Find)
                {
                    case animationConst:
                        return GameObjectTrie<Animation>.Get(layer1Find, layer3Pass, GameObject.AnimationsI);
                    case colliderConst:
                        return GameObjectTrie<Collider>.Get(layer1Find, layer3Pass, GameObject.CollidersI);
                    case transformConst:
                        return GameObjectTrie<Rectangle>.Get(layer1Find, layer3Pass, GameObject.TransformsI);
                    case positionConst:
                        return GameObjectTrie<Vector2>.Get(layer1Find, layer3Pass, GameObject.PositionsI);
                    case spritesConst:
                        return GameObjectTrie<Texture2D>.Get(layer1Find, layer3Pass, GameObject.SpritesI);
                    case objectEnumsConst:
                        return GameObjectTrie<string>.Get(layer1Find, layer3Pass, GameObject.ObjectEnumsI);
                    case scalesConst:
                        return GameObjectTrie<float>.Get(layer1Find, layer3Pass, GameObject.ScalesI);
                    case effectConst:
                        return GameObjectTrie<Effect>.Get(layer1Find, layer3Pass, GameObject.EffectI);
                    default:
                        return GameObjectTrie<Texture2D>.Get(layer1Find, layer3Pass, GameObject.SpritesI);
                }
            }

            /// <summary>
            ///  Determines the storage and sets the data
            /// </summary>
            /// <param name="layer1Pass">Type of content</param>
            /// <param name="layer3Pass">Instance number</param>
            /// <param name="value">Value to set</param>
            public void SetPass(ushort layer1Pass, ushort layer3Pass, object value)
            {
                switch (layer1Pass)
                {
                    case animationConst:
                        GameObjectTrie<Animation>.Set(layer1Pass, layer3Pass, GameObject.AnimationsI, (Animation)value);
                        break;
                    case colliderConst:
                        GameObjectTrie<Collider>.Set(layer1Pass, layer3Pass, GameObject.CollidersI, (Collider)value);
                        break;
                    case transformConst:
                        GameObjectTrie<Rectangle>.Set(layer1Pass, layer3Pass, GameObject.TransformsI, (Rectangle)value);
                        break;
                    case positionConst:
                        GameObjectTrie<Vector2>.Set(layer1Pass, layer3Pass, GameObject.PositionsI, (Vector2)value);
                        break;
                    case spritesConst:
                        GameObjectTrie<Texture2D>.Set(layer1Pass, layer3Pass, GameObject.SpritesI, (Texture2D)value);
                        break;
                    case objectEnumsConst:
                        GameObjectTrie<string>.Set(layer1Pass, layer3Pass, GameObject.ObjectEnumsI, (string)value);
                        break;
                    case scalesConst:
                        GameObjectTrie<float>.Set(layer1Pass, layer3Pass, GameObject.ScalesI, (float)value);
                        break;
                    case effectConst:
                        GameObjectTrie<Effect>.Set(layer1Pass, layer3Pass, GameObject.EffectI, (Effect)value);
                        break;
                    default:
                        GameObjectTrie<float>.Set(layer1Pass, layer3Pass, GameObject.ScalesI, (float)value);
                        break;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="layer1Find"></param>
            /// <param name="layer3Find"></param>
            /// <param name="data"></param>
            /// <returns></returns>
            #nullable disable
            public static T Get(ushort layer1Find, ushort layer3Find, List<T> data)
            {
                if (layer1Find >= Trie.Count)
                {
                    for (ushort i = (ushort)Trie.Count; i <= layer1Find; i++)
                    {
                        Trie.Add(new List<List<ushort>>());
                    }
                }
                if (GameObject.Instance.Index >= Trie[layer1Find].Count)
                {
                    for (ushort i = (ushort)Trie[layer1Find].Count; i <= GameObject.Instance.Index; i++)
                    {
                        Trie[layer1Find].Add(new List<ushort>());
                    }
                }
                if (layer3Find >= Trie[layer1Find][GameObject.Instance.Index].Count)
                {
                    for (ushort i = (ushort)Trie[layer1Find][GameObject.Instance.Index].Count; i <= layer3Find; i++)
                    {
                        Trie[layer1Find][GameObject.Instance.Index].Add((ushort)(data.Count));
                        data.Add(default(T));
                    }
                }
                return data[Trie[layer1Find][GameObject.Instance.Index][layer3Find]];
            }

            /// <summary>
            ///  Adds info into the trie for the type of content
            /// </summary>
            /// <param name="layer1Find">Type of content</param>
            /// <param name="data">storage</param>
            /// <param name="value">Value to add</param>
            /// <returns>Index of addition</returns>
            public static ushort Add(ushort layer1Find, List<T> data, T value)
            {
                if (layer1Find >= Trie.Count)
                {
                    for (ushort i = (ushort)Trie.Count; i <= layer1Find; i++)
                    {
                        Trie.Add(new List<List<ushort>>());
                    }
                }
                if (GameObject.Instance.Index >= Trie[layer1Find].Count)
                {
                    for (ushort i = (ushort)Trie[layer1Find].Count; i <= GameObject.Instance.Index; i++)
                    {
                        Trie[layer1Find].Add(new List<ushort>());
                    }
                }
                Trie[layer1Find][GameObject.Instance.Index].Add((ushort)data.Count);
                ushort layer3Find = (ushort)(data.Count);
                data.Add(value);
                return layer3Find;
            }

            /// <summary>
            ///  Sets the value in the trie 
            /// </summary>
            /// <param name="layer1Find">Type of content</param>
            /// <param name="layer3Find">Instance number</param>
            /// <param name="data">storage</param>
            /// <param name="value">Value to set</param>
            public static void Set(ushort layer1Find, ushort layer3Find, List<T> data, T value)
            {
                if (layer1Find >= Trie.Count)
                {
                    for (ushort i = (ushort)Trie.Count; i <= layer1Find; i++)
                    {
                        Trie.Add(new List<List<ushort>>());
                    }
                }
                if (GameObject.Instance.Index >= Trie[layer1Find].Count)
                {
                    for (ushort i = (ushort)Trie[layer1Find].Count; i <= GameObject.Instance.Index; i++)
                    {
                        Trie[layer1Find].Add(new List<ushort>());
                    }
                }
                if (layer3Find >= Trie[layer1Find][GameObject.Instance.Index].Count)
                {
                    for (int i = Trie[layer1Find][GameObject.Instance.Index].Count; i <= layer3Find; i++)
                    {
                        Trie[layer1Find][GameObject.Instance.Index].Add((ushort)(data.Count));
                        data.Add(value);
                    }
                }
                data[Trie[layer1Find][GameObject.Instance.Index][layer3Find]] = value;
            }

            /// <summary>
            ///  Retrive the List of Data for the specific type of layer1 content
            /// </summary>
            /// <param name="layer1Find">type of content</param>
            /// <param name="data">The data as an List</param>
            /// <returns>List of data</returns>
            public static List<T> GetArray(ushort layer1Find, List<T> data)
            {
                if (layer1Find >= Trie.Count)
                {
                    for (int i = Trie.Count; i <= layer1Find; i++)
                    {
                        Trie.Add(new List<List<ushort>>());
                    }
                }
                if (GameObject.Instance.Index >= Trie[layer1Find].Count)
                {
                    for (int i = Trie[layer1Find].Count; i <= GameObject.Instance.Index; i++)
                    {
                        Trie[layer1Find].Add(new List<ushort>());
                    }
                }
                List<T> result = new List<T>();
                foreach (int index in Trie[layer1Find][GameObject.Instance.Index])
                {
                    result.Add(data[index]);
                }

                return result;
            }

            /// <summary>
            ///  Returns the entore fourth layer
            /// </summary>
            /// <param name="layer1Find">Type of content</param>
            /// <param name="data">Storage</param>
            /// <returns></returns>
            public static List<ushort> GetLayer4(ushort layer1Find, List<T> data)
            {
                if (layer1Find >= Trie.Count)
                {
                    for (int i = Trie.Count; i <= layer1Find; i++)
                    {
                        Trie.Add(new List<List<ushort>>());
                    }
                }
                if (GameObject.Instance.Index >= Trie[layer1Find].Count)
                {
                    for (int i = Trie[layer1Find].Count; i <= GameObject.Instance.Index; i++)
                    {
                        Trie[layer1Find].Add(new List<ushort>());
                    }
                }
                List<ushort> result = new List<ushort>();
                foreach (ushort index in Trie[layer1Find][GameObject.Instance.Index])
                {
                    result.Add(index);
                }
                return result;
            }
            #nullable enable

            /// <summary>
            ///  Returns the fourth layer instance number of the Trie that contains all of the instances of this type of data
            /// </summary>
            /// <param name="layer1Find">The type of info</param>
            /// <param name="layer3Find">The which instance</param>
            /// <param name="data">storage</param>
            /// <returns>Layer 4</returns>
            public static ushort GetLayer4Instance(ushort layer1Find, ushort layer3Find, List<T> data)
            {
                if (layer1Find >= Trie.Count)
                {
                    for (int i = Trie.Count; i <= layer1Find; i++)
                    {
                        Trie.Add(new List<List<ushort>>());
                    }
                }
                if ((GameObject.Instance.Index) >= Trie[layer1Find].Count)
                {
                    for (int i = Trie[layer1Find].Count; i <= (GameObject.Instance.Index); i++)
                    {
                        Trie[layer1Find].Add(new List<ushort>());
                    }
                }
                return Trie[layer1Find][GameObject.Instance.Index][layer3Find];
            }
            #nullable enable
        }

        /// <summary>
        ///  The Instance number for this Game Object
        /// </summary>
        public ushort InstanceID
        {
            get
            {
                return GameObject.Instance.instance;
            }
            set
            {
                GameObject.Instance.instance = value;
            }
        }

        /// <summary>
        ///  GameObject master storage place
        /// </summary>
        public static GameObject Instance
        {
            // Singleton for GameObject
            get
            {
                if (allGameObjects == null)
                {
                    allGameObjects = new GameObject();
                }
                return allGameObjects;
            }
        }

        /// <summary>
        ///  Returns the Instance
        /// </summary>
        /// <typeparam name="T">Type of GameObject to determine</typeparam>
        /// <returns>Instance</returns>
        public GameObject GetInstance<T>()
        {
            // Magically determine the type by accesing the instance
            return GameObject.Instance;
        }

        /// <summary>
        ///  Trie instance
        /// </summary>
        private static List<List<List<ushort>>> Trie
        {
            get
            {
                if (trie == null)
                {
                    trie = new List<List<List<ushort>>>();
                }
                return trie;
            }
            set
            {
                trie = value;
            }
        }

        /// <summary>
        ///  Internal Sprites storage
        /// </summary>
        private static List<Texture2D> SpritesI
        {
            get
            {
                if (sprites == null)
                {
                    sprites = new List<Texture2D>();
                }
                return sprites;
            }
            set
            {
                sprites = value;
            }
        }

        /// <summary>
        ///  Internal Scaler storage
        /// </summary>
        private static List<float> ScalesI
        {
            get
            {
                if (scales == null)
                {
                    scales = new List<float>();
                }
                return scales;
            }
            set
            {
                scales = value;
            }
        }

        /// <summary>
        ///  Internal Enums storage
        /// </summary>
        private static List<string> ObjectEnumsI
        {
            get
            {
                if (objectEnums == null)
                {
                    objectEnums = new List<string>();
                }
                return objectEnums;
            }
            set
            {
                objectEnums = value;
            }
        }

        /// <summary>
        ///  internal Animation storage
        /// </summary>
        private static List<Animation> AnimationsI
        {
            get
            {
                if (animations == null)
                {
                    animations = new List<Animation>();
                }
                return animations;
            }
            set
            {
                animations = value;
            }
        }

        /// <summary>
        ///  Inernal Colliders storage
        /// </summary>
        private static List<Collider> CollidersI
        {
            get
            {
                if (colliders == null)
                {
                    colliders = new List<Collider>();
                }
                return colliders;
            }
            set
            {
                colliders = value;
            }
        }

        /// <summary>
        ///  Internal Transforms storage
        /// </summary>
        private static List<Rectangle> TransformsI
        {
            get
            {
                if (transforms == null)
                {
                    transforms = new List<Rectangle>();
                }
                return transforms;
            }
            set
            {
                transforms = value;
            }
        }

        /// <summary>
        /// Internal Positions storage
        /// </summary>
        private static List<Vector2> PositionsI
        {
            get
            {
                if (positions == null)
                {
                    positions = new List<Vector2>();
                }
                return positions;
            }
            set
            {
                positions = value;
            }
        }

        /// <summary>
        ///  Internal Effects storage
        /// </summary>
        private static List<Effect> EffectI
        {
            get
            {
                if (effects == null)
                {
                    effects = new List<Effect>();
                }
                return effects;
            }
            set
            {
                effects = value;
            }
        }

        /// <summary>
        ///  Shader that applies to everything
        /// </summary>
        public Effect UniversalShader
        {
            get
            {
                return universalShader;
            }
            set
            {
                universalShader = value;
            }
        }

        /// <summary>
        ///  Top Position of the level
        /// </summary>
        public static Vector2 EndPosition
        {
            get
            {
                return new Vector2(0, GameObject.Instance.GraphicsDevice.Viewport.Height * -3);
            }
        }
        
        /// <summary>
        ///  Retrives the sprite for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <returns>Sprite Texture2D</returns>
        public Texture2D GetSprite(ushort instancePass)
        {
            #nullable disable
            unsafe
            {
                return (spritesI).GetPass(spritesConst, instancePass) as Texture2D;
            }
            #nullable enable
        }

        /// <summary>
        ///  Retrives the scale for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <returns>Scale float</returns>
        public float GetScale(ushort instancePass)
        {
            #nullable disable
            unsafe
            {
                return (float)(scalesI).GetPass(scalesConst, instancePass);
            }
            #nullable enable
        }

        /// <summary>
        ///  Retrives the object enum for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <returns>Enum string</returns>
        public string GetObjectEnum(ushort instancePass)
        {
            #nullable disable
            unsafe
            {
                return (objectEnumsI).GetPass(objectEnumsConst, instancePass) as string;
            }
            #nullable enable
        }

        /// <summary>
        ///  Retrives the animation for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <returns>Animation</returns>
        public Animation GetAnimation(ushort instancePass)
        {
            #nullable disable
            unsafe
            {
                return (animationsI).GetPass(animationConst, instancePass) as Animation;
            }
            #nullable enable
        }

        /// <summary>
        ///  Retrives the collider for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <returns>Collider</returns>
        public Collider GetCollider(ushort instancePass)
        {
            #nullable disable
            unsafe
            {
                return (collidersI).GetPass(colliderConst, instancePass) as Collider;
            }
            #nullable enable
        }

        /// <summary>
        ///  Retrives the Transform for the isntance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <returns>Transform Rectangle</returns>
        public Rectangle GetTransform(ushort instancePass)
        {
            #nullable disable
            unsafe
            {
                return (Rectangle)(transformsI).GetPass(transformConst, instancePass);
            }
            #nullable enable
        }

        /// <summary>
        ///  Retrives the Position for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <returns>Position Vector2</returns>
        public Vector2 GetPosition(ushort instancePass)
        {
            #nullable disable
            unsafe
            {
                return (Vector2)(positionsI).GetPass(positionConst, instancePass);
            }
            #nullable enable
        }

        /// <summary>
        ///  Retrives the Effect for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <returns>Effect</returns>
        public Effect GetEffect(ushort instancePass)
        {
            #nullable disable
            unsafe
            {
                return (Effect)(effectI).GetPass(effectConst, instancePass);
            }
            #nullable enable
        }

        /// <summary>
        ///  Set the sprite for the instasnce
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <param name="value">Texure2D sprite</param>
        public void SetSprite(ushort instancePass, object value)
        {
            #nullable disable
            unsafe
            {
                (spritesI).SetPass(spritesConst, instancePass, value);
            }
            #nullable enable
        }

        /// <summary>
        ///  Set the scale for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <param name="value">float scale</param>
        public void SetScale(ushort instancePass, object value)
        {
            #nullable disable
            unsafe
            {
                (scalesI).SetPass(scalesConst, instancePass, value);
            }
            #nullable enable
        }

        /// <summary>
        ///  Set the game object enum for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <param name="value">string enum</param>
        public void SetObjectEnum(ushort instancePass, object value)
        {
            #nullable disable
            unsafe
            {
                (objectEnumsI).SetPass(objectEnumsConst, instancePass, value);
            }
            #nullable enable
        }

        /// <summary>
        ///  Set the animation for the instance
        /// </summary>
        /// <param name="instancePass">instance number</param>
        /// <param name="value">Animation</param>
        public void SetAnimation(ushort instancePass, object value)
        {
            #nullable disable
            unsafe
            {
                (animationsI).SetPass(animationConst, instancePass, value);
            }
            #nullable enable
        }

        /// <summary>
        ///  Set the collider for the instance
        /// </summary>
        /// <param name="instancePass">Instance number</param>
        /// <param name="value">Collider</param>
        public void SetCollider(ushort instancePass, object value)
        {
            #nullable disable
            unsafe
            {
                (collidersI).SetPass(colliderConst, instancePass, value);
            }
            #nullable enable
        }

        /// <summary>
        ///  Sets the transform for the instance
        /// </summary>
        /// <param name="instancePass">Instance number</param>
        /// <param name="value">Transform Rectangle</param>
        public void SetTransform(ushort instancePass, object value)
        {
            #nullable disable
            unsafe
            {
                (transformsI).SetPass(transformConst, instancePass, value);
            }
            #nullable enable
        }

        /// <summary>
        ///  Sets as a refrence the Position of the instance
        /// </summary>
        /// <param name="instancePass">Instance number</param>
        /// <param name="value">Position Vector2</param>
        public void SetPosition(ushort instancePass, object value)
        {
            #nullable disable
            unsafe
            {
                (positionsI).SetPass(positionConst, instancePass, value);
            }
            #nullable enable
        }

        /// <summary>
        ///  Sets as a referne the Effect
        /// </summary>
        /// <param name="instancePass">Instance number</param>
        /// <param name="value">Effect</param>
        public void SetEffect(ushort instancePass, object value)
        {
            #nullable disable
            unsafe
            {
                (effectI).SetPass(effectConst, instancePass, value);
            }
            #nullable enable
        }

        /// <summary>
        ///  Red amount
        /// </summary>
        public static float RedShade
        {
            get
            {
                return redShade;
            }
            set
            {
                redShade = value;
            }
        }

        /// <summary>
        ///  The shade sine wave
        /// </summary>
        public static float ShadeTime
        {
            get
            {
                return timeShadeEffect;
            }
            set
            {
                timeShadeEffect = value;
            }
        }

        /// <summary>
        ///  The Actual storage of colliders
        /// </summary>
        /// <returns></returns>
        public ref List<Collider> ColliderCollisions()
        {
            return ref colliders;
        }

        /// <summary>
        ///  Retrives all of the colliders trie
        /// </summary>
        /// <returns>All of the colliders</returns>
        public List<ushort> ColliderLayer4()
        {
            return GameObjectTrie<Collider>.GetLayer4(colliderConst, colliders);
        }

        /// <summary>
        ///  Retrives the foruth layer index for the instance in the trie
        /// </summary>
        /// <param name="instanceNumber">Instance of the collider</param>
        /// <returns></returns>
        public ushort ColliderLayer4Instance(ushort instanceNumber)
        {
            return GameObjectTrie<Collider>.GetLayer4Instance(colliderConst, instanceNumber, colliders);
        }

        /// <summary>
        ///  Deletes all of the data for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void Delete(ushort instanceNumber)
        {
            (spritesI).SetPass(spritesConst, instanceNumber, default(Texture2D));
            (scalesI).SetPass(scalesConst, instanceNumber, default(float));
            (objectEnumsI).SetPass(objectEnumsConst, instanceNumber, default(string));
            (animationsI).SetPass(animationConst, instanceNumber, default(Animation));
            (collidersI).SetPass(colliderConst, instanceNumber, default(Collider));
            (transformsI).SetPass(transformConst, instanceNumber, default(Rectangle));
            (positionsI).SetPass(positionConst, instanceNumber, default(Vector2));
            (effectI).SetPass(effectConst, instanceNumber, default(Effect));
        }

        /// <summary>
        ///  Delets the collider for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void DeleteCollider(ushort instanceNumber)
        {
            (collidersI).SetPass(colliderConst, instanceNumber, default(Collider));
        }

        /// <summary>
        ///  Delets the sprite for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void DeleteSprite(ushort instanceNumber)
        {
            (spritesI).SetPass(spritesConst, instanceNumber, default(Texture2D));
        }

        /// <summary>
        ///  Delets the scale for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void DeleteScale(ushort instanceNumber)
        {
            (scalesI).SetPass(scalesConst, instanceNumber, default(float));
        }

        /// <summary>
        ///  Deletes the enum for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void DeleteEnum(ushort instanceNumber)
        {
            (objectEnumsI).SetPass(objectEnumsConst, instanceNumber, default(string));
        }

        /// <summary>
        ///  Deletes the Animation for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void DeleteAnimation(ushort instanceNumber)
        {
            (animationsI).SetPass(animationConst, instanceNumber, default(Animation));
        }

        /// <summary>
        ///  Delets the transform for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void DeleteTransform(ushort instanceNumber)
        {
            (transformsI).SetPass(transformConst, instanceNumber, default(Rectangle));
        }

        /// <summary>
        ///  Deletes the position for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void DeletePosition(ushort instanceNumber)
        {
            (positionsI).SetPass(positionConst, instanceNumber, default(Vector2));
        }

        /// <summary>
        ///  Delets the effect for the given instance
        /// </summary>
        /// <param name="instanceNumber"></param>
        public void DeleteEffect(ushort instanceNumber)
        {
            (effectI).SetPass(effectConst, instanceNumber, default(Effect));
        }

        /// <summary>
        ///  Content name of the Game Object
        ///  Matches the dynamic for the Content
        ///  Ex: GameObject.Content.example_content
        /// </summary>
        public ushort ContentName
        {
            get
            {
                return contentName;
            }
        }

        /// <summary>
        ///  Current index relation to all of the content arrays
        /// </summary>
        public ushort Index
        {
            // ONLY allow for receiving the index here
            get
            {
                //System.Diagnostics.Debug.WriteLine(GameObject.Instance.index);
                return (ushort)(GameObject.Instance.index);
            }
        }

        /// <summary>
        ///  Gets the GameObject Instance for the index
        ///  Sets the index for the Instance
        /// </summary>
        public dynamic Content
        {
            get
            {
                return this;
            }
            set
            {
                GameObject.Instance.index = value;
            }
        }

        /// <summary>
        ///  Graphics Device
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            // Only allow to recive the GraphicsDevice
            get
            {
                return GameObject.Instance.graphicsDevice;
            }
        }

        /// <summary>
        ///  Sprite Batch
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            // Only alow to set the SpriteBatch
            get
            {
                return GameObject.Instance.spriteBatch;
            }
        }

        /// <summary>
        ///  Fade of the the universal shader
        /// </summary>
        public static float Fade
        {
            get
            {
                return fade;
            }
            set
            {
                fade = value;
            }
        }

        /// <summary>
        ///  If the collider should wait on update
        /// </summary>
        public bool HoldCollider
        {
            get
            {
                return holdCollider;
            }
            set
            {
                holdCollider = value;
            }
        }

        /// <summary>
        ///  Number of positions sotred
        /// </summary>
        public ushort PositionLength
        {
            get
            {
                return (ushort)positions.Count;
            }
        }

        /// <summary>
        ///  Determinate for if the boss effect is active or not
        /// </summary>
        public bool IsBossEffectActive
        {
            get
            {
                return bossEffectIsActive;
            }
        }

        /// <summary>
        ///  The Shade value for the boss effect
        /// </summary>
        public float TimeShade
        {
            get
            {
                return timeShadeEffect;
            }
        }

        /// <summary>
        ///  Starts the boss effect
        /// </summary>
        public void StartBossEffect()
        {
            bossEffectIsActive = true;
        }

        /// <summary>
        ///  Stops the boss effect
        /// </summary>
        public void StopBossEffect()
        {
            bossEffectIsActive = false;
        }

        /// <summary>
        ///  Playes the red flashing effect
        ///  adjusts the timeShadeEffect to be a sine wave
        /// </summary>
        public void PlayBossEffect()
        {
            // Create a sine wave by constatnly adding and subtractive values to 0 - 1 
            if (timeShadeEffect >= 1)
            {
                flipSine = !flipSine;
                timeShadeEffect -= 0.01f;
            }
            else if (timeShadeEffect <= 0)
            {
                flipSine = !flipSine;
                timeShadeEffect += 0.01f;
            }
            else
            {
                if (flipSine)
                {
                    timeShadeEffect -= 0.01f;
                }
                else
                {
                    timeShadeEffect += 0.01f;
                }
            }
        }


        /// <summary>
        ///  Generates a index for the instance Content 
        ///  property that cannot be found
        ///  The binder name will be stored at the index
        ///  in objectEnums that matches the index generated
        /// </summary>
        /// <param name="binder">Property binder</param>
        /// <param name="result">The generated index</param>
        /// <returns>GameObject Index</returns>
        public override bool TryGetMember(
            GetMemberBinder binder,
            out object result
        )
        {
            bool exist = false; // If the property exist
            ushort index = 0;   // The index of the property

            // Search to find the property
            foreach (string sprite in GameObject.ObjectEnumsI)
            {
                if (binder.Name == sprite)
                {
                    exist = true;
                }
                if (exist)
                {
                    this.index = index;
                    result = index;
                    return exist;
                }
                index++;
            }

            // When the property does not exist create it
            if (!exist)
            {
                exist = true;
                GameObject.ObjectEnumsI.Add(binder.Name);
                GameObject.ObjectEnumsI = GameObject.ObjectEnumsI;
            }
            this.index = index;
            result = index;
            return exist;
        }

        /// <summary>
        ///  Retrives the sprite from the given content name and instance number
        /// </summary>
        /// <param name="contentName">Content name of the sprite</param>
        /// <param name="instanceNumber">Instance number of the sprite</param>
        /// <returns></returns>
        public Texture2D GetSpriteFrom(ushort contentName, ushort instanceNumber)
        {
            GameObject.Instance.Content = contentName;
            GameObject.Instance.instance = instanceNumber;
            string path = GameObject.ObjectEnumsI[contentName];
            GameObject.Instance.index = contentName;
            string start = "../../../Content";
            string[] files = Directory.GetFiles(start, path + ".*", SearchOption.AllDirectories);
            files[0] = files[0].Replace(start, "");
            files[0] = files[0].Replace("\\", "/");
            files[0] = files[0].Substring(1);
            files[0] = files[0].Substring(0, files[0].LastIndexOf('.'));
            return GameObject.Instance.contentManager.Load<Texture2D>(files[0]);
        }

        /// <summary>
        ///  Loads a Spirte via its content name and instance number
        /// </summary>
        /// <param name="contentName">Content name of the sprite</param>
        /// <param name="instanceNumber">Instance number of the sprite</param>
        public void LoadSprite(ushort contentName, ushort instanceNumber)
        {
            GameObject.Instance.Content = contentName;
            GameObject.Instance.instance = instanceNumber;
            string path = GameObject.ObjectEnumsI[contentName];
            GameObject.Instance.index = contentName;
            string start = "../../../Content";
            string[] files = Directory.GetFiles(start, path + ".*", SearchOption.AllDirectories);
            files[0] = files[0].Replace(start, "");
            files[0] = files[0].Replace("\\", "/");
            files[0] = files[0].Substring(1);
            files[0] = files[0].Substring(0, files[0].LastIndexOf('.'));
            SetSprite(instanceNumber, GameObject.Instance.contentManager.Load<Texture2D>(files[0]));
        }

        /// <summary>
        ///  Just creates the Game Object to reference
        /// </summary>
        private GameObject()
        {
            // Does nothing, just is used to create a singleton instance
        }

        /// <summary>
        ///  Creates a GameObject from the content name
        ///  the content name must be in the format file_strip(i) where 
        ///  (i) is the number of sprites in the sprite sheet
        ///  Define this GameObject as a specific instance via its instanceNumber
        /// </summary>
        /// <param name="contentName">content name</param>
        /// <param name="instanceNumber">index of the instance</param>
        unsafe public GameObject(
            ushort contentName,
            ushort instanceNumber,
            CollisionGroup collisionGroup
        )
        {
            // Foce no overlap on the first instance number
            // Determine the collision group
            GameObject.Instance.Content = contentName;
            if (GetScale(instanceNumber) != 0)
            {
                instanceNumber = (ushort)(PositionLength);
                if (instanceNumber == 0)
                {
                    instanceNumber = 1;
                }
            }
            GameObject.Instance.InstanceID = instanceNumber;
            switch (collisionGroup)
            {
                case CollisionGroup.Player:
                    typeOfObject = typeof(Player);
                    break;
                case CollisionGroup.Bullet:
                    typeOfObject = typeof(Bullet);
                    break;
                case CollisionGroup.Tile:
                    typeOfObject = typeof(Tile);
                    break;
                case CollisionGroup.Enemy:
                    typeOfObject = typeof(Enemy);
                    break;
            }
            this.contentName = contentName;
            // Determine collision group and set the collision group
            CollisionGroupISet(contentName, instanceNumber, collisionGroup);
            this.collisionGroup = collisionGroup;
            string path = GameObject.ObjectEnumsI[contentName];
            string start = "../../../Content";
            string[] files = Directory.GetFiles(start, path + ".*", SearchOption.AllDirectories);
            files[0] = files[0].Replace(start, "");
            files[0] = files[0].Replace("\\", "/");
            files[0] = files[0].Substring(1);
            files[0] = files[0].Substring(0, files[0].LastIndexOf('.'));
            ushort strip = ushort.Parse(path.Split("strip")[1]);

            // Creates the animaiton, position, transform, scale and sprite
            SetSprite(instanceNumber, GameObject.Instance.contentManager.Load<Texture2D>(files[0]));
            SetScale(instanceNumber, 1.0f);
            SetAnimation(instanceNumber, new Animation(GetSprite(instanceNumber).Width, GetSprite(instanceNumber).Height, strip));
            SetPosition(instanceNumber, Vector2.Zero);
            SetTransform(instanceNumber,
                new Rectangle(
                    (GetSprite(instanceNumber).Width / strip),         // Sprite starts at the frame starting position
                    0,                                                 // Sprite starts at Y = 0
                    GetSprite(instanceNumber).Width / strip,           // Width of the sprite
                    GetSprite(instanceNumber).Height                   // Height of the sprite
                )
            );

            // Create Collider
            Collider newCollider = new Collider(GetSprite(instanceNumber), GetPosition(instanceNumber), GetTransform(instanceNumber), SpriteEffects.None, GetScale(instanceNumber), GraphicsDevice, SpriteBatch, (ushort)collisionGroup, this);
            newCollider.Layer = contentName;
            newCollider.Resolved = true;
            newCollider.self = this;
            SetCollider(instanceNumber, newCollider);

        }

        /// <summary>
        ///  Scale to the grid size
        /// </summary>
        /// <returns>Scale</returns>
        public float PostScaleRatio()
        {
            return ((this.GetTransform(instance).Width < this.GetTransform(instance).Height) ? universalScaleX / this.GetTransform(instance).Width * GameObject.Instance.GraphicsDevice.Viewport.Width / GameObject.Instance.GraphicsDevice.Viewport.Height : universalScaleY / this.GetTransform(instance).Height * GameObject.Instance.GraphicsDevice.Viewport.Height / GameObject.Instance.GraphicsDevice.Viewport.Width);
        }

        /// <summary>
        ///  Scale to the grid size
        /// </summary>
        /// <param name="isVector2">Determinate for a Vector2</param>
        /// <returns>Scale</returns>
        public Vector2 PostScaleRatio(bool isVector2)
        {
            float fixedScaler = 1.0f;
            return new Vector2(
                universalScaleX / this.GetTransform(instance).Width,
                universalScaleY / this.GetTransform(instance).Height
            ) * fixedScaler;
        }

        /// <summary>
        ///  Type of the Game Object
        /// </summary>
        public System.Type GameObjectType
        {
            get
            {
                return typeOfObject;
            }
        }

        /// <summary>
        ///  Initalizes the instance with 
        ///  a contentManager, graphicsDevice,
        ///  spriteBatch
        /// </summary>
        /// <param name="contentManager">Any: ContentManager</param>
        /// <param name="graphicsDevice">Any: GraphicsDevice</param>
        /// <param name="spriteBatch">Any: SpriteBatch</param>
        /// <returns>Game Object Instance as a dynamic</returns>
        public dynamic Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Effect effect)
        {
            this.universalShader = effect;
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
            this.contentManager = contentManager;
            GameObject.fade = 1;
            GameObject.Instance.holdCollider = false;
            redShade = 1;
            timeShadeEffect = 1;
            flipSine = false;
            cameraStopPositions = new List<Vector2>();
            flipSine = false;
            universalScaleX = 1;
            universalScaleY = 1;
            clockTime = 0;
            enableCollisionDebug = false;
            return new GameObject();
        }

        /// <summary>
        ///  Retrives the collision group for the specified content and instance
        /// </summary>
        /// <param name="contentName">Content dynamic</param>
        /// <param name="instance">Instance number</param>
        /// <returns>Collision group of the GameObject</returns>
        private CollisionGroup CollisionGroupIGet(ushort contentName, ushort instance)
        {
            // Navigates the trie to the matching content name and instance number
            // Retricves the collision group from the trie
            if (GameObject.Instance.collisionGroups == null)
            {
                GameObject.Instance.collisionGroups = new List<List<CollisionGroup>>();
            }
            for (int i = GameObject.Instance.collisionGroups.Count - 1; i <= contentName; i++)
            {
                GameObject.Instance.collisionGroups.Add(new List<CollisionGroup>());
            }
            for (int i = GameObject.Instance.collisionGroups[contentName].Count - 1; i <= instance; i++)
            {
                GameObject.Instance.collisionGroups[contentName].Add(CollisionGroup.None);
            }
            return GameObject.Instance.collisionGroups[contentName][instance];
        }

        /// <summary>
        ///  Sets the collision group for the specified content and instance
        /// </summary>
        /// <param name="contentName">Content dynamic</param>
        /// <param name="instance">Instance number</param>
        /// <returns>Collision group of the GameObject</returns>
        private CollisionGroup CollisionGroupISet(ushort contentName, ushort instance, CollisionGroup value)
        {
            // Navigates the trie to the matching content name and instance number
            // Adds the collision group to the trie
            if (GameObject.Instance.collisionGroups == null)
            {
                GameObject.Instance.collisionGroups = new List<List<CollisionGroup>>();
            }
            for (int i = GameObject.Instance.collisionGroups.Count - 1; i <= contentName; i++)
            {
                GameObject.Instance.collisionGroups.Add(new List<CollisionGroup>());
            }
            for (int i = GameObject.Instance.collisionGroups[contentName].Count - 1; i <= instance; i++)
            {
                GameObject.Instance.collisionGroups[contentName].Add(value);
            }
            return GameObject.Instance.collisionGroups[contentName][instance];
        }

        /// <summary>
        ///  The Rectangle to draw the Game Object
        /// </summary>
        public Rectangle Transform
        {
            get
            {
                return GetTransform(instance);
            }
        }

        /// <summary>
        ///  Position of the Game Object
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return GetPosition(instance);
            }
        }

        // Ignore all of these IConvertable requires these,
        // this is to foce functionality to be handled on GameObject
        // GameObject does not use these
        #region NotImplemented
        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }
        public bool ToBoolean(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public byte ToByte(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public char ToChar(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public DateTime ToDateTime(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public decimal ToDecimal(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public double ToDouble(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public short ToInt16(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public int ToInt32(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public long ToInt64(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public sbyte ToSByte(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public float ToSingle(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public string ToString(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public ushort ToUInt16(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public uint ToUInt32(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        public ulong ToUInt64(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }
        #endregion

        // Force IConvertable to accept these as GameObject
        #region Forced Convert to Parent GameObject Instance
        public object ToPlayer()
        {
            return GameObject.Instance;
        }
        public object ToBullet()
        {
            return GameObject.Instance;
        }
        public object ToTile()
        {
            return GameObject.Instance;
        }
        #endregion

        // Determine Which GmaeObject to convert to otherwise default to not implemented
        public object ToType(System.Type conversionType, IFormatProvider? provider)
        {
            if (conversionType == typeof(Player))
            {
                return ToPlayer();
            }
            if (conversionType == typeof(Tile))
            {
                return ToTile();
            }
            if (conversionType == typeof(Bullet))
            {
                return ToBullet();
            }
            return default(object);
        }

        /// <summary>
        ///  Draws all submitted to the 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DebugDraw(SpriteBatch spriteBatch)
        {
            // increment clock time
            clockTime += 0.025f;

            // Reset clock time
            if (clockTime >= 1)
            {
                clockTime = 0;
            }

            // Draw all debug
            if (Debug != null)
            {
                Debug(spriteBatch);
                Debug = null;
            }
        }

        /// <summary>
        ///  Galabingus Content Manger
        /// </summary>
        public ContentManager ContentManager
        {
            get
            {
                return contentManager;
            }
        }

        /// <summary>
        ///  Every position for the camera to stop at
        /// </summary>
        /// <returns>Position for the camera to stop at</returns>
        public List<Vector2> GetCameraStopPositions()
        {
            return GameObject.cameraStopPositions;
        }

        /// <summary>
        ///  Determinate for collision debug
        /// </summary>
        public bool EnableCollisionDebug
        {
            get
            {
                return enableCollisionDebug;
            }
            set
            {
                enableCollisionDebug = value;
            }
        }

        /// <summary>
        ///  Timer that counts 0 - 1 at a fixed rate (not v-synced)
        /// </summary>
        public static float ClockTime
        {
            get
            {
                return clockTime;
            }
            set
            {
                clockTime = value;
            }
        }

        /// <summary>
        ///  Converts and removes an index of a camera stop
        /// </summary>
        /// <param name="index"></param>
        public void CameraStopRemoveAt(int index)
        {
            // Split info around and remove the camera stop
            List<Vector2> preI = new List<Vector2>();
            List<Vector2> postI = new List<Vector2>();
            List<Vector2> result = new List<Vector2>();

            // Conjoin the split info left and right side around the stop 
            for (int i = 0; i < index; i++)
            {
                preI.Add(GameObject.cameraStopPositions[i]);
            }
            for (int i = index + 1; i < GameObject.cameraStopPositions.Count; i++)
            {
                postI.Add(GameObject.cameraStopPositions[i]);
            }
            foreach (Vector2 pos in preI)
            {
                result.Add(pos);
            }
            foreach (Vector2 pos in postI)
            {
                result.Add(pos);
            }
            GameObject.cameraStopPositions = result;
        }

        /// <summary>
        ///  Converts level editor rows and columns to Game Coordinates
        /// </summary>
        /// <param name="width">Width (number of tiles)</param>
        /// <param name="height">Height (number of tiles)</param>
        /// <param name="row">Which tile Row is it</param>
        /// <param name="column">Which tile Column is it</param>
        /// <returns>Coordinate of the tile</returns>
        public Vector2 CalculateLevelEditorPositions(float width, float height, float row, float column)
        {
            // Calculate the screen ratios to width and height
            float fixedScaler = 1.5f;
            float coordianteXScale = GameObject.Instance.GraphicsDevice.Viewport.Width / width;
            float coordinateYScale = (-EndPosition.Y + GameObject.Instance.GraphicsDevice.Viewport.Height) / height;
            float startingY = EndPosition.Y;
            startingY *= fixedScaler;

            // Center shift based on the aditoinal scalar (fixedScalar)
            // Also scale the coordinates to match this
            float leftShift = coordianteXScale * width * (1 - fixedScaler) * 0.5f;
            float topShift = (GameObject.Instance.GraphicsDevice.Viewport.Height / height) * height * (1 - fixedScaler) * 0.5f;
            universalScaleX = coordianteXScale * fixedScaler;
            universalScaleY = coordinateYScale * fixedScaler;
            coordianteXScale *= fixedScaler;
            coordinateYScale *= fixedScaler;

            // Resulting Coodinate
            return new Vector2(coordianteXScale * column + leftShift, coordinateYScale * row + startingY + topShift);
        }

        /// <summary>
        ///  Places tiles from a Tile .Level file
        ///  Tiles are in the range of 10 - 35
        /// </summary>
        /// <param name="fileName">Tiles level file</param>
        public void LoadTileLevelFile(string fileName)
        {
            // Read the level file
            StreamReader reader = new StreamReader("../../../" + fileName);
            int lineNumber = 0;
            int width = 0;
            int height = 0;
            int xInput = 0;
            int yInput = 0;
            int boxIdentifier = 0;
            string? data;
            while ((data = reader.ReadLine()) != null)
            {
                // Read in the tile level setup info
                if (lineNumber < 5)
                {
                    switch (lineNumber)
                    {
                        case 0:
                            data = "";
                            break;
                        case 1:
                            height = int.Parse(data);
                            data = "";
                            break;
                        case 2:
                            width = int.Parse(data);
                            data = "";
                            break;
                        case 3:
                            data = "";
                            break;
                        case 4:
                            data = "";
                            break;
                    }
                }
                else
                {
                    // Read in the level row
                    string[] column = data.Split('|');
                    foreach (string num in column)
                    {
                        Vector2 assetPosition = CalculateLevelEditorPositions(width, height, yInput, xInput);
                        if (int.Parse(num) != -1 && int.Parse(num) > 9)
                        {
                            // Place the tile
                            TileManager.Instance.CreateObject(GameObject.Instance.Content.tile_strip26, assetPosition, (ushort)(int.Parse(num) - 10));
                        }
                        xInput++;
                        boxIdentifier++;
                    }
                    xInput = 0;
                    yInput++;
                }
                lineNumber++;
            }
            // Close the file
            reader.Close();
        }

        /// <summary>
        ///  Reads in a .Level file and places enemies
        ///  Enemies are in the range of 0 - 6
        ///  In addition camera stops are number 9
        /// </summary>
        /// <param name="fileName">Galabingus Level file for enemies and level stop</param>
        /// <returns>Enmies list to work with the Enemy Manager</returns>
        public List<int[]> LoadEnemyLeveFile(string fileName)
        {
            // Read a level file
            List<int[]> enemies = new List<int[]>();
            StreamReader reader = new StreamReader("../../../" + fileName);
            int lineNumber = 0;
            int width = 0;
            int height = 0;
            int xInput = 0;
            int yInput = 0;
            int boxIdentifier = 0;
            bool ready = false;
            string? data = "";
            do
            {
                if (ready)
                {
                    if (lineNumber < 5)
                    {
                        // Read in the level defining properites that are not external tool specific
                        switch (lineNumber)
                        {
                            case 0:
                                data = "";
                                break;
                            case 1:
                                height = int.Parse(data);
                                data = "";
                                break;
                            case 2:
                                width = int.Parse(data);
                                data = "";
                                break;
                            case 3:
                                data = "";
                                break;
                            case 4:
                                data = "";
                                break;
                        }
                    }
                    else
                    {
                        // Read in the level row
                        string[] column = data.Split('|');
                        foreach (string num in column)
                        {
                            // Match the data to hardcoded level to Game Object data
                            Vector2 assetPosition = CalculateLevelEditorPositions(width, height, yInput, xInput);
                            if (int.Parse(num) != -1 && int.Parse(num) < 7)
                            {
                                enemies.Add(new int[] { 1, int.Parse(num), (int)assetPosition.X, (int)assetPosition.Y, 1 });
                            }
                            if (int.Parse(num) == 9)
                            {
                                cameraStopPositions.Add(assetPosition);
                            }
                            xInput++;
                            boxIdentifier++;
                        }
                        xInput = 0;
                        yInput++;
                    }
                    lineNumber++;
                }
                ready = true;
            }
            while ((data = reader.ReadLine()) != null);

            // Close the file and return the enemies loaded
            reader.Close();
            return enemies;
        }

        /// <summary>
        ///  Resets the GameObject Instance
        /// </summary>
        public void Reset()
        {
            // Reset GameObject Data Storage
            allGameObjects = null;
            animations = null;
            colliders = null;
            transforms = null;
            positions = null;
            sprites = null;
            scales = null;
            objectEnums = null;
            effects = null;

            // Reset Information Access Tries
            animationsI = new GameObjectTrie<Animation>();
            collidersI = new GameObjectTrie<Collider>();
            transformsI = new GameObjectTrie<Rectangle>();
            positionsI = new GameObjectTrie<Vector2>();
            spritesI = new GameObjectTrie<Texture2D>();
            scalesI = new GameObjectTrie<float>();
            objectEnumsI = new GameObjectTrie<string>();
            effectI = new GameObjectTrie<Effect>();
            trie = null;

            // Reset static feilds
            bossEffectIsActive = false;
            flipSine = false;
            cameraStopPositions = null;
            clockTime = 0;
        }
    }
}