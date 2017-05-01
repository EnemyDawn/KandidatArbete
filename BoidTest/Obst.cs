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

        float influenceRadius;

        public bool active = false;

        public Obst(ContentManager content, Vector2 pos)
        {
            this.FeedTex = content.Load<Texture2D>("Body");
            this.pos = pos;

            influenceRadius = 80;
           // Vector2 influenceRadius = new Vector2(this.FeedTex.Height / 2, this.FeedTex.Width / 2);
           // this.influenceRadius = influenceRadius.Length();
           // this.influenceRadius *= size * 5;

        }

        public void Update()
        {
           if(Keyboard.GetState().IsKeyDown(Keys.Space) && !active)
            {
                active = true;
                pos = new Vector2(800, 800);
            }

           if(active)
            {
                Vector2 dir = (new Vector2(100, 100) - pos);// * 3;
                dir.Normalize();
                pos += dir * 4;
                if ((new Vector2(100, 100) - pos).Length() <10)
                    active = false;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {

            if(active)
                spriteBatch.Draw(this.FeedTex, this.pos, new Rectangle(0, 0, this.FeedTex.Width, this.FeedTex.Height),
                    Color.Red,
                    0,
                    new Vector2(0,0),
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
