using System;
using System.Collections.Generic;

using Geomancer.Model;

using Domino;

namespace Geomancer {
  public class TerrainTilePresenter {
    private GameToDominoConnection domino;
    // private MemberToViewMapperOld vivimap;
    public readonly Location location;
    private TerrainTile terrainTile;
    private Geomancer.Model.Terrain terrain;

    string tileViewId = "";
    string unitViewId = "";

    private ulong nextMemberId = 1;
    // (member ID, member string)
    private List<(string, string)> members = new List<(string, string)>();
    
    // (member ID, value)
    // private List<(string, IVec4iAnimation)> membersFrontColors = new List<(string, IVec4iAnimation)>();
    // private List<(string, IVec4iAnimation)> membersSideColors = new List<(string, IVec4iAnimation)>();
    // private List<(string, InitialSymbol)> membersFeatures = new List<(string, InitialSymbol)>();
    // private List<(string, InitialSymbol)> membersOverlays = new List<(string, InitialSymbol)>();
    private List<(string, InitialSymbol)> membersItems = new List<(string, InitialSymbol)>();
    private List<(string, InitialSymbol)> membersDominoSymbols = new List<(string, InitialSymbol)>();
    private List<(string, IVec4iAnimation)> membersDominoColors = new List<(string, IVec4iAnimation)>();
    private List<(string, InitialSymbol)> membersUnitFaces = new List<(string, InitialSymbol)>();
    private List<(string, InitialSymbol)> membersDetails = new List<(string, InitialSymbol)>();

    private bool highlighted;
    private bool selected;

    public TerrainTilePresenter(
        GameToDominoConnection domino,
        // MemberToViewMapperOld vivimap,
        Geomancer.Model.Terrain terrain,
        Location location,
        TerrainTile terrainTile) {
      this.domino = domino;
      // this.vivimap = vivimap;
      this.location = location;
      this.terrainTile = terrainTile;
      this.terrain = terrain;

      // var eternalMemberId = nextMemberId++;
      // membersFrontColors.Add((eternalMemberId, ConstantVec4iAnimation.blue));
      // membersSideColors.Add((eternalMemberId, ConstantVec4iAnimation.blue));

      foreach (var member in terrainTile.members) {
        OnAddMember(member);
      }

      // var patternTile = terrain.pattern.patternTiles[location.indexInGroup];
      // var pattern = terrain.pattern;

      var initialTileDescription =
          new InitialTile(
              location,
              terrainTile.elevation,
              terrainTile.members,
              // CalculateTintedFrontColor(membersFrontColors[membersFrontColors.Count - 1].Item2, selected, highlighted),
              // membersSideColors[membersSideColors.Count - 1].Item2,
              // CalculateMaybeOverlay(membersOverlays),
              // CalculateMaybeFeature(membersFeatures),
              membersItems);
      tileViewId = domino.CreateTile(initialTileDescription);
      
      RefreshUnit();
      
      // var position = CalculatePosition(terrain.elevationStepHeight, terrain.pattern, location, terrainTile.elevation);
      //   var tile = terrain.tiles[location];
      //   int lowestNeighborElevation = tile.elevation;
      //   foreach (var neighborLoc in pattern.GetAdjacentLocations(tile.location, false)) {
      //     if (terrain.TileExists(neighborLoc)) {
      //       lowestNeighborElevation = Math.Min(lowestNeighborElevation, terrain.tiles[neighborLoc].elevation);
      //     } else {
      //       lowestNeighborElevation = 0;
      //     }
      //   }
      //   int depth = Math.Max(1, tile.elevation - lowestNeighborElevation);
      //
      //   var patternTile = pattern.patternTiles[tile.location.indexInGroup];
      //
      //   var highlighted = false;
      //   var frontColor = highlighted ? new Color(.1f, .1f, .1f) : new Color(0f, 0, 0f);
      //   var wallColor = highlighted ? new Color(.1f, .1f, .1f) : new Color(0f, 0, 0f);
      //
      // var patternTileIndex = location.indexInGroup;
      // var shapeIndex = pattern.patternTiles[patternTileIndex].shapeIndex;
      // //   var radianards = pattern.patternTiles[patternTileIndex].rotateRadianards;
      // //   var radians = radianards * 0.001f;
      // //   var degrees = (float)(radians * 180f / Math.PI);
      // //   var rotation = Quaternion.AngleAxis(-degrees, Vector3.up);
      // var unityElevationStepHeight = terrain.elevationStepHeight * ModelExtensions.ModelToUnityMultiplier;
    }

