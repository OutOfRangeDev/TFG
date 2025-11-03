using System.Diagnostics;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Systems.Core;

namespace TFG.Scripts.Core.Systems.Physics;

public class PhysicsSystem : ISystem
{
    private readonly Vector2 _globalGravity = new Vector2(0f, 980f);
    
    public void Update(World.World world, GameTime gameTime)
    {
        var entities = world.Query().
            With<PhysicsComponent>().
            With<TransformComponent>().
            Execute();

        foreach (var entity in entities)
        {
            var physics = world.GetComponent<PhysicsComponent>(entity);
            var transform = world.GetComponent<TransformComponent>(entity);
            bool wasGrounded = physics.IsGrounded;
            physics.IsGrounded = false;

            // Apply gravity
            if(!wasGrounded)
                physics.Velocity += _globalGravity * physics.GravityScale * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply drag
            physics.Velocity *= 1f - physics.Drag * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update position
            transform.Position += physics.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Save the updated components back to the world
            world.SetComponent(entity, physics);
            world.SetComponent(entity, transform);
            
            //Debug.WriteLine($"Entity {entity.Id} - Position: {transform.Position}, Velocity: {physics.Velocity}");
        }
    }
}