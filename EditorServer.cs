using System;
using System.Collections.Generic;
using System.IO;
using Domino;
using Geomancer.Model;
using SimpleJSON;
using Terrain = Geomancer.Model.Terrain;

namespace Geomancer {
  class MemberStuff {
    public static Dictionary<char, string> memberByKeyCode = new Dictionary<char, string>() {
      ['b'] = "fire",
      ['g'] = "grass",
      ['m'] = "mud",
      ['d'] = "dirt",
      ['r'] = "rocks",
      ['o'] = "obsidian",
      ['s'] = "darkRocks",
      ['x'] = "marker",
      ['c'] = "cave",
      ['f'] = "floor",
      ['t'] = "tree",
      ['l'] = "magma",
      ['h'] = "healthPotion",
      ['p'] = "manaPotion",
      ['w'] = "caveWall",
      ['z'] = "obsidianFloor",
      ['v'] = "avelisk",
      ['y'] = "zeddy",
      ['#'] = "wall",
      ['`'] = "water",
    };
  }
  
  public class EditorServer {
    GameToDominoConnection domino;

    private const int elevationStepHeight = 200;
    private int screenGW = 0;
    private int screenGH = 0;
    private Terrain terrain;

    private string levelContentsListContainerId;
    private LevelContentsListView levelContentsListView;
    private HudView hudView;
    private LevelView levelView;
    private ToolbarView toolbarView;

    TerrainController terrainPresenter;
    private SortedSet<Location> selectedLocations = new SortedSet<Location>();
    private Location maybeHoveredLocation;
    private Location maybeLookedLocation;
    private Dictionary<string, TileStyle> tileStylees;

    public EditorServer(
        string resourcesPath,
        GameToDominoConnection domino,
        int screenGW,
        int screenGH) {
      this.domino = domino;

      this.screenGW = screenGW;
      this.screenGH = screenGH;
      
      var pattern = PentagonPattern9.makePentagon9Pattern();

      terrain = new Geomancer.Model.Terrain(pattern, 200, new SortedDictionary<Location, TerrainTile>());

      using (var fileStream = new FileStream("level.lev", FileMode.OpenOrCreate)) {
        using (var reader = new StreamReader(fileStream)) {
          while (true) {
            string line = reader.ReadLine();
            if (line == null) {
              break;
            }
            if (line == "") {
              continue;
            }
            string[] parts = line.Split(' ');
            int groupX = int.Parse(parts[0]);
            int groupY = int.Parse(parts[1]);
            int indexInGroup = int.Parse(parts[2]);
            int elevation = int.Parse(parts[3]);

            var location = new Location(groupX, groupY, indexInGroup);
            var tile = new TerrainTile(location, elevation, new List<string>());
            terrain.tiles.Add(location, tile);

            for (int i = 4; i < parts.Length; i++) {
              tile.members.Add(parts[i]);
            }
            // if (!tile.members.Contains("Tree")) {
            //   tile.members.Add("Tree");
            // }
          }
        }
      }
      
      if (terrain.tiles.Count == 0) {
        var tile = new TerrainTile(new Location(0, 0, 0), 1, new List<string>());
        terrain.tiles.Add(new Location(0, 0, 0), tile);
      }

      Location startLocation = new Location(0, 0, 0);
      if (!terrain.tiles.ContainsKey(startLocation)) {
        foreach (var locationAndTile in terrain.tiles) {
          startLocation = locationAndTile.Key;
          break;
        }
      }
      
      var cameraLookAtPosition = terrain.GetTileCenter(startLocation);
      Vec3 lookatOffsetToCamera = new Vec3(0, 5000, -10000);
      domino.SetupGame(
          cameraLookAtPosition,
          lookatOffsetToCamera,
          elevationStepHeight, pattern);


      domino.AddView(
          "__rootContainer",
          levelContentsListContainerId =
              domino.CreateContainer("100%", Direction.horizontal, "",
                new string[] {
                  (levelContentsListView = new LevelContentsListView(domino)).getViewId(),
                  domino.CreateContainer("grow", Direction.vertical, "",
                      new string[] {
                      (hudView = new HudView(domino)).getViewId(),
                      domino.CreateContainer("grow", Direction.vertical, "",
                          new string[] {
                          (levelView = new LevelView(domino)).getViewId()
                      })
                  }),
                  (toolbarView = new ToolbarView(domino)).getViewId()
                }));

      var styleMap =
          JsonHarvester.ParseIdToStyleMap(
              JsonHarvester.LoadFromFile(resourcesPath, "vivimap.json"));
      styleMap.Add(
          "_phantom",
          new Style(
              "_phantom",
              new TileStyle(
                  new ConstantVec4iAnimation(new Vec4i(64, 64, 64, 255)),
                  new ConstantVec4iAnimation(new Vec4i(48, 48, 48, 255)),
                  null,
                  null)));
      var idToStyleMap = new Dictionary<string, Style>();
      foreach (var entry in styleMap) {
        var name = entry.Key;
        var style = entry.Value;
        var id = domino.CreateStyle(style);
        idToStyleMap.Add(id, style);
      }

      // private List<(string, IVec4iAnimation)> membersFrontColors = new List<(string, IVec4iAnimation)>();
    // private List<(string, IVec4iAnimation)> membersSideColors = new List<(string, IVec4iAnimation)>();
    // private List<(string, InitialSymbol)> membersFeatures = new List<(string, InitialSymbol)>();
    // private List<(string, InitialSymbol)> membersOverlays = new List<(string, InitialSymbol)>();
    // private List<(string, InitialSymbol)> membersItems = new List<(string, InitialSymbol)>();

      terrainPresenter = new TerrainController(domino, terrain);
      terrainPresenter.PhantomTileClicked += HandlePhantomTileClicked;
      terrainPresenter.TerrainTileClicked += HandleTerrainTileClicked;
      terrainPresenter.TerrainTileHovered += HandleTerrainTileHovered;
    }

