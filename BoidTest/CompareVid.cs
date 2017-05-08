using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using DirectShowLib;

namespace BoidTest
{
    class CompareVid
    {
        NewVideoPlayer[] videos;

        Vector2 windowSize;
        Vector2 videoFrame;

        Rectangle[] renderScreens;

        bool mouseDown = true;

        public CompareVid(Game1 game,string [] inNames)
        {
            videos = new NewVideoPlayer[inNames.Length];

            windowSize = new Vector2(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);

            videoFrame = new Vector2(windowSize.X / 2.8f, windowSize.Y / 2.6f);
            renderScreens = new Rectangle[inNames.Length];

            for (int i = 0;i< inNames.Length;i++)
            {
                videos[i] = new NewVideoPlayer(inNames[i], game.GraphicsDevice);
                
                if (videos[i].MillisecondsPerFrame != -1)
                    game.TargetElapsedTime = TimeSpan.FromMilliseconds(videos[i].MillisecondsPerFrame);

                renderScreens[i] = new Rectangle((((int)videoFrame.X+5) * i % (int)windowSize.X),100+ ((int)videoFrame.Y+5) * (((int)videoFrame.X * i) / (int)windowSize.X), (int)videoFrame.X, (int)videoFrame.Y);
            };

            for (int i = 0; i < inNames.Length; i++)
            {
                videos[i].CurrentState = VideoState.Playing;
            };

        }

        public void Unload()
        {
            for (int i = 0; i < videos.Length; i++)
            {
                videos[i].Stop();
                videos[i].Dispose();
                while(videos[i].IsDisposed == false)
                {

                }
            }

        }

        public string Update()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!mouseDown)
                {
                    mouseDown = true;
                    Rectangle mouseRec = new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Position.Y, 1, 1);
                    for (int i = 0; i < videos.Length; i++)
                    {
                        if (renderScreens[i].Intersects(mouseRec))
                            return videos[i].FileName;
                    }
                }
            }
            else
                mouseDown = false;

            return "";
        }

        public void Draw(SpriteBatch batch)
        {
            bool shouldRewind = true;
            for (int i = 0; i < videos.Length; i++)
            {
                videos[i].Update();

                if (videos[i].Duration > videos[i].CurrentPosition)
                {
                    shouldRewind = false;
                }

                batch.Draw(videos[i].OutputFrame, renderScreens[i], Color.White);
            }

            if(shouldRewind)
            {
                for (int i = 0; i < videos.Length; i++)
                    videos[i].Rewind();
            }
        }
    }
}
