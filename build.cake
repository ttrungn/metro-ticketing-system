Task("Docker-Build-SampleService")
    .Does(() =>
{
    Information("Starting Docker build...");
    // Context is SampleService/src/Web because the Dockerfile is there
    StartProcess("docker", 
        "build -t sample-service:latest -f SampleService/src/Web/Dockerfile .");
});