    public void SetHoveredLocation(string tileViewId, Location newMaybeHoveredLocation) {
      terrainPresenter.SetHoveredLocation(newMaybeHoveredLocation);
      HandleTerrainTileHovered(newMaybeHoveredLocation);
    }

    public void LocationMouseDown(string tileViewId, Location location) {
      terrainPresenter.LocationMouseDown(tileViewId, location);
    }

    public void KeyDown(int c, bool leftShiftDown, bool rightShiftDown, bool ctrlDown, bool leftAltDown, bool rightAltDown) {
      Console.WriteLine("Got key down: " + c);
      switch (c) {
        case '\u001b':
          SetSelection(new SortedSet<Location>());
          break;
        case '/':
          var allLocations = new SortedSet<Location>();
          foreach (var locationAndTile in terrainPresenter.terrain.tiles) {
            allLocations.Add(locationAndTile.Key);
          }
          SetSelection(allLocations);
          break;
        case 'z':
          // var panelId = domino.MakePanel("", -1, -1, screenGW + 2, screenGH + 2);
          // var rect = domino.AddRectangle(panelId, 0, 0, screenGW + 2, screenGH + 2, 0, Vec4i.black, Vec4i.black);
          // GameToDominoConnection.IEventHandler remove = null;
          // var onClick = domino.MakeEvent(() => {
          //   Console.WriteLine("clicked!");
          //   remove();
          // });
          // var onMouseIn = domino.MakeEvent(() => Console.WriteLine("mouse in!"));
          // var onMouseOut = domino.MakeEvent(() => Console.WriteLine("mouse out!"));
          // var button = domino.AddButton(
          //     rect, 2, 2, 15, 4, 0, new Vec4i(153, 153, 153, 255), new Vec4i(153, 153, 153, 255), new Vec4i(102, 102, 102, 255), onClick, onMouseIn, onMouseOut);
          // var strIds = domino.AddString(button, 3, 3, 15, Vec4i.cyan, "Cascadia", "hello");
          // remove = () => {
          //   foreach (var id in strIds) {
          //     domino.RemoveView(id);
          //   }
          //   domino.RemoveView(button);
          //   domino.DestroyEvent(onMouseOut);
          //   domino.DestroyEvent(onMouseIn);
          //   domino.DestroyEvent(onClick);
          //   domino.RemoveView(rect);
          //   domino.RemoveView(panelId);
          // };
          break;
        case '=':
        case '+':
        case -2:
          foreach (var loc in selectedLocations) {
            terrainPresenter.GetTilePresenter(loc).SetElevation(terrainPresenter.terrain.tiles[loc].elevation + 1);
          }
          Save();
          UpdateLookPanelView();
          break;
        case '-':
        case '_':
        case -1:
          foreach (var loc in selectedLocations) {
            terrainPresenter.GetTilePresenter(loc).SetElevation(
                Math.Max(1, terrainPresenter.terrain.tiles[loc].elevation - 1));
          }
          Save();
          UpdateLookPanelView();
          break;
        case '\u007F':
          foreach (var loc in new SortedSet<Location>(selectedLocations)) {
            selectedLocations.Remove(loc);
            var tile = terrainPresenter.terrain.tiles[loc];
            terrainPresenter.terrain.tiles.Remove(loc);
            tile.Destruct();
          }
          Save();
          UpdateLookPanelView();
          break;
      }

      foreach (var keyCodeAndMember in MemberStuff.memberByKeyCode) {
        if (c == keyCodeAndMember.Key) {
          bool addKeyDown = rightAltDown;
          bool removeKeyDown = leftAltDown;
          ChangeMember(keyCodeAndMember.Value, addKeyDown, removeKeyDown);
          Save();
        }
        UpdateLookPanelView();
      }
    }

