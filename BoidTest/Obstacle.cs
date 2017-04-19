using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BoidTest
{
    class Obstacle
    {
        Texture2D ObstacleTex;
        float size = 0.1f;
        Vector2 pos;
        Vector2 dir;

        float speed = 0.1f;
        float multiplier;

        public Obstacle(ContentManager content, Vector2 pos)
        {
            this.ObstacleTex = content.Load<Texture2D>("fine");
            this.pos = pos;
            this.dir = new Vector2(20, 20);



        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = (float)(Math.Atan2(dir.Y, dir.X));

            spriteBatch.Draw(
                this.FeedTex, this.pos, new Rectangle(0, 0, this.FeedTex.Width, this.FeedTex.Height), Color.White,
                rotation, new Vector2(this.FeedTex.Width, (this.FeedTex.Height / 2)),
                size,
                SpriteEffects.None, 1
                );

        }

        public Vector2 GetPos()
        {
            return new Vector2();
        }
    }
}
