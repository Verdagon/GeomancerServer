using System.Collections.Generic;
using Domino;
using Geomancer.Model;
using SimpleJSON;

namespace GeomancerServer {
  public static class Jsonify {
    
    public static JSONObject ToJson(this Vec3 vec) {
      var obj = new JSONObject();
      obj.Add("x", vec.x);
      obj.Add("y", vec.y);
      obj.Add("z", vec.z);
      return obj;
    }

    public static JSONObject ToJson(this Vec2 vec) {
      var obj = new JSONObject();
      obj.Add("x", vec.x);
      obj.Add("y", vec.y);
      return obj;
    }
    public static JSONObject ToJson(this PatternTile obj) { 
      var json = new JSONObject();
      json.Add("shape_index", obj.shapeIndex);
      json.Add("rotate_radianards", obj.rotateRadianards);
      json.Add("translate", obj.translate.ToJson());
      json.Add("side_index_to_side_adjacencies", obj.sideIndexToSideAdjacencies.ToJson());
      json.Add("corner_index_to_corner_adjacencies", obj.cornerIndexToCornerAdjacencies.ToJson());
      return json;
    }
    public static JSONArray ToJson(this PatternSideAdjacencyImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONObject ToJson(this PatternSideAdjacency obj) {
      var json = new JSONObject();
      json.Add("group_relative_x", obj.groupRelativeX);
      json.Add("group_relative_y", obj.groupRelativeY);
      json.Add("tile_index", obj.tileIndex);
      json.Add("side_index", obj.sideIndex);
      return json;
    }
    public static JSONArray ToJson(this PatternCornerAdjacencyImmListImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONArray ToJson(this PatternCornerAdjacencyImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONObject ToJson(this PatternCornerAdjacency obj) {
      var json = new JSONObject();
      json.Add("group_relative_x", obj.groupRelativeX);
      json.Add("group_relative_y", obj.groupRelativeY);
      json.Add("tile_index", obj.tileIndex);
      json.Add("corner_index", obj.cornerIndex);
      return json;
    }



    public static JSONObject ToJson(this Pattern obj) {
      var json = new JSONObject();
      json.Add("name", obj.name);
      json.Add("x_offset", obj.xOffset.ToJson());
      json.Add("y_offset", obj.yOffset.ToJson());
      json.Add("shape_index_to_corners", obj.shapeIndexToCorners.ToJson());
      json.Add("pattern_tiles", obj.patternTiles.ToJson());
      return json;
    }
    
    public static JSONArray ToJson(this PatternTileImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONArray ToJson(this Vec2ImmListImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONArray ToJson(this Vec2ImmList obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    public static JSONObject ToJson(this (ulong, InitialSymbol) obj) {
      var json = new JSONObject();
      json.Add("id", obj.Item1);
      json.Add("symbol", obj.Item2.ToJson());
      return json;
    }
    public static JSONArray ToJson(this List<(ulong, InitialSymbol)> obj) {
      var json = new JSONArray();
      foreach (var el in obj) {
        json.Add(el.ToJson());
      }
      return json;
    }
    
    public static JSONObject ToJson(this Location obj) {
      var json = new JSONObject();
      json.Add("group_x", obj.groupX);
      json.Add("group_y", obj.groupY);
      json.Add("index_in_group", obj.indexInGroup);
      return json;
    }
    public static JSONObject ToJson(this Vec4i obj) {
      var json = new JSONObject();
      json.Add("x", obj.x);
      json.Add("y", obj.y);
      json.Add("z", obj.z);
      json.Add("w", obj.w);
      return json;
    }
    public static JSONNode ToJson(this IVec4iAnimation anim) {
      if (anim == null) {
        return JSONNull.CreateOrGet();
      }
      if (anim is ConstantVec4iAnimation constant) {
        var json = new JSONObject();
        json.Add("Vec4iAnimation", "ConstantVec4iAnimation");
        json.Add("val", constant.vec.ToJson());
        return json;
      } else if (anim is AddVec4iAnimation add) {
        var json = new JSONObject();
        json.Add("Vec4iAnimation", "AddVec4iAnimation");
        json.Add("left", add.left.ToJson());
        json.Add("right", add.right.ToJson());
        return json;
      } else if (anim is MultiplyVec4iAnimation multiply) {
        var json = new JSONObject();
        json.Add("Vec4iAnimation", "MultiplyVec4iAnimation");
        json.Add("left", multiply.left.ToJson());
        json.Add("right", multiply.right.ToJson());
        return json;
      } else if (anim is DivideVec4iAnimation divide) {
        var json = new JSONObject();
        json.Add("Vec4iAnimation", "DivideVec4iAnimation");
        json.Add("left", divide.left.ToJson());
        json.Add("right", divide.right.ToJson());
        return json;
      } else {
        Asserts.Assert(false);
        return null;
      }
    }
    public static JSONNode ToJson(this SymbolId obj) {
      var json = new JSONObject();
      json.Add("font_name", obj.fontName);
      json.Add("unicode", obj.unicode);
      return json;
    }
    public static JSONNode ToJson(this InitialSymbolGlyph obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      json.Add("symbol_id", obj.symbolId.ToJson());
      json.Add("color", obj.color.ToJson());
      return json;
    }
    public static JSONNode ToJson(this InitialSymbolOutline obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      switch (obj.mode) {
        case OutlineMode.NoOutline:
          Asserts.Assert(false);
          break;
        case OutlineMode.CenteredOutline:
          json.Add("type", "centered");
          break;
        case OutlineMode.OuterOutline:
          json.Add("type", "outer");
          break;
        default:
          Asserts.Assert(false);
          break;
      }
      json.Add("color", obj.color.ToJson());
      return json;
    }
    public static JSONNode ToJson(this InitialSymbolSides obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      json.Add("depth_percent", obj.depthPercent);
      json.Add("color", obj.color.ToJson());
      return json;
    }
    public static JSONNode ToJson(this InitialSymbol obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      json.Add("glyph", obj.glyph.ToJson());
      json.Add("outline", obj.outline.ToJson());
      json.Add("sides", obj.sides.ToJson());
      json.Add("rotation_degrees", obj.rotationDegrees);
      json.Add("size_percent", obj.sizePercent);
      return json;
    }
    public static JSONNode ToJson(this InitialUnit obj) {
      if (obj == null) {
        return JSONNull.CreateOrGet();
      }
      var json = new JSONObject();
      json.Add("location", obj.location.ToJson());
      json.Add("domino", obj.dominoSymbol.ToJson());
      json.Add("face", obj.faceSymbol.ToJson());
      json.Add("id_to_detail_symbol", obj.idToDetailSymbol.ToJson());
      json.Add("hp_ratio", obj.hpRatio);
      json.Add("mp_ratio", obj.mpRatio);
      return json;
    }
    public static JSONObject ToJson(this InitialTile obj) {
      var json = new JSONObject();
      json.Add("location", obj.location.ToJson());
      json.Add("elevation", obj.elevation);
      json.Add("top_color", obj.topColor.ToJson());
      json.Add("side_color", obj.sideColor.ToJson());
      json.Add("maybe_overlay_symbol", obj.maybeOverlaySymbol.ToJson());
      json.Add("maybe_feature_symbol", obj.maybeFeatureSymbol.ToJson());
      json.Add("item_id_to_symbol", obj.itemIdToSymbol.ToJson());
      return json;
    }

    public static JSONObject ToJson(this SetupGameMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "SetupGameCommand");
      json.Add("look_at", obj.cameraPosition.ToJson());
      json.Add("look_at_offset_to_camera", obj.lookatOffsetToCamera.ToJson());
      json.Add("elevation_step_height", new JSONNumber(obj.elevationStepHeight));
      json.Add("pattern", obj.pattern.ToJson());
      return json;
    }

    public static JSONObject ToJson(this MakePanelMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "MakePanelCommand");
      json.Add("id", obj.id);
      json.Add("panel_grid_x_in_screen", obj.panelGXInScreen);
      json.Add("panel_grid_y_in_screen", obj.panelGYInScreen);
      json.Add("panel_grid_width", obj.panelGW);
      json.Add("panel_grid_height", obj.panelGH);
      return json;
    }

    public static JSONObject ToJson(this CreateTileMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "CreateTileCommand");
      json.Add("tile_id", obj.newTileId);
      json.Add("initial_tile", obj.initialTile.ToJson());
      return json;
    }

    public static JSONObject ToJson(this DestroyTileMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "DestroyTileCommand");
      json.Add("tile_id", obj.tileViewId);
      return json;
    }

    public static JSONObject ToJson(this SetSurfaceColorMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "SetSurfaceColorCommand");
      json.Add("tile_id", obj.tileViewId);
      json.Add("color", obj.frontColor.ToJson());
      return json;
    }

    public static JSONObject ToJson(this SetCliffColorMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "SetCliffColorCommand");
      json.Add("tile_id", obj.tileViewId);
      json.Add("color", obj.sideColor.ToJson());
      return json;
    }