    public void HandlePhantomTileClicked(Location location) {
      var terrainTile = new TerrainTile(location, 1, new List<string>());
      terrainPresenter.AddTile(terrainTile);
      Save();

      var newSelection = new SortedSet<Location>(selectedLocations);
      newSelection.Add(location);
      SetSelection(newSelection);
    }

    public void HandleTerrainTileClicked(Location location) {
      var newSelection = new SortedSet<Location>(selectedLocations);
      if (newSelection.Contains(location)) {
        newSelection.Remove(location);
      } else {
        newSelection.Add(location);
      }
      SetSelection(newSelection);
    }

    public void HandleTerrainTileHovered(Location location) {
      UpdateLookPanelView();
    }

    public void HandleCustomRequest(JSONObject obj) {
      var type = JsonHarvester.ExpectMemberString(obj, "request");
      switch (type) {
        case "ExpandLevelContentsListViewRequest":
          this.levelContentsListView.HandleExpand();
          break;
        case "ExpandLevelContentsDetailsViewRequest":
          this.levelContentsListView.HandleExpandDetails();
          break;
        default:
          throw new Exception("Unknown custom request: " + type);
      }
    }

    private void Save() {
      using (var fileStream = new FileStream("level.lev", FileMode.Create)) {
        using (var writer = new StreamWriter(fileStream)) {
          Save(writer);
        }
      }

      var timestamp = (int) new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
      using (var fileStream = new FileStream("level" + timestamp + ".lev", FileMode.Create)) {
        using (var writer = new StreamWriter(fileStream)) {
          Save(writer);
        }
      }
    }

    private void Save(StreamWriter writer) {
      foreach (var locAndTile in terrainPresenter.terrain.tiles) {
        var loc = locAndTile.Key;
        var tile = locAndTile.Value;
        string line = loc.groupX + " " + loc.groupY + " " + loc.indexInGroup + " " + tile.elevation;
        foreach (var member in tile.members) {
          line += " " + member;
        }
        writer.WriteLine(line);
      }
      writer.Close();
    }

