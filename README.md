# fsu
[![Coverage Status](https://coveralls.io/repos/github/Maxstupo/fsu/badge.svg?branch=master)](https://coveralls.io/github/Maxstupo/fsu?branch=develop)

### Example Usage
Print all files in C:\Pictures to console and text file:
```
C:\Pictures >> scan files >> print >> out "file_list.txt"
```


Print all images above 100mb in C:\Media to console with tabular output:
```
C:\Media >> scan files >> filter @size > 100mb & @mime >~ image >> transform "@{filepath}|@{size:mb}" >> print
```

Print filepaths to all videos within a directory with resolutions that are above average, sorted by duration:
```
C:\Movies >> scan files >> filter @mime >~ video >> avg @megapixels >> filter @megapixels > $avg_megapixels >> sort @duration desc >> print
```
