using System.Linq;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Components.Physics;
using TFG.Scripts.Core.Helper;

namespace TFG.Scripts.Core.Systems;

public class PhysicsSystem : ISystem
{
    private readonly Vector2 _globalGravity = new Vector2(0f, 980f);
    
    public void Update(Data.World world, GameTime gameTime)
    {
        
        float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
        
        //Get all the entities that have a physics component. And collider.
        var dynamicEntities = world.Query().
            With<PhysicsComponent>().
            With<TransformComponent>().
            With<ColliderComponent>().
            Execute().
            Where(entity => !world.GetComponent<PhysicsComponent>(entity).IsStatic).
            ToList();

        var solidObstacles = world.Query().
            With<ColliderComponent>().
            With<TransformComponent>().
            Execute().
            Where(entity => !world.TryGetComponent(entity, out PhysicsComponent p) || p.IsStatic).
            ToList();

        foreach (var moverEntity in dynamicEntities)
        {
            ref var moverPhysics = ref world.GetComponent<PhysicsComponent>(moverEntity);
            ref var moverTransform = ref world.GetComponent<TransformComponent>(moverEntity);
            
            //------------- Gravity --------------
            
            bool wasGrounded = moverPhysics.IsGrounded;
            moverPhysics.IsGrounded = false;
            if (!wasGrounded)
            {
                moverPhysics.Velocity.Y += _globalGravity.Y * deltaTime * moverPhysics.GravityScale;
            }
            
            // ------------- Horizontal Movement --------------
            
            // First, we update the position of the mover.
            moverTransform.Position = moverTransform.Position 
                with {X = moverTransform.Position.X + moverPhysics.Velocity.X * deltaTime};

            // Now we get the bounds of the mover.
            var moverBoundsH = CollisionHelper.GetWorldBounds(moverEntity, world);

            // And check for collisions with solid obstacles.
            foreach (var obstacleEntity in solidObstacles)
            {
                // Get the bounds of the obstacle.
                var obstacleBounds = CollisionHelper.GetWorldBounds(obstacleEntity, world);
                // And check if they intersect.
                if (CollisionHelper.AreColliding(moverBoundsH, obstacleBounds))
                {
                    // If they do, we resolve the collision.
                    ResolveHorizontalCollision(ref moverTransform, ref moverPhysics, moverBoundsH, obstacleBounds);
                    // And update the bounds of the mover.
                    moverBoundsH = CollisionHelper.GetWorldBounds(moverEntity, world);
                }
            }
            
            // ------------- Vertical Movement --------------
            
            // First, we update the position of the mover.
            moverTransform.Position = moverTransform.Position 
                with {Y = moverTransform.Position.Y + moverPhysics.Velocity.Y * deltaTime};
            
            // Small fix to make sure the mover doesn't fall through the ground.
            moverTransform.Position = moverTransform.Position with {Y = moverTransform.Position.Y + moverPhysics.SkinWidth};
            
            // Now we get the bounds of the mover.
            var moverBoundsV = CollisionHelper.GetWorldBounds(moverEntity, world);

            // And check for collisions with solid obstacles.
            foreach (var obstacleEntity in solidObstacles)
            {
                // Get the bounds of the obstacle.
                var obstacleBounds = CollisionHelper.GetWorldBounds(obstacleEntity, world);
                // And check if they intersect.
                if (CollisionHelper.AreColliding(moverBoundsV, obstacleBounds))
                {
                    // If they do, we resolve the collision.
                    ResolveVerticalCollision(ref moverTransform, ref moverPhysics, moverBoundsV, obstacleBounds);
                    // And update the bounds of the mover.
                    moverBoundsV = CollisionHelper.GetWorldBounds(moverEntity, world);
                }
            }
        }
    }

    private void ResolveHorizontalCollision(ref TransformComponent moverTransform, ref PhysicsComponent moverPhysics,
        Rectangle moverBoundsH, Rectangle obstacleBounds)
    {
        var intersection = Rectangle.Intersect(moverBoundsH, obstacleBounds);
        
        if(moverBoundsH.Center.X < obstacleBounds.Center.X)
            moverTransform.Position = moverTransform.Position with {X = moverTransform.Position.X - intersection.Width};
        else
            moverTransform.Position = moverTransform.Position with {X = moverTransform.Position.X + intersection.Width};
        
        moverPhysics.Velocity.X = 0;
    }

    private void ResolveVerticalCollision(ref TransformComponent moverTransform, ref PhysicsComponent moverPhysics,
        Rectangle moverBoundsV, Rectangle obstacleBounds)
    {
        var intersection = Rectangle.Intersect(moverBoundsV, obstacleBounds);

        if (moverBoundsV.Center.Y < obstacleBounds.Center.Y)
        {
            moverTransform.Position = moverTransform.Position with {Y = moverTransform.Position.Y - intersection.Height};
            moverPhysics.IsGrounded = true;
        }
        else
            moverTransform.Position = moverTransform.Position with {Y = moverTransform.Position.Y + intersection.Height};
        
        moverPhysics.Velocity.Y = 0;
    }
}