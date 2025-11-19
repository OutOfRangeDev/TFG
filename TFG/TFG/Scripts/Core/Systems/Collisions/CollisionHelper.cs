using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Systems.Core;
using TFG.Scripts.Core.World;

namespace TFG.Scripts.Core.Systems.Collisions;

public static class CollisionHelper
{
    public static Rectangle GetWorldBounds(Entity entity, World.World world)
    {
        ref var transform = ref world.GetComponent<TransformComponent>(entity);
        ref var collider = ref world.GetComponent<ColliderComponent>(entity);
        
        return new Rectangle(
            (int) transform.Position.X + (int) collider.Offset.X, 
            (int) transform.Position.Y + (int) collider.Offset.Y, 
            (int) collider.Size.X, 
            (int) collider.Size.Y);
    }

    public static bool AreColliding(Rectangle boundsA, Rectangle boundsB)
    {
        return boundsA.Intersects(boundsB);
    }
}