using System;
using TFG.Scripts.Core.Components;

namespace TFG.Scripts.Game.Data;

public static class CollisionRules
{
    public static bool[,] CreateCollisionMatrix()
    {
        int numLayers = Enum.GetNames(typeof(CollisionLayer)).Length;
        var matrix = new bool[numLayers, numLayers];
        
        //ENVIRONMENT LAYERS
        SetCollision(matrix, CollisionLayer.Player, CollisionLayer.Environment, true);
        SetCollision(matrix, CollisionLayer.Enemy,  CollisionLayer.Environment, true);
        // DAMAGE LAYERS
        SetCollision(matrix, CollisionLayer.Player, CollisionLayer.HitPlayer, true);
        SetCollision(matrix, CollisionLayer.Enemy, CollisionLayer.HitEnemy, true);
        
        return matrix;
    }
    
    private static void SetCollision(bool[,] collisionMatrix, CollisionLayer layerA, CollisionLayer layerB, bool canCollide)
    {
        collisionMatrix[(int)layerA, (int)layerB] = canCollide;
        collisionMatrix[(int)layerB, (int)layerA] = canCollide;
    }
}