using System;
using System.Collections;
using System.Collections.Generic;

using Domino;
using Geomancer.Model;
using SimpleJSON;

namespace Geomancer {
  public class LevelContentsDetailsView {
    private GameToDominoConnection domino;

    private string collapserViewId;
    private string detailsViewId;

    public LevelContentsDetailsView(
        GameToDominoConnection domino) {
      this.domino = domino;

      var expandSidebar = new JSONObject();
      expandSidebar.Add("request", "ExpandLevelContentsListViewRequest");
      var expandDetails = new JSONObject();
      expandDetails.Add("request", "ExpandLevelContentsDetailsViewRequest");

      collapserViewId =
          domino.CreateCollapser(Position.bottom, CollapserStrategy.popup, false,
            domino.CreateContainer("", Direction.vertical, "", new [] {
              domino.CreateButton("", "pi pi-bars", expandDetails)
            }),
            detailsViewId = domino.CreateContainer("336px", Direction.vertical, "", new string[0]));
      for (int i = 0; i < 50; i++) {
        domino.AddView(detailsViewId, domino.CreateLabel("details view!"));
      }
    }

    public string getViewId() { return collapserViewId; }

    public void HandleExpand() {
      Console.WriteLine("Sending open!");
      domino.SetCollapserOpen(collapserViewId, true);
    }
  }
}
