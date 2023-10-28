using System.Collections;
using System.Collections.Generic;

using Domino;
using Geomancer.Model;
using SimpleJSON;

namespace Geomancer {
  public class HudView {
    private GameToDominoConnection domino;

    private string containerViewId;

    public HudView(
        GameToDominoConnection domino) {
      this.domino = domino;

      containerViewId = domino.CreateContainer("60px", Direction.vertical, "", new string[0]);
    }

    public string getViewId() { return containerViewId; }
  }
}
