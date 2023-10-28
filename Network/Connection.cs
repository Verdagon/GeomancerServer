using System;
using System.Collections.Generic;
using Geomancer;
using Geomancer.Model;
using SimpleJSON;

namespace Domino {

  public class GameToDominoConnection {
    public delegate void IEventHandler();
    public interface IEvent {}
    public class Event : IEvent, IDisposable {
      public string id { get; private set; }
      public IEventHandler handler { get; private set; }
      public Event(string id, IEventHandler handler) {
        this.id = id;
        this.handler = handler;
      }
      public void Dispose() {
        Asserts.Assert(handler != null);
        id = "";
        handler = null;
      }
      public void Trigger() {
        Asserts.Assert(handler != null);
        handler();
      }
    }

    // public DominoToGameConnection otherSide;
    // public EditorServer server;

    private List<IDominoMessage> messages = new List<IDominoMessage>();
    private ulong nextId = 1;

    private Dictionary<string, Event> events;

    public GameToDominoConnection() {
      events = new Dictionary<string, Event>();
      // this.otherSide = otherSide;
      // server = new EditorServer(this);
    }

    // public string MakeList(
    //     string parentId,
    //     int panelGXInScreen,
    //     int panelGYInScreen,
    //     int panelGW,
    //     int panelGH) {
    //   string id = (nextId++).ToString();
    //   messages.Add(new MakeListMessage(id, parentId, panelGXInScreen, panelGYInScreen, panelGW, panelGH));
    //   return id;
    //   // return new Panel(this, id, panelGW, panelGH);
    // }

    public IEvent MakeEvent(IEventHandler handler) {
      var id = (nextId++).ToString();
      var e = new Event(id, handler);
      events.Add(id, e);
      return e;
    }
    public Event DestroyEvent(IEvent ie) {
      var e = ie as Event;
      Asserts.Assert(e != null);
      events.Remove(e.id);
      e.Dispose();
      return e;
    }

    public void TriggerEvent(string id) {
      if (events.TryGetValue(id, out var e)) {
        e.Trigger();
      } else {
        throw new Exception("Unknown event triggered: " + id);
      }
    }

    public string CreateStyle(Style style) {
      string id = (nextId++).ToString();
      messages.Add(new CreateStyleMessage(id, style));
      return id;
    }

    public string CreateTile(InitialTile initialTile) {
      string id = (nextId++).ToString();
      messages.Add(new CreateTileMessage(id, initialTile));
      return id;
    }

    public string CreateUnit(InitialUnit initialUnit) {
      string id = (nextId++).ToString();
      messages.Add(new CreateUnitMessage(id, initialUnit));
      return id;
    }

    public void SetupGame(Vec3 cameraPosition, Vec3 lookatOffsetToCamera, int elevationStepHeight, Pattern pattern) {
      messages.Add(new SetupGameMessage(cameraPosition, lookatOffsetToCamera, elevationStepHeight, pattern));
    }

    public void ShowPrism(string tileViewId, InitialSymbol prismDescription, InitialSymbol prismOverlayDescription) {
      messages.Add(new ShowPrismMessage(tileViewId, prismDescription, prismOverlayDescription));
    }
    public void FadeInThenOut(string tileViewId, long inDurationMs, long outDurationMs) {
      messages.Add(new FadeInThenOutMessage(tileViewId, inDurationMs, outDurationMs));
    }
    public void ShowRune(string tileViewId, InitialSymbol runeSymbolDescription) {
      messages.Add(new ShowRuneMessage(tileViewId, runeSymbolDescription));
    }
    public void SetOverlay(string tileViewId, InitialSymbol maybeOverlay) {
      messages.Add(new SetOverlayMessage(tileViewId, maybeOverlay));
    }
    public void SetFeature(string tileViewId, InitialSymbol maybeFeature) {
      messages.Add(new SetFeatureMessage(tileViewId, maybeFeature));
    }
    public void SetCliffColor(string tileViewId, IVec4iAnimation wallColor) {
      messages.Add(new SetCliffColorMessage(tileViewId, wallColor));
    }
    public void SetSurfaceColor(string tileViewId, IVec4iAnimation frontColor) {
      messages.Add(new SetSurfaceColorMessage(tileViewId, frontColor));
    }
    public void SetElevation(string tileViewId, int elevation) {
      messages.Add(new SetElevationMessage(tileViewId, elevation));
    }
    public void RemoveItem(string tileViewId, string id) {
      messages.Add(new RemoveItemMessage(tileViewId, id));
    }
    public void ClearItems(string tileViewId) {
      messages.Add(new ClearItemsMessage(tileViewId));
    }
    public void AddItem(string tileViewId, string itemId, InitialSymbol symbolDescription) {
      messages.Add(new AddItemMessage(tileViewId, itemId, symbolDescription));
    }
    public void DestroyTile(string tileViewId) {
      messages.Add(new DestroyTileMessage(tileViewId));
    }

    public void DestroyUnit(string unitViewId) {
      messages.Add(new DestroyUnitMessage(unitViewId));
    }

    public void SetFadeOut(string id, FadeOut fadeOut) {
      messages.Add(new SetFadeOutMessage(id, fadeOut));
    }

    public void SetFadeIn(string id, FadeIn fadeIn) {
      messages.Add(new SetFadeInMessage(id, fadeIn));
    }

    public string AddView(string parentId, string id) {
      messages.Add(new AddViewMessage(parentId, id));
      return id;
    }

    public string CreateContainer(
        string length,
        Direction direction,
        string childMargin,
        string[] childIds) {
      string id = (nextId++).ToString();
      messages.Add(new CreateContainerMessage(id, length, direction, childMargin, childIds));
      return id;
    }

