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
        Rectangle rec;

        float speed = 3.0f;
        float size = 0.4f;
        float keepDistance = 80;
        float visibalDistance = 100;

        Vector2 pos;
        Vector2 dir;

        Vector2 windowSize;

        public Boid(ContentManager content,Vector2 windowSize,Random randum)
        {
            this.windowSize = windowSize;

            
            dir = new Vector2(randum.Next(-100, 100), randum.Next(-100, 100));

            pos = new Vector2(randum.Next(0, (int)windowSize.X),randum.Next(0, (int)windowSize.Y));

            fishTex = content.Load<Texture2D>("Arrow");
           // rec = new Rectangle(40, 40, (int)(fishTex.Width * size), (int)(fishTex.Height * size));
                
        }

        public void Update(List<Boid> boids)
        {

            Vector2 newAveragePosition = pos;
            int boidsInVisibalDistance = 1;

            for (int n = 0;n< boids.Count;n++)
            {
               Vector2 boidVec = (boids[n].pos - pos);
               if (boidVec.Length() < keepDistance && boids[n] != this)
                {
                    boidVec = ((boidVec.Length() / keepDistance)-1) * (boidVec / boidVec.Length());

                    //boidVec.Normalize();

                    dir += boidVec;// * cordilate;
                }

               if(boidVec.Length() < visibalDistance && boids[n] != this)
                {
                    newAveragePosition += boids[n].pos;
                    boidsInVisibalDistance++;
                }
            }

            if(newAveragePosition != pos)
            {
                newAveragePosition /= boidsInVisibalDistance;
                Vector2 dirToCenter = newAveragePosition - pos;

                dir += dirToCenter / dirToCenter.Length();
                //dirToCenter.Normalize();

            }
           



                dir.Normalize();
            pos +=(dir * speed);

            if(pos.X > windowSize.X + (fishTex.Width * size) )
            {
                pos.X = 0;// -(fishTex.Width * size);
            }
            if(pos.Y > windowSize.Y + (fishTex.Height * size))
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
    }
}