    // private bool initialized => tileViewId != 0;

    // private InitialTile MakeInitialTile() {
    //   
    // }

    private static Vec3 CalculatePosition(int elevationStepHeight, Pattern pattern, Location location, int elevation) {
      var positionVec2 = pattern.GetTileCenter(location);
      var positionVec3 = new Vec3(positionVec2.x, positionVec2.y, elevation * elevationStepHeight);
      return positionVec3;
    }

    private void AddOrRemoveMember(bool control, string name) {
      if (control) {
        this.members.Add(((this.nextMemberId++).ToString(), name));
      } else {
        for (int i = this.members.Count - 1; i >= 0; i--) {
          if (this.members[0].Item2 == name) {
            this.members.RemoveAt(i);
          }
        }
      }
    }

    public void SetHighlighted(bool highlighted) {
      this.highlighted = highlighted;
      AddOrRemoveMember(highlighted, ".highlighted");
    }
    public void SetSelected(bool selected) {
      this.selected = selected;
      AddOrRemoveMember(highlighted, ".selected");
    }

    // private void RefreshSurfaceColor() {
    //   domino.SetSurfaceColor(
    //       tileViewId,
    //       CalculateTintedFrontColor(
    //           membersFrontColors[membersFrontColors.Count - 1].Item2, selected, highlighted));
    // }
    //
    // private void RefreshSideColor() {
    //   domino.SetCliffColor(tileViewId, membersSideColors[membersSideColors.Count - 1].Item2);
    // }
    
    // private void RefreshFeature() {
    //   domino.SetFeature(tileViewId, membersFeatures.Count == 0 ? null : membersFeatures[membersFeatures.Count - 1].Item2);
    // }
    
    private void RefreshUnit() {
      if (this.unitViewId != "") {
        domino.DestroyUnit(this.unitViewId);
        this.unitViewId = "";
      }
      if (this.unitViewId == "" && membersUnitFaces.Count > 0) {
        // var defaultColor = new Color(102, 102, 0, 1);
        var defaultSymbol =
                new InitialSymbol(
                    new InitialSymbolGlyph(
                        new SymbolId("AthSymbols", 0x006a),
                        ConstantVec4iAnimation.red),
                    null,
                    null,
                    0,
                    100);
        
        // var position = CalculatePosition(terrain.elevationStepHeight, terrain.pattern, location, terrainTile.elevation);
        var initialUnit =
            new InitialUnit(
                location,
                membersDominoSymbols.Count > 0 ? membersDominoSymbols[membersDominoSymbols.Count - 1].Item2 : defaultSymbol,
                membersUnitFaces[membersUnitFaces.Count - 1].Item2,
                membersDetails,
                1,
                1);
        this.unitViewId = domino.CreateUnit(initialUnit);
      }
    }

    private void RefreshDomino() {
      // TODO: replace this with a call to unitView.SetDomino
      RefreshUnit();
    }

    private void RefreshUnitFace() {
      // TODO: replace this with a call to unitView.SetFace
      RefreshUnit();
    }

    // private void RefreshOverlay() {
    //   domino.SetOverlay(
    //       tileViewId, membersOverlays.Count == 0 ? null : membersOverlays[membersOverlays.Count - 1].Item2);
    // }
    
    private void RefreshItems() {
      domino.ClearItems(tileViewId);
      foreach (var x in membersItems) {
        domino.AddItem(tileViewId, x.Item1, x.Item2);
      }
    }

    private void RefreshDetails() {
      // unitView.ClearDetails();
      // foreach (var x in membersDetails) {
      //   unitView.AddItem(x.Item1, x.Item2);
      // }
      // TODO put the above in
      RefreshUnit();
    }

