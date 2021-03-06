# fsu
[![License](https://img.shields.io/github/license/Maxstupo/fsu)](https://github.com/Maxstupo/fsu/blob/master/LICENSE.md)
[![Latest Release](https://img.shields.io/github/v/release/Maxstupo/fsu?include_prereleases)](https://github.com/Maxstupo/fsu/releases/latest)
[![Build Status](https://ci.appveyor.com/api/projects/status/54p0js5x3wmj0ire?svg=true)](https://ci.appveyor.com/project/Maxstupo/fsu)
[![Coverage Status](https://coveralls.io/repos/github/Maxstupo/fsu/badge.svg?branch=master)](https://coveralls.io/github/Maxstupo/fsu?branch=master)

### Example Usage
Print all files in C:\Pictures to console and text file:
```
"C:\Pictures" >> scan files >> print >> out "file_list.txt"
```


Print all images above 100mb in C:\Media to console with tabular output:
```
"C:\Media" >> scan files >> filter @{size} > 100mb & @{mime} >~ "image" >> transform "@{filepath}|@{size:mb}" >> print
```

Print filepaths to all videos within a directory with resolutions that are above average, sorted by duration:
```
"C:\Movies" >> scan files >> filter @{mime} >~ "video" >> avg @{megapixels} >> filter @{megapixels} > ${avg_megapixels} >> sort @{duration} desc >> print
```
