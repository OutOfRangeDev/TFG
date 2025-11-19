using System;
using System.Linq;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Systems.Core;

namespace TFG.Scripts.Core.Systems.Collisions;

public class CollisionSystem : ISystem
{
    // From here -----------------------------------------------------------------------------
    
    private bool[,] _collisionMatrix;
    
    public CollisionSystem()
    {
        InitializeMatrix();
    }

    private void InitializeMatrix()
    {
        int numLayers = Enum.GetNames(typeof(CollisionLayer)).Length;
        _collisionMatrix = new bool[numLayers, numLayers];
        
        SetCollision(CollisionLayer.Player, CollisionLayer.Environment, true);
    }
    
    private void SetCollision(CollisionLayer layerA, CollisionLayer layerB, bool canCollide)
    {
        _collisionMatrix[(int)layerA, (int)layerB] = canCollide;
        _collisionMatrix[(int)layerB, (int)layerA] = canCollide;
    }
    
    //To here -----------------------------------------------------------------------------
    // It's just a simple construction of a matrix to check collisions. In the future the intention
    // will be to create a data-driven system with JSON files. So it can be easily modified.
    
    
    public void Update(World.World world, GameTime gameTime)
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

                if (!_collisionMatrix[(int)colliderA.Layer, (int)colliderB.Layer]) continue;
                
                var boundsA = CollisionHelper.GetWorldBounds(entityA, world);
                var boundsB = CollisionHelper.GetWorldBounds(entityB, world);

                if (CollisionHelper.AreColliding(boundsA, boundsB))
                {
                    if (colliderA.IsTrigger || colliderB.IsTrigger)
                    {
                        world.AddCollisionEvent(new World.World.CollisionEvent(entityA, entityB));
                    }
                }
            }
        }
    }
}