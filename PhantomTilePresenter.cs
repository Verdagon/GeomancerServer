using System;
using System.Collections.Generic;

using Geomancer.Model;

using Domino;

namespace Geomancer {
  public class PhantomTilePresenter {
    //public delegate void OnMouseInEvent();
    //public delegate void OnMouseOutEvent();
    public delegate void OnPhantomTileClickedEvent();

    // IClock clock;
    // ITimer timer;
  Pattern pattern;
    public readonly Location location;
    // ILoader loader;
    // private TileShapeMeshCache tileShapeMeshCache;

    private int elevationStepHeight;
    
    Vec3 tileCenter;
    ulong tileViewId;
    private bool highlighted;
    private GameToDominoConnection domino;

    public PhantomTilePresenter(
        GameToDominoConnection domino,
        Pattern pattern,
        Location location,
        int elevationStepHeight) {
      this.domino = domino;
      // this.clock = clock;
      // this.timer = timer;
      this.pattern = pattern;
      this.location = location;
      // this.loader = loader;
      // this.tileShapeMeshCache = tileShapeMeshCache;
      this.elevationStepHeight = elevationStepHeight;

      var positionVec2 = pattern.GetTileCenter(location);

      tileCenter = new Vec3(positionVec2.x, positionVec2.y, 0);

      ResetViews();
    }

    private static (IVec4iAnimation, IVec4iAnimation) GetColors(bool highlighted) {
      var frontColor = highlighted ? new Vec4i(51, 51, 51, 255) : new Vec4i(0, 0, 0, 255);
      var sideColor = highlighted ? new Vec4i(51, 51, 51, 255) : new Vec4i(0, 0, 0, 255);
      return (new ConstantVec4iAnimation(frontColor), new ConstantVec4iAnimation(sideColor));
    }
    
    public void SetHighlighted(bool highlighted) {
      var (frontColor, sideColor) = GetColors(highlighted);
      domino.SetSurfaceColor(tileViewId, frontColor);
      domino.SetCliffColor(tileViewId, sideColor);
      // tileView.SetDescription(GetTileDescription(pattern, location, highlighted));
    }

    private void ResetViews() {
      if (tileViewId != 0) {
        domino.DestroyTile(tileViewId);
        // tileView.DestroyTile();
        tileViewId = 0;
      }

      var position = CalculatePosition(elevationStepHeight, pattern, location);

      var patternTileIndex = location.indexInGroup;
      var shapeIndex = pattern.patternTiles[patternTileIndex].shapeIndex;
      //   var radianards = pattern.patternTiles[patternTileIndex].rotateRadianards;
      //   var radians = radianards * 0.001f;
      //   var degrees = (float)(radians * 180 / Math.PI);
      //   var rotation = Quaternion.AngleAxis(-degrees, Vector3.up);
      var unityElevationStepHeight = elevationStepHeight * ModelExtensions.ModelToUnityMultiplier;
      var tileDescription = GetTileDescription(pattern, location, elevationStepHeight, highlighted);

      tileViewId = domino.CreateTile(tileDescription);
      
      // var (groundMesh, outlinesMesh) = tileShapeMeshCache.Get(shapeIndex, unityElevationStepHeight, .025f);
      // tileView = TileView.Create(loader, groundMesh, outlinesMesh, clock, timer, tileDescription);
      // tileView.gameObject.AddComponent<PhantomTilePresenterTile>().Init(this);
      // tileView.gameObject.transform.localPosition = position;
    }
    
    private static Vec3 CalculatePosition(int elevationStepHeight, Pattern pattern, Location location) {
      var positionVec2 = pattern.GetTileCenter(location);
      return new Vec3(positionVec2.x, positionVec2.y, elevationStepHeight);
    }
    
    //
    // private static SymbolId GetTerrainTileShapeSymbol(Pattern pattern, PatternTile patternTile) {
    //   switch (pattern.name) {
    //     case "square":
    //       if (patternTile.shapeIndex == 0) {
    //         return new SymbolId("AthSymbols", 0x006A);
    //       }
    //       break;
    //     case "pentagon9":
    //       if (patternTile.shapeIndex == 0) {
    //         return new SymbolId("AthSymbols", 0x0069);
    //       } else if (patternTile.shapeIndex == 1) {
    //         return new SymbolId("AthSymbols", 0x0068);
    //       }
    //       break;
    //     case "hex":
    //       if (patternTile.shapeIndex == 0) {
    //         return new SymbolId("AthSymbols", 0x0035);
    //       }
    //       break;
    //   }
    //   return new SymbolId("AthSymbols", 0x0065);
    // }
    //
    private static InitialTile GetTileDescription(
        Pattern pattern, Location location, float elevationStepHeight, bool highlighted) {
      var patternTile = pattern.patternTiles[location.indexInGroup];

      var (frontColor, sideColor) = GetColors(highlighted);
    
      return new InitialTile(
          location,
          1,
          frontColor,
          sideColor,
          null,
          null,
          new List<(ulong, InitialSymbol)>());
    }

    public void DestroyPhantomTilePresenter() {
      domino.DestroyTile(tileViewId);// tileView.DestroyTile();
      tileViewId = 0;
    }

    // public void OnStrMutListEffect(IStrMutListEffect effect) {
    //   effect.visitIStrMutListEffect(this);
    // }
    //
    // public void visitStrMutListCreateEffect(StrMutListCreateEffect effect) { }
    //
    // public void visitStrMutListDeleteEffect(StrMutListDeleteEffect effect) { }
    //
    // public void visitStrMutListAddEffect(StrMutListAddEffect effect) {
    //   ResetViews();
    // }
    //
    // public void visitStrMutListRemoveEffect(StrMutListRemoveEffect effect) {
    //   ResetViews();
    // }
  }
}
