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
    public class Obst
    {
        Texture2D FeedTex;
        float size = 0.1f;
        Vector2 pos;
        Vector2 dir;

        float speed = 0.1f;
        float influenceRadius;

        public Obst(ContentManager content, Vector2 pos)
        {
            this.FeedTex = content.Load<Texture2D>("fine");
            this.pos = pos;
            this.dir = new Vector2(1.0f, 0.0f);

            Vector2 influenceRadius = new Vector2(this.FeedTex.Height / 2, this.FeedTex.Width / 2);
            this.influenceRadius = influenceRadius.Length();
            this.influenceRadius *= size * 5;

        }

        public void Update()
        {
            this.pos.X = Mouse.GetState().X;
            this.pos.Y = Mouse.GetState().Y;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float rotation = (float)(Math.Atan2(dir.Y, dir.X));// / (2 * Math.PI));

            spriteBatch.Draw(this.FeedTex, this.pos, new Rectangle(0, 0, this.FeedTex.Width, this.FeedTex.Height), Color.White,
                rotation,
                new Vector2(this.FeedTex.Width / 2, (this.FeedTex.Height / 2)),
                this.size,
                SpriteEffects.None, 1
                );
        }

        public Vector2 GetPos()
        {
            return this.pos;
        }

        public float GetInfluenceRange()
        {
            return this.influenceRadius;
        }
    }
}
