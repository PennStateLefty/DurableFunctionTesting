# Event-driven Durable Functions
## Overview
This sample demonstrates how to use the [Durable Functions](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview) extension to build an event-driven workflow. The sample uses the [Azure Function HTTP Trigger](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?pivots=programming-language-csharp&tabs=python-v2%2Cin-process%2Cfunctionsv2) to kick off a [Durable orchestration](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-orchestrations?tabs=csharp-inproc). The orchestrator creates several simple [Activity functions](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-types-features-overview) before pausing to wait for an [external event](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-external-events?tabs=csharp). A subscription is created to Blob Created events raised by [Azure Storage](https://docs.microsoft.com/en-us/azure/storage/) and is picked up by the Function App using an [Azure Event Grid](https://docs.microsoft.com/en-us/azure/event-grid/overview) trigger to raise the external event the orchestration is waiting for. The orchestrator then resumes and completes the workflow. 

## Architecture
![Block diagram of the architecture described above](/images/high-level-design.jpg)

### Flow
1. The user calls the HTTP Trigger function with a POST request. The function creates a new instance of the orchestration and returns the instance ID to the user.
2. The orchestration creates several Activity functions and then waits for an external event.
3. The user uploads a blob to the storage account. This raises a Blob Created event which is picked up by the Function App using an Event Grid trigger.
4. The Function App raises an external event which is picked up by the Function App.
5. The orchestration is resumed after receiving a Blob Created event from the subscription and completes the workflow.

## Prerequisites
### Development Tools
- [Visual Studio Code](https://code.visualstudio.com/)
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Azure Account Extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode.azure-account)
- [Azure Resoures Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azureresources)
- [Azure Functions Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash#v2)
- [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
- [REST Client Extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)

### Azure Resources
- [Azure Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-create?tabs=azure-portal)
- [Azure Function App](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal)
  - Consumption tier, Windows hosting, .NET 6 runtime
- [Event Grid Subscription](https://learn.microsoft.com/en-us/azure/event-grid/subscribe-through-portal)
  - [Example Setup]](https://learn.microsoft.com/en-us/azure/azure-functions/functions-event-grid-blob-trigger?pivots=programming-language-csharp)

## Setup
1. Clone the repository locally
2. Provision Azure resources outlined above (Storage Account and Function App)
3. [Create a new Blob container in the Storage Account](https://learn.microsoft.com/en-us/azure/storage/blobs/blob-containers-portal#create-a-container)
4. [Deploy the Function App to Azure](https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=csharp#republish-project-files)
5. Subscribe to Blob Created events using the Event Grid Subscription

### Subscribe to the Blob Created Event
![Screenshot of creating an event subscription](/images/create-event-subscription.jpg)

### Bind the subscription to the Function App
![Screenshot of the Event Grid Subscription](/images/subscription-setup.jpg)

## Testing
1. Open scratchpad.http from the project files in Visual Studio Code
2. Update the URL in the first line to point to the HTTP Trigger function in your Function App
3. Send the request to the HTTP Trigger function
4. Capture the instance ID and the storage key code from the response and update the variables in the second and third line of the file respectively. ***Be sure to never check-in the storage key code to source control***
5. Upload any file to the Blob container created in the setup steps
6. Check the logs in the Function App to see the workflow complete