    private void SetSelection(SortedSet<Location> locations) {
      selectedLocations = locations;
      terrainPresenter.SetHighlightedLocations(selectedLocations);

      SortedSet<string> commonMembers = null;
      foreach (var loc in selectedLocations) {
        if (commonMembers == null) {
          commonMembers = new SortedSet<string>();
          foreach (var member in terrainPresenter.terrain.tiles[loc].members) {
            commonMembers.Add(member);
          }
        } else {
          var members = new SortedSet<string>();
          foreach (var member in terrainPresenter.terrain.tiles[loc].members) {
            members.Add(member);
          }
          foreach (var member in new SortedSet<string>(commonMembers)) {
            if (!members.Contains(member)) {
              commonMembers.Remove(member);
            }
          }
        }
      }

      // var entries = new List<ListView.Entry>();
      // if (commonMembers != null) {
      //   foreach (var member in commonMembers) {
      //     entries.Add(
      //         new ListView.Entry(
      //             new SymbolId("AthSymbols", 0x0072),
      //             new Vec4i(255, 255, 255, 255),
      //             member,
      //             new Vec4i(255, 255, 255, 255)));
      //   }
      // }
      // membersView.ShowEntries(entries);
    }
    
    private void UpdateLookPanelView() {
      var location = terrainPresenter.GetMaybeMouseHighlightLocation();
      if (location != maybeLookedLocation) {
        maybeLookedLocation = location;
        if (location == null) {
          // lookPanelView.SetStuff(false, "", "", new List<KeyValuePair<InitialSymbol, string>>());
        } else {
          var message = "(" + location.groupX + ", " + location.groupY + ", " + location.indexInGroup + ")";

          var symbolsAndDescriptions = new List<KeyValuePair<InitialSymbol, string>>();
          if (terrainPresenter.terrain.tiles.ContainsKey(location)) {
            message += " elevation " + terrainPresenter.terrain.tiles[location].elevation;
            foreach (var member in terrainPresenter.terrain.tiles[location].members) {
              var symbol =
                  new InitialSymbol(
                      new InitialSymbolGlyph(
                          new SymbolId("AthSymbols", 0x0072),
                          new ConstantVec4iAnimation(new Vec4i(255, 255, 255, 255))),
                      new InitialSymbolOutline(
                          OutlineMode.OuterOutline,
                          new ConstantVec4iAnimation(Vec4i.black)),
                      null,
                      180,
                      100);
              symbolsAndDescriptions.Add(new KeyValuePair<InitialSymbol, string>(symbol, member));
            }
          }

          // lookPanelView.SetStuff(true, message, "", symbolsAndDescriptions);
        }
      }
    }
    
    private void ChangeMember(string member, bool addKeyDown, bool removeKeyDown) {
      if (addKeyDown && removeKeyDown) {
        return;
      } else if (addKeyDown) {
        // Add one to each tile
        foreach (var location in selectedLocations) {
          terrainPresenter.GetTilePresenter(location).AddMember(member);
        }
      } else if (removeKeyDown) {
        foreach (var location in selectedLocations) {
          if (LocationHasMember(location, member)) {
            terrainPresenter.GetTilePresenter(location).RemoveMember(member);
          }
        }
      } else {
        // Toggle; ensure it's there if its not
        if (!AllLocationsHaveMember(selectedLocations, member)) {
          foreach (var location in selectedLocations) {
            // Add it if its not already there
            if (!LocationHasMember(location, member)) {
              terrainPresenter.GetTilePresenter(location).AddMember(member);
            }
          }
        } else {
          foreach (var location in selectedLocations) {
            // Remove all of them that are present
            while (LocationHasMember(location, member)) {
              terrainPresenter.GetTilePresenter(location).RemoveMember(member);
            }
          }
        }
      }
    }

    private bool AllLocationsHaveMember(SortedSet<Location> locations, string member) {
      foreach (var location in locations) {
        if (!LocationHasMember(location, member)) {
          return false;
        }
      }
      return true;
    }

    private bool LocationHasMember(Location location, string member) {
      foreach (var hayMember in terrainPresenter.terrain.tiles[location].members) {
        if (member == hayMember) {
          return true;
        }
      }
      return false;
    }
  }
}
