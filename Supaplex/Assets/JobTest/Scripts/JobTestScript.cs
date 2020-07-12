using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TestA
{
    public float data;
}

// Job adding two floating point values together
public struct MyJob : IJob
{
    //public TestA obj;
    public float a;
    public float b;
    public NativeArray<float> result;

    public void Execute()
    {
        result[0] = a + b;
        //obj.data = a + b;
    }
}

public class JobTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Create a native array of a single float to store the result. This example waits for the job to complete for illustration purposes
        NativeArray<float> result = new NativeArray<float>(1, Allocator.TempJob);

        // Set up the job data
        MyJob jobData = new MyJob();
        //jobData.obj = new TestA();
        jobData.a = 10;
        jobData.b = 10;
        jobData.result = result;

        // Schedule the job
        JobHandle handle = jobData.Schedule();

        // Wait for the job to complete
        handle.Complete();

        // All copies of the NativeArray point to the same memory, you can access the result in "your" copy of the NativeArray
        float aPlusB = result[0];

        // Free the memory allocated by the result array
        result.Dispose();

        var sw = new System.Diagnostics.Stopwatch();
        var a = new NativeArray<long>(4096, Allocator.Temp);
        var b = new long[4096];
        var count = 10000000;
        Random.InitState(0);
        long s = 0;
        var monitor = new object();
        sw.Restart();
        for (int i = 0; i < count; i++)
        {
            var index = Random.Range(0, 4096);
            b[index] = Random.Range(int.MinValue, int.MaxValue);
            s += b[index];
        }
        sw.Stop();
        Debug.Log($"{sw.ElapsedTicks}, {sw.ElapsedMilliseconds}, {s}"); // 0,0000509

        Random.InitState(0);
        s = 0;
        sw.Restart();
        for (int i = 0; i < count; i++)
        {
            lock (monitor)
            {
                var index = Random.Range(0, 4096);
                b[index] = Random.Range(int.MinValue, int.MaxValue);
                s += b[index];
            }
        }
        sw.Stop();
        Debug.Log($"{sw.ElapsedTicks}, {sw.ElapsedMilliseconds}, {s}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
