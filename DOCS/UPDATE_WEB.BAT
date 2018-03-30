echo getting SourceSafe code...
PATH = %PATH%;C:\Program Files\Microsoft Visual SourceSafe
SET SSDIR=\\VSS\team911_vss
SS GET $/_Misc/Testing/DOCS -I- -YSvan,vsm911 -R -GL\\192.168.0.1\bst\DOCS\
echo SourceSafe code recieving finishined.