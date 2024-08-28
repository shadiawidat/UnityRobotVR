
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
    public GameObject ball; // Assign the ball GameObject in the Inspector
    public LayerMask layerMask; // Define which layers should be considered for raycasting
    private Rigidbody ballRigidbody;
    
    private Vector3 targetPosition;
    private Vector3 position=new Vector3(0,0,0);

    private Vector3 direction= new Vector3(0, 0, 1);
    public string stars = "*******************";
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
        ball.transform.position = new Vector3(0, 10, 0);
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

            //await LeftAsync();
            //await Task.Delay(1000);
            //await RightAsync();
            //await Task.Delay(1000);
            stars = stars + "*";
            print(stars );
            //Vector3 hit_normalized = hit.point.normalized;
            //Vector3 position_normalized = position.normalized;
            
            targetPosition = (hit.point - position);
            Vector3 crossProduct = Vector3.Cross(direction, targetPosition);
            float angle = Vector3.Angle(direction, targetPosition);

            //if (crossProduct.y > 0)
            //{
            //    print("Right"+ stars);
            //}

            float distance = Vector3.Distance(position, hit.point);
            //print("distance is :");
            //print(distance);
            direction = (hit.point - position).normalized;
            position = hit.point;

            //print("position is " + position.ToString());
            //print("targetPosition is " + targetPosition.ToString());
            //print("distance is " + distance.ToString());

            //print("angle is " + angle.ToString());
            //print(angle);

            //position = targetPosition;
            //await ForwardAsync();

            //await Task.Delay(1000);

            //await LeftAsync();
            //await Task.Delay(1000);
            //await ForwardAsync();
            //await Task.Delay(1000);
            //await LeftAsync();

            //await Task.Delay(500);

            // 2*angle / 360


            //await LeftAsync();
            //await Task.Delay(8000/3);
            if (crossProduct.y < 0)
            {
                Debug.Log("LEFT" + stars);
                int wait_time = (int)( angle * 8200 / 1080);
                await LeftAsync();

                await Task.Delay(wait_time);
                await StopAsync();
                await Task.Delay(200);

                await ForwardAsync();

                await Task.Delay((int)((int)distance * 500));
            }
            else
            {
                Debug.Log("RIGHT" + stars);
                //int wait_time = (int)(3 * angle * 8000 / 360);
                int wait_time = (int)(angle * 8200 / 1080);

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