    private void OnAddMember(string member) {
      ulong memberId = nextMemberId++;
      members.Add((memberId.ToString(), member));
      // var visitor = new AttributeAddingVisitor(this, memberId);
      // foreach (var thing in vivimap.getEntries(member)) {
      //   if (thing is MemberToViewMapperOld.TopColorDescriptionForIDescription topColor) {
      //     membersFrontColors.Add((memberId, topColor.color));
      //     if (initialized) {
      //       if (tileViewId != 0) {
      //         RefreshSurfaceColor();
      //       }
      //     }
      //   } else if (thing is MemberToViewMapperOld.SideColorDescriptionForIDescription wallColor) {
      //     membersSideColors.Add((memberId, wallColor.color));
      //     if (initialized) {
      //       if (tileViewId != 0) {
      //         RefreshSideColor();
      //       }
      //     }
      //   } else if (thing is MemberToViewMapperOld.OverlayDescriptionForIDescription overlay) {
      //     membersOverlays.Add((memberId, overlay.symbol));
      //     if (initialized) {
      //       if (tileViewId != 0) {
      //         RefreshOverlay();
      //       }
      //     }
      //   } else if (thing is MemberToViewMapperOld.FeatureDescriptionForIDescription feature) {
      //     membersFeatures.Add((memberId, feature.symbol));
      //     if (initialized) {
      //       if (tileViewId != 0) {
      //         RefreshFeature();
      //       }
      //     }
      //   } else if (thing is MemberToViewMapperOld.DominoShapeDescriptionForIDescription dominoShape) {
      //     membersDominoSymbols.Add((memberId, dominoShape.symbol));
      //     if (initialized) {
      //       if (unitViewId == 0) {
      //         RefreshUnit();
      //       } else {
      //         RefreshDomino();
      //       }
      //     }
      //   } else if (thing is MemberToViewMapperOld.FaceDescriptionForIDescription face) {
      //     membersUnitFaces.Add((memberId, face.symbol));
      //     if (initialized) {
      //       if (unitViewId == 0) {
      //         RefreshUnit();
      //       } else {
      //         RefreshUnitFace();
      //       }
      //     }
      //   } else if (thing is MemberToViewMapperOld.DetailDescriptionForIDescription detail) {
      //     membersDetails.Add((memberId, detail.symbol));
      //     if (initialized) {
      //       if (unitViewId == 0) {
      //         RefreshUnit();
      //       } else {
      //         RefreshDetails();
      //       }
      //     }
      //   } else if (thing is MemberToViewMapperOld.ItemDescriptionForIDescription item) {
      //     membersItems.Add((memberId, item.symbol));
      //     if (initialized) {
      //       if (tileViewId != 0) {
      //         RefreshItems();
      //       }
      //     }
      //   } else {
      //     Asserts.Assert(false);
      //   }
      // }
    }

    public void OnRemoveMember(int index) {
      var (memberId, member) = members[index];
      members.RemoveAt(index);
      // foreach (var thing in vivimap.getEntries(member)) {
      //   if (thing is MemberToViewMapperOld.TopColorDescriptionForIDescription topColor) {
      //     membersFrontColors.RemoveAll(x => x.Item1 == memberId);
      //     if (tileViewId != 0) {
      //       RefreshSurfaceColor();
      //     }
      //   } else if (thing is MemberToViewMapperOld.SideColorDescriptionForIDescription wallColor) {
      //     membersSideColors.RemoveAll(x => x.Item1 == memberId);
      //     if (tileViewId != 0) {
      //       RefreshSideColor();
      //     }
      //   } else if (thing is MemberToViewMapperOld.OverlayDescriptionForIDescription overlay) {
      //     membersOverlays.RemoveAll(x => x.Item1 == memberId);
      //     if (tileViewId != 0) {
      //       RefreshOverlay();
      //     }
      //   } else if (thing is MemberToViewMapperOld.FeatureDescriptionForIDescription feature) {
      //     membersFeatures.RemoveAll(x => x.Item1 == memberId);
      //     if (tileViewId != 0) {
      //       RefreshFeature();
      //     }
      //   } else if (thing is MemberToViewMapperOld.DominoShapeDescriptionForIDescription dominoShape) {
      //     membersDominoSymbols.RemoveAll(x => x.Item1 == memberId);
      //     if (unitViewId != 0) {
      //       RefreshDomino();
      //     }
      //   } else if (thing is MemberToViewMapperOld.FaceDescriptionForIDescription face) {
      //     membersUnitFaces.RemoveAll(x => x.Item1 == memberId);
      //     if (unitViewId != 0) {
      //       RefreshUnitFace();
      //     }
      //   } else if (thing is MemberToViewMapperOld.DetailDescriptionForIDescription detail) {
      //     membersDetails.RemoveAll(x => x.Item1 == memberId);
      //     if (unitViewId != 0) {
      //       RefreshDetails();
      //     }
      //   } else if (thing is MemberToViewMapperOld.ItemDescriptionForIDescription item) {
      //     membersItems.RemoveAll(x => x.Item1 == memberId);
      //     if (tileViewId != 0) {
      //       RefreshItems();
      //     }
      //   } else {
      //     Asserts.Assert(false);
      //   }
      // }
    }

