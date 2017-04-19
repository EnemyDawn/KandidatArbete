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
       
        float speed =300.0f;
        float size = 0.08f;
        float keepDistance = 100;
        float visibalDistance = 140;

        Vector2[] pos;
        Vector2 dir;

        Vector2 windowSize;
        Color color;

        public Boid(ContentManager content,Vector2 windowSize,Random randum)
        {
            this.windowSize = windowSize;

            //apply random postitions in the game
            speed = randum.Next(90,190);
            dir = new Vector2(randum.Next(-100, 100), randum.Next(-100, 100));
            pos = new Vector2[randum.Next(14,30)];
            pos[0] = new Vector2(randum.Next(0, (int)windowSize.X),randum.Next(0, (int)windowSize.Y));
            dir.Normalize();
            for (int n = 1; n < pos.Length; n++)
            {
                pos[n] = pos[0];// + (dir * (1 * n));
            }

            color =  new Color(randum.Next(150,160), randum.Next(150, 170), randum.Next(30, 40));

            size = randum.Next(5, 10) * 0.01f;// 0.08f;
            fishTex = content.Load<Texture2D>("Body");
        }

        public void Update(List<Boid> boids, List<Feed> feeds, List<Obst> obst,GameTime gameTime,bool loopAround)
        {
            //this function satisfy the separation, alignment and cohation roles
            //with is the rules of the boid algorithm
            this.BoidsFirstRules(boids, false);

            //this is an extention of the boid, which makes the fish attracted to
            //to a point in the game, called a feed
            //this.FollowingFeed(feeds);
            this.avoidingObst(obst);



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

        private void BoidsFirstRules(List<Boid> boids,bool loopAround)
        {
            Vector2 newAveragePosition = pos[0];
            Vector2 averageDirection = new Vector2(0.0f, 0.0f);
            int boidsInVisibalDistance = 1;
            if (!loopAround)
            {
                for (int n = 0; n < boids.Count; n++)
                {
                    if (boids[n] != this)
                    {
                        Vector2 boidVec = (boids[n].pos[0] - pos[0]);
                        if (boidVec.Length() < keepDistance)
                        {
                            //Separation, the closer to a flockmate, the more they are repelled
                            boidVec = ((boidVec.Length() / keepDistance) - 1) * (boidVec / boidVec.Length());
                            dir += boidVec;// * cordilate;
                        }


                        if ((boidVec.Length() < visibalDistance))
                        {
                            //calculate avg data for Alignment and Cohation
                            newAveragePosition += boids[n].pos[0];
                            averageDirection += boids[n].dir;

                            boidsInVisibalDistance++;
                        }
                    }
                }
            }
            else
            {
                for (int n = 0; n < boids.Count; n++)
                {
                    if (boids[n] != this)
                    {
                        Vector2 vecToWindow = (boids[n].pos[0]);
                        if(boids[n].pos[0].X < keepDistance) // Check X close to 0
                        {
                            vecToWindow.X = windowSize.X - (keepDistance - boids[n].pos[0].X);
                        }
                        else if ((windowSize.X - boids[n].pos[0].X) < keepDistance) // Check X close to Edge
                        {
                            vecToWindow.X = boids[n].pos[0].X % (windowSize.X - keepDistance);
                        }

                        if (boids[n].pos[0].Y < keepDistance) // Check Y close to 0
                        {
                            vecToWindow.Y = windowSize.Y - (keepDistance - boids[n].pos[0].Y);
                        }
                        else if ((windowSize.Y - boids[n].pos[0].Y) < keepDistance) // Check Y close to Edge
                        {
                            vecToWindow.Y = boids[n].pos[0].Y % (windowSize.Y- keepDistance);
                        }



                        Vector2 boidVec = (vecToWindow - pos[0]);
                        if (boidVec.Length() < keepDistance)
                        {
                            //Separation, the closer to a flockmate, the more they are repelled
                            boidVec = ((boidVec.Length() / keepDistance) - 1) * (boidVec / boidVec.Length());
                            dir += boidVec;// * cordilate;
                        }
                        else
                        {

                        }


                        if ((boidVec.Length() < visibalDistance))
                        {
                            //calculate avg data for Alignment and Cohation
                            newAveragePosition += boids[n].pos[0];
                            averageDirection += boids[n].dir;

                            boidsInVisibalDistance++;
                        }
                    }
                }
            }

            //Adjust boid to follow the flocks average position, cohation 
            if (newAveragePosition != pos[0])
            {
                newAveragePosition /= boidsInVisibalDistance;
                Vector2 dirToCenter = newAveragePosition - pos[0];

                dir += dirToCenter / dirToCenter.Length();
            }

            //Adjust the moving direction according to the flocks average direction, Alignment
            if (averageDirection != dir)
            {
                averageDirection /= boidsInVisibalDistance;

                dir += averageDirection;
            }
        }

        private void FollowingFeed(List<Feed> feeds)
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
                        this.dir += FoodVec;
                    }

                }

            }
        }

        private void avoidingObst(List<Obst> obst)
        {
            for(int i = 0; i < obst.Count; i++)
            {
                Vector2 ObstVec = obst[i].GetPos() - this.pos[0];

                float test = ObstVec.Length();
                float test2 = obst[i].GetInfluenceRange();

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

            //if (pos[0].X > windowSize.X + (fishTex.Width * size))
            //{
            //    pos[0].X = 0;// -(fishTex.Width * size);
            //}
            //if (pos[0].Y > windowSize.Y + (fishTex.Height * size))
            //{
            //    pos[0].Y = 0;// -(fishTex.Height * size);
            //}
            //if (pos[0].X < -(fishTex.Width * size))
            //{
            //    pos[0].X = windowSize.X + (fishTex.Width * size);
            //}
            //if (pos[0].Y < -(fishTex.Height * size))
            //{
            //    pos[0].Y = windowSize.Y + (fishTex.Height * size);
            //}
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
