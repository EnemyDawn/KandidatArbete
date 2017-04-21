﻿using System;
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
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Boid> boids;

        List<Boid> boids2;
        List<Boid> boids3;
        List<Boid> boids4;
        List<Feed> feed;
        List<Obst> obst;

        VariableSet[] sets;
        int currentSet=0;

        Vector2 windowSize = new Vector2(1920,720);
        //Vector2 windowSize = new Vector2(200, 200);

        BitmapFont font;
        bool loopAround = false;

        public float keepDistance = 100;
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

            this.boids = new List<Boid>();
            this.boids2 = new List<Boid>();

            this.boids3 = new List<Boid>();
            this.boids4 = new List<Boid>();

            this.feed = new List<Feed>();
            this.obst = new List<Obst>();

            //this.feed.Add(new Feed(Content, new Vector2(200.0f, 200.0f)));
            //this.feed.Add(new Feed(Content, new Vector2(200.0f, 400.0f)));

            this.obst.Add(new Obst(Content, new Vector2(100.0f, 100.0f)));

            this.font = Content.Load<BitmapFont>("BIG");

            for (int n = 0; n < 200; n++)
            {
                boids.Add(new Boid(Content, windowSize, randum));

                boids2.Add(new Boid(Content, windowSize, randum));

                boids3.Add(new Boid(Content, windowSize, randum));
                boids4.Add(new Boid(Content, windowSize, randum));
            }

            sets = new VariableSet[]
            {
                new VariableSet(100,140),
                new VariableSet(44,20),
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

            if (Keyboard.GetState().IsKeyDown(Keys.Y))
                keepDistance--;
            if (Keyboard.GetState().IsKeyDown(Keys.U))
                keepDistance++;

            if (Keyboard.GetState().IsKeyDown(Keys.H))
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
                if (currentSet < sets.Length-1)
                    currentSet++;
                SetCurrentSet();
            }

            // TODO: Add your update logic here

            for (int i = 0; i < feed.Count; i++)
                this.feed[i].Update();

            for (int i = 0; i < obst.Count; i++)
                this.obst[i].Update();

            for (int n = 0;n<boids.Count;n++)
             {
                 boids[n].Update(this.boids, this.feed, this.obst, gameTime, loopAround,keepDistance, visibalDistance);
                boids2[n].Update(this.boids2, this.feed, this.obst, gameTime, loopAround, keepDistance, visibalDistance);
                boids3[n].Update(this.boids2, this.feed, this.obst, gameTime, loopAround, keepDistance, visibalDistance);
                boids4[n].Update(this.boids2, this.feed, this.obst, gameTime, loopAround, keepDistance, visibalDistance);
            }

            lastState = Keyboard.GetState();
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
                boids2[n].Draw(spriteBatch);
                boids3[n].Draw(spriteBatch);
                boids4[n].Draw(spriteBatch);
            }

            spriteBatch.DrawString(font, "KeepDistance:" + keepDistance, new Vector2(0,0),Color.White);
            spriteBatch.DrawString(font, "ViewDistance:" + visibalDistance, new Vector2(0, 60), Color.White);
            spriteBatch.DrawString(font, "Current Set:" + currentSet, new Vector2(0, 120), Color.White);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetCurrentSet()
        {
            keepDistance = sets[currentSet].keepDistance;
            visibalDistance = sets[currentSet].visibalDistance;
        }
    }
}