    public void AddMember(string member) {
      terrainTile.members.Add(member);
      OnAddMember(member);
    }

    public void RemoveMember(string member) {
      int index = terrainTile.members.IndexOf(member);
      Asserts.Assert(index >= 0);
      terrainTile.members.RemoveAt(index);
      OnRemoveMember(index);
    }

    public void RemoveMemberAt(int index) {
      terrainTile.members.RemoveAt(index);
      OnRemoveMember(index);
    }

    public void SetElevation(int elevation) {
      terrainTile.elevation = elevation;
      domino.SetElevation(tileViewId, elevation);
    }

    //   if (unitView) {
    //     unitView.Destruct();
    //     unitView = null;
    //   }
    //
    //   if (maybeUnitDescription != null) {
    //     
    //     // unitView.SetDescription(maybeUnitDescription);
    //   }
    // }

    // private (TileDescription, UnitDescription) GetDescriptions() {
    //   var defaultUnitDescription =
    //     new UnitDescription(
    //       null,
    //       new DominoDescription(false, new Color(.5f, 0, .5f)),
    //       new InitialSymbol(
    //         RenderPriority.DOMINO,
    //         new SymbolDescription(
    //           "a", new Color(0, 1, 0), 45, 1, OutlineMode.WithBackOutline),
    //         true,
    //         new Color(0, 0, 0)),
    //       new List<(int, InitialSymbol)>(),
    //       1,
    //       1);
    //
    //   var (tileDescription, unitDescription) =
    //     vivimap.Vivify(defaultTileDescription, defaultUnitDescription, members);
    //   return (tileDescription, unitDescription);
    // }

    public void DestroyTerrainTilePresenter() {
      domino.DestroyTile(tileViewId);
    }

    private static InitialSymbol CalculateMaybeOverlay(List<(string, InitialSymbol)> membersOverlays) {
      return membersOverlays.Count == 0 ? null : membersOverlays[membersOverlays.Count - 1].Item2;
    }

    private static InitialSymbol CalculateMaybeFeature(List<(string, InitialSymbol)> membersFeatures) {
      return membersFeatures.Count == 0 ? null : membersFeatures[membersFeatures.Count - 1].Item2;
    }

    private static IVec4iAnimation CalculateTintedFrontColor(
        IVec4iAnimation membersFrontColor, bool selected, bool highlighted) {
      if (selected && highlighted) {
        return new DivideVec4iAnimation(
            new AddVec4iAnimation(
                new MultiplyVec4iAnimation(
                    membersFrontColor, ConstantVec4iAnimation.All(63)),
                new MultiplyVec4iAnimation(
                    ConstantVec4iAnimation.white, ConstantVec4iAnimation.All(37))),
            ConstantVec4iAnimation.All(100));
      } else if (selected) {
        return new DivideVec4iAnimation(
            new AddVec4iAnimation(
                new MultiplyVec4iAnimation(
                    membersFrontColor, ConstantVec4iAnimation.All(75)),
                new MultiplyVec4iAnimation(
                    ConstantVec4iAnimation.white, ConstantVec4iAnimation.All(25))),
            ConstantVec4iAnimation.All(100));
      } else if (highlighted) {
        return new DivideVec4iAnimation(
            new AddVec4iAnimation(
                new MultiplyVec4iAnimation(
                    membersFrontColor, ConstantVec4iAnimation.All(87)),
                new MultiplyVec4iAnimation(
                    ConstantVec4iAnimation.white, ConstantVec4iAnimation.All(13))),
            ConstantVec4iAnimation.All(100));
      } else {
        return membersFrontColor;
      }
    }
  }
}
