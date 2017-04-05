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

        float speed = 1.0f;
        float size = 0.4f;
        float keepDistance = 80;
        float visibalDistance = 100;

        Vector2 pos;
        Vector2 dir;

        Vector2 windowSize;

        public Boid(ContentManager content,Vector2 windowSize,Random randum)
        {
            this.windowSize = windowSize;

            //apply random postitions in the game
            dir = new Vector2(randum.Next(-100, 100), randum.Next(-100, 100));
            pos = new Vector2(randum.Next(0, (int)windowSize.X),randum.Next(0, (int)windowSize.Y));

            fishTex = content.Load<Texture2D>("Arrow");
        }

        public void Update(List<Boid> boids, List<Feed> feeds)
        {
            //this function satisfy the separation, alignment and cohation roles
            //with is the rules of the boid algorithm
            this.boidsFirstRules(boids);

            //this is an extention of the boid, which makes the fish attracted to
            //to a point in the game, called a feed
            this.FollowingFeed(feeds);

            //simple function that moves the boids back to the screen
            this.ReinitializeBoidPosition();

            //the direction should be normailized to maintain speed reliability
            dir.Normalize();
            pos +=(dir * speed);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = (float)(Math.Atan2(dir.Y, dir.X));// / (2 * Math.PI));

            spriteBatch.Draw(fishTex, pos, new Rectangle(0, 0, fishTex.Width, fishTex.Height), Color.White, 
                rotation,
                new Vector2(fishTex.Width, (fishTex.Height / 2)),
                size,
                SpriteEffects.None,1
                );
        }

        private void boidsFirstRules(List<Boid> boids)
        {
            Vector2 newAveragePosition = pos;
            Vector2 averageDirection = new Vector2(0.0f, 0.0f);
            int boidsInVisibalDistance = 1;

            for (int n = 0; n < boids.Count; n++)
            {
                if (boids[n] != this)
                {
                    Vector2 boidVec = (boids[n].pos - pos);
                    if (boidVec.Length() < keepDistance)
                    {
                        //Separation, the closer to a flockmate, the more they are repelled
                        boidVec = ((boidVec.Length() / keepDistance) - 1) * (boidVec / boidVec.Length());
                        dir += boidVec;// * cordilate;
                    }


                    if ((boidVec.Length() < visibalDistance))
                    {
                        //calculate avg data for Alignment and Cohation
                        newAveragePosition += boids[n].pos;
                        averageDirection += boids[n].dir;

                        boidsInVisibalDistance++;
                    }
                }
            }

            //Adjust boid to follow the flocks average position, cohation 
            if (newAveragePosition != pos)
            {
                newAveragePosition /= boidsInVisibalDistance;
                Vector2 dirToCenter = newAveragePosition - pos;

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
                Vector2 FoodVec = feeds[i].GetPos() - this.pos;

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

        private void ReinitializeBoidPosition()
        {
            if (pos.X > windowSize.X + (fishTex.Width * size))
            {
                pos.X = 0;// -(fishTex.Width * size);
            }
            if (pos.Y > windowSize.Y + (fishTex.Height * size))
            {
                pos.Y = 0;// -(fishTex.Height * size);
            }
            if (pos.X < -(fishTex.Width * size))
            {
                pos.X = windowSize.X;// + (fishTex.Width * size);
            }
            if (pos.Y < -(fishTex.Height * size))
            {
                pos.Y = windowSize.Y + (fishTex.Height * size);
            }
        }
    }
}
