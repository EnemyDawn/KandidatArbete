using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace BoidTest
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Boid> boids;
        List<Feed> feed;
        List<Obst> obst;

        Vector2 windowSize = new Vector2(1920,720);
        //Vector2 windowSize = new Vector2(200, 200);


        bool loopAround = false;

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

            this.boids = new List<Boid>();
            this.feed = new List<Feed>();
            this.obst = new List<Obst>();

            //this.feed.Add(new Feed(Content, new Vector2(200.0f, 200.0f)));
            //this.feed.Add(new Feed(Content, new Vector2(200.0f, 400.0f)));

            this.obst.Add(new Obst(Content, new Vector2(100.0f, 100.0f)));


            for (int n = 0;n< 250;n++)
                boids.Add(new Boid(Content,windowSize, randum));


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

            // TODO: Add your update logic here

            for (int i = 0; i < feed.Count; i++)
                this.feed[i].Update();

            for (int i = 0; i < obst.Count; i++)
                this.obst[i].Update();

            for (int n = 0;n<boids.Count;n++)
             {
                 boids[n].Update(this.boids, this.feed, this.obst, gameTime, loopAround);
             }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            for (int i = 0; i < feed.Count; i++)
                this.feed[i].Draw(spriteBatch);

            for (int i = 0; i < this.obst.Count; i++)
                this.obst[i].Draw(spriteBatch);

            for (int n = 0; n < boids.Count; n++)
            {
                boids[n].Draw(spriteBatch);
            }

            

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
