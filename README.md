# nots-chatapp

A multi-client chat application built with .NET 6 &amp; Maui

This application is for demonstration purposes only and should not be considered a 'good practice' overall. The main goals of this application are:

1. Demonstrate the usage of Task API in a modern day desktop application (GUI)
2. Demonstrate simple networking concepts using TCP/sockets and other high level constructs in combination with async/await.

Please note that .NET MAUI is currently in preview stage, and is not yet a stable/ finished product. As a result of this project the following issue(s) were opened:

* [Compiling on m1 arm64 yields x64 app #4475](https://github.com/dotnet/maui/issues/4475)

## Applications

This repository contains the following applications:

* ChatClient & ChatServer: two console applications designed to be quick and non-blocking. Using the socket API. These demonstrate network client/server communications using TCP.
* ChatClientTCP & ChatServerTCP: two console applications using higher level concepts such as TCPclient, TCPlistener and streams.
* ChatClient.gui and MyMauiApp: two GUI client applications using the maui preview versions 12 and 13 respectively. Warning: these are in no way stable, and can not be used reliably. They do demonstrate a non-blocking UI and the ability to compile on both MacOS and Windows.

## MacOS

First make sure to [have the latest preview version of maui installed](https://github.com/dotnet/maui/wiki/macOS-Install)

To build & run: `dotnet build -t:Run -f net6.0-maccatalyst`

## Windows

First make sure to [have the latest preview version of maui installed](https://docs.microsoft.com/en-gb/dotnet/maui/get-started/installation)

To build & run: `dotnet build -t:Run`
