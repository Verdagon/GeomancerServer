using System.Collections;
using System.Collections.Generic;

using Domino;
using Geomancer.Model;

// namespace Geomancer {
//   public class ListView {
//     public class Entry {
//       public SymbolId symbol;
//       public string text;
//
//       public Entry(SymbolId symbol, string text) {
//         this.symbol = symbol;
//         this.text = text;
//       }
//     }
//
//     private GameToDominoConnection domino;
//
//     private string panelId = "";
//     // private Panel panel;
//     private readonly int viewGW, viewGH;
//     private List<string> descendantIds;
//
//     //IClock cinematicTimer;
//     //OverlayPaneler overlayPaneler;
//     // OverlayPanelView view;
//
//     public ListView(
//         GameToDominoConnection domino,
//         // ulong viewId,
//         int x,
//         int y,
//         int viewGW,
//         int viewGH) {//OverlayPanelView view) {
//       this.domino = domino;
//       this.viewGW = viewGW;
//       this.viewGH = viewGH;
//       this.panelId = domino.MakePanel(x, y, viewGW, viewGH);
//       descendantIds = new List<string>();
//       //this.cinematicTimer = cinematicTimer;
//       //this.overlayPaneler = overlayPaneler;
//     }
//
//     public void ShowEntries(List<Entry> entries) {
//       foreach (var descendantId in descendantIds) {
//         domino.RemoveView(descendantId);
//       }
//       descendantIds.Clear();
//       // panel.Clear();
//
//       if (entries.Count > 0) {
//         descendantIds.Add(
//           domino.AddRectangle(panelId, -1, -1, viewGW + 2, viewGH, 0, new Vec4i(0, 0, 0, 230), new Vec4i(0, 0, 0, 0)));
//
//         for (int i = 0; i < entries.Count; i++) {
//           // view.AddSymbol(0, 1, view.symbolsHigh - (i * 2 + 2), 2.0f, 0, new Color(1, 1, 1), entries[i].symbol);
//           descendantIds.AddRange(
//             domino.AddString(panelId, 5, viewGH - (i * 2 + 2000 - 500), viewGW - 3, new Vec4i(255, 255, 255, 255), "Cascadia", entries[i].text));
//         }
//       }
//     }
//   }
// }