    public static JSONObject ToJson(this AddRectangleMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "AddRectangleCommand");
      json.Add("new_view_id", obj.newViewId);
      json.Add("parent_view_id", obj.parentViewId);
      json.Add("x", obj.x);
      json.Add("y", obj.y);
      json.Add("width", obj.width);
      json.Add("height", obj.height);
      json.Add("z", obj.z);
      json.Add("color", obj.color.ToJson());
      json.Add("border_color", obj.borderColor.ToJson());
      return json;
    }

    public static JSONObject ToJson(this AddSymbolMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "AddSymbolCommand");
      json.Add("new_view_id", obj.newViewId);
      json.Add("parent_view_id", obj.parentViewId);
      json.Add("x", obj.x);
      json.Add("y", obj.y);
      json.Add("size", obj.size);
      json.Add("z", obj.z);
      json.Add("color", obj.color.ToJson());
      json.Add("symbol_id", obj.symbolId.ToJson());
      json.Add("centered", obj.centered);
      return json;
    }

    public static JSONObject ToJson(this ScheduleCloseMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "ScheduleCloseCommand");
      json.Add("view_id", obj.viewId);
      json.Add("start_ms_from_now", obj.startMsFromNow);
      return json;
    }

    public static JSONObject ToJson(this SetElevationMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "SetElevationCommand");
      json.Add("tile_id", obj.tileViewId);
      json.Add("elevation", obj.elevation);
      return json;
    }

    public static JSONObject ToJson(this SetOverlayMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "SetOverlayCommand");
      json.Add("tile_id", obj.tileId);
      json.Add("symbol", obj.symbol.ToJson());
      return json;
    }

    public static JSONObject ToJson(this FadeIn obj) {
      var json = new JSONObject();
      json.Add("fade_in_start_time_ms", obj.fadeInStartTimeMs);
      json.Add("fade_in_end_time_ms", obj.fadeInEndTimeMs);
      return json;
    }

    public static JSONObject ToJson(this FadeOut obj) {
      var json = new JSONObject();
      json.Add("fade_out_start_time_ms", obj.fadeOutStartTimeMs);
      json.Add("fade_out_end_time_ms", obj.fadeOutEndTimeMs);
      return json;
    }

    public static JSONObject ToJson(this SetFadeInMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "SetFadeInCommand");
      json.Add("id", obj.id);
      json.Add("fade_in", obj.fadeIn.ToJson());
      return json;
    }

    public static JSONObject ToJson(this SetFadeOutMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "SetFadeOutCommand");
      json.Add("id", obj.id);
      json.Add("fade_out", obj.fadeOut.ToJson());
      return json;
    }

    public static JSONObject ToJson(this RemoveViewMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "RemoveViewCommand");
      json.Add("view_id", obj.viewId);
      return json;
    }

    public static JSONObject ToJson(this CreateUnitMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "CreateUnitCommand");
      json.Add("unit_id", obj.id);
      json.Add("initial_unit", obj.initialUnit.ToJson());
      return json;
    }

    public static JSONObject ToJson(this DestroyUnitMessage obj) {
      var json = new JSONObject();
      json.Add("ICommand", "DestroyUnitCommand");
      json.Add("unit_id", obj.unitId);
      return json;
    }

    public static JSONObject ToJson(this IDominoMessage command) {
      if (command is SetupGameMessage setupGame) {
        return setupGame.ToJson();
      } else if (command is MakePanelMessage makePanel) {
        return makePanel.ToJson();
      } else if (command is CreateTileMessage createTile) {
        return createTile.ToJson();
      } else if (command is DestroyTileMessage destroyTile) {
        return destroyTile.ToJson();
      } else if (command is SetSurfaceColorMessage setSurfaceColor) {
        return setSurfaceColor.ToJson();
      } else if (command is SetCliffColorMessage setCliffColor) {
        return setCliffColor.ToJson();
      } else if (command is AddRectangleMessage addRectangle) {
        return addRectangle.ToJson();
      } else if (command is AddSymbolMessage addSymbol) {
        return addSymbol.ToJson();
      } else if (command is ScheduleCloseMessage scheduleClose) {
        return scheduleClose.ToJson();
      } else if (command is SetElevationMessage setElevation) {
        return setElevation.ToJson();
      } else if (command is SetOverlayMessage setOverlay) {
        return setOverlay.ToJson();
      } else if (command is SetFadeInMessage setFadeIn) {
        return setFadeIn.ToJson();
      } else if (command is SetFadeOutMessage setFadeOut) {
        return setFadeOut.ToJson();
      } else if (command is RemoveViewMessage removeView) {
        return removeView.ToJson();
      } else if (command is CreateUnitMessage createUnit) {
        return createUnit.ToJson();
      } else if (command is DestroyUnitMessage destroyUnit) {
        return destroyUnit.ToJson();
      } else {
        Asserts.Assert(false);
        return null;
      }
    }
  }
}