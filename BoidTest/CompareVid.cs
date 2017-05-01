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
    class CompareVid
    {
        NewVideoPlayer video;

        Vector2 windowSize;

        public CompareVid(GraphicsDevice device)
        {
            video = new NewVideoPlayer("ds", device);
            
        }
    }
}
