using System;
using System.IO;
using System.Net;
using Domino;
using Geomancer.Model;
using GeomancerServer;
using SimpleJSON;

namespace Geomancer {
  public class HttpServer {
    public static void Main(string[] args) {
      SimpleListenerExample(
          new [] {"http://localhost:5000/", "http://127.0.0.1:5000/"});
    }
    
    // This example requires the System and System.Net namespaces.
    public static void SimpleListenerExample(string[] prefixes) {
      if (!HttpListener.IsSupported) {
        Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
        return;
      }
      // URI prefixes are required,
      // for example "http://contoso.com:8080/index/".
      if (prefixes == null || prefixes.Length == 0)
        throw new ArgumentException("prefixes");

      // Create a listener.
      HttpListener listener = new HttpListener();
      // Add the prefixes.
      foreach (string s in prefixes) {
        listener.Prefixes.Add(s);
      }
      listener.Start();

      Console.WriteLine("Listening...");

      while (true) {
        try {
          // Note: The GetContext method blocks while waiting for a request.
          HttpListenerContext startRequestContext = listener.GetContext();
          HttpListenerRequest startRequest = startRequestContext.Request;
          var startRequestStr = new StreamReader(startRequest.InputStream).ReadToEnd();
          Console.WriteLine("Got request:\n" + startRequestStr);
          var startRequestsNode = JSONObject.Parse(startRequestStr);
          var startRequestsObj = JsonHarvester.ExpectObject(startRequestsNode, "Expected JSON object!");
          var startRequestsArray = JsonHarvester.ExpectMemberArray(startRequestsObj, "requests");
          if (startRequestsArray.Count != 1) {
            throw new Exception("Expected array field 'requests' of length 1, was length " + startRequestsArray.Count);
          }
          var startRequestNode = startRequestsArray[0];
          var startRequestObj = JsonHarvester.ExpectObject(startRequestNode, "Request must be an object!");
          var startRequestType = JsonHarvester.ExpectMemberString(startRequestObj, "request_type");
          if (startRequestType == "Reset") {
            HttpListenerResponse response = startRequestContext.Response;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("{\"commands\":[]}");
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            continue;
          }
          if (startRequestType != "Start") {
            throw new Exception("First request must be start!");
          }
          var gameToDominoConnection = new GameToDominoConnection();
          var editor =
              new EditorServer(
                  System.IO.Directory.GetCurrentDirectory(),
                  gameToDominoConnection,
                  JsonHarvester.ExpectMemberInteger(startRequestObj, "screen_grid_width"),
                  JsonHarvester.ExpectMemberInteger(startRequestObj, "screen_grid_height"));
          Respond(startRequestContext, gameToDominoConnection);

          while (HandleRequest(listener, editor, gameToDominoConnection)) { }
        }
        catch (Exception e) {
          Console.WriteLine("Encountered exception:");
          Console.WriteLine(e.Message);
        }
        Console.WriteLine("Restarting!");
      }

      listener.Stop();
    }

    private static void Respond(
        HttpListenerContext context,
        GameToDominoConnection gameToDominoConnection) {
      
      var responseMessages = gameToDominoConnection.TakeMessages();
      JSONObject responseObj = new JSONObject();
      JSONArray commandsArray = new JSONArray();
      responseObj.Add("commands", commandsArray);
      foreach (var message in responseMessages) {
        var json = message.ToJson();
        Console.WriteLine("Sending: " + json.ToString());
        commandsArray.Add(json);
      }
      
      // Obtain a response object.
      HttpListenerResponse response = context.Response;
      // Construct a response.
      string responseString = responseObj.ToString();
      byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
      // Get a response stream and write the response to it.
      response.ContentLength64 = buffer.Length;
      System.IO.Stream output = response.OutputStream;
      output.Write(buffer, 0, buffer.Length);
      // You must close the output stream.
      output.Close();
    }

    private static bool HandleRequest(
        HttpListener listener,
        EditorServer server,
        GameToDominoConnection gameToDominoConnection) {
      // Note: The GetContext method blocks while waiting for a request.
      HttpListenerContext requestContext = listener.GetContext();
      HttpListenerRequest request = requestContext.Request;
      var requestStr = new StreamReader(request.InputStream).ReadToEnd();
      Console.WriteLine("Got request:\n" + requestStr);
      var requestsNode = JSONObject.Parse(requestStr);
      var requestsObj = JsonHarvester.ExpectObject(requestsNode, "Expected JSON object!");
      var requestsArray = JsonHarvester.ExpectMemberArray(requestsObj, "requests");
      bool keepRunning = true;
      foreach (var requestNode in requestsArray) {
        var requestObj = JsonHarvester.ExpectObject(requestNode, "Request must be an object!");
        var requestType = JsonHarvester.ExpectMemberString(requestObj, "request_type");
        switch (requestType) {
          case "Reset":
            Console.WriteLine("Got Reset request, stopping...");
            keepRunning = false;
            break;
          case "SetHoveredLocation":
            HandleSetHoveredLocation(server, requestObj);
            break;
          case "LocationMouseDown":
            HandleLocationMouseDown(server, requestObj);
            break;
          case "KeyDown":
            HandleKeyDown(server, requestObj);
            break;
          case "Custom":
            HandleCustomRequest(server, requestObj);
            break;
          default:
            Asserts.Assert(false, "Unknown request: " + requestType);
            keepRunning = false;
            break;
        }
        if (!keepRunning) {
          break;
        }
      }
      Respond(requestContext, gameToDominoConnection);
      return keepRunning;
    }

    private static void HandleSetHoveredLocation(
        EditorServer server,
        JSONObject requestObj) {
      server.SetHoveredLocation(
          JsonHarvester.GetMaybeMemberString(requestObj, "tile_id", out var s) ? s : "",
          JsonHarvester.GetMaybeMemberLocation(requestObj, "location", out var loc) ? loc : null);
    }

    private static void HandleLocationMouseDown(
        EditorServer server,
        JSONObject requestObj) {
      server.LocationMouseDown(
          JsonHarvester.ExpectMemberString(requestObj, "tile_id"),
          JsonHarvester.ExpectMemberLocation(requestObj, "location"));
    }

    private static void HandleCustomRequest(
        EditorServer server,
        JSONObject requestObj) {
      server.HandleCustomRequest(
          JsonHarvester.ExpectMemberObject(requestObj, "data"));
    }

    private static void HandleKeyDown(
        EditorServer server,
        JSONObject requestObj) {
      server.KeyDown(
          JsonHarvester.ExpectMemberInteger(requestObj, "unicode"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "left_shift_down"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "right_shift_down"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "ctrl_down"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "left_alt_down"),
          JsonHarvester.ExpectMemberBoolean(requestObj, "right_alt_down"));
    }
  }
}
