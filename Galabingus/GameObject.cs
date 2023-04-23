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
// 2023, 3, 13
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
//
// GameObject.Instance - All of the instances of GameObject
// GameObject.Instance.Index - The current GameObject
// GameObject.Instance.Content - Dynamic required for creating content on the fly
// GameObject.Instance.GraphicsDevice - a GraphicsDevice
// GameObject.Instance.SpriteBatch - a SpriteBatch
// GameObject.Instance[Animation.Empty] - animation array of the current Instance
// GameObject.Instance[Collider.Empty] - collider array of the current Instance
// GameObject.Instance[Vector2.Zero] - position array of the current Instance
// GameObject.Instance[Rectangle.Empty] - transform array of the current Instance
// GameObject.Instance[i, Rectangle.Empty] - transform at i of the current Instance
// GameObject.Instance[i, Vector2.Zero] - position at i of the current Instance
// GameObject.Instance[i, Animation.Empty] - animation at i of the current Instance
// GameObject.Instance[i, Collider.Empty] - collider at i of the current Instance
// GameObject.Sprite - sprite of the current Instance
// GameObject.Scale - scale of the sprite of the current Instance
// GameObject.Colliders - All instances collider arrays <!> Warning <!>

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
        private static float universalScale;
        private static float redShade;
        private static float timeShadeEffect;
        private static bool flipSine;
        private static bool bossEffectIsActive;

        public struct GameObjectTrie<T>
        {
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

        public GameObject GetInstance<T>()
        {
            //Debug.WriteLine(CollisionGroupIGet(index,instance));
            //if (this is T)
            //{
            //    return (T)Convert.ChangeType(this, typeof(T));
            //}
            //else
            //{

            if (typeof(T) == typeof(Player))
            {
                if (CollisionGroup.Player == CollisionGroupIGet(GameObject.Instance.Index, GameObject.Instance.instance))
                {
                    // return ToPlayer();
                }
                else
                {
                    return GameObject.Instance;
                }
            }
            if (typeof(T) == typeof(Tile))
            {
                if (CollisionGroup.Tile == CollisionGroupIGet(GameObject.Instance.Index, GameObject.Instance.instance))
                {
                    //return ToTile();
                }
                else
                {
                    return GameObject.Instance;
                }
            }
            if (typeof(T) == typeof(Bullet))
            {
                if (CollisionGroup.Bullet == CollisionGroupIGet(GameObject.Instance.Index, GameObject.Instance.instance))
                {
                    //return ToBullet();
                }
                else
                {
                    return GameObject.Instance;
                }
            }

            return GameObject.Instance;
            //}
        }

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

        public static Vector2 EndPosition
        {
            get
            {
                return new Vector2(0, GameObject.Instance.GraphicsDevice.Viewport.Height * -4);
            }
        }
        
        public Texture2D GetSprite(ushort instancePass)
        {
#nullable disable
            unsafe
            {
                return (spritesI).GetPass(spritesConst, instancePass) as Texture2D;
            }
#nullable enable
        }

        public float GetScale(ushort instancePass)
        {
#nullable disable
            unsafe
            {
                return (float)(scalesI).GetPass(scalesConst, instancePass);
            }
#nullable enable
        }

        public string GetObjectEnum(ushort instancePass)
        {
#nullable disable
            unsafe
            {
                return (objectEnumsI).GetPass(objectEnumsConst, instancePass) as string;
            }
#nullable enable
        }

        public Animation GetAnimation(ushort instancePass)
        {
#nullable disable
            unsafe
            {
                return (animationsI).GetPass(animationConst, instancePass) as Animation;
            }
#nullable enable
        }

        public Collider GetCollider(ushort instancePass)
        {
#nullable disable
            unsafe
            {
                return (collidersI).GetPass(colliderConst, instancePass) as Collider;
            }
#nullable enable
        }

        public Rectangle GetTransform(ushort instancePass)
        {
#nullable disable
            unsafe
            {
                return (Rectangle)(transformsI).GetPass(transformConst, instancePass);
            }
#nullable enable
        }

        public Vector2 GetPosition(ushort instancePass)
        {
#nullable disable
            unsafe
            {
                return (Vector2)(positionsI).GetPass(positionConst, instancePass);
            }
#nullable enable
        }

        public Effect GetEffect(ushort instancePass)
        {
#nullable disable
            unsafe
            {
                return (Effect)(effectI).GetPass(effectConst, instancePass);
            }
#nullable enable
        }

        public void SetSprite(ushort instancePass, object value)
        {
#nullable disable
            unsafe
            {
                (spritesI).SetPass(spritesConst, instancePass, value);
            }
#nullable enable
        }

        public void SetScale(ushort instancePass, object value)
        {
#nullable disable
            unsafe
            {
                (scalesI).SetPass(scalesConst, instancePass, value);
            }
#nullable enable
        }

        public void SetObjectEnum(ushort instancePass, object value)
        {
#nullable disable
            unsafe
            {
                (objectEnumsI).SetPass(objectEnumsConst, instancePass, value);
            }
#nullable enable
        }

        public void SetAnimation(ushort instancePass, object value)
        {
#nullable disable
            unsafe
            {
                (animationsI).SetPass(animationConst, instancePass, value);
            }
#nullable enable
        }

        public void SetCollider(ushort instancePass, object value)
        {
#nullable disable
            unsafe
            {
                (collidersI).SetPass(colliderConst, instancePass, value);
            }
#nullable enable
        }

        public void SetTransform(ushort instancePass, object value)
        {
#nullable disable
            unsafe
            {
                (transformsI).SetPass(transformConst, instancePass, value);
            }
#nullable enable
        }

        public void SetPosition(ushort instancePass, object value)
        {
#nullable disable
            unsafe
            {
                (positionsI).SetPass(positionConst, instancePass, value);
            }
#nullable enable
        }

        public void SetEffect(ushort instancePass, object value)
        {
#nullable disable
            unsafe
            {
                (effectI).SetPass(effectConst, instancePass, value);
            }
#nullable enable
        }

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


        public ref List<Collider> ColliderCollisions()
        {
            return ref colliders;
        }

        public List<ushort> ColliderLayer4()
        {
            return GameObjectTrie<Collider>.GetLayer4(colliderConst, colliders);
        }

        public ushort ColliderLayer4Instance(ushort instanceNumber)
        {
            return GameObjectTrie<Collider>.GetLayer4Instance(colliderConst, instanceNumber, colliders);
        }

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

        public void DeleteCollider(ushort instanceNumber)
        {
            (collidersI).SetPass(colliderConst, instanceNumber, default(Collider));
        }

        public void DeleteSprite(ushort instanceNumber)
        {
            (spritesI).SetPass(spritesConst, instanceNumber, default(Texture2D));
        }

        public void DeleteScale(ushort instanceNumber)
        {
            (scalesI).SetPass(scalesConst, instanceNumber, default(float));
        }

        public void DeleteEnum(ushort instanceNumber)
        {
            (objectEnumsI).SetPass(objectEnumsConst, instanceNumber, default(string));
        }

        public void DeleteAnimation(ushort instanceNumber)
        {
            (animationsI).SetPass(animationConst, instanceNumber, default(Animation));
        }

        public void DeleteTransform(ushort instanceNumber)
        {
            (transformsI).SetPass(transformConst, instanceNumber, default(Rectangle));
        }

        public void DeletePosition(ushort instanceNumber)
        {
            (positionsI).SetPass(positionConst, instanceNumber, default(Vector2));
        }

        public void DeleteEffect(ushort instanceNumber)
        {
            (effectI).SetPass(effectConst, instanceNumber, default(Effect));
        }

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


        public ushort PositionLength
        {
            get
            {
                return (ushort)positions.Count;
            }
        }

        public bool IsBossEffectActive
        {
            get
            {
                return bossEffectIsActive;
            }
        }

        public float TimeShade
        {
            get
            {
                return timeShadeEffect;
            }
        }

        public void StartBossEffect()
        {
            bossEffectIsActive = true;
        }

        public void StopBossEffect()
        {
            bossEffectIsActive = false;
        }


        public void PlayBossEffect()
        {
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

            //System.Diagnostics.Debug.WriteLine(timeShadeEffect);
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
                    //GameObject.Instance.Content = index;
                    return exist;
                }
                index++;
            }

            // When teh property does not exist create it
            if (!exist)
            {
                exist = true;
                GameObject.ObjectEnumsI.Add(binder.Name);
                GameObject.ObjectEnumsI = GameObject.ObjectEnumsI;
            }
            this.index = index;
            result = index;
            //GameObject.Instance.Content = index;
            return exist;
        }

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
            //this.index = (ushort)(contentName + instanceNumber);

            CollisionGroupISet(contentName, instanceNumber, collisionGroup);
            this.collisionGroup = collisionGroup;
            //instance = instanceNumber;
            string path = GameObject.ObjectEnumsI[contentName];
            string start = "../../../Content";
            string[] files = Directory.GetFiles(start, path + ".*", SearchOption.AllDirectories);
            files[0] = files[0].Replace(start, "");
            files[0] = files[0].Replace("\\", "/");
            files[0] = files[0].Substring(1);
            files[0] = files[0].Substring(0, files[0].LastIndexOf('.'));
            ushort strip = ushort.Parse(path.Split("strip")[1]);
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
            Collider newCollider = new Collider(GetSprite(instanceNumber), GetPosition(instanceNumber), GetTransform(instanceNumber), SpriteEffects.None, GetScale(instanceNumber), GraphicsDevice, SpriteBatch, (ushort)collisionGroup, this);
            newCollider.Layer = contentName;
            newCollider.Resolved = true;
            newCollider.self = this;
            SetCollider(instanceNumber, newCollider);
        }

        public float PostScaleRatio()
        {
            //System.Diagnostics.Debug.WriteLine("EEEEAA " + this.GetTransform(instance).Width);
            //System.Diagnostics.Debug.WriteLine("FFFEEEE " + universalScale);
            return ((this.GetTransform(instance).Width < this.GetTransform(instance).Height) ? universalScale / this.GetTransform(instance).Width * GameObject.Instance.GraphicsDevice.Viewport.Width / GameObject.Instance.GraphicsDevice.Viewport.Height : universalScale / this.GetTransform(instance).Height * GameObject.Instance.GraphicsDevice.Viewport.Height / GameObject.Instance.GraphicsDevice.Viewport.Width);
        }

        public Vector2 PostScaleRatio(bool isVector2)
        {
            return new Vector2(
                universalScale / this.GetTransform(instance).Width * GameObject.Instance.GraphicsDevice.Viewport.Width / GameObject.Instance.GraphicsDevice.Viewport.Height,
                universalScale / this.GetTransform(instance).Height * GameObject.Instance.GraphicsDevice.Viewport.Height / GameObject.Instance.GraphicsDevice.Viewport.Width
            );
        }

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
        /// <returns></returns>
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
            //shade = false;
            flipSine = false;
            cameraStopPositions = new List<Vector2>();
            flipSine = false;
            return new GameObject();
        }

        private CollisionGroup CollisionGroupIGet(ushort contentName, ushort instance)
        {
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

        private CollisionGroup CollisionGroupISet(ushort contentName, ushort instance, CollisionGroup value)
        {
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

        public Rectangle Transform
        {
            get
            {
                return GetTransform(instance);
            }
        }

        public Vector2 Position
        {
            get
            {
                return GetPosition(instance);
            }
        }

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
            //throw new NotImplementedException();
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

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            if (Debug != null)
            {
                Debug(spriteBatch);
                Debug = null;
            }
        }

        public ContentManager ContentManager
        {
            get
            {
                return contentManager;
            }
        }

        public List<Vector2> GetCameraStopPositions()
        {
            return GameObject.cameraStopPositions;
        }

        public void CameraStopRemoveAt(int index)
        {
            List<Vector2> preI = new List<Vector2>();
            List<Vector2> postI = new List<Vector2>();
            List<Vector2> result = new List<Vector2>();
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

        public Vector2 CalculateLevelEditorPositions(int width, int height, int row, int column)
        {
            float coordianteXScale = GameObject.Instance.GraphicsDevice.Viewport.Width / width;
            universalScale = coordianteXScale;
            float coordinateYScale = -EndPosition.Y / height * GameObject.Instance.GraphicsDevice.Viewport.Height / GameObject.Instance.GraphicsDevice.Viewport.Width;
            float startingY = EndPosition.Y + GameObject.Instance.GraphicsDevice.Viewport.Height;
            return new Vector2(coordianteXScale * column, coordinateYScale * row + startingY);
        }

        public void LoadTileLevelFile(string fileName)
        {
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
                //Debug.WriteLine(data);

                if (lineNumber < 6)
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
                    string[] column = data.Split('|');

                    //System.Diagnostics.Debug.WriteLine(height);

                    foreach (string num in column)
                    {
                        Vector2 assetPosition = CalculateLevelEditorPositions(width, height, yInput, xInput);

                        if (int.Parse(num) != -1 && int.Parse(num) > 9)
                        {
                            //System.Diagnostics.Debug.WriteLine(assetPosition);
                            //TileManager.Instance.CreateObject(GameObject.Instance.Content.smallbullet_strip4, Vector2.Zero);
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

            reader.Close();
        }

        public void TriggerBossEffect()
        {

        }


        public List<int[]> LoadEnemyLeveFile(string fileName)
        {
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
                //Debug.WriteLine(data);
                if (ready)
                {
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
                        string[] column = data.Split('|');
                        //System.Diagnostics.Debug.WriteLine(data);
                        foreach (string num in column)
                        {
                            Vector2 assetPosition = CalculateLevelEditorPositions(width, height, yInput, xInput);
                            //System.Diagnostics.Debug.WriteLine(assetPosition);
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

            reader.Close();

            return enemies;
        }

        public void Reset()
        {
            allGameObjects = null;
            animations = null;
            colliders = null;
            transforms = null;
            positions = null;
            sprites = null;
            scales = null;
            objectEnums = null;
            effects = null;

            animationsI = new GameObjectTrie<Animation>();
            collidersI = new GameObjectTrie<Collider>();
            transformsI = new GameObjectTrie<Rectangle>();
            positionsI = new GameObjectTrie<Vector2>();
            spritesI = new GameObjectTrie<Texture2D>();
            scalesI = new GameObjectTrie<float>();
            objectEnumsI = new GameObjectTrie<string>();
            effectI = new GameObjectTrie<Effect>();
            trie = null;

            cameraStopPositions = null;
        }
    }
}