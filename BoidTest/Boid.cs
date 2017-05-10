using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace BoidTest
{
    public class Boid
    {
        Texture2D[] fishTex;
       
        float speed =100.0f;
        float size = 0.2f;


        Vector2[] pos;
        Vector2 dir;

        Vector2 windowSize;
        Color color;

        float roationInc = 0.01f;

        public Boid(ContentManager content,Vector2 windowSize,Vector2 posIn,Random randum)
        {
            this.windowSize = windowSize;

            //apply random postitions in the game
            //speed = randum.Next(50,130);
            dir = new Vector2(1,1);
            pos = new Vector2[61];
            pos[0] = posIn;// new Vector2(randum.Next(100, 120),randum.Next(100, 120));
            dir.Normalize();
            for (int n = 1; n < pos.Length; n++)
            {
                pos[n] = pos[0];// + (dir * (1 * n));
            }

            color = new Color(randum.Next(150,255), randum.Next(150, 255), randum.Next(50, 60));

            //size = randum.Next(5, 10) * 0.01f;// 0.08f;
            fishTex = new Texture2D[]
            {
                content.Load<Texture2D>("Fish\\head"),
                content.Load<Texture2D>("Fish\\b1"),
                content.Load<Texture2D>("Fish\\b2"),
                content.Load<Texture2D>("Fish\\b3"),
                content.Load<Texture2D>("Fish\\b4"),
                content.Load<Texture2D>("Fish\\tail"),
            };
                
        }

        public void Update(Boid[] boids, List<Feed> feeds, List<Obst> obst,GameTime gameTime,bool loopAround,float keepDistance,float viewDistance)
        {
            //this function satisfy the separation, alignment and cohation roles
            //with is the rules of the boid algorithm

            if (Keyboard.GetState().IsKeyDown(Keys.T))
                roationInc += 0.1f;

            this.FollowingFeed(feeds, 1000);

            this.BoidsFirstRules(boids, false, keepDistance, viewDistance);

            //this is an extention of the boid, which makes the fish attracted to
            //to a point in the game, called a feed
            
            //this.avoidingObst(obst);
            this.AvoidEnemyBoids(obst);


            //simple function that moves the boids back to the screen
            this.ReinitializeBoidPosition(loopAround);

            //the direction should be normailized to maintain speed reliability
            dir.Normalize();
            pos[0] += (dir * speed) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int n = pos.Length-1; n > 0; n--)
            {
                pos[n] = pos[n-1];
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = (float)(Math.Atan2(dir.Y, dir.X));// / (2 * Math.PI));
            Vector2 rotToFront = new Vector2();
            int countDown = 50;
            Vector2 savePos = pos[1];

            for (int n = fishTex.Length - 1; n >= 1; n--)
            {
                rotToFront = pos[countDown - 10] -pos[countDown];
                //rotToFront.Normalize();
                
                rotation = (float)(Math.Atan2(rotToFront.Y, rotToFront.X));// / (2 * Math.PI));

                rotation += MathHelper.ToRadians(-90);

                spriteBatch.Draw(fishTex[n], savePos - (rotToFront * 1), new Rectangle(0, 0, fishTex[n].Width, fishTex[n].Height), new Color(color.R + (n * 2), color.G + (n * 2), color.B + (n * 2)),
                    rotation,
                    new Vector2(fishTex[n].Width / 2, (fishTex[n].Height)),
                    size,
                    SpriteEffects.None, 1
                    );
                savePos = pos[countDown];

                countDown -= 10;
            }
            rotToFront = pos[0] - pos[10];
            rotation = (float)(Math.Atan2(rotToFront.Y, rotToFront.X));// / (2 * Math.PI));
            rotation += MathHelper.ToRadians(-90);

            spriteBatch.Draw(fishTex[0], pos[1], new Rectangle(0, 0, fishTex[0].Width, fishTex[0].Height), new Color(color.R + (0 * 2), color.G + (0 * 2), color.B + (0 * 2)),
                    rotation,
                    new Vector2(fishTex[0].Width / 2, (fishTex[0].Height)),
                    size,
                    SpriteEffects.None, 1
                    );
        }

        private void BoidsFirstRules(Boid[] boids,bool loopAround,float keepDistance,float visibalDistance)
        {
            Vector2 averageSepForce = new Vector2(0.0f, 0.0f);
            Vector2 newAveragePosition = new Vector2(0.0f, 0.0f);
            Vector2 averageDirection = new Vector2(0.0f, 0.0f);
            int boidsInVisibalDistance = 0;
            int boidsKeepDistance = 0;

            for (int n = 0; n < boids.Length; n++)
            {
                if (boids[n] != this)
                {
                    Vector2 boidVec = (boids[n].pos[0] - pos[0]);
                    if ((boidVec.Length() < visibalDistance))
                    {
                        //calculate avg data for Alignment and Cohation
                        newAveragePosition += boids[n].pos[0];

                        averageDirection += boids[n].dir;

                        boidsInVisibalDistance++;

                    }
                    if (boidVec.Length() < keepDistance)
                    {
                        //Separation, the closer to a flockmate, the more they are repelled
                        boidVec = ((boidVec.Length() / keepDistance) - 1) * (boidVec / boidVec.Length());

                        if (float.IsNaN(boidVec.X))
                            boidVec = new Vector2(-1 + (n * 0.1f), 0);
                        if (float.IsNaN(boidVec.Y))
                            boidVec = new Vector2(0, -1 + (n * 0.1f));

                        averageSepForce += boidVec;// * cordilate;
                        boidsKeepDistance++;
                    }

                }
            }
            


            //Adjust boid to follow the flocks average position, cohation 

            if (boidsInVisibalDistance > 0)
            {
                newAveragePosition /= boidsInVisibalDistance;
                if (newAveragePosition != pos[0])
                {
                    
                    Vector2 dirToCenter = newAveragePosition - pos[0];
                    dir += dirToCenter / dirToCenter.Length();

                }

                //Adjust the moving direction according to the flocks average direction, Alignment
                if (averageDirection != dir)
                {
                    averageDirection /= boidsInVisibalDistance;

                    dir += averageDirection;
                }


                    //averageSepForce /= boidsInVisibalDistance;
                    dir += averageSepForce;

            }
        }

        private void FollowingFeed(List<Feed> feeds,float visibalDistance)
        {
            //following a feed
            for (int i = 0; i < feeds.Count; i++)
            {
                Vector2 FoodVec = feeds[i].GetPos() - this.pos[0];

                if (FoodVec.Length() < visibalDistance)
                {
                    if(FoodVec.Length() > 0.1)
                    {
                        FoodVec /= FoodVec.Length();
                        this.dir += FoodVec * 0.2f;
                    }

                }

            }
        }

        private void avoidingObst(List<Obst> obst)
        {
            for(int i = 0; i < obst.Count; i++)
            {
                Vector2 ObstVec = obst[i].GetPos() - this.pos[0];

                if (ObstVec.Length() < obst[i].GetInfluenceRange())
                {
                    double First = ObstVec.LengthSquared();

                    double Second = obst[i].GetInfluenceRange() / 2;
                    Second = Math.Pow(Second, 2);

                    First = First - Second;

                    float CosA = (float)First / ObstVec.Length();

                    //angle between the vectors
                    float angle = Vector2.Dot(ObstVec, this.dir);

                    if (Math.Abs(angle) < Math.Abs(CosA))
                    {
                        double cosAngle = Math.Cos(angle);
                        Vector2 addVec = new Vector2(0, 0);

                        float third = 1 - (ObstVec.Length() / obst[i].GetInfluenceRange());
                        addVec = ObstVec / ObstVec.Length();
                        this.dir -= 2*addVec;

                    }
                }
            }
        }

        private void AvoidEnemyBoids(List<Obst> obst)
        {
            Vector2 v = new Vector2(0, 0);
            for (int i = 0; i < obst.Count; i++)
            {
                Vector2 OtherVec = this.pos[0] - obst[i].GetPos();
                if(OtherVec.Length() < obst[i].GetInfluenceRange() && obst[i].active)
                {
                    float constant = (OtherVec.Length() / obst[i].GetInfluenceRange()) * -1.0f;
                    v = OtherVec / OtherVec.Length();
                    v = v * constant;

                    float w = 2.0f;

                    float speedMod = OtherVec.Length() / obst[i].GetInfluenceRange();

                    this.speed = this.speed * (1+speedMod);
                    if (250 < this.speed)
                        this.speed = 250;

                    this.dir -=  w*v;
                }
                else
                {
                    this.speed = 100;
                }
            }
        }

        private void ReinitializeBoidPosition(bool loopAround)
        {
            if (loopAround)
            {
                if (pos[0].X < -(fishTex[0].Width * size))
                {
                    pos[0].X = windowSize.X + (fishTex[0].Width * size);

                }
                else if (pos[0].Y < -(fishTex[0].Height * size) )
                {
                    pos[0].Y = windowSize.Y + (fishTex[0].Height * size);
                }
                else
                {
                    pos[0].X = pos[0].X % (windowSize.X + (fishTex[0].Width * size) );
                    pos[0].Y = pos[0].Y % (windowSize.Y + (fishTex[0].Height * size));
                }
            }
            else
            {
                ///This if for when we do not want to loop the fish tank, we simply tell the boids to bound of the edges
                if (pos[0].X > windowSize.X + (fishTex[0].Width * size))
                {
                    dir = -dir;
                }
                if (pos[0].Y > windowSize.Y + (fishTex[0].Height * size))
                {
                    dir = -dir;
                }
                if (pos[0].X < -(fishTex[0].Width * size))
                {
                    dir = -dir;
                }
                if (pos[0].Y < -(fishTex[0].Height * size))
                {
                    dir = -dir;
                }
            }
        }
    }
}
