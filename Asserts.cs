using System;
using System.Runtime.CompilerServices;

// namespace Geomancer.Scripts {
  public static class Asserts {
    public static void Assert(
        bool condition,
        string message = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0) {
      if (!condition) {
        throw new Exception("Error at " + sourceFilePath + ":" + sourceLineNumber + " " + memberName + ": " + message);
      }
    }
  }
// }
