using System.Collections;
using System.Collections.Generic;

using Domino;
using Geomancer.Model;
using SimpleJSON;

namespace Geomancer {
  public class LevelView {
    private GameToDominoConnection domino;

    private string containerViewId;

    public LevelView(
        GameToDominoConnection domino) {
      this.domino = domino;

      containerViewId = domino.CreateStageContainer();
    }

    public string getViewId() { return containerViewId; }
  }
}
