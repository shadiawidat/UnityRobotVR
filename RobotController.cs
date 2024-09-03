
using System;
using UnityEngine;
using UnityEngine.XR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class RobotController : MonoBehaviour, IMixedRealityPointerHandler
{
    private static readonly string RpcUrl = "http://192.168.149.1:9030";
    private static readonly HttpClient HttpClient = new HttpClient();
    public GameObject robot;
    public LayerMask layerMask; 
    private Rigidbody robotRigidbody;
    
    private Vector3 targetPosition;
    private Vector3 position=new Vector3(0,0,0);
    private Vector3 direction= new Vector3(0, 0, 1);
    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
    }

    private async void Start()
    {
        Debug.Log(await SetSonarRgbModeAsync(0));
        Debug.Log($"rgb: {await SetSonarRgbAsync(0, 255, 0, 0)}");
        await Task.Delay(1000);


        //await ForwardAsync();
        //await Task.Delay(1000);

        //await BackwardsAsync();
        //await Task.Delay(1000);

        //await LeftAsync();
        //await Task.Delay(1000);

        //await RightAsync();
        //await Task.Delay(1000);

        //await StopAsync();


        //await RightAsync();
        //await Task.Delay(1000);

        await StopAsync();
    }


    private static async Task<string> SendRpcRequestAsync(string method, object[] parameters = null)
    {
        var payload = new
        {
            jsonrpc = "2.0",
            method,
            @params = parameters ?? new object[] { },
            id = 1
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

        try
        {
            var response = await HttpClient.PostAsync(RpcUrl, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JObject.Parse(responseString);
            return result["result"]?.ToString() ?? "No result";
        }
        catch (Exception e)
        {
            return $"Error: {e.Message}";
        }
    }

    private static Task<string> SetSonarRgbModeAsync(int mode = 0) => SendRpcRequestAsync("SetSonarRGBMode", new object[] { mode });

    private static Task<string> SetSonarRgbAsync(int r, int g, int b, int index = 0) => SendRpcRequestAsync("SetSonarRGB", new object[] { index, r, g, b });

    private static Task ForwardAsync() => SetMovementVelocityAsync(50, 90, 0);
    private static Task BackwardsAsync() => SetMovementVelocityAsync(50, 270, 0);
    private static Task LeftAsync() => SetMovementVelocityAsync(50, 135, 0);
    private static Task RightAsync() => SetMovementVelocityAsync(50, 45, 0);
    private static Task StopAsync() => SetMovementVelocityAsync(0, 0, 0);


    private static Task MoveAsync(int var1,int var2,int var3) => SetMovementVelocityAsync(var1, var2, var3);

    private static Task<string> SetMovementVelocityAsync(int velocity, int direction, int angularRate) => SendRpcRequestAsync("SetMovement", new object[] { velocity, direction, angularRate });

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    public async void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        print("OnPointerClicked: robot");
        Ray ray = new Ray(eventData.Pointer.Position, eventData.Pointer.Rotation * Vector3.forward);  
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {

            
            targetPosition = (hit.point - position);
            Vector3 crossProduct = Vector3.Cross(direction, targetPosition); 
            float angle = Vector3.Angle(direction, targetPosition); // the angle between the direction of the robot and the target point that we clicked on the plane

            float distance = Vector3.Distance(position, hit.point); // the distance between the current position of the robot and the target point that we clicked on the plane

            direction = (hit.point - position).normalized;
            position = hit.point;


            if (crossProduct.y < 0) // the angle that we calculated is to the left of the direction vector
            {
                Debug.Log("LEFT" );
                int wait_time = (int)( angle * 8200 / 1080); // calculation that gives time to the robot to finish the angle rotation
                await LeftAsync();

                await Task.Delay(wait_time);
                await StopAsync();
                await Task.Delay(200);

                await ForwardAsync();

                await Task.Delay((int)((int)distance * 500));
            }
            else  // the angle that we calculated is to the right of the direction vector
            {
                Debug.Log("RIGHT" );
                int wait_time = (int)(angle * 8200 / 1080);   // calculation that gives time to the robot to finish the angle rotation

                await RightAsync();

                await Task.Delay(wait_time);
                await StopAsync();
                await Task.Delay(200);
                await ForwardAsync();

                await Task.Delay((int)((int)distance * 500));
            }
            print("angle is :");
            print(angle);

            await StopAsync();

        }
    }
}
