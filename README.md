# FTP 2 Azure #

## Changes from Originals.

+ Change To .NET Core 2.0
+ setting file is appsettings.json


## What is this? ##
This is a tool that allows you to access as Azure Blob Storage Account as if it was an FTP Server. It's intended to be configured and run on your machine and then you use your favourite FTP Client to connect to localhost, but it can also run on a separate machine and you make your FTP Client connect to that other machine.
Basically, what this tool does is act as a proxy between the FTP protocol and the Azure Blob Storage API.

This code is based on [ftp2azure by richardparker](http://ftp2azure.codeplex.com/) which seems to have been abandoned (last commit was on 2 Nov 2012), that ftp2azure project contains ftp protocol code from an [article by Mohammed Habeeb](http://www.codeguru.com/csharp/csharp/cs_internet/desktopapplications/article.php/c13163/Simple-FTP-Demo-Application-Using-CNET-20.htm), which is in turn based on an [article by David McClarnon](http://www.codeguru.com/csharp/csharp/cs_network/sockets/article.php/c7409/A-C-FTP-Server.htm).

I just updated the original ftp2azure project to use the latest Azure SDK, changed it from an Azure Worker Role into a Console Application, and added a few features :)

## What's with the crazy $root hack? ##
So one thing I found missing from the original project was a way to list/create/delete Blob Containers, you just had the ability to set a certain container as your "ftp root" and couldn't go to the higher level.

I've  read that Azure has a special container called "$root", blobs under $root could be accessed directly without the container name in the uri (thus acting as if it's in a root folder), however you the names of these blobs couldn't contain any delimiters (no directories under $root).

I wanted to bridge the gap between Azure's blob structure and FTP's file structure, so here is what I did:

- Folders under the ftp root folder map to Blob Containers
- Files under the ftp root folder map to Blobs under the $root Container
- Sub-folders in ftp map to virtual directories (blob path prefixes)

This feature is turned on only when you login with the username "$root", otherwise it behaves as it was in the original project.

## Configuration ##
Please check the [documentation of the original project](http://ftp2azure.codeplex.com/documentation) for configuration instructions.

## License ##
My code as well as the original project are licensed under Apache License 2.0

However the license for the ftp protocol code from the codeguru articles is unknown, someone even asked in a comment and got no response from the author.