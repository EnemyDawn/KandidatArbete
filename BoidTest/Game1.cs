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

        int amountOfFish = 100;
        Boid[] boids;

        List<Feed> feed;
        List<Obst> obst;

        VariableSet[] sets;
        int currentSet = 0;
        Texture2D fill;
        TestPart testPart;

        Vector2 windowSize = new Vector2(1920, 1080);
        //Vector2 windowSize = new Vector2(200, 200);

        BitmapFont font;
        bool loopAround = false;

        public float keepDistance = 50;
        public float visibalDistance = 140;

        KeyboardState lastState;

        CompareVid compareVid;
        string resultString;

        bool[] donepart;
        bool showStats;

        #region 

        string genderChosen = "";
        Rectangle boyRec;
        Rectangle girlRec;

        int age = 18;

        Rectangle[]  experienceRecs;
        int chosenExperience = -1;

        #endregion

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
            
            this.showStats = true;

            testPart = TestPart.intro;

            LoadBoids();
            fill = new Texture2D(GraphicsDevice, 1, 1);
            fill.SetData(new Color[] { Color.White });

            this.feed = new List<Feed>();
            this.obst = new List<Obst>();

            donepart = new bool[3] { false, false, false };

            this.feed.Add(new Feed(Content, new Vector2(800.0f, 800.0f)));
            //this.feed.Add(new Feed(Content, new Vector2(200.0f, 400.0f)));

            this.obst.Add(new Obst(Content, new Vector2(100.0f, 100.0f)));

            this.font = Content.Load<BitmapFont>("BIG");

            int startPos = 400;
            experienceRecs = new Rectangle[5]
            {
                new Rectangle(startPos+50,700,100,200),
                new Rectangle(startPos+160,700,100,200),
                new Rectangle(startPos+270,700,100,200),
                new Rectangle(startPos+380,700,100,200),
                new Rectangle(startPos+490,700,100,200),
            };

            sets = new VariableSet[]
            {
                new VariableSet(20,200),
                new VariableSet(40,200),
                new VariableSet(60,200),
                new VariableSet(80,200),
                new VariableSet(100,200),

                new VariableSet(40,100),
                new VariableSet(40,70),
                new VariableSet(40,50),
            };

            boyRec = new Rectangle(300,140,100,60);
            girlRec = new Rectangle(1000, 140, 100, 60);

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
                Rectangle mouseRec = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y,1,1);

                if (mouseRec.Intersects(boyRec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                    genderChosen = "Boy";

                if (mouseRec.Intersects(girlRec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                    genderChosen = "Girl";

                if (Keyboard.GetState().IsKeyDown(Keys.E) && lastState != Keyboard.GetState())
                    age--;

                if (Keyboard.GetState().IsKeyDown(Keys.R) && lastState != Keyboard.GetState())
                    age++;

                for(int n = 0;n<experienceRecs.Length;n++)
                {
                    if (mouseRec.Intersects(experienceRecs[n]) && Mouse.GetState().LeftButton == ButtonState.Pressed)
                        chosenExperience = n;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && lastState != Keyboard.GetState()  && genderChosen != "" && chosenExperience != -1)
                {
                    chosenExperience++;
                    resultString += "Gender: " + genderChosen + "\n";
                    resultString += "Age: " + age + "\n";
                    resultString += "Media Experience: " + chosenExperience + "\n";

                    NextPart();
                }
            }
            else if (testPart == TestPart.freeMode)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Y) && keepDistance>0)
                    keepDistance--;
                if (Keyboard.GetState().IsKeyDown(Keys.U) && keepDistance < visibalDistance - 10)
                    keepDistance++;

                if (Keyboard.GetState().IsKeyDown(Keys.H) && keepDistance < visibalDistance - 10)
                    visibalDistance--;
                if (Keyboard.GetState().IsKeyDown(Keys.J) && visibalDistance<140)
                    visibalDistance++;

                //if (Keyboard.GetState().IsKeyDown(Keys.O) && lastState != Keyboard.GetState())
                //{
                //    if (currentSet > 0)
                //        currentSet--;
                //    SetCurrentSet();
                //}
                //if (Keyboard.GetState().IsKeyDown(Keys.P) && lastState != Keyboard.GetState())
                //{
                //    if (currentSet < sets.Length - 1)
                //        currentSet++;
                //    SetCurrentSet();
                //}

                //if (Keyboard.GetState().IsKeyDown(Keys.N) && amountOfFish > 2)
                //    amountOfFish--;
                   
                //if (Keyboard.GetState().IsKeyDown(Keys.M) && amountOfFish < 500)
                //    amountOfFish++;

                //if(Keyboard.GetState().IsKeyDown(Keys.R))
                //    LoadBoids();



                if (Keyboard.GetState().IsKeyDown(Keys.Tab) && lastState != Keyboard.GetState())
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
                    "Boids5Coh\\5coh50.Wmv",
                    "Boids5Coh\\5coh70.Wmv",
                    "Boids5Coh\\5coh100.Wmv",
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
                    "Boids50Coh\\50coh50.Wmv",
                    "Boids50Coh\\50coh70.Wmv",
                    "Boids50Coh\\50coh100.Wmv",
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
                    "Boids100Coh\\100coh50.Wmv",
                    "Boids100Coh\\100coh70.Wmv",
                    "Boids100Coh\\100coh100.Wmv",
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
                    spriteBatch.DrawString(font, "KeepDistance:" + keepDistance + " Y- U+", new Vector2(0, 0), Color.White);
                    spriteBatch.DrawString(font, "ViewDistance: " + visibalDistance + " H- J+", new Vector2(0, 60), Color.White);
                   // spriteBatch.DrawString(font, "Current Set:" + currentSet + " O- P+", new Vector2(0, 120), Color.White);
                    //spriteBatch.DrawString(font, "Fish Amount:" + amountOfFish + " N- M+", new Vector2(0, 300), Color.White);
                    
                    spriteBatch.DrawString(font, "Hide text with Tab", new Vector2(0, 180), Color.White);
                }
              

            }
            #endregion
            else if (testPart == TestPart.intro)
            {
               // spriteBatch.DrawString(font, introText, new Vector2(0, 0), Color.White);

                spriteBatch.DrawString(font, "Boy or Girl?", new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(font, "Boy", new Vector2(boyRec.X, boyRec.Y), Color.White);
                spriteBatch.DrawString(font, "Girl", new Vector2(girlRec.X, girlRec.Y), Color.White);

                if(genderChosen == "Boy")
                    spriteBatch.DrawString(font, "Boy", new Vector2(boyRec.X, boyRec.Y), Color.Red);
                else if (genderChosen == "Girl")
                    spriteBatch.DrawString(font, "Girl", new Vector2(girlRec.X, girlRec.Y), Color.Red);

                spriteBatch.DrawString(font, "Age?     Adjust with 'E' and 'R' ", new Vector2(0, 300), Color.White);


                spriteBatch.DrawString(font, "Age: " + age, new Vector2(300, 400), Color.White);

                spriteBatch.DrawString(font, "On a scale of 1-5, how experienced are you with games or digital media?", new Vector2(0, 500), Color.White);

                for(int n = 0;n< experienceRecs.Length;n++)
                {
                    if(chosenExperience == n)
                        spriteBatch.Draw(fill, experienceRecs[n], Color.Red);
                    else
                    spriteBatch.Draw(fill,experienceRecs[n], Color.White);
                    spriteBatch.DrawString(font, (n+1).ToString(), new Vector2(experienceRecs[n].X + 28, experienceRecs[n].Y + 70), Color.Black);

                }

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
                spriteBatch.DrawString(font, "Which school of fish size do you prefer?", new Vector2(0, 0), Color.White);

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
                    "Boids5Sep\\5sep60.Wmv",
                    "Boids50Sep\\50sep60.Wmv",
                    "Boids100Sep\\100sep60.Wmv",
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
                    "Boids5Sep\\5sep20.Wmv",
                    "Boids5Sep\\5sep40.Wmv",
                    "Boids5Sep\\5sep60.Wmv",
                    "Boids5Sep\\5sep80.Wmv",
                    "Boids5Sep\\5sep100.Wmv",
                };
                compareVid = new CompareVid(this, ds);
            }
            else if(activePart == 1)  /// Mid
            {
                
                testPart = TestPart.Seperation50;
                resultString += "Test Seperation 50 boids: ";
                string[] ds = new string[]
                {
                    "Boids50Sep\\50sep20.Wmv",
                    "Boids50Sep\\50sep40.Wmv",
                    "Boids50Sep\\50sep60.Wmv",
                    "Boids50Sep\\50sep80.Wmv",
                    "Boids50Sep\\50sep100.Wmv",
                };
                compareVid = new CompareVid(this, ds);

            }
            else if (activePart == 2)  // High
            {
                testPart = TestPart.Seperation100;

                resultString += "Test Seperation 100 boids: ";
                string[] ds = new string[]
                {
                    "Boids100Sep\\100sep20.Wmv",
                    "Boids100Sep\\100sep40.Wmv",
                    "Boids100Sep\\100sep60.Wmv",
                    "Boids100Sep\\100sep80.Wmv",
                    "Boids100Sep\\100sep100.Wmv",
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

        private void LoadBoids()
        {
            Random randum = new Random();

            boids = new Boid[amountOfFish];

            Vector2 startPos = new Vector2(200, 200);
            for (int n = 0; n < amountOfFish; n++)
            {
                boids[n] = new Boid(Content, windowSize, new Vector2(startPos.X, startPos.Y), randum);
            }
        }
    }
}