    public string CreateStageContainer() {
      string id = "__stage";
      messages.Add(new CreateContainerMessage(id, "grow", Direction.vertical, "", new string[0]));
      return id;
    }

    public string CreateLabel(string text) {
      string id = (nextId++).ToString();
      messages.Add(new CreateLabelMessage(id, text));
      return id;
    }

    public string CreateButton(string label, string icon, JSONObject data) {
      string id = (nextId++).ToString();
      messages.Add(new CreateButtonMessage(id, label, icon, data));
      return id;
    }

    public string CreateTree(string[] nodeIds) {
      string id = (nextId++).ToString();
      messages.Add(new CreateTreeMessage(id, nodeIds));
      return id;
    }

    public string CreateTreeNode(string rowId, string[] childIds) {
      string id = (nextId++).ToString();
      messages.Add(new CreateTreeNodeMessage(id, rowId, childIds));
      return id;
    }

    public string CreateCollapser(Position position, CollapserStrategy strategy, bool large, string collapsedId, string expandedId) {
      string id = (nextId++).ToString();
      messages.Add(new CreateCollapserMessage(id, strategy, large, position, collapsedId, expandedId));
      return id;
    }

    public string SetCollapserOpen(string id, bool open) {
      messages.Add(new SetCollapserOpenMessage(id, open));
      return id;
    }

    public void ScheduleClose(string viewId, long startMsFromNow) {
      messages.Add(new ScheduleCloseMessage(viewId, startMsFromNow));
    }

    public void RemoveView(string viewId) {
      messages.Add(new RemoveViewMessage(viewId));
    }

    public void SetOpacity(string viewId, int id, int percent) {
      messages.Add(new SetOpacityMessage(viewId, id, percent));
    }
    //
    // public List<string> AddString(
    //     string parentViewId,
    //     int x,
    //     int y,
    //     int maxWide,
    //     Vec4i color,
    //     string fontName,
    //     string str) {
    //   List<string> newViewIds = new List<string>();
    //   for (int i = 0; i < str.Length; i++) {
    //     newViewIds.Add(AddSymbol(parentViewId, x + i, y, 1, 1, color, new SymbolId(fontName, char.ConvertToUtf32(str[i].ToString(), 0)), true));
    //   }
    //   return newViewIds;
    // }
    //
    // public string AddButton(
    //     string parentViewId,
    //     int x,
    //     int y,
    //     int width,
    //     int height,
    //     int z,
    //     Vec4i color,
    //     Vec4i borderColor,
    //     Vec4i pressedColor,
    //     IEvent onClickedI,
    //     IEvent onMouseInI,
    //     IEvent onMouseOutI) {
    //   var onClicked = onClickedI as Event;
    //   Asserts.Assert(onClicked != null);
    //   var onMouseIn = onMouseInI as Event;
    //   Asserts.Assert(onMouseIn != null);
    //   var onMouseOut = onMouseOutI as Event;
    //   Asserts.Assert(onMouseOut != null);
    //
    //   string newViewId = (nextId++).ToString();
    //   messages.Add(
    //       new AddButtonMessage(
    //           newViewId, parentViewId, x, y, width, height, z, color, borderColor, pressedColor, onClicked.id,
    //           onMouseIn.id, onMouseOut.id));
    //   return newViewId;
    // }
    //
    // // public string AddFullscreenRect(string parentViewId, Color color) {
    // //   ulong newViewId = (nextId++).ToString();
    // //   messages.Add(new AddFullscreenRectMessage(newViewId, parentViewId, color));
    // //   return newViewId;
    // // }
    //
    // public string AddRectangle(
    //     string parentViewId,
    //     int x,
    //     int y,
    //     int width,
    //     int height,
    //     int z,
    //     Vec4i color,
    //     Vec4i borderColor) {
    //   string newViewId = (nextId++).ToString();
    //   messages.Add(
    //       new AddRectangleMessage(newViewId, parentViewId, x, y, width, height, z, color, borderColor));
    //   return newViewId;
    // }
    //
    // public string AddSymbol(
    //     string parentViewId,
    //     int x,
    //     int y,
    //     int size,
    //     int z,
    //     Vec4i color,
    //     SymbolId symbol,
    //     bool centered) {
    //   string newViewId = (nextId++).ToString();
    //   messages.Add(new AddSymbolMessage(newViewId, parentViewId, x, y, size, z, color, symbol, centered));
    //   return newViewId;
    // }
    //
    // public string AddInlineSymbol(
    //     string parentViewId,
    //     Vec4i color,
    //     SymbolId symbolId) {
    //   string newViewId = (nextId++).ToString();
    //   messages.Add(new AddInlineSymbolMessage(newViewId, parentViewId, color, symbolId));
    //   return newViewId;
    // }
    //
    // public string AddInlineString(
    //     string parentViewId,
    //     Vec4i color,
    //     string text) {
    //   string newViewId = (nextId++).ToString();
    //   messages.Add(new AddInlineStringMessage(newViewId, parentViewId, color, text));
    //   return newViewId;
    // }
    //
    // public string AddInlineSpan(
    //     string parentViewId,
    //     Vec4i color) {
    //   string newViewId = (nextId++).ToString();
    //   messages.Add(new AddInlineSpanMessage(newViewId, parentViewId, color));
    //   return newViewId;
    // }

    public List<IDominoMessage> TakeMessages() {
      var copy = new List<IDominoMessage>(messages);
      messages.Clear();
      return copy;
    }
  }
}
