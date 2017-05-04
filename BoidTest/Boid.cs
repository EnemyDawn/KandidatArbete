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
        Texture2D fishTex;
       
        float speed = 100.0f;
        float size = 0.08f;


        Vector2[] pos;
        Vector2 dir;

        Vector2 windowSize;
        Color color;

        Vector2 vSep;
        Vector2 vAlign;
        Vector2 vCohe;

        Vector2 vFeed;
        Vector2 vEnemy;


        public Boid(ContentManager content,Vector2 windowSize,Vector2 posIn,Random randum)
        {
            this.windowSize = windowSize;

            //apply random postitions in the game
            //speed = randum.Next(50,130);
            dir = new Vector2(1,1);
            pos = new Vector2[22];

            this.vSep = new Vector2(1, 1);
            this.vAlign = new Vector2(1, 1);
            this.vSep = new Vector2(1, 1);

            this.vFeed = new Vector2(1, 1);
            this.vEnemy = new Vector2(1, 1);

            pos[0] = posIn;// new Vector2(randum.Next(100, 120),randum.Next(100, 120));
            dir.Normalize();

            for (int n = 1; n < pos.Length; n++)
            {
                pos[n] = pos[0];// + (dir * (1 * n));
            }

            color = new Color(randum.Next(150,255), randum.Next(150, 255), randum.Next(50, 60));

            //size = randum.Next(5, 10) * 0.01f;// 0.08f;
            fishTex = content.Load<Texture2D>("Body");
        }

        public void Update(Boid[] boids, List<Feed> feeds, List<Obst> obst,GameTime gameTime,bool loopAround,float keepDistance,float viewDistance)
        {
            //this function satisfy the separation, alignment and cohation roles
            //with is the rules of the boid algorithm

            Vector2 avgPos = new Vector2(1, 1);
            Vector2 avgDir = new Vector2(1, 1);
            int visibleBoids = 0;


            //this.FollowingFeed(feeds, 2000);

            this.calcSeparation(boids, keepDistance, viewDistance);

            //inte säker på att denna fungerar
            //this.calcAvg(boids, keepDistance, viewDistance, ref avgPos, ref avgDir, ref visibleBoids);

            //this.calcCohesion(boids, keepDistance, viewDistance, visibleBoids, avgPos);
            
            
            //this.avoidingObst(obst);
            //this.AvoidEnemyBoids(obst);


            //simple function that moves the boids back to the screen
            //this.ReinitializeBoidPosition(loopAround);

            //the direction should be normailized to maintain speed reliability

            this.dir = this.vSep;// + 0.2f* this.vAlign + 0.02f * this.vCohe + 0.2f * this.vFeed;

            //dir += 0.2f * this.vAlign;
            //dir += 0.2f * this.vCohe;
            //dir += this.vFeed;

            this.dir.Normalize();

            this.pos[0] += (dir * 10) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //pos[0] += (dir * speed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
           



            for (int n = pos.Length-1; n > 0; n--)
            {
                pos[n] = pos[n-1];
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = (float)(Math.Atan2(dir.Y, dir.X));// / (2 * Math.PI));

            float tempSize = size / pos.Length;
            float startValue = tempSize;

            for (int n = pos.Length - 1; n >= 0; n--)
            {
                tempSize += startValue;

                spriteBatch.Draw(fishTex, pos[n], new Rectangle(0, 0, fishTex.Width, fishTex.Height), new Color(color.R + (n * 2), color.G + (n * 2), color.B + (n * 2)),
                    rotation,
                    new Vector2(fishTex.Width, (fishTex.Height / 2)),
                    tempSize,
                    SpriteEffects.None, 1
                    );
            }
        }

        private void calcSeparation(Boid[] boids, float keepDistance, float visibalDistance)
        {
            for(int i = 0; i < boids.Length; i++)
            {
                //boidVec is not working
                if(boids[i] != this)
                {
                    Vector2 boidVec = boids[i].pos[0] - this.pos[0];
                    if (boidVec.Length() < visibalDistance)
                    {
                        if (boidVec.Length() == 0)
                        {
                            this.vSep = new Vector2(-1, -1);
                            this.pos[0].X += 2.0f;
                            this.pos[0].Y += 2.0f;
                        }
                        else
                        {
                            float mod = boidVec.Length() / keepDistance;
                            mod -= 1;

                            this.vSep = (mod * (boidVec / boidVec.Length()));
                        }
                    }
                    else
                    {
                        this.vSep = new Vector2(0, 0);
                    }
                }
              
            }
       }

        private void calcAvg(Boid[] boids, float keepDistance, float visibalDistance, ref Vector2 newAveragePosition, ref Vector2 averageDirection, ref int boidsInVisibalDistance)
        {
            Vector2 centerVec = new Vector2(0, 0);
            Vector2 toOtherBoid = new Vector2(0, 0);

            //neighboorhood
            for (int i = 0; i < boids.Length; i++)
            {
                if(boids[i] != this)
                {
                    Vector2 boidVec = boids[i].pos[0] - pos[0];
                    if ((boidVec.Length() < visibalDistance))
                    {
                        //calculate avg data for Alignment and Cohation
                        newAveragePosition += boids[i].pos[0];
                        averageDirection += boids[i].dir;

                        boidsInVisibalDistance++;
                    }
                }

            }
            this.vAlign = averageDirection / boidsInVisibalDistance;
        }

        private void calcCohesion(Boid[] boids, float keepDistance, float visibalDistance, int boidsInVisibalDistance, Vector2 newAveragePosition)
        {
            //Adjust boid to follow the flocks average position, cohation 
            if (boidsInVisibalDistance > 0)
            {
                if (newAveragePosition != pos[0])
                {
                    newAveragePosition /= (boidsInVisibalDistance);

                    Vector2 dirToCenter = newAveragePosition - pos[0];

                    //Vector2 dirToCenter = pos[0] -newAveragePosition;

                    //dir += (dirToCenter / dirToCenter.Length());
                    this.vCohe = (dirToCenter / dirToCenter.Length());
                }
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
                    if((FoodVec.Length()) > 0.1)
                    {
                        FoodVec /= FoodVec.Length();
                        this.vFeed = FoodVec;
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
                if (pos[0].X < -(fishTex.Width * size))
                {
                    pos[0].X = windowSize.X + (fishTex.Width * size);

                }
                else if (pos[0].Y < -(fishTex.Height * size) )
                {
                    pos[0].Y = windowSize.Y + (fishTex.Height * size);
                }
                else
                {
                    pos[0].X = pos[0].X % (windowSize.X + (fishTex.Width * size) );
                    pos[0].Y = pos[0].Y % (windowSize.Y + (fishTex.Height * size));
                }
            }
            else
            {
                ///This if for when we do not want to loop the fish tank, we simply tell the boids to bound of the edges
                if (pos[0].X > windowSize.X + (fishTex.Width * size))
                {
                    dir = -dir;
                }
                if (pos[0].Y > windowSize.Y + (fishTex.Height * size))
                {
                    dir = -dir;
                }
                if (pos[0].X < -(fishTex.Width * size))
                {
                    dir = -dir;
                }
                if (pos[0].Y < -(fishTex.Height * size))
                {
                    dir = -dir;
                }
            }
        }
    }
}
