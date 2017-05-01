using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using MonoGame.Extended.BitmapFonts;
namespace BoidTest
{
    struct VariableSet
    {
        public float keepDistance;
        public float visibalDistance;

        public VariableSet(float inKeep,float inVisiable)
        {
            this.keepDistance = inKeep;
            this.visibalDistance = inVisiable;
        }
    }

    enum TestPart
    {
        intro,
        Seperation15,
        Cohesion15,
        freeMode
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int amountOfFish = 5;
        Boid[] boids;

        List<Feed> feed;
        List<Obst> obst;

        VariableSet[] sets;
        int currentSet = 0;

        TestPart testPart;

        //Vector2 windowSize = new Vector2(900, 900);
        //Vector2 windowSize = new Vector2(200, 200);
        Vector2 windowSize = new Vector2(1280, 720);

        BitmapFont font;
        bool loopAround = false;

        public float keepDistance = 50;
        public float visibalDistance = 140;

        KeyboardState lastState;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)windowSize.X;
            graphics.PreferredBackBufferHeight = (int)windowSize.Y;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Random randum = new Random();

            testPart = TestPart.freeMode;

            boids = new Boid[amountOfFish];

            this.feed = new List<Feed>();
            this.obst = new List<Obst>();

            this.feed.Add(new Feed(Content, new Vector2(800.0f, 800.0f)));
            //this.feed.Add(new Feed(Content, new Vector2(200.0f, 400.0f)));

            this.obst.Add(new Obst(Content, new Vector2(100.0f, 100.0f)));

            this.font = Content.Load<BitmapFont>("BIG");

            Vector2 startPos = new Vector2(100, 100);
            for (int n = 0; n < amountOfFish; n++)
            {
                boids[n] = new Boid(Content, windowSize, new Vector2(startPos.X, startPos.Y), randum);
            }

            sets = new VariableSet[]
            {
                //new VariableSet(20,200),
                //new VariableSet(30,200),
                new VariableSet(40,200),
                //new VariableSet(50,200),
                //new VariableSet(60,200),
                //new VariableSet(70,200),
                //new VariableSet(80,200),
                //new VariableSet(90,200),
                //new VariableSet(100,200),

                //new VariableSet(40,100),
                //new VariableSet(40,90),
                //new VariableSet(40,80),
                //new VariableSet(40,70),
                //new VariableSet(40,60),
                //new VariableSet(40,52),
            };


            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (testPart == TestPart.intro)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && lastState != Keyboard.GetState())
                {
                    testPart = TestPart.freeMode;
                }
            }
            else if (testPart == TestPart.freeMode)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Y))
                    keepDistance--;
                if (Keyboard.GetState().IsKeyDown(Keys.U) && keepDistance < visibalDistance - 10)
                    keepDistance++;

                if (Keyboard.GetState().IsKeyDown(Keys.H) && keepDistance < visibalDistance - 10)
                    visibalDistance--;
                if (Keyboard.GetState().IsKeyDown(Keys.J))
                    visibalDistance++;

                if (Keyboard.GetState().IsKeyDown(Keys.O) && lastState != Keyboard.GetState())
                {
                    if (currentSet > 0)
                        currentSet--;
                    SetCurrentSet();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.P) && lastState != Keyboard.GetState())
                {
                    if (currentSet < sets.Length - 1)
                        currentSet++;
                    SetCurrentSet();
                }

                // TODO: Add your update logic here

                for (int i = 0; i < feed.Count; i++)
                    this.feed[i].Update(gameTime);

                for (int i = 0; i < obst.Count; i++)
                    this.obst[i].Update();

                for (int n = 0; n < boids.Length; n++)
                {
                    boids[n].Update(this.boids, this.feed, this.obst, gameTime, loopAround, keepDistance, visibalDistance);

                }


            }
            else
            {

            }

            lastState = Keyboard.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            #region FreeMode
            if (testPart == TestPart.freeMode)
            {
                for (int i = 0; i < feed.Count; i++)
                    this.feed[i].Draw(spriteBatch);

                for (int i = 0; i < this.obst.Count; i++)
                    this.obst[i].Draw(spriteBatch);


                for (int n = 0; n < boids.Length; n++)
                {
                    boids[n].Draw(spriteBatch);

                }

                //spriteBatch.DrawString(font, "KeepDistance:" + keepDistance, new Vector2(0, 0), Color.White);
                //spriteBatch.DrawString(font, "ViewDistance:" + visibalDistance, new Vector2(0, 60), Color.White);
                //spriteBatch.DrawString(font, "Current Set:" + currentSet, new Vector2(0, 120), Color.White);


                
            }
            #endregion
            else if (testPart == TestPart.intro)
            {
                spriteBatch.DrawString(font, "Text that explains he what this test \nis for", new Vector2(0, 0), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetCurrentSet()
        {
            keepDistance = sets[currentSet].keepDistance;
            visibalDistance = sets[currentSet].visibalDistance;
            Console.WriteLine("Visiable" + sets[currentSet].visibalDistance);
        }
    }
}
