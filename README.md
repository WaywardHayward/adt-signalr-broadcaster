# ADT Signalr Broadcaster
[![Build](https://github.com/WaywardHayward/adt-signalr-broadcaster/actions/workflows/dotnet.yml/badge.svg)](https://github.com/WaywardHayward/adt-signalr-broadcaster/actions/workflows/dotnet.yml)

This is an azure function which will take Azure Digital Twin Patches from an Event Hub and Forward them onto a SignalR Hub.

## Settings

Place these settings in either a local.settings.json file or in the environment variables on the function app.

| Setting | Description |
| --- | --- |
| ADT_EVENTHUB_NAME | The name of the Event Hub containing the Azure Digital Twin Patches to listen to |
| ADT_EVENTHUB_CONNECTION_STRING | The connection string to the Event Hub |
| ADT_EVENTHUB_CONSUMERGROUP | The consumer group to use when listening to the Event Hub |
| ADT_SUBJECT_FILTER | The subject filter to use when listening to the Event Hub, this will filter messages using the cloudevents:subject property (defaults to *) |
| ADT_SIGNALR_HUB_NAME | The name of the SignalR Hub to forward the patches to |
| ADT_SIGNALR_HUB_CONNECTION_STRING | The connection string of the SignalR Hub to send messages to |
