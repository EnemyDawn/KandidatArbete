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

        Vector3 pos = new Vector3(1,1,1);
        float scale = 1;
        float angle = 1;
        public _3dBoid(ContentManager content,string modelName)
        {
            model = content.Load<Model>(modelName);
            world = Matrix.CreateScale(scale) * Matrix.CreateRotationY(MathHelper.ToRadians(angle)) * Matrix.CreateTranslation(pos);// Matrix.CreateWorld(pos, Vector3.Forward, Vector3.Up);

        }

        public void Draw(GraphicsDevice device, BasicEffect effect)
        {

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

    }
}
