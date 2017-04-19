using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace _3DBoids
{
    class _3dBoid
    {
        Model model;
        
        Matrix world;

        Vector3 pos;// = new Vector3(100,100,-120);
        float size = 3;
        float angle = 0;

        float speed = 1;
       
        float keepDistance = 330;
        float visibalDistance = 200;
        Vector3 dir;

        Vector2 windowSize;

        public _3dBoid(ContentManager content, Vector2 windowSize, Random randum)
        {
            model = content.Load<Model>("sphere//sphere");
            world = Matrix.CreateScale(size) * Matrix.CreateRotationY(MathHelper.ToRadians(angle)) * Matrix.CreateTranslation(pos);// Matrix.CreateWorld(pos, Vector3.Forward, Vector3.Up);
            dir = new Vector3(randum.Next(-100, 100), randum.Next(-100, 100), 0);
            pos = new Vector3(0, 0, -7);
            
            this.windowSize = windowSize;
        }

        public void Update(List<_3dBoid> boids, GameTime gameTime, bool loopAround)
        {
            //this function satisfy the separation, alignment and cohation roles
            //with is the rules of the boid algorithm
            this.BoidsFirstRules(boids);

            //this is an extention of the boid, which makes the fish attracted to
            //to a point in the game, called a feed
            // this.FollowingFeed(feeds);

            //simple function that moves the boids back to the screen
            this.ReinitializeBoidPosition(false);

            //the direction should be normailized to maintain speed reliability
            dir.Normalize();
            pos += (dir * speed) * (float)gameTime.ElapsedGameTime.TotalSeconds;



        }
        public void Draw(GraphicsDevice device, BasicEffect effect)
        {
            //effect.Projection.
            world.Translation = pos;
            //model.Draw(world, view, projection);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect eff in mesh.Effects)
                {
                    eff.World = world;
                    eff.View = effect.View;
                    eff.Projection = effect.Projection;
                    eff.EnableDefaultLighting();

                    eff.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

                    eff.DirectionalLight0.Enabled = false;
                }
                mesh.Draw();
            }

        }

        public void BoidsFirstRules(List<_3dBoid> boids)
        {
            Vector3 newAveragePosition = pos;
            Vector3 averageDirection = new Vector3(0.0f, 0.0f, 0.0f);
            int boidsInVisibalDistance = 1;
            for (int n = 0; n < boids.Count; n++)
            {
                if (boids[n] != this)
                {
                    Vector3 boidVec = (boids[n].pos - pos);
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
                Vector3 dirToCenter = newAveragePosition - pos;

                dir += dirToCenter / dirToCenter.Length();
            }

            //Adjust the moving direction according to the flocks average direction, Alignment
            if (averageDirection != dir)
            {
                averageDirection /= boidsInVisibalDistance;

                dir += averageDirection;
            }
        }
            
        public void ReinitializeBoidPosition(bool loopAround)
        {

            if (loopAround)
            {
                if (pos.X < -(2 * size))
                {
                    pos.X = windowSize.X/2 + (1 * size);

                }
                else if (pos.Y < -(2 * size))
                {
                    pos.Y = windowSize.Y/2 + (1 * size);
                }
                else
                {
                    pos.X = pos.X % (windowSize.X/2);
                    pos.Y = pos.Y % (windowSize.Y/2);
                }
            }
            else
            {
                ///This if for when we do not want to loop the fish tank, we simply tell the boids to bound of the edges
                if (pos.X > windowSize.X/2)
                {
                    dir = -dir;
                }
                if (pos.Y > windowSize.Y/2)
                {
                    dir = -dir;
                }
                if (pos.X < -windowSize.X/2)
                {
                    dir = -dir;
                }
                if (pos.Y < -windowSize.Y/2)
                {
                    dir = -dir;
                }

                if (pos.Z > -120)
                {
                    dir = -dir;
                }

                if (pos.Z < -320)
                {
                    dir = -dir;
                }

            }
        }
    }
}
