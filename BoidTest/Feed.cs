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
    public class Feed
    {
        Texture2D FeedTex;
        float size = 0.3f;
        Vector2 pos;
        Vector2 dir;

        float speed = 0.1f;

        int target = 0;
        double countDown = 7;

        public Feed(ContentManager content, Vector2 pos)
        {
            this.FeedTex = content.Load<Texture2D>("bread");
            this.pos = pos;
            this.dir = new Vector2(1.0f, 0.0f);
        }

        public void Update(GameTime gameTime)
        {
            if(target == 0)
            {
                this.pos = new Vector2(200, 200);
            }
            else
            {
                this.pos = new Vector2(1000, 500);
            }
            countDown -= gameTime.ElapsedGameTime.TotalSeconds;
            if(countDown<0)
            {
                target++;
                target = target %2;
                countDown = 7;
            }


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = (float)(Math.Atan2(dir.Y, dir.X));// / (2 * Math.PI));

            spriteBatch.Draw(this.FeedTex, this.pos, new Rectangle(0, 0, this.FeedTex.Width, this.FeedTex.Height), Color.White,
                rotation,
                new Vector2(this.FeedTex.Width, (this.FeedTex.Height / 2)),
                size,
                SpriteEffects.None, 1
                );
        }

        public Vector2 GetPos()
        {
            return this.pos;
        }
    }
}
