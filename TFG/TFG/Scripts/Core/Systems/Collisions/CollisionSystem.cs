using System;
using System.Linq;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Systems.Core;
using TFG.Scripts.Core.Systems.Physics;
using TFG.Scripts.Core.World;

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
            With<ColliderComponent>().With<TransformComponent>().
            Execute().ToList();

        // Check for collisions between all pairs of entities.
        for (int i = 0; i < entities.Count; i++)
        {
            for (int j = i + 1; j < entities.Count; j++)
            {
                // Get the entities.
                var entityA = entities[i];
                var entityB = entities[j];

                // Get the collision/transform components for each entity.
                var collisionA = world.GetComponent<ColliderComponent>(entityA);
                var transformA = world.GetComponent<TransformComponent>(entityA);
                var collisionB = world.GetComponent<ColliderComponent>(entityB);
                var transformB = world.GetComponent<TransformComponent>(entityB);

                // Check if the entities are colliding.
                if (_collisionMatrix[(int)collisionA.Layer, (int)collisionB.Layer])
                {
                    // First we create the bounds of each entity. Making the rectangles at the moment.
                    var boundsA = new Rectangle(
                        (int)(transformA.Position.X + collisionA.Offset.X),
                        (int)(transformA.Position.Y + collisionA.Offset.Y),
                        (int)collisionA.Size.X,
                        (int)collisionA.Size.Y);

                    var boundsB = new Rectangle((int)(transformB.Position.X + collisionB.Offset.X),
                        (int)(transformB.Position.Y + collisionB.Offset.Y),
                        (int)collisionB.Size.X,
                        (int)collisionB.Size.Y);
                    
                    // To then check if they are colliding.
                    if (boundsA.Intersects(boundsB))
                    {
                        // Collision detected.
                        // If any of the entities are triggers, we don't resolve the collision.
                        // We launch an event instead. And let the desired scripts handle it.
                        if (collisionA.IsTrigger || collisionB.IsTrigger)
                        {
                            world.AddCollisionEvent(new World.World.CollisionEvent(entityA, entityB));
                        }
                        // If they aren't triggers, we resolve the collision.
                        else
                        {
                            // First, we check if they have physics.
                            bool hasPhysicsA = world.TryGetComponent(entityA, out PhysicsComponent physicsA);
                            bool hasPhysicsB = world.TryGetComponent(entityB, out PhysicsComponent physicsB);

                            bool isADinamic = hasPhysicsA && !physicsA.IsStatic;
                            bool isBDinamic = hasPhysicsB && !physicsB.IsStatic;
                            
                            // If both of them are dynamic, or only A.
                            if(isADinamic && isBDinamic || isADinamic)
                                ResolveSolidCollision(entityA, entityB, world);
                            else if(isBDinamic)
                                // If only B is dynamic, or only B.
                                ResolveSolidCollision(entityB, entityA, world);
                        }
                    }
                }
            }
        }
    }

    // Resolve the collision between two entities.
    // This is a simple implementation of a simple collision resolution.
    private void ResolveSolidCollision(Entity mover, Entity obstacle, World.World world)
    {
        // We get the components of the entities.
        var moverTransform = world.GetComponent<TransformComponent>(mover);
        var moverCollider = world.GetComponent<ColliderComponent>(mover);
        var moverPhysics = world.GetComponent<PhysicsComponent>(mover);
        
        var obstacleTransform = world.GetComponent<TransformComponent>(obstacle);
        var obstacleCollider = world.GetComponent<ColliderComponent>(obstacle);
        
        //Create the bounds of the entities.
        var boundsMover = new Rectangle(
            (int)(moverTransform.Position.X + moverCollider.Offset.X),
            (int)(moverTransform.Position.Y + moverCollider.Offset.Y),
            (int)moverCollider.Size.X,
            (int)moverCollider.Size.Y);
        
        var boundsObstacle = new Rectangle((int)(obstacleTransform.Position.X + obstacleCollider.Offset.X),
            (int)(obstacleTransform.Position.Y + obstacleCollider.Offset.Y),
            (int)obstacleCollider.Size.X,
            (int)obstacleCollider.Size.Y);
        
        //Check how much the entities are overlapping. The intersection is the area that is overlapping.
        var intersection = Rectangle.Intersect(boundsMover, boundsObstacle);

        Vector2 penetrationVector;

        // Is it wider or taller?
        if (intersection.Width > intersection.Height)
        {
            // If it's wider, we push the mover to the left or right.
            float pushDirection = (boundsMover.Center.X < boundsObstacle.Center.X ? -intersection.Width : intersection.Width);
            penetrationVector = new Vector2(pushDirection, 0);
        }
        else
        {
            // If it's taller, we push the mover up or down.'
            float pushDirection = boundsMover.Center.Y < boundsObstacle.Center.Y ? -intersection.Height : intersection.Height;
            penetrationVector = new Vector2(0, pushDirection);
        }

        //Apply the correction to the position.
        moverTransform.Position += penetrationVector;
        
        //If we move it vertically = floor or ceiling, then we cancel the vertical velocity.
        if(penetrationVector.Y < 0)
            moverPhysics.Velocity.Y = 0; moverPhysics.IsGrounded = true;
        if(penetrationVector.Y > 0)
            moverPhysics.Velocity.Y = 0; 
        //If we move it horizontally = left or right, then we cancel the horizontal velocity.
        if(penetrationVector.X != 0)
            moverPhysics.Velocity.X = 0;

        
        
        //Update the components.
        world.SetComponent(mover, moverTransform);
        world.SetComponent(mover, moverPhysics);
    }
}