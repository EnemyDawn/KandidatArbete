using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        Seperation5,
        Cohesion5,
        Seperation50,
        Cohesion50,
        Seperation100,
        Cohesion100,

        nrOfTest,
        freeMode,
        testDone
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

        Vector2 windowSize = new Vector2(1280, 720);
        //Vector2 windowSize = new Vector2(200, 200);

        BitmapFont font;
        bool loopAround = false;

        public float keepDistance = 50;
        public float visibalDistance = 140;

        KeyboardState lastState;

        CompareVid compareVid;
        string resultString;

        string introText;

        bool[] donepart;
        bool showStats;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (int)windowSize.X;
            graphics.PreferredBackBufferHeight = (int)windowSize.Y;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Random randum = new Random();
            this.showStats = false;

            testPart = TestPart.freeMode;

            boids = new Boid[amountOfFish];

            this.feed = new List<Feed>();
            this.obst = new List<Obst>();

            donepart = new bool[3] { false, false, false };

            this.feed.Add(new Feed(Content, new Vector2(800.0f, 800.0f)));
            //this.feed.Add(new Feed(Content, new Vector2(200.0f, 400.0f)));

            this.obst.Add(new Obst(Content, new Vector2(100.0f, 100.0f)));

            this.font = Content.Load<BitmapFont>("BIG");

            Vector2 startPos = new Vector2(200, 200);
            for (int n = 0; n < amountOfFish; n++)
            {
                boids[n] = new Boid(Content, windowSize, new Vector2(startPos.X, startPos.Y), randum);
            }

            sets = new VariableSet[]
            {
                new VariableSet(20,200),
                new VariableSet(40,200),
                new VariableSet(60,200),
                new VariableSet(80,200),
                new VariableSet(100,200),

                //new VariableSet(40,100),
                //new VariableSet(40,90),
                //new VariableSet(40,80),
                //new VariableSet(40,70),
                //new VariableSet(40,60),
                //new VariableSet(40,52),
            };

            introText = "In this case study we study the boid algorithm and try to make is as \nrealistic as possible."+
                        "In this form you will \nbe shown multiple instances of a flock and at the same time \n" +
                        "choose witch one of the videos you find the most realistic.\n" +
                        "Please understand that you can quit the test at anytime if you wish,\nif so; no personal data will be saved.\n" +

                       "Thank you for taking the time to participate in this study.\n" +
                       "Best wishes\n" +
                        "Sebastian Lundgren\n" +
                        "Max Larsson\n";

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
                    NextPart();
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
                if(Keyboard.GetState().IsKeyDown(Keys.Tab) && lastState != Keyboard.GetState())
                {
                    this.showStats = !this.showStats;
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

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && lastState != Keyboard.GetState())
                {
                    resultString += "Freeform KeepDistance: "+ keepDistance +"\n";
                    resultString += "Freeform ViewDistance: " + visibalDistance + "\n";
                    testPart = TestPart.testDone;
                }

            }
            else if (testPart == TestPart.Seperation5)
            {
                string tempString = compareVid.Update();
                if (tempString != "")
                {
                    resultString += tempString + "\n";
                    testPart = TestPart.Cohesion5;

                    UnloadVideo();

                    resultString += "Test Cohesion 5 boids: ";
                    //CHANGE THIS LATER
                    string[] ds = new string[]
                    {
                    "Boids50Sep\\CS_20.Wmv",
                    "Boids50Sep\\CS_40.Wmv",
                    "Boids50Sep\\CS_60.Wmv",
                    "Boids50Sep\\CS_80.Wmv",
                    "Boids50Sep\\CS_100.Wmv",
                    };
                    compareVid = new CompareVid(this, ds);
                }
            }
            else if (testPart == TestPart.Cohesion5)
            {
                string tempString = compareVid.Update();
                if (tempString != "")
                {
                    resultString += tempString + "\n";
                    UnloadVideo();
                    NextPart();
                }
            }
            else if(testPart == TestPart.Seperation50)
            {
                string tempString = compareVid.Update();
                if(tempString != "")
                {
                    resultString += tempString + "\n";
                    testPart = TestPart.Cohesion50;

                    UnloadVideo();

                    resultString += "Test Cohesion 50 boids: ";
                    //CHANGE THIS LATER
                    string[] ds = new string[]
                    {
                    "Boids50Sep\\CS_20.Wmv",
                    "Boids50Sep\\CS_40.Wmv",
                    "Boids50Sep\\CS_60.Wmv",
                    "Boids50Sep\\CS_80.Wmv",
                    "Boids50Sep\\CS_100.Wmv",
                    };
                    compareVid = new CompareVid(this, ds);
                }
            }
            else if (testPart == TestPart.Cohesion50)
            {
                string tempString = compareVid.Update();
                if (tempString != "")
                {
                    resultString += tempString + "\n";
                    UnloadVideo();
                    NextPart();
                }
            }
            else if (testPart == TestPart.Seperation100)
            {
                string tempString = compareVid.Update();
                if (tempString != "")
                {
                    resultString += tempString + "\n";

                    UnloadVideo();

                    testPart = TestPart.Cohesion100;

                    resultString += "Test Cohesion 100 boids: ";
                    //CHANGE THIS LATER
                    string[] ds = new string[]
                    {
                    "Boids50Sep\\CS_20.Wmv",
                    "Boids50Sep\\CS_40.Wmv",
                    "Boids50Sep\\CS_60.Wmv",
                    "Boids50Sep\\CS_80.Wmv",
                    "Boids50Sep\\CS_100.Wmv",
                    };
                    compareVid = new CompareVid(this, ds);
                }
            }
            else if (testPart == TestPart.Cohesion100)
            {
                string tempString = compareVid.Update();
                if (tempString != "")
                {
                    resultString += tempString + "\n";
                    UnloadVideo();
                    NextPart();
                }
            }
            else if (testPart == TestPart.nrOfTest)
            {
                string tempString = compareVid.Update();
                if (tempString != "")
                {
                    resultString += tempString + "\n";
                    UnloadVideo();
                    testPart = TestPart.freeMode;
                }
            }
            else if (testPart == TestPart.testDone)
            {
                SaveResult();
                this.Exit();
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

                if(this.showStats == true)
                {
                    spriteBatch.DrawString(font, "KeepDistance:" + keepDistance, new Vector2(0, 0), Color.White);
                    spriteBatch.DrawString(font, "ViewDistance:" + visibalDistance, new Vector2(0, 60), Color.White);
                    spriteBatch.DrawString(font, "Current Set:" + currentSet, new Vector2(0, 120), Color.White);
                }


            }
            #endregion
            else if (testPart == TestPart.intro)
            {
                spriteBatch.DrawString(font, introText, new Vector2(0, 0), Color.White);

            }
            else if (testPart == TestPart.Seperation5)
            {
                spriteBatch.DrawString(font, "Please click on the school of fish that looks more realistic? ", new Vector2(0, 0), Color.White);

                compareVid.Draw(spriteBatch);
            }
            else if (testPart == TestPart.Cohesion5)
            {
                spriteBatch.DrawString(font, "Which school of fish reaction to the predator is the most realistic?", new Vector2(0, 0), Color.White);

                compareVid.Draw(spriteBatch);
            }
            else if(testPart == TestPart.Seperation50)
            {
                spriteBatch.DrawString(font, "Please click on the school of fish that looks more realistic? ", new Vector2(0, 0), Color.White);

                compareVid.Draw(spriteBatch);
            }
            else if (testPart == TestPart.Cohesion50)
            {
                spriteBatch.DrawString(font, "Which school of fish reaction to the predator is the most realistic?", new Vector2(0, 0), Color.White);

                compareVid.Draw(spriteBatch);
            }
            else if (testPart == TestPart.Seperation100)
            {
                spriteBatch.DrawString(font, "Please click on the school of fish that looks more realistic? ", new Vector2(0, 0), Color.White);
                compareVid.Draw(spriteBatch);
            }
            else if (testPart == TestPart.Cohesion100)
            {
                spriteBatch.DrawString(font, "Which school of fish reaction to the predator is the most realistic?", new Vector2(0, 0), Color.White);

                compareVid.Draw(spriteBatch);
            }
            else if (testPart == TestPart.nrOfTest)
            {
                spriteBatch.DrawString(font, "Which school of fish do you prefer?", new Vector2(0, 0), Color.White);

                compareVid.Draw(spriteBatch);
            }



            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SaveResult()
        {
            Random rand = new Random();

            string filename = "TestResult\\Test" + rand.Next(0,100) +"-"+rand.Next(0,300);//= setFileName;

            StreamWriter sWriter = new StreamWriter(filename);

            sWriter.Write(resultString);

            sWriter.Close();
        }

        private void UnloadVideo()
        {
            if (compareVid != null)
                compareVid.Unload();
            compareVid = null;
        }

        private void NextPart()
        {
            bool done = true;
           for(int n = 0;n<3;n++)
            {
                if (!donepart[n])
                    done = false;
            }
            if (done)
            {
                testPart = TestPart.nrOfTest;

                resultString += "Test Preferred NrOF: ";
                ////////////CHANGE
                string[] ds = new string[]
                {
                    "Boids50Sep\\CS_40.Wmv",
                    "Boids50Sep\\CS_40.Wmv",
                    "Boids100Sep\\CS_40.Wmv",
                };
                compareVid = new CompareVid(this, ds);

                return;
            }

            Random rand = new Random();
            int activePart = rand.Next(0, 3);

            while(donepart[activePart])
            {
                activePart = rand.Next(0, 3);
            }
            donepart[activePart] = true;
            if (activePart == 0) /// low
            {
                testPart = TestPart.Seperation5;
                resultString += "Test Seperation 5 boids: ";
                ////////////CHANGE
                string[] ds = new string[]
                {
                    "Boids50Sep\\CS_20.Wmv",
                    "Boids50Sep\\CS_40.Wmv",
                    "Boids50Sep\\CS_60.Wmv",
                    "Boids50Sep\\CS_80.Wmv",
                    "Boids50Sep\\CS_100.Wmv",
                };
                compareVid = new CompareVid(this, ds);
            }
            else if(activePart == 1)  /// Mid
            {
                
                testPart = TestPart.Seperation50;
                resultString += "Test Seperation 50 boids: ";
                string[] ds = new string[]
                {
                    "Boids50Sep\\CS_20.Wmv",
                    "Boids50Sep\\CS_40.Wmv",
                    "Boids50Sep\\CS_60.Wmv",
                    "Boids50Sep\\CS_80.Wmv",
                    "Boids50Sep\\CS_100.Wmv",
                };
                compareVid = new CompareVid(this, ds);

            }
            else if (activePart == 2)  // High
            {
                testPart = TestPart.Seperation100;

                resultString += "Test Seperation 100 boids: ";
                string[] ds = new string[]
                {
                    "Boids100Sep\\CS_20.Wmv",
                    "Boids100Sep\\CS_40.Wmv",
                    "Boids100Sep\\CS_60.Wmv",
                    "Boids100Sep\\CS_80.Wmv",
                    "Boids100Sep\\CS_100.Wmv",
                };
                compareVid = new CompareVid(this, ds);
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if(compareVid != null)
                compareVid.Unload();

            base.OnExiting(sender, args);
        }

        private void SetCurrentSet()
        {
            keepDistance = sets[currentSet].keepDistance;
            visibalDistance = sets[currentSet].visibalDistance;
            Console.WriteLine("Visiable" + sets[currentSet].visibalDistance);
        }
    }
}
