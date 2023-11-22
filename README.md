
# OsuCollectionDownloader

OsuCollectionDownloader is a simple command-line application for downloading collections from [osucollector](https://osucollector.com/) and optionally, using those collections to generate .osdb files which can be used by [Piotrekol's Collection Manager](https://github.com/Piotrekol/CollectionManager) for collection importing and sharing.

I strongly advise not use this application irresponsibly and instead support [osucollector](https://osucollector.com/client). 


## Installation

Make sure you have .NET 8 installed. If you don't have it, install it from [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Clone the repository
```bash
cd <dir>
git clone https://github.com/waylaa/OsuCollectionDownloader.git
```

Open the .sln file in your IDE that suports it, restore the nuget packages and you're done.
## Usage

On Windows 10/11, if you want to just download a collection.
```
.\OsuCollectionDownloader.exe --id=<osucollector collection id> --osu-songs-directory=<your 'Songs' folder in osu!> 
```

Optionally, if you want to generate a .osdb file as well.
```
.\OsuCollectionDownloader.exe --id=<osucollector collection id> --osu-songs-directory=<your 'Songs' folder in osu!> --osdb-file-directory=<A random directory>
```
\
On Linux, if you want to just download a collection.
```
.\OsuCollectionDownloader --id=<osucollector collection id> --osu-songs-directory=<your 'Songs' folder in osu!> 
```

Optionally, if you want to generate a .osdb file as well.
```
.\OsuCollectionDownloader --id=<osucollector collection id> --osu-songs-directory=<your 'Songs' folder in osu!> --osdb-file-directory=<A random directory>
```
