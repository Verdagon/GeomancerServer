using System;
using System.Collections;
using System.Collections.Generic;

using Domino;
using Geomancer.Model;
using SimpleJSON;

namespace Geomancer {
  public class ToolbarView {
    private GameToDominoConnection domino;

    private string collapserViewId;
    private string listViewId;
    private LevelContentsDetailsView detailsCollapserView;

    public ToolbarView(
        GameToDominoConnection domino) {
      this.domino = domino;

      var expandSidebar = new JSONObject();
      expandSidebar.Add("request", "ExpandToolbarViewRequest");
      var expandDetails = new JSONObject();
      expandDetails.Add("request", "ExpandLevelContentsDetailsViewRequest");

      collapserViewId =
          domino.CreateCollapser(
              Position.left, CollapserStrategy.sidebar, true,
              domino.CreateContainer("", Direction.vertical, "2px", new [] {
                domino.CreateButton("", "pi pi-plus", expandSidebar),
                domino.CreateButton("", "pi pi-circle", expandSidebar),
                domino.CreateButton("", "pi pi-backward", expandSidebar),
                domino.CreateButton("", "pi pi-forward", expandSidebar),
                domino.CreateButton("", "pi pi-bars", expandSidebar),
                domino.CreateButton("", "pi pi-undo", expandSidebar),
                domino.CreateButton("", "pi pi-sort", expandSidebar),
                domino.CreateButton("", "pi pi-bars", expandSidebar),
                domino.CreateButton("", "pi pi-bars", expandSidebar),
                domino.CreateButton("", "pi pi-clone", expandSidebar),
                domino.CreateButton("", "pi pi-filter", expandSidebar),
                domino.CreateButton("", "pi pi-percentage", expandSidebar),
                domino.CreateButton("", "pi pi-bars", expandSidebar),
                domino.CreateButton("", "pi pi-sitemap", expandSidebar),
                domino.CreateButton("", "pi pi-map", expandSidebar),
              }),
              domino.CreateContainer("200px", Direction.vertical, "2px", new[] {
                domino.CreateButton("Save selection", "pi pi-plus", expandSidebar),
                domino.CreateButton("Square select", "pi pi-circle", expandSidebar),
                domino.CreateButton("Undo", "pi pi-backward", expandSidebar),
                domino.CreateButton("Redo", "pi pi-forward", expandSidebar),
                domino.CreateButton("Select all", "pi pi-bars", expandSidebar),
                domino.CreateButton("Rotate view", "pi pi-undo", expandSidebar),
                domino.CreateButton("Top-down view", "pi pi-sort", expandSidebar),
                domino.CreateButton("Swap selection", "pi pi-bars", expandSidebar),
                domino.CreateButton("Grow/shrink", "pi pi-bars", expandSidebar),
                domino.CreateButton("Copy selection", "pi pi-clone", expandSidebar),
                domino.CreateButton("Filter selection", "pi pi-filter", expandSidebar),
                domino.CreateButton("Fill", "pi pi-percentage", expandSidebar),
                domino.CreateButton("Average elevation", "pi pi-bars", expandSidebar),
                domino.CreateButton("Add/subtract elev.", "pi pi-sitemap", expandSidebar),
                domino.CreateButton("Cellular automata", "pi pi-map", expandSidebar),
              }));

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
