# OsuCollectionDownloader
A command-line application for downloading collections from [osu!collector](https://osucollector.com/) and optionally, using those collections to generate .osdb files which can be used by [Piotrekol's Collection Manager](https://github.com/Piotrekol/CollectionManager) for importing collections in osu! or just sharing them.

I strongly advise not use this application irresponsibly and instead support [osu!collector](https://osucollector.com/client).

## Usage
1. Get the latest build from the [Releases](https://github.com/waylaa/OsuCollectionDownloader/releases) page.
2. Copy the collection ID from the url of a collection at [osu!collector](https://osucollector.com/). For example:
   ```https://osucollector.com/collections/xxxx/collection-name```
   **where xxxx is the ID of the collection.**
3. Open any terminal and run whichever command you want below.
   
### Windows

#### Download a Collection
```
.\OsuCollectionDownloader.exe --id=<osucollector collection id> --extraction-directory=<your 'Songs' folder in osu!> 
```

#### Download a Collection and Generate a .osdb File
```
.\OsuCollectionDownloader.exe --id=<osucollector collection id> --extraction-directory=<your 'Songs' folder in osu!> --osdb-generation-directory=<A random directory>
```

### MacOS/Linux

#### Download a Collection
```
.\OsuCollectionDownloader --id=<osucollector collection id> --extraction-directory=<your 'Songs' folder in osu!> 
```

#### Download a Collection and Generate a .osdb File
```
.\OsuCollectionDownloader --id=<osucollector collection id> --extraction-directory=<your 'Songs' folder in osu!> --osdb-generation-directory=<A random directory>
```

An additional option for downloading sequentially exists and can be enabled by having the ```--download-sequentially``` option alongside the other said options above. **Only enable this if you're getting rate-limited by the beatmap mirrors, having a really slow internet connection speed to the point of causing timeouts or experiencing issues with the concurrent downloader.**

4. Open osu! and press F5.

## Building/Contributing
### 1. Prerequisites (Visual Studio Installer)
  - .NET desktop development
  - Desktop development with C++ (Required in order to AOT build and publish)
    
### 2. Install .NET 8
- Make sure you have .NET 8 installed on your machine.
- If not installed, download and install it from [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

### 3. Clone
- Open your preferred terminal.
- Navigate to the directory where you want to clone the repository:
```
cd <dir>
```

- Clone the repository
```
git clone https://github.com/waylaa/OsuCollectionDownloader.git
```

- Open the .sln file, restore the nuget packages and you're done.

## Disclaimer
This project is not endorsed by or affiliated with [osu!](https://osu.ppy.sh), [osu!collector](https://osucollector.com/) and [Piotrekol's Collection Manager](https://github.com/Piotrekol/CollectionManager). Any mention of products, services, or entities is for informational purposes only and does not constitute an endorsement.
## License

[GNU General Public License v3.0](https://choosealicense.com/licenses/gpl-3.0/)
