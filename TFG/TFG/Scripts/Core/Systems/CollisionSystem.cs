using System.Linq;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Helper;

namespace TFG.Scripts.Core.Systems;

public class CollisionSystem(bool[,] collisionMatrix) : ISystem
{
    public void Update(Data.World world, GameTime gameTime)
    {
        // Get all entities with a collision component.
        var entities = world.Query().
            With<ColliderComponent>().
            With<TransformComponent>().
            Execute().ToList();

        for (int i = 0; i < entities.Count; i++)
        {
            for (int j = i + 1; j < entities.Count; j++)
            {
                var entityA = entities[i];
                var entityB = entities[j];

                ref var colliderA = ref world.GetComponent<ColliderComponent>(entityA);
                ref var colliderB = ref world.GetComponent<ColliderComponent>(entityB);

                if (!collisionMatrix[(int)colliderA.Layer, (int)colliderB.Layer]) continue;
                
                var boundsA = CollisionHelper.GetWorldBounds(entityA, world);
                var boundsB = CollisionHelper.GetWorldBounds(entityB, world);

                if (CollisionHelper.AreColliding(boundsA, boundsB))
                {
                    if (colliderA.IsTrigger || colliderB.IsTrigger)
                    {
                        world.AddCollisionEvent(new Data.World.CollisionEvent(entityA, entityB));
                    }
                }
            }
        }
    }
}