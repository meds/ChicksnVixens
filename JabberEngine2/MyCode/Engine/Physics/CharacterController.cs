using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Phone.Tasks;
using System.Text;

namespace Jabber.Physics
{
    //a custom controller class written using the JabWorld physics abstraction layer
    public class CharacterController : JabJect
    {
        public CharacterController(JabWorld world, Vector2 pos, Vector2 dim)
        {
            this.world = world;
            this.dim = dim;
            body = world.CreateBox(dim, pos, JabActor.BodyType.STATIC);
            float xpos = body.PosX;
        }
        public override void UnloadContent()
        {
            world.DestroyActor(body);
            base.UnloadContent();
        }
        Vector2 dim;
        public void Move(Vector2 dir)
        {
            if (dir == Vector2.Zero)
            {
                return; // not interested in zero movement
            }
            body.IgnoreRayCast = true;
            Vector2 rayStart = position;

            if (dir.X > 0)
            {
                rayStart.X += dim.X / 2.0f - 1;
            }
            else if (dir.X < 0)
            {
                rayStart.X -= dim.X / 2.0f - 1.0f;
            }
            if (dir.Y > 0)
            {
                rayStart.Y += dim.Y / 2.0f - 1.0f;
            }
            else if (dir.Y < 0)
            {
                rayStart.Y -= dim.Y / 2.0f - 1.0f;
            }
            RayCastHit hit = world.RayCast(position, rayStart + dir);

            if (hit != null)
            {
                if (hit.actor == null)
                {
                    body.Position += dir;
                }
                else
                {
                    Vector2 hitPoint = hit.worldImpact;
                    Vector2 targetPos = rayStart + dir;
                    dir = targetPos - hitPoint;

                    float leftPart = hitPoint.X - dim.X / 2.0f;
                    float rightPart = hitPoint.X + dim.X / 2.0f;

                    float leftOverlap = leftPart - hitPoint.X;
                    float rightOverlap = rightPart - hitPoint.X;
                    body.Position += dir;// hitPoint;// -new Vector2(dim.X / 2.0f, 0);// dir;
                    // body.PosX = leftPart;
                    dir *= 1.0001f;
                    if (hitPoint.X > (rayStart + dir).X && dir.X < 0)
                    {
                        RayCastHit sidehit = world.RayCast(rayStart, new Vector2(dir.X, 0) + rayStart);
                        if (sidehit.actor != null)
                        {
                            body.PosX = sidehit.worldImpact.X + dim.X / 2.0f;
                        }
                    }
                    else if (hitPoint.X < (rayStart + dir).X && dir.X > 0)
                    {
                        RayCastHit sidehit = world.RayCast(rayStart, new Vector2(dir.X, 0) + rayStart);
                        if (sidehit.actor != null)
                        {
                            body.PosX = sidehit.worldImpact.X - dim.X / 2.0f;
                        }
                    }
                    if (hitPoint.Y > (rayStart + dir).Y && dir.Y < 0)
                    {
                        RayCastHit sidehit = world.RayCast(rayStart, new Vector2(0, dir.Y) + rayStart);
                        if (sidehit.actor != null)
                        {
                            body.PosY = sidehit.worldImpact.Y + dim.Y / 2.0f;
                        }
                    }
                    else if (hitPoint.Y < (rayStart + dir).Y && dir.Y > 0)
                    {
                        RayCastHit sidehit = world.RayCast(rayStart, new Vector2(0, dir.Y) + rayStart);
                        if (sidehit.actor != null)
                        {
                            body.PosY = sidehit.worldImpact.Y - dim.Y / 2.0f;
                        }
                    }
                }
            }
            position = body.Position;
        }
        public override void Update(GameTime gameTime)
        {
            position = body.Position;
            base.Update(gameTime);
        }

        JabWorld world = null;
        JabActor body = null;
    }
}
