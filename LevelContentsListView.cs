using System;
using System.Collections;
using System.Collections.Generic;

using Domino;
using Geomancer.Model;
using SimpleJSON;

namespace Geomancer {
  public class LevelContentsListView {
    private GameToDominoConnection domino;

    private string collapserViewId;
    private string listViewId;
    private LevelContentsDetailsView detailsCollapserView;

    public LevelContentsListView(
        GameToDominoConnection domino) {
      this.domino = domino;

      var expandSidebar = new JSONObject();
      expandSidebar.Add("request", "ExpandLevelContentsListViewRequest");
      var expandDetails = new JSONObject();
      expandDetails.Add("request", "ExpandLevelContentsDetailsViewRequest");

      collapserViewId =
          domino.CreateCollapser(
              Position.left, CollapserStrategy.sidebar, false,
              domino.CreateContainer("", Direction.horizontal, "", new [] {
                domino.CreateButton("", "pi pi-bars", expandSidebar)
              }),
              domino.CreateContainer("336px", Direction.vertical, "", new [] {
                listViewId = domino.CreateTree(new string[] { }),
                (detailsCollapserView = new LevelContentsDetailsView(domino)).getViewId()
              }));

// show in left:
// - selections (clipboard selection first)
//   - update button
//   - select button
//   - intersect
//   - union
//   - set color
// - units, named ones first
//   - components, can set their parameters
// - locations, shown as groups
// - palettes (new, reference)
//   - any imported palettes
// - other levels (titles only) (new button)
//   - switch button
// if small, only show flat list of icons, click for tiny popup to right w buttons?
//
// right toolbar:
// - save new selection
// - selection mode (square vs drag)
// - undo
// - redo
// - select all
// - change perspective (45, 0, -45, above)
// - algorithms: (will show on bottom, commit button on each)
//   - if selection:
//     - swap with clipboard
//     - expand/contract
//     - copy selection to clipboard
//     - filter
//   - fill
//     - percentage
//     - component
//   - average elevation
//     - amount slider
//   - change elevation
//     - area selection
//     - peaks selection (if none, then center)
//     - uniform vs normal vs triangular slider
//     - amount slider
//     - jaggedness/noise slider
//   - cellular automata
//     - gens
//     - threshold
//     - seed
//
// bottom:
// - 10 recent components to add or algorithms, collapsed numbered
    }

    public string getViewId() { return collapserViewId; }

    public void HandleExpand() {
      Console.WriteLine("Sending open!");
      domino.SetCollapserOpen(collapserViewId, true);
    }
    public void HandleExpandDetails() {
      detailsCollapserView.HandleExpand();
    }
  }
}
