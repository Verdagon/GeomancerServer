using System;

using Geomancer.Model;

namespace Geomancer {
  public static class ModelExtensions {
    public const float ModelToUnityMultiplier = .001f;
    
    public static Vec3 ToVec3(this Vec2 vec2) {
      return new Vec3(vec2.x, vec2.y, 0);
    }
  }
}
