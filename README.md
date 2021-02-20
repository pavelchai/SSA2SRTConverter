# SSA2SRTConverter

Converts the SubStationAlpha subtitles (.ssa, .ass) to the SubRip subtitles (.srt)

Targets:

1) SSA2SRT.Model - .Net Standard 1.3

2) SSA2SRT.Desktop.Console - .Net Core 2.1

3) SSA2SRT.Web - .Net 5

# How to use (Web)

Online demo: https://altphotonic.ru/SSA2SRT
REST-API: https://altphotonic.ru/api/SSA2SRTConverterService
Swagger UI: https://altphotonic.ru/swagger/index.html?urls.primaryName=SSA2SRTConverterService

1) Open link in browser
2) Drop your subtitles (*.ssa, *.ass) / zip files with the subtitles in form below or select files in file dialog
3) Click on individual upload button to convert specified subtitle / zip file with subtitles
4) Click on global upload button to convert all files (all converted files will be stored in converted_time_date.zip file)

# How to use (Desktop, Console)

1) Open console application
2) Select input directory with the .ssa/.ass subtitles or/and ZIP32/64 storages with these files
3) Select output directory

All .ssa/.ass files in the input directory will be converted in the .srt files and these files will be added in the output directory 

All .ssa/.ass files in .zip files will be converted in the .srt files, and converted.zip files will be added in the output directory 

# Building a solution

1) Install VS2019
2) Install node.js and npm
3) Run `npm install` (in SSA2SRT.Web project directory) - download required modules from information in `package.json`
4) Run `npm run dev` (in SSA2SRT.Web project directory) - build required bundles (with using WebPack, WebPack plugins and `webpack.config.js`)
5) Build solution
