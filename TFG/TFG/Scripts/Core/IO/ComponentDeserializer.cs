using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.IO;

public static class ComponentDeserializer
{
    private static MethodInfo _deserializeMethod;
    
    public static IComponent Deserialize(JsonElement componentData, Type componentType)
    {
        
        _deserializeMethod = typeof(JsonSerializer)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => 
            {
                if (m.Name != "Deserialize" || !m.IsGenericMethod) return false;
                var p = m.GetParameters();
                // Be VERY specific: We want the one that takes a string and JsonSerializerOptions.
                return p.Length == 2 && p[0].ParameterType == typeof(string) && p[1].ParameterType == typeof(JsonSerializerOptions);
            });
        
        // Get the generic Deserialize method
        if (_deserializeMethod == null)
        {
            // This is the most complex part. We are using Reflection to find the
            // exact overload of JsonSerializer.Deserialize we want.
            _deserializeMethod = typeof(JsonSerializer)
                .GetMethods(BindingFlags.Public | BindingFlags.Static) // We're looking for a public static method
                .FirstOrDefault(m =>
                {
                    // The method must be named "Deserialize"
                    if (m.Name != "Deserialize" || !m.IsGenericMethod)
                        return false;

                    // It must take two parameters
                    var parameters = m.GetParameters();
                    return parameters.Length == 2 &&
                           parameters[0].ParameterType == typeof(string) && // First param is a string (the JSON)
                           parameters[1].ParameterType == typeof(JsonSerializerOptions); // Second param is the options
                });

            if (_deserializeMethod == null)
            {
                // This should never happen, but it's a good safety check.
                throw new InvalidOperationException("Could not find the required JsonSerializer.Deserialize<T>(string, options) method via reflection.");
            }
        }

        // --- STEP 2: Create a Specific Version of the Method ---
        // We take the generic "template" method and create a concrete version.
        // e.g., if componentType is typeof(PhysicsComponent), this creates
        // a reference to the Deserialize<PhysicsComponent>(string, options) method.
        var genericMethod = _deserializeMethod.MakeGenericMethod(componentType);

        // --- STEP 3: Prepare the Arguments for the Call ---
        // We get the raw JSON text from the JsonElement,
        // and we use our shared JsonOptions that contain all our custom converters.
        var parameters = new object[] { componentData.GetRawText(), JsonOptions.Default };

        // --- STEP 4: Invoke the Method ---
        // We call the generic method we just constructed.
        // The first argument is 'null' because it's a static method.
        // The second argument is our array of parameters.
        // The result is cast to IComponent so it can be returned.
        return (IComponent)genericMethod.Invoke(null, parameters);
    }
}